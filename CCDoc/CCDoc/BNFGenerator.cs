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
using CSharpCC.Parser;

namespace CSharpCC.CCDoc;

public class BNFGenerator : Generator
{
    private readonly Dictionary<string, string> IdMap = new();
    private int id = 1;
    protected TextWriter? writer;
    private bool printing = true;

    protected string GetId(string nt)
    {
        if (IdMap.TryGetValue(nt, out var i))
        {
            i = "prod" + id++;
            IdMap.Add(nt, i);
        }
        return i;
    }

    protected TextWriter CreateOutputStream()
    {

        if (CCDocOptions.GetOutputFile() == (""))
        {
            if (CCDocGlobals.InputFile == ("standard input"))
            {
                return Console.Out;
            }
            else
            {
                var ext = ".bnf";
                var i = CCDocGlobals.InputFile.LastIndexOf('.');
                if (i == -1)
                {
                    CCDocGlobals.OutputFile = CCDocGlobals.InputFile + ext;
                }
                else
                {
                    var suffix = CCDocGlobals.InputFile[i..];
                    if (suffix == (ext))
                    {
                        CCDocGlobals.OutputFile = CCDocGlobals.InputFile + ext;
                    }
                    else
                    {
                        CCDocGlobals.OutputFile = CCDocGlobals.InputFile[..i]
                            + ext;
                    }
                }
            }
        }
        else
        {
            CCDocGlobals.OutputFile = CCDocOptions.GetOutputFile();
        }

        try
        {
            writer = new StreamWriter(CCDocGlobals.OutputFile);
        }
        catch (IOException)
        {
            Error($"CCDoc: can't open output stream on file {CCDocGlobals.OutputFile}.  Using standard output.");
            writer = Console.Out;
        }

        return writer;
    }

    private void Println(string s)
    {
        Print(s + "\n");
    }

    public void Text(string s)
    {
        if (printing && !(s.Length == 1 && (s[0] == '\n' || s[0] == '\r')))
        {
            Print(s);
        }
    }
    public void Print(string s)
    {
        writer.Write(s);
    }

    public void DocumentStart()
    {
        writer = CreateOutputStream();
    }
    public void DocumentEnd()
    {
        writer.Close();
    }
    public void SpecialTokens(string s)
    {
    }
    //  public void tokenStart(TokenProduction tp) {
    //    printing = false;
    //  }
    //  public void tokenEnd(TokenProduction tp) {
    //    printing = true;
    //  }
    public void NonterminalsStart() { }
    public void NonterminalsEnd() { }
    //@Override
    public void TokensStart() { }
    //@Override 
    public void TokensEnd() { }
    public void Javacode(CSharpCodeProduction jp) { }
    public void Cppcode(CppCodeProduction cp) { }
    public void ExpansionEnd(Expansion e, bool first) { }
    public void NonTerminalStart(NonTerminal nt) { }
    public void NonTerminalEnd(NonTerminal nt) { }
    public void ProductionStart(NormalProduction np)
    {
        Println("");
        Print(np.Lhs + " ::= ");
    }
    public void ProductionEnd(NormalProduction np)
    {
        Println("");
    }
    public void ExpansionStart(Expansion e, bool first)
    {
        if (!first)
        {
            Print(" | ");
        }
    }
    public void ReStart(RegularExpression r)
    {
        if (r is RJustName || r is RCharacterList)
        {
            printing = false;
        }
    }
    public void ReEnd(RegularExpression r)
    {
        printing = true;
    }

    public void Debug(string message)
        => Console.Error.WriteLine(message);
    public void Info(string message)
        => Console.Error.WriteLine(message);
    public void Warn(string message)
        => Console.Error.WriteLine(message);
    public void Error(string message)
        => Console.Error.WriteLine(message);

    public void HandleTokenProduction(TokenProduction tp)
    {
        printing = false;
        var _text = CCDoc.GetStandardTokenProductionText(tp);
        Text(_text);
        printing = true;
    }
}
