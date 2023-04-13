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

namespace CSharpCC.Parser;


/**
 * Describes non terminals.
 */

public class NonTerminal : Expansion
{

    /**
     * The LHS to which the return value of the non-terminal
     * is assigned.  In case there is no LHS, then the vector
     * remains empty.
     */
    private List<Token> lhsTokens = new ();

    /**
     * The name of the non-terminal.
     */
    private string name;

    /**
     * The list of all tokens in the argument list.
     */
    private List<Token> argument_tokens = new ();

    private List<Token> parametrized_type__tokens = new ();
    /**
     * The production this non-terminal corresponds to.
     */
    private NormalProduction prod;

    public override StringBuilder Dump(int indent, HashSet<Expansion> alreadyDumped)
    {
        var value = base.Dump(indent, alreadyDumped).Append(' ').Append(name);
        return value;
    }

    /**
     * @return the lhsTokens
     */
    /**
 * @param lhsTokens the lhsTokens to set
 */
    public List<Token> LhsTokens { get => lhsTokens; set => this.lhsTokens = value; }

    /**
     * @return the name
     */
    /**
 * @param name the name to set
 */
    public string Name { get => name; set => this.name = value; }

    /**
     * @return the argument_tokens
     */
    /**
 * @param argument_tokens the argument_tokens to set
 */
    public List<Token> ParametrizedTypeTokens { get => parametrized_type__tokens; set => this.argument_tokens = value; }

    /**
     * @return the argument_tokens
     */
    /**
 * @param argument_tokens the argument_tokens to set
 */
    public List<Token> ArgumentTokens { get => argument_tokens; set => this.parametrized_type__tokens = value; }

    /**
     * @return the prod
     */
    /**
 * @param prod the prod to set
 */
    public NormalProduction Production { get => prod; set => this.prod = value; }
}
