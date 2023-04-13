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
 * Describes lookahead rule for a particular expansion or expansion
 * sequence (See Sequence.java).  In case this describes the lookahead
 * rule for a single expansion unit, then a sequence is created with
 * this node as the first element, and the expansion unit as the second
 * and last element.
 */

public class Lookahead : Expansion
{

    /**
     * Contains the list of tokens that make up the semantic lookahead
     * if any.  If this node represents a different kind of lookahead (other
     * than semantic lookahead), then this list contains nothing.  If
     * this list contains something, then it is the boolean expression
     * that forms the semantic lookahead.  In this case, the following
     * fields "amount" and "la_expansion" are ignored.
     */
    private readonly List<Token> actionTokens = new();

    /**
     * The lookahead amount.  Its default value essentially gives us
     * infinite lookahead.
     */
    private int amount = int.MaxValue;

    /**
     * The expansion used to determine whether or not to choose the
     * corresponding parse option.  This expansion is parsed upto
     * "amount" tokens of lookahead or until a complete match for it
     * is found.  Usually, this is the same as the expansion to be
     * parsed.
     */
    private Expansion laExpansion;

    /**
     * Is set to true if this is an explicit lookahead specification.
     */
    private bool isExplicit;

    public override StringBuilder Dump(int indent, HashSet<Expansion> alreadyDumped)
    {
        var builder = base.Dump(indent, alreadyDumped).Append(isExplicit ? " explicit" : " implicit");
        if (alreadyDumped.Contains(this))
            return builder;
        alreadyDumped.Add(this);
        builder.Append(eol).Append(laExpansion.Dump(indent + 1, alreadyDumped));
        return builder;
    }

    /**
     * @return the action_tokens
     */
    public List<Token> ActionTokens => actionTokens;

    /**
     * @return the amount
     */
    /**
 * @param amount the amount to set
 */
    public int Amount { get => amount; set => this.amount = value; }

    /**
     * @return the la_expansion
     */
    /**
 * @param la_expansion the la_expansion to set
 */
    public Expansion LaExpansion { get => laExpansion; set => this.laExpansion = value; }

    /**
     * @return the isExplicit
     */
    public bool IsExplicit() => isExplicit;
    public void SetExplicit(bool value) => this.isExplicit = value;
}
