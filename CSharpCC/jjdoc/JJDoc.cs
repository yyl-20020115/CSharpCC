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

using org.javacc.jjtree;
using org.javacc.parser;

namespace org.javacc.jjdoc;


/**
 * The main entry point for JJDoc.
 */
public class JJDoc : JJDocGlobals
{

    public static void start()
    {
        generator = getGenerator();
        generator.documentStart();
        emitTokenProductions(generator, rexprlist);
        emitNormalProductions(generator, bnfproductions);
        generator.documentEnd();
    }
    private static Token getPrecedingSpecialToken(Token tok)
    {
        Token t = tok;
        while (t.specialToken != null)
        {
            t = t.specialToken;
        }
        return (t != tok) ? t : null;
    }
    private static void emitTopLevelSpecialTokens(Token tok, Generator gen)
    {
        if (tok == null)
        {
            // Strange ...
            return;
        }
        tok = getPrecedingSpecialToken(tok);
        string s = "";
        if (tok != null)
        {
            cline = tok.beginLine;
            ccol = tok.beginColumn;
            while (tok != null)
            {
                s += printTokenOnly(tok);
                tok = tok.next;
            }
        }
        if (s != (""))
            gen.specialTokens(s);
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

    private static void emitTokenProductions(Generator gen, List prods)
    {
        gen.tokensStart();
        // FIXME there are many empty productions here
        for (Iterator it = prods.iterator(); it.hasNext();)
        {
            TokenProduction tp = (TokenProduction)it.next();
            emitTopLevelSpecialTokens(tp.firstToken, gen);



            gen.handleTokenProduction(tp);

            //      if (!token==("")) {
            //        gen.tokenStart(tp);
            //        String token = getStandardTokenProductionText(tp);
            //          gen.text(token);
            //        gen.tokenEnd(tp);
            //      }
        }
        gen.tokensEnd();
    }
    public static string getStandardTokenProductionText(TokenProduction tp)
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
            for (Iterator it2 = tp.respecs.iterator(); it2.hasNext();)
            {
                RegExprSpec res = (RegExprSpec)it2.next();

                token += emitRE(res.rexp);

                if (res.nsTok != null)
                {
                    token += " : " + res.nsTok.image;
                }

                token += "\n";
                if (it2.hasNext())
                {
                    token += "| ";
                }
            }
            token += "}\n\n";
        }
        return token;
    }

    private static void emitNormalProductions(Generator gen, List prods)
    {
        gen.nonterminalsStart();
        for (Iterator it = prods.iterator(); it.hasNext();)
        {
            NormalProduction np = (NormalProduction)it.next();
            emitTopLevelSpecialTokens(np.getFirstToken(), gen);
            if (np is BNFProduction)
            {
                gen.productionStart(np);
                if (np.getExpansion() is Choice)
                {
                    bool first = true;
                    Choice c = (Choice)np.getExpansion();
                    for (Iterator expansionsIterator = c.getChoices().iterator(); expansionsIterator.hasNext();)
                    {
                        Expansion e = (Expansion)(expansionsIterator.next());
                        gen.expansionStart(e, first);
                        emitExpansionTree(e, gen);
                        gen.expansionEnd(e, first);
                        first = false;
                    }
                }
                else
                {
                    gen.expansionStart(np.getExpansion(), true);
                    emitExpansionTree(np.getExpansion(), gen);
                    gen.expansionEnd(np.getExpansion(), true);
                }
                gen.productionEnd(np);
            }
            else if (np is CppCodeProduction)
            {
                gen.cppcode((CppCodeProduction)np);
            }
            else if (np is JavaCodeProduction)
            {
                gen.javacode((JavaCodeProduction)np);
            }
        }
        gen.nonterminalsEnd();
    }
    private static void emitExpansionTree(Expansion exp, Generator gen)
    {
        //     gen.text("[->" + exp.getClass().getName() + "]");
        if (exp is parser.Action)
        {
            emitExpansionAction((parser.Action)exp, gen);
        }
        else if (exp is Choice)
        {
            emitExpansionChoice((Choice)exp, gen);
        }
        else if (exp is Lookahead)
        {
            emitExpansionLookahead((Lookahead)exp, gen);
        }
        else if (exp is NonTerminal)
        {
            emitExpansionNonTerminal((NonTerminal)exp, gen);
        }
        else if (exp is OneOrMore)
        {
            emitExpansionOneOrMore((OneOrMore)exp, gen);
        }
        else if (exp is RegularExpression)
        {
            emitExpansionRegularExpression((RegularExpression)exp, gen);
        }
        else if (exp is Sequence)
        {
            emitExpansionSequence((Sequence)exp, gen);
        }
        else if (exp is TryBlock)
        {
            emitExpansionTryBlock((TryBlock)exp, gen);
        }
        else if (exp is ZeroOrMore)
        {
            emitExpansionZeroOrMore((ZeroOrMore)exp, gen);
        }
        else if (exp is ZeroOrOne)
        {
            emitExpansionZeroOrOne((ZeroOrOne)exp, gen);
        }
        else
        {
            error("Oops: Unknown expansion type.");
        }
        //     gen.text("[<-" + exp.getClass().getName() + "]");
    }
    private static void emitExpansionAction(Action a, Generator gen)
    {
    }
    private static void emitExpansionChoice(Choice c, Generator gen)
    {
        for (Iterator it = c.getChoices().iterator(); it.hasNext();)
        {
            Expansion e = (Expansion)(it.next());
            emitExpansionTree(e, gen);
            if (it.hasNext())
            {
                gen.text(" | ");
            }
        }
    }
    private static void emitExpansionLookahead(Lookahead l, Generator gen)
    {
    }
    private static void emitExpansionNonTerminal(NonTerminal nt, Generator gen)
    {
        gen.nonTerminalStart(nt);
        gen.text(nt.getName());
        gen.nonTerminalEnd(nt);
    }
    private static void emitExpansionOneOrMore(OneOrMore o, Generator gen)
    {
        gen.text("( ");
        emitExpansionTree(o.expansion, gen);
        gen.text(" )+");
    }
    private static void emitExpansionRegularExpression(RegularExpression r,
        Generator gen)
    {
        string reRendered = emitRE(r);
        if (reRendered != (""))
        {
            gen.reStart(r);
            gen.text(reRendered);
            gen.reEnd(r);
        }
    }
    private static void emitExpansionSequence(Sequence s, Generator gen)
    {
        bool firstUnit = true;
        for (Iterator it = s.units.iterator(); it.hasNext();)
        {
            Expansion e = (Expansion)it.next();
            if (e is Lookahead || e is Action)
            {
                continue;
            }
            if (!firstUnit)
            {
                gen.text(" ");
            }
            bool needParens = (e is Choice) || (e is Sequence);
            if (needParens)
            {
                gen.text("( ");
            }
            emitExpansionTree(e, gen);
            if (needParens)
            {
                gen.text(" )");
            }
            firstUnit = false;
        }
    }
    private static void emitExpansionTryBlock(TryBlock t, Generator gen)
    {
        bool needParens = t.exp is Choice;
        if (needParens)
        {
            gen.text("( ");
        }
        emitExpansionTree(t.exp, gen);
        if (needParens)
        {
            gen.text(" )");
        }
    }
    private static void emitExpansionZeroOrMore(ZeroOrMore z, Generator gen)
    {
        gen.text("( ");
        emitExpansionTree(z.expansion, gen);
        gen.text(" )*");
    }
    private static void emitExpansionZeroOrOne(ZeroOrOne z, Generator gen)
    {
        gen.text("( ");
        emitExpansionTree(z.expansion, gen);
        gen.text(" )?");
    }
    public static string emitRE(RegularExpression re)
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
        if (re is RCharacterList)
        {
            RCharacterList cl = (RCharacterList)re;
            if (cl.negated_list)
            {
                returnString += "~";
            }
            returnString += "[";
            for (Iterator it = cl.descriptors.iterator(); it.hasNext();)
            {
                Object o = it.next();
                if (o is SingleCharacter)
                {
                    returnString += "\"";
                    char s[] = { ((SingleCharacter)o).ch };
                    returnString += add_escapes(new String(s));
                    returnString += "\"";
                }
                else if (o is CharacterRange)
                {
                    returnString += "\"";
                    char s[] = { ((CharacterRange)o).getLeft() };
                    returnString += add_escapes(new String(s));
                    returnString += "\"-\"";
                    s[0] = ((CharacterRange)o).getRight();
                    returnString += add_escapes(new String(s));
                    returnString += "\"";
                }
                else
                {
                    error("Oops: unknown character list element type.");
                }
                if (it.hasNext())
                {
                    returnString += ",";
                }
            }
            returnString += "]";
        }
        else if (re is RChoice)
        {
            RChoice c = (RChoice)re;
            var first = true;
            foreach(RegularExpression sub in c.getChoices() )
            {
                if (!first)
                {
                    returnString += " | ";
                }
                first = false;
                returnString += emitRE(sub);
            }
        }
        else if (re is REndOfFile)
        {
            returnString += "EOF";
        }
        else if (re is RJustName)
        {
            RJustName jn = (RJustName)re;
            returnString += jn.label;
        }
        else if (re is ROneOrMore)
        {
            ROneOrMore om = (ROneOrMore)re;
            returnString += "(";
            returnString += emitRE(om.regexpr);
            returnString += ")+";
        }
        else if (re is RSequence)
        {
            RSequence s = (RSequence)re;
            var first = true;
            foreach(RegularExpression sub in s.units)
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
                returnString += emitRE(sub);
                if (needParens)
                {
                    returnString += ")";
                }
            }
        }
        else if (re is RStringLiteral)
        {
            RStringLiteral sl = (RStringLiteral)re;
            returnString += ("\"" + JavaCCParserInternals.add_escapes(sl.image) + "\"");
        }
        else if (re is RZeroOrMore)
        {
            RZeroOrMore zm = (RZeroOrMore)re;
            returnString += "(";
            returnString += emitRE(zm.regexpr);
            returnString += ")*";
        }
        else if (re is RZeroOrOne)
        {
            RZeroOrOne zo = (RZeroOrOne)re;
            returnString += "(";
            returnString += emitRE(zo.regexpr);
            returnString += ")?";
        }
        else if (re is RRepetitionRange)
        {
            RRepetitionRange zo = (RRepetitionRange)re;
            returnString += "(";
            returnString += emitRE(zo.regexpr);
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
            error("Oops: Unknown regular expression type.");
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
