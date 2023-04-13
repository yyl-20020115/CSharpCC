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
using CSharpCC.Utils;
using Action = CSharpCC.Parser.Action;
using Token = CSharpCC.Parser.Token;

namespace CSharpCC.CCDoc;


/**
 * The main entry point for JJDoc.
 */
public class CCDoc : CCDocGlobals
{

    public static void Start()
    {
        Generator = GetGenerator();
        Generator.DocumentStart();
        EmitTokenProductions(Generator, rexprlist);
        EmitNormalProductions(Generator, bnfproductions);
        Generator.DocumentEnd();
    }
    private static Token GetPrecedingSpecialToken(Token tok)
    {
        Token t = tok;
        while (t.specialToken != null)
        {
            t = t.specialToken;
        }
        return (t != tok) ? t : null;
    }
    private static void EmitTopLevelSpecialTokens(Token tok, Generator gen)
    {
        if (tok == null)
        {
            // Strange ...
            return;
        }
        tok = GetPrecedingSpecialToken(tok);
        string s = "";
        if (tok != null)
        {
            cline = tok.beginLine;
            ccol = tok.beginColumn;
            while (tok != null)
            {
                s += PrintTokenOnly(tok);
                tok = tok.next;
            }
        }
        if (s != (""))
            gen.SpecialTokens(s);
    }

    /*
    private static bool toplevelExpansion(Expansion exp) {
      return exp.parent != null
        && ( (exp.parent is NormalProduction)
           ||
           (exp.parent is TokenProduction)
           );
    }
    */

    private static void EmitTokenProductions(Generator gen, List<TokenProduction> prods)
    {
        gen.TokensStart();
        // FIXME there are many empty productions here
        foreach (TokenProduction tp in prods)
        {
            EmitTopLevelSpecialTokens(tp.firstToken, gen);

            gen.HandleTokenProduction(tp);

            //      if (!token==("")) {
            //        gen.tokenStart(tp);
            //        String token = getStandardTokenProductionText(tp);
            //          gen.text(token);
            //        gen.tokenEnd(tp);
            //      }
        }
        gen.TokensEnd();
    }
    public static string GetStandardTokenProductionText(TokenProduction tp)
    {
        string token = "";
        if (tp.isExplicit)
        {
            if (tp.lexStates == null)
            {
                token += "<*> ";
            }
            else
            {
                token += "<";
                for (int i = 0; i < tp.lexStates.Length; ++i)
                {
                    token += tp.lexStates[i];
                    if (i < tp.lexStates.Length - 1)
                    {
                        token += ",";
                    }
                }
                token += "> ";
            }
            token += TokenProduction.kindImage[tp.kind];
            if (tp.ignoreCase)
            {
                token += " [IGNORE_CASE]";
            }
            token += " : {\n";
            var first = true;
            foreach (RegExprSpec res in tp.respecs)
            {
                if (!first)
                {
                    token += "| ";
                }
                first = false;
                token += EmitRE(res.rexp);

                if (res.nsTok != null)
                {
                    token += " : " + res.nsTok.image;
                }

                token += "\n";
            }
            token += "}\n\n";
        }
        return token;
    }

    private static void EmitNormalProductions(Generator gen, List<NormalProduction> prods)
    {
        gen.NonterminalsStart();
        foreach (NormalProduction np in prods)
        {
            EmitTopLevelSpecialTokens(np.GetFirstToken(), gen);
            if (np is BNFProduction)
            {
                gen.ProductionStart(np);
                if (np.GetExpansion() is Choice c)
                {
                    bool first = true;
                    foreach (Expansion e in c.GetChoices())
                    {
                        gen.ExpansionStart(e, first);
                        EmitExpansionTree(e, gen);
                        gen.ExpansionEnd(e, first);
                        first = false;
                    }
                }
                else
                {
                    gen.ExpansionStart(np.GetExpansion(), true);
                    EmitExpansionTree(np.GetExpansion(), gen);
                    gen.ExpansionEnd(np.GetExpansion(), true);
                }
                gen.ProductionEnd(np);
            }
            else if (np is CppCodeProduction production)
            {
                gen.Cppcode(production);
            }
            else if (np is CSharpCodeProduction production1)
            {
                gen.Javacode(production1);
            }
        }
        gen.NonterminalsEnd();
    }
    private static void EmitExpansionTree(Expansion exp, Generator gen)
    {
        //     gen.text("[->" + exp.getClass().getName() + "]");
        if (exp is global::CSharpCC.Parser.Action pa)
        {
            EmitExpansionAction(pa, gen);
        }
        else if (exp is Choice choice)
        {
            EmitExpansionChoice(choice, gen);
        }
        else if (exp is Lookahead lookahead)
        {
            EmitExpansionLookahead(lookahead, gen);
        }
        else if (exp is NonTerminal terminal)
        {
            EmitExpansionNonTerminal(terminal, gen);
        }
        else if (exp is OneOrMore more)
        {
            EmitExpansionOneOrMore(more, gen);
        }
        else if (exp is RegularExpression expression)
        {
            EmitExpansionRegularExpression(expression, gen);
        }
        else if (exp is Sequence sequence)
        {
            EmitExpansionSequence(sequence, gen);
        }
        else if (exp is TryBlock block)
        {
            EmitExpansionTryBlock(block, gen);
        }
        else if (exp is ZeroOrMore more1)
        {
            EmitExpansionZeroOrMore(more1, gen);
        }
        else if (exp is ZeroOrOne one)
        {
            EmitExpansionZeroOrOne(one, gen);
        }
        else
        {
            Error("Oops: Unknown expansion type.");
        }
        //     gen.text("[<-" + exp.getClass().getName() + "]");
    }
    private static void EmitExpansionAction(Action a, Generator gen)
    {
    }
    private static void EmitExpansionChoice(Choice c, Generator gen)
    {
        var first = true;
        foreach (var e in c.GetChoices())
        {
            if (!first)
            {
                gen.Text(" | ");
            }
            first = false;
            EmitExpansionTree(e, gen);
        }
    }
    private static void EmitExpansionLookahead(Lookahead l, Generator gen)
    {
    }
    private static void EmitExpansionNonTerminal(NonTerminal nt, Generator gen)
    {
        gen.NonTerminalStart(nt);
        gen.Text(nt.GetName());
        gen.NonTerminalEnd(nt);
    }
    private static void EmitExpansionOneOrMore(OneOrMore o, Generator gen)
    {
        gen.Text("( ");
        EmitExpansionTree(o.expansion, gen);
        gen.Text(" )+");
    }
    private static void EmitExpansionRegularExpression(RegularExpression r,
        Generator gen)
    {
        var reRendered = EmitRE(r);
        if (reRendered != (""))
        {
            gen.ReStart(r);
            gen.Text(reRendered);
            gen.ReEnd(r);
        }
    }
    private static void EmitExpansionSequence(Sequence s, Generator gen)
    {
        bool firstUnit = true;
        foreach (var e in s.units)
        {
            if (e is Lookahead || e is Action)
            {
                continue;
            }
            if (!firstUnit)
            {
                gen.Text(" ");
            }
            bool needParens = (e is Choice) || (e is Sequence);
            if (needParens)
            {
                gen.Text("( ");
            }
            EmitExpansionTree(e, gen);
            if (needParens)
            {
                gen.Text(" )");
            }
            firstUnit = false;
        }
    }
    private static void EmitExpansionTryBlock(TryBlock t, Generator gen)
    {
        var needParens = t.exp is Choice;
        if (needParens)
        {
            gen.Text("( ");
        }
        EmitExpansionTree(t.exp, gen);
        if (needParens)
        {
            gen.Text(" )");
        }
    }
    private static void EmitExpansionZeroOrMore(ZeroOrMore z, Generator gen)
    {
        gen.Text("( ");
        EmitExpansionTree(z.expansion, gen);
        gen.Text(" )*");
    }
    private static void EmitExpansionZeroOrOne(ZeroOrOne z, Generator gen)
    {
        gen.Text("( ");
        EmitExpansionTree(z.expansion, gen);
        gen.Text(" )?");
    }
    public static string EmitRE(RegularExpression re)
    {
        string returnString = "";
        bool hasLabel = re.label != ("");
        bool justName = re is RJustName;
        bool eof = re is REndOfFile;
        bool isString = re is RStringLiteral;
        bool toplevelRE = (re.tpContext != null);
        bool needBrackets
          = justName || eof || hasLabel || (!isString && toplevelRE);
        if (needBrackets)
        {
            returnString += "<";
            if (!justName)
            {
                if (re.private_rexp)
                {
                    returnString += "#";
                }
                if (hasLabel)
                {
                    returnString += re.label;
                    returnString += ": ";
                }
            }
        }
        if (re is RCharacterList cl)
        {
            if (cl.negated_list)
            {
                returnString += "~";
            }
            returnString += "[";
            var first = true;
            foreach (var o in cl.descriptors)
            {
                if (!first)
                {
                    returnString += ",";
                }
                first = false;
                if (o is SingleCharacter character)
                {
                    returnString += "\"";
                    char[] s = { character.ch };
                    returnString += StringEscapeHelpers.AddEscapes(new string(s));
                    returnString += "\"";
                }
                else if (o is CharacterRange range)
                {
                    returnString += "\"";
                    char[] s = { range.GetLeft() };
                    returnString += StringEscapeHelpers.AddEscapes(new string(s));
                    returnString += "\"-\"";
                    s[0] = range.GetRight();
                    returnString += StringEscapeHelpers.AddEscapes(new string(s));
                    returnString += "\"";
                }
                else
                {
                    Error("Oops: unknown character list element type.");
                }
            }
            returnString += "]";
        }
        else if (re is RChoice c)
        {
            var first = true;
            foreach (var sub in c.GetChoices())
            {
                if (!first)
                {
                    returnString += " | ";
                }
                first = false;
                returnString += EmitRE(sub as RegularExpression);
            }
        }
        else if (re is REndOfFile)
        {
            returnString += "EOF";
        }
        else if (re is RJustName jn)
        {
            returnString += jn.label;
        }
        else if (re is ROneOrMore om)
        {
            returnString += "(";
            returnString += EmitRE(om.regexpr);
            returnString += ")+";
        }
        else if (re is RSequence s)
        {
            var first = true;
            foreach (var sub in s.units)
            {
                if (!first)
                {
                    returnString += " ";
                }
                first = false;
                bool needParens = false;
                if (sub is RChoice)
                {
                    needParens = true;
                }
                if (needParens)
                {
                    returnString += "(";
                }
                returnString += EmitRE(sub);
                if (needParens)
                {
                    returnString += ")";
                }
            }
        }
        else if (re is RStringLiteral sl)
        {
            returnString += ("\"" + StringEscapeHelpers.AddEscapes(sl.image) + "\"");
        }
        else if (re is RZeroOrMore zm)
        {
            returnString += "(";
            returnString += EmitRE(zm.regexpr);
            returnString += ")*";
        }
        else if (re is RZeroOrOne)
        {
            RZeroOrOne zo = (RZeroOrOne)re;
            returnString += "(";
            returnString += EmitRE(zo.regexpr);
            returnString += ")?";
        }
        else if (re is RRepetitionRange zo)
        {
            returnString += "(";
            returnString += EmitRE(zo.regexpr);
            returnString += ")";
            returnString += "{";
            if (zo.hasMax)
            {
                returnString += zo.min;
                returnString += ",";
                returnString += zo.max;
            }
            else
            {
                returnString += zo.min;
            }
            returnString += "}";
        }
        else
        {
            Error("Oops: Unknown regular expression type.");
        }
        if (needBrackets)
        {
            returnString += ">";
        }
        return returnString;
    }

    /*
    private static string v2s(Vector v, bool newLine) {
      string s = "";
      bool firstToken = true;
      for (Enumeration enumeration = v.elements(); enumeration.hasMoreElements();) {
        Token tok = (Token)enumeration.nextElement();
        Token stok = getPrecedingSpecialToken(tok);
        if (firstToken) {
          if (stok != null) {
            cline = stok.beginLine;
            ccol = stok.beginColumn;
          } else {
            cline = tok.beginLine;
            ccol = tok.beginColumn;
          }
          s = ws(ccol - 1);
          firstToken = false;
        }
        while (stok != null) {
          s += printToken(stok);
          stok = stok.next;
        }
        s += printToken(tok);
      }
      return s;
    }
    */
    /**
     * A utility to produce a string of blanks.
     */

    /*
    private static string ws(int len) {
      string s = "";
      for (int i = 0; i < len; ++i) {
        s += " ";
      }
      return s;
    }
    */

}
