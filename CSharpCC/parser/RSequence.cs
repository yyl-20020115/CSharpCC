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
 * Describes regular expressions which are sequences of
 * other regular expressions.
 */

public class RSequence : RegularExpression
{

    /**
     * The list of units in this regular expression sequence.  Each
     * list component will narrow to RegularExpression.
     */
    public List<RegularExpression> units = new ();

    public override Nfa GenerateNfa(bool ignoreCase)
    {
        if (units.Count == 1)
            return units[0].GenerateNfa(ignoreCase);

        var retVal = new Nfa();
        var startState = retVal.Start;
        var finalState = retVal.End;
        Nfa temp1;
        Nfa temp2 = null;

        RegularExpression curRE;

        curRE = units[0];
        temp1 = curRE.GenerateNfa(ignoreCase);
        startState.AddMove(temp1.Start);

        for (int i = 1; i < units.Count; i++)
        {
            curRE = units[i];

            temp2 = curRE.GenerateNfa(ignoreCase);
            temp1.End.AddMove(temp2.Start);
            temp1 = temp2;
        }

        temp2.End.AddMove(finalState);

        return retVal;
    }

    public RSequence() { }

    public RSequence(List<RegularExpression> seq)
    {
        ordinal = int.MaxValue;
        units = seq;
    }
}
