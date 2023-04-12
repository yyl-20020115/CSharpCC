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
 * Output BNF in HTML 3.2 format.
 */
public class HTMLGenerator : TextGenerator
{
    private Dictionary<string, string> id_map = new();
    private int id = 1;

    public HTMLGenerator()
        : base()
    {
    }

    protected string get_id(string nt)
    {
        if (!id_map.TryGetValue(nt,out var i))
        {
            i = "prod" + id++;
            id_map.Add(nt, i);
        }
        return i;
    }

    private void println(string s)
    {
        print(s + "\n");
    }

    public override void text(string s)
    {
        string ss = "";
        for (int i = 0; i < s.Length; ++i)
        {
            if (s[i] == '<')
            {
                ss += "&lt;";
            }
            else if (s[i] == '>')
            {
                ss += "&gt;";
            }
            else if (s[i] == '&')
            {
                ss += "&amp;";
            }
            else
            {
                ss += s[i];
            }
        }
        print(ss);
    }

    public override void print(string s)
    {
        ostr.Write(s);
    }

    public override void documentStart()
    {
        ostr = create_output_stream();
        println("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 3.2//EN\">");
        println("<HTML>");
        println("<HEAD>");
        if ("" != (JJDocOptions.getCSS()))
        {
            println("<LINK REL=\"stylesheet\" type=\"text/css\" href=\"" + JJDocOptions.getCSS() + "\"/>");
        }
        if (JJDocGlobals.input_file != null)
        {
            println("<TITLE>BNF for " + JJDocGlobals.input_file + "</TITLE>");
        }
        else
        {
            println("<TITLE>A BNF grammar by JJDoc</TITLE>");
        }
        println("</HEAD>");
        println("<BODY>");
        println("<H1 ALIGN=CENTER>BNF for " + JJDocGlobals.input_file + "</H1>");
    }

    public override void documentEnd()
    {
        println("</BODY>");
        println("</HTML>");
        ostr.Close();
    }

    /**
     * Prints out comments, used for tokens and non-terminals.
     * {@inheritDoc}
     * @see org.javacc.jjdoc.TextGenerator#specialTokens(java.lang.String)
     */
    public override void specialTokens(string s)
    {
        println(" <!-- Special token -->");
        println(" <TR>");
        println("  <TD>");
        println("<PRE>");
        print(s);
        println("</PRE>");
        println("  </TD>");
        println(" </TR>");
    }

    public override void handleTokenProduction(TokenProduction tp)
    {
        println(" <!-- Token -->");
        println(" <TR>");
        println("  <TD>");
        println("   <PRE>");
        string _text = JJDoc.getStandardTokenProductionText(tp);
        text(_text);
        println("   </PRE>");
        println("  </TD>");
        println(" </TR>");
    }


    public override void nonterminalsStart()
    {
        println("<H2 ALIGN=CENTER>NON-TERMINALS</H2>");
        if (JJDocOptions.getOneTable())
        {
            println("<TABLE>");
        }
    }
    public void nonterminalsEnd()
    {
        if (JJDocOptions.getOneTable())
        {
            println("</TABLE>");
        }
    }

    public void tokensStart()
    {
        println("<H2 ALIGN=CENTER>TOKENS</H2>");
        println("<TABLE>");
    }
    public void tokensEnd()
    {
        println("</TABLE>");
    }

    public void javacode(JavaCodeProduction jp)
    {
        productionStart(jp);
        println("<I>java code</I></TD></TR>");
        productionEnd(jp);
    }

    public void cppcode(CppCodeProduction cp)
    {
        productionStart(cp);
        println("<I>cpp code</I></TD></TR>");
        productionEnd(cp);
    }

    public void productionStart(NormalProduction np)
    {
        if (!JJDocOptions.getOneTable())
        {
            println("");
            println("<TABLE ALIGN=CENTER>");
            println("<CAPTION><STRONG>" + np.getLhs() + "</STRONG></CAPTION>");
        }
        println("<TR>");
        println("<TD ALIGN=RIGHT VALIGN=BASELINE><A NAME=\"" + get_id(np.getLhs()) + "\">" + np.getLhs() + "</A></TD>");
        println("<TD ALIGN=CENTER VALIGN=BASELINE>::=</TD>");
        print("<TD ALIGN=LEFT VALIGN=BASELINE>");
    }
    public void productionEnd(NormalProduction np)
    {
        if (!JJDocOptions.getOneTable())
        {
            println("</TABLE>");
            println("<HR>");
        }
    }

    public void expansionStart(Expansion e, bool first)
    {
        if (!first)
        {
            println("<TR>");
            println("<TD ALIGN=RIGHT VALIGN=BASELINE></TD>");
            println("<TD ALIGN=CENTER VALIGN=BASELINE>|</TD>");
            print("<TD ALIGN=LEFT VALIGN=BASELINE>");
        }
    }
    public void expansionEnd(Expansion e, bool first)
    {
        println("</TD>");
        println("</TR>");
    }

    public void nonTerminalStart(NonTerminal nt)
    {
        print("<A HREF=\"#" + get_id(nt.getName()) + "\">");
    }
    public void nonTerminalEnd(NonTerminal nt)
    {
        print("</A>");
    }

    public void reStart(RegularExpression r)
    {
    }
    public void reEnd(RegularExpression r)
    {
    }
}
