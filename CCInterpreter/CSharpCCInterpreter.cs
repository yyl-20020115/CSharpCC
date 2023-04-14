using CSharpCC.Parser;

namespace CSharpCC.Interpreter;

public class CSharpCCInterpreter
{
    public static void Main(string[] args)
    {
        // Initialize all static state
        MainParser.ReInitAll();
        for (int arg = 0; arg < args.Length - 2; arg++)
        {
            if (!Options.IsOption(args[arg]))
            {
                Console.WriteLine($"Argument \"{args[arg]}\" must be an option setting.");
                Environment.Exit(1);
            }
            Options.SetCmdLineOption(args[arg]);
        }

        var input = string.Empty;
        var grammar = string.Empty;
        try
        {
            var fp = args[^2];
            var inputFile = args[^1];
            grammar = File.ReadAllText(fp);
            input = File.ReadAllText(inputFile);
        }
        catch (FileNotFoundException e)
        {
            //e.printStackTrace();
            Environment.Exit(1);
        }
        catch (Exception)
        {
            Environment.Exit(1);
        }
        var start = DateTime.Now.Millisecond;
        new CSharpCCInterpreter().RunTokenizer(grammar, input);
        Console.Error.WriteLine("Tokenized in: " + (DateTime.Now.Millisecond - start));
    }

    public void RunTokenizer(string grammar, string input)
    {
        try
        {
            var Parser = new CSharpCCParser(new StringReader(grammar));
            Parser.CSharpCC_Input();
            Semanticize.Start();
            LexGen.GenerateDataOnly = true;
            var lg = new LexGen();
            lg.Start();
            TokenizerData td = LexGen.TokenizerData;
            if (CSharpCCErrors.ErrorCount == 0)
            {
                Tokenize(td, input);
            }
        }
        catch (MetaParseException e)
        {
            Console.WriteLine("Detected " + CSharpCCErrors.ErrorCount +
                               " errors and "
                               + CSharpCCErrors.WarningCount + " warnings.");
            Environment.Exit(1);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            Console.WriteLine("Detected " + (CSharpCCErrors.ErrorCount + 1) +
                               " errors and "
                               + CSharpCCErrors.WarningCount + " warnings.");
            Environment.Exit(1);
        }
    }

    public static void Tokenize(TokenizerData td, string input)
    {
        // First match the string literals.
        int input_size = input.Length;
        int curPos = 0;
        int curLexState = td.defaultLexState;
        HashSet<int> curStates = new();
        HashSet<int> newStates = new();
        while (curPos < input_size)
        {
            int beg = curPos;
            int matchedPos = beg;
            int matchedKind = int.MaxValue;
            if(td.initialStates.TryGetValue(curLexState,out var nfaStartState))
            {

            }
            char c = input[curPos];
            if (Options.GetIgnoreCase()) c = char.ToLower(c);
            int key = curLexState << 16 | (int)c;
            
            if (td.literalSequence.TryGetValue(key, out var literals))
            {
                // We need to go in order so that the longest match works.
                int litIndex = 0;
                foreach (string s in literals)
                {
                    int index = 1;
                    // See which literal matches.
                    while (index < s.Length && curPos + index < input_size)
                    {
                        c = input[curPos + index];
                        if (Options.GetIgnoreCase()) c = char.ToLower(c);
                        if (c != s[index]) break;
                        index++;
                    }
                    if (index == s.Length)
                    {
                        // Found a string literal match.
                        if(td.literalKinds.TryGetValue(key,out var rets))
                        {
                            matchedKind = rets[litIndex];
                            matchedPos = curPos + index - 1;
                            if (td.kindToNfaStartState.TryGetValue(matchedKind, out nfaStartState))
                            {

                            }
                            curPos += index;
                        }
                        break;
                    }
                    litIndex++;
                }
            }

            if (nfaStartState != -1)
            {
                // We need to add the composite states first.
                int kind = int.MaxValue;
                curStates.Add(nfaStartState);
                if(td.nfa.TryGetValue(nfaStartState,out var sret))
                {
                    curStates.UnionWith(sret.compositeStates);
                }
                do
                {
                    c = input[curPos];
                    if (Options.GetIgnoreCase()) c = char.ToLower(c);
                    foreach (int state in curStates)
                    {
                        if (td.nfa.TryGetValue(state,out var nfaState) && nfaState.characters.Contains(c))
                        {
                            if (kind > nfaState.kind)
                            {
                                kind = nfaState.kind;
                            }
                            newStates.UnionWith(nfaState.nextStates);
                        }
                    }
                    (curStates, newStates) = (newStates, curStates);
                    newStates.Clear();
                    if (kind != int.MaxValue)
                    {
                        matchedKind = kind;
                        matchedPos = curPos;
                        kind = int.MaxValue;
                    }
                } while (curStates.Count>0 && ++curPos < input_size);
            }
            if ( matchedPos == beg && td.wildcardKind.TryGetValue(curLexState,out var ret) && matchedKind > ret)
            {
                matchedKind = ret;
            }
            if (matchedKind != int.MaxValue)
            {
                if (td.allMatches.TryGetValue(matchedKind,out var matchInfo) 
                    && matchInfo?.action != null)
                {
                    Console.Error.WriteLine(
                        "Actions not implemented (yet) in intererpreted mode");
                }
                if (matchInfo.matchType == MatchTypes.TOKEN)
                {
                    Console.Error.WriteLine("Token: " + matchedKind + "; image: \"" +
                                       input[beg.. (matchedPos + 1)] + "\"");
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
                                   input[curPos]);
                Environment.Exit(1);
            }
        }
        Console.Error.WriteLine("Matched EOF");
    } 
}
