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

using System.Text;

namespace org.javacc.parser;


public class KindInfo
{
    public long[] validKinds;
    public long[] finalKinds;
    public int validKindCnt = 0;
    public int finalKindCnt = 0;
    public HashSet<int> finalKindSet = new ();
    public HashSet<int> validKindSet = new ();

    public KindInfo(int maxKind)
    {
        validKinds = new long[maxKind / 64 + 1];
        finalKinds = new long[maxKind / 64 + 1];
    }

    public void InsertValidKind(int kind)
    {
        validKinds[kind / 64] |= (1L << (kind % 64));
        validKindCnt++;
        validKindSet.Add(kind);
    }

    public void InsertFinalKind(int kind)
    {
        finalKinds[kind / 64] |= (1L << (kind % 64));
        finalKindCnt++;
        finalKindSet.Add(kind);
    }
};

/**
 * Describes string literals.
 */

public class RStringLiteral : RegularExpression
{

    /**
     * The string image of the literal.
     */
    public string image;

    public RStringLiteral()
    {
    }

    public RStringLiteral(Token t, string image)
    {
        this.setLine(t.beginLine);
        this.setColumn(t.beginColumn);
        this.image = image;
    }

    private static int maxStrKind = 0;
    private static int maxLen = 0;
    private static int charCnt = 0;
    private static List charPosKind = new(); // Elements are hashtables
                                             // with single char keys;
    private static int[] maxLenForActive = new int[100]; // 6400 tokens
    public static String[] allImages;
    private static int[][] intermediateKinds;
    private static int[][] intermediateMatchedPos;

    private static int startStateCnt = 0;
    private static bool[] subString;
    private static bool[] subStringAtPos;
    private static Dictionary[] statesForPos;

    /**
     * Initialize all the static variables, so that there is no interference
     * between the various states of the lexer.
     *
     * Need to call this method after generating code for each lexical state.
     */
    public static void ReInit()
    {
        maxStrKind = 0;
        maxLen = 0;
        charPosKind = new ();
        maxLenForActive = new int[100]; // 6400 tokens
        intermediateKinds = null;
        intermediateMatchedPos = null;
        startStateCnt = 0;
        subString = null;
        subStringAtPos = null;
        statesForPos = null;
    }

    public static void DumpStrLiteralImages(CodeGenerator codeGenerator)
    {
        // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
        if (Options.isOutputLanguageJava())
        {
            DumpStrLiteralImagesForJava(codeGenerator);
            return;
        }
        else if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
        {

            // For C++
            string image;
            int i;
            charCnt = 0; // Set to zero in reInit() but just to be sure

            codeGenerator.genCodeLine("");
            codeGenerator.genCodeLine("/** Token literal values. */");
            int literalCount = 0;
            codeGenerator.switchToStaticsFile();

            if (allImages == null || allImages.Length == 0)
            {
                codeGenerator.genCodeLine("static const JJString jjstrLiteralImages[] = {};");
                return;
            }

            allImages[0] = "";
            for (i = 0; i < allImages.Length; i++)
            {
                if ((image = allImages[i]) == null ||
                    ((Main.lg.toSkip[i / 64] & (1L << (i % 64))) == 0L &&
                     (Main.lg.toMore[i / 64] & (1L << (i % 64))) == 0L &&
                     (Main.lg.toToken[i / 64] & (1L << (i % 64))) == 0L) ||
                    (Main.lg.toSkip[i / 64] & (1L << (i % 64))) != 0L ||
                    (Main.lg.toMore[i / 64] & (1L << (i % 64))) != 0L ||
                    Main.lg.canReachOnMore[Main.lg.lexStates[i]] ||
                    ((Options.getIgnoreCase() || Main.lg.ignoreCase[i]) &&
                     (!image == (image.ToLower(Locale.ENGLISH)) ||
                      !image == (image.ToUpper(Locale.ENGLISH)))))
                {
                    allImages[i] = null;
                    if ((charCnt += 6) > 80)
                    {
                        codeGenerator.genCodeLine("");
                        charCnt = 0;
                    }

                    codeGenerator.genCodeLine("static JJChar jjstrLiteralChars_"
                        + literalCount++ + "[] = {0};");
                    continue;
                }

                string toPrint = "static JJChar jjstrLiteralChars_" +
                                     literalCount++ + "[] = {";
                for (int j = 0; j < image.Length; j++)
                {
                    string hexVal = int.toHexString((int)image.charAt(j));
                    toPrint += "0x" + hexVal + ", ";
                }

                // Null char
                toPrint += "0};";

                if ((charCnt += toPrint.Length) >= 80)
                {
                    codeGenerator.genCodeLine("");
                    charCnt = 0;
                }

                codeGenerator.genCodeLine(toPrint);
            }

            while (++i < Main.lg.maxOrdinal)
            {
                if ((charCnt += 6) > 80)
                {
                    codeGenerator.genCodeLine("");
                    charCnt = 0;
                }

                codeGenerator.genCodeLine("static JJChar jjstrLiteralChars_" +
                                           literalCount++ + "[] = {0};");
                continue;
            }

            // Generate the array here.
            codeGenerator.genCodeLine("static const JJString " +
                                      "jjstrLiteralImages[] = {");
            for (int j = 0; j < literalCount; j++)
            {
                codeGenerator.genCodeLine("jjstrLiteralChars_" + j + ", ");
            }
            codeGenerator.genCodeLine("};");
        }
        else
        {
            throw new Exception("Output language type not fully implemented : " + Options.getOutputLanguage());
        }
    }

    public static void DumpStrLiteralImagesForJava(CodeGenerator codeGenerator)
    {
        string image;
        int i;
        charCnt = 0; // Set to zero in reInit() but just to be sure

        codeGenerator.genCodeLine("");
        codeGenerator.genCodeLine("/** Token literal values. */");
        codeGenerator.genCodeLine("public static readonly String[] jjstrLiteralImages = {");

        if (allImages == null || allImages.Length == 0)
        {
            codeGenerator.genCodeLine("};");
            return;
        }

        allImages[0] = "";
        for (i = 0; i < allImages.Length; i++)
        {
            if ((image = allImages[i]) == null ||
                ((Main.lg.toSkip[i / 64] & (1L << (i % 64))) == 0L &&
                 (Main.lg.toMore[i / 64] & (1L << (i % 64))) == 0L &&
                 (Main.lg.toToken[i / 64] & (1L << (i % 64))) == 0L) ||
                (Main.lg.toSkip[i / 64] & (1L << (i % 64))) != 0L ||
                (Main.lg.toMore[i / 64] & (1L << (i % 64))) != 0L ||
                Main.lg.canReachOnMore[Main.lg.lexStates[i]] ||
                ((Options.getIgnoreCase() || Main.lg.ignoreCase[i]) &&
                 (!image == (image.ToLower(Locale.ENGLISH)) ||
                  !image == (image.ToUpper(Locale.ENGLISH)))))
            {
                allImages[i] = null;
                if ((charCnt += 6) > 80)
                {
                    codeGenerator.genCodeLine("");
                    charCnt = 0;
                }

                codeGenerator.genCode("null, ");
                continue;
            }

            string toPrint = "\"";
            for (int j = 0; j < image.Length; j++)
            {
                if (codeGenerator.isJavaLanguage() && image.charAt(j) <= 0xff)
                    toPrint += ("\\" + int.toOctalString((int)image.charAt(j)));
                else
                {
                    string hexVal = int.toHexString((int)image.charAt(j));
                    if (hexVal.Length == 3)
                        hexVal = "0" + hexVal;
                    toPrint += ("\\u" + hexVal);
                }
            }

            toPrint += ("\", ");

            if ((charCnt += toPrint.Length) >= 80)
            {
                codeGenerator.genCodeLine("");
                charCnt = 0;
            }

            codeGenerator.genCode(toPrint);
        }

        while (++i < Main.lg.maxOrdinal)
        {
            if ((charCnt += 6) > 80)
            {
                codeGenerator.genCodeLine("");
                charCnt = 0;
            }

            codeGenerator.genCode("null, ");
            continue;
        }

        codeGenerator.genCodeLine("};");
    }

    /**
     * Used for top level string literals.
     */
    public override void GenerateDfa(CodeGenerator codeGenerator, int kind)
    {
        string s;
        Dictionary temp;
        KindInfo info;
        int len;

        if (maxStrKind <= ordinal)
            maxStrKind = ordinal + 1;

        if ((len = image.Length) > maxLen)
            maxLen = len;

        char c;
        for (int i = 0; i < len; i++)
        {
            if (Options.getIgnoreCase())
                s = ("" + (c = image[i])).ToLower(Locale.ENGLISH);
            else
                s = "" + (c = image[i]);

            if (!NfaState.unicodeWarningGiven && c > 0xff &&
                !Options.getJavaUnicodeEscape() &&
                !Options.getUserCharStream())
            {
                NfaState.unicodeWarningGiven = true;
                JavaCCErrors.warning(Main.lg.curRE, "Non-ASCII characters used in regular expression." +
                   "Please make sure you use the correct Reader when you create the parser, " +
                   "one that can handle your character set.");
            }

            if (i >= charPosKind.Count) // Kludge, but OK
                charPosKind.Add(temp = new Dictionary());
            else
                temp = (Dictionary)charPosKind[i];

            if ((info = (KindInfo)temp.get(s)) == null)
                temp.Add(s, info = new KindInfo(Main.lg.maxOrdinal));

            if (i + 1 == len)
                info.InsertFinalKind(ordinal);
            else
                info.InsertValidKind(ordinal);

            if (!Options.getIgnoreCase() && Main.lg.ignoreCase[ordinal] &&
                c != char.ToLower(c))
            {
                s = ("" + image[i]).ToLower(Locale.ENGLISH);

                if (i >= charPosKind.Count) // Kludge, but OK
                    charPosKind.Add(temp = new Dictionary());
                else
                    temp = (Dictionary)charPosKind[i];

                if ((info = (KindInfo)temp.get(s)) == null)
                    temp.Add(s, info = new KindInfo(Main.lg.maxOrdinal));

                if (i + 1 == len)
                    info.InsertFinalKind(ordinal);
                else
                    info.InsertValidKind(ordinal);
            }

            if (!Options.getIgnoreCase() && Main.lg.ignoreCase[ordinal] &&
                c != char.ToUpper(c))
            {
                s = ("" + image[i]).ToUpper();

                if (i >= charPosKind.Count) // Kludge, but OK
                    charPosKind.Add(temp = new Dictionary());
                else
                    temp = (Dictionary)charPosKind[i];

                if ((info = (KindInfo)temp.get(s)) == null)
                    temp.Add(s, info = new KindInfo(Main.lg.maxOrdinal));

                if (i + 1 == len)
                    info.InsertFinalKind(ordinal);
                else
                    info.InsertValidKind(ordinal);
            }
        }

        maxLenForActive[ordinal / 64] = Math.Max(maxLenForActive[ordinal / 64],
                                                                           len - 1);
        allImages[ordinal] = image;
    }

    public Nfa GenerateNfa(bool ignoreCase)
    {
        if (image.Length == 1)
        {
            RCharacterList temp = new RCharacterList(image.charAt(0));
            return temp.GenerateNfa(ignoreCase);
        }

        NfaState startState = new NfaState();
        NfaState theStartState = startState;
        NfaState finalState = null;

        if (image.Length == 0)
            return new Nfa(theStartState, theStartState);

        int i;

        for (i = 0; i < image.Length; i++)
        {
            finalState = new NfaState();
            startState.charMoves = new char[1];
            startState.AddChar(image[i]);

            if (Options.getIgnoreCase() || ignoreCase)
            {
                startState.AddChar(char.ToLower(image[i]));
                startState.AddChar(char.ToUpper(image[i]));
            }

            startState.next = finalState;
            startState = finalState;
        }

        return new Nfa(theStartState, finalState);
    }

    static void DumpNullStrLiterals(CodeGenerator codeGenerator)
    {
        codeGenerator.genCodeLine("{");

        if (NfaState.generatedStates != 0)
            codeGenerator.genCodeLine("   return jjMoveNfa" + Main.lg.lexStateSuffix + "(" + NfaState.InitStateName() + ", 0);");
        else
            codeGenerator.genCodeLine("   return 1;");

        codeGenerator.genCodeLine("}");
    }

    private static int GetStateSetForKind(int pos, int kind)
    {
        if (Main.lg.mixed[Main.lg.lexStateIndex] || NfaState.generatedStates == 0)
            return -1;

        Dictionary allStateSets = statesForPos[pos];

        if (allStateSets == null)
            return -1;

        Enumeration e = allStateSets.keys();

        while (e.hasMoreElements())
        {
            string s = (String)e.nextElement();
            long[] actives = (long[])allStateSets.get(s);

            s = s.substring(s.IndexOf(", ") + 2);
            s = s.substring(s.IndexOf(", ") + 2);

            if (s == ("null;"))
                continue;

            if (actives != null &&
                (actives[kind / 64] & (1L << (kind % 64))) != 0L)
            {
                return NfaState.AddStartStateSet(s);
            }
        }

        return -1;
    }

    static string GetLabel(int kind)
    {
        RegularExpression re = Main.lg.rexprs[kind];

        if (re is RStringLiteral)
            return " \"" + JavaCCGlobals.add_escapes(((RStringLiteral)re).image) + "\"";
        else if (!re.label == (""))
            return " <" + re.label + ">";
        else
            return " <token of kind " + kind + ">";
    }

    static int GetLine(int kind)
    {
        return Main.lg.rexprs[kind].getLine();
    }

    static int GetColumn(int kind)
    {
        return Main.lg.rexprs[kind].getColumn();
    }

    /**
     * Returns true if s1 starts with s2 (ignoring case for each character).
     */
    static private bool StartsWithIgnoreCase(string s1, string s2)
    {
        if (s1.Length < s2.Length)
            return false;

        for (int i = 0; i < s2.Length; i++)
        {
            char c1 = s1[i], c2 = s2[i];

            if (c1 != c2 && char.ToLower(c2) != c1 &&
                char.ToUpper(c2) != c1)
                return false;
        }

        return true;
    }

    public static void FillSubString()
    {
        string image;
        subString = new bool[maxStrKind + 1];
        subStringAtPos = new bool[maxLen];

        for (int i = 0; i < maxStrKind; i++)
        {
            subString[i] = false;

            if ((image = allImages[i]) == null ||
                Main.lg.lexStates[i] != Main.lg.lexStateIndex)
                continue;

            if (Main.lg.mixed[Main.lg.lexStateIndex])
            {
                // We will not optimize for mixed case
                subString[i] = true;
                subStringAtPos[image.Length - 1] = true;
                continue;
            }

            for (int j = 0; j < maxStrKind; j++)
            {
                if (j != i && Main.lg.lexStates[j] == Main.lg.lexStateIndex &&
                    ((String)allImages[j]) != null)
                {
                    if (((String)allImages[j]).IndexOf(image) == 0)
                    {
                        subString[i] = true;
                        subStringAtPos[image.Length - 1] = true;
                        break;
                    }
                    else if (Options.getIgnoreCase() &&
                             StartsWithIgnoreCase((String)allImages[j], image))
                    {
                        subString[i] = true;
                        subStringAtPos[image.Length - 1] = true;
                        break;
                    }
                }
            }
        }
    }

    static void DumpStartWithStates(CodeGenerator codeGenerator)
    {
        // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
        if (Options.isOutputLanguageJava())
        {
            codeGenerator.genCodeLine((Options.getStatic() ? "static " : "") + "private int " +
                         "jjStartNfaWithStates" + Main.lg.lexStateSuffix + "(int pos, int kind, int state)");
        }
        else if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
        {
            codeGenerator.generateMethodDefHeader("int", Main.lg.tokMgrClassName, "jjStartNfaWithStates" + Main.lg.lexStateSuffix + "(int pos, int kind, int state)");
        }
        else
        {
            throw new Exception("Output language type not fully implemented : " + Options.getOutputLanguage());
        }
        codeGenerator.genCodeLine("{");
        codeGenerator.genCodeLine("   jjmatchedKind = kind;");
        codeGenerator.genCodeLine("   jjmatchedPos = pos;");

        if (Options.getDebugTokenManager())
        {
            if (codeGenerator.isJavaLanguage())
            {
                codeGenerator.genCodeLine("   debugStream.println(\"   No more string literal token matches are possible.\");");
                codeGenerator.genCodeLine("   debugStream.println(\"   Currently matched the first \" " +
                        "+ (jjmatchedPos + 1) + \" characters as a \" + tokenImage[jjmatchedKind] + \" token.\");");
            }
            else
            {
                codeGenerator.genCodeLine("   fprintf(debugStream, \"   No more string literal token matches are possible.\");");
                codeGenerator.genCodeLine("   fprintf(debugStream, \"   Currently matched the first %d characters as a \\\"%s\\\" token.\\n\",  (jjmatchedPos + 1),  addUnicodeEscapes(tokenImage[jjmatchedKind]).c_str());");
            }
        }

        // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
        if (Options.isOutputLanguageJava())
        {
            codeGenerator.genCodeLine("   try { curChar = input_stream.readChar(); }");
            codeGenerator.genCodeLine("   catch(java.io.IOException e) { return pos + 1; }");
        }
        else if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
        {
            codeGenerator.genCodeLine("   if (input_stream->endOfInput()) { return pos + 1; }");
            codeGenerator.genCodeLine("   curChar = input_stream->readChar();");
        }
        else
        {
            throw new Exception("Output language type not fully implemented : " + Options.getOutputLanguage());
        }
        if (Options.getDebugTokenManager())
        {
            if (codeGenerator.isJavaLanguage())
            {
                codeGenerator.genCodeLine("   debugStream.println(" +
                     (LexGen.maxLexStates > 1 ? "\"<\" + lexStateNames[curLexState] + \">\" + " : "") +
                     "\"Current character : \" + " + Options.getTokenMgrErrorClass() +
                     ".addEscapes(String.valueOf(curChar)) + \" (\" + (int)curChar + \") " +
                     "at line \" + input_stream.getEndLine() + \" column \" + input_stream.getEndColumn());");
            }
            else if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
            {
                codeGenerator.genCodeLine("   fprintf(debugStream, " +
                   "\"<%s>Current character : %c(%d) at line %d column %d\\n\"," +
                   "addUnicodeEscapes(lexStateNames[curLexState]).c_str(), curChar, (int)curChar, " +
                   "input_stream->getEndLine(), input_stream->getEndColumn());");
            }
            else
            {
                throw new Exception("Output language type not fully implemented : " + Options.getOutputLanguage());
            }
        }

        codeGenerator.genCodeLine("   return jjMoveNfa" + Main.lg.lexStateSuffix + "(state, pos + 1);");
        codeGenerator.genCodeLine("}");
    }

    private static bool boilerPlateDumped = false;
    static void DumpBoilerPlate(CodeGenerator codeGenerator)
    {
        // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
        if (Options.isOutputLanguageJava())
        {
            codeGenerator.genCodeLine((Options.getStatic() ? "static " : "") + "private int " +
                         "jjStopAtPos(int pos, int kind)");
        }
        else if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
        {
            codeGenerator.generateMethodDefHeader(" int ", Main.lg.tokMgrClassName, "jjStopAtPos(int pos, int kind)");
        }
        else
        {
            throw new Exception("Output language type not fully implemented : " + Options.getOutputLanguage());
        }
        codeGenerator.genCodeLine("{");
        codeGenerator.genCodeLine("   jjmatchedKind = kind;");
        codeGenerator.genCodeLine("   jjmatchedPos = pos;");

        if (Options.getDebugTokenManager())
        {
            // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
            if (codeGenerator.isJavaLanguage())
            {
                codeGenerator.genCodeLine("   debugStream.println(\"   No more string literal token matches are possible.\");");
                codeGenerator.genCodeLine("   debugStream.println(\"   Currently matched the first \" + (jjmatchedPos + 1) + " +
                       "\" characters as a \" + tokenImage[jjmatchedKind] + \" token.\");");
            }
            else if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
            {
                codeGenerator.genCodeLine("   fprintf(debugStream, \"   No more string literal token matches are possible.\");");
                codeGenerator.genCodeLine("   fprintf(debugStream, \"   Currently matched the first %d characters as a \\\"%s\\\" token.\\n\",  (jjmatchedPos + 1),  addUnicodeEscapes(tokenImage[jjmatchedKind]).c_str());");
            }
            else
            {
                throw new Exception("Output language type not fully implemented : " + Options.getOutputLanguage());
            }
        }

        codeGenerator.genCodeLine("   return pos + 1;");
        codeGenerator.genCodeLine("}");
    }

    static String[] ReArrange(Dictionary tab)
    {
        String[] ret = new String[tab.Count];
        Enumeration e = tab.keys();
        int cnt = 0;

        while (e.hasMoreElements())
        {
            int i = 0, j;
            string s;
            char c = (s = (String)e.nextElement()).charAt(0);

            while (i < cnt && ret[i].charAt(0) < c) i++;

            if (i < cnt)
                for (j = cnt - 1; j >= i; j--)
                    ret[j + 1] = ret[j];

            ret[i] = s;
            cnt++;
        }

        return ret;
    }

    public static void DumpDfaCode(CodeGenerator codeGenerator)
    {
        Dictionary tab;
        string key;
        KindInfo info;
        int maxLongsReqd = maxStrKind / 64 + 1;
        int i, j, k;
        bool ifGenerated;
        Main.lg.maxLongsReqd[Main.lg.lexStateIndex] = maxLongsReqd;

        if (maxLen == 0)
        {
            // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
            if (Options.isOutputLanguageJava())
            {
                codeGenerator.genCodeLine((Options.getStatic() ? "static " : "") + "private int " +
                               "jjMoveStringLiteralDfa0" + Main.lg.lexStateSuffix + "()");
            }
            else if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
            {
                codeGenerator.generateMethodDefHeader(" int ", Main.lg.tokMgrClassName, "jjMoveStringLiteralDfa0" + Main.lg.lexStateSuffix + "()");
            }
            else
            {
                throw new Exception("Output language type not fully implemented : " + Options.getOutputLanguage());
            }
            DumpNullStrLiterals(codeGenerator);
            return;
        }

        if (!boilerPlateDumped)
        {
            DumpBoilerPlate(codeGenerator);
            boilerPlateDumped = true;
        }

        bool createStartNfa = false; ;
        for (i = 0; i < maxLen; i++)
        {
            bool atLeastOne = false;
            bool startNfaNeeded = false;
            tab = (Dictionary)charPosKind[i];
            String[] keys = ReArrange(tab);

            StringBuilder _params = new StringBuilder();
            _params.Append("(");
            if (i != 0)
            {
                if (i == 1)
                {
                    for (j = 0; j < maxLongsReqd - 1; j++)
                        if (i <= maxLenForActive[j])
                        {
                            if (atLeastOne)
                                _params.Append(", ");
                            else
                                atLeastOne = true;
                            _params.Append("" + Options.getLongType() + " active" + j);
                        }

                    if (i <= maxLenForActive[j])
                    {
                        if (atLeastOne)
                            _params.Append(", ");
                        _params.Append("" + Options.getLongType() + " active" + j);
                    }
                }
                else
                {
                    for (j = 0; j < maxLongsReqd - 1; j++)
                        if (i <= maxLenForActive[j] + 1)
                        {
                            if (atLeastOne)
                                _params.Append(", ");
                            else
                                atLeastOne = true;
                            _params.Append("" + Options.getLongType() + " old" + j + ", " + Options.getLongType() + " active" + j);
                        }

                    if (i <= maxLenForActive[j] + 1)
                    {
                        if (atLeastOne)
                            _params.Append(", ");
                        _params.Append("" + Options.getLongType() + " old" + j + ", " + Options.getLongType() + " active" + j);
                    }
                }
            }
            _params.Append(")");

            // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
            if (Options.isOutputLanguageJava())
            {
                codeGenerator.genCode((Options.getStatic() ? "static " : "") + "private int " +
                               "jjMoveStringLiteralDfa" + i + Main.lg.lexStateSuffix + _params);
            }
            else if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
            {
                codeGenerator.generateMethodDefHeader(" int ", Main.lg.tokMgrClassName, "jjMoveStringLiteralDfa" + i + Main.lg.lexStateSuffix + _params);
            }
            else
            {
                throw new Exception("Output language type not fully implemented : " + Options.getOutputLanguage());
            }

            codeGenerator.genCodeLine("{");

            if (i != 0)
            {
                if (i > 1)
                {
                    atLeastOne = false;
                    codeGenerator.genCode("   if ((");

                    for (j = 0; j < maxLongsReqd - 1; j++)
                        if (i <= maxLenForActive[j] + 1)
                        {
                            if (atLeastOne)
                                codeGenerator.genCode(" | ");
                            else
                                atLeastOne = true;
                            codeGenerator.genCode("(active" + j + " &= old" + j + ")");
                        }

                    if (i <= maxLenForActive[j] + 1)
                    {
                        if (atLeastOne)
                            codeGenerator.genCode(" | ");
                        codeGenerator.genCode("(active" + j + " &= old" + j + ")");
                    }

                    codeGenerator.genCodeLine(") == 0L)");
                    if (!Main.lg.mixed[Main.lg.lexStateIndex] && NfaState.generatedStates != 0)
                    {
                        codeGenerator.genCode("      return jjStartNfa" + Main.lg.lexStateSuffix +
                                        "(" + (i - 2) + ", ");
                        for (j = 0; j < maxLongsReqd - 1; j++)
                            if (i <= maxLenForActive[j] + 1)
                                codeGenerator.genCode("old" + j + ", ");
                            else
                                codeGenerator.genCode("0L, ");
                        if (i <= maxLenForActive[j] + 1)
                            codeGenerator.genCodeLine("old" + j + ");");
                        else
                            codeGenerator.genCodeLine("0L);");
                    }
                    else if (NfaState.generatedStates != 0)
                        codeGenerator.genCodeLine("      return jjMoveNfa" + Main.lg.lexStateSuffix +
                                "(" + NfaState.InitStateName() + ", " + (i - 1) + ");");
                    else
                        codeGenerator.genCodeLine("      return " + i + ";");
                }

                if (i != 0 && Options.getDebugTokenManager())
                {
                    if (codeGenerator.isJavaLanguage())
                    {
                        codeGenerator.genCodeLine("   if (jjmatchedKind != 0 && jjmatchedKind != 0x" + int.toHexString(int.MaxValue) + ")");
                        codeGenerator.genCodeLine("      debugStream.println(\"   Currently matched the first \" + " + "(jjmatchedPos + 1) + \" characters as a \" + tokenImage[jjmatchedKind] + \" token.\");");
                        codeGenerator.genCodeLine("   debugStream.println(\"   Possible string literal matches : { \"");
                    }
                    else
                    {
                        codeGenerator.genCodeLine("   if (jjmatchedKind != 0 && jjmatchedKind != 0x" + int.toHexString(int.MaxValue) + ")");
                        codeGenerator.genCodeLine("      fprintf(debugStream, \"   Currently matched the first %d characters as a \\\"%s\\\" token.\\n\", (jjmatchedPos + 1), addUnicodeEscapes(tokenImage[jjmatchedKind]).c_str());");
                        codeGenerator.genCodeLine("   fprintf(debugStream, \"   Possible string literal matches : { \");");
                    }

                    StringBuilder fmt = new StringBuilder();
                    StringBuilder args = new StringBuilder();
                    for (int vecs = 0; vecs < maxStrKind / 64 + 1; vecs++)
                    {
                        if (i <= maxLenForActive[vecs])
                        {
                            if (codeGenerator.isJavaLanguage())
                            {
                                codeGenerator.genCodeLine(" +");
                                codeGenerator.genCode("         jjKindsForBitVector(" + vecs + ", ");
                                codeGenerator.genCode("active" + vecs + ") ");
                            }
                            else
                            {
                                if (fmt.Length > 0)
                                {
                                    fmt.Append(", ");
                                    args.Append(", ");
                                }

                                fmt.Append("%s");
                                args.Append("         jjKindsForBitVector(" + vecs + ", ");
                                args.Append("active" + vecs + ")" + (codeGenerator.isJavaLanguage() ? " " : ".c_str() "));
                            }
                        }
                    }

                    // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
                    if (codeGenerator.isJavaLanguage())
                    {
                        codeGenerator.genCodeLine(" + \" } \");");
                    }
                    else if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
                    {
                        fmt.Append("}\\n");
                        codeGenerator.genCodeLine("    fprintf(debugStream, \"" + fmt + "\"," + args + ");");
                    }
                    else
                    {
                        throw new Exception("Output language type not fully implemented : " + Options.getOutputLanguage());
                    }
                }

                // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
                if (Options.isOutputLanguageJava())
                {
                    codeGenerator.genCodeLine("   try { curChar = input_stream.readChar(); }");
                    codeGenerator.genCodeLine("   catch(java.io.IOException e) {");
                }
                else if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
                {
                    codeGenerator.genCodeLine("   if (input_stream->endOfInput()) {");
                }
                else
                {
                    throw new Exception("Output language type not fully implemented : " + Options.getOutputLanguage());
                }

                if (!Main.lg.mixed[Main.lg.lexStateIndex] && NfaState.generatedStates != 0)
                {
                    codeGenerator.genCode("      jjStopStringLiteralDfa" + Main.lg.lexStateSuffix + "(" + (i - 1) + ", ");
                    for (k = 0; k < maxLongsReqd - 1; k++)
                    {
                        if (i <= maxLenForActive[k])
                            codeGenerator.genCode("active" + k + ", ");
                        else
                            codeGenerator.genCode("0L, ");
                    }

                    if (i <= maxLenForActive[k])
                    {
                        codeGenerator.genCodeLine("active" + k + ");");
                    }
                    else
                    {
                        codeGenerator.genCodeLine("0L);");
                    }


                    if (i != 0 && Options.getDebugTokenManager())
                    {
                        if (codeGenerator.isJavaLanguage())
                        {
                            codeGenerator.genCodeLine("      if (jjmatchedKind != 0 && jjmatchedKind != 0x" + int.toHexString(int.MaxValue) + ")");
                            codeGenerator.genCodeLine("         debugStream.println(\"   Currently matched the first \" + " + "(jjmatchedPos + 1) + \" characters as a \" + tokenImage[jjmatchedKind] + \" token.\");");
                        }
                        else
                        {
                            codeGenerator.genCodeLine("      if (jjmatchedKind != 0 && jjmatchedKind != 0x" + int.toHexString(int.MaxValue) + ")");
                            codeGenerator.genCodeLine("      fprintf(debugStream, \"   Currently matched the first %d characters as a \\\"%s\\\" token.\\n\", (jjmatchedPos + 1),  addUnicodeEscapes(tokenImage[jjmatchedKind]).c_str());");
                        }
                    }

                    codeGenerator.genCodeLine("      return " + i + ";");
                }
                else if (NfaState.generatedStates != 0)
                {
                    codeGenerator.genCodeLine("   return jjMoveNfa" + Main.lg.lexStateSuffix + "(" + NfaState.InitStateName() +
                            ", " + (i - 1) + ");");
                }
                else
                {
                    codeGenerator.genCodeLine("      return " + i + ";");
                }

                codeGenerator.genCodeLine("   }");
            }



            // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
            if (i != 0 && Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
            {
                codeGenerator.genCodeLine("   curChar = input_stream->readChar();");
            }

            if (i != 0 && Options.getDebugTokenManager())
            {

                // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
                if (codeGenerator.isJavaLanguage())
                {
                    codeGenerator.genCodeLine("   debugStream.println(" +
                           (LexGen.maxLexStates > 1 ? "\"<\" + lexStateNames[curLexState] + \">\" + " : "") +
                           "\"Current character : \" + " + Options.getTokenMgrErrorClass() +
                           ".addEscapes(String.valueOf(curChar)) + \" (\" + (int)curChar + \") " +
                           "at line \" + input_stream.getEndLine() + \" column \" + input_stream.getEndColumn());");
                }
                else if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
                {
                    codeGenerator.genCodeLine("   fprintf(debugStream, " +
                      "\"<%s>Current character : %c(%d) at line %d column %d\\n\"," +
                      "addUnicodeEscapes(lexStateNames[curLexState]).c_str(), curChar, (int)curChar, " +
                      "input_stream->getEndLine(), input_stream->getEndColumn());");
                }
                else
                {
                    throw new Exception("Output language type not fully implemented : " + Options.getOutputLanguage());
                }
            }

            codeGenerator.genCodeLine("   switch(curChar)");
            codeGenerator.genCodeLine("   {");

        CaseLoop:
            for (int q = 0; q < keys.Length; q++)
            {
                key = keys[q];
                info = (KindInfo)tab.get(key);
                ifGenerated = false;
                char c = key.charAt(0);

                if (i == 0 && c < 128 && info.finalKindCnt != 0 &&
                    (NfaState.generatedStates == 0 || !NfaState.CanStartNfaUsingAscii(c)))
                {
                    int kind;
                    for (j = 0; j < maxLongsReqd; j++)
                        if (info.finalKinds[j] != 0L)
                            break;

                    for (k = 0; k < 64; k++)
                        if ((info.finalKinds[j] & (1L << k)) != 0L &&
                            !subString[kind = (j * 64 + k)])
                        {
                            if ((intermediateKinds != null &&
                                 intermediateKinds[(j * 64 + k)] != null &&
                                 intermediateKinds[(j * 64 + k)][i] < (j * 64 + k) &&
                                 intermediateMatchedPos != null &&
                                 intermediateMatchedPos[(j * 64 + k)][i] == i) ||
                                (Main.lg.canMatchAnyChar[Main.lg.lexStateIndex] >= 0 &&
                                 Main.lg.canMatchAnyChar[Main.lg.lexStateIndex] < (j * 64 + k)))
                                break;
                            else if ((Main.lg.toSkip[kind / 64] & (1L << (kind % 64))) != 0L &&
                                     (Main.lg.toSpecial[kind / 64] & (1L << (kind % 64))) == 0L &&
                                     Main.lg.actions[kind] == null &&
                                     Main.lg.newLexState[kind] == null)
                            {
                                Main.lg.AddCharToSkip(c, kind);

                                if (Options.getIgnoreCase())
                                {
                                    if (c != char.ToUpper(c))
                                        Main.lg.AddCharToSkip(char.ToUpper(c), kind);

                                    if (c != char.ToLower(c))
                                        Main.lg.AddCharToSkip(char.ToLower(c), kind);
                                }
                                continue CaseLoop;
                            }
                        }
                }

                // Since we know key is a single character ...
                if (Options.getIgnoreCase())
                {
                    if (c != char.ToUpper(c))
                        codeGenerator.genCodeLine("      case " + (int)char.ToUpper(c) + ":");

                    if (c != char.ToLower(c))
                        codeGenerator.genCodeLine("      case " + (int)char.ToLower(c) + ":");
                }

                codeGenerator.genCodeLine("      case " + (int)c + ":");

                long matchedKind;
                string prefix = (i == 0) ? "         " : "            ";

                if (info.finalKindCnt != 0)
                {
                    for (j = 0; j < maxLongsReqd; j++)
                    {
                        if ((matchedKind = info.finalKinds[j]) == 0L)
                            continue;

                        for (k = 0; k < 64; k++)
                        {
                            if ((matchedKind & (1L << k)) == 0L)
                                continue;

                            if (ifGenerated)
                            {
                                codeGenerator.genCode("         else if ");
                            }
                            else if (i != 0)
                                codeGenerator.genCode("         if ");

                            ifGenerated = true;

                            int kindToPrint;
                            if (i != 0)
                            {
                                codeGenerator.genCodeLine("((active" + j +
                                   " & 0x" + Long.toHexString(1L << k) + "L) != 0L)");
                            }

                            if (intermediateKinds != null &&
                                intermediateKinds[(j * 64 + k)] != null &&
                                intermediateKinds[(j * 64 + k)][i] < (j * 64 + k) &&
                                intermediateMatchedPos != null &&
                                intermediateMatchedPos[(j * 64 + k)][i] == i)
                            {
                                JavaCCErrors.warning(" \"" +
                                    JavaCCGlobals.add_escapes(allImages[j * 64 + k]) +
                                    "\" cannot be matched as a string literal token " +
                                    "at line " + GetLine(j * 64 + k) + ", column " + GetColumn(j * 64 + k) +
                                    ". It will be matched as " +
                                    GetLabel(intermediateKinds[(j * 64 + k)][i]) + ".");
                                kindToPrint = intermediateKinds[(j * 64 + k)][i];
                            }
                            else if (i == 0 &&
                                 Main.lg.canMatchAnyChar[Main.lg.lexStateIndex] >= 0 &&
                                 Main.lg.canMatchAnyChar[Main.lg.lexStateIndex] < (j * 64 + k))
                            {
                                JavaCCErrors.warning(" \"" +
                                    JavaCCGlobals.add_escapes(allImages[j * 64 + k]) +
                                    "\" cannot be matched as a string literal token " +
                                    "at line " + GetLine(j * 64 + k) + ", column " + GetColumn(j * 64 + k) +
                                    ". It will be matched as " +
                                    GetLabel(Main.lg.canMatchAnyChar[Main.lg.lexStateIndex]) + ".");
                                kindToPrint = Main.lg.canMatchAnyChar[Main.lg.lexStateIndex];
                            }
                            else
                                kindToPrint = j * 64 + k;

                            if (!subString[(j * 64 + k)])
                            {
                                int stateSetName = GetStateSetForKind(i, j * 64 + k);

                                if (stateSetName != -1)
                                {
                                    createStartNfa = true;
                                    codeGenerator.genCodeLine(prefix + "return jjStartNfaWithStates" +
                                        Main.lg.lexStateSuffix + "(" + i +
                                        ", " + kindToPrint + ", " + stateSetName + ");");
                                }
                                else
                                    codeGenerator.genCodeLine(prefix + "return jjStopAtPos" + "(" + i + ", " + kindToPrint + ");");
                            }
                            else
                            {
                                if ((Main.lg.initMatch[Main.lg.lexStateIndex] != 0 &&
                                     Main.lg.initMatch[Main.lg.lexStateIndex] != int.MaxValue) ||
                                     i != 0)
                                {
                                    codeGenerator.genCodeLine("         {");
                                    codeGenerator.genCodeLine(prefix + "jjmatchedKind = " +
                                                               kindToPrint + ";");
                                    codeGenerator.genCodeLine(prefix + "jjmatchedPos = " + i + ";");
                                    codeGenerator.genCodeLine("         }");
                                }
                                else
                                    codeGenerator.genCodeLine(prefix + "jjmatchedKind = " +
                                                               kindToPrint + ";");
                            }
                        }
                    }
                }

                if (info.validKindCnt != 0)
                {
                    atLeastOne = false;

                    if (i == 0)
                    {
                        codeGenerator.genCode("         return ");

                        codeGenerator.genCode("jjMoveStringLiteralDfa" + (i + 1) +
                                       Main.lg.lexStateSuffix + "(");
                        for (j = 0; j < maxLongsReqd - 1; j++)
                            if ((i + 1) <= maxLenForActive[j])
                            {
                                if (atLeastOne)
                                    codeGenerator.genCode(", ");
                                else
                                    atLeastOne = true;

                                codeGenerator.genCode("0x" + Long.toHexString(info.validKinds[j]) + (codeGenerator.isJavaLanguage() ? "L" : "L"));
                            }

                        if ((i + 1) <= maxLenForActive[j])
                        {
                            if (atLeastOne)
                                codeGenerator.genCode(", ");

                            codeGenerator.genCode("0x" + Long.toHexString(info.validKinds[j]) + (codeGenerator.isJavaLanguage() ? "L" : "L"));
                        }
                        codeGenerator.genCodeLine(");");
                    }
                    else
                    {
                        codeGenerator.genCode("         return ");

                        codeGenerator.genCode("jjMoveStringLiteralDfa" + (i + 1) +
                                       Main.lg.lexStateSuffix + "(");

                        for (j = 0; j < maxLongsReqd - 1; j++)
                            if ((i + 1) <= maxLenForActive[j] + 1)
                            {
                                if (atLeastOne)
                                    codeGenerator.genCode(", ");
                                else
                                    atLeastOne = true;

                                if (info.validKinds[j] != 0L)
                                    codeGenerator.genCode("active" + j + ", 0x" +
                                            Long.toHexString(info.validKinds[j]) + (codeGenerator.isJavaLanguage() ? "L" : "L"));
                                else
                                    codeGenerator.genCode("active" + j + ", 0L");
                            }

                        if ((i + 1) <= maxLenForActive[j] + 1)
                        {
                            if (atLeastOne)
                                codeGenerator.genCode(", ");
                            if (info.validKinds[j] != 0L)
                                codeGenerator.genCode("active" + j + ", 0x" +
                                           Long.toHexString(info.validKinds[j]) + (codeGenerator.isJavaLanguage() ? "L" : "L"));
                            else
                                codeGenerator.genCode("active" + j + ", 0L");
                        }

                        codeGenerator.genCodeLine(");");
                    }
                }
                else
                {
                    // A very special case.
                    if (i == 0 && Main.lg.mixed[Main.lg.lexStateIndex])
                    {

                        if (NfaState.generatedStates != 0)
                            codeGenerator.genCodeLine("         return jjMoveNfa" + Main.lg.lexStateSuffix +
                                    "(" + NfaState.InitStateName() + ", 0);");
                        else
                            codeGenerator.genCodeLine("         return 1;");
                    }
                    else if (i != 0) // No more str literals to look for
                    {
                        codeGenerator.genCodeLine("         break;");
                        startNfaNeeded = true;
                    }
                }
            }

            /* default means that the current character is not in any of the
               strings at this position. */
            codeGenerator.genCodeLine("      default :");

            if (Options.getDebugTokenManager())
            {
                if (codeGenerator.isJavaLanguage())
                {
                    codeGenerator.genCodeLine("      debugStream.println(\"   No string literal matches possible.\");");
                }
                else
                {
                    codeGenerator.genCodeLine("      fprintf(debugStream, \"   No string literal matches possible.\\n\");");
                }
            }

            if (NfaState.generatedStates != 0)
            {
                if (i == 0)
                {
                    /* This means no string literal is possible. Just move nfa with
                       this guy and return. */
                    codeGenerator.genCodeLine("         return jjMoveNfa" + Main.lg.lexStateSuffix +
                            "(" + NfaState.InitStateName() + ", 0);");
                }
                else
                {
                    codeGenerator.genCodeLine("         break;");
                    startNfaNeeded = true;
                }
            }
            else
            {
                codeGenerator.genCodeLine("         return " + (i + 1) + ";");
            }


            codeGenerator.genCodeLine("   }");

            if (i != 0)
            {
                if (startNfaNeeded)
                {
                    if (!Main.lg.mixed[Main.lg.lexStateIndex] && NfaState.generatedStates != 0)
                    {
                        /* Here, a string literal is successfully matched and no more
                           string literals are possible. So set the kind and state set
                           upto and including this position for the matched string. */

                        codeGenerator.genCode("   return jjStartNfa" + Main.lg.lexStateSuffix + "(" + (i - 1) + ", ");
                        for (k = 0; k < maxLongsReqd - 1; k++)
                            if (i <= maxLenForActive[k])
                                codeGenerator.genCode("active" + k + ", ");
                            else
                                codeGenerator.genCode("0L, ");
                        if (i <= maxLenForActive[k])
                            codeGenerator.genCodeLine("active" + k + ");");
                        else
                            codeGenerator.genCodeLine("0L);");
                    }
                    else if (NfaState.generatedStates != 0)
                        codeGenerator.genCodeLine("   return jjMoveNfa" + Main.lg.lexStateSuffix +
                                "(" + NfaState.InitStateName() + ", " + i + ");");
                    else
                        codeGenerator.genCodeLine("   return " + (i + 1) + ";");
                }
            }

            codeGenerator.genCodeLine("}");
        }

        if (!Main.lg.mixed[Main.lg.lexStateIndex] && NfaState.generatedStates != 0 && createStartNfa)
            DumpStartWithStates(codeGenerator);
    }

    static int GetStrKind(string str)
    {
        for (int i = 0; i < maxStrKind; i++)
        {
            if (Main.lg.lexStates[i] != Main.lg.lexStateIndex)
                continue;

            string image = allImages[i];
            if (image != null && image == (str))
                return i;
        }

        return int.MaxValue;
    }

    public static void GenerateNfaStartStates(CodeGenerator codeGenerator,
                                                  NfaState initialState)
    {
        bool[] seen = new bool[NfaState.generatedStates];
        Dictionary stateSets = new Dictionary();
        string stateSetString = "";
        int i, j, kind, jjmatchedPos = 0;
        int maxKindsReqd = maxStrKind / 64 + 1;
        long[] actives;
        List newStates = new ();
        List oldStates = null, jjtmpStates;

        statesForPos = new Dictionary[maxLen];
        intermediateKinds = new int[maxStrKind + 1][];
        intermediateMatchedPos = new int[maxStrKind + 1][];

        for (i = 0; i < maxStrKind; i++)
        {
            if (Main.lg.lexStates[i] != Main.lg.lexStateIndex)
                continue;

            string image = allImages[i];

            if (image == null || image.Length < 1)
                continue;

            try
            {
                if ((oldStates = (List)initialState.epsilonMoves.clone()) == null ||
                    oldStates.Count == 0)
                {
                    DumpNfaStartStatesCode(statesForPos, codeGenerator);
                    return;
                }
            }
            catch (Exception e)
            {
                JavaCCErrors.semantic_error("Error cloning state vector");
            }

            intermediateKinds[i] = new int[image.Length];
            intermediateMatchedPos[i] = new int[image.Length];
            jjmatchedPos = 0;
            kind = int.MaxValue;

            for (j = 0; j < image.Length; j++)
            {
                if (oldStates == null || oldStates.Count <= 0)
                {
                    // Here, j > 0
                    kind = intermediateKinds[i][j] = intermediateKinds[i][j - 1];
                    jjmatchedPos = intermediateMatchedPos[i][j] = intermediateMatchedPos[i][j - 1];
                }
                else
                {
                    kind = NfaState.MoveFromSet(image.charAt(j), oldStates, newStates);
                    oldStates.Clear();

                    if (j == 0 && kind != int.MaxValue &&
                        Main.lg.canMatchAnyChar[Main.lg.lexStateIndex] != -1 &&
                        kind > Main.lg.canMatchAnyChar[Main.lg.lexStateIndex])
                        kind = Main.lg.canMatchAnyChar[Main.lg.lexStateIndex];

                    if (GetStrKind(image.substring(0, j + 1)) < kind)
                    {
                        intermediateKinds[i][j] = kind = int.MaxValue;
                        jjmatchedPos = 0;
                    }
                    else if (kind != int.MaxValue)
                    {
                        intermediateKinds[i][j] = kind;
                        jjmatchedPos = intermediateMatchedPos[i][j] = j;
                    }
                    else if (j == 0)
                        kind = intermediateKinds[i][j] = int.MaxValue;
                    else
                    {
                        kind = intermediateKinds[i][j] = intermediateKinds[i][j - 1];
                        jjmatchedPos = intermediateMatchedPos[i][j] = intermediateMatchedPos[i][j - 1];
                    }

                    stateSetString = NfaState.GetStateSetString(newStates);
                }

                if (kind == int.MaxValue &&
                    (newStates == null || newStates.Count == 0))
                    continue;

                int p;
                if (stateSets.get(stateSetString) == null)
                {
                    stateSets.Add(stateSetString, stateSetString);
                    for (p = 0; p < newStates.Count; p++)
                    {
                        if (seen[((NfaState)newStates.get(p)).stateName])
                            ((NfaState)newStates.get(p)).inNextOf++;
                        else
                            seen[((NfaState)newStates.get(p)).stateName] = true;
                    }
                }
                else
                {
                    for (p = 0; p < newStates.Count; p++)
                        seen[((NfaState)newStates.get(p)).stateName] = true;
                }

                jjtmpStates = oldStates;
                oldStates = newStates;
                (newStates = jjtmpStates).Clear();

                if (statesForPos[j] == null)
                    statesForPos[j] = new Dictionary();

                if ((actives = ((long[])statesForPos[j].get(kind + ", " +
                                         jjmatchedPos + ", " + stateSetString))) == null)
                {
                    actives = new long[maxKindsReqd];
                    statesForPos[j].Add(kind + ", " + jjmatchedPos + ", " +
                                                       stateSetString, actives);
                }

                actives[i / 64] |= 1L << (i % 64);
                //string name = NfaState.StoreStateSet(stateSetString);
            }
        }

        // TODO(Sreeni) : Fix this mess.
        if (Options.getTokenManagerCodeGenerator() == null)
        {
            DumpNfaStartStatesCode(statesForPos, codeGenerator);
        }
    }

    static void DumpNfaStartStatesCode(Dictionary[] statesForPos,
                                                CodeGenerator codeGenerator)
    {
        if (maxStrKind == 0)
        { // No need to generate this function
            return;
        }

        int i, maxKindsReqd = maxStrKind / 64 + 1;
        bool condGenerated = false;
        int ind = 0;

        StringBuilder _params = new StringBuilder();
        for (i = 0; i < maxKindsReqd - 1; i++)
            _params.Append("" + Options.getLongType() + " active" + i + ", ");
        _params.Append("" + Options.getLongType() + " active" + i + ")");

        // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
        if (Options.isOutputLanguageJava())
        {
            codeGenerator.genCode("private" + (Options.getStatic() ? " static" : "") + " final int jjStopStringLiteralDfa" +
                         Main.lg.lexStateSuffix + "(int pos, " + _params);
        }
        else if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
        {
            codeGenerator.generateMethodDefHeader(" int", Main.lg.tokMgrClassName, "jjStopStringLiteralDfa" + Main.lg.lexStateSuffix + "(int pos, " + _params);
        }
        else
        {
            throw new Exception("Output language type not fully implemented : " + Options.getOutputLanguage());
        }

        codeGenerator.genCodeLine("{");

        if (Options.getDebugTokenManager())
        {
            // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
            if (codeGenerator.isJavaLanguage())
            {
                codeGenerator.genCodeLine("      debugStream.println(\"   No more string literal token matches are possible.\");");
            }
            else if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
            {
                codeGenerator.genCodeLine("      fprintf(debugStream, \"   No more string literal token matches are possible.\");");
            }
            else
            {
                throw new Exception("Output language type not fully implemented : " + Options.getOutputLanguage());
            }
        }

        codeGenerator.genCodeLine("   switch (pos)");
        codeGenerator.genCodeLine("   {");

        for (i = 0; i < maxLen - 1; i++)
        {
            if (statesForPos[i] == null)
                continue;

            codeGenerator.genCodeLine("      case " + i + ":");

            Enumeration e = statesForPos[i].keys();
            while (e.hasMoreElements())
            {
                string stateSetString = (String)e.nextElement();
                long[] actives = (long[])statesForPos[i].get(stateSetString);

                for (int j = 0; j < maxKindsReqd; j++)
                {
                    if (actives[j] == 0L)
                        continue;

                    if (condGenerated)
                        codeGenerator.genCode(" || ");
                    else
                        codeGenerator.genCode("         if (");

                    condGenerated = true;

                    codeGenerator.genCode("(active" + j + " & 0x" +
                        Long.toHexString(actives[j]) + "L) != 0L");
                }

                if (condGenerated)
                {
                    codeGenerator.genCodeLine(")");

                    string kindStr = stateSetString.substring(0,
                                             ind = stateSetString.IndexOf(", "));
                    string afterKind = stateSetString.substring(ind + 2);
                    int jjmatchedPos = int.parseInt(
                                 afterKind.substring(0, afterKind.IndexOf(", ")));

                    if (!kindStr == (String.valueOf(int.MaxValue)))
                        codeGenerator.genCodeLine("         {");

                    if (!kindStr == (String.valueOf(int.MaxValue)))
                    {
                        if (i == 0)
                        {
                            codeGenerator.genCodeLine("            jjmatchedKind = " + kindStr + ";");

                            if ((Main.lg.initMatch[Main.lg.lexStateIndex] != 0 &&
                                Main.lg.initMatch[Main.lg.lexStateIndex] != int.MaxValue))
                                codeGenerator.genCodeLine("            jjmatchedPos = 0;");
                        }
                        else if (i == jjmatchedPos)
                        {
                            if (subStringAtPos[i])
                            {
                                codeGenerator.genCodeLine("            if (jjmatchedPos != " + i + ")");
                                codeGenerator.genCodeLine("            {");
                                codeGenerator.genCodeLine("               jjmatchedKind = " + kindStr + ";");
                                codeGenerator.genCodeLine("               jjmatchedPos = " + i + ";");
                                codeGenerator.genCodeLine("            }");
                            }
                            else
                            {
                                codeGenerator.genCodeLine("            jjmatchedKind = " + kindStr + ";");
                                codeGenerator.genCodeLine("            jjmatchedPos = " + i + ";");
                            }
                        }
                        else
                        {
                            if (jjmatchedPos > 0)
                                codeGenerator.genCodeLine("            if (jjmatchedPos < " + jjmatchedPos + ")");
                            else
                                codeGenerator.genCodeLine("            if (jjmatchedPos == 0)");
                            codeGenerator.genCodeLine("            {");
                            codeGenerator.genCodeLine("               jjmatchedKind = " + kindStr + ";");
                            codeGenerator.genCodeLine("               jjmatchedPos = " + jjmatchedPos + ";");
                            codeGenerator.genCodeLine("            }");
                        }
                    }

                    kindStr = stateSetString.substring(0,
                                          ind = stateSetString.IndexOf(", "));
                    afterKind = stateSetString.substring(ind + 2);
                    stateSetString = afterKind.substring(
                                             afterKind.IndexOf(", ") + 2);

                    if (stateSetString == ("null;"))
                        codeGenerator.genCodeLine("            return -1;");
                    else
                        codeGenerator.genCodeLine("            return " +
                           NfaState.AddStartStateSet(stateSetString) + ";");

                    if (!kindStr == (String.valueOf(int.MaxValue)))
                        codeGenerator.genCodeLine("         }");
                    condGenerated = false;
                }
            }

            codeGenerator.genCodeLine("         return -1;");
        }

        codeGenerator.genCodeLine("      default :");
        codeGenerator.genCodeLine("         return -1;");
        codeGenerator.genCodeLine("   }");
        codeGenerator.genCodeLine("}");

        _params.Capacity=0;
        _params.Append("(int pos, ");
        for (i = 0; i < maxKindsReqd - 1; i++)
            _params.Append("" + Options.getLongType() + " active" + i + ", ");
        _params.Append("" + Options.getLongType() + " active" + i + ")");

        if (codeGenerator.isJavaLanguage())
        {
            codeGenerator.genCode("private" + (Options.getStatic() ? " static" : "") + " final int jjStartNfa" +
                       Main.lg.lexStateSuffix + _params);
        }
        else
        {
            codeGenerator.generateMethodDefHeader("int ", Main.lg.tokMgrClassName, "jjStartNfa" + Main.lg.lexStateSuffix + _params);
        }
        codeGenerator.genCodeLine("{");

        if (Main.lg.mixed[Main.lg.lexStateIndex])
        {
            if (NfaState.generatedStates != 0)
                codeGenerator.genCodeLine("   return jjMoveNfa" + Main.lg.lexStateSuffix +
                        "(" + NfaState.InitStateName() + ", pos + 1);");
            else
                codeGenerator.genCodeLine("   return pos + 1;");

            codeGenerator.genCodeLine("}");
            return;
        }

        codeGenerator.genCode("   return jjMoveNfa" + Main.lg.lexStateSuffix + "(" +
                  "jjStopStringLiteralDfa" + Main.lg.lexStateSuffix + "(pos, ");
        for (i = 0; i < maxKindsReqd - 1; i++)
            codeGenerator.genCode("active" + i + ", ");
        codeGenerator.genCode("active" + i + ")");
        codeGenerator.genCodeLine(", pos + 1);");
        codeGenerator.genCodeLine("}");
    }
    /**
     * Return to original state.
     */
    public static void reInit()
    {
        ReInit();

        charCnt = 0;
        allImages = null;
        boilerPlateDumped = false;
    }

    public StringBuilder dump(int indent, Set alreadyDumped)
    {
        StringBuilder sb = base.dump(indent, alreadyDumped).Append(' ').Append(image);
        return sb;
    }

    public override string ToString()
    {
        return base.ToString() + " - " + image;
    }

    /*
      static void GenerateData(TokenizerData tokenizerData) {
         Dictionary tab;
         string key;
         KindInfo info;
         for (int i = 0; i < maxLen; i++) {
            tab = (Dictionary)charPosKind[i];
            String[] keys = ReArrange(tab);
            if (Options.getIgnoreCase()) {
              for (string s : keys) {
                char c = s.charAt(0);
                tab.Add(char.ToLower(c), tab.get(c));
                tab.Add(char.ToUpper(c), tab.get(c));
              }
            }
            for (int q = 0; q < keys.Length; q++) {
               key = keys[q];
               info = (KindInfo)tab.get(key);
               char c = key.charAt(0);
               for (int kind : info.finalKindSet) {
                 tokenizerData.addDfaFinalKindAndState(
                     i, c, kind, GetStateSetForKind(i, kind));
               }
               for (int kind : info.validKindSet) {
                 tokenizerData.addDfaValidKind(i, c, kind);
               }
            }
         }
         for (int i = 0; i < maxLen; i++) {
            Enumeration e = statesForPos[i].keys();
            while (e.hasMoreElements())
            {
               string stateSetString = (String)e.nextElement();
               long[] actives = (long[])statesForPos[i].get(stateSetString);
               int ind = stateSetString.IndexOf(", ");
               string kindStr = stateSetString.substring(0, ind);
               string afterKind = stateSetString.substring(ind + 2);
               stateSetString = afterKind.substring(afterKind.IndexOf(", ") + 2);
               BitSet bits = BitSet.valueOf(actives);

               for (int j = 0; j < bits.Length; j++) {
                 if (bits[j]) tokenizerData.addFinalDfaKind(j);
               }
               // Pos
               codeGenerator.genCode(
                   ", " + afterKind.substring(0, afterKind.IndexOf(", ")));
               // Kind
               codeGenerator.genCode(", " + kindStr);

               // State
               if (stateSetString.equals("null;")) {
                  codeGenerator.genCodeLine(", -1");
               } else {
                  codeGenerator.genCodeLine(
                      ", " + NfaState.AddStartStateSet(stateSetString));
               }
            }
            codeGenerator.genCode("}");
         }
         codeGenerator.genCodeLine("};");
      }
    */

    static readonly Dictionary<int, List<String>> literalsByLength =
        new Dictionary<int, List<String>>();
    static readonly Dictionary<int, List<int>> literalKinds =
        new Dictionary<int, List<int>>();
    static readonly Dictionary<int, int> kindToLexicalState =
        new Dictionary<int, int>();
    static readonly Dictionary<int, NfaState> nfaStateMap =
        new Dictionary<int, NfaState>();
    public static void UpdateStringLiteralData(
        int generatedNfaStates, int lexStateIndex)
    {
        for (int kind = 0; kind < allImages.Length; kind++)
        {
            if (allImages[kind] == null || allImages[kind] == ("") ||
                Main.lg.lexStates[kind] != lexStateIndex)
            {
                continue;
            }
            string s = allImages[kind];
            int actualKind;
            if (intermediateKinds != null &&
                intermediateKinds[kind][s.Length - 1] != int.MaxValue &&
                intermediateKinds[kind][s.Length - 1] < kind)
            {
                JavaCCErrors.warning("Token: " + s + " will not be matched as " +
                                     "specified. It will be matched as token " +
                                     "of kind: " +
                                     intermediateKinds[kind][s.Length - 1] +
                                     " instead.");
                actualKind = intermediateKinds[kind][s.Length - 1];
            }
            else
            {
                actualKind = kind;
            }
            kindToLexicalState.Add(actualKind, lexStateIndex);
            if (Options.getIgnoreCase())
            {
                s = s.ToLower(Locale.ENGLISH);
            }
            char c = s.charAt(0);
            int key = (int)Main.lg.lexStateIndex << 16 | (int)c;
            List<String> l = literalsByLength.get(key);
            List<int> kinds = literalKinds.get(key);
            int j = 0;
            if (l == null)
            {
                literalsByLength.Add(key, l = new());
                assert(kinds == null);
                kinds = new ArrayList<int>();
                literalKinds.Add(key, kinds = new List<int>());
            }
            while (j < l.Count && l[j].Length > s.Length) j++;
            l.Add(j, s);
            kinds.Add(j, actualKind);
            int stateIndex = GetStateSetForKind(s.Length - 1, kind);
            if (stateIndex != -1)
            {
                nfaStateMap.Add(actualKind, NfaState.getNfaState(stateIndex));
            }
            else
            {
                nfaStateMap.Add(actualKind, null);
            }
        }
    }

    public static void BuildTokenizerData(TokenizerData tokenizerData)
    {
        Dictionary<int, int> nfaStateIndices = new Dictionary<int, int>();
        foreach (int kind in nfaStateMap.keySet())
        {
            if (nfaStateMap.get(kind) != null)
            {
                nfaStateIndices.Add(kind, nfaStateMap.get(kind).stateName);
            }
            else
            {
                nfaStateIndices.Add(kind, -1);
            }
        }
        tokenizerData.setLiteralSequence(literalsByLength);
        tokenizerData.setLiteralKinds(literalKinds);
        tokenizerData.setKindToNfaStartState(nfaStateIndices);
    }
}
