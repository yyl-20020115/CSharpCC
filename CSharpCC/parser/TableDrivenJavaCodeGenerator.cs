namespace org.javacc.parser;


/**
 * Class that implements a table driven code generator for the token manager in
 * java.
 */
public class TableDrivenJavaCodeGenerator : TokenManagerCodeGenerator
{
    private static readonly string TokenManagerTemplate =
        "/templates/TableDrivenTokenManager.template";
    private CodeGenerator codeGenerator = new CodeGenerator();

    //@Override
    public void GenerateCode(TokenizerData tokenizerData)
    {
        string superClass = (String)Options.getOptions().get(
                                 Options.USEROPTION__TOKEN_MANAGER_SUPER_CLASS);
        Dictionary<String, Object> options = Options.getOptions();
        options.Add("maxOrdinal", tokenizerData.allMatches.Count);
        options.Add("maxLexStates", tokenizerData.lexStateNames.Length);
        options.Add("stateSetSize", tokenizerData.nfa.Count);
        options.Add("parserName", tokenizerData.parserName);
        options.Add("maxLongs", tokenizerData.allMatches.Count / 64 + 1);
        options.Add("parserName", tokenizerData.parserName);
        options.Add("charStreamName", CodeGenerator.GetCharStreamName());
        options.Add("defaultLexState", tokenizerData.defaultLexState);
        options.Add("decls", tokenizerData.decls);
        options.Add("superClass", (superClass == null || superClass == (""))
                          ? "" : "extends " + superClass);
        options.Add("noDfa", Options.getNoDfa());
        options.Add("generatedStates", tokenizerData.nfa.Count);
        try
        {
            codeGenerator.WriteTemplate(TokenManagerTemplate, options);
            dumpDfaTables(codeGenerator, tokenizerData);
            dumpNfaTables(codeGenerator, tokenizerData);
            dumpMatchInfo(codeGenerator, tokenizerData);
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
        if (!Options.getBuildParser()) return;
        string fileName = Options.getOutputDirectory() + File.separator +
                          tokenizerData.parserName + "TokenManager.java";
        codeGenerator.SaveOutput(fileName);
    }

    private void dumpDfaTables(
        CodeGenerator codeGenerator, TokenizerData tokenizerData)
    {
        Dictionary<int, int[]> startAndSize = new Dictionary<int, int[]>();
        int i = 0;

        codeGenerator.GenCodeLine(
            "private static final int[] stringLiterals = {");
        foreach (int key in tokenizerData.literalSequence.keySet())
        {
            int[] arr = new int[2];
            List<string> l = tokenizerData.literalSequence.get(key);
            List<int> kinds = tokenizerData.literalKinds.get(key);
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
                    codeGenerator.GenCode((int)s.charAt(k));
                    i++;
                }
                int kind = kinds[j];
                codeGenerator.GenCode(", " + kind);
                codeGenerator.genCode(
                    ", " + tokenizerData.kindToNfaStartState.get(kind));
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
        for (int key : tokenizerData.literalSequence.keySet())
        {
            int[] arr = startAndSize.get(key);
            codeGenerator.GenCodeLine("startAndSize.Add(" + key + ", new int[]{" +
                                       arr[0] + ", " + arr[1] + "});");
        }
        codeGenerator.GenCodeLine("}");
    }

    private void dumpNfaTables(
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
            BitSet bits = new BitSet();
            for (char c in tmp.characters)
            {
                bits.set(c);
            }
            long[] longs = bits.toLongArray();
            for (int k = 0; k < longs.Length; k++)
            {
                int rep = 1;
                while (k + rep < longs.Length && longs[k + rep] == longs[k]) rep++;
                if (k > 0) codeGenerator.GenCode(", ");
                codeGenerator.GenCode(rep + ", ");
                codeGenerator.genCode("0x" + Long.toHexString(longs[k]) + "L");
                k += rep - 1;
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
            int k = 0;
            for (int st in tmp.compositeStates)
            {
                if (k++ > 0) codeGenerator.GenCode(", ");
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
            int k = 0;
            codeGenerator.GenCode("{");
            foreach (int s in tmp.nextStates)
            {
                if (k++ > 0) codeGenerator.GenCode(", ");
                codeGenerator.GenCode(s);
            }
            codeGenerator.GenCode("}");
        }
        codeGenerator.GenCodeLine("};");

        codeGenerator.GenCodeLine(
            "private static final int[] jjInitStates  = {");
        int k = 0;
        foreach (int i in tokenizerData.initialStates.keySet())
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

    private void dumpMatchInfo(
        CodeGenerator codeGenerator, TokenizerData tokenizerData)
    {
        Dictionary<int, TokenizerData.MatchInfo> allMatches =
            tokenizerData.allMatches;

        // A bit ugly.

        BitSet toSkip = new BitSet(allMatches.Count);
        BitSet toSpecial = new BitSet(allMatches.Count);
        BitSet toMore = new BitSet(allMatches.Count);
        BitSet toToken = new BitSet(allMatches.Count);
        int[] newStates = new int[allMatches.Count];
        toSkip.set(allMatches.Count + 1, true);
        toToken.set(allMatches.Count + 1, true);
        toMore.set(allMatches.Count + 1, true);
        toSpecial.set(allMatches.Count + 1, true);
        // Kind map.
        codeGenerator.GenCodeLine(
            "public static final String[] jjstrLiteralImages = {");

        int k = 0;
        foreach (int i in allMatches.keySet())
        {
            TokenizerData.MatchInfo matchInfo = allMatches[i];
            switch (matchInfo.matchType)
            {
                case SKIP: toSkip.set(i); break;
                case SPECIAL_TOKEN: toSpecial.set(i); break;
                case MORE: toMore.set(i); break;
                case TOKEN: toToken.set(i); break;
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
                        codeGenerator.genCode(
                            "\\" + int.toOctalString((int)image[j]));
                    }
                    else
                    {
                        string hexVal = int.toHexString((int)image[j]);
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
        generateBitVector("jjtoSkip", toSkip, codeGenerator);
        generateBitVector("jjtoSpecial", toSpecial, codeGenerator);
        generateBitVector("jjtoMore", toMore, codeGenerator);
        generateBitVector("jjtoToken", toToken, codeGenerator);

        codeGenerator.GenCodeLine("private static final int[] jjnewLexState = {");
        for (int i = 0; i < newStates.Length; i++)
        {
            if (i > 0) codeGenerator.GenCode(", ");
            codeGenerator.genCode("0x" + int.toHexString(newStates[i]));
        }
        codeGenerator.GenCodeLine("};");

        // Action functions.

        string staticString = Options.getStatic() ? "static " : "";
        // Token actions.
        codeGenerator.GenCodeLine(
            staticString + "void TokenLexicalActions(Token matchedToken) {");
        dumpLexicalActions(allMatches, TokenizerData.MatchType.TOKEN,
                           "matchedToken.kind", codeGenerator);
        codeGenerator.GenCodeLine("}");

        // Skip actions.
        // TODO(sreeni) : Streamline this mess.

        codeGenerator.GenCodeLine(
            staticString + "void SkipLexicalActions(Token matchedToken) {");
        dumpLexicalActions(allMatches, TokenizerData.MatchType.SKIP,
                           "jjmatchedKind", codeGenerator);
        dumpLexicalActions(allMatches, TokenizerData.MatchType.SPECIAL_TOKEN,
                           "jjmatchedKind", codeGenerator);
        codeGenerator.GenCodeLine("}");

        // More actions.
        codeGenerator.GenCodeLine(
            staticString + "void MoreLexicalActions() {");
        codeGenerator.GenCodeLine(
            "jjimageLen += (lengthOfMatch = jjmatchedPos + 1);");
        dumpLexicalActions(allMatches, TokenizerData.MatchType.MORE,
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

    private void dumpLexicalActions(
        Dictionary<int, TokenizerData.MatchInfo> allMatches,
        TokenizerData.MatchType matchType, string kindString,
        CodeGenerator codeGenerator)
    {
        codeGenerator.GenCodeLine("  switch(" + kindString + ") {");
        foreach (int i in allMatches.keySet())
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

    private static void generateBitVector(
        string name, BitSet bits, CodeGenerator codeGenerator)
    {
        codeGenerator.GenCodeLine("private static final long[] " + name + " = {");
        long[] longs = bits.toLongArray();
        for (int i = 0; i < longs.Length; i++)
        {
            if (i > 0) codeGenerator.GenCode(", ");
            codeGenerator.genCode("0x" + Long.toHexString(longs[i]) + "L");
        }
        codeGenerator.GenCodeLine("};");
    }
}
