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
    private readonly Dictionary<string, string> IdMap = new();
    private int id = 1;

    public HTMLGenerator()
        : base()
    {
    }

    protected string GetId(string nt)
    {
        if (!IdMap.TryGetValue(nt,out var i))
        {
            i = "prod" + id++;
            IdMap.Add(nt, i);
        }
        return i;
    }

    private void Println(string s)
    {
        Print(s + "\n");
    }

    public override void Text(string s)
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
        Print(ss);
    }

    public override void Print(string s)
    {
        ostr.Write(s);
    }

    public override void DocumentStart()
    {
        ostr = CreateOutputStream();
        Println("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 3.2//EN\">");
        Println("<HTML>");
        Println("<HEAD>");
        if ("" != (JJDocOptions.GetCSS()))
        {
            Println("<LINK REL=\"stylesheet\" type=\"text/css\" href=\"" + JJDocOptions.GetCSS() + "\"/>");
        }
        if (JJDocGlobals.input_file != null)
        {
            Println("<TITLE>BNF for " + JJDocGlobals.input_file + "</TITLE>");
        }
        else
        {
            Println("<TITLE>A BNF grammar by JJDoc</TITLE>");
        }
        Println("</HEAD>");
        Println("<BODY>");
        Println("<H1 ALIGN=CENTER>BNF for " + JJDocGlobals.input_file + "</H1>");
    }

    public override void DocumentEnd()
    {
        Println("</BODY>");
        Println("</HTML>");
        ostr.Close();
    }

    /**
     * Prints out comments, used for tokens and non-terminals.
     * {@inheritDoc}
     * @see org.javacc.jjdoc.TextGenerator#specialTokens(java.lang.String)
     */
    public override void SpecialTokens(string s)
    {
        Println(" <!-- Special token -->");
        Println(" <TR>");
        Println("  <TD>");
        Println("<PRE>");
        Print(s);
        Println("</PRE>");
        Println("  </TD>");
        Println(" </TR>");
    }

    public override void HandleTokenProduction(TokenProduction tp)
    {
        Println(" <!-- Token -->");
        Println(" <TR>");
        Println("  <TD>");
        Println("   <PRE>");
        string _text = JJDoc.getStandardTokenProductionText(tp);
        Text(_text);
        Println("   </PRE>");
        Println("  </TD>");
        Println(" </TR>");
    }


    public override void NonterminalsStart()
    {
        Println("<H2 ALIGN=CENTER>NON-TERMINALS</H2>");
        if (JJDocOptions.GetOneTable())
        {
            Println("<TABLE>");
        }
    }
    public override void NonterminalsEnd()
    {
        if (JJDocOptions.GetOneTable())
        {
            Println("</TABLE>");
        }
    }

    public override void TokensStart()
    {
        Println("<H2 ALIGN=CENTER>TOKENS</H2>");
        Println("<TABLE>");
    }
    public override void TokensEnd()
    {
        Println("</TABLE>");
    }

    public override void Javacode(JavaCodeProduction jp)
    {
        ProductionStart(jp);
        Println("<I>java code</I></TD></TR>");
        ProductionEnd(jp);
    }

    public override void Cppcode(CppCodeProduction cp)
    {
        ProductionStart(cp);
        Println("<I>cpp code</I></TD></TR>");
        ProductionEnd(cp);
    }

    public override void ProductionStart(NormalProduction np)
    {
        if (!JJDocOptions.GetOneTable())
        {
            Println("");
            Println("<TABLE ALIGN=CENTER>");
            Println("<CAPTION><STRONG>" + np.getLhs() + "</STRONG></CAPTION>");
        }
        Println("<TR>");
        Println("<TD ALIGN=RIGHT VALIGN=BASELINE><A NAME=\"" + GetId(np.getLhs()) + "\">" + np.getLhs() + "</A></TD>");
        Println("<TD ALIGN=CENTER VALIGN=BASELINE>::=</TD>");
        Print("<TD ALIGN=LEFT VALIGN=BASELINE>");
    }
    public override void ProductionEnd(NormalProduction np)
    {
        if (!JJDocOptions.GetOneTable())
        {
            Println("</TABLE>");
            Println("<HR>");
        }
    }

    public override void ExpansionStart(Expansion e, bool first)
    {
        if (!first)
        {
            Println("<TR>");
            Println("<TD ALIGN=RIGHT VALIGN=BASELINE></TD>");
            Println("<TD ALIGN=CENTER VALIGN=BASELINE>|</TD>");
            Print("<TD ALIGN=LEFT VALIGN=BASELINE>");
        }
    }
    public override void ExpansionEnd(Expansion e, bool first)
    {
        Println("</TD>");
        Println("</TR>");
    }

    public override void NonTerminalStart(NonTerminal nt)
    {
        Print("<A HREF=\"#" + GetId(nt.getName()) + "\">");
    }
    public override void NonTerminalEnd(NonTerminal nt)
    {
        Print("</A>");
    }

    public override void ReStart(RegularExpression r)
    {
    }
    public override void ReEnd(RegularExpression r)
    {
    }
}
