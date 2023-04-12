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
using System.Text;

namespace org.javacc.jjdoc;



/**
 * Output BNF in HTML 3.2 format.
 */
public class XTextGenerator : TextGenerator
{
    private Dictionary<string, string> id_map = new();
    private int id = 1;

    static readonly string sep = Environment.NewLine; //'\n'

    public XTextGenerator() : base()
    {
    }
    public override void handleTokenProduction(TokenProduction tp)
    {

        var sb = new StringBuilder();

        foreach(var res in tp.respecs)
        {
            var regularExpressionText = JJDoc.emitRE(res.rexp);
            sb.Append(regularExpressionText);

            if (res.nsTok != null)
            {
                sb.Append(" : " + res.nsTok.image);
            }

            sb.Append('\n');
            //          if (it2.hasNext()) {
            //              sb.Append("| ");
            //          }
        }

        //text(sb.ToString());
    }

    protected string get_id(string nt)
    {
        if (id_map.TryGetValue(nt,out var i))
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
        //    string ss = "";
        //    for (int i = 0; i < s.Length; ++i) {
        //      if (s.charAt(i) == '<') {
        //  ss += "&lt;";
        //      } else if (s.charAt(i) == '>') {
        //  ss += "&gt;";
        //      } else if (s.charAt(i) == '&') {
        //  ss += "&amp;";
        //      } else {
        //  ss += s.charAt(i);
        //      }
        //    }
        print(s);
    }

    public override void print(string s)
    {
        ostr.Write(s);
    }


    public override void documentStart()
    {
        ostr = create_output_stream();
        println("grammar " + JJDocGlobals.input_file + " with org.eclipse.xtext.common.Terminals");
        println("import \"http://www.eclipse.org/emf/2002/Ecore\" as ecore");
        println("");
        //
        //
        //    WriteLine("<HTML>");
        //    WriteLine("<HEAD>");
        //    if (!""==(JJDocOptions.getCSS())) {
        //      WriteLine("<LINK REL=\"stylesheet\" type=\"text/css\" href=\"" + JJDocOptions.getCSS() + "\"/>");
        //    }
        //    if (JJDocGlobals.input_file != null) {
        //      WriteLine("<TITLE>BNF for " + JJDocGlobals.input_file + "</TITLE>");
        //    } else {
        //      WriteLine("<TITLE>A BNF grammar by JJDoc</TITLE>");
        //    }
        //    WriteLine("</HEAD>");
        //    WriteLine("<BODY>");
        //    WriteLine("<H1 ALIGN=CENTER>BNF for " + JJDocGlobals.input_file + "</H1>");
    }

    public override void documentEnd()
    {
        //    WriteLine("</BODY>");
        //    WriteLine("</HTML>");
        ostr.Close();
    }

    /**
     * Prints out comments, used for tokens and non-terminals.
     * {@inheritDoc}
     * @see org.javacc.jjdoc.TextGenerator#specialTokens(java.lang.String)
     */
    public override void specialTokens(string s)
    {
        //    WriteLine(" <!-- Special token -->");
        //    WriteLine(" <TR>");
        //    WriteLine("  <TD>");
        //    WriteLine("<PRE>");
        print(s);
        //    WriteLine("</PRE>");
        //    WriteLine("  </TD>");
        //    WriteLine(" </TR>");
    }


    public override void nonterminalsStart()
    {
        //    WriteLine("<H2 ALIGN=CENTER>NON-TERMINALS</H2>");
        //    if (JJDocOptions.getOneTable()) {
        //      WriteLine("<TABLE>");
        //    }
    }
    public override void nonterminalsEnd()
    {
        //    if (JJDocOptions.getOneTable()) {
        //      WriteLine("</TABLE>");
        //    }
    }

    public override void tokensStart()
    {
        //    WriteLine("<H2 ALIGN=CENTER>TOKENS</H2>");
        //    WriteLine("<TABLE>");
    }
    public override void tokensEnd()
    {
        //    WriteLine("</TABLE>");
    }

    public override void javacode(JavaCodeProduction jp)
    {
        //    productionStart(jp);
        //    WriteLine("<I>java code</I></TD></TR>");
        //    productionEnd(jp);
    }

    public override void cppcode(CppCodeProduction cp)
    {
        //    productionStart(cp);
        //    WriteLine("<I>c++ code</I></TD></TR>");
        //    productionEnd(cp);
    }

    public override void productionStart(NormalProduction np)
    {
        //    if (!JJDocOptions.getOneTable()) {
        //      WriteLine("");
        //      WriteLine("<TABLE ALIGN=CENTER>");
        //      WriteLine("<CAPTION><STRONG>" + np.getLhs() + "</STRONG></CAPTION>");
        //    }
        //    WriteLine("<TR>");
        //    WriteLine("<TD ALIGN=RIGHT VALIGN=BASELINE><A NAME=\"" + get_id(np.getLhs()) + "\">" + np.getLhs() + "</A></TD>");
        //    WriteLine("<TD ALIGN=CENTER VALIGN=BASELINE>::=</TD>");
        //    print("<TD ALIGN=LEFT VALIGN=BASELINE>");
    }
    public override void productionEnd(NormalProduction np)
    {
        //    if (!JJDocOptions.getOneTable()) {
        //      WriteLine("</TABLE>");
        //      WriteLine("<HR>");
        //    }
    }

    public override void expansionStart(Expansion e, bool first)
    {
        //
        //
        //
        //    if (!first) {
        //      WriteLine("<TR>");
        //      WriteLine("<TD ALIGN=RIGHT VALIGN=BASELINE></TD>");
        //      WriteLine("<TD ALIGN=CENTER VALIGN=BASELINE>|</TD>");
        //      print("<TD ALIGN=LEFT VALIGN=BASELINE>");
        //    }
    }
    public override void expansionEnd(Expansion e, bool first)
    {
        println(";");
    }

    public override void nonTerminalStart(NonTerminal nt)
    {
        print("terminal ");
    }
    public override void nonTerminalEnd(NonTerminal nt)
    {
        print(";");
    }

    public override void reStart(RegularExpression r)
    {
    }
    public override void reEnd(RegularExpression r)
    {
    }
}
