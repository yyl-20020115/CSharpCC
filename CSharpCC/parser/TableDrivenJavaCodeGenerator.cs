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
        options.Add("charStreamName", CodeGenerator.getCharStreamName());
        options.Add("defaultLexState", tokenizerData.defaultLexState);
        options.Add("decls", tokenizerData.decls);
        options.Add("superClass", (superClass == null || superClass == (""))
                          ? "" : "extends " + superClass);
        options.Add("noDfa", Options.getNoDfa());
        options.Add("generatedStates", tokenizerData.nfa.Count);
        try
        {
            codeGenerator.writeTemplate(TokenManagerTemplate, options);
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
        codeGenerator.genCodeLine("\n}");
        if (!Options.getBuildParser()) return;
        string fileName = Options.getOutputDirectory() + File.separator +
                          tokenizerData.parserName + "TokenManager.java";
        codeGenerator.saveOutput(fileName);
    }

    private void dumpDfaTables(
        CodeGenerator codeGenerator, TokenizerData tokenizerData)
    {
        Dictionary<int, int[]> startAndSize = new Dictionary<int, int[]>();
        int i = 0;

        codeGenerator.genCodeLine(
            "private static final int[] stringLiterals = {");
        foreach (int key in tokenizerData.literalSequence.keySet())
        {
            int[] arr = new int[2];
            List<String> l = tokenizerData.literalSequence.get(key);
            List<int> kinds = tokenizerData.literalKinds.get(key);
            arr[0] = i;
            arr[1] = l.Count;
            int j = 0;
            if (i > 0) codeGenerator.genCodeLine(", ");
            foreach (string s in l)
            {
                if (j > 0) codeGenerator.genCodeLine(", ");
                codeGenerator.genCode(s.Length);
                for (int k = 0; k < s.Length; k++)
                {
                    codeGenerator.genCode(", ");
                    codeGenerator.genCode((int)s.charAt(k));
                    i++;
                }
                int kind = kinds[j];
                codeGenerator.genCode(", " + kind);
                codeGenerator.genCode(
                    ", " + tokenizerData.kindToNfaStartState.get(kind));
                i += 3;
                j++;
            }
            startAndSize.Add(key, arr);
        }
        codeGenerator.genCodeLine("};");

        codeGenerator.genCodeLine(
            "private static final Dictionary<int, int[]> startAndSize =\n" +
            "    new HashMap<Integer, int[]>();");

        // Static block to actually initialize the map from the int array above.
        codeGenerator.genCodeLine("static {");
        for (int key : tokenizerData.literalSequence.keySet())
        {
            int[] arr = startAndSize.get(key);
            codeGenerator.genCodeLine("startAndSize.Add(" + key + ", new int[]{" +
                                       arr[0] + ", " + arr[1] + "});");
        }
        codeGenerator.genCodeLine("}");
    }

    private void dumpNfaTables(
        CodeGenerator codeGenerator, TokenizerData tokenizerData)
    {
        // WE do the following for java so that the generated code is reasonable
        // size and can be compiled. May not be needed for other languages.
        codeGenerator.genCodeLine("private static final long[][] jjCharData = {");
        Dictionary<int, TokenizerData.NfaState> nfa = tokenizerData.nfa;
        for (int i = 0; i < nfa.Count; i++)
        {
            TokenizerData.NfaState tmp = nfa[i];
            if (i > 0) codeGenerator.genCodeLine(",");
            if (tmp == null)
            {
                codeGenerator.genCode("{}");
                continue;
            }
            codeGenerator.genCode("{");
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
                if (k > 0) codeGenerator.genCode(", ");
                codeGenerator.genCode(rep + ", ");
                codeGenerator.genCode("0x" + Long.toHexString(longs[k]) + "L");
                k += rep - 1;
            }
            codeGenerator.genCode("}");
        }
        codeGenerator.genCodeLine("};");

        codeGenerator.genCodeLine(
             "private static final long[][] jjChars = ");
        codeGenerator.genCodeLine(
             "    new long[" + tokenizerData.nfa.Count +
             "][(Character.MAX_VALUE >> 6) + 1]; ");
        codeGenerator.genCodeLine(
             "static { ");
        codeGenerator.genCodeLine(
             "  for (int i = 0; i < " + tokenizerData.nfa.Count + "; i++) { ");
        codeGenerator.genCodeLine(
             "    int ind = 0; ");
        codeGenerator.genCodeLine(
             "    for (int j = 0; j < jjCharData[i].length; j += 2) { ");
        codeGenerator.genCodeLine(
             "      for (int k = 0; k < (int)jjCharData[i][j]; k++) { ");
        codeGenerator.genCodeLine(
             "        jjChars[i][ind++] = jjCharData[i][j + 1]; ");
        codeGenerator.genCodeLine(
             "      } ");
        codeGenerator.genCodeLine(
             "    } ");
        codeGenerator.genCodeLine(
             "  } ");
        codeGenerator.genCodeLine(
             "} ");


        codeGenerator.genCodeLine(
            "private static final int[][] jjcompositeState = {");
        for (int i = 0; i < nfa.Count; i++)
        {
            TokenizerData.NfaState tmp = nfa[i];
            if (i > 0) codeGenerator.genCodeLine(", ");
            if (tmp == null)
            {
                codeGenerator.genCode("{}");
                continue;
            }
            codeGenerator.genCode("{");
            int k = 0;
            for (int st in tmp.compositeStates)
            {
                if (k++ > 0) codeGenerator.genCode(", ");
                codeGenerator.genCode(st);
            }
            codeGenerator.genCode("}");
        }
        codeGenerator.genCodeLine("};");

        codeGenerator.genCodeLine("private static final int[] jjmatchKinds = {");
        for (int i = 0; i < nfa.Count; i++)
        {
            TokenizerData.NfaState tmp = nfa[i];
            if (i > 0) codeGenerator.genCodeLine(", ");
            // TODO(sreeni) : Fix this mess.
            if (tmp == null)
            {
                codeGenerator.genCode(int.MaxValue);
                continue;
            }
            codeGenerator.genCode(tmp.kind);
        }
        codeGenerator.genCodeLine("};");

        codeGenerator.genCodeLine(
            "private static final int[][]  jjnextStateSet = {");
        for (int i = 0; i < nfa.Count; i++)
        {
            TokenizerData.NfaState tmp = nfa[i];
            if (i > 0) codeGenerator.genCodeLine(", ");
            if (tmp == null)
            {
                codeGenerator.genCode("{}");
                continue;
            }
            int k = 0;
            codeGenerator.genCode("{");
            foreach (int s in tmp.nextStates)
            {
                if (k++ > 0) codeGenerator.genCode(", ");
                codeGenerator.genCode(s);
            }
            codeGenerator.genCode("}");
        }
        codeGenerator.genCodeLine("};");

        codeGenerator.genCodeLine(
            "private static final int[] jjInitStates  = {");
        int k = 0;
        foreach (int i in tokenizerData.initialStates.keySet())
        {
            if (k++ > 0) codeGenerator.genCode(", ");
            codeGenerator.genCode(tokenizerData.initialStates[i]);
        }
        codeGenerator.genCodeLine("};");

        codeGenerator.genCodeLine(
            "private static final int[] canMatchAnyChar = {");
        k = 0;
        for (int i = 0; i < tokenizerData.wildcardKind.Count; i++)
        {
            if (k++ > 0) codeGenerator.genCode(", ");
            codeGenerator.genCode(tokenizerData.wildcardKind[i]);
        }
        codeGenerator.genCodeLine("};");
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
        codeGenerator.genCodeLine(
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
            if (k++ > 0) codeGenerator.genCodeLine(", ");
            if (image != null)
            {
                codeGenerator.genCode("\"");
                for (int j = 0; j < image.Length; j++)
                {
                    if (image.charAt(j) <= 0xff)
                    {
                        codeGenerator.genCode(
                            "\\" + int.toOctalString((int)image.charAt(j)));
                    }
                    else
                    {
                        string hexVal = int.toHexString((int)image.charAt(j));
                        if (hexVal.Length == 3)
                            hexVal = "0" + hexVal;
                        codeGenerator.genCode("\\u" + hexVal);
                    }
                }
                codeGenerator.genCode("\"");
            }
            else
            {
                codeGenerator.genCodeLine("null");
            }
        }
        codeGenerator.genCodeLine("};");

        // Now generate the bit masks.
        generateBitVector("jjtoSkip", toSkip, codeGenerator);
        generateBitVector("jjtoSpecial", toSpecial, codeGenerator);
        generateBitVector("jjtoMore", toMore, codeGenerator);
        generateBitVector("jjtoToken", toToken, codeGenerator);

        codeGenerator.genCodeLine("private static final int[] jjnewLexState = {");
        for (int i = 0; i < newStates.Length; i++)
        {
            if (i > 0) codeGenerator.genCode(", ");
            codeGenerator.genCode("0x" + int.toHexString(newStates[i]));
        }
        codeGenerator.genCodeLine("};");

        // Action functions.

        string staticString = Options.getStatic() ? "static " : "";
        // Token actions.
        codeGenerator.genCodeLine(
            staticString + "void TokenLexicalActions(Token matchedToken) {");
        dumpLexicalActions(allMatches, TokenizerData.MatchType.TOKEN,
                           "matchedToken.kind", codeGenerator);
        codeGenerator.genCodeLine("}");

        // Skip actions.
        // TODO(sreeni) : Streamline this mess.

        codeGenerator.genCodeLine(
            staticString + "void SkipLexicalActions(Token matchedToken) {");
        dumpLexicalActions(allMatches, TokenizerData.MatchType.SKIP,
                           "jjmatchedKind", codeGenerator);
        dumpLexicalActions(allMatches, TokenizerData.MatchType.SPECIAL_TOKEN,
                           "jjmatchedKind", codeGenerator);
        codeGenerator.genCodeLine("}");

        // More actions.
        codeGenerator.genCodeLine(
            staticString + "void MoreLexicalActions() {");
        codeGenerator.genCodeLine(
            "jjimageLen += (lengthOfMatch = jjmatchedPos + 1);");
        dumpLexicalActions(allMatches, TokenizerData.MatchType.MORE,
                           "jjmatchedKind", codeGenerator);
        codeGenerator.genCodeLine("}");

        codeGenerator.genCodeLine("public String[] lexStateNames = {");
        for (int i = 0; i < tokenizerData.lexStateNames.Length; i++)
        {
            if (i > 0) codeGenerator.genCode(", ");
            codeGenerator.genCode("\"" + tokenizerData.lexStateNames[i] + "\"");
        }
        codeGenerator.genCodeLine("};");
    }

    private void dumpLexicalActions(
        Dictionary<int, TokenizerData.MatchInfo> allMatches,
        TokenizerData.MatchType matchType, string kindString,
        CodeGenerator codeGenerator)
    {
        codeGenerator.genCodeLine("  switch(" + kindString + ") {");
        foreach (int i in allMatches.keySet())
        {
            TokenizerData.MatchInfo matchInfo = allMatches[i];
            if (matchInfo.action == null ||
                matchInfo.matchType != matchType)
            {
                continue;
            }
            codeGenerator.genCodeLine("    case " + i + ": {\n");
            codeGenerator.genCodeLine("      " + matchInfo.action);
            codeGenerator.genCodeLine("      break;");
            codeGenerator.genCodeLine("    }");
        }
        codeGenerator.genCodeLine("    default: break;");
        codeGenerator.genCodeLine("  }");
    }

    private static void generateBitVector(
        string name, BitSet bits, CodeGenerator codeGenerator)
    {
        codeGenerator.genCodeLine("private static final long[] " + name + " = {");
        long[] longs = bits.toLongArray();
        for (int i = 0; i < longs.Length; i++)
        {
            if (i > 0) codeGenerator.genCode(", ");
            codeGenerator.genCode("0x" + Long.toHexString(longs[i]) + "L");
        }
        codeGenerator.genCodeLine("};");
    }
}
