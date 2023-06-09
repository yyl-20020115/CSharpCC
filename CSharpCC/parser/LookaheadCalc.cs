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
using CSharpCC.Utils;

namespace CSharpCC.Parser;


public class LookaheadCalc : CSharpCCGlobals
{

    static MatchInfo Overlap(List<MatchInfo> v1, List<MatchInfo> v2)
    {
        MatchInfo m1, m2, m3;
        int size;
        bool diff;
        for (int i = 0; i < v1.Count; i++)
        {
            m1 = v1[i];
            for (int j = 0; j < v2.Count; j++)
            {
                m2 = v2[j];
                size = m1.firstFreeLoc; m3 = m1;
                if (size > m2.firstFreeLoc)
                {
                    size = m2.firstFreeLoc; m3 = m2;
                }
                if (size == 0) return null;
                // we wish to ignore empty expansions and the JAVACODE stuff here.
                diff = false;
                for (int k = 0; k < size; k++)
                {
                    if (m1.match[k] != m2.match[k])
                    {
                        diff = true;
                        break;
                    }
                }
                if (!diff) return m3;
            }
        }
        return null;
    }

    static bool JavaCodeCheck(List<MatchInfo> v)
    {
        for (int i = 0; i < v.Count; i++)
        {
            if (v[i].firstFreeLoc == 0)
            {
                return true;
            }
        }
        return false;
    }

    static string Image(MatchInfo m)
    {
        string ret = "";
        for (int i = 0; i < m.firstFreeLoc; i++)
        {
            if (m.match[i] == 0)
            {
                ret += " <EOF>";
            }
            else
            {
                if (RegexpsOfTokens.TryGetValue(m.match[i], out var re) 
                    && re is RStringLiteral rs)
                {
                    ret += " \"" +StringEscapeHelpers.AddEscapes(rs.image) + "\"";
                }
                else if (!string.IsNullOrEmpty(re?.label))
                {
                    ret += " <" + re.label + ">";
                }
                else
                {
                    ret += " <token of kind " + i + ">";
                }
            }
        }
        if (m.firstFreeLoc == 0)
        {
            return "";
        }
        else
        {
            return ret[1..];
        }
    }

    public static void ChoiceCalc(Choice ch)
    {
        int first = FirstChoice(ch);
        // dbl[i] and dbr[i] are lists of size limited matches for choice i
        // of ch.  dbl ignores matches with semantic lookaheads (when force_la_check
        // is false), while dbr ignores semantic lookahead.
        List<MatchInfo>[] dbl = new List<MatchInfo>[ch.Choices.Count];
        List<MatchInfo>[] dbr = new List<MatchInfo>[ch.Choices.Count];
        int[] minLA = new int[ch.Choices.Count - 1];
        MatchInfo[] overlapInfo = new MatchInfo[ch.Choices.Count - 1];
        int[] other = new int[ch.Choices.Count - 1];
        MatchInfo m;
        List<MatchInfo> v;
        bool overlapDetected;
        for (int la = 1; la <= Options.GetChoiceAmbiguityCheck(); la++)
        {
            MatchInfo.laLimit = la;
            LookaheadWalk.ConsiderSemanticLA = !Options.GetForceLaCheck();
            for (int i = first; i < ch.Choices.Count - 1; i++)
            {
                LookaheadWalk.SizeLimitedMatches = new();
                m = new MatchInfo();
                m.firstFreeLoc = 0;
                v = new();
                v.Add(m);
                LookaheadWalk.GenFirstSet(v, (Expansion)ch.Choices[i]);
                dbl[i] = LookaheadWalk.SizeLimitedMatches;
            }
            LookaheadWalk.ConsiderSemanticLA = false;
            for (int i = first + 1; i < ch.Choices.Count; i++)
            {
                LookaheadWalk.SizeLimitedMatches = new();
                m = new MatchInfo();
                m.firstFreeLoc = 0;
                v = new();
                v.Add(m);
                LookaheadWalk.GenFirstSet(v, (Expansion)ch.Choices[i]);
                dbr[i] = LookaheadWalk.SizeLimitedMatches;
            }
            if (la == 1)
            {
                for (int i = first; i < ch.Choices.Count - 1; i++)
                {
                    Expansion exp = (Expansion)ch.Choices[i];
                    if (Semanticize.EmptyExpansionExists(exp))
                    {
                        CSharpCCErrors.Warning(exp, "This choice can expand to the empty token sequence " +
                                "and will therefore always be taken in favor of the choices appearing later.");
                        break;
                    }
                    else if (JavaCodeCheck(dbl[i]))
                    {
                        CSharpCCErrors.Warning(exp, "JAVACODE non-terminal will force this choice to be taken " +
                                "in favor of the choices appearing later.");
                        break;
                    }
                }
            }
            overlapDetected = false;
            for (int i = first; i < ch.Choices.Count - 1; i++)
            {
                for (int j = i + 1; j < ch.Choices.Count; j++)
                {
                    if ((m = Overlap(dbl[i], dbr[j])) != null)
                    {
                        minLA[i] = la + 1;
                        overlapInfo[i] = m;
                        other[i] = j;
                        overlapDetected = true;
                        break;
                    }
                }
            }
            if (!overlapDetected)
            {
                break;
            }
        }
        for (int i = first; i < ch.Choices.Count - 1; i++)
        {
            if (ExplicitLA(ch.Choices[i]) && !Options.GetForceLaCheck())
            {
                continue;
            }
            if (minLA[i] > Options.GetChoiceAmbiguityCheck())
            {
                CSharpCCErrors.Warning("Choice conflict involving two expansions at");
                Console.Error.Write("         line " + ((Expansion)ch.Choices[i]).Line);
                Console.Error.Write(", column " + ((Expansion)ch.Choices[i]).Column);
                Console.Error.Write(" and line " + ((Expansion)ch.Choices[(other[i])]).Line);
                Console.Error.Write(", column " + ((Expansion)ch.Choices[(other[i])]).Column);
                Console.Error.WriteLine(" respectively.");
                Console.Error.WriteLine("         A common prefix is: " + Image(overlapInfo[i]));
                Console.Error.WriteLine("         Consider using a lookahead of " + minLA[i] + " or more for earlier expansion.");
            }
            else if (minLA[i] > 1)
            {
                CSharpCCErrors.Warning("Choice conflict involving two expansions at");
                Console.Error.Write("         line " + ((Expansion)ch.Choices[i]).Line);
                Console.Error.Write(", column " + ((Expansion)ch.Choices[i]).Column);
                Console.Error.Write(" and line " + ((Expansion)ch.Choices[(other[i])]).Line);
                Console.Error.Write(", column " + ((Expansion)ch.Choices[(other[i])]).Column);
                Console.Error.WriteLine(" respectively.");
                Console.Error.WriteLine("         A common prefix is: " + Image(overlapInfo[i]));
                Console.Error.WriteLine("         Consider using a lookahead of " + minLA[i] + " for earlier expansion.");
            }
        }
    }

    static bool ExplicitLA(Expansion exp)
    {
        if (exp is not Sequence)
        {
            return false;
        }
        Sequence seq = (Sequence)exp;
        Object obj = seq.units[0];
        if (obj is not Lookahead)
        {
            return false;
        }
        Lookahead la = (Lookahead)obj;
        return la.IsExplicit();
    }

    static int FirstChoice(Choice ch)
    {
        if (!Options.GetForceLaCheck())
        {
            for (int i = 0; i < ch.Choices.Count; i++)
            {
                if (!ExplicitLA(ch.Choices[i]))
                {
                    return i;
                }
            }
            return ch.Choices.Count;
        }
        return 0;
    }

    private static string Image(Expansion exp)
    {
        if (exp is OneOrMore)
        {
            return "(...)+";
        }
        else if (exp is ZeroOrMore)
        {
            return "(...)*";
        }
        else /* if (exp is ZeroOrOne) */
        {
            return "[...]";
        }
    }

    public static void EbnfCalc(Expansion exp, Expansion nested)
    {
        // exp is one of OneOrMore, ZeroOrMore, ZeroOrOne
        MatchInfo m, m1 = null;
        List<MatchInfo> v;
        List<MatchInfo> first, follow;
        int la;
        for (la = 1; la <= Options.GetOtherAmbiguityCheck(); la++)
        {
            MatchInfo.laLimit = la;
            LookaheadWalk.SizeLimitedMatches = new();
            m = new MatchInfo();
            m.firstFreeLoc = 0;
            v = new();
            v.Add(m);
            LookaheadWalk.ConsiderSemanticLA = !Options.GetForceLaCheck();
            LookaheadWalk.GenFirstSet(v, nested);
            first = LookaheadWalk.SizeLimitedMatches;
            LookaheadWalk.SizeLimitedMatches = new();
            LookaheadWalk.ConsiderSemanticLA = false;
            LookaheadWalk.GenFollowSet(v, exp, Expansion.NextGenerationIndex++);
            follow = LookaheadWalk.SizeLimitedMatches;
            if (la == 1)
            {
                if (JavaCodeCheck(first))
                {
                    CSharpCCErrors.Warning(nested, "JAVACODE non-terminal within " + Image(exp) +
                            " construct will force this construct to be entered in favor of " +
                            "expansions occurring after construct.");
                }
            }
            if ((m = Overlap(first, follow)) == null)
            {
                break;
            }
            m1 = m;
        }
        if (la > Options.GetOtherAmbiguityCheck())
        {
            CSharpCCErrors.Warning("Choice conflict in " + Image(exp) + " construct " +
                    "at line " + exp.Line + ", column " + exp.Column + ".");
            Console.Error.WriteLine("         Expansion nested within construct and expansion following construct");
            Console.Error.WriteLine("         have common prefixes, one of which is: " + Image(m1));
            Console.Error.WriteLine("         Consider using a lookahead of " + la + " or more for nested expansion.");
        }
        else if (la > 1)
        {
            CSharpCCErrors.Warning("Choice conflict in " + Image(exp) + " construct " +
                    "at line " + exp.Line + ", column " + exp.Column + ".");
            Console.Error.WriteLine("         Expansion nested within construct and expansion following construct");
            Console.Error.WriteLine("         have common prefixes, one of which is: " + Image(m1));
            Console.Error.WriteLine("         Consider using a lookahead of " + la + " for nested expansion.");
        }
    }

}
