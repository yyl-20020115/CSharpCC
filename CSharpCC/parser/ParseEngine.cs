// Copyright 2011 Google Inc. All Rights Reserved.
// Author: sreeni@google.com (Sreeni Viswanadha)

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
using System.Text;

namespace org.javacc.parser;

public class ParseEngine
{
    private int gensymindex = 0;
    private int indentamt;
    private bool jj2LA;
    private CodeGenerator _CodeGenerator;
    private readonly bool isJavaDialect = Options.IsOutputLanguageJava();

    /**
     * These lists are used to maintain expansions for which code generation
     * in phase 2 and phase 3 is required.  Whenever a call is generated to
     * a phase 2 or phase 3 routine, a corresponding entry is added here if
     * it has not already been added.
     * The phase 3 routines have been optimized in version 0.7pre2.  Essentially
     * only those methods (and only those portions of these methods) are
     * generated that are required.  The lookahead amount is used to determine
     * this.  This change requires the use of a hash table because it is now
     * possible for the same phase 3 routine to be requested multiple times
     * with different lookaheads.  The hash table provides a easily searchable
     * capability to determine the previous requests.
     * The phase 3 routines now are performed in a two step process - the first
     * step gathers the requests (replacing requests with lower lookaheads with
     * those requiring larger lookaheads).  The second step then generates these
     * methods.
     * This optimization and the hashtable makes it look like we do not need
     * the flag "phase3done" any more.  But this has not been removed yet.
     */
    private List<Lookahead> phase2list = new();
    private List<Phase3Data> phase3list = new();
    private Dictionary<Expansion, Phase3Data> phase3table = new();

    /**
     * The phase 1 routines generates their output into String's and dumps
     * these String's once for each method.  These String's contain the
     * special characters '\u0001' to indicate a positive indent, and '\u0002'
     * to indicate a negative indent.  '\n' is used to indicate a line terminator.
     * The characters '\u0003' and '\u0004' are used to delineate portions of
     * text where '\n's should not be followed by an indentation.
     */

    /**
     * Returns true if there is a JAVACODE production that the argument expansion
     * may directly expand to (without consuming tokens or encountering lookahead).
     */
    private bool JavaCodeCheck(Expansion exp)
    {
        if (exp is RegularExpression)
        {
            return false;
        }
        else if (exp is NonTerminal terminal)
        {
            var prod = terminal.GetProd();
            if (prod is CodeProduction)
            {
                return true;
            }
            else
            {
                return JavaCodeCheck(prod.GetExpansion());
            }
        }
        else if (exp is Choice ch)
        {
            for (int i = 0; i < ch.GetChoices().Count; i++)
            {
                if (JavaCodeCheck((Expansion)(ch.GetChoices()[i])))
                {
                    return true;
                }
            }
            return false;
        }
        else if (exp is Sequence seq)
        {
            for (int i = 0; i < seq.units.Count; i++)
            {
                var units = seq.units;
                if (units[i] is Lookahead lookahead && lookahead.IsExplicit())
                {
                    // An explicit lookahead (rather than one generated implicitly). Assume
                    // the user knows what he / she is doing, e.g.
                    //    "A" ( "B" | LOOKAHEAD("X") jcode() | "C" )* "D"
                    return false;
                }
                else if (JavaCodeCheck((units[i])))
                {
                    return true;
                }
                else if (!Semanticize.EmptyExpansionExists(units[i]))
                {
                    return false;
                }
            }
            return false;
        }
        else if (exp is OneOrMore om)
        {
            return JavaCodeCheck(om.expansion);
        }
        else if (exp is ZeroOrMore zm)
        {
            return JavaCodeCheck(zm.expansion);
        }
        else if (exp is ZeroOrOne zo)
        {
            return JavaCodeCheck(zo.expansion);
        }
        else if (exp is TryBlock tb)
        {
            return JavaCodeCheck(tb.exp);
        }
        else
        {
            return false;
        }
    }

    /**
     * An array used to store the first sets generated by the following method.
     * A true entry means that the corresponding token is in the first set.
     */
    private bool[] firstSet;

    /**
     * Sets up the array "firstSet" above based on the Expansion argument
     * passed to it.  Since this is a recursive function, it assumes that
     * "firstSet" has been reset before the first call.
     */
    private void GenFirstSet(Expansion exp)
    {
        if (exp is RegularExpression expression)
        {
            firstSet[expression.ordinal] = true;
        }
        else if (exp is NonTerminal terminal)
        {
            if (terminal.GetProd() is not CodeProduction)
            {
                GenFirstSet(((BNFProduction)(terminal.GetProd())).GetExpansion());
            }
        }
        else if (exp is Choice ch)
        {
            for (int i = 0; i < ch.GetChoices().Count; i++)
            {
                GenFirstSet(ch.GetChoices()[i]);
            }
        }
        else if (exp is Sequence seq)
        {
            object obj = seq.units[0];
            if ((obj is Lookahead lookahead) && (lookahead.GetActionTokens().Count != 0))
            {
                jj2LA = true;
            }
            for (int i = 0; i < seq.units.Count; i++)
            {
                Expansion unit = (Expansion)seq.units[i];
                // Javacode productions can not have FIRST sets. Instead we generate the FIRST set
                // for the preceding LOOKAHEAD (the semantic checks should have made sure that
                // the LOOKAHEAD is suitable).
                if (unit is NonTerminal terminal1 && terminal1.GetProd() is CodeProduction)
                {
                    if (i > 0 && seq.units[i - 1] is Lookahead)
                    {
                        Lookahead la = (Lookahead)seq.units[i - 1];
                        GenFirstSet(la.GetLaExpansion());
                    }
                }
                else
                {
                    GenFirstSet(seq.units[i]);
                }
                if (!Semanticize.EmptyExpansionExists(seq.units[i]))
                {
                    break;
                }
            }
        }
        else if (exp is OneOrMore om)
        {
            GenFirstSet(om.expansion);
        }
        else if (exp is ZeroOrMore zm)
        {
            GenFirstSet(zm.expansion);
        }
        else if (exp is ZeroOrOne zo)
        {
            GenFirstSet(zo.expansion);
        }
        else if (exp is TryBlock tb)
        {
            GenFirstSet(tb.exp);
        }
    }

    /**
     * Constants used in the following method "buildLookaheadChecker".
     */
    const int NOOPENSTM = 0;
    const int OPENIF = 1;
    const int OPENSWITCH = 2;

    private static void DumpLookaheads(Lookahead[] conds, String[] actions)
    {
        for (int i = 0; i < conds.Length; i++)
        {
            Console.Error.WriteLine("Lookahead: " + i);
            Console.Error.WriteLine(conds[i].Dump(0, new()));
            Console.Error.WriteLine();
        }
    }

    /**
     * This method takes two parameters - an array of Lookahead's
     * "conds", and an array of String's "actions".  "actions" contains
     * exactly one element more than "conds".  "actions" are Java source
     * code, and "conds" translate to conditions - so lets say
     * "f(conds[i])" is true if the lookahead required by "conds[i]" is
     * indeed the case.  This method returns a string corresponding to
     * the Java code for:
     *
     *   if (f(conds[0]) actions[0]
     *   else if (f(conds[1]) actions[1]
     *   . . .
     *   else actions[action.Length-1]
     *
     * A particular action entry ("actions[i]") can be null, in which
     * case, a noop is generated for that action.
     */
    string BuildLookaheadChecker(Lookahead[] conds, String[] actions)
    {

        // The state variables.
        int state = NOOPENSTM;
        int indentAmt = 0;
        bool[] casedValues = new bool[JavaCCGlobals.tokenCount];
        string retval = "";
        Lookahead la;
        Token t = null;
        int tokenMaskSize = (JavaCCGlobals.tokenCount - 1) / 32 + 1;
        int[] tokenMask = null;

        // Iterate over all the conditions.
        int index = 0;
        while (index < conds.Length)
        {

            la = conds[index];
            jj2LA = false;

            if ((la.GetAmount() == 0) ||
                Semanticize.EmptyExpansionExists(la.GetLaExpansion()) ||
                JavaCodeCheck(la.GetLaExpansion())
            )
            {

                // This handles the following cases:
                // . If syntactic lookahead is not wanted (and hence explicitly specified
                //   as 0).
                // . If it is possible for the lookahead expansion to recognize the empty
                //   string - in which case the lookahead trivially passes.
                // . If the lookahead expansion has a JAVACODE production that it directly
                //   expands to - in which case the lookahead trivially passes.
                if (la.GetActionTokens().Count == 0)
                {
                    // In addition, if there is no semantic lookahead, then the
                    // lookahead trivially succeeds.  So break the main loop and
                    // treat this case as the default last action.
                    break;
                }
                else
                {
                    // This case is when there is only semantic lookahead
                    // (without any preceding syntactic lookahead).  In this
                    // case, an "if" statement is generated.
                    switch (state)
                    {
                        case NOOPENSTM:
                            retval += "\n" + "if (";
                            indentAmt++;
                            break;
                        case OPENIF:
                            retval += "\u0002\n" + "} else if (";
                            break;
                        case OPENSWITCH:
                            retval += "\u0002\n" + "default:" + "\u0001";
                            if (Options.GetErrorReporting())
                            {
                                retval += "\njj_la1[" + JavaCCGlobals.maskindex + "] = jj_gen;";
                                JavaCCGlobals.maskindex++;
                            }
                            JavaCCGlobals.maskVals.Add(tokenMask);
                            retval += "\n" + "if (";
                            indentAmt++;
                            break;
                    }
                    JavaCCGlobals.PrintTokenSetup((la.GetActionTokens()[0]));
                    foreach (var t2 in la.GetActionTokens())
                    {
                        retval += _CodeGenerator.GetStringToPrint(t = t2);
                    }
                    retval += _CodeGenerator.GetTrailingComments(t);
                    retval += ") {\u0001" + actions[index];
                    state = OPENIF;
                }

            }
            else if (la.GetAmount() == 1 && la.GetActionTokens().Count == 0)
            {
                // Special optimal processing when the lookahead is exactly 1, and there
                // is no semantic lookahead.

                firstSet ??= new bool[JavaCCGlobals.tokenCount];
                for (int i = 0; i < JavaCCGlobals.tokenCount; i++)
                {
                    firstSet[i] = false;
                }
                // jj2LA is set to false at the beginning of the containing "if" statement.
                // It is checked immediately after the end of the same statement to determine
                // if lookaheads are to be performed using calls to the jj2 methods.
                GenFirstSet(la.GetLaExpansion());
                // genFirstSet may find that semantic attributes are appropriate for the next
                // token.  In which case, it sets jj2LA to true.
                if (!jj2LA)
                {

                    // This case is if there is no applicable semantic lookahead and the lookahead
                    // is one (excluding the earlier cases such as JAVACODE, etc.).
                    switch (state)
                    {
                        case OPENIF:
                            retval += "\u0002\n" + "} else {\u0001";
                            break;
                        //$FALL-THROUGH$ Control flows through to next case.
                        case NOOPENSTM:
                            retval += "\n" + "switch (";
                            if (Options.GetCacheTokens())
                            {
                                if (Options.IsOutputLanguageCpp())
                                {
                                    retval += "jj_nt->kind";
                                }
                                else
                                {
                                    retval += "jj_nt.kind";
                                }
                                retval += ") {\u0001";
                            }
                            else
                            {
                                retval += "(jj_ntk==-1)?jj_ntk_f():jj_ntk) {\u0001";
                            }
                            for (int i = 0; i < JavaCCGlobals.tokenCount; i++)
                            {
                                casedValues[i] = false;
                            }
                            indentAmt++;
                            tokenMask = new int[tokenMaskSize];
                            for (int i = 0; i < tokenMaskSize; i++)
                            {
                                tokenMask[i] = 0;
                            }
                            break;
                            // Don't need to do anything if state is OPENSWITCH.
                    }
                    for (int i = 0; i < JavaCCGlobals.tokenCount; i++)
                    {
                        if (firstSet[i])
                        {
                            if (!casedValues[i])
                            {
                                casedValues[i] = true;
                                retval += "\u0002\ncase ";
                                int j1 = i / 32;
                                int j2 = i % 32;
                                tokenMask[j1] |= 1 << j2;
                                //string s = (String)(names_of_tokens.get((i)));
                                if (JavaCCGlobals.names_of_tokens.TryGetValue(i, out var s))
                                {
                                    retval += i;
                                }
                                else
                                {
                                    retval += s;
                                }
                                retval += ":\u0001";
                            }
                        }
                    }
                    retval += "{";
                    retval += actions[index];
                    retval += "\nbreak;\n}";
                    state = OPENSWITCH;
                }

            }
            else
            {
                // This is the case when lookahead is determined through calls to
                // jj2 methods.  The other case is when lookahead is 1, but semantic
                // attributes need to be evaluated.  Hence this crazy control structure.

                jj2LA = true;

            }

            if (jj2LA)
            {
                // In this case lookahead is determined by the jj2 methods.

                switch (state)
                {
                    case NOOPENSTM:
                        retval += "\n" + "if (";
                        indentAmt++;
                        break;
                    case OPENIF:
                        retval += "\u0002\n" + "} else if (";
                        break;
                    case OPENSWITCH:
                        retval += "\u0002\n" + "default:" + "\u0001";
                        if (Options.GetErrorReporting())
                        {
                            retval += "\njj_la1[" + JavaCCGlobals.maskindex + "] = jj_gen;";
                            JavaCCGlobals.maskindex++;
                        }
                        JavaCCGlobals.maskVals.Add(tokenMask);
                        retval += "\n" + "if (";
                        indentAmt++;
                        break;
                }
                JavaCCGlobals.jj2index++;
                // At this point, la.la_expansion.internal_name must be "".
                la.GetLaExpansion().internal_name = "_" + JavaCCGlobals.jj2index;
                la.GetLaExpansion().internal_index = JavaCCGlobals.jj2index;
                phase2list.Add(la);
                retval += "jj_2" + la.GetLaExpansion().internal_name + "(" + la.GetAmount() + ")";
                if (la.GetActionTokens().Count != 0)
                {
                    // In addition, there is also a semantic lookahead.  So concatenate
                    // the semantic check with the syntactic one.
                    retval += " && (";
                    JavaCCGlobals.PrintTokenSetup(la.GetActionTokens()[0]);
                    foreach (var t2 in la.GetActionTokens())
                    {
                        retval += _CodeGenerator.GetStringToPrint(t = t2);
                    }
                    retval += _CodeGenerator.GetTrailingComments(t);
                    retval += ")";
                }
                retval += ") {\u0001" + actions[index];
                state = OPENIF;
            }

            index++;
        }

        // Generate code for the default case.  Note this may not
        // be the last entry of "actions" if any condition can be
        // statically determined to be always "true".

        switch (state)
        {
            case NOOPENSTM:
                retval += actions[index];
                break;
            case OPENIF:
                retval += "\u0002\n" + "} else {\u0001" + actions[index];
                break;
            case OPENSWITCH:
                retval += "\u0002\n" + "default:" + "\u0001";
                if (Options.GetErrorReporting())
                {
                    retval += "\njj_la1[" + JavaCCGlobals.maskindex + "] = jj_gen;";
                    JavaCCGlobals.maskVals.Add(tokenMask);
                    JavaCCGlobals.maskindex++;
                }
                retval += actions[index];
                break;
        }
        for (int i = 0; i < indentAmt; i++)
        {
            retval += "\u0002\n}";
        }

        return retval;

    }

    void DumpFormattedString(string str)
    {
        char ch = ' ';
        char prevChar;
        bool indentOn = true;
        for (int i = 0; i < str.Length; i++)
        {
            prevChar = ch;
            ch = str[i];
            if (ch == '\n' && prevChar == '\r')
            {
                // do nothing - we've already printed a new line for the '\r'
                // during the previous iteration.
            }
            else if (ch == '\n' || ch == '\r')
            {
                if (indentOn)
                {
                    phase1NewLine();
                }
                else
                {
                    _CodeGenerator.GenCodeLine("");
                }
            }
            else if (ch == '\u0001')
            {
                indentamt += 2;
            }
            else if (ch == '\u0002')
            {
                indentamt -= 2;
            }
            else if (ch == '\u0003')
            {
                indentOn = false;
            }
            else if (ch == '\u0004')
            {
                indentOn = true;
            }
            else
            {
                _CodeGenerator.GenCode(ch);
            }
        }
    }

    // Print CPPCODE method header.
    private string GenerateCPPMethodheader(CppCodeProduction p)
    {
        var sig = new StringBuilder();
        string ret, _params;
        Token t = null;

        string method_name = p.GetLhs();
        bool void_ret = false;
        bool ptr_ret = false;

        //    codeGenerator.printTokenSetup(t); ccol = 1;
        //    string comment1 = codeGenerator.getLeadingComments(t);
        //    cline = t.beginLine;
        //    ccol = t.beginColumn;
        //    sig.Append(t.image);
        //    if (t.kind == JavaCCParserConstants.VOID) void_ret = true;
        //    if (t.kind == JavaCCParserConstants.STAR) ptr_ret = true;

        for (int i = 0; i < p.GetReturnTypeTokens().Count; i++)
        {
            t = (Token)(p.GetReturnTypeTokens()[i]);
            string s = _CodeGenerator.GetStringToPrint(t);
            sig.Append(t.ToString());
            sig.Append(' ');
            if (t.kind == JavaCCParserConstants.VOID) void_ret = true;
            if (t.kind == JavaCCParserConstants.STAR) ptr_ret = true;
        }

        string comment2 = "";
        if (t != null)
            comment2 = _CodeGenerator.GetTrailingComments(t);
        ret = sig.ToString();

        sig.Capacity = 0;
        sig.Append('(');
        if (p.GetParameterListTokens().Count != 0)
        {
            JavaCCGlobals.PrintTokenSetup((Token)(p.GetParameterListTokens()[0]));
            foreach (var t2 in p.GetParameterListTokens())
            {
                sig.Append(_CodeGenerator.GetStringToPrint(t = t2));
            }
            sig.Append(_CodeGenerator.GetTrailingComments(t));
        }
        sig.Append(')');
        _params = sig.ToString();

        // For now, just ignore comments
        _CodeGenerator.GenerateMethodDefHeader(ret, JavaCCGlobals.CuName, p.GetLhs() + _params, sig.ToString());

        return "";
    }

    // Print method header and return the ERROR_RETURN string.
    private string generateCPPMethodheader(BNFProduction p, Token t)
    {
        StringBuilder sig = new StringBuilder();
        string ret, _params;

        string method_name = p.GetLhs();
        bool void_ret = false;
        bool ptr_ret = false;

        for (int i = 1; i < p.GetReturnTypeTokens().Count; i++)
        {
            t = (Token)(p.GetReturnTypeTokens()[i]);
            sig.Append(_CodeGenerator.GetStringToPrint(t));
            if (t.kind == JavaCCParserConstants.VOID) void_ret = true;
            if (t.kind == JavaCCParserConstants.STAR) ptr_ret = true;
        }

        string comment2 = _CodeGenerator.GetTrailingComments(t);
        ret = sig.ToString();

        sig.Capacity = 0;
        sig.Append('(');
        if (p.GetParameterListTokens().Count != 0)
        {
            CodeGenerator.PrintTokenSetup((Token)(p.GetParameterListTokens()[0]));
            foreach (var t2 in p.GetParameterListTokens())
            {
                sig.Append(_CodeGenerator.GetStringToPrint(t = t2));
            }
            sig.Append(_CodeGenerator.GetTrailingComments(t));
        }
        sig.Append(')');
        _params = sig.ToString();

        // For now, just ignore comments
        _CodeGenerator.GenerateMethodDefHeader(ret, JavaCCGlobals.CuName, p.GetLhs() + _params, sig.ToString());

        // Generate a default value for error return.
        string default_return;
        if (ptr_ret) default_return = "NULL";
        else if (void_ret) default_return = "";
        else default_return = "0";  // 0 converts to most (all?) basic types.

        StringBuilder ret_val =
            new StringBuilder("\n#if !defined ERROR_RET_" + method_name + "\n");
        ret_val.Append("#define ERROR_RET_" + method_name + " " +
                       default_return + "\n");
        ret_val.Append("#endif\n");
        ret_val.Append("#define __ERROR_RET__ ERROR_RET_" + method_name + "\n");

        return ret_val.ToString();
    }


    void genStackCheck(bool voidReturn)
    {
        if (Options.GetDepthLimit() > 0)
        {
            if (isJavaDialect)
            {
                _CodeGenerator.GenCodeLine("if(++jj_depth > " + Options.GetDepthLimit() + ") {");
                _CodeGenerator.GenCodeLine("  jj_consume_token(-1);");
                _CodeGenerator.GenCodeLine("  throw new ParseException();");
                _CodeGenerator.GenCodeLine("}");
                _CodeGenerator.GenCodeLine("try {");
            }
            else
            {
                if (!voidReturn)
                {
                    _CodeGenerator.GenCodeLine("if(jj_depth_error){ return __ERROR_RET__; }");
                }
                else
                {
                    _CodeGenerator.GenCodeLine("if(jj_depth_error){ return; }");
                }
                _CodeGenerator.GenCodeLine("__jj_depth_inc __jj_depth_counter(this);");
                _CodeGenerator.GenCodeLine("if(jj_depth > " + Options.GetDepthLimit() + ") {");
                _CodeGenerator.GenCodeLine("  jj_depth_error = true;");
                _CodeGenerator.GenCodeLine("  jj_consume_token(-1);");
                _CodeGenerator.GenCodeLine("  errorHandler->handleParseError(token, getToken(1), __FUNCTION__, this), hasError = true;");
                if (!voidReturn)
                {
                    _CodeGenerator.GenCodeLine("  return __ERROR_RET__;");  // Non-recoverable error
                }
                else
                {
                    _CodeGenerator.GenCodeLine("  return;");  // Non-recoverable error
                }
                _CodeGenerator.GenCodeLine("}");
            }
        }
    }

    private void genStackCheckEnd()
    {
        if (Options.GetDepthLimit() > 0)
        {
            if (isJavaDialect)
            {
                _CodeGenerator.GenCodeLine(" } finally {");
                _CodeGenerator.GenCodeLine("   --jj_depth;");
                _CodeGenerator.GenCodeLine(" }");
            }
        }
    }

    void buildPhase1Routine(BNFProduction p)
    {
        Token t;
        t = (Token)(p.GetReturnTypeTokens()[0]);
        bool voidReturn = false;
        if (t.kind == JavaCCParserConstants.VOID)
        {
            voidReturn = true;
        }
        string error_ret = null;
        if (isJavaDialect)
        {
            CodeGenerator.PrintTokenSetup(t);
            JavaCCGlobals.ccol = 1;
            _CodeGenerator.PrintLeadingComments(t);
            _CodeGenerator.GenCode("  " + JavaCCGlobals.StaticOpt() + "final " + (p.GetAccessMod() ?? "public") + " ");
            JavaCCGlobals.cline = t.beginLine;
            JavaCCGlobals.ccol = t.beginColumn;
            _CodeGenerator.PrintTokenOnly(t);
            for (int i = 1; i < p.GetReturnTypeTokens().Count; i++)
            {
                t = p.GetReturnTypeTokens()[i];
                _CodeGenerator.PrintToken(t);
            }
            _CodeGenerator.PrintTrailingComments(t);
            _CodeGenerator.GenCode(" " + p.GetLhs() + "(");
            if (p.GetParameterListTokens().Count != 0)
            {
                JavaCCGlobals.PrintTokenSetup((p.GetParameterListTokens()[0]));
                foreach (var t2 in p.GetParameterListTokens())
                {
                    _CodeGenerator.PrintToken(t = t2);
                }
                _CodeGenerator.PrintTrailingComments(t);
            }
            _CodeGenerator.GenCode(")");
            _CodeGenerator.GenCode(" throws ParseException");

            foreach(var name in p.GetThrowsList())
            {
                _CodeGenerator.GenCode(", ");
                foreach(var t2 in name)
                {
                    t = t2;
                    _CodeGenerator.GenCode(t.image);
                }
            }
        }
        else
        {
            error_ret = generateCPPMethodheader(p, t);
        }

        _CodeGenerator.GenCode(" {");

        if ((Options.BooleanValue(Options.USEROPTION__CPP_STOP_ON_FIRST_ERROR) && error_ret != null)
            || (Options.GetDepthLimit() > 0 && !voidReturn && !isJavaDialect))
        {
            _CodeGenerator.GenCode(error_ret);
        }
        else
        {
            error_ret = null;
        }

        genStackCheck(voidReturn);

        indentamt = 4;
        if (Options.GetDebugParser())
        {
            _CodeGenerator.GenCodeLine("");
            if (isJavaDialect)
            {
                _CodeGenerator.GenCodeLine("    trace_call(\"" + JavaCCGlobals.AddUnicodeEscapes(p.GetLhs()) + "\");");
            }
            else
            {
                _CodeGenerator.GenCodeLine("    JJEnter<std::function<void()>> jjenter([this]() {trace_call  (\"" + JavaCCGlobals.AddUnicodeEscapes(p.GetLhs()) + "\"); });");
                _CodeGenerator.GenCodeLine("    JJExit <std::function<void()>> jjexit ([this]() {trace_return(\"" + JavaCCGlobals.AddUnicodeEscapes(p.GetLhs()) + "\"); });");
            }
            _CodeGenerator.GenCodeLine("    try {");
            indentamt = 6;
        }

        if (!Options.BooleanValue(Options.USEROPTION__CPP_IGNORE_ACTIONS) &&
            p.GetDeclarationTokens().Count != 0)
        {
            JavaCCGlobals.PrintTokenSetup((p.GetDeclarationTokens()[0])); 
            JavaCCGlobals.cline--;
            foreach(var t2 in p.GetDeclarationTokens())
            {
                _CodeGenerator.PrintToken(t  = t2);
            }
            _CodeGenerator.PrintTrailingComments(t);
        }

        string code = phase1ExpansionGen(p.GetExpansion());
        DumpFormattedString(code);
        _CodeGenerator.GenCodeLine("");

        if (p.IsJumpPatched() && !voidReturn)
        {
            if (isJavaDialect)
            {
                // This line is required for Java!
                _CodeGenerator.GenCodeLine("    throw new " + (Options.IsLegacyExceptionHandling() ? "Error" : "RuntimeException") + "(\"Missing return statement in function\");");
            }
            else
            {
                _CodeGenerator.GenCodeLine("    throw \"Missing return statement in function\";");
            }
        }
        if (Options.GetDebugParser())
        {
            if (isJavaDialect)
            {
                _CodeGenerator.GenCodeLine("    } finally {");
                _CodeGenerator.GenCodeLine("      trace_return(\"" + JavaCCGlobals.AddUnicodeEscapes(p.GetLhs()) + "\");");
            }
            else
            {
                _CodeGenerator.GenCodeLine("    } catch(...) { }");
            }
            if (isJavaDialect)
            {
                _CodeGenerator.GenCodeLine("    }");
            }
        }
        if (!isJavaDialect && !voidReturn)
        {
            _CodeGenerator.GenCodeLine("assert(false);");
        }


        if (error_ret != null)
        {
            _CodeGenerator.GenCodeLine("\n#undef __ERROR_RET__\n");
        }
        genStackCheckEnd();
        _CodeGenerator.GenCodeLine("}");
        _CodeGenerator.GenCodeLine("");
    }

    void phase1NewLine()
    {
        _CodeGenerator.GenCodeLine("");
        for (int i = 0; i < indentamt; i++)
        {
            _CodeGenerator.GenCode(" ");
        }
    }

    string phase1ExpansionGen(Expansion e)
    {
        string retval = "";
        Token t = null;
        Lookahead[] conds;
        String[] actions;
        if (e is RegularExpression e_nrw)
        {
            retval += "\n";
            if (e_nrw.lhsTokens.Count != 0)
            {
                JavaCCGlobals.PrintTokenSetup((Token)(e_nrw.lhsTokens[0]));
                foreach(var t2 in e_nrw.lhsTokens)
                {
                    retval += _CodeGenerator.GetStringToPrint(t=t2);
                }
                retval += _CodeGenerator.GetTrailingComments(t);
                retval += " = ";
            }
            string tail = e_nrw.rhsToken == null ? ");" :
                      (isJavaDialect ? ")." : ")->") + e_nrw.rhsToken.image + ";";
            if (e_nrw.label == (""))
            {
                if (JavaCCGlobals.names_of_tokens.TryGetValue(e_nrw.ordinal,out var label))
                {
                    retval += "jj_consume_token(" + (String)label + tail;
                }
                else
                {
                    retval += "jj_consume_token(" + e_nrw.ordinal + tail;
                }
            }
            else
            {
                retval += "jj_consume_token(" + e_nrw.label + tail;
            }

            if (!isJavaDialect && Options.BooleanValue(Options.USEROPTION__CPP_STOP_ON_FIRST_ERROR))
            {
                retval += "\n    { if (hasError) { return __ERROR_RET__; } }\n";
            }

        }
        else if (e is NonTerminal e_nrw6)
        {
            retval += "\n";
            if (e_nrw6.GetLhsTokens().Count != 0)
            {
                JavaCCGlobals.PrintTokenSetup(e_nrw6.GetLhsTokens()[0]);
                foreach(var t2 in e_nrw6.GetLhsTokens())
                {
                    retval += _CodeGenerator.GetStringToPrint(t = t2);
                }
                retval += _CodeGenerator.GetTrailingComments(t);
                retval += " = ";
            }
            retval += e_nrw6.GetName() + "(";
            if (e_nrw6.GetArgumentTokens().Count != 0)
            {
                JavaCCGlobals.PrintTokenSetup(e_nrw6.GetArgumentTokens()[0]);
                foreach(var t2 in e_nrw6.GetArgumentTokens())
                {
                    retval += _CodeGenerator.GetStringToPrint(t = t2);
                }
                retval += _CodeGenerator.GetTrailingComments(t);
            }
            retval += ");";
            if (!isJavaDialect && Options.BooleanValue(Options.USEROPTION__CPP_STOP_ON_FIRST_ERROR))
            {
                retval += "\n    { if (hasError) { return __ERROR_RET__; } }\n";
            }
        }
        else if (e is Action e_nrw7)
        {
            retval += "\u0003\n";
            if (!Options.BooleanValue(Options.USEROPTION__CPP_IGNORE_ACTIONS) &&
                e_nrw7.GetActionTokens().Count != 0)
            {
                JavaCCGlobals.PrintTokenSetup((e_nrw7.GetActionTokens()[0]));
                JavaCCGlobals.ccol = 1;
                foreach(var t2 in e_nrw7.GetActionTokens())
                {
                    retval += _CodeGenerator.GetStringToPrint(t = t2);
                }
                retval += _CodeGenerator.GetTrailingComments(t);
            }
            retval += "\u0004";
        }
        else if (e is Choice e_nrw8)
        {
            conds = new Lookahead[e_nrw8.GetChoices().Count];
            actions = new String[e_nrw8.GetChoices().Count + 1];
            actions[e_nrw8.GetChoices().Count] = "\n" + "jj_consume_token(-1);\n" +
                      (isJavaDialect ? "throw new ParseException();"
                                      : ("errorHandler->handleParseError(token, getToken(1), __FUNCTION__, this), hasError = true;" +
                       (Options.BooleanValue(Options.USEROPTION__CPP_STOP_ON_FIRST_ERROR) ? "return __ERROR_RET__;\n" : "")));

            // In previous line, the "throw" never throws an exception since the
            // evaluation of jj_consume_token(-1) causes ParseException to be
            // thrown first.
            Sequence nestedSeq;
            for (int i = 0; i < e_nrw8.GetChoices().Count; i++)
            {
                nestedSeq = (Sequence)(e_nrw8.GetChoices()[i]);
                actions[i] = phase1ExpansionGen(nestedSeq);
                conds[i] = (Lookahead)(nestedSeq.units[0]);
            }
            retval = BuildLookaheadChecker(conds, actions);
        }
        else if (e is Sequence e_nrw9)
        {
            // We skip the first element in the following iteration since it is the
            // Lookahead object.
            for (int i = 1; i < e_nrw9.units.Count; i++)
            {
                // For C++, since we are not using exceptions, we will protect all the
                // expansion choices with if (!error)
                bool wrap_in_block = false;
                if (!JavaCCGlobals.JjtreeGenerated && !isJavaDialect)
                {
                    // for the last one, if it's an action, we will not protect it.
                    Expansion elem = (Expansion)e_nrw9.units[i];
                    if (elem is not Action ||
                        e.parent is not BNFProduction ||
                        i != e_nrw9.units.Count - 1)
                    {
                        wrap_in_block = true;
                        retval += "\nif (" + (isJavaDialect ? "true" : "!hasError") + ") {";
                    }
                }
                retval += phase1ExpansionGen((Expansion)(e_nrw9.units[i]));
                if (wrap_in_block)
                {
                    retval += "\n}";
                }
            }
        }
        else if (e is OneOrMore e_nrw12)
        {
            Expansion nested_e = e_nrw12.expansion;
            Lookahead la;
            if (nested_e is Sequence)
            {
                la = (Lookahead)(((Sequence)nested_e).units[0]);
            }
            else
            {
                la = new Lookahead();
                la.SetAmount(Options.GetLookahead());
                la.SetLaExpansion(nested_e);
            }
            retval += "\n";
            int labelIndex = ++gensymindex;
            if (isJavaDialect)
            {
                retval += "label_" + labelIndex + ":\n";
            }
            retval += "while (" + (isJavaDialect ? "true" : "!hasError") + ") {\u0001";
            retval += phase1ExpansionGen(nested_e);
            conds = new Lookahead[1];
            conds[0] = la;
            actions = new string[2];
            actions[0] = "\n;";

            if (isJavaDialect)
            {
                actions[1] = "\nbreak label_" + labelIndex + ";";
            }
            else
            {
                actions[1] = "\ngoto end_label_" + labelIndex + ";";
            }

            retval += BuildLookaheadChecker(conds, actions);
            retval += "\u0002\n" + "}";
            if (!isJavaDialect)
            {
                retval += "\nend_label_" + labelIndex + ": ;";
            }
        }
        else if (e is ZeroOrMore e_nrw13)
        {
            Expansion nested_e = e_nrw13.expansion;
            Lookahead la;
            if (nested_e is Sequence)
            {
                la = (Lookahead)(((Sequence)nested_e).units[0]);
            }
            else
            {
                la = new Lookahead();
                la.SetAmount(Options.GetLookahead());
                la.SetLaExpansion(nested_e);
            }
            retval += "\n";
            int labelIndex = ++gensymindex;
            if (isJavaDialect)
            {
                retval += "label_" + labelIndex + ":\n";
            }
            retval += "while (" + (isJavaDialect ? "true" : "!hasError") + ") {\u0001";
            conds = new Lookahead[1];
            conds[0] = la;
            actions = new String[2];
            actions[0] = "\n;";
            if (isJavaDialect)
            {
                actions[1] = "\nbreak label_" + labelIndex + ";";
            }
            else
            {
                actions[1] = "\ngoto end_label_" + labelIndex + ";";
            }
            retval += BuildLookaheadChecker(conds, actions);
            retval += phase1ExpansionGen(nested_e);
            retval += "\u0002\n" + "}";
            if (!isJavaDialect)
            {
                retval += "\nend_label_" + labelIndex + ": ;";
            }
        }
        else if (e is ZeroOrOne e_nrw14)
        {
            Expansion nested_e = e_nrw14.expansion;
            Lookahead la;
            if (nested_e is Sequence sequence)
            {
                la = (Lookahead)(sequence.units[0]);
            }
            else
            {
                la = new Lookahead();
                la.SetAmount(Options.GetLookahead());
                la.SetLaExpansion(nested_e);
            }
            conds = new Lookahead[1];
            conds[0] = la;
            actions = new String[2];
            actions[0] = phase1ExpansionGen(nested_e);
            actions[1] = "\n;";
            retval += BuildLookaheadChecker(conds, actions);
        }
        else if (e is TryBlock e_nrw15)
        {
            Expansion nested_e = e_nrw15.exp;
            List<Token> list;
            retval += "\n";
            retval += "try {\u0001";
            retval += phase1ExpansionGen(nested_e);
            retval += "\u0002\n" + "}";
            for (int i = 0; i < e_nrw15.catchblks.Count; i++)
            {
                retval += " catch (";
                list = (e_nrw15.types[i]);
                if (list.Count != 0)
                {
                    JavaCCGlobals.PrintTokenSetup((Token)(list[0]));
                    for (Iterator it = list.iterator(); it.hasNext();)
                    {
                        t = (Token)it.next();
                        retval += _CodeGenerator.GetStringToPrint(t);
                    }
                    retval += _CodeGenerator.GetTrailingComments(t);
                }
                retval += " ";
                t = (e_nrw15.ids[i]);
                JavaCCGlobals.PrintTokenSetup(t);
                retval += _CodeGenerator.GetStringToPrint(t);
                retval += _CodeGenerator.GetTrailingComments(t);
                retval += ") {\u0003\n";
                list = (e_nrw15.catchblks[i]);
                if (list.Count != 0)
                {
                    JavaCCGlobals.PrintTokenSetup((list[0]));
                    JavaCCGlobals.ccol = 1;
                    for (Iterator it = list.iterator(); it.hasNext();)
                    {
                        t = (Token)it.next();
                        retval += _CodeGenerator.GetStringToPrint(t);
                    }
                    retval += _CodeGenerator.GetTrailingComments(t);
                }
                retval += "\u0004\n" + "}";
            }
            if (e_nrw15.finallyblk != null)
            {
                if (isJavaDialect)
                {
                    retval += " finally {\u0003\n";
                }
                else
                {
                    retval += " finally {\u0003\n";
                }

                if (e_nrw15.finallyblk.Count != 0)
                {
                    JavaCCGlobals.PrintTokenSetup((Token)(e_nrw15.finallyblk[0])); 
                    JavaCCGlobals.ccol = 1;
                    for (Iterator it = e_nrw15.finallyblk.iterator(); it.hasNext();)
                    {
                        t = (Token)it.next();
                        retval += _CodeGenerator.GetStringToPrint(t);
                    }
                    retval += _CodeGenerator.GetTrailingComments(t);
                }
                retval += "\u0004\n" + "}";
            }
        }
        return retval;
    }

    void buildPhase2Routine(Lookahead la)
    {
        Expansion e = la.GetLaExpansion();
        if (isJavaDialect)
        {
            _CodeGenerator.GenCodeLine("  " + JavaCCGlobals.StaticOpt() + "private " + Options.GetBooleanType() + " jj_2" + e.internal_name + "(int xla)");
        }
        else
        {
            _CodeGenerator.GenCodeLine(" inline bool ", "jj_2" + e.internal_name + "(int xla)");
        }
        _CodeGenerator.GenCodeLine(" {");
        _CodeGenerator.GenCodeLine("    jj_la = xla; jj_lastpos = jj_scanpos = token;");

        string ret_suffix = "";
        if (Options.GetDepthLimit() > 0)
        {
            ret_suffix = " && !jj_depth_error";
        }

        if (isJavaDialect)
        {
            _CodeGenerator.GenCodeLine("    try { return (!jj_3" + e.internal_name + "()" + ret_suffix + "); }");
            _CodeGenerator.GenCodeLine("    catch(LookaheadSuccess ls) { return true; }");
        }
        else
        {
            _CodeGenerator.GenCodeLine("    jj_done = false;");
            _CodeGenerator.GenCodeLine("    return (!jj_3" + e.internal_name + "() || jj_done)" + ret_suffix + ";");
        }
        if (Options.GetErrorReporting())
        {
            _CodeGenerator.GenCodeLine((isJavaDialect ? "    finally " : " ") + "{ jj_save(" + (int.parseInt(e.internal_name[1..]) - 1) + ", xla); }");
        }
        _CodeGenerator.GenCodeLine("  }");
        _CodeGenerator.GenCodeLine("");
        var p3d = new Phase3Data(e, la.GetAmount());
        phase3list.Add(p3d);
        phase3table.Add(e, p3d);
    }

    private bool xsp_declared;

    Expansion jj3_expansion;

    string genReturn(bool value)
    {
        string retval = (value ? "true" : "false");
        if (Options.GetDebugLookahead() && jj3_expansion != null)
        {
            string tracecode = "trace_return(\"" + JavaCCGlobals.AddUnicodeEscapes(((NormalProduction)jj3_expansion.parent).GetLhs()) +
            "(LOOKAHEAD " + (value ? "FAILED" : "SUCCEEDED") + ")\");";
            if (Options.GetErrorReporting())
            {
                tracecode = "if (!jj_rescan) " + tracecode;
            }
            return "{ " + tracecode + " return " + retval + "; }";
        }
        else
        {
            return "return " + retval + ";";
        }
    }

    private void generate3R(Expansion e, Phase3Data inf, Expansion seq)
    {
        Expansion seq = e;
        if (e.internal_name == (""))
        {
            while (true)
            {
                if (seq is Sequence && ((Sequence)seq).units.Count == 2)
                {
                    seq = (Expansion)((Sequence)seq).units.get(1);
                }
                else if (seq is NonTerminal)
                {
                    NonTerminal e_nrw = (NonTerminal)seq;
                    NormalProduction ntprod = (NormalProduction)production_table.get(e_nrw.GetName());
                    if (ntprod is CodeProduction)
                    {
                        break; // nothing to do here
                    }
                    else
                    {
                        seq = ntprod.GetExpansion();
                    }
                }
                else
                    break;
            }

            if (seq is RegularExpression expression)
            {
                e.internal_name = "jj_scan_token(" + expression.ordinal + ")";
                return;
            }

            gensymindex++;
            //    if (gensymindex == 100)
            //    {
            //    new Error().codeGenerator.printStackTrace();
            //    Console.WriteLine(" ***** seq: " + seq.internal_name + "; size: " + ((Sequence)seq).units.Count);
            //    }
            e.internal_name = "R_" + e.GetProductionName() + "_" + e.GetLine() + "_" + e.GetColumn() + "_" + gensymindex;
            e.internal_index = gensymindex;
        }
        if (!phase3table.TryGetValue(e, out var p3d) || p3d.count < inf.count)
        {
            p3d = new Phase3Data(e, inf.count);
            phase3list.Add(p3d);
            phase3table.Add(e, p3d);
        }
    }

    void setupPhase3Builds(Phase3Data inf)
    {
        Expansion e = inf.exp;
        if (e is RegularExpression)
        {
            ; // nothing to here
        }
        else if (e is NonTerminal)
        {
            // All expansions of non-terminals have the "name" fields set.  So
            // there's no need to check it below for "e_nrw" and "ntexp".  In
            // fact, we rely here on the fact that the "name" fields of both these
            // variables are the same.
            NonTerminal e_nrw = (NonTerminal)e;
            NormalProduction ntprod = (NormalProduction)(production_table.get(e_nrw.GetName()));
            if (ntprod is CodeProduction)
            {
                ; // nothing to do here
            }
            else
            {
                generate3R(ntprod.GetExpansion(), inf, seq);
            }
        }
        else if (e is Choice)
        {
            Choice e_nrw = (Choice)e;
            for (int i = 0; i < e_nrw.GetChoices().Count; i++)
            {
                generate3R((Expansion)(e_nrw.GetChoices()[i]), inf, seq);
            }
        }
        else if (e is Sequence)
        {
            Sequence e_nrw = (Sequence)e;
            // We skip the first element in the following iteration since it is the
            // Lookahead object.
            int cnt = inf.count;
            for (int i = 1; i < e_nrw.units.Count; i++)
            {
                Expansion eseq = (Expansion)(e_nrw.units[i]);
                setupPhase3Builds(new Phase3Data(eseq, cnt));
                cnt -= minimumSize(eseq);
                if (cnt <= 0) break;
            }
        }
        else if (e is TryBlock)
        {
            TryBlock e_nrw = (TryBlock)e;
            setupPhase3Builds(new Phase3Data(e_nrw.exp, inf.count));
        }
        else if (e is OneOrMore)
        {
            OneOrMore e_nrw = (OneOrMore)e;
            generate3R(e_nrw.expansion, inf, seq);
        }
        else if (e is ZeroOrMore)
        {
            ZeroOrMore e_nrw = (ZeroOrMore)e;
            generate3R(e_nrw.expansion, inf, seq);
        }
        else if (e is ZeroOrOne e_nrw)
        {
            generate3R(e_nrw.expansion, inf, seq);
        }
    }

    private string getTypeForToken()
    {
        return isJavaDialect ? "Token" : "Token *";
    }

    private string genjj_3Call(Expansion e)
    {
        if (e.internal_name.StartsWith("jj_scan_token"))
            return e.internal_name;
        else
            return "jj_3" + e.internal_name + "()";
    }

    Dictionary generated = new Dictionary();
    void buildPhase3Routine(Phase3Data inf, bool recursive_call)
    {
        Expansion e = inf.exp;
        Token t = null;
        if (e.internal_name.StartsWith("jj_scan_token"))
            return;

        if (!recursive_call)
        {
            if (isJavaDialect)
            {
                _CodeGenerator.GenCodeLine("  " + JavaCCGlobals.StaticOpt() + "private " + Options.GetBooleanType() + " jj_3" + e.internal_name + "()");
            }
            else
            {
                _CodeGenerator.GenCodeLine(" inline bool ", "jj_3" + e.internal_name + "()");
            }

            _CodeGenerator.GenCodeLine(" {");
            if (!isJavaDialect)
            {
                _CodeGenerator.GenCodeLine("    if (jj_done) return true;");
                if (Options.GetDepthLimit() > 0)
                {
                    _CodeGenerator.GenCodeLine("#define __ERROR_RET__ true");
                }
            }
            genStackCheck(false);
            xsp_declared = false;
            if (Options.GetDebugLookahead() && e.parent is NormalProduction)
            {
                _CodeGenerator.GenCode("    ");
                if (Options.GetErrorReporting())
                {
                    _CodeGenerator.GenCode("if (!jj_rescan) ");
                }
                _CodeGenerator.GenCodeLine("trace_call(\"" + JavaCCGlobals.AddUnicodeEscapes(((NormalProduction)e.parent).GetLhs()) + "(LOOKING AHEAD...)\");");
                jj3_expansion = e;
            }
            else
            {
                jj3_expansion = null;
            }
        }
        if (e is RegularExpression)
        {
            RegularExpression e_nrw = (RegularExpression)e;
            if (e_nrw.label == "")
            {
                if (JavaCCGlobals.names_of_tokens.TryGetValue(e_nrw.ordinal,out var label))
                {
                    _CodeGenerator.GenCodeLine("    if (jj_scan_token(" + (String)label + ")) " + genReturn(true));
                }
                else
                {
                    _CodeGenerator.GenCodeLine("    if (jj_scan_token(" + e_nrw.ordinal + ")) " + genReturn(true));
                }
            }
            else
            {
                _CodeGenerator.GenCodeLine("    if (jj_scan_token(" + e_nrw.label + ")) " + genReturn(true));
            }
            //codeGenerator.GenCodeLine("    if (jj_la == 0 && jj_scanpos == jj_lastpos) " + genReturn(false));
        }
        else if (e is NonTerminal)
        {
            // All expansions of non-terminals have the "name" fields set.  So
            // there's no need to check it below for "e_nrw" and "ntexp".  In
            // fact, we rely here on the fact that the "name" fields of both these
            // variables are the same.
            NonTerminal e_nrw = (NonTerminal)e;
            NormalProduction ntprod = (NormalProduction)(production_table.get(e_nrw.GetName()));
            if (ntprod is CodeProduction)
            {
                _CodeGenerator.GenCodeLine("    if (true) { jj_la = 0; jj_scanpos = jj_lastpos; " + genReturn(false) + "}");
            }
            else
            {
                Expansion ntexp = ntprod.GetExpansion();
                //codeGenerator.GenCodeLine("    if (jj_3" + ntexp.internal_name + "()) " + genReturn(true));
                _CodeGenerator.GenCodeLine("    if (" + genjj_3Call(ntexp) + ") " + genReturn(true));
                //codeGenerator.GenCodeLine("    if (jj_la == 0 && jj_scanpos == jj_lastpos) " + genReturn(false));
            }
        }
        else if (e is Choice)
        {
            Sequence nested_seq;
            Choice e_nrw = (Choice)e;
            if (e_nrw.GetChoices().Count != 1)
            {
                if (!xsp_declared)
                {
                    xsp_declared = true;
                    _CodeGenerator.GenCodeLine("    " + getTypeForToken() + " xsp;");
                }
                _CodeGenerator.GenCodeLine("    xsp = jj_scanpos;");
            }
            for (int i = 0; i < e_nrw.GetChoices().Count; i++)
            {
                nested_seq = (Sequence)(e_nrw.GetChoices()[i]);
                Lookahead la = (Lookahead)(nested_seq.units[0]);
                if (la.GetActionTokens().Count != 0)
                {
                    // We have semantic lookahead that must be evaluated.
                    lookaheadNeeded = true;
                    _CodeGenerator.GenCodeLine("    jj_lookingAhead = true;");
                    _CodeGenerator.GenCode("    jj_semLA = ");
                    JavaCCGlobals.PrintTokenSetup((Token)(la.GetActionTokens()[0]));
                    for (Iterator it = la.GetActionTokens().iterator(); it.hasNext();)
                    {
                        t = (Token)it.next();
                        _CodeGenerator.PrintToken(t);
                    }
                    _CodeGenerator.PrintTrailingComments(t);
                    _CodeGenerator.GenCodeLine(";");
                    _CodeGenerator.GenCodeLine("    jj_lookingAhead = false;");
                }
                _CodeGenerator.GenCode("    if (");
                if (la.GetActionTokens().Count != 0)
                {
                    _CodeGenerator.GenCode("!jj_semLA || ");
                }
                if (i != e_nrw.GetChoices().Count - 1)
                {
                    //codeGenerator.GenCodeLine("jj_3" + nested_seq.internal_name + "()) {");
                    _CodeGenerator.GenCodeLine(genjj_3Call(nested_seq) + ") {");
                    _CodeGenerator.GenCodeLine("    jj_scanpos = xsp;");
                }
                else
                {
                    //codeGenerator.GenCodeLine("jj_3" + nested_seq.internal_name + "()) " + genReturn(true));
                    _CodeGenerator.GenCodeLine(genjj_3Call(nested_seq) + ") " + genReturn(true));
                    //codeGenerator.GenCodeLine("    if (jj_la == 0 && jj_scanpos == jj_lastpos) " + genReturn(false));
                }
            }
            for (int i = 1; i < e_nrw.GetChoices().Count; i++)
            {
                //codeGenerator.GenCodeLine("    } else if (jj_la == 0 && jj_scanpos == jj_lastpos) " + genReturn(false));
                _CodeGenerator.GenCodeLine("    }");
            }
        }
        else if (e is Sequence)
        {
            Sequence e_nrw = (Sequence)e;
            // We skip the first element in the following iteration since it is the
            // Lookahead object.
            int cnt = inf.count;
            for (int i = 1; i < e_nrw.units.Count; i++)
            {
                Expansion eseq = (Expansion)(e_nrw.units[i]);
                buildPhase3Routine(new Phase3Data(eseq, cnt), true);

                //      Console.WriteLine("minimumSize: line: " + eseq.line + ", column: " + eseq.column + ": " +
                //      minimumSize(eseq));//Test Code

                cnt -= minimumSize(eseq);
                if (cnt <= 0) break;
            }
        }
        else if (e is TryBlock)
        {
            TryBlock e_nrw = (TryBlock)e;
            buildPhase3Routine(new Phase3Data(e_nrw.exp, inf.count), true);
        }
        else if (e is OneOrMore)
        {
            if (!xsp_declared)
            {
                xsp_declared = true;
                _CodeGenerator.GenCodeLine("    " + getTypeForToken() + " xsp;");
            }
            OneOrMore e_nrw = (OneOrMore)e;
            Expansion nested_e = e_nrw.expansion;
            //codeGenerator.GenCodeLine("    if (jj_3" + nested_e.internal_name + "()) " + genReturn(true));
            _CodeGenerator.GenCodeLine("    if (" + genjj_3Call(nested_e) + ") " + genReturn(true));
            //codeGenerator.GenCodeLine("    if (jj_la == 0 && jj_scanpos == jj_lastpos) " + genReturn(false));
            _CodeGenerator.GenCodeLine("    while (true) {");
            _CodeGenerator.GenCodeLine("      xsp = jj_scanpos;");
            //codeGenerator.GenCodeLine("      if (jj_3" + nested_e.internal_name + "()) { jj_scanpos = xsp; break; }");
            _CodeGenerator.GenCodeLine("      if (" + genjj_3Call(nested_e) + ") { jj_scanpos = xsp; break; }");
            //codeGenerator.GenCodeLine("      if (jj_la == 0 && jj_scanpos == jj_lastpos) " + genReturn(false));
            _CodeGenerator.GenCodeLine("    }");
        }
        else if (e is ZeroOrMore)
        {
            if (!xsp_declared)
            {
                xsp_declared = true;
                _CodeGenerator.GenCodeLine("    " + getTypeForToken() + " xsp;");
            }
            ZeroOrMore e_nrw = (ZeroOrMore)e;
            Expansion nested_e = e_nrw.expansion;
            _CodeGenerator.GenCodeLine("    while (true) {");
            _CodeGenerator.GenCodeLine("      xsp = jj_scanpos;");
            //codeGenerator.GenCodeLine("      if (jj_3" + nested_e.internal_name + "()) { jj_scanpos = xsp; break; }");
            _CodeGenerator.GenCodeLine("      if (" + genjj_3Call(nested_e) + ") { jj_scanpos = xsp; break; }");
            //codeGenerator.GenCodeLine("      if (jj_la == 0 && jj_scanpos == jj_lastpos) " + genReturn(false));
            _CodeGenerator.GenCodeLine("    }");
        }
        else if (e is ZeroOrOne)
        {
            if (!xsp_declared)
            {
                xsp_declared = true;
                _CodeGenerator.GenCodeLine("    " + getTypeForToken() + " xsp;");
            }
            ZeroOrOne e_nrw = (ZeroOrOne)e;
            Expansion nested_e = e_nrw.expansion;
            _CodeGenerator.GenCodeLine("    xsp = jj_scanpos;");
            //codeGenerator.GenCodeLine("    if (jj_3" + nested_e.internal_name + "()) jj_scanpos = xsp;");
            _CodeGenerator.GenCodeLine("    if (" + genjj_3Call(nested_e) + ") jj_scanpos = xsp;");
            //codeGenerator.GenCodeLine("    else if (jj_la == 0 && jj_scanpos == jj_lastpos) " + genReturn(false));
        }
        if (!recursive_call)
        {
            _CodeGenerator.GenCodeLine("    " + genReturn(false));
            genStackCheckEnd();
            if (!isJavaDialect && Options.GetDepthLimit() > 0)
            {
                _CodeGenerator.GenCodeLine("#undef __ERROR_RET__");
            }
            _CodeGenerator.GenCodeLine("  }");
            _CodeGenerator.GenCodeLine("");
        }
    }

    int minimumSize(Expansion e)
    {
        return minimumSize(e, int.MaxValue);
    }

    /*
     * Returns the minimum number of tokens that can parse to this expansion.
     */
    int minimumSize(Expansion e, int oldMin)
    {
        int retval = 0;  // should never be used.  Will be bad if it is.
        if (e.inMinimumSize)
        {
            // recursive search for minimum size unnecessary.
            return int.MaxValue;
        }
        e.inMinimumSize = true;
        if (e is RegularExpression)
        {
            retval = 1;
        }
        else if (e is NonTerminal e_nrw6)
        {
            NormalProduction ntprod = (NormalProduction)(production_table.get(e_nrw6.GetName()));
            if (ntprod is CodeProduction)
            {
                retval = int.MaxValue;
                // Make caller think this is unending (for we do not go beyond JAVACODE during
                // phase3 execution).
            }
            else
            {
                Expansion ntexp = ntprod.GetExpansion();
                retval = minimumSize(ntexp);
            }
        }
        else if (e is Choice)
        {
            int min = oldMin;
            Expansion nested_e;
            Choice e_nrw = (Choice)e;
            for (int i = 0; min > 1 && i < e_nrw.GetChoices().Count; i++)
            {
                nested_e = e_nrw.GetChoices()[i];
                int min1 = minimumSize(nested_e, min);
                if (min > min1) min = min1;
            }
            retval = min;
        }
        else if (e is Sequence e_nrw11)
        {
            int min = 0;
            // We skip the first element in the following iteration since it is the
            // Lookahead object.
            for (int i = 1; i < e_nrw11.units.Count; i++)
            {
                Expansion eseq = e_nrw11.units[i];
                int mineseq = minimumSize(eseq);
                if (min == int.MaxValue || mineseq == int.MaxValue)
                {
                    min = int.MaxValue; // Adding infinity to something results in infinity.
                }
                else
                {
                    min += mineseq;
                    if (min > oldMin)
                        break;
                }
            }
            retval = min;
        }
        else if (e is TryBlock e_nrw7)
        {
            retval = minimumSize(e_nrw7.exp);
        }
        else if (e is OneOrMore e_nrw8)
        {
            retval = minimumSize(e_nrw8.expansion);
        }
        else if (e is ZeroOrMore)
        {
            retval = 0;
        }
        else if (e is ZeroOrOne)
        {
            retval = 0;
        }
        else if (e is Lookahead)
        {
            retval = 0;
        }
        else if (e is Action)
        {
            retval = 0;
        }
        e.inMinimumSize = false;
        return retval;
    }

    public void build(CodeGenerator codeGenerator)
    {
        Token t = null;

        this._CodeGenerator = codeGenerator;
        foreach(var p in JavaCCGlobals.bnfproductions)
        {
            if (p is CppCodeProduction cp)
            {
                GenerateCPPMethodheader(cp);
                //          t = (Token)(cp.getReturnTypeTokens()[0]);
                //          codeGenerator.printTokenSetup(t); ccol = 1;
                //          codeGenerator.printLeadingComments(t);
                //          codeGenerator.GenCode("  " + staticOpt() + (p.getAccessMod() != null ? p.getAccessMod() + " " : ""));
                //          cline = t.beginLine; ccol = t.beginColumn;
                //          codeGenerator.printTokenOnly(t);
                //          for (int i = 1; i < cp.getReturnTypeTokens().Count; i++) {
                //            t = (Token)(cp.getReturnTypeTokens()[i]);
                //            codeGenerator.printToken(t);
                //          }
                //          codeGenerator.printTrailingComments(t);
                //          codeGenerator.GenCode(" " + cp.getLhs() + "(");
                //          if (cp.getParameterListTokens().Count != 0) {
                //            codeGenerator.printTokenSetup((Token)(cp.getParameterListTokens()[0]));
                //            for (Iterator it = cp.getParameterListTokens().iterator(); it.hasNext();) {
                //              t = (Token)it.next();
                //              codeGenerator.printToken(t);
                //            }
                //            codeGenerator.printTrailingComments(t);
                //          }
                //          codeGenerator.GenCode(")");
                //          for (Iterator it = cp.getThrowsList().iterator(); it.hasNext();) {
                //            codeGenerator.GenCode(", ");
                //            List name = (List)it.next();
                //            for (Iterator it2 = name.iterator(); it2.hasNext();) {
                //              t = (Token)it2.next();
                //              codeGenerator.GenCode(t.image);
                //            }
                //          }
                codeGenerator.GenCodeLine(" {");
                if (Options.GetDebugParser())
                {
                    codeGenerator.GenCodeLine("");
                    if (isJavaDialect)
                    {
                        codeGenerator.GenCodeLine("    trace_call(\"" + JavaCCGlobals.AddUnicodeEscapes(cp.GetLhs()) + "\");");
                    }
                    else
                    {
                        codeGenerator.GenCodeLine("    JJEnter<std::function<void()>> jjenter([this]() {trace_call  (\"" + JavaCCGlobals.AddUnicodeEscapes(cp.GetLhs()) + "\"); });");
                        codeGenerator.GenCodeLine("    JJExit <std::function<void()>> jjexit ([this]() {trace_return(\"" + JavaCCGlobals.AddUnicodeEscapes(cp.GetLhs()) + "\"); });");
                    }
                    codeGenerator.GenCodeLine("    try {");
                }
                if (cp.GetCodeTokens().Count != 0)
                {
                    JavaCCGlobals.PrintTokenSetup(cp.GetCodeTokens()[0]); 
                    JavaCCGlobals.cline--;
                    codeGenerator.PrintTokenList(cp.GetCodeTokens());
                }
                codeGenerator.GenCodeLine("");
                if (Options.GetDebugParser())
                {
                    codeGenerator.GenCodeLine("    } catch(...) { }");
                }
                codeGenerator.GenCodeLine("  }");
                codeGenerator.GenCodeLine("");
            }
            else
            if (p is JavaCodeProduction jp)
            {
                if (!isJavaDialect)
                {
                    JavaCCErrors.SemanticError("Cannot use JAVACODE productions with C++ output (yet).");
                    continue;
                }
                t = jp.GetReturnTypeTokens()[0];
                JavaCCGlobals.PrintTokenSetup(t);
                JavaCCGlobals.ccol = 1;
                codeGenerator.PrintLeadingComments(t);
                codeGenerator.GenCode("  " + JavaCCGlobals.StaticOpt() + (p.GetAccessMod() != null ? p.GetAccessMod() + " " : ""));
                JavaCCGlobals.cline = t.beginLine; 
                JavaCCGlobals.ccol = t.beginColumn;
                codeGenerator.PrintTokenOnly(t);
                for (int i = 1; i < jp.GetReturnTypeTokens().Count; i++)
                {
                    t = (Token)(jp.GetReturnTypeTokens()[i]);
                    codeGenerator.PrintToken(t);
                }
                codeGenerator.PrintTrailingComments(t);
                codeGenerator.GenCode(" " + jp.GetLhs() + "(");
                if (jp.GetParameterListTokens().Count != 0)
                {
                    JavaCCGlobals.PrintTokenSetup((jp.GetParameterListTokens()[0]));
                    foreach(var t2 in jp.GetParameterListTokens())
                    {
                        codeGenerator.PrintToken(t=t2);
                    }
                    codeGenerator.PrintTrailingComments(t);
                }
                codeGenerator.GenCode(")");
                if (isJavaDialect)
                {
                    codeGenerator.GenCode(" throws ParseException");
                }
                foreach(var name in jp.GetThrowsList())
                {
                    codeGenerator.GenCode(", ");
                    foreach(var t2 in name)
                    {
                        t = t2;
                        codeGenerator.GenCode(t.image);
                    }
                }
                codeGenerator.GenCode(" {");
                if (Options.GetDebugParser())
                {
                    codeGenerator.GenCodeLine("");
                    codeGenerator.GenCodeLine("    trace_call(\"" + JavaCCGlobals.AddUnicodeEscapes(jp.GetLhs()) + "\");");
                    codeGenerator.GenCode("    try {");
                }
                if (jp.GetCodeTokens().Count != 0)
                {
                    JavaCCGlobals.PrintTokenSetup((jp.GetCodeTokens()[0]));
                    JavaCCGlobals.cline--;
                    codeGenerator.PrintTokenList(jp.GetCodeTokens());
                }
                codeGenerator.GenCodeLine("");
                if (Options.GetDebugParser())
                {
                    codeGenerator.GenCodeLine("    } finally {");
                    codeGenerator.GenCodeLine("      trace_return(\"" + JavaCCGlobals.AddUnicodeEscapes(jp.GetLhs()) + "\");");
                    codeGenerator.GenCodeLine("    }");
                }
                codeGenerator.GenCodeLine("  }");
                codeGenerator.GenCodeLine("");
            }
            else
            {
                buildPhase1Routine((BNFProduction)p);
            }
        }

        codeGenerator.SwitchToIncludeFile();
        for (int phase2index = 0; phase2index < phase2list.Count; phase2index++)
        {
            buildPhase2Routine((Lookahead)(phase2list[phase2index]));
        }

        int phase3index = 0;

        while (phase3index < phase3list.Count)
        {
            for (; phase3index < phase3list.Count; phase3index++)
            {
                setupPhase3Builds((Phase3Data)(phase3list[phase3index]));
            }
        }

        foreach(var phase3 in phase3table.Values)
        {
            buildPhase3Routine(phase3, false);
        }
        // for (Enumeration enumeration = phase3table.elements(); enumeration.hasMoreElements();) {
        // Phase3Data inf = (Phase3Data)(enumeration.nextElement());
        // Console.Error.WriteLine("**** Table for: " + inf.exp.internal_name);
        // buildPhase3Table(inf);
        // Console.Error.WriteLine("**** END TABLE *********");
        // }

        codeGenerator.SwitchToMainFile();
    }

    public void reInit()
    {
        gensymindex = 0;
        indentamt = 0;
        jj2LA = false;
        phase2list = new();
        phase3list = new();
        phase3table = new();
        firstSet = null;
        xsp_declared = false;
        jj3_expansion = null;
    }

    // Table driven.
    void buildPhase3Table(Phase3Data inf)
    {
        Expansion e = inf.exp;
        Token t = null;
        if (e is RegularExpression e_nrw)
        {
            Console.Error.WriteLine("TOKEN, " + e_nrw.ordinal);
        }
        else if (e is NonTerminal e_nrw2)
        {
            if (!JavaCCGlobals.production_table.TryGetValue(e_nrw2.GetName(), out var ntprod))
                return;

            if (ntprod is CodeProduction)
            {
                // javacode, true - always (warn?)
                Console.Error.WriteLine("JAVACODE_PROD, true");
            }
            else
            {
                Expansion ntexp = ntprod.GetExpansion();
                // nt exp's table.
                Console.Error.WriteLine("PRODUCTION, " + ntexp.internal_index);
                //buildPhase3Table(new Phase3Data(ntexp, inf.count));
            }
        }
        else if (e is Choice e_nrw4)
        {
            Sequence nested_seq;
            Console.Error.Write("CHOICE, ");
            for (int i = 0; i < e_nrw4.GetChoices().Count; i++)
            {
                if (i > 0) Console.Error.Write("\n|");
                nested_seq = (Sequence)(e_nrw4.GetChoices()[i]);
                Lookahead la = (Lookahead)(nested_seq.units[0]);
                if (la.GetActionTokens().Count != 0)
                {
                    Console.Error.Write("SEMANTIC,");
                }
                else
                {
                    buildPhase3Table(new Phase3Data(nested_seq, inf.count));
                }
            }
            Console.Error.WriteLine();
        }
        else if (e is Sequence e_nrw5)
        {
            int cnt = inf.count;
            if (e_nrw5.units.Count > 2)
            {
                Console.Error.WriteLine("SEQ, " + cnt);
                for (int i = 1; i < e_nrw5.units.Count; i++)
                {
                    Console.Error.Write("   ");
                    Expansion eseq = (Expansion)(e_nrw5.units[i]);
                    buildPhase3Table(new Phase3Data(eseq, cnt));
                    cnt -= minimumSize(eseq);
                    if (cnt <= 0) break;
                }
            }
            else
            {
                var tmp = e_nrw5.units[1];
                while (tmp is NonTerminal tx)
                {
                    if (!JavaCCGlobals.production_table.TryGetValue(tx.GetName(), out var ntprod))
                        break;

                    if (ntprod is CodeProduction) break;
                    tmp = ntprod.GetExpansion();
                }
                buildPhase3Table(new Phase3Data(tmp, cnt));
            }
            Console.Error.WriteLine();
        }
        else if (e is TryBlock e_nrw6)
        {
            buildPhase3Table(new Phase3Data(e_nrw6.exp, inf.count));
        }
        else if (e is OneOrMore e_nrw15)
        {
            Console.Error.WriteLine("SEQ PROD " + e_nrw15.expansion.internal_index);
            Console.Error.WriteLine("ZEROORMORE " + e_nrw15.expansion.internal_index);
        }
        else if (e is ZeroOrMore e_nrw14)
        {
            Console.Error.Write("ZEROORMORE, " + e_nrw14.expansion.internal_index);
        }
        else if (e is ZeroOrOne e_nrw3)
        {
            Console.Error.WriteLine("ZERORONE, " + e_nrw3.expansion.internal_index);
        }
        else
        {
            //assert(false);
            // table for nested_e - optional
        }
    }
}
