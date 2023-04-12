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


public class LookaheadCalc:JavaCCGlobals {

  static MatchInfo overlap(List v1, List v2) {
    MatchInfo m1, m2, m3;
    int size;
    bool diff;
    for (int i = 0; i < v1.Count; i++) {
      m1 = (MatchInfo)v1.get(i);
      for (int j = 0; j < v2.Count; j++) {
        m2 = (MatchInfo)v2.get(j);
        size = m1.firstFreeLoc; m3 = m1;
        if (size > m2.firstFreeLoc) {
          size = m2.firstFreeLoc; m3 = m2;
        }
        if (size == 0) return null;
        // we wish to ignore empty expansions and the JAVACODE stuff here.
        diff = false;
        for (int k = 0; k < size; k++) {
          if (m1.match[k] != m2.match[k]) {
            diff = true;
            break;
          }
        }
        if (!diff) return m3;
      }
    }
    return null;
  }

  static bool javaCodeCheck(List v) {
    for (int i = 0; i < v.Count; i++) {
      if (((MatchInfo)v.get(i)).firstFreeLoc == 0) {
        return true;
      }
    }
    return false;
  }

  static string image(MatchInfo m) {
    string ret = "";
    for (int i = 0; i < m.firstFreeLoc; i++) {
      if (m.match[i] == 0) {
        ret += " <EOF>";
      } else {
        RegularExpression re = (RegularExpression)rexps_of_tokens.get((m.match[i]));
        if (re is RStringLiteral) {
          ret += " \"" + add_escapes(((RStringLiteral)re).image) + "\"";
        } else if (re.label != null && !re.label==("")) {
          ret += " <" + re.label + ">";
        } else {
          ret += " <token of kind " + i + ">";
        }
      }
    }
    if (m.firstFreeLoc == 0) {
      return "";
    } else {
      return ret.substring(1);
    }
  }

  public static void choiceCalc(Choice ch) {
    int first = firstChoice(ch);
    // dbl[i] and dbr[i] are lists of size limited matches for choice i
    // of ch.  dbl ignores matches with semantic lookaheads (when force_la_check
    // is false), while dbr ignores semantic lookahead.
    List[] dbl = new List[ch.getChoices().Count];
    List[] dbr = new List[ch.getChoices().Count];
    int[] minLA = new int[ch.getChoices().Count-1];
    MatchInfo[] overlapInfo = new MatchInfo[ch.getChoices().Count-1];
    int[] other = new int[ch.getChoices().Count-1];
    MatchInfo m;
    List<MatchInfo> v;
    bool overlapDetected;
    for (int la = 1; la <= Options.getChoiceAmbiguityCheck(); la++) {
      MatchInfo.laLimit = la;
      LookaheadWalk.considerSemanticLA = !Options.getForceLaCheck();
      for (int i = first; i < ch.getChoices().Count-1; i++) {
        LookaheadWalk.sizeLimitedMatches = new ArrayList<MatchInfo>();
        m = new MatchInfo();
        m.firstFreeLoc = 0;
        v = new ArrayList<MatchInfo>();
        v.Add(m);
        LookaheadWalk.genFirstSet(v, (Expansion)ch.getChoices().get(i));
        dbl[i] = LookaheadWalk.sizeLimitedMatches;
      }
      LookaheadWalk.considerSemanticLA = false;
      for (int i = first+1; i < ch.getChoices().Count; i++) {
        LookaheadWalk.sizeLimitedMatches = new ArrayList<MatchInfo>();
        m = new MatchInfo();
        m.firstFreeLoc = 0;
        v = new ArrayList<MatchInfo>();
        v.Add(m);
        LookaheadWalk.genFirstSet(v, (Expansion)ch.getChoices().get(i));
        dbr[i] = LookaheadWalk.sizeLimitedMatches;
      }
      if (la == 1) {
        for (int i = first; i < ch.getChoices().Count-1; i++) {
          Expansion exp = (Expansion)ch.getChoices().get(i);
          if (Semanticize.emptyExpansionExists(exp)) {
            JavaCCErrors.warning(exp, "This choice can expand to the empty token sequence " +
                    "and will therefore always be taken in favor of the choices appearing later.");
            break;
          } else if (javaCodeCheck(dbl[i])) {
            JavaCCErrors.warning(exp, "JAVACODE non-terminal will force this choice to be taken " +
                    "in favor of the choices appearing later.");
            break;
          }
        }
      }
      overlapDetected = false;
      for (int i = first; i < ch.getChoices().Count-1; i++) {
        for (int j = i+1; j < ch.getChoices().Count; j++) {
          if ((m = overlap(dbl[i], dbr[j])) != null) {
            minLA[i] = la+1;
            overlapInfo[i] = m;
            other[i] = j;
            overlapDetected = true;
            break;
          }
        }
      }
      if (!overlapDetected) {
        break;
      }
    }
    for (int i = first; i < ch.getChoices().Count-1; i++) {
      if (explicitLA((Expansion)ch.getChoices().get(i)) && !Options.getForceLaCheck()) {
        continue;
      }
      if (minLA[i] > Options.getChoiceAmbiguityCheck()) {
        JavaCCErrors.warning("Choice conflict involving two expansions at");
        Console.Error.Write("         line " + ((Expansion)ch.getChoices().get(i)).getLine());
        Console.Error.Write(", column " + ((Expansion)ch.getChoices().get(i)).getColumn());
        Console.Error.Write(" and line " + ((Expansion)ch.getChoices().get(other[i])).getLine());
        Console.Error.Write(", column " + ((Expansion)ch.getChoices().get(other[i])).getColumn());
        Console.Error.WriteLine(" respectively.");
        Console.Error.WriteLine("         A common prefix is: " + image(overlapInfo[i]));
        Console.Error.WriteLine("         Consider using a lookahead of " + minLA[i] + " or more for earlier expansion.");
      } else if (minLA[i] > 1) {
        JavaCCErrors.warning("Choice conflict involving two expansions at");
        Console.Error.Write("         line " + ((Expansion)ch.getChoices().get(i)).getLine());
        Console.Error.Write(", column " + ((Expansion)ch.getChoices().get(i)).getColumn());
        Console.Error.Write(" and line " + ((Expansion)ch.getChoices().get(other[i])).getLine());
        Console.Error.Write(", column " + ((Expansion)ch.getChoices().get(other[i])).getColumn());
        Console.Error.WriteLine(" respectively.");
        Console.Error.WriteLine("         A common prefix is: " + image(overlapInfo[i]));
        Console.Error.WriteLine("         Consider using a lookahead of " + minLA[i] + " for earlier expansion.");
      }
    }
  }

  static bool explicitLA(Expansion exp) {
    if (!(exp is Sequence)) {
      return false;
    }
    Sequence seq = (Sequence)exp;
    Object obj = seq.units.get(0);
    if (!(obj is Lookahead)) {
      return false;
    }
    Lookahead la = (Lookahead)obj;
    return la.isExplicit();
  }

  static int firstChoice(Choice ch) {
    if (Options.getForceLaCheck()) {
      return 0;
    }
    for (int i = 0; i < ch.getChoices().Count; i++) {
      if (!explicitLA((Expansion)ch.getChoices().get(i))) {
        return i;
      }
    }
    return ch.getChoices().Count;
  }

  private static string image(Expansion exp) {
    if (exp is OneOrMore) {
      return "(...)+";
    } else if (exp is ZeroOrMore) {
      return "(...)*";
    } else /* if (exp is ZeroOrOne) */ {
      return "[...]";
    }
  }

  public static void ebnfCalc(Expansion exp, Expansion nested) {
    // exp is one of OneOrMore, ZeroOrMore, ZeroOrOne
    MatchInfo m, m1 = null;
    List<MatchInfo> v;
    List<MatchInfo> first, follow;
    int la;
    for (la = 1; la <= Options.getOtherAmbiguityCheck(); la++) {
      MatchInfo.laLimit = la;
      LookaheadWalk.sizeLimitedMatches = new ArrayList<MatchInfo>();
      m = new MatchInfo();
      m.firstFreeLoc = 0;
      v = new ArrayList<MatchInfo>();
      v.Add(m);
      LookaheadWalk.considerSemanticLA = !Options.getForceLaCheck();
      LookaheadWalk.genFirstSet(v, nested);
      first = LookaheadWalk.sizeLimitedMatches;
      LookaheadWalk.sizeLimitedMatches = new ArrayList<MatchInfo>();
      LookaheadWalk.considerSemanticLA = false;
      LookaheadWalk.genFollowSet(v, exp, Expansion.nextGenerationIndex++);
      follow = LookaheadWalk.sizeLimitedMatches;
      if (la == 1) {
        if (javaCodeCheck(first)) {
          JavaCCErrors.warning(nested, "JAVACODE non-terminal within " + image(exp) +
                  " construct will force this construct to be entered in favor of " +
                  "expansions occurring after construct.");
        }
      }
      if ((m = overlap(first, follow)) == null) {
        break;
      }
      m1 = m;
    }
    if (la > Options.getOtherAmbiguityCheck()) {
      JavaCCErrors.warning("Choice conflict in " + image(exp) + " construct " +
              "at line " + exp.getLine() + ", column " + exp.getColumn() + ".");
      Console.Error.WriteLine("         Expansion nested within construct and expansion following construct");
      Console.Error.WriteLine("         have common prefixes, one of which is: " + image(m1));
      Console.Error.WriteLine("         Consider using a lookahead of " + la + " or more for nested expansion.");
    } else if (la > 1) {
      JavaCCErrors.warning("Choice conflict in " + image(exp) + " construct " +
              "at line " + exp.getLine() + ", column " + exp.getColumn() + ".");
      Console.Error.WriteLine("         Expansion nested within construct and expansion following construct");
      Console.Error.WriteLine("         have common prefixes, one of which is: " + image(m1));
      Console.Error.WriteLine("         Consider using a lookahead of " + la + " for nested expansion.");
    }
  }

}
