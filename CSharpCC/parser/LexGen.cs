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
using org.javacc.utils;
using System.Text;

namespace org.javacc.parser;



/**
 * Generate lexer.
 */
public class LexGen : CodeGenerator
{
    private static readonly string DUMP_STATIC_VAR_DECLARATIONS_TEMPLATE_RESOURCE_URL = "/templates/DumpStaticVarDeclarations.template";
    private static readonly string DUMP_DEBUG_METHODS_TEMPLATE_RESOURCE_URL = "/templates/DumpDebugMethods.template";
    private static readonly string BOILERPLATER_METHOD_RESOURCE_URL = "/templates/TokenManagerBoilerPlateMethods.template";

    public static string staticString;
    public static string tokMgrClassName;

    // Dictionary of vectors
    public static Dictionary allTpsForState = new();
    public static int lexStateIndex = 0;
    public static int[] kinds;
    public static int maxOrdinal = 1;
    public static string lexStateSuffix;
    public static String[] newLexState;
    public static int[] lexStates;
    public static bool[] ignoreCase;
    public static Action[] actions;
    public static Dictionary initStates = new();
    public static int stateSetSize;
    public static int totalNumStates;
    public static int maxLexStates;
    public static String[] lexStateName;
    public static NfaState[] singlesToSkip;
    public static long[] toSkip;
    public static long[] toSpecial;
    public static long[] toMore;
    public static long[] toToken;
    public static int defaultLexState;
    public static RegularExpression[] rexprs;
    public static int[] maxLongsReqd;
    public static int[] initMatch;
    public static int[] canMatchAnyChar;
    public static bool hasEmptyMatch;
    public static bool[] canLoop;
    public static bool[] stateHasActions;
    public static bool hasLoop = false;
    public static bool[] canReachOnMore;
    public static bool[] hasNfa;
    public static bool[] mixed;
    public static NfaState initialState;
    public static int curKind;
    public static bool hasSkipActions = false;
    public static bool hasMoreActions = false;
    public static bool hasTokenActions = false;
    public static bool hasSpecial = false;
    public static bool hasSkip = false;
    public static bool hasMore = false;
    public static RegularExpression curRE;
    public static bool keepLineCol;
    public static string errorHandlingClass;
    public static TokenizerData tokenizerData;
    public static bool generateDataOnly;

    void PrintClassHead()
    {
        int i, j;
        bool bHasImport = false;

        List<string> tn = new(toolNames);
        tn.Add(toolName);
        // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
        genCodeLine("/* " + getIdString(tn, tokMgrClassName + getFileExtension(Options.getOutputLanguage())) + " */");

        int l = 0, kind;
        i = 1;
        for (; ; )
        {
            if (cu_to_insertion_point_1.Count <= l)
                break;

            kind = ((Token)cu_to_insertion_point_1.get(l)).kind;
            if (kind == PACKAGE || kind == IMPORT)
            {
                if (kind == IMPORT)
                    bHasImport = true;
                for (; i < cu_to_insertion_point_1.Count; i++)
                {
                    kind = ((Token)cu_to_insertion_point_1[i]).kind;
                    if (kind == SEMICOLON ||
                        kind == ABSTRACT ||
                        kind == FINAL ||
                        kind == PUBLIC ||
                        kind == CLASS ||
                        kind == INTERFACE)
                    {
                        cline = ((Token)(cu_to_insertion_point_1.get(l))).beginLine;
                        ccol = ((Token)(cu_to_insertion_point_1.get(l))).beginColumn;
                        for (j = l; j < i; j++)
                        {
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
        }

        GenCodeLine("");
        GenCodeLine("/** Token Manager. */");

        if (bHasImport && Options.getGenerateAnnotations())
        {
            GenCodeLine("@SuppressWarnings (\"unused\")");
        }
        if (Options.getSupportClassVisibilityPublic())
        {
            //genModifier("public ");
            genModifier("public ");
        }
        //genCodeLine("class " + tokMgrClassName + " implements " +
        //cu_name + "Constants");
        //String superClass = Options.stringValue(Options.USEROPTION__TOKEN_MANAGER_SUPER_CLASS);
        genClassStart(null, tokMgrClassName, new String[] { }, new String[] { cu_name + "Constants" });
        //genCodeLine("{"); // }

        if (token_mgr_decls != null && token_mgr_decls.Count > 0)
        {
            Token t = (Token)token_mgr_decls[0];
            bool commonTokenActionSeen = false;
            bool commonTokenActionNeeded = Options.getCommonTokenAction();

            PrintTokenSetup((Token)token_mgr_decls[0]);
            ccol = 1;

            for (j = 0; j < token_mgr_decls.Count; j++)
            {
                t = (Token)token_mgr_decls[j];
                if (t.kind == IDENTIFIER &&
                    commonTokenActionNeeded &&
                    !commonTokenActionSeen)
                    commonTokenActionSeen = t.image == ("CommonTokenAction");

                printToken(t);
            }

            GenCodeLine("");
            if (commonTokenActionNeeded && !commonTokenActionSeen)
                JavaCCErrors.warning("You have the COMMON_TOKEN_ACTION option set. " +
                    "But it appears you have not defined the method :\n" +
                    "      " + staticString + "void CommonTokenAction(Token t)\n" +
                "in your TOKEN_MGR_DECLS. The generated token manager will not compile.");

        }
        else if (Options.getCommonTokenAction())
        {
            JavaCCErrors.warning("You have the COMMON_TOKEN_ACTION option set. " +
                "But you have not defined the method :\n" +
                "      " + staticString + "void CommonTokenAction(Token t)\n" +
            "in your TOKEN_MGR_DECLS. The generated token manager will not compile.");
        }

        GenCodeLine("");
        GenCodeLine("  /** Debug output. */");
        GenCodeLine("  public " + staticString + " java.io.PrintStream debugStream = System.out;");
        GenCodeLine("  /** Set debug output. */");
        GenCodeLine("  public " + staticString + " void setDebugStream(java.io.PrintStream ds) { debugStream = ds; }");

        if (Options.getTokenManagerUsesParser())
        {
            GenCodeLine("");
            genCodeLine("  public " + cu_name + " parser = null;");
        }
    }


    protected void writeTemplate(string name, params object[] additionalOptions)
    {
        Dictionary<String, object> options = new Dictionary<String, object>(Options.getOptions());

        options.Add("maxOrdinal", (maxOrdinal));
        options.Add("maxLexStates", (maxLexStates));
        options.Add("hasEmptyMatch", (hasEmptyMatch));
        options.Add("hasSkip", (hasSkip));
        options.Add("hasMore", (hasMore));
        options.Add("hasSpecial", (hasSpecial));
        options.Add("hasMoreActions", (hasMoreActions));
        options.Add("hasSkipActions", (hasSkipActions));
        options.Add("hasTokenActions", (hasTokenActions));
        options.Add("stateSetSize", stateSetSize);
        options.Add("hasActions", hasMoreActions || hasSkipActions || hasTokenActions);
        options.Add("tokMgrClassName", tokMgrClassName);
        int x = 0;
    for (int l in maxLongsReqd) x = Math.Max(x, l);
        options.Add("maxLongs", x);
        options.Add("cu_name", cu_name);

        // options.Add("", .valueOf(maxOrdinal));


        for (int i = 0; i < additionalOptions.Length; i++)
        {
            object o = additionalOptions[i];

            if (o is Dictionary<string, object>)
            {
                options.putAll((Dictionary<String, object>)o);
            }
            else
            {
                if (i == additionalOptions.Length - 1)
                    throw new IllegalArgumentException("Must supply pairs of [name value] args");

                options.Add((String)o, additionalOptions[i + 1]);
                i++;
            }
        }

        OutputFileGenerator gen = new OutputFileGenerator(name, options);
        StringWriter sw = new StringWriter();
        gen.Generate(new TextWriter(sw));
        sw.Close();
        GenCode(sw.ToString());
    }

    void DumpDebugMethods()
    {
        writeTemplate(DUMP_DEBUG_METHODS_TEMPLATE_RESOURCE_URL);
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
                if ((tps = (List)allTpsForState.get(tp.lexStates[i])) == null)
                {
                    tmpLexStateName[maxLexStates++] = tp.lexStates[i];
                    allTpsForState.Add(tp.lexStates[i], tps = new ());
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
        initStates = new Dictionary();
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

    public void start()
    {
        if (!Options.getBuildTokenManager() ||
            Options.getUserTokenManager() ||
            JavaCCErrors.get_error_count() > 0)
            return;

        string codeGeneratorClass = Options.getTokenManagerCodeGenerator();
        keepLineCol = Options.getKeepLineColumn();
        errorHandlingClass = Options.getTokenMgrErrorClass();
        List choices = new ();
        Enumeration e;
        TokenProduction tp;
        int i, j;

        staticString = (Options.getStatic() ? "static " : "");
        tokMgrClassName = cu_name + "TokenManager";

        if (!generateDataOnly && codeGeneratorClass == null) PrintClassHead();
        BuildLexStatesTable();

        e = allTpsForState.keys();

        bool ignoring = false;

        while (e.hasMoreElements())
        {
            int startState = -1;
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

                    if (!Options.getNoDfa() && curRE is RStringLiteral &&
                        !((RStringLiteral)curRE).image == (""))
                    {
                        ((RStringLiteral)curRE).GenerateDfa(this, curRE.ordinal);
                        if (i != 0 && !mixed[lexStateIndex] && ignoring != ignore)
                        {
                            mixed[lexStateIndex] = true;
                        }
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
                    //System.out.WriteLine("   ordina : " + curRE.ordinal);

                    kinds[curRE.ordinal] = kind;

                    if (respec.nextState != null &&
                        respec.nextState != (lexStateName[lexStateIndex]))
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
                startState = initialState.GenerateInitMoves(this);
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

            if (generateDataOnly || codeGeneratorClass != null)
            {
                RStringLiteral.UpdateStringLiteralData(totalNumStates, lexStateIndex);
                NfaState.UpdateNfaData(totalNumStates, startState, lexStateIndex,
                                       canMatchAnyChar[lexStateIndex]);
            }
            else
            {
                RStringLiteral.DumpDfaCode(this);
                if (hasNfa[lexStateIndex])
                {
                    NfaState.DumpMoveNfa(this);
                }
            }
            totalNumStates += NfaState.generatedStates;
            if (stateSetSize < NfaState.generatedStates)
                stateSetSize = NfaState.generatedStates;
        }

        for (i = 0; i < choices.Count; i++)
            ((RChoice)choices[i]).CheckUnmatchability();

        CheckEmptyStringMatch();

        if (generateDataOnly || codeGeneratorClass != null)
        {
            tokenizerData.setParserName(cu_name);
            NfaState.BuildTokenizerData(tokenizerData);
            RStringLiteral.BuildTokenizerData(tokenizerData);
            int[] newLexStateIndices = new int[maxOrdinal];
            StringBuilder tokenMgrDecls = new StringBuilder();
            if (token_mgr_decls != null && token_mgr_decls.Count > 0)
            {
                Token t = (Token)token_mgr_decls[0];
                for (j = 0; j < token_mgr_decls.Count; j++)
                {
                    tokenMgrDecls.Append(((Token)token_mgr_decls[j]).image + " ");
                }
            }
            tokenizerData.setDecls(tokenMgrDecls.ToString());
            Dictionary<int, String> actionStrings = new Dictionary<int, String>();
            for (i = 0; i < maxOrdinal; i++)
            {
                if (newLexState[i] == null)
                {
                    newLexStateIndices[i] = -1;
                }
                else
                {
                    newLexStateIndices[i] = GetIndex(newLexState[i]);
                }
                // For java, we have this but for other languages, eventually we will
                // simply have a string.
                Action act = actions[i];
                if (act == null) continue;
                StringBuilder sb = new StringBuilder();
                for (int k = 0; k < act.GetActionTokens().Count; k++)
                {
                    sb.Append(((Token)act.GetActionTokens().get(k)).image);
                    sb.Append(" ");
                }
                actionStrings.Add(i, sb.ToString());
            }
            tokenizerData.setDefaultLexState(defaultLexState);
            tokenizerData.setLexStateNames(lexStateName);
            tokenizerData.updateMatchInfo(
                actionStrings, newLexStateIndices,
                toSkip, toSpecial, toMore, toToken);
            if (generateDataOnly) return;
            Class<TokenManagerCodeGenerator> codeGenClazz;
            TokenManagerCodeGenerator gen;
            try
            {
                codeGenClazz = (Class<TokenManagerCodeGenerator>)Class.forName(codeGeneratorClass);
                gen = codeGenClazz.newInstance();
            }
            catch (Exception ee)
            {
                JavaCCErrors.semantic_error(
                    "Could not load the token manager code generator class: " +
                    codeGeneratorClass + "\nError: " + ee.Message);
                return;
            }
            gen.generateCode(tokenizerData);
            gen.finish(tokenizerData);
            return;
        }

        RStringLiteral.DumpStrLiteralImages(this);
        DumpFillToken();
        NfaState.DumpStateSets(this);
        NfaState.DumpNonAsciiMoveMethods(this);
        DumpGetNextToken();

        if (Options.getDebugTokenManager())
        {
            NfaState.DumpStatesForKind(this);
            DumpDebugMethods();
        }

        if (hasLoop)
        {
            GenCodeLine(staticString + "int[] jjemptyLineNo = new int[" + maxLexStates + "];");
            GenCodeLine(staticString + "int[] jjemptyColNo = new int[" + maxLexStates + "];");
            GenCodeLine(staticString + "" + Options.getBooleanType() + "[] jjbeenHere = new " + Options.getBooleanType() + "[" + maxLexStates + "];");
        }

        DumpSkipActions();
        DumpMoreActions();
        DumpTokenActions();

        NfaState.PrintBoilerPlate(this);

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

        writeTemplate(BOILERPLATER_METHOD_RESOURCE_URL,
          "charStreamName", charStreamName,
          "lexStateNameLength", lexStateName.Length,
          "defaultLexState", defaultLexState,
          "noDfa", Options.getNoDfa(),
          "generatedStates", totalNumStates);

        DumpStaticVarDeclarations(charStreamName);
        GenCodeLine(/*{*/ "}");

        // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
        string fileName = Options.getOutputDirectory() + File.separator +
                          tokMgrClassName +
                          getFileExtension(Options.getOutputLanguage());

        if (Options.getBuildParser())
        {
            SaveOutput(fileName);
        }
    }

    static void CheckEmptyStringMatch()
    {
        int i, j, k, len;
        bool[] seen = new bool[maxLexStates];
        bool[] done = new bool[maxLexStates];
        string cycle;
        string reList;

    Outer:
        for (i = 0; i < maxLexStates; i++)
        {
            if (done[i] || initMatch[i] == 0 || initMatch[i] == int.MaxValue ||
                canMatchAnyChar[i] != -1)
                continue;

            done[i] = true;
            len = 0;
            cycle = "";
            reList = "";

            for (k = 0; k < maxLexStates; k++)
                seen[k] = false;

            j = i;
            seen[i] = true;
            cycle += lexStateName[j] + "-->";
            while (newLexState[initMatch[j]] != null)
            {
                cycle += newLexState[initMatch[j]];
                if (seen[j = GetIndex(newLexState[initMatch[j]])])
                    break;

                cycle += "-->";
                done[j] = true;
                seen[j] = true;
                if (initMatch[j] == 0 || initMatch[j] == int.MaxValue ||
                    canMatchAnyChar[j] != -1)
                    continue Outer;
                if (len != 0)
                    reList += "; ";
                reList += "line " + rexprs[initMatch[j]].GetLine() + ", column " +
                rexprs[initMatch[j]].GetColumn();
                len++;
            }

            if (newLexState[initMatch[j]] == null)
                cycle += lexStateName[lexStates[initMatch[j]]];

            for (k = 0; k < maxLexStates; k++)
                canLoop[k] |= seen[k];

            hasLoop = true;
            if (len == 0)
                JavaCCErrors.warning(rexprs[initMatch[i]],
                    "Regular expression" + ((rexprs[initMatch[i]].label == (""))
                        ? "" : (" for " + rexprs[initMatch[i]].label)) +
                        " can be matched by the empty string (\"\") in lexical state " +
                        lexStateName[i] + ". This can result in an endless loop of " +
                "empty string matches.");
            else
            {
                JavaCCErrors.warning(rexprs[initMatch[i]],
                    "Regular expression" + ((rexprs[initMatch[i]].label == (""))
                        ? "" : (" for " + rexprs[initMatch[i]].label)) +
                        " can be matched by the empty string (\"\") in lexical state " +
                        lexStateName[i] + ". This regular expression along with the " +
                        "regular expressions at " + reList + " forms the cycle \n   " +
                        cycle + "\ncontaining regular expressions with empty matches." +
                " This can result in an endless loop of empty string matches.");
            }
        }
    }

    private void DumpStaticVarDeclarations(string charStreamName)
    {
        int i;

        GenCodeLine("");
        GenCodeLine("/** Lexer state names. */");
        GenCodeLine("public static final String[] lexStateNames = {");
        for (i = 0; i < maxLexStates; i++)
            GenCodeLine("   \"" + lexStateName[i] + "\",");
        GenCodeLine("};");

        {
            GenCodeLine("");
            GenCodeLine("/** Lex State array. */");
            GenCode("public static final int[] jjnewLexState = {");

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

        {
            // Bit vector for TOKEN
            GenCode("static final long[] jjtoToken = {");
            for (i = 0; i < maxOrdinal / 64 + 1; i++)
            {
                if (i % 4 == 0)
                    GenCode("\n   ");
                genCode("0x" + Long.toHexString(toToken[i]) + "L, ");
            }
            GenCodeLine("\n};");
        }

        {
            // Bit vector for SKIP
            GenCode("static final long[] jjtoSkip = {");
            for (i = 0; i < maxOrdinal / 64 + 1; i++)
            {
                if (i % 4 == 0)
                    GenCode("\n   ");
                genCode("0x" + Long.toHexString(toSkip[i]) + "L, ");
            }
            GenCodeLine("\n};");
        }

        {
            // Bit vector for SPECIAL
            GenCode("static final long[] jjtoSpecial = {");
            for (i = 0; i < maxOrdinal / 64 + 1; i++)
            {
                if (i % 4 == 0)
                    GenCode("\n   ");
                genCode("0x" + Long.toHexString(toSpecial[i]) + "L, ");
            }
            GenCodeLine("\n};");
        }

        {
            // Bit vector for MORE
            GenCode("static final long[] jjtoMore = {");
            for (i = 0; i < maxOrdinal / 64 + 1; i++)
            {
                if (i % 4 == 0)
                    GenCode("\n   ");
                genCode("0x" + Long.toHexString(toMore[i]) + "L, ");
            }
            GenCodeLine("\n};");
        }

        writeTemplate(DUMP_STATIC_VAR_DECLARATIONS_TEMPLATE_RESOURCE_URL,
          "charStreamName", charStreamName,
          "protected", isJavaLanguage() ? "protected" : "",
          "private", isJavaLanguage() ? "private" : "",
          "final", isJavaLanguage() ? "final" : "",
          "lexStateNameLength", lexStateName.Length);
    }

    // Assumes l != 0L
    static char MaxChar(long l)
    {
        for (int i = 64; i-- > 0;)
            if ((l & (1L << i)) != 0L)
                return (char)i;

        return 0xffff;
    }

    void DumpFillToken()
    {
        double tokenVersion = JavaFiles.getVersion("Token.java");
        bool hasBinaryNewToken = tokenVersion > 4.09;

        GenCodeLine(staticString + "protected Token jjFillToken()");
        GenCodeLine("{");
        GenCodeLine("   final Token t;");
        GenCodeLine("   final String curTokenImage;");
        if (keepLineCol)
        {
            GenCodeLine("   final int beginLine;");
            GenCodeLine("   final int endLine;");
            GenCodeLine("   final int beginColumn;");
            GenCodeLine("   final int endColumn;");
        }

        if (hasEmptyMatch)
        {
            GenCodeLine("   if (jjmatchedPos < 0)");
            GenCodeLine("   {");
            GenCodeLine("      if (image == null)");
            GenCodeLine("         curTokenImage = \"\";");
            GenCodeLine("      else");
            GenCodeLine("         curTokenImage = image.toString();");

            if (keepLineCol)
            {
                GenCodeLine("      beginLine = endLine = input_stream.getEndLine();");
                GenCodeLine("      beginColumn = endColumn = input_stream.getEndColumn();");
            }

            GenCodeLine("   }");
            GenCodeLine("   else");
            GenCodeLine("   {");
            GenCodeLine("      String im = jjstrLiteralImages[jjmatchedKind];");
            GenCodeLine("      curTokenImage = (im == null) ? input_stream.GetImage() : im;");

            if (keepLineCol)
            {
                GenCodeLine("      beginLine = input_stream.getBeginLine();");
                GenCodeLine("      beginColumn = input_stream.getBeginColumn();");
                GenCodeLine("      endLine = input_stream.getEndLine();");
                GenCodeLine("      endColumn = input_stream.getEndColumn();");
            }

            GenCodeLine("   }");
        }
        else
        {
            GenCodeLine("   String im = jjstrLiteralImages[jjmatchedKind];");
            GenCodeLine("   curTokenImage = (im == null) ? input_stream.GetImage() : im;");
            if (keepLineCol)
            {
                GenCodeLine("   beginLine = input_stream.getBeginLine();");
                GenCodeLine("   beginColumn = input_stream.getBeginColumn();");
                GenCodeLine("   endLine = input_stream.getEndLine();");
                GenCodeLine("   endColumn = input_stream.getEndColumn();");
            }
        }

        if (Options.getTokenFactory().Length > 0)
        {
            GenCodeLine("   t = " + Options.getTokenFactory() + ".newToken(jjmatchedKind, curTokenImage);");
        }
        else if (hasBinaryNewToken)
        {
            GenCodeLine("   t = Token.newToken(jjmatchedKind, curTokenImage);");
        }
        else
        {
            GenCodeLine("   t = Token.newToken(jjmatchedKind);");
            GenCodeLine("   t.kind = jjmatchedKind;");
            GenCodeLine("   t.image = curTokenImage;");
        }

        if (keepLineCol)
        {
            GenCodeLine("");
            GenCodeLine("   t.beginLine = beginLine;");
            GenCodeLine("   t.endLine = endLine;");
            GenCodeLine("   t.beginColumn = beginColumn;");
            GenCodeLine("   t.endColumn = endColumn;");
        }

        GenCodeLine("");
        GenCodeLine("   return t;");
        GenCodeLine("}");
    }

    void DumpGetNextToken()
    {
        int i;

        GenCodeLine("");
        GenCodeLine(staticString + "int curLexState = " + defaultLexState + ";");
        GenCodeLine(staticString + "int defaultLexState = " + defaultLexState + ";");
        GenCodeLine(staticString + "int jjnewStateCnt;");
        GenCodeLine(staticString + "int jjround;");
        GenCodeLine(staticString + "int jjmatchedPos;");
        GenCodeLine(staticString + "int jjmatchedKind;");
        GenCodeLine("");
        GenCodeLine("/** Get the next Token. */");
        GenCodeLine("public " + staticString + "Token getNextToken()" +
        " ");
        GenCodeLine("{");
        if (hasSpecial)
        {
            GenCodeLine("  Token specialToken = null;");
        }
        GenCodeLine("  Token matchedToken;");
        GenCodeLine("  int curPos = 0;");
        GenCodeLine("");
        GenCodeLine("  EOFLoop :\n  for (;;)");
        GenCodeLine("  {");
        GenCodeLine("   try");
        GenCodeLine("   {");
        GenCodeLine("      curChar = input_stream.BeginToken();");
        GenCodeLine("   }");
        GenCodeLine("   catch(Exception e)");
        GenCodeLine("   {");

        if (Options.getDebugTokenManager())
            GenCodeLine("      debugStream.println(\"Returning the <EOF> token.\\n\");");

        GenCodeLine("      jjmatchedKind = 0;");
        GenCodeLine("      jjmatchedPos = -1;");
        GenCodeLine("      matchedToken = jjFillToken();");

        if (hasSpecial)
            GenCodeLine("      matchedToken.specialToken = specialToken;");

        if (nextStateForEof != null || actForEof != null)
            GenCodeLine("      TokenLexicalActions(matchedToken);");

        if (Options.getCommonTokenAction())
            GenCodeLine("      CommonTokenAction(matchedToken);");

        GenCodeLine("      return matchedToken;");
        GenCodeLine("   }");

        if (hasMoreActions || hasSkipActions || hasTokenActions)
        {
            GenCodeLine("   image = jjimage;");
            GenCodeLine("   image.setLength(0);");
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
                GenCodeLine(prefix + "try { input_stream.backup(0);");
                if (singlesToSkip[i].asciiMoves[0] != 0L &&
                    singlesToSkip[i].asciiMoves[1] != 0L)
                {
                    genCodeLine(prefix + "   while ((curChar < 64" + " && (0x" +
                        Long.toHexString(singlesToSkip[i].asciiMoves[0]) +
                        "L & (1L << curChar)) != 0L) || \n" +
                        prefix + "          (curChar >> 6) == 1" +
                        " && (0x" +
                        Long.toHexString(singlesToSkip[i].asciiMoves[1]) +
                    "L & (1L << (curChar & 077))) != 0L)");
                }
                else if (singlesToSkip[i].asciiMoves[1] == 0L)
                {
                    genCodeLine(prefix + "   while (curChar <= " +
                        (int)MaxChar(singlesToSkip[i].asciiMoves[0]) + " && (0x" +
                        Long.toHexString(singlesToSkip[i].asciiMoves[0]) +
                    "L & (1L << curChar)) != 0L)");
                }
                else if (singlesToSkip[i].asciiMoves[0] == 0L)
                {
                    genCodeLine(prefix + "   while (curChar > 63 && curChar <= " +
                        ((int)MaxChar(singlesToSkip[i].asciiMoves[1]) + 64) +
                        " && (0x" +
                        Long.toHexString(singlesToSkip[i].asciiMoves[1]) +
                    "L & (1L << (curChar & 077))) != 0L)");
                }

                if (Options.getDebugTokenManager())
                {
                    GenCodeLine(prefix + "{");
                    GenCodeLine("      debugStream.println(" +
                        (maxLexStates > 1 ?
                            "\"<\" + lexStateNames[curLexState] + \">\" + " : "") +
                            "\"Skipping character : \" + " +
                    errorHandlingClass + ".addEscapes(String.valueOf(curChar)) + \" (\" + (int)curChar + \")\");");
                }
                GenCodeLine(prefix + "      curChar = input_stream.BeginToken();");

                if (Options.getDebugTokenManager())
                    GenCodeLine(prefix + "}");

                GenCodeLine(prefix + "}");
                GenCodeLine(prefix + "catch (java.io.IOException e1) { continue EOFLoop; }");
            }

            if (initMatch[i] != int.MaxValue && initMatch[i] != 0)
            {
                if (Options.getDebugTokenManager())
                    GenCodeLine("      debugStream.println(\"   Matched the empty string as \" + tokenImage[" +
                        initMatch[i] + "] + \" token.\");");

                GenCodeLine(prefix + "jjmatchedKind = " + initMatch[i] + ";");
                GenCodeLine(prefix + "jjmatchedPos = -1;");
                GenCodeLine(prefix + "curPos = 0;");
            }
            else
            {
                genCodeLine(prefix + "jjmatchedKind = 0x" + int.toHexString(int.MaxValue) + ";");
                GenCodeLine(prefix + "jjmatchedPos = 0;");
            }

            if (Options.getDebugTokenManager())
                GenCodeLine("      debugStream.println(" +
                    (maxLexStates > 1 ? "\"<\" + lexStateNames[curLexState] + \">\" + " : "") +
                    "\"Current character : \" + " +
                    errorHandlingClass + ".addEscapes(String.valueOf(curChar)) + \" (\" + (int)curChar + \") " +
                "at line \" + input_stream.getEndLine() + \" column \" + input_stream.getEndColumn());");

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
                    GenCodeLine("           debugStream.println(\"   Current character matched as a \" + tokenImage[" +
                        canMatchAnyChar[i] + "] + \" token.\");");
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
            genCodeLine("       jjmatchedKind = 0x" + int.toHexString(int.MaxValue) + ";");

        if (maxLexStates > 1)
            prefix = "  ";
        else
            prefix = "";

        if (maxLexStates > 0)
        {
            genCodeLine(prefix + "   if (jjmatchedKind != 0x" + int.toHexString(int.MaxValue) + ")");
            GenCodeLine(prefix + "   {");
            GenCodeLine(prefix + "      if (jjmatchedPos + 1 < curPos)");

            if (Options.getDebugTokenManager())
            {
                GenCodeLine(prefix + "      {");
                GenCodeLine(prefix + "         debugStream.println(" +
                "\"   Putting back \" + (curPos - jjmatchedPos - 1) + \" characters into the input stream.\");");
            }

            GenCodeLine(prefix + "         input_stream.backup(curPos - jjmatchedPos - 1);");

            if (Options.getDebugTokenManager())
                GenCodeLine(prefix + "      }");

            if (Options.getDebugTokenManager())
            {
                if (Options.getJavaUnicodeEscape() ||
                    Options.getUserCharStream())
                    GenCodeLine("    debugStream.println(" +
                        "\"****** FOUND A \" + tokenImage[jjmatchedKind] + \" MATCH " +
                        "(\" + " + errorHandlingClass + ".addEscapes(new String(input_stream.GetSuffix(jjmatchedPos + 1))) + " +
                    "\") ******\\n\");");
                else
                    GenCodeLine("    debugStream.println(" +
                        "\"****** FOUND A \" + tokenImage[jjmatchedKind] + \" MATCH " +
                        "(\" + " + errorHandlingClass + ".addEscapes(new String(input_stream.GetSuffix(jjmatchedPos + 1))) + " +
                    "\") ******\\n\");");
            }

            if (hasSkip || hasMore || hasSpecial)
            {
                GenCodeLine(prefix + "      if ((jjtoToken[jjmatchedKind >> 6] & " +
                "(1L << (jjmatchedKind & 077))) != 0L)");
                GenCodeLine(prefix + "      {");
            }

            GenCodeLine(prefix + "         matchedToken = jjFillToken();");

            if (hasSpecial)
                GenCodeLine(prefix + "         matchedToken.specialToken = specialToken;");

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

                        GenCodeLine(prefix + "            if (specialToken == null)");
                        GenCodeLine(prefix + "               specialToken = matchedToken;");
                        GenCodeLine(prefix + "            else");
                        GenCodeLine(prefix + "            {");
                        GenCodeLine(prefix + "               matchedToken.specialToken = specialToken;");
                        GenCodeLine(prefix + "               specialToken = (specialToken.next = matchedToken);");
                        GenCodeLine(prefix + "            }");

                        if (hasSkipActions)
                            GenCodeLine(prefix + "            SkipLexicalActions(matchedToken);");

                        GenCodeLine(prefix + "         }");

                        if (hasSkipActions)
                        {
                            GenCodeLine(prefix + "         else");
                            GenCodeLine(prefix + "            SkipLexicalActions(null);");
                        }
                    }
                    else if (hasSkipActions)
                        GenCodeLine(prefix + "         SkipLexicalActions(null);");

                    if (maxLexStates > 1)
                    {
                        GenCodeLine("         if (jjnewLexState[jjmatchedKind] != -1)");
                        GenCodeLine(prefix + "         curLexState = jjnewLexState[jjmatchedKind];");
                    }

                    GenCodeLine(prefix + "         continue EOFLoop;");
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
                    genCodeLine(prefix + "      jjmatchedKind = 0x" + int.toHexString(int.MaxValue) + ";");

                    GenCodeLine(prefix + "      try {");
                    GenCodeLine(prefix + "         curChar = input_stream.readChar();");

                    if (Options.getDebugTokenManager())
                        GenCodeLine("   debugStream.println(" +
                            (maxLexStates > 1 ? "\"<\" + lexStateNames[curLexState] + \">\" + " : "") +
                            "\"Current character : \" + " +
                            "" + errorHandlingClass + ".addEscapes(String.valueOf(curChar)) + \" (\" + (int)curChar + \") " +
                        "at line \" + input_stream.getEndLine() + \" column \" + input_stream.getEndColumn());");
                    GenCodeLine(prefix + "         continue;");
                    GenCodeLine(prefix + "      }");
                    GenCodeLine(prefix + "      catch (java.io.IOException e1) { }");
                }
            }

            GenCodeLine(prefix + "   }");
            GenCodeLine(prefix + "   int error_line = input_stream.getEndLine();");
            GenCodeLine(prefix + "   int error_column = input_stream.getEndColumn();");
            GenCodeLine(prefix + "   String error_after = null;");
            GenCodeLine(prefix + "   " + Options.getBooleanType() + " EOFSeen = false;");
            GenCodeLine(prefix + "   try { input_stream.readChar(); input_stream.backup(1); }");
            GenCodeLine(prefix + "   catch (java.io.IOException e1) {");
            GenCodeLine(prefix + "      EOFSeen = true;");
            GenCodeLine(prefix + "      error_after = curPos <= 1 ? \"\" : input_stream.GetImage();");
            GenCodeLine(prefix + "      if (curChar == '\\n' || curChar == '\\r') {");
            GenCodeLine(prefix + "         error_line++;");
            GenCodeLine(prefix + "         error_column = 0;");
            GenCodeLine(prefix + "      }");
            GenCodeLine(prefix + "      else");
            GenCodeLine(prefix + "         error_column++;");
            GenCodeLine(prefix + "   }");
            GenCodeLine(prefix + "   if (!EOFSeen) {");
            GenCodeLine(prefix + "      input_stream.backup(1);");
            GenCodeLine(prefix + "      error_after = curPos <= 1 ? \"\" : input_stream.GetImage();");
            GenCodeLine(prefix + "   }");
            GenCodeLine(prefix + "   throw new " + errorHandlingClass + "(" +
            "EOFSeen, curLexState, error_line, error_column, error_after, curChar, " + errorHandlingClass + ".LEXICAL_ERROR);");
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

        GenCodeLine(staticString + "void SkipLexicalActions(Token matchedToken)");
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
                    continue Outer;

                GenCodeLine("      case " + i + " :");

                if (initMatch[lexStates[i]] == i && canLoop[lexStates[i]])
                {
                    GenCodeLine("         if (jjmatchedPos == -1)");
                    GenCodeLine("         {");
                    GenCodeLine("            if (jjbeenHere[" + lexStates[i] + "] &&");
                    GenCodeLine("                jjemptyLineNo[" + lexStates[i] + "] == input_stream.getBeginLine() &&");
                    GenCodeLine("                jjemptyColNo[" + lexStates[i] + "] == input_stream.getBeginColumn())");
                    GenCodeLine("               throw new " + errorHandlingClass + "(" +
                        "(\"Error: Bailing out of infinite loop caused by repeated empty string matches " +
                        "at line \" + input_stream.getBeginLine() + \", " +
                    "column \" + input_stream.getBeginColumn() + \".\"), " + errorHandlingClass + ".LOOP_DETECTED);");
                    GenCodeLine("            jjemptyLineNo[" + lexStates[i] + "] = input_stream.getBeginLine();");
                    GenCodeLine("            jjemptyColNo[" + lexStates[i] + "] = input_stream.getBeginColumn();");
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
                    GenCodeLine("        lengthOfMatch = jjstrLiteralImages[" + i + "].Length;");
                }
                else
                {
                    GenCodeLine("(input_stream.GetSuffix(jjimageLen + (lengthOfMatch = jjmatchedPos + 1)));");
                }

                PrintTokenSetup((Token)act.GetActionTokens()[0]);
                ccol = 1;

                for (int j = 0; j < act.GetActionTokens().Count; j++)
                    printToken((Token)act.GetActionTokens()[j]);
                GenCodeLine("");

                break;
            }

            GenCodeLine("         break;");
        }

        GenCodeLine("      default :");
        GenCodeLine("         break;");
        GenCodeLine("   }");
        GenCodeLine("}");
    }

    public void DumpMoreActions()
    {
        Action act;

        GenCodeLine(staticString + "void MoreLexicalActions()");
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
                    continue Outer;

                GenCodeLine("      case " + i + " :");

                if (initMatch[lexStates[i]] == i && canLoop[lexStates[i]])
                {
                    GenCodeLine("         if (jjmatchedPos == -1)");
                    GenCodeLine("         {");
                    GenCodeLine("            if (jjbeenHere[" + lexStates[i] + "] &&");
                    GenCodeLine("                jjemptyLineNo[" + lexStates[i] + "] == input_stream.getBeginLine() &&");
                    GenCodeLine("                jjemptyColNo[" + lexStates[i] + "] == input_stream.getBeginColumn())");
                    GenCodeLine("               throw new " + errorHandlingClass + "(" +
                        "(\"Error: Bailing out of infinite loop caused by repeated empty string matches " +
                        "at line \" + input_stream.getBeginLine() + \", " +
                    "column \" + input_stream.getBeginColumn() + \".\"), " + errorHandlingClass + ".LOOP_DETECTED);");
                    GenCodeLine("            jjemptyLineNo[" + lexStates[i] + "] = input_stream.getBeginLine();");
                    GenCodeLine("            jjemptyColNo[" + lexStates[i] + "] = input_stream.getBeginColumn();");
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
                    GenCodeLine("(input_stream.GetSuffix(jjimageLen));");

                GenCodeLine("         jjimageLen = 0;");
                PrintTokenSetup((Token)act.GetActionTokens()[0]);
                ccol = 1;

                for (int j = 0; j < act.GetActionTokens().Count; j++)
                    printToken((Token)act.GetActionTokens()[j]);
                GenCodeLine("");

                break;
            }

            GenCodeLine("         break;");
        }

        GenCodeLine("      default :");
        GenCodeLine("         break;");

        GenCodeLine("   }");
        GenCodeLine("}");
    }

    public void DumpTokenActions()
    {
        Action act;
        int i;

        GenCodeLine(staticString + "void TokenLexicalActions(Token matchedToken)");
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
                    continue Outer;

                GenCodeLine("      case " + i + " :");

                if (initMatch[lexStates[i]] == i && canLoop[lexStates[i]])
                {
                    GenCodeLine("         if (jjmatchedPos == -1)");
                    GenCodeLine("         {");
                    GenCodeLine("            if (jjbeenHere[" + lexStates[i] + "] &&");
                    GenCodeLine("                jjemptyLineNo[" + lexStates[i] + "] == input_stream.getBeginLine() &&");
                    GenCodeLine("                jjemptyColNo[" + lexStates[i] + "] == input_stream.getBeginColumn())");
                    GenCodeLine("               throw new " + errorHandlingClass + "(" +
                        "(\"Error: Bailing out of infinite loop caused by repeated empty string matches " +
                        "at line \" + input_stream.getBeginLine() + \", " +
                    "column \" + input_stream.getBeginColumn() + \".\"), " + errorHandlingClass + ".LOOP_DETECTED);");
                    GenCodeLine("            jjemptyLineNo[" + lexStates[i] + "] = input_stream.getBeginLine();");
                    GenCodeLine("            jjemptyColNo[" + lexStates[i] + "] = input_stream.getBeginColumn();");
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
                        GenCodeLine("        lengthOfMatch = jjstrLiteralImages[" + i + "].Length;");
                    }
                    else
                    {
                        GenCodeLine("(input_stream.GetSuffix(jjimageLen + (lengthOfMatch = jjmatchedPos + 1)));");
                    }
                }

                PrintTokenSetup((Token)act.GetActionTokens()[0]);
                ccol = 1;

                for (int j = 0; j < act.GetActionTokens().Count; j++)
                    printToken((Token)act.GetActionTokens()[j]);
                GenCodeLine("");

                break;
            }

            GenCodeLine("         break;");
        }

        GenCodeLine("      default :");
        GenCodeLine("         break;");
        GenCodeLine("   }");
        GenCodeLine("}");
    }

    public static void reInit()
    {
        actions = null;
        allTpsForState = new Dictionary();
        canLoop = null;
        canMatchAnyChar = null;
        canReachOnMore = null;
        curKind = 0;
        curRE = null;
        defaultLexState = 0;
        errorHandlingClass = null;
        hasEmptyMatch = false;
        hasLoop = false;
        hasMore = false;
        hasMoreActions = false;
        hasNfa = null;
        hasSkip = false;
        hasSkipActions = false;
        hasSpecial = false;
        hasTokenActions = false;
        ignoreCase = null;
        initMatch = null;
        initStates = new Dictionary();
        initialState = null;
        keepLineCol = false;
        kinds = null;
        lexStateIndex = 0;
        lexStateName = null;
        lexStateSuffix = null;
        lexStates = null;
        maxLexStates = 0;
        maxLongsReqd = null;
        maxOrdinal = 1;
        mixed = null;
        newLexState = null;
        rexprs = null;
        singlesToSkip = null;
        stateHasActions = null;
        stateSetSize = 0;
        staticString = null;
        toMore = null;
        toSkip = null;
        toSpecial = null;
        toToken = null;
        tokMgrClassName = null;
        tokenizerData = new TokenizerData();
        generateDataOnly = false;
    }

}
