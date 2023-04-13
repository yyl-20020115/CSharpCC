// Copyright 2011 Google Inc. All Rights Reserved.
// Author: sreeni@google.com (Sreeni Viswanadha)

/* Copyright (c) 2006, Sun Microsystems, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *     * Redistributions of source code must retain the above copyright notice,
 *       this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Sun Microsystems, Inc. nor the names of its
 *       contributors may be used to endorse or promote products derived from
 *       this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */

using org.javacc.jjtree;

namespace org.javacc.parser;

/**
 * Generate lexer. 
 */
public class LexGenCPP : LexGen //CodeGenerator implements JavaCCParserConstants
{
    //@Override
    void PrintClassHead()
    {
        int i, j;

        List<string> tn = new(ToolNames);
        tn.Add(ToolName);

        SwitchToStaticsFile();

        //standard includes
        SwitchToIncludeFile();
        GenCodeLine("#include \"stdio.h\"");
        GenCodeLine("#include \"JavaCC.h\"");
        GenCodeLine("#include \"CharStream.h\"");
        GenCodeLine("#include \"Token.h\"");
        GenCodeLine("#include \"ErrorHandler.h\"");
        GenCodeLine("#include \"TokenManager.h\"");
        GenCodeLine("#include \"" + CuName + "Constants.h\"");

        if (Options.stringValue(Options.USEROPTION__CPP_TOKEN_MANAGER_INCLUDE).Length > 0)
        {
            GenCodeLine("#include \"" + Options.stringValue(Options.USEROPTION__CPP_TOKEN_MANAGER_INCLUDE) + "\"\n");
        }

        GenCodeLine("");

        if (Options.stringValue(Options.USEROPTION__CPP_NAMESPACE).Length > 0)
        {
            GenCodeLine("namespace " + Options.stringValue("NAMESPACE_OPEN"));
        }

        GenCodeLine("class " + CuName + ";");

        int l = 0, kind;
        i = 1;
        /* namespace?
        for (;;)
        {
          if (cu_to_insertion_point_1.size() <= l)
            break;

          kind = ((Token)cu_to_insertion_point_1.get(l)).kind;
          if(kind == PACKAGE || kind == IMPORT) {
            for (; i < cu_to_insertion_point_1.size(); i++) {
              kind = ((Token)cu_to_insertion_point_1[i]).kind;
              if (kind == CLASS)
              {
                cline = ((Token)(cu_to_insertion_point_1.get(l))).beginLine;
                ccol = ((Token)(cu_to_insertion_point_1.get(l))).beginColumn;
                for (j = l; j < i; j++) {
                  printToken((Token)(cu_to_insertion_point_1[j]));
                }
                if (kind == SEMICOLON)
                  printToken((Token)(cu_to_insertion_point_1[j]));
                GenCodeLine("");
                break;
              }
            }
            l = ++i;
          }
          else
            break;
        }*/

        GenCodeLine("");
        GenCodeLine("/** Token Manager. */");
        string superClass = Options.stringValue(Options.USEROPTION__TOKEN_MANAGER_SUPER_CLASS);
        GenClassStart(null, tokMgrClassName, new String[] { }, new String[] { "public TokenManager" + (superClass == null ? "" : ", public " + superClass) });

        if (token_mgr_decls != null && token_mgr_decls.Count > 0)
        {
            Token t = (Token)token_mgr_decls[0];
            bool commonTokenActionSeen = false;
            bool commonTokenActionNeeded = Options.getCommonTokenAction();

            PrintTokenSetup((Token)token_mgr_decls[0]);
            ccol = 1;

            SwitchToMainFile();
            for (j = 0; j < token_mgr_decls.Count; j++)
            {
                t = (Token)token_mgr_decls[j];
                if (t.kind == IDENTIFIER &&
                    commonTokenActionNeeded &&
                    !commonTokenActionSeen)
                {
                    commonTokenActionSeen = t.image == ("CommonTokenAction");
                    if (commonTokenActionSeen)
                        t.image = CuName + "TokenManager::" + t.image;
                }

                PrintToken(t);
            }

            SwitchToIncludeFile();
            GenCodeLine("  void CommonTokenAction(Token* token);");

            if (Options.getTokenManagerUsesParser())
            {
                GenCodeLine("  void setParser(void* parser) {");
                GenCodeLine("      this->parser = (" + CuName + "*) parser;");
                GenCodeLine("  }");
            }
            GenCodeLine("");

            if (commonTokenActionNeeded && !commonTokenActionSeen)
                JavaCCErrors.Warning("You have the COMMON_TOKEN_ACTION option set. " +
                    "But it appears you have not defined the method :\n" +
                    "      " + staticString + "void CommonTokenAction(Token *t)\n" +
                "in your TOKEN_MGR_DECLS. The generated token manager will not compile.");

        }
        else if (Options.getCommonTokenAction())
        {
            JavaCCErrors.Warning("You have the COMMON_TOKEN_ACTION option set. " +
                "But you have not defined the method :\n" +
                "      " + staticString + "void CommonTokenAction(Token *t)\n" +
            "in your TOKEN_MGR_DECLS. The generated token manager will not compile.");
        }

        GenCodeLine("");
        GenCodeLine("  FILE *debugStream;");

        GenerateMethodDefHeader("  void ", tokMgrClassName, "setDebugStream(FILE *ds)");
        GenCodeLine("{ debugStream = ds; }");

        SwitchToIncludeFile();
        if (Options.getTokenManagerUsesParser())
        {
            GenCodeLine("");
            GenCodeLine("private:");
            GenCodeLine("  " + CuName + "* parser = nullptr;");
        }
        SwitchToMainFile();
    }

    void DumpDebugMethods()
    {
        writeTemplate("/templates/cpp/DumpDebugMethods.template",
              "maxOrdinal", maxOrdinal,
              "stateSetSize", stateSetSize);
    }

    static void BuildLexStatesTable()
    {
        Iterator<TokenProduction> it = rexprlist.iterator();
        TokenProduction tp;
        int i;

        String[] tmpLexStateName = new String[lexstate_I2S.Count];
        while (it.hasNext())
        {
            tp = it.next();
            List<RegExprSpec> respecs = tp.respecs;
            List<TokenProduction> tps;

            for (i = 0; i < tp.lexStates.Length; i++)
            {
                if ((tps = (List<TokenProduction>)allTpsForState.get(tp.lexStates[i])) == null)
                {
                    tmpLexStateName[maxLexStates++] = tp.lexStates[i];
                    allTpsForState.Add(tp.lexStates[i], tps = new());
                }

                tps.Add(tp);
            }

            if (respecs == null || respecs.Count == 0)
                continue;

            RegularExpression re;
            for (i = 0; i < respecs.Count; i++)
                if (maxOrdinal <= (re = ((RegExprSpec)respecs[i]).rexp).ordinal)
                    maxOrdinal = re.ordinal + 1;
        }

        kinds = new int[maxOrdinal];
        toSkip = new long[maxOrdinal / 64 + 1];
        toSpecial = new long[maxOrdinal / 64 + 1];
        toMore = new long[maxOrdinal / 64 + 1];
        toToken = new long[maxOrdinal / 64 + 1];
        toToken[0] = 1L;
        actions = new Action[maxOrdinal];
        actions[0] = actForEof;
        hasTokenActions = actForEof != null;
        initStates = new ();
        canMatchAnyChar = new int[maxLexStates];
        canLoop = new bool[maxLexStates];
        stateHasActions = new bool[maxLexStates];
        lexStateName = new String[maxLexStates];
        singlesToSkip = new NfaState[maxLexStates];
        Array.Copy(tmpLexStateName, 0, lexStateName, 0, maxLexStates);

        for (i = 0; i < maxLexStates; i++)
            canMatchAnyChar[i] = -1;

        hasNfa = new bool[maxLexStates];
        mixed = new bool[maxLexStates];
        maxLongsReqd = new int[maxLexStates];
        initMatch = new int[maxLexStates];
        newLexState = new String[maxOrdinal];
        newLexState[0] = nextStateForEof;
        hasEmptyMatch = false;
        lexStates = new int[maxOrdinal];
        ignoreCase = new bool[maxOrdinal];
        rexprs = new RegularExpression[maxOrdinal];
        RStringLiteral.allImages = new String[maxOrdinal];
        canReachOnMore = new bool[maxLexStates];
    }

    static int GetIndex(string name)
    {
        for (int i = 0; i < lexStateName.Length; i++)
            if (lexStateName[i] != null && lexStateName[i] == (name))
                return i;

        throw new Error(); // Should never come here
    }

    public static void AddCharToSkip(char c, int kind)
    {
        singlesToSkip[lexStateIndex].AddChar(c);
        singlesToSkip[lexStateIndex].kind = kind;
    }

    public new void Start()
    {
        if (!Options.getBuildTokenManager() ||
            Options.getUserTokenManager() ||
            JavaCCErrors.GetErrorCount() > 0)
            return;

        keepLineCol = Options.getKeepLineColumn();
        List<Expansion> choices = new();
        Enumeration e;
        TokenProduction tp;
        int i, j;

        staticString = (Options.getStatic() ? "static " : "");
        tokMgrClassName = CuName + "TokenManager";

        PrintClassHead();
        BuildLexStatesTable();

        e = allTpsForState.keys();

        bool ignoring = false;

        while (e.hasMoreElements())
        {
            NfaState.ReInit();
            RStringLiteral.ReInit();

            string key = (String)e.nextElement();

            lexStateIndex = GetIndex(key);
            lexStateSuffix = "_" + lexStateIndex;
            List<TokenProduction> allTps = (List<TokenProduction>)allTpsForState.get(key);
            initStates.Add(key, initialState = new NfaState());
            ignoring = false;

            singlesToSkip[lexStateIndex] = new NfaState();
            singlesToSkip[lexStateIndex].dummy = true;

            if (key == ("DEFAULT"))
                defaultLexState = lexStateIndex;

            for (i = 0; i < allTps.Count; i++)
            {
                tp = (TokenProduction)allTps[i];
                int kind = tp.kind;
                bool ignore = tp.ignoreCase;
                List<RegExprSpec> rexps = tp.respecs;

                if (i == 0)
                    ignoring = ignore;

                for (j = 0; j < rexps.Count; j++)
                {
                    RegExprSpec respec = (RegExprSpec)rexps[j];
                    curRE = respec.rexp;

                    rexprs[curKind = curRE.ordinal] = curRE;
                    lexStates[curRE.ordinal] = lexStateIndex;
                    ignoreCase[curRE.ordinal] = ignore;

                    if (curRE.private_rexp)
                    {
                        kinds[curRE.ordinal] = -1;
                        continue;
                    }

                    if (curRE is RStringLiteral &&
                        !((RStringLiteral)curRE).image == (""))
                    {
                        ((RStringLiteral)curRE).GenerateDfa(this, curRE.ordinal);
                        if (i != 0 && !mixed[lexStateIndex] && ignoring != ignore)
                            mixed[lexStateIndex] = true;
                    }
                    else if (curRE.CanMatchAnyChar())
                    {
                        if (canMatchAnyChar[lexStateIndex] == -1 ||
                            canMatchAnyChar[lexStateIndex] > curRE.ordinal)
                            canMatchAnyChar[lexStateIndex] = curRE.ordinal;
                    }
                    else
                    {
                        Nfa temp;

                        if (curRE is RChoice)
                            choices.Add(curRE);

                        temp = curRE.GenerateNfa(ignore);
                        temp.end.isFinal = true;
                        temp.end.kind = curRE.ordinal;
                        initialState.AddMove(temp.start);
                    }

                    if (kinds.Length < curRE.ordinal)
                    {
                        int[] tmp = new int[curRE.ordinal + 1];

                        Array.Copy(kinds, 0, tmp, 0, kinds.Length);
                        kinds = tmp;
                    }
                    //Console.WriteLine("   ordina : " + curRE.ordinal);

                    kinds[curRE.ordinal] = kind;

                    if (respec.nextState != null &&
                        !respec.nextState == (lexStateName[lexStateIndex]))
                        newLexState[curRE.ordinal] = respec.nextState;

                    if (respec.act != null && respec.act.GetActionTokens() != null &&
                        respec.act.GetActionTokens().Count > 0)
                        actions[curRE.ordinal] = respec.act;

                    switch (kind)
                    {
                        case TokenProduction.SPECIAL:
                            hasSkipActions |= (actions[curRE.ordinal] != null) ||
                            (newLexState[curRE.ordinal] != null);
                            hasSpecial = true;
                            toSpecial[curRE.ordinal / 64] |= 1L << (curRE.ordinal % 64);
                            toSkip[curRE.ordinal / 64] |= 1L << (curRE.ordinal % 64);
                            break;
                        case TokenProduction.SKIP:
                            hasSkipActions |= (actions[curRE.ordinal] != null);
                            hasSkip = true;
                            toSkip[curRE.ordinal / 64] |= 1L << (curRE.ordinal % 64);
                            break;
                        case TokenProduction.MORE:
                            hasMoreActions |= (actions[curRE.ordinal] != null);
                            hasMore = true;
                            toMore[curRE.ordinal / 64] |= 1L << (curRE.ordinal % 64);

                            if (newLexState[curRE.ordinal] != null)
                                canReachOnMore[GetIndex(newLexState[curRE.ordinal])] = true;
                            else
                                canReachOnMore[lexStateIndex] = true;

                            break;
                        case TokenProduction.TOKEN:
                            hasTokenActions |= (actions[curRE.ordinal] != null);
                            toToken[curRE.ordinal / 64] |= 1L << (curRE.ordinal % 64);
                            break;
                    }
                }
            }

            // Generate a static block for initializing the nfa transitions
            NfaState.ComputeClosures();

            for (i = 0; i < initialState.epsilonMoves.Count; i++)
                ((NfaState)initialState.epsilonMoves.elementAt(i)).GenerateCode();

            hasNfa[lexStateIndex] = (NfaState.generatedStates != 0);
            if (hasNfa[lexStateIndex])
            {
                initialState.GenerateCode();
                initialState.GenerateInitMoves(this);
            }

            if (initialState.kind != int.MaxValue && initialState.kind != 0)
            {
                if ((toSkip[initialState.kind / 64] & (1L << initialState.kind)) != 0L ||
                    (toSpecial[initialState.kind / 64] & (1L << initialState.kind)) != 0L)
                    hasSkipActions = true;
                else if ((toMore[initialState.kind / 64] & (1L << initialState.kind)) != 0L)
                    hasMoreActions = true;
                else
                    hasTokenActions = true;

                if (initMatch[lexStateIndex] == 0 ||
                    initMatch[lexStateIndex] > initialState.kind)
                {
                    initMatch[lexStateIndex] = initialState.kind;
                    hasEmptyMatch = true;
                }
            }
            else if (initMatch[lexStateIndex] == 0)
                initMatch[lexStateIndex] = int.MaxValue;

            RStringLiteral.FillSubString();

            if (hasNfa[lexStateIndex] && !mixed[lexStateIndex])
                RStringLiteral.GenerateNfaStartStates(this, initialState);

            RStringLiteral.DumpDfaCode(this);

            if (hasNfa[lexStateIndex])
                NfaState.DumpMoveNfa(this);

            if (stateSetSize < NfaState.generatedStates)
                stateSetSize = NfaState.generatedStates;
        }

        for (i = 0; i < choices.Count; i++)
            ((RChoice)choices[i]).CheckUnmatchability();

        NfaState.DumpStateSets(this);
        CheckEmptyStringMatch();
        NfaState.DumpNonAsciiMoveMethods(this);
        RStringLiteral.DumpStrLiteralImages(this);
        DumpFillToken();
        DumpGetNextToken();

        if (Options.getDebugTokenManager())
        {
            NfaState.DumpStatesForKind(this);
            DumpDebugMethods();
        }

        if (hasLoop)
        {
            SwitchToStaticsFile();
            GenCodeLine("static int  jjemptyLineNo[" + maxLexStates + "];");
            GenCodeLine("static int  jjemptyColNo[" + maxLexStates + "];");
            GenCodeLine("static bool jjbeenHere[" + maxLexStates + "];");
            SwitchToMainFile();
        }

        if (hasSkipActions)
            DumpSkipActions();
        if (hasMoreActions)
            DumpMoreActions();
        if (hasTokenActions)
            DumpTokenActions();

        NfaState.PrintBoilerPlateCPP(this);

        string charStreamName;
        if (Options.getUserCharStream())
            charStreamName = "CharStream";
        else
        {
            if (Options.getJavaUnicodeEscape())
                charStreamName = "JavaCharStream";
            else
                charStreamName = "SimpleCharStream";
        }

        writeTemplate("/templates/cpp/TokenManagerBoilerPlateMethods.template",
          "charStreamName", "CharStream",
          "parserClassName", CuName,
          "defaultLexState", "defaultLexState",
          "lexStateNameLength", lexStateName.Length);

        dumpBoilerPlateInHeader();

        // in the include file close the class signature
        DumpStaticVarDeclarations(); // static vars actually inst

        SwitchToIncludeFile(); // remaining variables
        writeTemplate("/templates/cpp/DumpVarDeclarations.template",
          "charStreamName", "CharStream",
          "lexStateNameLength", lexStateName.Length);
        GenCodeLine(/*{*/ "};");

        SwitchToStaticsFile();
        // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
        string fileName = Options.getOutputDirectory() + File.separator +
                          tokMgrClassName +
                          GetFileExtension(Options.getOutputLanguage());
        SaveOutput(fileName);
    }

    private void dumpBoilerPlateInHeader()
    {
        SwitchToIncludeFile();
        GenCodeLine("#ifndef JAVACC_CHARSTREAM");
        GenCodeLine("#define JAVACC_CHARSTREAM CharStream");
        GenCodeLine("#endif");
        GenCodeLine("");

        GenCodeLine("private:");
        GenCodeLine("  void ReInitRounds();");
        GenCodeLine("");
        GenCodeLine("public:");
        GenCodeLine("  " + tokMgrClassName + "(JAVACC_CHARSTREAM *stream, int lexState = " + defaultLexState + ");");
        GenCodeLine("  virtual ~" + tokMgrClassName + "();");
        GenCodeLine("  void ReInit(JAVACC_CHARSTREAM *stream, int lexState = " + defaultLexState + ");");
        GenCodeLine("  void SwitchTo(int lexState);");
        GenCodeLine("  void clear();");
        GenCodeLine("  const JJSimpleString jjKindsForBitVector(int i, " + Options.getLongType() + " vec);");
        GenCodeLine("  const JJSimpleString jjKindsForStateVector(int lexState, int vec[], int start, int end);");
        GenCodeLine("");
    }

    private void DumpStaticVarDeclarations()
    {
        int i;

        SwitchToStaticsFile(); // remaining variables
        GenCodeLine("");
        GenCodeLine("/** Lexer state names. */");
        GenStringLiteralArrayCPP("lexStateNames", lexStateName);

        if (maxLexStates > 1)
        {
            GenCodeLine("");
            GenCodeLine("/** Lex State array. */");
            GenCode("static const int jjnewLexState[] = {");

            for (i = 0; i < maxOrdinal; i++)
            {
                if (i % 25 == 0)
                    GenCode("\n   ");

                if (newLexState[i] == null)
                    GenCode("-1, ");
                else
                    GenCode(GetIndex(newLexState[i]) + ", ");
            }
            GenCodeLine("\n};");
        }

        if (hasSkip || hasMore || hasSpecial)
        {
            // Bit vector for TOKEN
            GenCode("static const " + Options.getLongType() + " jjtoToken[] = {");
            for (i = 0; i < maxOrdinal / 64 + 1; i++)
            {
                if (i % 4 == 0)
                    GenCode("\n   ");
                GenCode("0x" + Long.toHexString(toToken[i]) + "L, ");
            }
            GenCodeLine("\n};");
        }

        if (hasSkip || hasSpecial)
        {
            // Bit vector for SKIP
            GenCode("static const " + Options.getLongType() + " jjtoSkip[] = {");
            for (i = 0; i < maxOrdinal / 64 + 1; i++)
            {
                if (i % 4 == 0)
                    GenCode("\n   ");
                GenCode("0x" + Long.toHexString(toSkip[i]) + "L, ");
            }
            GenCodeLine("\n};");
        }

        if (hasSpecial)
        {
            // Bit vector for SPECIAL
            GenCode("static const " + Options.getLongType() + " jjtoSpecial[] = {");
            for (i = 0; i < maxOrdinal / 64 + 1; i++)
            {
                if (i % 4 == 0)
                    GenCode("\n   ");
                GenCode("0x" + Long.toHexString(toSpecial[i]) + "L, ");
            }
            GenCodeLine("\n};");
        }

        /*if (hasMore) // Not needed as we just use else
        {
          // Bit vector for MORE
          GenCode("static const " + Options.getLongType() + " jjtoMore[] = {");
          for (i = 0; i < maxOrdinal / 64 + 1; i++)
          {
            if (i % 4 == 0)
              GenCode("\n   ");
            GenCode("0x" + Long.toHexString(toMore[i]) + "L, ");
          }
          GenCodeLine("\n};");
        }*/
    }

    void DumpFillToken()
    {
        double tokenVersion = JavaFiles.GetVersion("Token.java");
        bool hasBinaryNewToken = tokenVersion > 4.09;

        GenerateMethodDefHeader("Token *", tokMgrClassName, "jjFillToken()");
        GenCodeLine("{");
        GenCodeLine("   Token *t;");
        GenCodeLine("   JJString curTokenImage;");
        if (keepLineCol)
        {
            GenCodeLine("   int beginLine   = -1;");
            GenCodeLine("   int endLine     = -1;");
            GenCodeLine("   int beginColumn = -1;");
            GenCodeLine("   int endColumn   = -1;");
        }

        if (hasEmptyMatch)
        {
            GenCodeLine("   if (jjmatchedPos < 0)");
            GenCodeLine("   {");
            GenCodeLine("       curTokenImage = image.c_str();");

            if (keepLineCol)
            {
                GenCodeLine("   if (input_stream->getTrackLineColumn()) {");
                GenCodeLine("      beginLine = endLine = input_stream->getEndLine();");
                GenCodeLine("      beginColumn = endColumn = input_stream->getEndColumn();");
                GenCodeLine("   }");
            }

            GenCodeLine("   }");
            GenCodeLine("   else");
            GenCodeLine("   {");
            GenCodeLine("      JJString im = jjstrLiteralImages[jjmatchedKind];");
            GenCodeLine("      curTokenImage = (im.length() == 0) ? input_stream->GetImage() : im;");

            if (keepLineCol)
            {
                GenCodeLine("   if (input_stream->getTrackLineColumn()) {");
                GenCodeLine("      beginLine = input_stream->getBeginLine();");
                GenCodeLine("      beginColumn = input_stream->getBeginColumn();");
                GenCodeLine("      endLine = input_stream->getEndLine();");
                GenCodeLine("      endColumn = input_stream->getEndColumn();");
                GenCodeLine("   }");
            }

            GenCodeLine("   }");
        }
        else
        {
            GenCodeLine("   JJString im = jjstrLiteralImages[jjmatchedKind];");
            GenCodeLine("   curTokenImage = (im.length() == 0) ? input_stream->GetImage() : im;");
            if (keepLineCol)
            {
                GenCodeLine("   if (input_stream->getTrackLineColumn()) {");
                GenCodeLine("     beginLine = input_stream->getBeginLine();");
                GenCodeLine("     beginColumn = input_stream->getBeginColumn();");
                GenCodeLine("     endLine = input_stream->getEndLine();");
                GenCodeLine("     endColumn = input_stream->getEndColumn();");
                GenCodeLine("   }");
            }
        }

        if (Options.getTokenFactory().Length > 0)
        {
            GenCodeLine("   t = " + GetClassQualifier(Options.getTokenFactory()) + "newToken(jjmatchedKind, curTokenImage);");
        }
        else if (hasBinaryNewToken)
        {
            GenCodeLine("   t = " + GetClassQualifier("Token") + "newToken(jjmatchedKind, curTokenImage);");
        }
        else
        {
            GenCodeLine("   t = " + GetClassQualifier("Token") + "newToken(jjmatchedKind);");
            GenCodeLine("   t->kind = jjmatchedKind;");
            GenCodeLine("   t->image = curTokenImage;");
        }
        GenCodeLine("   t->specialToken = nullptr;");
        GenCodeLine("   t->next = nullptr;");

        if (keepLineCol)
        {
            GenCodeLine("");
            GenCodeLine("   if (input_stream->getTrackLineColumn()) {");
            GenCodeLine("   t->beginLine = beginLine;");
            GenCodeLine("   t->endLine = endLine;");
            GenCodeLine("   t->beginColumn = beginColumn;");
            GenCodeLine("   t->endColumn = endColumn;");
            GenCodeLine("   }");
        }

        GenCodeLine("");
        GenCodeLine("   return t;");
        GenCodeLine("}");
    }

    void DumpGetNextToken()
    {
        int i;

        SwitchToIncludeFile();
        GenCodeLine("");
        GenCodeLine("public:");
        GenCodeLine("    int curLexState = 0;");
        GenCodeLine("    int jjnewStateCnt = 0;");
        GenCodeLine("    int jjround = 0;");
        GenCodeLine("    int jjmatchedPos = 0;");
        GenCodeLine("    int jjmatchedKind = 0;");
        GenCodeLine("");
        SwitchToMainFile();
        GenCodeLine("const int defaultLexState = " + defaultLexState + ";");
        GenCodeLine("/** Get the next Token. */");
        GenerateMethodDefHeader("Token *", tokMgrClassName, "getNextToken()");
        GenCodeLine("{");
        if (hasSpecial)
        {
            GenCodeLine("  Token *specialToken = nullptr;");
        }
        GenCodeLine("  Token *matchedToken = nullptr;");
        GenCodeLine("  int curPos = 0;");
        GenCodeLine("");
        GenCodeLine("  for (;;)");
        GenCodeLine("  {");
        GenCodeLine("   EOFLoop: ");
        //GenCodeLine("   {");
        //GenCodeLine("      curChar = input_stream->BeginToken();");
        //GenCodeLine("   }");
        GenCodeLine("   if (input_stream->endOfInput())");
        GenCodeLine("   {");
        //GenCodeLine("     input_stream->backup(1);");

        if (Options.getDebugTokenManager())
            GenCodeLine("      fprintf(debugStream, \"Returning the <EOF> token.\\n\");");

        GenCodeLine("      jjmatchedKind = 0;");
        GenCodeLine("      jjmatchedPos = -1;");
        GenCodeLine("      matchedToken = jjFillToken();");

        if (hasSpecial)
            GenCodeLine("      matchedToken->specialToken = specialToken;");

        if (nextStateForEof != null || actForEof != null)
            GenCodeLine("      TokenLexicalActions(matchedToken);");

        if (Options.getCommonTokenAction())
            GenCodeLine("      CommonTokenAction(matchedToken);");

        GenCodeLine("      return matchedToken;");
        GenCodeLine("   }");
        GenCodeLine("   curChar = input_stream->BeginToken();");

        if (hasMoreActions || hasSkipActions || hasTokenActions)
        {
            GenCodeLine("   image = jjimage;");
            GenCodeLine("   image.clear();");
            GenCodeLine("   jjimageLen = 0;");
        }

        GenCodeLine("");

        string prefix = "";
        if (hasMore)
        {
            GenCodeLine("   for (;;)");
            GenCodeLine("   {");
            prefix = "  ";
        }

        string endSwitch = "";
        string caseStr = "";
        // this also sets up the start state of the nfa
        if (maxLexStates > 1)
        {
            GenCodeLine(prefix + "   switch(curLexState)");
            GenCodeLine(prefix + "   {");
            endSwitch = prefix + "   }";
            caseStr = prefix + "     case ";
            prefix += "    ";
        }

        prefix += "   ";
        for (i = 0; i < maxLexStates; i++)
        {
            if (maxLexStates > 1)
                GenCodeLine(caseStr + i + ":");

            if (singlesToSkip[i].HasTransitions())
            {
                // added the backup(0) to make JIT happy
                GenCodeLine(prefix + "{ input_stream->backup(0);");
                if (singlesToSkip[i].asciiMoves[0] != 0L &&
                    singlesToSkip[i].asciiMoves[1] != 0L)
                {
                    GenCodeLine(prefix + "   while ((curChar < 64" + " && (0x" +
                        Long.toHexString(singlesToSkip[i].asciiMoves[0]) +
                        "L & (1L << curChar)) != 0L) || \n" +
                        prefix + "          (curChar >> 6) == 1" +
                        " && (0x" +
                        Long.toHexString(singlesToSkip[i].asciiMoves[1]) +
                    "L & (1L << (curChar & 077))) != 0L)");
                }
                else if (singlesToSkip[i].asciiMoves[1] == 0L)
                {
                    GenCodeLine(prefix + "   while (curChar <= " +
                        (int)MaxChar(singlesToSkip[i].asciiMoves[0]) + " && (0x" +
                        Long.toHexString(singlesToSkip[i].asciiMoves[0]) +
                    "L & (1L << curChar)) != 0L)");
                }
                else if (singlesToSkip[i].asciiMoves[0] == 0L)
                {
                    GenCodeLine(prefix + "   while (curChar > 63 && curChar <= " +
                        ((int)MaxChar(singlesToSkip[i].asciiMoves[1]) + 64) +
                        " && (0x" +
                        Long.toHexString(singlesToSkip[i].asciiMoves[1]) +
                    "L & (1L << (curChar & 077))) != 0L)");
                }

                GenCodeLine(prefix + "{");
                if (Options.getDebugTokenManager())
                {
                    if (maxLexStates > 1)
                    {
                        GenCodeLine("      fprintf(debugStream, \"<%s>\" , addUnicodeEscapes(lexStateNames[curLexState]).c_str());");
                    }

                    GenCodeLine("      fprintf(debugStream, \"Skipping character : %c(%d)\\n\", curChar, (int)curChar);");
                }

                GenCodeLine(prefix + "if (input_stream->endOfInput()) { goto EOFLoop; }");
                GenCodeLine(prefix + "curChar = input_stream->BeginToken();");
                GenCodeLine(prefix + "}");
                GenCodeLine(prefix + "}");
            }

            if (initMatch[i] != int.MaxValue && initMatch[i] != 0)
            {
                if (Options.getDebugTokenManager())
                    GenCodeLine("      fprintf(debugStream, \"   Matched the empty string as %s token.\\n\", addUnicodeEscapes(tokenImage[" + initMatch[i] + "]).c_str());");

                GenCodeLine(prefix + "jjmatchedKind = " + initMatch[i] + ";");
                GenCodeLine(prefix + "jjmatchedPos = -1;");
                GenCodeLine(prefix + "curPos = 0;");
            }
            else
            {
                GenCodeLine(prefix + "jjmatchedKind = 0x" + Convert.ToString(int.MaxValue, 16) + ";");
                GenCodeLine(prefix + "jjmatchedPos = 0;");
            }

            if (Options.getDebugTokenManager())
            {
                GenCodeLine("   fprintf(debugStream, " +
                  "\"<%s>Current character : %c(%d) at line %d column %d\\n\"," +
                  "addUnicodeEscapes(lexStateNames[curLexState]).c_str(), curChar, (int)curChar, " +
                  "input_stream->getEndLine(), input_stream->getEndColumn());");
            }

            GenCodeLine(prefix + "curPos = jjMoveStringLiteralDfa0_" + i + "();");

            if (canMatchAnyChar[i] != -1)
            {
                if (initMatch[i] != int.MaxValue && initMatch[i] != 0)
                    GenCodeLine(prefix + "if (jjmatchedPos < 0 || (jjmatchedPos == 0 && jjmatchedKind > " +
                        canMatchAnyChar[i] + "))");
                else
                    GenCodeLine(prefix + "if (jjmatchedPos == 0 && jjmatchedKind > " +
                        canMatchAnyChar[i] + ")");
                GenCodeLine(prefix + "{");

                if (Options.getDebugTokenManager())
                {
                    GenCodeLine("           fprintf(debugStream, \"   Current character matched as a %s token.\\n\", addUnicodeEscapes(tokenImage[" + canMatchAnyChar[i] + "]).c_str());");
                }
                GenCodeLine(prefix + "   jjmatchedKind = " + canMatchAnyChar[i] + ";");

                if (initMatch[i] != int.MaxValue && initMatch[i] != 0)
                    GenCodeLine(prefix + "   jjmatchedPos = 0;");

                GenCodeLine(prefix + "}");
            }

            if (maxLexStates > 1)
                GenCodeLine(prefix + "break;");
        }

        if (maxLexStates > 1)
            GenCodeLine(endSwitch);
        else if (maxLexStates == 0)
            GenCodeLine("       jjmatchedKind = 0x" + Convert.ToString(int.MaxValue, 16) + ";");

        if (maxLexStates > 1)
            prefix = "  ";
        else
            prefix = "";

        if (maxLexStates > 0)
        {
            GenCodeLine(prefix + "   if (jjmatchedKind != 0x" + Convert.ToString(int.MaxValue, 16) + ")");
            GenCodeLine(prefix + "   {");
            GenCodeLine(prefix + "      if (jjmatchedPos + 1 < curPos)");

            if (Options.getDebugTokenManager())
            {
                GenCodeLine(prefix + "      {");
                GenCodeLine(prefix + "         fprintf(debugStream, " +
                "\"   Putting back %d characters into the input stream.\\n\", (curPos - jjmatchedPos - 1));");
            }

            GenCodeLine(prefix + "         input_stream->backup(curPos - jjmatchedPos - 1);");

            if (Options.getDebugTokenManager())
            {
                GenCodeLine(prefix + "      }");
            }

            if (Options.getDebugTokenManager())
            {
                GenCodeLine("    fprintf(debugStream, " +
                    "\"****** FOUND A %d(%s) MATCH (%s) ******\\n\", jjmatchedKind, addUnicodeEscapes(tokenImage[jjmatchedKind]).c_str(), addUnicodeEscapes(input_stream->GetSuffix(jjmatchedPos + 1)).c_str());");
            }

            if (hasSkip || hasMore || hasSpecial)
            {
                GenCodeLine(prefix + "      if ((jjtoToken[jjmatchedKind >> 6] & " +
                "(1L << (jjmatchedKind & 077))) != 0L)");
                GenCodeLine(prefix + "      {");
            }

            GenCodeLine(prefix + "         matchedToken = jjFillToken();");

            if (hasSpecial)
                GenCodeLine(prefix + "         matchedToken->specialToken = specialToken;");

            if (hasTokenActions)
                GenCodeLine(prefix + "         TokenLexicalActions(matchedToken);");

            if (maxLexStates > 1)
            {
                GenCodeLine("       if (jjnewLexState[jjmatchedKind] != -1)");
                GenCodeLine(prefix + "       curLexState = jjnewLexState[jjmatchedKind];");
            }

            if (Options.getCommonTokenAction())
                GenCodeLine(prefix + "         CommonTokenAction(matchedToken);");

            GenCodeLine(prefix + "         return matchedToken;");

            if (hasSkip || hasMore || hasSpecial)
            {
                GenCodeLine(prefix + "      }");

                if (hasSkip || hasSpecial)
                {
                    if (hasMore)
                    {
                        GenCodeLine(prefix + "      else if ((jjtoSkip[jjmatchedKind >> 6] & " +
                        "(1L << (jjmatchedKind & 077))) != 0L)");
                    }
                    else
                        GenCodeLine(prefix + "      else");

                    GenCodeLine(prefix + "      {");

                    if (hasSpecial)
                    {
                        GenCodeLine(prefix + "         if ((jjtoSpecial[jjmatchedKind >> 6] & " +
                        "(1L << (jjmatchedKind & 077))) != 0L)");
                        GenCodeLine(prefix + "         {");

                        GenCodeLine(prefix + "            matchedToken = jjFillToken();");

                        GenCodeLine(prefix + "            if (specialToken == nullptr)");
                        GenCodeLine(prefix + "               specialToken = matchedToken;");
                        GenCodeLine(prefix + "            else");
                        GenCodeLine(prefix + "            {");
                        GenCodeLine(prefix + "               matchedToken->specialToken = specialToken;");
                        GenCodeLine(prefix + "               specialToken = (specialToken->next = matchedToken);");
                        GenCodeLine(prefix + "            }");

                        if (hasSkipActions)
                            GenCodeLine(prefix + "            SkipLexicalActions(matchedToken);");

                        GenCodeLine(prefix + "         }");

                        if (hasSkipActions)
                        {
                            GenCodeLine(prefix + "         else");
                            GenCodeLine(prefix + "            SkipLexicalActions(nullptr);");
                        }
                    }
                    else if (hasSkipActions)
                        GenCodeLine(prefix + "         SkipLexicalActions(nullptr);");

                    if (maxLexStates > 1)
                    {
                        GenCodeLine("         if (jjnewLexState[jjmatchedKind] != -1)");
                        GenCodeLine(prefix + "         curLexState = jjnewLexState[jjmatchedKind];");
                    }

                    GenCodeLine(prefix + "         goto EOFLoop;");
                    GenCodeLine(prefix + "      }");
                }

                if (hasMore)
                {
                    if (hasMoreActions)
                        GenCodeLine(prefix + "      MoreLexicalActions();");
                    else if (hasSkipActions || hasTokenActions)
                        GenCodeLine(prefix + "      jjimageLen += jjmatchedPos + 1;");

                    if (maxLexStates > 1)
                    {
                        GenCodeLine("      if (jjnewLexState[jjmatchedKind] != -1)");
                        GenCodeLine(prefix + "      curLexState = jjnewLexState[jjmatchedKind];");
                    }
                    GenCodeLine(prefix + "      curPos = 0;");
                    GenCodeLine(prefix + "      jjmatchedKind = 0x" + Convert.ToString(int.MaxValue,16) + ";");

                    GenCodeLine(prefix + "   if (!input_stream->endOfInput()) {");
                    GenCodeLine(prefix + "         curChar = input_stream->readChar();");

                    if (Options.getDebugTokenManager())
                    {
                        GenCodeLine("   fprintf(debugStream, " +
                         "\"<%s>Current character : %c(%d) at line %d column %d\\n\"," +
                         "addUnicodeEscapes(lexStateNames[curLexState]).c_str(), curChar, (int)curChar, " +
                         "input_stream->getEndLine(), input_stream->getEndColumn());");
                    }
                    GenCodeLine(prefix + "   continue;");
                    GenCodeLine(prefix + " }");
                }
            }

            GenCodeLine(prefix + "   }");
            GenCodeLine(prefix + "   int error_line = input_stream->getEndLine();");
            GenCodeLine(prefix + "   int error_column = input_stream->getEndColumn();");
            GenCodeLine(prefix + "   JJString error_after;");
            GenCodeLine(prefix + "   bool EOFSeen = false;");
            GenCodeLine(prefix + "   if (input_stream->endOfInput()) {");
            GenCodeLine(prefix + "      EOFSeen = true;");
            GenCodeLine(prefix + "      error_after = curPos <= 1 ? EMPTY : input_stream->GetImage();");
            GenCodeLine(prefix + "      if (curChar == '\\n' || curChar == '\\r') {");
            GenCodeLine(prefix + "         error_line++;");
            GenCodeLine(prefix + "         error_column = 0;");
            GenCodeLine(prefix + "      }");
            GenCodeLine(prefix + "      else");
            GenCodeLine(prefix + "         error_column++;");
            GenCodeLine(prefix + "   }");
            GenCodeLine(prefix + "   if (!EOFSeen) {");
            GenCodeLine(prefix + "      error_after = curPos <= 1 ? EMPTY : input_stream->GetImage();");
            GenCodeLine(prefix + "   }");
            GenCodeLine(prefix + "   errorHandler->lexicalError(EOFSeen, curLexState, error_line, error_column, error_after, curChar, this);");
        }

        if (hasMore)
            GenCodeLine(prefix + " }");

        GenCodeLine("  }");
        GenCodeLine("}");
        GenCodeLine("");
    }

    public void DumpSkipActions()
    {
        Action act;

        GenerateMethodDefHeader("void ", tokMgrClassName, "SkipLexicalActions(Token *matchedToken)");
        GenCodeLine("{");
        GenCodeLine("   switch(jjmatchedKind)");
        GenCodeLine("   {");

    Outer:
        for (int i = 0; i < maxOrdinal; i++)
        {
            if ((toSkip[i / 64] & (1L << (i % 64))) == 0L)
                continue;

            for (; ; )
            {
                if (((act = (Action)actions[i]) == null ||
                    act.GetActionTokens() == null ||
                    act.GetActionTokens().Count == 0) && !canLoop[lexStates[i]])
                    goto OuterExit;

                GenCodeLine("      case " + i + " : {");

                if (initMatch[lexStates[i]] == i && canLoop[lexStates[i]])
                {
                    GenCodeLine("         if (jjmatchedPos == -1)");
                    GenCodeLine("         {");
                    GenCodeLine("            if (jjbeenHere[" + lexStates[i] + "] &&");
                    GenCodeLine("                jjemptyLineNo[" + lexStates[i] + "] == input_stream->getBeginLine() &&");
                    GenCodeLine("                jjemptyColNo[" + lexStates[i] + "] == input_stream->getBeginColumn())");
                    GenCodeLine("               errorHandler->lexicalError(JJString(\"(\"Error: Bailing out of infinite loop caused by repeated empty string matches \" + \"at line \" + input_stream->getBeginLine() + \", \" + \"column \" + input_stream->getBeginColumn() + \".\")), this);");
                    GenCodeLine("            jjemptyLineNo[" + lexStates[i] + "] = input_stream->getBeginLine();");
                    GenCodeLine("            jjemptyColNo[" + lexStates[i] + "] = input_stream->getBeginColumn();");
                    GenCodeLine("            jjbeenHere[" + lexStates[i] + "] = true;");
                    GenCodeLine("         }");
                }

                if ((act = (Action)actions[i]) == null ||
                    act.GetActionTokens().Count == 0)
                    break;

                GenCode("         image.append");
                if (RStringLiteral.allImages[i] != null)
                {
                    GenCodeLine("(jjstrLiteralImages[" + i + "]);");
                    GenCodeLine("        lengthOfMatch = jjstrLiteralImages[" + i + "].length();");
                }
                else
                {
                    GenCodeLine("(input_stream->GetSuffix(jjimageLen + (lengthOfMatch = jjmatchedPos + 1)));");
                }

                PrintTokenSetup((Token)act.GetActionTokens()[0]);
                ccol = 1;

                for (int j = 0; j < act.GetActionTokens().Count; j++)
                    PrintToken((Token)act.GetActionTokens()[j]);
                GenCodeLine("");

                break;
            }

            GenCodeLine("         break;");
            GenCodeLine("       }");
        OuterExit:
            ;
        }

        GenCodeLine("      default :");
        GenCodeLine("         break;");
        GenCodeLine("   }");
        GenCodeLine("}");
    }

    public void DumpMoreActions()
    {
        Action act;

        GenerateMethodDefHeader("void ", tokMgrClassName, "MoreLexicalActions()");
        GenCodeLine("{");
        GenCodeLine("   jjimageLen += (lengthOfMatch = jjmatchedPos + 1);");
        GenCodeLine("   switch(jjmatchedKind)");
        GenCodeLine("   {");

    Outer:
        for (int i = 0; i < maxOrdinal; i++)
        {
            if ((toMore[i / 64] & (1L << (i % 64))) == 0L)
                continue;

            for (; ; )
            {
                if (((act = (Action)actions[i]) == null ||
                    act.GetActionTokens() == null ||
                    act.GetActionTokens().Count == 0) && !canLoop[lexStates[i]])
                    goto OuterExit;

                GenCodeLine("      case " + i + " : {");

                if (initMatch[lexStates[i]] == i && canLoop[lexStates[i]])
                {
                    GenCodeLine("         if (jjmatchedPos == -1)");
                    GenCodeLine("         {");
                    GenCodeLine("            if (jjbeenHere[" + lexStates[i] + "] &&");
                    GenCodeLine("                jjemptyLineNo[" + lexStates[i] + "] == input_stream->getBeginLine() &&");
                    GenCodeLine("                jjemptyColNo[" + lexStates[i] + "] == input_stream->getBeginColumn())");
                    GenCodeLine("               errorHandler->lexicalError(JJString(\"(\"Error: Bailing out of infinite loop caused by repeated empty string matches \" + \"at line \" + input_stream->getBeginLine() + \", \" + \"column \" + input_stream->getBeginColumn() + \".\")), this);");
                    GenCodeLine("            jjemptyLineNo[" + lexStates[i] + "] = input_stream->getBeginLine();");
                    GenCodeLine("            jjemptyColNo[" + lexStates[i] + "] = input_stream->getBeginColumn();");
                    GenCodeLine("            jjbeenHere[" + lexStates[i] + "] = true;");
                    GenCodeLine("         }");
                }

                if ((act = (Action)actions[i]) == null ||
                    act.GetActionTokens().Count == 0)
                {
                    break;
                }

                GenCode("         image.append");

                if (RStringLiteral.allImages[i] != null)
                    GenCodeLine("(jjstrLiteralImages[" + i + "]);");
                else
                    GenCodeLine("(input_stream->GetSuffix(jjimageLen));");

                GenCodeLine("         jjimageLen = 0;");
                PrintTokenSetup((Token)act.GetActionTokens()[0]);
                ccol = 1;

                for (int j = 0; j < act.GetActionTokens().Count; j++)
                    PrintToken((Token)act.GetActionTokens()[j]);
                GenCodeLine("");

                break;
            }

            GenCodeLine("         break;");
            GenCodeLine("       }");
        OuterExit:
            ;
        }

        GenCodeLine("      default :");
        GenCodeLine("         break;");

        GenCodeLine("   }");
        GenCodeLine("}");
    }

    public new void DumpTokenActions()
    {
        Action act;
        int i;

        GenerateMethodDefHeader("void ", tokMgrClassName, "TokenLexicalActions(Token *matchedToken)");
        GenCodeLine("{");
        GenCodeLine("   switch(jjmatchedKind)");
        GenCodeLine("   {");

    Outer:
        for (i = 0; i < maxOrdinal; i++)
        {
            if ((toToken[i / 64] & (1L << (i % 64))) == 0L)
                continue;

            for (; ; )
            {
                if (((act = (Action)actions[i]) == null ||
                    act.GetActionTokens() == null ||
                    act.GetActionTokens().Count == 0) && !canLoop[lexStates[i]])
                    goto OuterExit;

                GenCodeLine("      case " + i + " : {");

                if (initMatch[lexStates[i]] == i && canLoop[lexStates[i]])
                {
                    GenCodeLine("         if (jjmatchedPos == -1)");
                    GenCodeLine("         {");
                    GenCodeLine("            if (jjbeenHere[" + lexStates[i] + "] &&");
                    GenCodeLine("                jjemptyLineNo[" + lexStates[i] + "] == input_stream->getBeginLine() &&");
                    GenCodeLine("                jjemptyColNo[" + lexStates[i] + "] == input_stream->getBeginColumn())");
                    GenCodeLine("               errorHandler->lexicalError(JJString(\"Error: Bailing out of infinite loop caused by repeated empty string matches " + "at line \" + input_stream->getBeginLine() + \", " + "column \" + input_stream->getBeginColumn() + \".\"), this);");
                    GenCodeLine("            jjemptyLineNo[" + lexStates[i] + "] = input_stream->getBeginLine();");
                    GenCodeLine("            jjemptyColNo[" + lexStates[i] + "] = input_stream->getBeginColumn();");
                    GenCodeLine("            jjbeenHere[" + lexStates[i] + "] = true;");
                    GenCodeLine("         }");
                }

                if ((act = (Action)actions[i]) == null ||
                    act.GetActionTokens().Count == 0)
                    break;

                if (i == 0)
                {
                    GenCodeLine("      image.setLength(0);"); // For EOF no image is there
                }
                else
                {
                    GenCode("        image.append");

                    if (RStringLiteral.allImages[i] != null)
                    {
                        GenCodeLine("(jjstrLiteralImages[" + i + "]);");
                        GenCodeLine("        lengthOfMatch = jjstrLiteralImages[" + i + "].length();");
                    }
                    else
                    {
                        GenCodeLine("(input_stream->GetSuffix(jjimageLen + (lengthOfMatch = jjmatchedPos + 1)));");
                    }
                }

                PrintTokenSetup((Token)act.GetActionTokens()[0]);
                ccol = 1;

                for (int j = 0; j < act.GetActionTokens().Count; j++)
                    PrintToken((Token)act.GetActionTokens()[j]);
                GenCodeLine("");

                break;
            }

            GenCodeLine("         break;");
            GenCodeLine("       }");
        OuterExit:
            ;
        }

        GenCodeLine("      default :");
        GenCodeLine("         break;");
        GenCodeLine("   }");
        GenCodeLine("}");
    }
}
