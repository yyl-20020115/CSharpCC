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
 * A report generator for a grammar.
 * @author timp
 * @since 11-Dec-2006
 *
 */
public interface Generator
{

    /**
     * Output string with entity substitution for brackets and ampersands.
     * @param s the string to output
     */
    void Text(string s);

    /**
     * Output String.
     * @param s string to output
     */
    void Print(string s);

    /**
     * Output document header.
     */
    void DocumentStart();

    /**
     * Output document footer.
     */
    void DocumentEnd();

    /**
     * Output Special Tokens.
     * @param s tokens to output
     */
    void SpecialTokens(string s);



    void HandleTokenProduction(TokenProduction tp);

    //  /**
    //   * Output start of a TokenProduction.
    //   * @param tp the TokenProduction being output
    //   */
    //  void tokenStart(TokenProduction tp);
    //
    //  /**
    //   * Output end of a TokenProduction.
    //   * @param tp the TokenProduction being output
    //   */
    //  void tokenEnd(TokenProduction tp);

    /**
     * Output start of non-terminal.
     */
    void NonterminalsStart();

    /**
     * Output end of non-terminal.
     */
    void NonterminalsEnd();

    /**
     * Output start of tokens.
     */
    void TokensStart();

    /**
     * Output end of tokens.
     */
    void TokensEnd();

    /**
     * Output comment from a production.
     * @param jp the JavaCodeProduction to output
     */
    void Javacode(CSharpCodeProduction jp);

    /**
     * Output comment from a production.
     * @param cp the CppCodeProduction to output
     */
    void Cppcode(CppCodeProduction cp);

    /**
     * Output start of a normal production.
     * @param np the NormalProduction being output
     */
    void ProductionStart(NormalProduction np);

    /**
     * Output end of a normal production.
     * @param np the NormalProduction being output
     */
    void ProductionEnd(NormalProduction np);

    /**
     * Output start of an Expansion.
     * @param e Expansion being output
     * @param first whether this is the first expansion
     */
    void ExpansionStart(Expansion e, bool first);

    /**
     * Output end of Expansion.
     * @param e Expansion being output
     * @param first whether this is the first expansion
     */
    void ExpansionEnd(Expansion e, bool first);

    /**
     * Output start of non-terminal.
     * @param nt the NonTerminal being output
     */
    void NonTerminalStart(NonTerminal nt);

    /**
     * Output end of non-terminal.
     * @param nt the NonTerminal being output
     */
    void NonTerminalEnd(NonTerminal nt);

    /**
     * Output start of regular expression.
     * @param re the RegularExpression being output
     */
    void ReStart(RegularExpression re);

    /**
     * Output end of regular expression.
     * @param re the RegularExpression being output
     */
    void ReEnd(RegularExpression re);

    /**
     * Log debug messages.
     * @param message the string to log
     */
    void Debug(string message);

    /**
     * Log informational messages.
     * @param message the string to log
     */
    void Info(string message);

    /**
     * Log warning messages.
     * @param message the string to log
     */
    void Warn(string message);

    /**
     * Log error messages.
     * @param message the string to log
     */
    void Error(string message);
}
