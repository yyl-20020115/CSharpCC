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
using org.javacc.parser;

namespace org.javacc.jjdoc;

public class BNFGenerator : Generator
{
    private Dictionary<string, string> id_map = new();
    private int id = 1;
    protected TextWriter ostr;
    private bool printing = true;

    protected string get_id(string nt)
    {
        if (id_map.TryGetValue(nt, out var i))
        {
            i = "prod" + id++;
            id_map.Add(nt, i);
        }
        return i;
    }

    protected TextWriter create_output_stream()
    {

        if (JJDocOptions.getOutputFile() == (""))
        {
            if (JJDocGlobals.input_file == ("standard input"))
            {
                return Console.Out;
            }
            else
            {
                string ext = ".bnf";
                int i = JJDocGlobals.input_file.LastIndexOf('.');
                if (i == -1)
                {
                    JJDocGlobals.output_file = JJDocGlobals.input_file + ext;
                }
                else
                {
                    string suffix = JJDocGlobals.input_file[i..];
                    if (suffix == (ext))
                    {
                        JJDocGlobals.output_file = JJDocGlobals.input_file + ext;
                    }
                    else
                    {
                        JJDocGlobals.output_file = JJDocGlobals.input_file[..i]
                            + ext;
                    }
                }
            }
        }
        else
        {
            JJDocGlobals.output_file = JJDocOptions.getOutputFile();
        }
        try
        {
            ostr = new StreamWriter(JJDocGlobals.output_file);
        }
        catch (IOException e)
        {
            error("JJDoc: can't open output stream on file "
                + JJDocGlobals.output_file + ".  Using standard output.");
            ostr = Console.Out;
        }

        return ostr;
    }

    private void WriteLine(string s)
    {
        print(s + "\n");
    }

    public void text(string s)
    {
        if (printing && !(s.Length == 1 && (s[0] == '\n' || s[0] == '\r')))
        {
            print(s);
        }
    }
    public void print(string s)
    {
        ostr.Write(s);
    }

    public void documentStart()
    {
        ostr = create_output_stream();
    }
    public void documentEnd()
    {
        ostr.Close();
    }
    public void specialTokens(string s)
    {
    }
    //  public void tokenStart(TokenProduction tp) {
    //    printing = false;
    //  }
    //  public void tokenEnd(TokenProduction tp) {
    //    printing = true;
    //  }
    public void nonterminalsStart() { }
    public void nonterminalsEnd() { }
    //@Override
    public void tokensStart() { }
    //@Override 
    public void tokensEnd() { }
    public void javacode(JavaCodeProduction jp) { }
    public void cppcode(CppCodeProduction cp) { }
    public void expansionEnd(Expansion e, bool first) { }
    public void nonTerminalStart(NonTerminal nt) { }
    public void nonTerminalEnd(NonTerminal nt) { }
    public void productionStart(NormalProduction np)
    {
        WriteLine("");
        print(np.getLhs() + " ::= ");
    }
    public void productionEnd(NormalProduction np)
    {
        WriteLine("");
    }
    public void expansionStart(Expansion e, bool first)
    {
        if (!first)
        {
            print(" | ");
        }
    }
    public void reStart(RegularExpression r)
    {
        if (r is RJustName || r is RCharacterList)
        {
            printing = false;
        }
    }
    public void reEnd(RegularExpression r)
    {
        printing = true;
    }

    public void debug(string message) { Console.Error.WriteLine(message); }
    public void info(string message) { Console.Error.WriteLine(message); }
    public void warn(string message) { Console.Error.WriteLine(message); }
    public void error(string message) { Console.Error.WriteLine(message); }

    //@Override
    public void handleTokenProduction(TokenProduction tp)
    {
        printing = false;
        string _text = JJDoc.getStandardTokenProductionText(tp);
        text(_text);
        printing = true;
    }



}
