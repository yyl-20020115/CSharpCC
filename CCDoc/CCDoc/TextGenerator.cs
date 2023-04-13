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



/**
 * Output BNF in text format.
 */
public class TextGenerator : Generator
{
    protected TextWriter writer;

    public TextGenerator()
    {
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#text(java.lang.String)
     */
    public virtual void Text(string s)
    {
        Print(s);
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#print(java.lang.String)
     */
    public virtual void Print(string s)
    {
        writer.Write(s);
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#documentStart()
     */
    public virtual void DocumentStart()
    {
        writer = CreateOutputStream();
        writer.Write("\nDOCUMENT START\n");
    }
    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#documentEnd()
     */
    public virtual void DocumentEnd()
    {
        writer.Write("\nDOCUMENT END\n");
        writer.Close();
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#specialTokens(java.lang.String)
     */
    public virtual void SpecialTokens(string s)
    {
        writer.Write(s);
    }


    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#nonterminalsStart()
     */
    public virtual void NonterminalsStart()
    {
        Text("NON-TERMINALS\n");
    }
    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#nonterminalsEnd()
     */
    public virtual void NonterminalsEnd()
    {
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#tokensStart()
     */
    public virtual void TokensStart()
    {
        Text("TOKENS\n");
    }

    //@Override
    public virtual void HandleTokenProduction(TokenProduction tp)
    {
        string t = CCDoc.GetStandardTokenProductionText(tp);
        Text(t);
    }


    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#tokensEnd()
     */
    public virtual void TokensEnd()
    {
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#javacode(org.javacc.parser.JavaCodeProduction)
     */
    public virtual void Javacode(CSharpCodeProduction jp)
    {
        ProductionStart(jp);
        Text("java code");
        ProductionEnd(jp);
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#cppcode(org.javacc.parser.CppCodeProduction)
     */
    public virtual void Cppcode(CppCodeProduction cp)
    {
        ProductionStart(cp);
        Text("c++ code");
        ProductionEnd(cp);
    }
    public virtual void CScode(CppCodeProduction cp)
    {
        ProductionStart(cp);
        Text("c# code");
        ProductionEnd(cp);
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#productionStart(org.javacc.parser.NormalProduction)
     */
    public virtual void ProductionStart(NormalProduction np)
    {
        writer.Write("\t" + np.Lhs + "\t:=\t");
    }
    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#productionEnd(org.javacc.parser.NormalProduction)
     */
    public virtual void ProductionEnd(NormalProduction np)
    {
        writer.Write("\n");
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#expansionStart(org.javacc.parser.Expansion, boolean)
     */
    public virtual void ExpansionStart(Expansion e, bool first)
    {
        if (!first)
        {
            writer.Write("\n\t\t|\t");
        }
    }
    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#expansionEnd(org.javacc.parser.Expansion, boolean)
     */
    public virtual void ExpansionEnd(Expansion e, bool first)
    {
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#nonTerminalStart(org.javacc.parser.NonTerminal)
     */
    public virtual void NonTerminalStart(NonTerminal nt)
    {
    }
    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#nonTerminalEnd(org.javacc.parser.NonTerminal)
     */
    public virtual void NonTerminalEnd(NonTerminal nt)
    {
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#reStart(org.javacc.parser.RegularExpression)
     */
    public virtual void ReStart(RegularExpression r)
    {
    }
    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#reEnd(org.javacc.parser.RegularExpression)
     */
    public virtual void ReEnd(RegularExpression r)
    {
    }

    /**
     * Create an output stream for the generated Jack code. Try to open a file
     * based on the name of the parser, but if that fails use the standard output
     * stream.
     */
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
                string ext = ".html";

                if (CCDocOptions.GetText())
                {
                    ext = ".txt";
                }
                else if (CCDocOptions.GetXText())
                {
                    ext = ".xtext";
                }

                int i = CCDocGlobals.InputFile.LastIndexOf('.');
                if (i == -1)
                {
                    CCDocGlobals.OutputFile = CCDocGlobals.InputFile + ext;
                }
                else
                {
                    string suffix = CCDocGlobals.InputFile[i..];
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
        catch (IOException e)
        {
            Error("CCDoc: can't open output stream on file "
                + CCDocGlobals.OutputFile + ".  Using standard output.");
            writer = Console.Out;
        }

        return writer;
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#debug(java.lang.String)
     */
    public void Debug(string message)
    {
        Console.Error.WriteLine(message);
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#info(java.lang.String)
     */
    public void Info(string message)
    {
        Console.Error.WriteLine(message);
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#warn(java.lang.String)
     */
    public void Warn(string message)
    {
        Console.Error.WriteLine(message);
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#error(java.lang.String)
     */
    public void Error(string message)
    {
        Console.Error.WriteLine(message);
    }
}
