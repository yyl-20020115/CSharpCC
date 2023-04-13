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
namespace org.javacc.parser;

/**
 * Describes regular expressions which are choices from
 * from among included regular expressions.
 */

public class RChoice : RegularExpression
{

    /**
     * The list of choices of this regular expression.  Each
     * list component will narrow to RegularExpression.
     */
    private List<Expansion> choices = new();

    /**
     * @param choices the choices to set
     */
    public void SetChoices(List<Expansion> choices)
    {
        this.choices = choices;
    }

    /**
     * @return the choices
     */
    public List<Expansion> GetChoices()
    {
        return choices;
    }

    public override Nfa GenerateNfa(bool ignoreCase)
    {
        CompressCharLists(curRE);

        if (GetChoices().Count == 1)
            return ((RegularExpression)GetChoices()[0]).GenerateNfa(ignoreCase);

        Nfa retVal = new Nfa();
        NfaState startState = retVal.start;
        NfaState finalState = retVal.end;

        for (int i = 0; i < GetChoices().Count; i++)
        {
            Nfa temp;
            RegularExpression curRE = (RegularExpression)GetChoices()[i];

            temp = curRE.GenerateNfa(ignoreCase);

            startState.AddMove(temp.start);
            temp.end.AddMove(finalState);
        }

        return retVal;
    }

    void CompressCharLists(RegularExpression curRE)
    {
        CompressChoices(curRE); // Unroll nested choices
        RegularExpression curRE;
        RCharacterList curCharList = null;

        for (int i = 0; i < GetChoices().Count; i++)
        {
            curRE = (RegularExpression)GetChoices()[i];

            while (curRE is RJustName)
                curRE = ((RJustName)curRE).regexpr;

            if (curRE is RStringLiteral literal &&
                literal.image.Length == 1)
                GetChoices()[i] = curRE = new RCharacterList(
                           literal.image[0]);

            if (curRE is RCharacterList)
            {
                if (((RCharacterList)curRE).negated_list)
                    ((RCharacterList)curRE).RemoveNegation();

                var tmp = ((RCharacterList)curRE).descriptors;

                if (curCharList == null)
                    GetChoices()[i] = curRE = curCharList = new RCharacterList();
                else
                    GetChoices().RemoveAt(i--);

                for (int j = tmp.Count; j-- > 0;)
                    curCharList.descriptors.Add(tmp[j]);
            }

        }
    }

    void CompressChoices(RegularExpression curRE)
    {
        RegularExpression curRE;

        for (int i = 0; i < GetChoices().Count; i++)
        {
            curRE = (RegularExpression)GetChoices()[i];

            while (curRE is RJustName)
            {
                curRE = ((RJustName)curRE).regexpr;
            }

            if (curRE is RChoice)
            {
                GetChoices().RemoveAt(i--);
                for (int j = ((RChoice)curRE).GetChoices().Count; j-- > 0;)
                    GetChoices().Add(((RChoice)curRE).GetChoices()[j]);
            }
        }
    }

    public void CheckUnmatchability()
    {
        RegularExpression curRE;
        int numStrings = 0;

        for (int i = 0; i < GetChoices().Count; i++)
        {
            if (!(curRE = (RegularExpression)GetChoices()[i]).private_rexp &&
                //curRE is RJustName &&
                curRE.ordinal > 0 && curRE.ordinal < ordinal &&
                LexGen.lexStates[curRE.ordinal] == LexGen.lexStates[ordinal])
            {
                if (label != null)
                    JavaCCErrors.Warning(this, "Regular Expression choice : " +
                       curRE.label + " can never be matched as : " + label);
                else
                    JavaCCErrors.Warning(this, "Regular Expression choice : " +
                       curRE.label + " can never be matched as token of kind : " +
                                                                            ordinal);
            }

            if (!curRE.private_rexp && curRE is RStringLiteral)
                numStrings++;
        }
    }

}
