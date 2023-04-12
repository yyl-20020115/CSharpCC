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



/**
 * Output BNF in text format.
 */
public class TextGenerator : Generator
{
    protected TextWriter ostr;

    public TextGenerator()
    {
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#text(java.lang.String)
     */
    public virtual void text(string s)
    {
        print(s);
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#print(java.lang.String)
     */
    public virtual void print(string s)
    {
        ostr.Write(s);
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#documentStart()
     */
    public virtual void documentStart()
    {
        ostr = create_output_stream();
        ostr.Write("\nDOCUMENT START\n");
    }
    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#documentEnd()
     */
    public virtual void documentEnd()
    {
        ostr.Write("\nDOCUMENT END\n");
        ostr.Close();
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#specialTokens(java.lang.String)
     */
    public virtual void specialTokens(string s)
    {
        ostr.Write(s);
    }


    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#nonterminalsStart()
     */
    public virtual void nonterminalsStart()
    {
        text("NON-TERMINALS\n");
    }
    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#nonterminalsEnd()
     */
    public virtual void nonterminalsEnd()
    {
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#tokensStart()
     */
    public virtual void tokensStart()
    {
        text("TOKENS\n");
    }

    //@Override
    public virtual void handleTokenProduction(TokenProduction tp)
    {
        string t = JJDoc.getStandardTokenProductionText(tp);
        text(t);
    }


    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#tokensEnd()
     */
    public virtual void tokensEnd()
    {
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#javacode(org.javacc.parser.JavaCodeProduction)
     */
    public virtual void javacode(JavaCodeProduction jp)
    {
        productionStart(jp);
        text("java code");
        productionEnd(jp);
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#cppcode(org.javacc.parser.CppCodeProduction)
     */
    public virtual void cppcode(CppCodeProduction cp)
    {
        productionStart(cp);
        text("c++ code");
        productionEnd(cp);
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#productionStart(org.javacc.parser.NormalProduction)
     */
    public virtual void productionStart(NormalProduction np)
    {
        ostr.Write("\t" + np.getLhs() + "\t:=\t");
    }
    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#productionEnd(org.javacc.parser.NormalProduction)
     */
    public virtual void productionEnd(NormalProduction np)
    {
        ostr.Write("\n");
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#expansionStart(org.javacc.parser.Expansion, boolean)
     */
    public virtual void expansionStart(Expansion e, bool first)
    {
        if (!first)
        {
            ostr.Write("\n\t\t|\t");
        }
    }
    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#expansionEnd(org.javacc.parser.Expansion, boolean)
     */
    public virtual void expansionEnd(Expansion e, bool first)
    {
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#nonTerminalStart(org.javacc.parser.NonTerminal)
     */
    public virtual void nonTerminalStart(NonTerminal nt)
    {
    }
    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#nonTerminalEnd(org.javacc.parser.NonTerminal)
     */
    public virtual void nonTerminalEnd(NonTerminal nt)
    {
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#reStart(org.javacc.parser.RegularExpression)
     */
    public virtual void reStart(RegularExpression r)
    {
    }
    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#reEnd(org.javacc.parser.RegularExpression)
     */
    public virtual void reEnd(RegularExpression r)
    {
    }

    /**
     * Create an output stream for the generated Jack code. Try to open a file
     * based on the name of the parser, but if that fails use the standard output
     * stream.
     */
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
                string ext = ".html";

                if (JJDocOptions.getText())
                {
                    ext = ".txt";
                }
                else if (JJDocOptions.getXText())
                {
                    ext = ".xtext";
                }

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

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#debug(java.lang.String)
     */
    public void debug(string message)
    {
        Console.Error.WriteLine(message);
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#info(java.lang.String)
     */
    public void info(string message)
    {
        Console.Error.WriteLine(message);
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#warn(java.lang.String)
     */
    public void warn(string message)
    {
        Console.Error.WriteLine(message);
    }

    /**
     * {@inheritDoc}
     * @see org.javacc.jjdoc.Generator#error(java.lang.String)
     */
    public void error(string message)
    {
        Console.Error.WriteLine(message);
    }







}
