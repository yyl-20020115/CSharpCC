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

using CSharpCC.CCTree;

namespace CSharpCC.Parser;

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
        GenCodeLine("#include \"" + cu_name + "Constants.h\"");

        if (Options.StringValue(Options.USEROPTION__CPP_TOKEN_MANAGER_INCLUDE).Length > 0)
        {
            GenCodeLine("#include \"" + Options.StringValue(Options.USEROPTION__CPP_TOKEN_MANAGER_INCLUDE) + "\"\n");
        }

        GenCodeLine("");

        if (Options.StringValue(Options.USEROPTION__CPP_NAMESPACE).Length > 0)
        {
            GenCodeLine("namespace " + Options.StringValue("NAMESPACE_OPEN"));
        }

        GenCodeLine("class " + cu_name + ";");

        int l = 0, kind;
        i = 1;
        /* namespace?
        for (;;)
        {
          if (cu_to_insertion_point_1.size() <= l)
            break;

          kind = ((Token)cu_to_insertion_point_1[l]).kind;
          if(kind == PACKAGE || kind == IMPORT) {
            for (; i < cu_to_insertion_point_1.size(); i++) {
              kind = ((Token)cu_to_insertion_point_1[i]).kind;
              if (kind == CLASS) 
              { 
                cline = ((Token)(cu_to_insertion_point_1[l])).beginLine;
                ccol = ((Token)(cu_to_insertion_point_1[l])).beginColumn;
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
        string superClass = Options.StringValue(Options.USEROPTION__TOKEN_MANAGER_SUPER_CLASS);
        GenClassStart(null, TokMgrClassName, Array.Empty<string>(), new String[] { "public TokenManager" + (superClass == null ? "" : ", public " + superClass) });

        if (TokenManagerDeclarations != null && TokenManagerDeclarations.Count > 0)
        {
            Token t = (Token)TokenManagerDeclarations[0];
            bool commonTokenActionSeen = false;
            bool commonTokenActionNeeded = Options.GetCommonTokenAction();

            PrintTokenSetup((Token)TokenManagerDeclarations[0]);
            ccol = 1;

            SwitchToMainFile();
            for (j = 0; j < TokenManagerDeclarations.Count; j++)
            {
                t = (Token)TokenManagerDeclarations[j];
                if (t.kind == IDENTIFIER &&
                    commonTokenActionNeeded &&
                    !commonTokenActionSeen)
                {
                    commonTokenActionSeen = t.image == ("CommonTokenAction");
                    if (commonTokenActionSeen)
                        t.image = cu_name + "TokenManager::" + t.image;
                }

                PrintToken(t);
            }

            SwitchToIncludeFile();
            GenCodeLine("  void CommonTokenAction(Token* token);");

            if (Options.GetTokenManagerUsesParser())
            {
                GenCodeLine("  void setParser(void* parser) {");
                GenCodeLine("      this->parser = (" + cu_name + "*) parser;");
                GenCodeLine("  }");
            }
            GenCodeLine("");

            if (commonTokenActionNeeded && !commonTokenActionSeen)
                CSharpCCErrors.Warning("You have the COMMON_TOKEN_ACTION option set. " +
                    "But it appears you have not defined the method :\n" +
                    "      " + StaticString + "void CommonTokenAction(Token *t)\n" +
                "in your TOKEN_MGR_DECLS. The generated token manager will not compile.");

        }
        else if (Options.GetCommonTokenAction())
        {
            CSharpCCErrors.Warning("You have the COMMON_TOKEN_ACTION option set. " +
                "But you have not defined the method :\n" +
                "      " + StaticString + "void CommonTokenAction(Token *t)\n" +
            "in your TOKEN_MGR_DECLS. The generated token manager will not compile.");
        }

        GenCodeLine("");
        GenCodeLine("  FILE *debugStream;");

        GenerateMethodDefHeader("  void ", TokMgrClassName, "setDebugStream(FILE *ds)");
        GenCodeLine("{ debugStream = ds; }");

        SwitchToIncludeFile();
        if (Options.GetTokenManagerUsesParser())
        {
            GenCodeLine("");
            GenCodeLine("private:");
            GenCodeLine("  " + cu_name + "* parser = nullptr;");
        }
        SwitchToMainFile();
    }

    void DumpDebugMethods()
    {
        WriteTemplate("/templates/cpp/DumpDebugMethods.template",
              "maxOrdinal", MaxOrdinal,
              "stateSetSize", StateSetSize);
    }

    static void BuildLexStatesTable()
    {
        int i;

        String[] tmpLexStateName = new String[LexstateI2S.Count];
        foreach(var tp in RegexpList)
        {
            List<RegExprSpec> respecs = tp.respecs;

            for (i = 0; i < tp.lexStates.Length; i++)
            {
                if (!AllTpsForState.TryGetValue(tp.lexStates[i],out var tps))
                {
                    tmpLexStateName[MaxLexStates++] = tp.lexStates[i];
                    AllTpsForState.Add(tp.lexStates[i], tps = new());
                }

                tps.Add(tp);
            }

            if (respecs == null || respecs.Count == 0)
                continue;

            RegularExpression re;
            for (i = 0; i < respecs.Count; i++)
                if (MaxOrdinal <= (re = ((RegExprSpec)respecs[i]).Rexp).ordinal)
                    MaxOrdinal = re.ordinal + 1;
        }

        Kinds = new int[MaxOrdinal];
        ToSkip = new ulong[MaxOrdinal / 64 + 1];
        ToSpecial = new ulong[MaxOrdinal / 64 + 1];
        ToMore = new ulong[MaxOrdinal / 64 + 1];
        ToToken = new ulong[MaxOrdinal / 64 + 1];
        ToToken[0] = 1L;
        Actions = new Action[MaxOrdinal];
        Actions[0] = actForEof;
        HasTokenActions = actForEof != null;
        InitStates = new (); 
        CanMatchAnyChar = new int[MaxLexStates]; 
        CanLoop = new bool[MaxLexStates];
        StateHasActions = new bool[MaxLexStates];
        LexStateName = new String[MaxLexStates];
        SinglesToSkip = new NfaState[MaxLexStates];
        Array.Copy(tmpLexStateName, 0, LexStateName, 0, MaxLexStates);

        for (i = 0; i < MaxLexStates; i++)
            CanMatchAnyChar[i] = -1;

        HasNfa = new bool[MaxLexStates];
        Mixed = new bool[MaxLexStates];
        MaxLongsReqd = new int[MaxLexStates];
        InitMatch = new int[MaxLexStates];
        NewLexState = new String[MaxOrdinal];
        NewLexState[0] = nextStateForEof;
        HasEmptyMatch = false;
        LexStates = new int[MaxOrdinal];
        IgnoreCase = new bool[MaxOrdinal];
        Rexprs = new RegularExpression[MaxOrdinal];
        RStringLiteral.AllImages = new String[MaxOrdinal];
        CanReachOnMore = new bool[MaxLexStates];
    }

    static int GetIndex(string name)
    {
        for (int i = 0; i < LexStateName.Length; i++)
            if (LexStateName[i] != null && LexStateName[i] == (name))
                return i;

        throw new Error(); // Should never come here
    }

    public static void AddCharToSkip(char c, int kind)
    {
        SinglesToSkip[LexStateIndex].AddChar(c);
        SinglesToSkip[LexStateIndex].kind = kind;
    }

    public new void Start()
    {
        if (!Options.GetBuildTokenManager() ||
            Options.GetUserTokenManager() ||
            CSharpCCErrors.            ErrorCount > 0)
            return;

        KeepLineCol = Options.GetKeepLineColumn();
        List<Expansion> choices = new();
        TokenProduction tp;
        int i, j;

        StaticString = (Options.GetStatic() ? "static " : "");
        TokMgrClassName = cu_name + "TokenManager";

        PrintClassHead();
        BuildLexStatesTable();

        bool ignoring = false;

        foreach(var key in AllTpsForState.Keys)
        {
            NfaState.ReInit();
            RStringLiteral.ReInit();

            LexStateIndex = GetIndex(key);
            LexStateSuffix = "_" + LexStateIndex;
            var allTps = AllTpsForState[key];
            InitStates.Add(key, InitialState = new NfaState());
            ignoring = false;

            SinglesToSkip[LexStateIndex] = new NfaState();
            SinglesToSkip[LexStateIndex].dummy = true;

            if (key == ("DEFAULT"))
                DefaultLexState = LexStateIndex;

            for (i = 0; i < allTps.Count; i++)
            {
                tp = allTps[i];
                int kind = tp.kind;
                bool ignore = tp.ignoreCase;
                List<RegExprSpec> rexps = tp.respecs;

                if (i == 0)
                    ignoring = ignore;

                for (j = 0; j < rexps.Count; j++)
                {
                    RegExprSpec respec = rexps[j];
                    CurRE = respec.Rexp;

                    Rexprs[CurKind = CurRE.ordinal] = CurRE;
                    LexStates[CurRE.ordinal] = LexStateIndex;
                    IgnoreCase[CurRE.ordinal] = ignore;

                    if (CurRE.private_rexp)
                    {
                        Kinds[CurRE.ordinal] = -1;
                        continue;
                    }

                    if (CurRE is RStringLiteral literal &&
                        literal.image != (""))
                    {
                        literal.GenerateDfa(this, CurRE.ordinal);
                        if (i != 0 && !Mixed[LexStateIndex] && ignoring != ignore)
                            Mixed[LexStateIndex] = true;
                    }
                    else if (CurRE.CanMatchAnyChar)
                    {
                        if (CanMatchAnyChar[LexStateIndex] == -1 ||
                            CanMatchAnyChar[LexStateIndex] > CurRE.ordinal)
                            CanMatchAnyChar[LexStateIndex] = CurRE.ordinal;
                    }
                    else
                    {
                        Nfa temp;

                        if (CurRE is RChoice)
                            choices.Add(CurRE);

                        temp = CurRE.GenerateNfa(ignore);
                        temp.End.isFinal = true;
                        temp.End.kind = CurRE.ordinal;
                        InitialState.AddMove(temp.Start);
                    }

                    if (Kinds.Length < CurRE.ordinal)
                    {
                        int[] tmp = new int[CurRE.ordinal + 1];

                        Array.Copy(Kinds, 0, tmp, 0, Kinds.Length);
                        Kinds = tmp;
                    }
                    //Console.WriteLine("   ordina : " + curRE.ordinal);

                    Kinds[CurRE.ordinal] = kind;

                    if (respec.NextState != null &&
                        respec.NextState != (LexStateName[LexStateIndex]))
                        NewLexState[CurRE.ordinal] = respec.NextState;

                    if (respec.Act != null && respec.Act.ActionTokens != null &&
                        respec.Act.                        ActionTokens.Count > 0)
                        Actions[CurRE.ordinal] = respec.Act;

                    switch (kind)
                    {
                        case TokenProduction.SPECIAL:
                            HasSkipActions |= (Actions[CurRE.ordinal] != null) ||
                            (NewLexState[CurRE.ordinal] != null);
                            HasSpecial = true;
                            ToSpecial[CurRE.ordinal / 64] |= 1UL << (CurRE.ordinal % 64);
                            ToSkip[CurRE.ordinal / 64] |= 1UL << (CurRE.ordinal % 64);
                            break;
                        case TokenProduction.SKIP:
                            HasSkipActions |= (Actions[CurRE.ordinal] != null);
                            HasSkip = true;
                            ToSkip[CurRE.ordinal / 64] |= 1UL << (CurRE.ordinal % 64);
                            break;
                        case TokenProduction.MORE:
                            HasMoreActions |= (Actions[CurRE.ordinal] != null);
                            HasMore = true;
                            ToMore[CurRE.ordinal / 64] |= 1UL << (CurRE.ordinal % 64);

                            if (NewLexState[CurRE.ordinal] != null)
                                CanReachOnMore[GetIndex(NewLexState[CurRE.ordinal])] = true;
                            else
                                CanReachOnMore[LexStateIndex] = true;

                            break;
                        case TokenProduction.TOKEN:
                            HasTokenActions |= (Actions[CurRE.ordinal] != null);
                            ToToken[CurRE.ordinal / 64] |= 1UL << (CurRE.ordinal % 64);
                            break;
                    }
                }
            }

            // Generate a static block for initializing the nfa transitions
            NfaState.ComputeClosures();

            for (i = 0; i < InitialState.epsilonMoves.Count; i++)
                ((NfaState)InitialState.epsilonMoves[i]).GenerateCode();

            HasNfa[LexStateIndex] = (NfaState.generatedStates != 0);
            if (HasNfa[LexStateIndex])
            {
                InitialState.GenerateCode();
                InitialState.GenerateInitMoves(this);
            }

            if (InitialState.kind != int.MaxValue && InitialState.kind != 0)
            {
                if ((ToSkip[InitialState.kind / 64] & (1UL << InitialState.kind)) != 0L ||
                    (ToSpecial[InitialState.kind / 64] & (1UL << InitialState.kind)) != 0L)
                    HasSkipActions = true;
                else if ((ToMore[InitialState.kind / 64] & (1UL << InitialState.kind)) != 0L)
                    HasMoreActions = true;
                else
                    HasTokenActions = true;

                if (InitMatch[LexStateIndex] == 0 ||
                    InitMatch[LexStateIndex] > InitialState.kind)
                {
                    InitMatch[LexStateIndex] = InitialState.kind;
                    HasEmptyMatch = true;
                }
            }
            else if (InitMatch[LexStateIndex] == 0)
                InitMatch[LexStateIndex] = int.MaxValue;

            RStringLiteral.FillSubString();

            if (HasNfa[LexStateIndex] && !Mixed[LexStateIndex])
                RStringLiteral.GenerateNfaStartStates(this, InitialState);

            RStringLiteral.DumpDfaCode(this);

            if (HasNfa[LexStateIndex])
                NfaState.DumpMoveNfa(this);

            if (StateSetSize < NfaState.generatedStates)
                StateSetSize = NfaState.generatedStates;
        }

        for (i = 0; i < choices.Count; i++)
            ((RChoice)choices[i]).CheckUnmatchability();

        NfaState.DumpStateSets(this);
        CheckEmptyStringMatch();
        NfaState.DumpNonAsciiMoveMethods(this);
        RStringLiteral.DumpStrLiteralImages(this);
        DumpFillToken();
        DumpGetNextToken();

        if (Options.GetDebugTokenManager())
        {
            NfaState.DumpStatesForKind(this);
            DumpDebugMethods();
        }

        if (HasLoop)
        {
            SwitchToStaticsFile();
            GenCodeLine("static int  jjemptyLineNo[" + MaxLexStates + "];");
            GenCodeLine("static int  jjemptyColNo[" + MaxLexStates + "];");
            GenCodeLine("static bool jjbeenHere[" + MaxLexStates + "];");
            SwitchToMainFile();
        }

        if (HasSkipActions)
            DumpSkipActions();
        if (HasMoreActions)
            DumpMoreActions();
        if (HasTokenActions)
            DumpTokenActions();

        NfaState.PrintBoilerPlateCPP(this);

        string charStreamName;
        if (Options.GetUserCharStream())
            charStreamName = "CharStream";
        else
        {
            if (Options.GetJavaUnicodeEscape())
                charStreamName = "JavaCharStream";
            else
                charStreamName = "SimpleCharStream";
        }

        WriteTemplate("/templates/cpp/TokenManagerBoilerPlateMethods.template",
          "charStreamName", "CharStream",
          "parserClassName", cu_name,
          "defaultLexState", "defaultLexState",
          "lexStateNameLength", LexStateName.Length);

        dumpBoilerPlateInHeader();

        // in the include file close the class signature
        DumpStaticVarDeclarations(); // static vars actually inst

        SwitchToIncludeFile(); // remaining variables
        WriteTemplate("/templates/cpp/DumpVarDeclarations.template",
          "charStreamName", "CharStream",
          "lexStateNameLength", LexStateName.Length);
        GenCodeLine(/*{*/ "};");

        SwitchToStaticsFile();
        // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
        string fileName = Options.GetOutputDirectory() + Path.DirectorySeparatorChar +
                          TokMgrClassName +
                          GetFileExtension(Options.GetOutputLanguage());
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
        GenCodeLine("  " + TokMgrClassName + "(JAVACC_CHARSTREAM *stream, int lexState = " + DefaultLexState + ");");
        GenCodeLine("  virtual ~" + TokMgrClassName + "();");
        GenCodeLine("  void ReInit(JAVACC_CHARSTREAM *stream, int lexState = " + DefaultLexState + ");");
        GenCodeLine("  void SwitchTo(int lexState);");
        GenCodeLine("  void clear();");
        GenCodeLine("  const JJSimpleString jjKindsForBitVector(int i, " + Options.GetLongType() + " vec);");
        GenCodeLine("  const JJSimpleString jjKindsForStateVector(int lexState, int vec[], int start, int end);");
        GenCodeLine("");
    }

    private void DumpStaticVarDeclarations()
    {
        int i;

        SwitchToStaticsFile(); // remaining variables
        GenCodeLine("");
        GenCodeLine("/** Lexer state names. */");
        GenStringLiteralArrayCPP("lexStateNames", LexStateName);

        if (MaxLexStates > 1)
        {
            GenCodeLine("");
            GenCodeLine("/** Lex State array. */");
            GenCode("static const int jjnewLexState[] = {");

            for (i = 0; i < MaxOrdinal; i++)
            {
                if (i % 25 == 0)
                    GenCode("\n   ");

                if (NewLexState[i] == null)
                    GenCode("-1, ");
                else
                    GenCode(GetIndex(NewLexState[i]) + ", ");
            }
            GenCodeLine("\n};");
        }

        if (HasSkip || HasMore || HasSpecial)
        {
            // Bit vector for TOKEN
            GenCode("static const " + Options.GetLongType() + " jjtoToken[] = {");
            for (i = 0; i < MaxOrdinal / 64 + 1; i++)
            {
                if (i % 4 == 0)
                    GenCode("\n   ");
                GenCode("0x" + Convert.ToString((long)ToToken[i],16) + "L, ");
            }
            GenCodeLine("\n};");
        }

        if (HasSkip || HasSpecial)
        {
            // Bit vector for SKIP
            GenCode("static const " + Options.GetLongType() + " jjtoSkip[] = {");
            for (i = 0; i < MaxOrdinal / 64 + 1; i++)
            {
                if (i % 4 == 0)
                    GenCode("\n   ");
                GenCode("0x" + Convert.ToString((long)ToSkip[i],16) + "L, ");
            }
            GenCodeLine("\n};");
        }

        if (HasSpecial)
        {
            // Bit vector for SPECIAL
            GenCode("static const " + Options.GetLongType() + " jjtoSpecial[] = {");
            for (i = 0; i < MaxOrdinal / 64 + 1; i++)
            {
                if (i % 4 == 0)
                    GenCode("\n   ");
                GenCode("0x" + Convert.ToString((long)ToSpecial[i], 16) + "L, ");
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
        double tokenVersion = CSharpFiles.GetVersion("Token.java");
        bool hasBinaryNewToken = tokenVersion > 4.09;

        GenerateMethodDefHeader("Token *", TokMgrClassName, "jjFillToken()");
        GenCodeLine("{");
        GenCodeLine("   Token *t;");
        GenCodeLine("   JJString curTokenImage;");
        if (KeepLineCol)
        {
            GenCodeLine("   int beginLine   = -1;");
            GenCodeLine("   int endLine     = -1;");
            GenCodeLine("   int beginColumn = -1;");
            GenCodeLine("   int endColumn   = -1;");
        }

        if (HasEmptyMatch)
        {
            GenCodeLine("   if (jjmatchedPos < 0)");
            GenCodeLine("   {");
            GenCodeLine("       curTokenImage = image.c_str();");

            if (KeepLineCol)
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

            if (KeepLineCol)
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
            if (KeepLineCol)
            {
                GenCodeLine("   if (input_stream->getTrackLineColumn()) {");
                GenCodeLine("     beginLine = input_stream->getBeginLine();");
                GenCodeLine("     beginColumn = input_stream->getBeginColumn();");
                GenCodeLine("     endLine = input_stream->getEndLine();");
                GenCodeLine("     endColumn = input_stream->getEndColumn();");
                GenCodeLine("   }");
            }
        }

        if (Options.GetTokenFactory().Length > 0)
        {
            GenCodeLine("   t = " + GetClassQualifier(Options.GetTokenFactory()) + "newToken(jjmatchedKind, curTokenImage);");
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

        if (KeepLineCol)
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
        GenCodeLine("const int defaultLexState = " + DefaultLexState + ";");
        GenCodeLine("/** Get the next Token. */");
        GenerateMethodDefHeader("Token *", TokMgrClassName, "getNextToken()");
        GenCodeLine("{");
        if (HasSpecial)
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

        if (Options.GetDebugTokenManager())
            GenCodeLine("      fprintf(debugStream, \"Returning the <EOF> token.\\n\");");

        GenCodeLine("      jjmatchedKind = 0;");
        GenCodeLine("      jjmatchedPos = -1;");
        GenCodeLine("      matchedToken = jjFillToken();");

        if (HasSpecial)
            GenCodeLine("      matchedToken->specialToken = specialToken;");

        if (nextStateForEof != null || actForEof != null)
            GenCodeLine("      TokenLexicalActions(matchedToken);");

        if (Options.GetCommonTokenAction())
            GenCodeLine("      CommonTokenAction(matchedToken);");

        GenCodeLine("      return matchedToken;");
        GenCodeLine("   }");
        GenCodeLine("   curChar = input_stream->BeginToken();");

        if (HasMoreActions || HasSkipActions || HasTokenActions)
        {
            GenCodeLine("   image = jjimage;");
            GenCodeLine("   image.clear();");
            GenCodeLine("   jjimageLen = 0;");
        }

        GenCodeLine("");

        string prefix = "";
        if (HasMore)
        {
            GenCodeLine("   for (;;)");
            GenCodeLine("   {");
            prefix = "  ";
        }

        string endSwitch = "";
        string caseStr = "";
        // this also sets up the start state of the nfa
        if (MaxLexStates > 1)
        {
            GenCodeLine(prefix + "   switch(curLexState)");
            GenCodeLine(prefix + "   {");
            endSwitch = prefix + "   }";
            caseStr = prefix + "     case ";
            prefix += "    ";
        }

        prefix += "   ";
        for (i = 0; i < MaxLexStates; i++)
        {
            if (MaxLexStates > 1)
                GenCodeLine(caseStr + i + ":");

            if (SinglesToSkip[i].HasTransitions())
            {
                // added the backup(0) to make JIT happy
                GenCodeLine(prefix + "{ input_stream->backup(0);");
                if (SinglesToSkip[i].asciiMoves[0] != 0L &&
                    SinglesToSkip[i].asciiMoves[1] != 0L)
                {
                    GenCodeLine(prefix + "   while ((curChar < 64" + " && (0x" +
                        Convert.ToString((long)SinglesToSkip[i].asciiMoves[0],16) +
                        "L & (1L << curChar)) != 0L) || \n" +
                        prefix + "          (curChar >> 6) == 1" +
                        " && (0x" +
                        Convert.ToString((long)SinglesToSkip[i].asciiMoves[1], 16) +
                    "L & (1L << (curChar & 077))) != 0L)");
                }
                else if (SinglesToSkip[i].asciiMoves[1] == 0L)
                {
                    GenCodeLine(prefix + "   while (curChar <= " +
                        (int)MaxChar((long)SinglesToSkip[i].asciiMoves[0]) + " && (0x" +
                        Convert.ToString((long)SinglesToSkip[i].asciiMoves[0], 16) +
                    "L & (1L << curChar)) != 0L)");
                }
                else if (SinglesToSkip[i].asciiMoves[0] == 0L)
                {
                    GenCodeLine(prefix + "   while (curChar > 63 && curChar <= " +
                        ((int)MaxChar((long)SinglesToSkip[i].asciiMoves[1]) + 64) +
                        " && (0x" +
                        Convert.ToString((long)SinglesToSkip[i].asciiMoves[1], 16) +
                    "L & (1L << (curChar & 077))) != 0L)");
                }

                GenCodeLine(prefix + "{");
                if (Options.GetDebugTokenManager())
                {
                    if (MaxLexStates > 1)
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

            if (InitMatch[i] != int.MaxValue && InitMatch[i] != 0)
            {
                if (Options.GetDebugTokenManager())
                    GenCodeLine("      fprintf(debugStream, \"   Matched the empty string as %s token.\\n\", addUnicodeEscapes(tokenImage[" + InitMatch[i] + "]).c_str());");

                GenCodeLine(prefix + "jjmatchedKind = " + InitMatch[i] + ";");
                GenCodeLine(prefix + "jjmatchedPos = -1;");
                GenCodeLine(prefix + "curPos = 0;");
            }
            else
            {
                GenCodeLine(prefix + "jjmatchedKind = 0x" + Convert.ToString(int.MaxValue, 16) + ";");
                GenCodeLine(prefix + "jjmatchedPos = 0;");
            }

            if (Options.GetDebugTokenManager())
            {
                GenCodeLine("   fprintf(debugStream, " +
                  "\"<%s>Current character : %c(%d) at line %d column %d\\n\"," +
                  "addUnicodeEscapes(lexStateNames[curLexState]).c_str(), curChar, (int)curChar, " +
                  "input_stream->getEndLine(), input_stream->getEndColumn());");
            }

            GenCodeLine(prefix + "curPos = jjMoveStringLiteralDfa0_" + i + "();");

            if (CanMatchAnyChar[i] != -1)
            {
                if (InitMatch[i] != int.MaxValue && InitMatch[i] != 0)
                    GenCodeLine(prefix + "if (jjmatchedPos < 0 || (jjmatchedPos == 0 && jjmatchedKind > " +
                        CanMatchAnyChar[i] + "))");
                else
                    GenCodeLine(prefix + "if (jjmatchedPos == 0 && jjmatchedKind > " +
                        CanMatchAnyChar[i] + ")");
                GenCodeLine(prefix + "{");

                if (Options.GetDebugTokenManager())
                {
                    GenCodeLine("           fprintf(debugStream, \"   Current character matched as a %s token.\\n\", addUnicodeEscapes(tokenImage[" + CanMatchAnyChar[i] + "]).c_str());");
                }
                GenCodeLine(prefix + "   jjmatchedKind = " + CanMatchAnyChar[i] + ";");

                if (InitMatch[i] != int.MaxValue && InitMatch[i] != 0)
                    GenCodeLine(prefix + "   jjmatchedPos = 0;");

                GenCodeLine(prefix + "}");
            }

            if (MaxLexStates > 1)
                GenCodeLine(prefix + "break;");
        }

        if (MaxLexStates > 1)
            GenCodeLine(endSwitch);
        else if (MaxLexStates == 0)
            GenCodeLine("       jjmatchedKind = 0x" + Convert.ToString(int.MaxValue, 16) + ";");

        if (MaxLexStates > 1)
            prefix = "  ";
        else
            prefix = "";

        if (MaxLexStates > 0)
        {
            GenCodeLine(prefix + "   if (jjmatchedKind != 0x" + Convert.ToString(int.MaxValue, 16) + ")");
            GenCodeLine(prefix + "   {");
            GenCodeLine(prefix + "      if (jjmatchedPos + 1 < curPos)");

            if (Options.GetDebugTokenManager())
            {
                GenCodeLine(prefix + "      {");
                GenCodeLine(prefix + "         fprintf(debugStream, " +
                "\"   Putting back %d characters into the input stream.\\n\", (curPos - jjmatchedPos - 1));");
            }

            GenCodeLine(prefix + "         input_stream->backup(curPos - jjmatchedPos - 1);");

            if (Options.GetDebugTokenManager())
            {
                GenCodeLine(prefix + "      }");
            }

            if (Options.GetDebugTokenManager())
            {
                GenCodeLine("    fprintf(debugStream, " +
                    "\"****** FOUND A %d(%s) MATCH (%s) ******\\n\", jjmatchedKind, addUnicodeEscapes(tokenImage[jjmatchedKind]).c_str(), addUnicodeEscapes(input_stream->GetSuffix(jjmatchedPos + 1)).c_str());");
            }

            if (HasSkip || HasMore || HasSpecial)
            {
                GenCodeLine(prefix + "      if ((jjtoToken[jjmatchedKind >> 6] & " +
                "(1L << (jjmatchedKind & 077))) != 0L)");
                GenCodeLine(prefix + "      {");
            }

            GenCodeLine(prefix + "         matchedToken = jjFillToken();");

            if (HasSpecial)
                GenCodeLine(prefix + "         matchedToken->specialToken = specialToken;");

            if (HasTokenActions)
                GenCodeLine(prefix + "         TokenLexicalActions(matchedToken);");

            if (MaxLexStates > 1)
            {
                GenCodeLine("       if (jjnewLexState[jjmatchedKind] != -1)");
                GenCodeLine(prefix + "       curLexState = jjnewLexState[jjmatchedKind];");
            }

            if (Options.GetCommonTokenAction())
                GenCodeLine(prefix + "         CommonTokenAction(matchedToken);");

            GenCodeLine(prefix + "         return matchedToken;");

            if (HasSkip || HasMore || HasSpecial)
            {
                GenCodeLine(prefix + "      }");

                if (HasSkip || HasSpecial)
                {
                    if (HasMore)
                    {
                        GenCodeLine(prefix + "      else if ((jjtoSkip[jjmatchedKind >> 6] & " +
                        "(1L << (jjmatchedKind & 077))) != 0L)");
                    }
                    else
                        GenCodeLine(prefix + "      else");

                    GenCodeLine(prefix + "      {");

                    if (HasSpecial)
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

                        if (HasSkipActions)
                            GenCodeLine(prefix + "            SkipLexicalActions(matchedToken);");

                        GenCodeLine(prefix + "         }");

                        if (HasSkipActions)
                        {
                            GenCodeLine(prefix + "         else");
                            GenCodeLine(prefix + "            SkipLexicalActions(nullptr);");
                        }
                    }
                    else if (HasSkipActions)
                        GenCodeLine(prefix + "         SkipLexicalActions(nullptr);");

                    if (MaxLexStates > 1)
                    {
                        GenCodeLine("         if (jjnewLexState[jjmatchedKind] != -1)");
                        GenCodeLine(prefix + "         curLexState = jjnewLexState[jjmatchedKind];");
                    }

                    GenCodeLine(prefix + "         goto EOFLoop;");
                    GenCodeLine(prefix + "      }");
                }

                if (HasMore)
                {
                    if (HasMoreActions)
                        GenCodeLine(prefix + "      MoreLexicalActions();");
                    else if (HasSkipActions || HasTokenActions)
                        GenCodeLine(prefix + "      jjimageLen += jjmatchedPos + 1;");

                    if (MaxLexStates > 1)
                    {
                        GenCodeLine("      if (jjnewLexState[jjmatchedKind] != -1)");
                        GenCodeLine(prefix + "      curLexState = jjnewLexState[jjmatchedKind];");
                    }
                    GenCodeLine(prefix + "      curPos = 0;");
                    GenCodeLine(prefix + "      jjmatchedKind = 0x" + Convert.ToString(int.MaxValue,16) + ";");

                    GenCodeLine(prefix + "   if (!input_stream->endOfInput()) {");
                    GenCodeLine(prefix + "         curChar = input_stream->readChar();");

                    if (Options.GetDebugTokenManager())
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

        if (HasMore)
            GenCodeLine(prefix + " }");

        GenCodeLine("  }");
        GenCodeLine("}");
        GenCodeLine("");
    }

    public void DumpSkipActions()
    {
        Action act;

        GenerateMethodDefHeader("void ", TokMgrClassName, "SkipLexicalActions(Token *matchedToken)");
        GenCodeLine("{");
        GenCodeLine("   switch(jjmatchedKind)");
        GenCodeLine("   {");

    Outer:
        for (int i = 0; i < MaxOrdinal; i++)
        {
            if ((ToSkip[i / 64] & (1UL << (i % 64))) == 0L)
                continue;

            for (; ; )
            {
                if (((act = (Action)Actions[i]) == null ||
                    act.                    ActionTokens == null ||
                    act.                    ActionTokens.Count == 0) && !CanLoop[LexStates[i]])
                    goto OuterExit;

                GenCodeLine("      case " + i + " : {");

                if (InitMatch[LexStates[i]] == i && CanLoop[LexStates[i]])
                {
                    GenCodeLine("         if (jjmatchedPos == -1)");
                    GenCodeLine("         {");
                    GenCodeLine("            if (jjbeenHere[" + LexStates[i] + "] &&");
                    GenCodeLine("                jjemptyLineNo[" + LexStates[i] + "] == input_stream->getBeginLine() &&");
                    GenCodeLine("                jjemptyColNo[" + LexStates[i] + "] == input_stream->getBeginColumn())");
                    GenCodeLine("               errorHandler->lexicalError(JJString(\"(\"Error: Bailing out of infinite loop caused by repeated empty string matches \" + \"at line \" + input_stream->getBeginLine() + \", \" + \"column \" + input_stream->getBeginColumn() + \".\")), this);");
                    GenCodeLine("            jjemptyLineNo[" + LexStates[i] + "] = input_stream->getBeginLine();");
                    GenCodeLine("            jjemptyColNo[" + LexStates[i] + "] = input_stream->getBeginColumn();");
                    GenCodeLine("            jjbeenHere[" + LexStates[i] + "] = true;");
                    GenCodeLine("         }");
                }

                if ((act = (Action)Actions[i]) == null ||
                    act.                    ActionTokens.Count == 0)
                    break;

                GenCode("         image.append");
                if (RStringLiteral.AllImages[i] != null)
                {
                    GenCodeLine("(jjstrLiteralImages[" + i + "]);");
                    GenCodeLine("        lengthOfMatch = jjstrLiteralImages[" + i + "].length();");
                }
                else
                {
                    GenCodeLine("(input_stream->GetSuffix(jjimageLen + (lengthOfMatch = jjmatchedPos + 1)));");
                }

                PrintTokenSetup((Token)act.ActionTokens[0]);
                ccol = 1;

                for (int j = 0; j < act.ActionTokens.Count; j++)
                    PrintToken((Token)act.ActionTokens[j]);
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

        GenerateMethodDefHeader("void ", TokMgrClassName, "MoreLexicalActions()");
        GenCodeLine("{");
        GenCodeLine("   jjimageLen += (lengthOfMatch = jjmatchedPos + 1);");
        GenCodeLine("   switch(jjmatchedKind)");
        GenCodeLine("   {");

    Outer:
        for (int i = 0; i < MaxOrdinal; i++)
        {
            if ((ToMore[i / 64] & (1UL << (i % 64))) == 0L)
                continue;

            for (; ; )
            {
                if (((act = (Action)Actions[i]) == null ||
                    act.                    ActionTokens == null ||
                    act.                    ActionTokens.Count == 0) && !CanLoop[LexStates[i]])
                    goto OuterExit;

                GenCodeLine("      case " + i + " : {");

                if (InitMatch[LexStates[i]] == i && CanLoop[LexStates[i]])
                {
                    GenCodeLine("         if (jjmatchedPos == -1)");
                    GenCodeLine("         {");
                    GenCodeLine("            if (jjbeenHere[" + LexStates[i] + "] &&");
                    GenCodeLine("                jjemptyLineNo[" + LexStates[i] + "] == input_stream->getBeginLine() &&");
                    GenCodeLine("                jjemptyColNo[" + LexStates[i] + "] == input_stream->getBeginColumn())");
                    GenCodeLine("               errorHandler->lexicalError(JJString(\"(\"Error: Bailing out of infinite loop caused by repeated empty string matches \" + \"at line \" + input_stream->getBeginLine() + \", \" + \"column \" + input_stream->getBeginColumn() + \".\")), this);");
                    GenCodeLine("            jjemptyLineNo[" + LexStates[i] + "] = input_stream->getBeginLine();");
                    GenCodeLine("            jjemptyColNo[" + LexStates[i] + "] = input_stream->getBeginColumn();");
                    GenCodeLine("            jjbeenHere[" + LexStates[i] + "] = true;");
                    GenCodeLine("         }");
                }

                if ((act = (Action)Actions[i]) == null ||
                    act.                    ActionTokens.Count == 0)
                {
                    break;
                }

                GenCode("         image.append");

                if (RStringLiteral.AllImages[i] != null)
                    GenCodeLine("(jjstrLiteralImages[" + i + "]);");
                else
                    GenCodeLine("(input_stream->GetSuffix(jjimageLen));");

                GenCodeLine("         jjimageLen = 0;");
                PrintTokenSetup((Token)act.ActionTokens[0]);
                ccol = 1;

                for (int j = 0; j < act.ActionTokens.Count; j++)
                    PrintToken((Token)act.ActionTokens[j]);
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

        GenerateMethodDefHeader("void ", TokMgrClassName, "TokenLexicalActions(Token *matchedToken)");
        GenCodeLine("{");
        GenCodeLine("   switch(jjmatchedKind)");
        GenCodeLine("   {");

    Outer:
        for (i = 0; i < MaxOrdinal; i++)
        {
            if ((ToToken[i / 64] & (1UL << (i % 64))) == 0L)
                continue;

            for (; ; )
            {
                if (((act = (Action)Actions[i]) == null ||
                    act.                    ActionTokens == null ||
                    act.                    ActionTokens.Count == 0) && !CanLoop[LexStates[i]])
                    goto OuterExit;

                GenCodeLine("      case " + i + " : {");

                if (InitMatch[LexStates[i]] == i && CanLoop[LexStates[i]])
                {
                    GenCodeLine("         if (jjmatchedPos == -1)");
                    GenCodeLine("         {");
                    GenCodeLine("            if (jjbeenHere[" + LexStates[i] + "] &&");
                    GenCodeLine("                jjemptyLineNo[" + LexStates[i] + "] == input_stream->getBeginLine() &&");
                    GenCodeLine("                jjemptyColNo[" + LexStates[i] + "] == input_stream->getBeginColumn())");
                    GenCodeLine("               errorHandler->lexicalError(JJString(\"Error: Bailing out of infinite loop caused by repeated empty string matches " + "at line \" + input_stream->getBeginLine() + \", " + "column \" + input_stream->getBeginColumn() + \".\"), this);");
                    GenCodeLine("            jjemptyLineNo[" + LexStates[i] + "] = input_stream->getBeginLine();");
                    GenCodeLine("            jjemptyColNo[" + LexStates[i] + "] = input_stream->getBeginColumn();");
                    GenCodeLine("            jjbeenHere[" + LexStates[i] + "] = true;");
                    GenCodeLine("         }");
                }

                if ((act = (Action)Actions[i]) == null ||
                    act.                    ActionTokens.Count == 0)
                    break;

                if (i == 0)
                {
                    GenCodeLine("      image.setLength(0);"); // For EOF no image is there
                }
                else
                {
                    GenCode("        image.append");

                    if (RStringLiteral.AllImages[i] != null)
                    {
                        GenCodeLine("(jjstrLiteralImages[" + i + "]);");
                        GenCodeLine("        lengthOfMatch = jjstrLiteralImages[" + i + "].length();");
                    }
                    else
                    {
                        GenCodeLine("(input_stream->GetSuffix(jjimageLen + (lengthOfMatch = jjmatchedPos + 1)));");
                    }
                }

                PrintTokenSetup((Token)act.ActionTokens[0]);
                ccol = 1;

                for (int j = 0; j < act.ActionTokens.Count; j++)
                    PrintToken((Token)act.ActionTokens[j]);
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
