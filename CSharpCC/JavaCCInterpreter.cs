using org.javacc.parser;

public class JavaCCInterpreter
{
    public static void main(String[] args)
    {
        // Initialize all static state
        Main.reInitAll();
        JavaCCParser parser = null;
        for (int arg = 0; arg < args.Length - 2; arg++)
        {
            if (!Options.isOption(args[arg]))
            {
                Console.WriteLine("Argument \"" + args[arg] + "\" must be an option setting.");
                Environment.Exit(1);
            }
            Options.setCmdLineOption(args[arg]);
        }

        string input = "";
        string grammar = "";
        try
        {
            File fp = new File(args[^2]);
            byte[] buf = new byte[(int)fp.Length];
            new DataInputStream(
                new BufferedInputStream(
                    new FileInputStream(fp))).readFully(buf);
            grammar = new String(buf);
            File inputFile = new File(args[^1]);
            buf = new byte[(int)inputFile.Length];
            new DataInputStream(
                new BufferedInputStream(
                    new FileInputStream(inputFile))).readFully(buf);
            input = new String(buf);
        }
        catch (FileNotFoundException e)
        {
            //e.printStackTrace();
            Environment.Exit(1);
        }
        catch (Exception t)
        {
            Environment.Exit(1);
        }
        long l = System.currentTimeMillis();
        new JavaCCInterpreter().runTokenizer(grammar, input);
        Console.Error.WriteLine("Tokenized in: " + (System.currentTimeMillis() - l));
    }

    public void runTokenizer(string grammar, string input)
    {
        try
        {
            JavaCCParser parser = new JavaCCParser(new StringReader(grammar));
            parser.javacc_input();
            Semanticize.start();
            LexGen lg = new LexGen();
            lg.generateDataOnly = true;
            lg.start();
            TokenizerData td = LexGen.tokenizerData;
            if (JavaCCErrors.get_error_count() == 0)
            {
                tokenize(td, input);
            }
        }
        catch (MetaParseException e)
        {
            Console.WriteLine("Detected " + JavaCCErrors.get_error_count() +
                               " errors and "
                               + JavaCCErrors.get_warning_count() + " warnings.");
            Environment.Exit(1);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            Console.WriteLine("Detected " + (JavaCCErrors.get_error_count() + 1) +
                               " errors and "
                               + JavaCCErrors.get_warning_count() + " warnings.");
            Environment.Exit(1);
        }
    }

    public static void tokenize(TokenizerData td, string input)
    {
        // First match the string literals.
        int input_size = input.Length;
        int curPos = 0;
        int curLexState = td.defaultLexState;
        HashSet<int> curStates = new HashSet<int>();
        HashSet<int> newStates = new HashSet<int>();
        while (curPos < input_size)
        {
            int beg = curPos;
            int matchedPos = beg;
            int matchedKind = int.MAX_VALUE;
            int nfaStartState = td.initialStates.get(curLexState);

            char c = input.charAt(curPos);
            if (Options.getIgnoreCase()) c = Character.toLowerCase(c);
            int key = curLexState << 16 | (int)c;
            List<String> literals = td.literalSequence.get(key);
            if (literals != null)
            {
                // We need to go in order so that the longest match works.
                int litIndex = 0;
                foreach (string s in literals)
                {
                    int index = 1;
                    // See which literal matches.
                    while (index < s.Length && curPos + index < input_size)
                    {
                        c = input.charAt(curPos + index);
                        if (Options.getIgnoreCase()) c = Character.toLowerCase(c);
                        if (c != s.charAt(index)) break;
                        index++;
                    }
                    if (index == s.Length)
                    {
                        // Found a string literal match.
                        matchedKind = td.literalKinds.get(key).get(litIndex);
                        matchedPos = curPos + index - 1;
                        nfaStartState = td.kindToNfaStartState.get(matchedKind);
                        curPos += index;
                        break;
                    }
                    litIndex++;
                }
            }

            if (nfaStartState != -1)
            {
                // We need to add the composite states first.
                int kind = int.MAX_VALUE;
                curStates.Add(nfaStartState);
                curStates.addAll(td.nfa.get(nfaStartState).compositeStates);
                do
                {
                    c = input.charAt(curPos);
                    if (Options.getIgnoreCase()) c = Character.toLowerCase(c);
                    for (int state : curStates)
                    {
                        TokenizerData.NfaState nfaState = td.nfa.get(state);
                        if (nfaState.characters.Contains(c))
                        {
                            if (kind > nfaState.kind)
                            {
                                kind = nfaState.kind;
                            }
                            newStates.addAll(nfaState.nextStates);
                        }
                    }
                    HashSet<int> tmp = newStates;
                    newStates = curStates;
                    curStates = tmp;
                    newStates.Clear();
                    if (kind != int.MAX_VALUE)
                    {
                        matchedKind = kind;
                        matchedPos = curPos;
                        kind = int.MAX_VALUE;
                    }
                } while (!curStates.isEmpty() && ++curPos < input_size);
            }
            if (matchedPos == beg && matchedKind > td.wildcardKind.get(curLexState))
            {
                matchedKind = td.wildcardKind.get(curLexState);
            }
            if (matchedKind != int.MAX_VALUE)
            {
                TokenizerData.MatchInfo matchInfo = td.allMatches.get(matchedKind);
                if (matchInfo.action != null)
                {
                    Console.Error.WriteLine(
                        "Actions not implemented (yet) in intererpreted mode");
                }
                if (matchInfo.matchType == TokenizerData.MatchType.TOKEN)
                {
                    Console.Error.WriteLine("Token: " + matchedKind + "; image: \"" +
                                       input.substring(beg, matchedPos + 1) + "\"");
                }
                if (matchInfo.newLexState != -1)
                {
                    curLexState = matchInfo.newLexState;
                }
                curPos = matchedPos + 1;
            }
            else
            {
                Console.Error.WriteLine("Encountered token error at char: " +
                                   input.charAt(curPos));
                Environment.Exit(1);
            }
        }
        Console.Error.WriteLine("Matched EOF");
    }
}
