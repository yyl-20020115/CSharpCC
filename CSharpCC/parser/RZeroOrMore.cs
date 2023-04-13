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

namespace CSharpCC.Parser;

/**
 * Describes zero-or-more regular expressions (<foo*>).
 */

public class RZeroOrMore : RegularExpression
{

    /**
     * The regular expression which is repeated zero or more times.
     */
    public RegularExpression regexpr;

    public override Nfa GenerateNfa(bool ignoreCase)
    {
        var retVal = new Nfa();
        var startState = retVal.Start;
        var finalState = retVal.End;

        var temp = regexpr.GenerateNfa(ignoreCase);

        startState.AddMove(temp.Start);
        startState.AddMove(finalState);
        temp.End.AddMove(finalState);
        temp.End.AddMove(temp.Start);

        return retVal;
    }

    public RZeroOrMore() { }

    public RZeroOrMore(Token t, RegularExpression r)
    {
        this.        Line = t.beginLine;
        this.        Column = t.beginColumn;
        this.regexpr = r;
    }
}
