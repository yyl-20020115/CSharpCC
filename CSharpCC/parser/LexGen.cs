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
using CSharpCC.Utils;
using System.Text;

namespace CSharpCC.Parser;

/**
 * Generate lexer.
 */
public class LexGen : CodeGenerator
{
    private static readonly string DUMP_STATIC_VAR_DECLARATIONS_TEMPLATE_RESOURCE_URL = "/templates/DumpStaticVarDeclarations.template";
    private static readonly string DUMP_DEBUG_METHODS_TEMPLATE_RESOURCE_URL = "/templates/DumpDebugMethods.template";
    private static readonly string BOILERPLATER_METHOD_RESOURCE_URL = "/templates/TokenManagerBoilerPlateMethods.template";

    public static string StaticString;
    public static string TokMgrClassName;

    // Dictionary of vectors
    public static Dictionary<string, List<TokenProduction>> AllTpsForState = new();
    public static int LexStateIndex = 0;
    public static int[] Kinds;
    public static int MaxOrdinal = 1;
    public static string LexStateSuffix;
    public static string[] NewLexState;
    public static int[] LexStates;
    public static bool[] IgnoreCase;
    public static Action[] Actions;
    public static Dictionary<string, NfaState> InitStates = new();
    public static int StateSetSize;
    public static int TotalNumStates;
    public static int MaxLexStates;
    public static string[] LexStateName;
    public static NfaState[] SinglesToSkip;
    public static ulong[] ToSkip;
    public static ulong[] ToSpecial;
    public static ulong[] ToMore;
    public static ulong[] ToToken;
    public static int DefaultLexState;
    public static RegularExpression[] Rexprs;
    public static int[] MaxLongsReqd;
    public static int[] InitMatch;
    public static int[] CanMatchAnyChar;
    public static bool HasEmptyMatch;
    public static bool[] CanLoop;
    public static bool[] StateHasActions;
    public static bool HasLoop = false;
    public static bool[] CanReachOnMore;
    public static bool[] HasNfa;
    public static bool[] Mixed;
    public static NfaState InitialState;
    public static int CurKind;
    public static bool HasSkipActions = false;
    public static bool HasMoreActions = false;
    public static bool HasTokenActions = false;
    public static bool HasSpecial = false;
    public static bool HasSkip = false;
    public static bool HasMore = false;
    public static RegularExpression CurRE;
    public static bool KeepLineCol;
    public static string ErrorHandlingClass;
    public static TokenizerData TokenizerData;
    public static bool GenerateDataOnly;

    void PrintClassHead()
    {
        int i, j;
        bool bHasImport = false;

        List<string> tn = new(ToolNames);
        tn.Add(ToolName);
        // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
        GenCodeLine("/* " + GetIdString(tn, TokMgrClassName + GetFileExtension(Options.GetOutputLanguage())) + " */");

        int l = 0, kind;
        i = 1;
        for (; ; )
        {
            if (CuToInsertionPoint1.Count <= l)
                break;

            kind = CuToInsertionPoint1[l].kind;
            if (kind == PACKAGE || kind == IMPORT)
            {
                if (kind == IMPORT)
                    bHasImport = true;
                for (; i < CuToInsertionPoint1.Count; i++)
                {
                    kind = CuToInsertionPoint1[i].kind;
                    if (kind == SEMICOLON ||
                        kind == ABSTRACT ||
                        kind == FINAL ||
                        kind == PUBLIC ||
                        kind == CLASS ||
                        kind == INTERFACE)
                    {
                        cline = CuToInsertionPoint1[l].beginLine;
                        ccol = CuToInsertionPoint1[l].beginColumn;
                        for (j = l; j < i; j++)
                        {
                            PrintToken(CuToInsertionPoint1[j]);
                        }
                        if (kind == SEMICOLON)
                            PrintToken(CuToInsertionPoint1[j]);
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

        if (bHasImport && Options.GetGenerateAnnotations())
        {
            GenCodeLine("@SuppressWarnings (\"unused\")");
        }
        if (Options.GetSupportClassVisibilityPublic())
        {
            //genModifier("public ");
            GenModifier("public ");
        }
        //GenCodeLine("class " + tokMgrClassName + " implements " +
        //cu_name + "Constants");
        //String superClass = Options.stringValue(Options.USEROPTION__TOKEN_MANAGER_SUPER_CLASS);
        GenClassStart(null, TokMgrClassName, Array.Empty<string>(), new String[] { cu_name + "Constants" });
        //GenCodeLine("{"); // }

        if (TokenManagerDeclarations != null && TokenManagerDeclarations.Count > 0)
        {
            var t = TokenManagerDeclarations[0];
            bool commonTokenActionSeen = false;
            bool commonTokenActionNeeded = Options.GetCommonTokenAction();

            PrintTokenSetup(TokenManagerDeclarations[0]);
            ccol = 1;

            for (j = 0; j < TokenManagerDeclarations.Count; j++)
            {
                t = TokenManagerDeclarations[j];
                if (t.kind == IDENTIFIER &&
                    commonTokenActionNeeded &&
                    !commonTokenActionSeen)
                    commonTokenActionSeen = t.image == ("CommonTokenAction");

                PrintToken(t);
            }

            GenCodeLine("");
            if (commonTokenActionNeeded && !commonTokenActionSeen)
                CSharpCCErrors.Warning("You have the COMMON_TOKEN_ACTION option set. " +
                    "But it appears you have not defined the method :\n" +
                    "      " + StaticString + "void CommonTokenAction(Token t)\n" +
                "in your TOKEN_MGR_DECLS. The generated token manager will not compile.");

        }
        else if (Options.GetCommonTokenAction())
        {
            CSharpCCErrors.Warning("You have the COMMON_TOKEN_ACTION option set. " +
                "But you have not defined the method :\n" +
                "      " + StaticString + "void CommonTokenAction(Token t)\n" +
            "in your TOKEN_MGR_DECLS. The generated token manager will not compile.");
        }

        GenCodeLine("");
        GenCodeLine("  /** Debug output. */");
        GenCodeLine("  public " + StaticString + " java.io.PrintStream debugStream = System.out;");
        GenCodeLine("  /** Set debug output. */");
        GenCodeLine("  public " + StaticString + " void setDebugStream(java.io.PrintStream ds) { debugStream = ds; }");

        if (Options.GetTokenManagerUsesParser())
        {
            GenCodeLine("");
            GenCodeLine("  public " + cu_name + " parser = null;");
        }
    }


    protected void WriteTemplate(string name, params object[] additionalOptions)
    {
        Dictionary<string, object> options = new(Options.getOptions())
        {
            { "maxOrdinal", (MaxOrdinal) },
            { "maxLexStates", (MaxLexStates) },
            { "hasEmptyMatch", (HasEmptyMatch) },
            { "hasSkip", (HasSkip) },
            { "hasMore", (HasMore) },
            { "hasSpecial", (HasSpecial) },
            { "hasMoreActions", (HasMoreActions) },
            { "hasSkipActions", (HasSkipActions) },
            { "hasTokenActions", (HasTokenActions) },
            { "stateSetSize", StateSetSize },
            { "hasActions", HasMoreActions || HasSkipActions || HasTokenActions },
            { "tokMgrClassName", TokMgrClassName }
        };
        int x = 0;
        foreach (int l in MaxLongsReqd)
            x = Math.Max(x, l);
        options.Add("maxLongs", x);
        options.Add("cu_name", cu_name);

        // options.Add("", .valueOf(maxOrdinal));


        for (int i = 0; i < additionalOptions.Length; i++)
        {
            var o = additionalOptions[i];

            if (o is Dictionary<string, object> d)
            {
                foreach (var p in d)
                {
                    options.Add(p.Key, p.Value);
                }
            }
            else
            {
                if (i == additionalOptions.Length - 1)
                    throw new ArgumentException("Must supply pairs of [name value] args");

                options.Add((String)o, additionalOptions[i + 1]);
                i++;
            }
        }

        var gen = new OutputFileGenerator(name, options);
        var writer = new StringWriter();
        gen.Generate(writer);
        writer.Close();
        GenCode(writer.ToString());
    }

    void DumpDebugMethods()
    {
        WriteTemplate(DUMP_DEBUG_METHODS_TEMPLATE_RESOURCE_URL);
    }

    static void BuildLexStatesTable()
    {
        int i;

        var tmpLexStateName = new string[LexstateI2S.Count];
        foreach (var tp in RegexpList)
        {
            var respecs = tp.respecs;

            for (i = 0; i < tp.lexStates.Length; i++)
            {
                if (!AllTpsForState.TryGetValue(tp.lexStates[i], out var tps))
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
                if (MaxOrdinal <= (re = respecs[i].Rexp).ordinal)
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
        InitStates = new();
        CanMatchAnyChar = new int[MaxLexStates];
        CanLoop = new bool[MaxLexStates];
        StateHasActions = new bool[MaxLexStates];
        LexStateName = new string[MaxLexStates];
        SinglesToSkip = new NfaState[MaxLexStates];
        Array.Copy(tmpLexStateName, 0, LexStateName, 0, MaxLexStates);

        for (i = 0; i < MaxLexStates; i++)
            CanMatchAnyChar[i] = -1;

        HasNfa = new bool[MaxLexStates];
        Mixed = new bool[MaxLexStates];
        MaxLongsReqd = new int[MaxLexStates];
        InitMatch = new int[MaxLexStates];
        NewLexState = new string[MaxOrdinal];
        NewLexState[0] = nextStateForEof;
        HasEmptyMatch = false;
        LexStates = new int[MaxOrdinal];
        IgnoreCase = new bool[MaxOrdinal];
        Rexprs = new RegularExpression[MaxOrdinal];
        RStringLiteral.AllImages = new string[MaxOrdinal];
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

    public void Start()
    {
        if (!Options.GetBuildTokenManager() ||
            Options.GetUserTokenManager() ||
            CSharpCCErrors.ErrorCount > 0)
            return;

        var codeGeneratorClass = Options.GetTokenManagerCodeGenerator();
        KeepLineCol = Options.GetKeepLineColumn();
        ErrorHandlingClass = Options.getTokenMgrErrorClass();
        List<RChoice> choices = new();
        TokenProduction tp;
        int i, j;

        StaticString = (Options.GetStatic() ? "static " : "");
        TokMgrClassName = cu_name + "TokenManager";

        if (!GenerateDataOnly && codeGeneratorClass == null) PrintClassHead();
        BuildLexStatesTable();

        bool ignoring = false;

        foreach (var key in AllTpsForState.Keys)
        {
            int startState = -1;
            NfaState.ReInit();
            RStringLiteral.ReInit();

            LexStateIndex = GetIndex(key);
            LexStateSuffix = "_" + LexStateIndex;
            var allTps = AllTpsForState[key];
            InitStates.Add(key, InitialState = new NfaState());
            ignoring = false;

            SinglesToSkip[LexStateIndex] = new();
            SinglesToSkip[LexStateIndex].dummy = true;

            if (key == "DEFAULT")
                DefaultLexState = LexStateIndex;

            for (i = 0; i < allTps.Count; i++)
            {
                tp = allTps[i];
                int kind = tp.kind;
                bool ignore = tp.ignoreCase;
                var rexps = tp.respecs;

                if (i == 0)
                    ignoring = ignore;

                for (j = 0; j < rexps.Count; j++)
                {
                    var respec = rexps[j];
                    CurRE = respec.Rexp;

                    Rexprs[CurKind = CurRE.ordinal] = CurRE;
                    LexStates[CurRE.ordinal] = LexStateIndex;
                    IgnoreCase[CurRE.ordinal] = ignore;

                    if (CurRE.private_rexp)
                    {
                        Kinds[CurRE.ordinal] = -1;
                        continue;
                    }

                    if (!Options.GetNoDfa() && CurRE is RStringLiteral literal &&
                        literal.image != (""))
                    {
                        literal.GenerateDfa(this, CurRE.ordinal);
                        if (i != 0 && !Mixed[LexStateIndex] && ignoring != ignore)
                        {
                            Mixed[LexStateIndex] = true;
                        }
                    }
                    else if (CurRE.CanMatchAnyChar)
                    {
                        if (CanMatchAnyChar[LexStateIndex] == -1 ||
                            CanMatchAnyChar[LexStateIndex] > CurRE.ordinal)
                            CanMatchAnyChar[LexStateIndex] = CurRE.ordinal;
                    }
                    else
                    {
                        Nfa tnfa;

                        if (CurRE is RChoice rc)
                            choices.Add(rc);

                        tnfa = CurRE.GenerateNfa(ignore);
                        tnfa.End.isFinal = true;
                        tnfa.End.kind = CurRE.ordinal;
                        InitialState.AddMove(tnfa.Start);
                    }

                    if (Kinds.Length < CurRE.ordinal)
                    {
                        int[] tmp = new int[CurRE.ordinal + 1];

                        Array.Copy(Kinds, 0, tmp, 0, Kinds.Length);
                        Kinds = tmp;
                    }
                    //System.out.WriteLine("   ordina : " + curRE.ordinal);

                    Kinds[CurRE.ordinal] = kind;

                    if (respec.NextState != null &&
                        respec.NextState != (LexStateName[LexStateIndex]))
                        NewLexState[CurRE.ordinal] = respec.NextState;

                    if (respec.Act != null && respec.Act.ActionTokens != null &&
                        respec.Act.ActionTokens.Count > 0)
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
                InitialState.epsilonMoves[i].GenerateCode();

            HasNfa[LexStateIndex] = NfaState.generatedStates != 0;
            if (HasNfa[LexStateIndex])
            {
                InitialState.GenerateCode();
                startState = InitialState.GenerateInitMoves(this);
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

            if (GenerateDataOnly || codeGeneratorClass != null)
            {
                RStringLiteral.UpdateStringLiteralData(TotalNumStates, LexStateIndex);
                NfaState.UpdateNfaData(TotalNumStates, startState, LexStateIndex,
                                       CanMatchAnyChar[LexStateIndex]);
            }
            else
            {
                RStringLiteral.DumpDfaCode(this);
                if (HasNfa[LexStateIndex])
                {
                    NfaState.DumpMoveNfa(this);
                }
            }
            TotalNumStates += NfaState.generatedStates;
            if (StateSetSize < NfaState.generatedStates)
                StateSetSize = NfaState.generatedStates;
        }

        for (i = 0; i < choices.Count; i++)
            choices[i].CheckUnmatchability();

        CheckEmptyStringMatch();

        if (GenerateDataOnly || codeGeneratorClass != null)
        {
            TokenizerData.SetParserName(cu_name);
            NfaState.BuildTokenizerData(TokenizerData);
            RStringLiteral.BuildTokenizerData(TokenizerData);
            int[] newLexStateIndices = new int[MaxOrdinal];
            var tokenMgrDecls = new StringBuilder();
            if (TokenManagerDeclarations != null && TokenManagerDeclarations.Count > 0)
            {
                Token token = TokenManagerDeclarations[0];
                for (j = 0; j < TokenManagerDeclarations.Count; j++)
                {
                    tokenMgrDecls.Append(TokenManagerDeclarations[j].image + " ");
                }
            }
            TokenizerData.SetDecls(tokenMgrDecls.ToString());
            var actionStrings = new Dictionary<int, String>();
            for (i = 0; i < MaxOrdinal; i++)
            {
                if (NewLexState[i] == null)
                {
                    newLexStateIndices[i] = -1;
                }
                else
                {
                    newLexStateIndices[i] = GetIndex(NewLexState[i]);
                }
                // For java, we have this but for other languages, eventually we will
                // simply have a string.
                Action act = Actions[i];
                if (act == null) continue;
                var sb = new StringBuilder();
                for (int k = 0; k < act.ActionTokens.Count; k++)
                {
                    sb.Append(((Token)act.ActionTokens[k]).image);
                    sb.Append(' ');
                }
                actionStrings.Add(i, sb.ToString());
            }
            TokenizerData.SetDefaultLexState(DefaultLexState);
            TokenizerData.SetLexStateNames(LexStateName);
            TokenizerData.UpdateMatchInfo(
                actionStrings, newLexStateIndices,
                ToSkip, ToSpecial, ToMore, ToToken);
            if (GenerateDataOnly) return;
            Type codeGenClazz;
            TokenManagerCodeGenerator gen;
            try
            {
                codeGenClazz = Type.GetType(codeGeneratorClass);
                gen = codeGenClazz.Assembly.CreateInstance(codeGeneratorClass)
                    as TokenManagerCodeGenerator;
            }
            catch (Exception ee)
            {
                CSharpCCErrors.SemanticError(
                    "Could not load the token manager code generator class: " +
                    codeGeneratorClass + "\nError: " + ee.Message);
                return;
            }
            gen.GenerateCode(TokenizerData);
            gen.Finish(TokenizerData);
            return;
        }

        RStringLiteral.DumpStrLiteralImages(this);
        DumpFillToken();
        NfaState.DumpStateSets(this);
        NfaState.DumpNonAsciiMoveMethods(this);
        DumpGetNextToken();

        if (Options.GetDebugTokenManager())
        {
            NfaState.DumpStatesForKind(this);
            DumpDebugMethods();
        }

        if (HasLoop)
        {
            GenCodeLine(StaticString + "int[] jjemptyLineNo = new int[" + MaxLexStates + "];");
            GenCodeLine(StaticString + "int[] jjemptyColNo = new int[" + MaxLexStates + "];");
            GenCodeLine(StaticString + "" + Options.GetBooleanType() + "[] jjbeenHere = new " + Options.GetBooleanType() + "[" + MaxLexStates + "];");
        }

        DumpSkipActions();
        DumpMoreActions();
        DumpTokenActions();

        NfaState.PrintBoilerPlate(this);

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

        WriteTemplate(BOILERPLATER_METHOD_RESOURCE_URL,
          "charStreamName", charStreamName,
          "lexStateNameLength", LexStateName.Length,
          "defaultLexState", DefaultLexState,
          "noDfa", Options.GetNoDfa(),
          "generatedStates", TotalNumStates);

        DumpStaticVarDeclarations(charStreamName);
        GenCodeLine(/*{*/ "}");

        // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
        var fileName = Options.GetOutputDirectory() + Path.DirectorySeparatorChar +
                          TokMgrClassName +
                          GetFileExtension(Options.GetOutputLanguage());

        if (Options.GetBuildParser())
        {
            SaveOutput(fileName);
        }
    }

    public static void CheckEmptyStringMatch()
    {
        int i, j, k, len;
        bool[] seen = new bool[MaxLexStates];
        bool[] done = new bool[MaxLexStates];
        string cycle;
        string reList;

    Outer:
        for (i = 0; i < MaxLexStates; i++)
        {
            if (done[i] || InitMatch[i] == 0 || InitMatch[i] == int.MaxValue ||
                CanMatchAnyChar[i] != -1)
                continue;

            done[i] = true;
            len = 0;
            cycle = "";
            reList = "";

            for (k = 0; k < MaxLexStates; k++)
                seen[k] = false;

            j = i;
            seen[i] = true;
            cycle += LexStateName[j] + "-->";
            while (NewLexState[InitMatch[j]] != null)
            {
                cycle += NewLexState[InitMatch[j]];
                if (seen[j = GetIndex(NewLexState[InitMatch[j]])])
                    break;

                cycle += "-->";
                done[j] = true;
                seen[j] = true;
                if (InitMatch[j] == 0 || InitMatch[j] == int.MaxValue ||
                    CanMatchAnyChar[j] != -1)
                    goto OuterExit;
                if (len != 0)
                    reList += "; ";
                reList += "line " + Rexprs[InitMatch[j]].Line + ", column " +
                Rexprs[InitMatch[j]].Column;
                len++;
            }

            if (NewLexState[InitMatch[j]] == null)
                cycle += LexStateName[LexStates[InitMatch[j]]];

            for (k = 0; k < MaxLexStates; k++)
                CanLoop[k] |= seen[k];

            HasLoop = true;
            if (len == 0)
                CSharpCCErrors.Warning(Rexprs[InitMatch[i]],
                    "Regular expression" + ((Rexprs[InitMatch[i]].label == (""))
                        ? "" : (" for " + Rexprs[InitMatch[i]].label)) +
                        " can be matched by the empty string (\"\") in lexical state " +
                        LexStateName[i] + ". This can result in an endless loop of " +
                "empty string matches.");
            else
            {
                CSharpCCErrors.Warning(Rexprs[InitMatch[i]],
                    "Regular expression" + ((Rexprs[InitMatch[i]].label == (""))
                        ? "" : (" for " + Rexprs[InitMatch[i]].label)) +
                        " can be matched by the empty string (\"\") in lexical state " +
                        LexStateName[i] + ". This regular expression along with the " +
                        "regular expressions at " + reList + " forms the cycle \n   " +
                        cycle + "\ncontaining regular expressions with empty matches." +
                " This can result in an endless loop of empty string matches.");
            }
        OuterExit:
            ;
        }
    }

    private void DumpStaticVarDeclarations(string charStreamName)
    {
        int i;

        GenCodeLine("");
        GenCodeLine("/** Lexer state names. */");
        GenCodeLine("public static final String[] lexStateNames = {");
        for (i = 0; i < MaxLexStates; i++)
            GenCodeLine("   \"" + LexStateName[i] + "\",");
        GenCodeLine("};");

        {
            GenCodeLine("");
            GenCodeLine("/** Lex State array. */");
            GenCode("public static final int[] jjnewLexState = {");

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

        {
            // Bit vector for TOKEN
            GenCode("static final long[] jjtoToken = {");
            for (i = 0; i < MaxOrdinal / 64 + 1; i++)
            {
                if (i % 4 == 0)
                    GenCode("\n   ");
                GenCode("0x" + Convert.ToString((long)ToToken[i], 16) + "L, ");
            }
            GenCodeLine("\n};");
        }

        {
            // Bit vector for SKIP
            GenCode("static final long[] jjtoSkip = {");
            for (i = 0; i < MaxOrdinal / 64 + 1; i++)
            {
                if (i % 4 == 0)
                    GenCode("\n   ");
                GenCode("0x" + Convert.ToString((long)ToSkip[i], 16) + "L, ");
            }
            GenCodeLine("\n};");
        }

        {
            // Bit vector for SPECIAL
            GenCode("static final long[] jjtoSpecial = {");
            for (i = 0; i < MaxOrdinal / 64 + 1; i++)
            {
                if (i % 4 == 0)
                    GenCode("\n   ");
                GenCode("0x" + Convert.ToString((long)ToSpecial[i], 16) + "L, ");
            }
            GenCodeLine("\n};");
        }

        {
            // Bit vector for MORE
            GenCode("static final long[] jjtoMore = {");
            for (i = 0; i < MaxOrdinal / 64 + 1; i++)
            {
                if (i % 4 == 0)
                    GenCode("\n   ");
                GenCode("0x" + Convert.ToString((long)ToMore[i], 16) + "L, ");
            }
            GenCodeLine("\n};");
        }

        WriteTemplate(DUMP_STATIC_VAR_DECLARATIONS_TEMPLATE_RESOURCE_URL,
          "charStreamName", charStreamName,
          "protected", IsJavaLanguage() ? "protected" : "",
          "private", IsJavaLanguage() ? "private" : "",
          "final", IsJavaLanguage() ? "final" : "",
          "lexStateNameLength", LexStateName.Length);
    }

    // Assumes l != 0L
    public static char MaxChar(long l)
    {
        for (int i = 64; i-- > 0;)
            if ((l & (1L << i)) != 0L)
                return (char)i;

        return '\uffff';
    }

    void DumpFillToken()
    {
        double tokenVersion = CSharpFiles.GetVersion("Token.java");
        bool hasBinaryNewToken = tokenVersion > 4.09;

        GenCodeLine(StaticString + "protected Token jjFillToken()");
        GenCodeLine("{");
        GenCodeLine("   final Token t;");
        GenCodeLine("   final String curTokenImage;");
        if (KeepLineCol)
        {
            GenCodeLine("   final int beginLine;");
            GenCodeLine("   final int endLine;");
            GenCodeLine("   final int beginColumn;");
            GenCodeLine("   final int endColumn;");
        }

        if (HasEmptyMatch)
        {
            GenCodeLine("   if (jjmatchedPos < 0)");
            GenCodeLine("   {");
            GenCodeLine("      if (image == null)");
            GenCodeLine("         curTokenImage = \"\";");
            GenCodeLine("      else");
            GenCodeLine("         curTokenImage = image.toString();");

            if (KeepLineCol)
            {
                GenCodeLine("      beginLine = endLine = input_stream.getEndLine();");
                GenCodeLine("      beginColumn = endColumn = input_stream.getEndColumn();");
            }

            GenCodeLine("   }");
            GenCodeLine("   else");
            GenCodeLine("   {");
            GenCodeLine("      String im = jjstrLiteralImages[jjmatchedKind];");
            GenCodeLine("      curTokenImage = (im == null) ? input_stream.GetImage() : im;");

            if (KeepLineCol)
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
            if (KeepLineCol)
            {
                GenCodeLine("   beginLine = input_stream.getBeginLine();");
                GenCodeLine("   beginColumn = input_stream.getBeginColumn();");
                GenCodeLine("   endLine = input_stream.getEndLine();");
                GenCodeLine("   endColumn = input_stream.getEndColumn();");
            }
        }

        if (Options.GetTokenFactory().Length > 0)
        {
            GenCodeLine("   t = " + Options.GetTokenFactory() + ".newToken(jjmatchedKind, curTokenImage);");
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

        if (KeepLineCol)
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
        GenCodeLine(StaticString + "int curLexState = " + DefaultLexState + ";");
        GenCodeLine(StaticString + "int defaultLexState = " + DefaultLexState + ";");
        GenCodeLine(StaticString + "int jjnewStateCnt;");
        GenCodeLine(StaticString + "int jjround;");
        GenCodeLine(StaticString + "int jjmatchedPos;");
        GenCodeLine(StaticString + "int jjmatchedKind;");
        GenCodeLine("");
        GenCodeLine("/** Get the next Token. */");
        GenCodeLine("public " + StaticString + "Token getNextToken()" +
        " ");
        GenCodeLine("{");
        if (HasSpecial)
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

        if (Options.GetDebugTokenManager())
            GenCodeLine("      debugStream.println(\"Returning the <EOF> token.\\n\");");

        GenCodeLine("      jjmatchedKind = 0;");
        GenCodeLine("      jjmatchedPos = -1;");
        GenCodeLine("      matchedToken = jjFillToken();");

        if (HasSpecial)
            GenCodeLine("      matchedToken.specialToken = specialToken;");

        if (nextStateForEof != null || actForEof != null)
            GenCodeLine("      TokenLexicalActions(matchedToken);");

        if (Options.GetCommonTokenAction())
            GenCodeLine("      CommonTokenAction(matchedToken);");

        GenCodeLine("      return matchedToken;");
        GenCodeLine("   }");

        if (HasMoreActions || HasSkipActions || HasTokenActions)
        {
            GenCodeLine("   image = jjimage;");
            GenCodeLine("   image.setLength(0);");
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
                GenCodeLine(prefix + "try { input_stream.backup(0);");
                if (SinglesToSkip[i].asciiMoves[0] != 0L &&
                    SinglesToSkip[i].asciiMoves[1] != 0L)
                {
                    GenCodeLine(prefix + "   while ((curChar < 64" + " && (0x" +
                        Convert.ToString((long)(SinglesToSkip[i].asciiMoves[0]), 16) +
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

                if (Options.GetDebugTokenManager())
                {
                    GenCodeLine(prefix + "{");
                    GenCodeLine("      debugStream.println(" +
                        (MaxLexStates > 1 ?
                            "\"<\" + lexStateNames[curLexState] + \">\" + " : "") +
                            "\"Skipping character : \" + " +
                    ErrorHandlingClass + ".addEscapes(String.valueOf(curChar)) + \" (\" + (int)curChar + \")\");");
                }
                GenCodeLine(prefix + "      curChar = input_stream.BeginToken();");

                if (Options.GetDebugTokenManager())
                    GenCodeLine(prefix + "}");

                GenCodeLine(prefix + "}");
                GenCodeLine(prefix + "catch (java.io.IOException e1) { continue EOFLoop; }");
            }

            if (InitMatch[i] != int.MaxValue && InitMatch[i] != 0)
            {
                if (Options.GetDebugTokenManager())
                    GenCodeLine("      debugStream.println(\"   Matched the empty string as \" + tokenImage[" +
                        InitMatch[i] + "] + \" token.\");");

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
                GenCodeLine("      debugStream.println(" +
                    (MaxLexStates > 1 ? "\"<\" + lexStateNames[curLexState] + \">\" + " : "") +
                    "\"Current character : \" + " +
                    ErrorHandlingClass + ".addEscapes(String.valueOf(curChar)) + \" (\" + (int)curChar + \") " +
                "at line \" + input_stream.getEndLine() + \" column \" + input_stream.getEndColumn());");

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
                    GenCodeLine("           debugStream.println(\"   Current character matched as a \" + tokenImage[" +
                        CanMatchAnyChar[i] + "] + \" token.\");");
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
                GenCodeLine(prefix + "         debugStream.println(" +
                "\"   Putting back \" + (curPos - jjmatchedPos - 1) + \" characters into the input stream.\");");
            }

            GenCodeLine(prefix + "         input_stream.backup(curPos - jjmatchedPos - 1);");

            if (Options.GetDebugTokenManager())
                GenCodeLine(prefix + "      }");

            if (Options.GetDebugTokenManager())
            {
                if (Options.GetJavaUnicodeEscape() ||
                    Options.GetUserCharStream())
                    GenCodeLine("    debugStream.println(" +
                        "\"****** FOUND A \" + tokenImage[jjmatchedKind] + \" MATCH " +
                        "(\" + " + ErrorHandlingClass + ".addEscapes(new String(input_stream.GetSuffix(jjmatchedPos + 1))) + " +
                    "\") ******\\n\");");
                else
                    GenCodeLine("    debugStream.println(" +
                        "\"****** FOUND A \" + tokenImage[jjmatchedKind] + \" MATCH " +
                        "(\" + " + ErrorHandlingClass + ".addEscapes(new String(input_stream.GetSuffix(jjmatchedPos + 1))) + " +
                    "\") ******\\n\");");
            }

            if (HasSkip || HasMore || HasSpecial)
            {
                GenCodeLine(prefix + "      if ((jjtoToken[jjmatchedKind >> 6] & " +
                "(1L << (jjmatchedKind & 077))) != 0L)");
                GenCodeLine(prefix + "      {");
            }

            GenCodeLine(prefix + "         matchedToken = jjFillToken();");

            if (HasSpecial)
                GenCodeLine(prefix + "         matchedToken.specialToken = specialToken;");

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

                        GenCodeLine(prefix + "            if (specialToken == null)");
                        GenCodeLine(prefix + "               specialToken = matchedToken;");
                        GenCodeLine(prefix + "            else");
                        GenCodeLine(prefix + "            {");
                        GenCodeLine(prefix + "               matchedToken.specialToken = specialToken;");
                        GenCodeLine(prefix + "               specialToken = (specialToken.next = matchedToken);");
                        GenCodeLine(prefix + "            }");

                        if (HasSkipActions)
                            GenCodeLine(prefix + "            SkipLexicalActions(matchedToken);");

                        GenCodeLine(prefix + "         }");

                        if (HasSkipActions)
                        {
                            GenCodeLine(prefix + "         else");
                            GenCodeLine(prefix + "            SkipLexicalActions(null);");
                        }
                    }
                    else if (HasSkipActions)
                        GenCodeLine(prefix + "         SkipLexicalActions(null);");

                    if (MaxLexStates > 1)
                    {
                        GenCodeLine("         if (jjnewLexState[jjmatchedKind] != -1)");
                        GenCodeLine(prefix + "         curLexState = jjnewLexState[jjmatchedKind];");
                    }

                    GenCodeLine(prefix + "         continue EOFLoop;");
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
                    GenCodeLine(prefix + "      jjmatchedKind = 0x" + Convert.ToString(int.MaxValue, 16) + ";");

                    GenCodeLine(prefix + "      try {");
                    GenCodeLine(prefix + "         curChar = input_stream.readChar();");

                    if (Options.GetDebugTokenManager())
                        GenCodeLine("   debugStream.println(" +
                            (MaxLexStates > 1 ? "\"<\" + lexStateNames[curLexState] + \">\" + " : "") +
                            "\"Current character : \" + " +
                            "" + ErrorHandlingClass + ".addEscapes(String.valueOf(curChar)) + \" (\" + (int)curChar + \") " +
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
            GenCodeLine(prefix + "   " + Options.GetBooleanType() + " EOFSeen = false;");
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
            GenCodeLine(prefix + "   throw new " + ErrorHandlingClass + "(" +
            "EOFSeen, curLexState, error_line, error_column, error_after, curChar, " + ErrorHandlingClass + ".LEXICAL_ERROR);");
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

        GenCodeLine(StaticString + "void SkipLexicalActions(Token matchedToken)");
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
                    act.ActionTokens == null ||
                    act.ActionTokens.Count == 0) && !CanLoop[LexStates[i]])
                    goto OuterExit;

                GenCodeLine("      case " + i + " :");

                if (InitMatch[LexStates[i]] == i && CanLoop[LexStates[i]])
                {
                    GenCodeLine("         if (jjmatchedPos == -1)");
                    GenCodeLine("         {");
                    GenCodeLine("            if (jjbeenHere[" + LexStates[i] + "] &&");
                    GenCodeLine("                jjemptyLineNo[" + LexStates[i] + "] == input_stream.getBeginLine() &&");
                    GenCodeLine("                jjemptyColNo[" + LexStates[i] + "] == input_stream.getBeginColumn())");
                    GenCodeLine("               throw new " + ErrorHandlingClass + "(" +
                        "(\"Error: Bailing out of infinite loop caused by repeated empty string matches " +
                        "at line \" + input_stream.getBeginLine() + \", " +
                    "column \" + input_stream.getBeginColumn() + \".\"), " + ErrorHandlingClass + ".LOOP_DETECTED);");
                    GenCodeLine("            jjemptyLineNo[" + LexStates[i] + "] = input_stream.getBeginLine();");
                    GenCodeLine("            jjemptyColNo[" + LexStates[i] + "] = input_stream.getBeginColumn();");
                    GenCodeLine("            jjbeenHere[" + LexStates[i] + "] = true;");
                    GenCodeLine("         }");
                }

                if ((act = (Action)Actions[i]) == null ||
                    act.ActionTokens.Count == 0)
                    break;

                GenCode("         image.append");
                if (RStringLiteral.AllImages[i] != null)
                {
                    GenCodeLine("(jjstrLiteralImages[" + i + "]);");
                    GenCodeLine("        lengthOfMatch = jjstrLiteralImages[" + i + "].Length;");
                }
                else
                {
                    GenCodeLine("(input_stream.GetSuffix(jjimageLen + (lengthOfMatch = jjmatchedPos + 1)));");
                }

                PrintTokenSetup((Token)act.ActionTokens[0]);
                ccol = 1;

                for (int j = 0; j < act.ActionTokens.Count; j++)
                    PrintToken((Token)act.ActionTokens[j]);
                GenCodeLine("");

                break;
            }

            GenCodeLine("         break;");
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

        GenCodeLine(StaticString + "void MoreLexicalActions()");
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
                    act.ActionTokens == null ||
                    act.ActionTokens.Count == 0) && !CanLoop[LexStates[i]])
                    goto OuterExit;

                GenCodeLine("      case " + i + " :");

                if (InitMatch[LexStates[i]] == i && CanLoop[LexStates[i]])
                {
                    GenCodeLine("         if (jjmatchedPos == -1)");
                    GenCodeLine("         {");
                    GenCodeLine("            if (jjbeenHere[" + LexStates[i] + "] &&");
                    GenCodeLine("                jjemptyLineNo[" + LexStates[i] + "] == input_stream.getBeginLine() &&");
                    GenCodeLine("                jjemptyColNo[" + LexStates[i] + "] == input_stream.getBeginColumn())");
                    GenCodeLine("               throw new " + ErrorHandlingClass + "(" +
                        "(\"Error: Bailing out of infinite loop caused by repeated empty string matches " +
                        "at line \" + input_stream.getBeginLine() + \", " +
                    "column \" + input_stream.getBeginColumn() + \".\"), " + ErrorHandlingClass + ".LOOP_DETECTED);");
                    GenCodeLine("            jjemptyLineNo[" + LexStates[i] + "] = input_stream.getBeginLine();");
                    GenCodeLine("            jjemptyColNo[" + LexStates[i] + "] = input_stream.getBeginColumn();");
                    GenCodeLine("            jjbeenHere[" + LexStates[i] + "] = true;");
                    GenCodeLine("         }");
                }

                if ((act = (Action)Actions[i]) == null ||
                    act.ActionTokens.Count == 0)
                {
                    break;
                }

                GenCode("         image.append");

                if (RStringLiteral.AllImages[i] != null)
                    GenCodeLine("(jjstrLiteralImages[" + i + "]);");
                else
                    GenCodeLine("(input_stream.GetSuffix(jjimageLen));");

                GenCodeLine("         jjimageLen = 0;");
                PrintTokenSetup((Token)act.ActionTokens[0]);
                ccol = 1;

                for (int j = 0; j < act.ActionTokens.Count; j++)
                    PrintToken((Token)act.ActionTokens[j]);
                GenCodeLine("");

                break;
            }

            GenCodeLine("         break;");
        OuterExit:
            ;
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

        GenCodeLine(StaticString + "void TokenLexicalActions(Token matchedToken)");
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
                    act.ActionTokens == null ||
                    act.ActionTokens.Count == 0) && !CanLoop[LexStates[i]])
                    goto OuterExit;

                GenCodeLine("      case " + i + " :");

                if (InitMatch[LexStates[i]] == i && CanLoop[LexStates[i]])
                {
                    GenCodeLine("         if (jjmatchedPos == -1)");
                    GenCodeLine("         {");
                    GenCodeLine("            if (jjbeenHere[" + LexStates[i] + "] &&");
                    GenCodeLine("                jjemptyLineNo[" + LexStates[i] + "] == input_stream.getBeginLine() &&");
                    GenCodeLine("                jjemptyColNo[" + LexStates[i] + "] == input_stream.getBeginColumn())");
                    GenCodeLine("               throw new " + ErrorHandlingClass + "(" +
                        "(\"Error: Bailing out of infinite loop caused by repeated empty string matches " +
                        "at line \" + input_stream.getBeginLine() + \", " +
                    "column \" + input_stream.getBeginColumn() + \".\"), " + ErrorHandlingClass + ".LOOP_DETECTED);");
                    GenCodeLine("            jjemptyLineNo[" + LexStates[i] + "] = input_stream.getBeginLine();");
                    GenCodeLine("            jjemptyColNo[" + LexStates[i] + "] = input_stream.getBeginColumn();");
                    GenCodeLine("            jjbeenHere[" + LexStates[i] + "] = true;");
                    GenCodeLine("         }");
                }

                if ((act = Actions[i]) == null ||
                    act.ActionTokens.Count == 0)
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
                        GenCodeLine("        lengthOfMatch = jjstrLiteralImages[" + i + "].Length;");
                    }
                    else
                    {
                        GenCodeLine("(input_stream.GetSuffix(jjimageLen + (lengthOfMatch = jjmatchedPos + 1)));");
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
        OuterExit:
            ;
        }

        GenCodeLine("      default :");
        GenCodeLine("         break;");
        GenCodeLine("   }");
        GenCodeLine("}");
    }

    public static new void ReInit()
    {
        Actions = null;
        AllTpsForState = new();
        CanLoop = null;
        CanMatchAnyChar = null;
        CanReachOnMore = null;
        CurKind = 0;
        CurRE = null;
        DefaultLexState = 0;
        ErrorHandlingClass = null;
        HasEmptyMatch = false;
        HasLoop = false;
        HasMore = false;
        HasMoreActions = false;
        HasNfa = null;
        HasSkip = false;
        HasSkipActions = false;
        HasSpecial = false;
        HasTokenActions = false;
        IgnoreCase = null;
        InitMatch = null;
        InitStates = new();
        InitialState = null;
        KeepLineCol = false;
        Kinds = null;
        LexStateIndex = 0;
        LexStateName = null;
        LexStateSuffix = null;
        LexStates = null;
        MaxLexStates = 0;
        MaxLongsReqd = null;
        MaxOrdinal = 1;
        Mixed = null;
        NewLexState = null;
        Rexprs = null;
        SinglesToSkip = null;
        StateHasActions = null;
        StateSetSize = 0;
        StaticString = null;
        ToMore = null;
        ToSkip = null;
        ToSpecial = null;
        ToToken = null;
        TokMgrClassName = null;
        TokenizerData = new TokenizerData();
        GenerateDataOnly = false;
    }

}
