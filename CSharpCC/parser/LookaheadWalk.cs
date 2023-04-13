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


public static class LookaheadWalk
{

    public static bool considerSemanticLA;

    public static List<MatchInfo> sizeLimitedMatches;

    private static void ListAppend(List<MatchInfo> vToAppendTo, List<MatchInfo> vToAppend)
    {
        for (int i = 0; i < vToAppend.Count; i++)
        {
            vToAppendTo.Add(vToAppend[i]);
        }
    }

    public static List<MatchInfo> GenFirstSet(List<MatchInfo> partialMatches, Expansion exp)
    {
        if (exp is RegularExpression expression)
        {
            List<MatchInfo> retval = new();
            for (int i = 0; i < partialMatches.Count; i++)
            {
                MatchInfo m = (MatchInfo)partialMatches[i];
                MatchInfo mnew = new MatchInfo();
                for (int j = 0; j < m.firstFreeLoc; j++)
                {
                    mnew.match[j] = m.match[j];
                }
                mnew.firstFreeLoc = m.firstFreeLoc;
                mnew.match[mnew.firstFreeLoc++] = expression.ordinal;
                if (mnew.firstFreeLoc == MatchInfo.laLimit)
                {
                    sizeLimitedMatches.Add(mnew);
                }
                else
                {
                    retval.Add(mnew);
                }
            }
            return retval;
        }
        else if (exp is NonTerminal terminal)
        {
            NormalProduction prod = terminal.GetProd();
            if (prod is CodeProduction)
            {
                return new();
            }
            else
            {
                return GenFirstSet(partialMatches, prod.GetExpansion());
            }
        }
        else if (exp is Choice choice)
        {
            List<MatchInfo> retval = new();
            Choice ch = choice;
            for (int i = 0; i < ch.GetChoices().Count; i++)
            {
                List<MatchInfo> v = GenFirstSet(partialMatches, (Expansion)ch.GetChoices()[i]);
                ListAppend(retval, v);
            }
            return retval;
        }
        else if (exp is Sequence sequence)
        {
            List<MatchInfo> v = partialMatches;
            Sequence seq = sequence;
            for (int i = 0; i < seq.units.Count; i++)
            {
                v = GenFirstSet(v, (Expansion)seq.units[i]);
                if (v.Count == 0) break;
            }
            return v;
        }
        else if (exp is OneOrMore more)
        {
            List<MatchInfo> retval = new();
            List<MatchInfo> v = partialMatches;
            OneOrMore om = more;
            while (true)
            {
                v = GenFirstSet(v, om.expansion);
                if (v.Count == 0) break;
                ListAppend(retval, v);
            }
            return retval;
        }
        else if (exp is ZeroOrMore more1)
        {
            List<MatchInfo> retval = new();
            ListAppend(retval, partialMatches);
            List<MatchInfo> v = partialMatches;
            ZeroOrMore zm = more1;
            while (true)
            {
                v = GenFirstSet(v, zm.expansion);
                if (v.Count == 0) break;
                ListAppend(retval, v);
            }
            return retval;
        }
        else if (exp is ZeroOrOne one)
        {
            List<MatchInfo> retval = new();
            ListAppend(retval, partialMatches);
            ListAppend(retval, GenFirstSet(partialMatches, one.expansion));
            return retval;
        }
        else if (exp is TryBlock block)
        {
            return GenFirstSet(partialMatches, block.exp);
        }
        else if (considerSemanticLA &&
                   exp is Lookahead lookahead &&
                   lookahead.GetActionTokens().Count != 0
                  )
        {
            return new();
        }
        else
        {
            List<MatchInfo> retval = new();
            ListAppend(retval, partialMatches);
            return retval;
        }
    }

    private static void ListSplit(List<MatchInfo> toSplit, List<MatchInfo> mask, List<MatchInfo> partInMask, List<MatchInfo> rest)
    {
        //OuterLoop:
        for (int i = 0; i < toSplit.Count; i++)
        {
            for (int j = 0; j < mask.Count; j++)
            {
                if (toSplit[i] == mask[j])
                {
                    partInMask.Add(toSplit[i]);
                    //continue OuterLoop;
                    goto InnerExit;
                }
            }
            rest.Add(toSplit[i]);
        InnerExit:
            ;
        }
    }

    public static List<MatchInfo> GenFollowSet(List<MatchInfo> partialMatches, Expansion exp, long generation)
    {
        if (exp.myGeneration == generation)
        {
            return new();
        }
        //  Console.WriteLine("*** Parent: " + exp.parent);
        exp.myGeneration = generation;
        if (exp.parent == null)
        {
            List<MatchInfo> retval = new();
            ListAppend(retval, partialMatches);
            return retval;
        }
        else

        if (exp.parent is NormalProduction np)
        {
            List<Expansion> parents = np.GetParents();
            List<MatchInfo> retval = new();
            //    Console.WriteLine("1; gen: " + generation + "; exp: " + exp);
            for (int i = 0; i < parents.Count; i++)
            {
                List<MatchInfo> v = GenFollowSet(partialMatches, parents[i], generation);
                ListAppend(retval, v);
            }
            return retval;
        }
        else

        if (exp.parent is Sequence)
        {
            Sequence seq = (Sequence)exp.parent;
            List<MatchInfo> v = partialMatches;
            for (int i = exp.ordinal + 1; i < seq.units.Count; i++)
            {
                v = GenFirstSet(v, (Expansion)seq.units[i]);
                if (v.Count == 0) return v;
            }
            List<MatchInfo> v1 = new();
            List<MatchInfo> v2 = new();
            ListSplit(v, partialMatches, v1, v2);
            if (v1.Count != 0)
            {
                //Console.WriteLine("2; gen: " + generation + "; exp: " + exp);
                v1 = GenFollowSet(v1, seq, generation);
            }
            if (v2.Count != 0)
            {
                //Console.WriteLine("3; gen: " + generation + "; exp: " + exp);
                v2 = GenFollowSet(v2, seq, Expansion.nextGenerationIndex++);
            }
            ListAppend(v2, v1);
            return v2;
        }
        else

        if (exp.parent is OneOrMore || exp.parent is ZeroOrMore)
        {
            List<MatchInfo> moreMatches = new();
            ListAppend(moreMatches, partialMatches);
            List<MatchInfo> v = partialMatches;
            while (true)
            {
                v = GenFirstSet(v, exp);
                if (v.Count == 0) break;
                ListAppend(moreMatches, v);
            }
            List<MatchInfo> v1 = new();
            List<MatchInfo> v2 = new();
            ListSplit(moreMatches, partialMatches, v1, v2);
            if (v1.Count != 0)
            {
                //		Console.WriteLine("4; gen: " + generation + "; exp: " + exp);
                v1 = GenFollowSet(v1, (Expansion)exp.parent, generation);
            }
            if (v2.Count != 0)
            {
                //		Console.WriteLine("5; gen: " + generation + "; exp: " + exp);
                v2 = GenFollowSet(v2, (Expansion)exp.parent, Expansion.nextGenerationIndex++);
            }
            ListAppend(v2, v1);
            return v2;
        }
        else
        {
            //		Console.WriteLine("6; gen: " + generation + "; exp: " + exp);
            return GenFollowSet(partialMatches, (Expansion)exp.parent, generation);
        }
    }

    public static void reInit()
    {
        considerSemanticLA = false;
        sizeLimitedMatches = null;
    }

}
