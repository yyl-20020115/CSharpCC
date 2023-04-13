namespace CSharpCC.Parser;

/**
 * Class that implements a table driven code generator for the token manager in
 * java.
 */
public class TableDrivenJavaCodeGenerator : TokenManagerCodeGenerator
{
    private static readonly string TokenManagerTemplate =
        "/templates/TableDrivenTokenManager.template";
    private readonly CodeGenerator codeGenerator = new();

    //@Override
    public void GenerateCode(TokenizerData tokenizerData)
    {
        string superClass = (string)Options.getOptions()[Options.USEROPTION__TOKEN_MANAGER_SUPER_CLASS];
        Dictionary<string, object> options = Options.getOptions();
        options.Add("maxOrdinal", tokenizerData.allMatches.Count);
        options.Add("maxLexStates", tokenizerData.lexStateNames.Length);
        options.Add("stateSetSize", tokenizerData.nfa.Count);
        options.Add("parserName", tokenizerData.parserName);
        options.Add("maxLongs", tokenizerData.allMatches.Count / 64 + 1);
        options.Add("parserName", tokenizerData.parserName);
        options.Add("charStreamName", CodeGenerator.GetCharStreamName());
        options.Add("defaultLexState", tokenizerData.defaultLexState);
        options.Add("decls", tokenizerData.decls);
        options.Add("superClass", (superClass == null || superClass == ("")) ? "" : "extends " + superClass);
        options.Add("noDfa", Options.GetNoDfa());
        options.Add("generatedStates", tokenizerData.nfa.Count);
        try
        {
            codeGenerator.WriteTemplate(TokenManagerTemplate, options);
            DumpDfaTables(codeGenerator, tokenizerData);
            DumpNfaTables(codeGenerator, tokenizerData);
            DumpMatchInfo(codeGenerator, tokenizerData);
        }
        catch (IOException ioe)
        {
            //assert(false);
        }
    }

    //@Override
    public void Finish(TokenizerData tokenizerData)
    {
        // TODO(sreeni) : Fix this mess.
        codeGenerator.GenCodeLine("\n}");
        if (!Options.GetBuildParser()) return;
        var fileName = Options.GetOutputDirectory() + Path.DirectorySeparatorChar +
                          tokenizerData.parserName + "TokenManager.java";
        codeGenerator.SaveOutput(fileName);
    }

    private static void DumpDfaTables(
        CodeGenerator codeGenerator, TokenizerData tokenizerData)
    {
        Dictionary<int, int[]> startAndSize = new();
        int i = 0;

        codeGenerator.GenCodeLine(
            "private static final int[] stringLiterals = {");
        foreach (int key in tokenizerData.literalSequence.Keys)
        {
            int[] arr = new int[2];
            List<string> l = tokenizerData.literalSequence[key];
            List<int> kinds = tokenizerData.literalKinds[key];
            arr[0] = i;
            arr[1] = l.Count;
            int j = 0;
            if (i > 0) codeGenerator.GenCodeLine(", ");
            foreach (string s in l)
            {
                if (j > 0) codeGenerator.GenCodeLine(", ");
                codeGenerator.GenCode(s.Length);
                for (int k = 0; k < s.Length; k++)
                {
                    codeGenerator.GenCode(", ");
                    codeGenerator.GenCode((int)s[k]);
                    i++;
                }
                int kind = kinds[j];
                codeGenerator.GenCode(", " + kind);
                codeGenerator.GenCode(
                    ", " + tokenizerData.kindToNfaStartState[kind]);
                i += 3;
                j++;
            }
            startAndSize.Add(key, arr);
        }
        codeGenerator.GenCodeLine("};");

        codeGenerator.GenCodeLine(
            "private static final Dictionary<int, int[]> startAndSize =\n" +
            "    new HashMap<Integer, int[]>();");

        // Static block to actually initialize the map from the int array above.
        codeGenerator.GenCodeLine("static {");
        foreach (int key in tokenizerData.literalSequence.Keys)
        {
            int[] arr = startAndSize[key];
            codeGenerator.GenCodeLine("startAndSize.Add(" + key + ", new int[]{" +
                                       arr[0] + ", " + arr[1] + "});");
        }
        codeGenerator.GenCodeLine("}");
    }

    private void DumpNfaTables(
        CodeGenerator codeGenerator, TokenizerData tokenizerData)
    {
        // WE do the following for java so that the generated code is reasonable
        // size and can be compiled. May not be needed for other languages.
        codeGenerator.GenCodeLine("private static final long[][] jjCharData = {");
        Dictionary<int, TokenizerData.NfaState> nfa = tokenizerData.nfa;
        for (int i = 0; i < nfa.Count; i++)
        {
            TokenizerData.NfaState tmp = nfa[i];
            if (i > 0) codeGenerator.GenCodeLine(",");
            if (tmp == null)
            {
                codeGenerator.GenCode("{}");
                continue;
            }
            codeGenerator.GenCode("{");
            BitSet bits = new();
            foreach (char c in tmp.characters)
            {
                bits.Set(c);
            }
            long[] longs = bits.ToLongArray();
            for (int k0 = 0; k0 < longs.Length; k0++)
            {
                int rep = 1;
                while (k0 + rep < longs.Length && longs[k0 + rep] == longs[k0]) rep++;
                if (k0 > 0) codeGenerator.GenCode(", ");
                codeGenerator.GenCode(rep + ", ");
                codeGenerator.GenCode("0x" + Convert.ToString(longs[k0],16) + "L");
                k0 += rep - 1;
            }
            codeGenerator.GenCode("}");
        }
        codeGenerator.GenCodeLine("};");

        codeGenerator.GenCodeLine(
             "private static final long[][] jjChars = ");
        codeGenerator.GenCodeLine(
             "    new long[" + tokenizerData.nfa.Count +
             "][(Character.MAX_VALUE >> 6) + 1]; ");
        codeGenerator.GenCodeLine(
             "static { ");
        codeGenerator.GenCodeLine(
             "  for (int i = 0; i < " + tokenizerData.nfa.Count + "; i++) { ");
        codeGenerator.GenCodeLine(
             "    int ind = 0; ");
        codeGenerator.GenCodeLine(
             "    for (int j = 0; j < jjCharData[i].length; j += 2) { ");
        codeGenerator.GenCodeLine(
             "      for (int k = 0; k < (int)jjCharData[i][j]; k++) { ");
        codeGenerator.GenCodeLine(
             "        jjChars[i][ind++] = jjCharData[i][j + 1]; ");
        codeGenerator.GenCodeLine(
             "      } ");
        codeGenerator.GenCodeLine(
             "    } ");
        codeGenerator.GenCodeLine(
             "  } ");
        codeGenerator.GenCodeLine(
             "} ");


        codeGenerator.GenCodeLine(
            "private static final int[][] jjcompositeState = {");
        for (int i = 0; i < nfa.Count; i++)
        {
            TokenizerData.NfaState tmp = nfa[i];
            if (i > 0) codeGenerator.GenCodeLine(", ");
            if (tmp == null)
            {
                codeGenerator.GenCode("{}");
                continue;
            }
            codeGenerator.GenCode("{");
            int k2= 0;
            foreach (int st in tmp.compositeStates)
            {
                if (k2++ > 0) codeGenerator.GenCode(", ");
                codeGenerator.GenCode(st);
            }
            codeGenerator.GenCode("}");
        }
        codeGenerator.GenCodeLine("};");

        codeGenerator.GenCodeLine("private static final int[] jjmatchKinds = {");
        for (int i = 0; i < nfa.Count; i++)
        {
            TokenizerData.NfaState tmp = nfa[i];
            if (i > 0) codeGenerator.GenCodeLine(", ");
            // TODO(sreeni) : Fix this mess.
            if (tmp == null)
            {
                codeGenerator.GenCode(int.MaxValue);
                continue;
            }
            codeGenerator.GenCode(tmp.kind);
        }
        codeGenerator.GenCodeLine("};");

        codeGenerator.GenCodeLine(
            "private static final int[][]  jjnextStateSet = {");
        for (int i = 0; i < nfa.Count; i++)
        {
            TokenizerData.NfaState tmp = nfa[i];
            if (i > 0) codeGenerator.GenCodeLine(", ");
            if (tmp == null)
            {
                codeGenerator.GenCode("{}");
                continue;
            }
            int k2 = 0;
            codeGenerator.GenCode("{");
            foreach (int s in tmp.nextStates)
            {
                if (k2++ > 0) codeGenerator.GenCode(", ");
                codeGenerator.GenCode(s);
            }
            codeGenerator.GenCode("}");
        }
        codeGenerator.GenCodeLine("};");

        codeGenerator.GenCodeLine(
            "private static final int[] jjInitStates  = {");
        int k = 0;
        foreach (int i in tokenizerData.initialStates.Keys)
        {
            if (k++ > 0) codeGenerator.GenCode(", ");
            codeGenerator.GenCode(tokenizerData.initialStates[i]);
        }
        codeGenerator.GenCodeLine("};");

        codeGenerator.GenCodeLine(
            "private static final int[] canMatchAnyChar = {");
        k = 0;
        for (int i = 0; i < tokenizerData.wildcardKind.Count; i++)
        {
            if (k++ > 0) codeGenerator.GenCode(", ");
            codeGenerator.GenCode(tokenizerData.wildcardKind[i]);
        }
        codeGenerator.GenCodeLine("};");
    }

    private void DumpMatchInfo(
        CodeGenerator codeGenerator, TokenizerData tokenizerData)
    {
        Dictionary<int, TokenizerData.MatchInfo> allMatches =
            tokenizerData.allMatches;

        // A bit ugly.

        BitSet bitSet = new(allMatches.Count);
        BitSet toSkip = bitSet;
        BitSet toSpecial = new(allMatches.Count);
        BitSet toMore = new(allMatches.Count);
        BitSet toToken = new(allMatches.Count);
        int[] newStates = new int[allMatches.Count];
        toSkip.Set(allMatches.Count + 1);
        toToken.Set(allMatches.Count + 1);
        toMore.Set(allMatches.Count + 1);
        toSpecial.Set(allMatches.Count + 1);
        // Kind map.
        codeGenerator.GenCodeLine(
            "public static final String[] jjstrLiteralImages = {");

        int k = 0;
        foreach (int i in allMatches.Keys)
        {
            TokenizerData.MatchInfo matchInfo = allMatches[i];
            switch (matchInfo.matchType)
            {
                case MatchTypes.SKIP: toSkip.Set(i); break;
                case MatchTypes.SPECIAL_TOKEN: toSpecial.Set(i); break;
                case MatchTypes.MORE: toMore.Set(i); break;
                case MatchTypes.TOKEN: toToken.Set(i); break;
            }
            newStates[i] = matchInfo.newLexState;
            string image = matchInfo.image;
            if (k++ > 0) codeGenerator.GenCodeLine(", ");
            if (image != null)
            {
                codeGenerator.GenCode("\"");
                for (int j = 0; j < image.Length; j++)
                {
                    if (image[j] <= 0xff)
                    {
                        codeGenerator.GenCode(
                            "\\" + Convert.ToString((int)image[j]), 8);
                    }
                    else
                    {
                        string hexVal = Convert.ToString((int)image[j], 16);
                        if (hexVal.Length == 3)
                            hexVal = "0" + hexVal;
                        codeGenerator.GenCode("\\u" + hexVal);
                    }
                }
                codeGenerator.GenCode("\"");
            }
            else
            {
                codeGenerator.GenCodeLine("null");
            }
        }
        codeGenerator.GenCodeLine("};");

        // Now generate the bit masks.
        GenerateBitVector("jjtoSkip", toSkip, codeGenerator);
        GenerateBitVector("jjtoSpecial", toSpecial, codeGenerator);
        GenerateBitVector("jjtoMore", toMore, codeGenerator);
        GenerateBitVector("jjtoToken", toToken, codeGenerator);

        codeGenerator.GenCodeLine("private static final int[] jjnewLexState = {");
        for (int i = 0; i < newStates.Length; i++)
        {
            if (i > 0) codeGenerator.GenCode(", ");
            codeGenerator.GenCode("0x" + Convert.ToString(newStates[i],16));
        }
        codeGenerator.GenCodeLine("};");

        // Action functions.

        string staticString = Options.GetStatic() ? "static " : "";
        // Token actions.
        codeGenerator.GenCodeLine(
            staticString + "void TokenLexicalActions(Token matchedToken) {");
        DumpLexicalActions(allMatches, MatchTypes.TOKEN,
                           "matchedToken.kind", codeGenerator);
        codeGenerator.GenCodeLine("}");

        // Skip actions.
        // TODO(sreeni) : Streamline this mess.

        codeGenerator.GenCodeLine(
            staticString + "void SkipLexicalActions(Token matchedToken) {");
        DumpLexicalActions(allMatches, MatchTypes.SKIP,
                           "jjmatchedKind", codeGenerator);
        DumpLexicalActions(allMatches, MatchTypes.SPECIAL_TOKEN,
                           "jjmatchedKind", codeGenerator);
        codeGenerator.GenCodeLine("}");

        // More actions.
        codeGenerator.GenCodeLine(
            staticString + "void MoreLexicalActions() {");
        codeGenerator.GenCodeLine(
            "jjimageLen += (lengthOfMatch = jjmatchedPos + 1);");
        DumpLexicalActions(allMatches, MatchTypes.MORE,
                           "jjmatchedKind", codeGenerator);
        codeGenerator.GenCodeLine("}");

        codeGenerator.GenCodeLine("public String[] lexStateNames = {");
        for (int i = 0; i < tokenizerData.lexStateNames.Length; i++)
        {
            if (i > 0) codeGenerator.GenCode(", ");
            codeGenerator.GenCode("\"" + tokenizerData.lexStateNames[i] + "\"");
        }
        codeGenerator.GenCodeLine("};");
    }

    private static void DumpLexicalActions(
        Dictionary<int, TokenizerData.MatchInfo> allMatches,
        MatchTypes matchType, string kindString,
        CodeGenerator codeGenerator)
    {
        codeGenerator.GenCodeLine("  switch(" + kindString + ") {");
        foreach (int i in allMatches.Keys)
        {
            TokenizerData.MatchInfo matchInfo = allMatches[i];
            if (matchInfo.action == null ||
                matchInfo.matchType != matchType)
            {
                continue;
            }
            codeGenerator.GenCodeLine("    case " + i + ": {\n");
            codeGenerator.GenCodeLine("      " + matchInfo.action);
            codeGenerator.GenCodeLine("      break;");
            codeGenerator.GenCodeLine("    }");
        }
        codeGenerator.GenCodeLine("    default: break;");
        codeGenerator.GenCodeLine("  }");
    }

    private static void GenerateBitVector(
        string name, BitSet bits, CodeGenerator codeGenerator)
    {
        codeGenerator.GenCodeLine("private static final long[] " + name + " = {");
        long[] longs = bits.ToLongArray();
        for (int i = 0; i < longs.Length; i++)
        {
            if (i > 0) codeGenerator.GenCode(", ");
            codeGenerator.GenCode("0x" + Convert.ToString(longs[i]) + "L");
        }
        codeGenerator.GenCodeLine("};");
    }
}
