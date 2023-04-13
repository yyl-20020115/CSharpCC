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
using System.Collections.Generic;

namespace org.javacc.parser;


public static class LookaheadWalk
{

    public static bool considerSemanticLA;

    public static List<MatchInfo> sizeLimitedMatches;

    private static void listAppend(List<MatchInfo> vToAppendTo, List<MatchInfo> vToAppend)
    {
        for (int i = 0; i < vToAppend.Count; i++)
        {
            vToAppendTo.Add(vToAppend[i]);
        }
    }

    public static List<MatchInfo> genFirstSet(List<MatchInfo> partialMatches, Expansion exp)
    {
        if (exp is RegularExpression)
        {
            List<MatchInfo> retval = new ();
            for (int i = 0; i < partialMatches.Count; i++)
            {
                MatchInfo m = (MatchInfo)partialMatches[i];
                MatchInfo mnew = new MatchInfo();
                for (int j = 0; j < m.firstFreeLoc; j++)
                {
                    mnew.match[j] = m.match[j];
                }
                mnew.firstFreeLoc = m.firstFreeLoc;
                mnew.match[mnew.firstFreeLoc++] = ((RegularExpression)exp).ordinal;
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
        else if (exp is NonTerminal)
        {
            NormalProduction prod = ((NonTerminal)exp).GetProd();
            if (prod is CodeProduction)
            {
                return new ();
            }
            else
            {
                return genFirstSet(partialMatches, prod.getExpansion());
            }
        }
        else if (exp is Choice)
        {
            List<MatchInfo> retval = new ();
            Choice ch = (Choice)exp;
            for (int i = 0; i < ch.GetChoices().Count; i++)
            {
                List<MatchInfo> v = genFirstSet(partialMatches, (Expansion)ch.GetChoices()[i]);
                listAppend(retval, v);
            }
            return retval;
        }
        else if (exp is Sequence)
        {
            List<MatchInfo> v = partialMatches;
            Sequence seq = (Sequence)exp;
            for (int i = 0; i < seq.units.Count; i++)
            {
                v = genFirstSet(v, (Expansion)seq.units[i]);
                if (v.Count == 0) break;
            }
            return v;
        }
        else if (exp is OneOrMore)
        {
            List<MatchInfo> retval = new ();
            List<MatchInfo> v = partialMatches;
            OneOrMore om = (OneOrMore)exp;
            while (true)
            {
                v = genFirstSet(v, om.expansion);
                if (v.Count == 0) break;
                listAppend(retval, v);
            }
            return retval;
        }
        else if (exp is ZeroOrMore)
        {
            List<MatchInfo> retval = new ();
            listAppend(retval, partialMatches);
            List<MatchInfo> v = partialMatches;
            ZeroOrMore zm = (ZeroOrMore)exp;
            while (true)
            {
                v = genFirstSet(v, zm.expansion);
                if (v.Count == 0) break;
                listAppend(retval, v);
            }
            return retval;
        }
        else if (exp is ZeroOrOne)
        {
            List<MatchInfo> retval = new ();
            listAppend(retval, partialMatches);
            listAppend(retval, genFirstSet(partialMatches, ((ZeroOrOne)exp).expansion));
            return retval;
        }
        else if (exp is TryBlock)
        {
            return genFirstSet(partialMatches, ((TryBlock)exp).exp);
        }
        else if (considerSemanticLA &&
                   exp is Lookahead &&
                   ((Lookahead)exp).GetActionTokens().Count != 0
                  )
        {
            return new ();
        }
        else
        {
            List<MatchInfo> retval = new ();
            listAppend(retval, partialMatches);
            return retval;
        }
    }

    private static void listSplit(List toSplit, List mask, List partInMask, List rest)
    {
    OuterLoop:
        for (int i = 0; i < toSplit.Count; i++)
        {
            for (int j = 0; j < mask.Count; j++)
            {
                if (toSplit[i] == mask[j])
                {
                    partInMask.Add(toSplit[i]);
                    continue OuterLoop;
                }
            }
            rest.Add(toSplit[i]);
        }
    }

    public static List<MatchInfo> GenFollowSet(List<MatchInfo> partialMatches, Expansion exp, long generation)
    {
        if (exp.myGeneration == generation)
        {
            return new ();
        }
        //  Console.WriteLine("*** Parent: " + exp.parent);
        exp.myGeneration = generation;
        if (exp.parent == null)
        {
            List<MatchInfo> retval = new ();
            listAppend(retval, partialMatches);
            return retval;
        }
        else

        if (exp.parent is NormalProduction)
        {
            List parents = ((NormalProduction)exp.parent).getParents();
            List<MatchInfo> retval = new ();
            //    Console.WriteLine("1; gen: " + generation + "; exp: " + exp);
            for (int i = 0; i < parents.Count; i++)
            {
                List<MatchInfo> v = GenFollowSet(partialMatches, (Expansion)parents[i], generation);
                listAppend(retval, v);
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
                v = genFirstSet(v, (Expansion)seq.units[i]);
                if (v.Count == 0) return v;
            }
            List<MatchInfo> v1 = new ();
            List<MatchInfo> v2 = new ();
            listSplit(v, partialMatches, v1, v2);
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
            listAppend(v2, v1);
            return v2;
        }
        else

        if (exp.parent is OneOrMore || exp.parent is ZeroOrMore)
        {
            List<MatchInfo> moreMatches = new ();
            listAppend(moreMatches, partialMatches);
            List<MatchInfo> v = partialMatches;
            while (true)
            {
                v = genFirstSet(v, exp);
                if (v.Count == 0) break;
                listAppend(moreMatches, v);
            }
            List<MatchInfo> v1 = new ();
            List<MatchInfo> v2 = new ();
            listSplit(moreMatches, partialMatches, v1, v2);
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
            listAppend(v2, v1);
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
