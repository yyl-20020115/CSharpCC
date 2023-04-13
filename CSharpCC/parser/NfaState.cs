// Copyright 2011 Google Inc. All Rights Reserved.
// Author: sreeni@google.com (Sreeni Viswanadha)

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
using org.javacc.jjtree;

namespace org.javacc.parser;


/**
 * The state of a Non-deterministic Finite Automaton.
 */
public class NfaState
{
    public static bool unicodeWarningGiven = false;
    public static int generatedStates = 0;

    private static int idCnt = 0;
    private static int lohiByteCnt;
    private static int dummyStateIndex = -1;
    private static bool done;
    private static bool[] mark;
    private static bool[] stateDone;

    private static List<NfaState> allStates = new();
    private static List<NfaState> indexedAllStates = new();
    private static List<NfaState> nonAsciiTableForMethod = new();
    private static Dictionary<string,NfaState> equivStatesTable = new();
    private static Dictionary allNextStates = new();
    private static Dictionary lohiByteTab = new();
    private static Dictionary stateNameForComposite = new();
    private static Dictionary compositeStateTable = new();
    private static Dictionary stateBlockTable = new();
    private static Dictionary stateSetsToFix = new();

    private static bool jjCheckNAddStatesUnaryNeeded = false;
    private static bool jjCheckNAddStatesDualNeeded = false;

    public static void ReInit()
    {
        generatedStates = 0;
        idCnt = 0;
        dummyStateIndex = -1;
        done = false;
        mark = null;
        stateDone = null;

        allStates.Clear();
        indexedAllStates.Clear();
        equivStatesTable.Clear();
        allNextStates.Clear();
        compositeStateTable.Clear();
        stateBlockTable.Clear();
        stateNameForComposite.Clear();
        stateSetsToFix.Clear();
    }

    public ulong[] asciiMoves = new ulong[2];
    public char[] charMoves = null;
    public char[] rangeMoves = null;
    public NfaState next = null;
    private NfaState stateForCase;
    public List<NfaState> epsilonMoves = new();
    private string epsilonMovesString;
    private NfaState[] epsilonMoveArray;

    private readonly int id;
    public int stateName = -1;
    public int kind = int.MaxValue;
    private int lookingFor;
    private int usefulEpsilonMoves = 0;
    public int inNextOf;
    private int lexState;
    private int nonAsciiMethod = -1;
    private int kindToPrint = int.MaxValue;
    public bool dummy = false;
    private bool isComposite = false;
    private int[] compositeStates = null;
    public bool isFinal = false;
    private List<byte> loByteVec;
    private int[] nonAsciiMoveIndices;
    private int round = 0;
    private int onlyChar = 0;
    private char matchSingleChar;

    public NfaState()
    {
        id = idCnt++;
        allStates.Add(this);
        lexState = LexGen.lexStateIndex;
        lookingFor = LexGen.curKind;
    }

    NfaState CreateClone()
    {
        NfaState retVal = new NfaState();

        retVal.isFinal = isFinal;
        retVal.kind = kind;
        retVal.lookingFor = lookingFor;
        retVal.lexState = lexState;
        retVal.inNextOf = inNextOf;

        retVal.MergeMoves(this);

        return retVal;
    }

    static void InsertInOrder(List<NfaState> v, NfaState s)
    {
        int j;

        for (j = 0; j < v.Count; j++)
            if (((NfaState)v[j]).id > s.id)
                break;
            else if (((NfaState)v[j]).id == s.id)
                return;

        v.Insert(j, s);
    }

    private static char[] ExpandCharArr(char[] oldArr, int incr)
    {
        char[] ret = new char[oldArr.Length + incr];
        Array.Copy(oldArr, 0, ret, 0, oldArr.Length);
        return ret;
    }

    public void AddMove(NfaState newState)
    {
        if (!epsilonMoves.Contains(newState))
            InsertInOrder(epsilonMoves, newState);
    } 

    private void AddASCIIMove(char c)
    {
        asciiMoves[c / 64] |= (1UL << (c % 64));
    }

    public void AddChar(char c)
    {
        onlyChar++;
        matchSingleChar = c;
        int i;
        char temp;
        char temp1;

        if ((int)c < 128) // ASCII char
        {
            AddASCIIMove(c);
            return;
        }

        charMoves ??= new char[10];

        int len = charMoves.Length;

        if (charMoves[len - 1] != 0)
        {
            charMoves = ExpandCharArr(charMoves, 10);
            len += 10;
        }

        for (i = 0; i < len; i++)
            if (charMoves[i] == 0 || charMoves[i] > c)
                break;

        if (!unicodeWarningGiven && c > 0xff &&
            !Options.GetJavaUnicodeEscape() &&
            !Options.GetUserCharStream())
        {
            unicodeWarningGiven = true;
            JavaCCErrors.Warning(LexGen.curRE, "Non-ASCII characters used in regular expression.\n" +
                 "Please make sure you use the correct Reader when you create the parser, " +
                 "one that can handle your character set.");
        }

        temp = charMoves[i];
        charMoves[i] = c;

        for (i++; i < len; i++)
        {
            if (temp == 0)
                break;

            temp1 = charMoves[i];
            charMoves[i] = temp;
            temp = temp1;
        }
    }

    public void AddRange(char left, char right)
    {
        onlyChar = 2;
        int i;
        char tempLeft1, tempLeft2, tempRight1, tempRight2;

        if (left < 128)
        {
            if (right < 128)
            {
                for (; left <= right; left++)
                    AddASCIIMove(left);

                return;
            }

            for (; left < 128; left++)
                AddASCIIMove(left);
        }

        if (!unicodeWarningGiven && (left > 0xff || right > 0xff) &&
            !Options.GetJavaUnicodeEscape() &&
            !Options.GetUserCharStream())
        {
            unicodeWarningGiven = true;
            JavaCCErrors.Warning(LexGen.curRE, "Non-ASCII characters used in regular expression.\n" +
                 "Please make sure you use the correct Reader when you create the parser, " +
                 "one that can handle your character set.");
        }

        rangeMoves ??= new char[20];

        int len = rangeMoves.Length;

        if (rangeMoves[len - 1] != 0)
        {
            rangeMoves = ExpandCharArr(rangeMoves, 20);
            len += 20;
        }

        for (i = 0; i < len; i += 2)
            if (rangeMoves[i] == 0 ||
                (rangeMoves[i] > left) ||
                ((rangeMoves[i] == left) && (rangeMoves[i + 1] > right)))
                break;

        tempLeft1 = rangeMoves[i];
        tempRight1 = rangeMoves[i + 1];
        rangeMoves[i] = left;
        rangeMoves[i + 1] = right;

        for (i += 2; i < len; i += 2)
        {
            if (tempLeft1 == 0)
                break;

            tempLeft2 = rangeMoves[i];
            tempRight2 = rangeMoves[i + 1];
            rangeMoves[i] = tempLeft1;
            rangeMoves[i + 1] = tempRight1;
            tempLeft1 = tempLeft2;
            tempRight1 = tempRight2;
        }
    }

    // From hereon down all the functions are used for code generation

    private static bool EqualCharArr(char[] arr1, char[] arr2)
    {
        if (arr1 == arr2)
            return true;

        if (arr1 != null &&
            arr2 != null &&
            arr1.Length == arr2.Length)
        {
            for (int i = arr1.Length; i-- > 0;)
                if (arr1[i] != arr2[i])
                    return false;

            return true;
        }

        return false;
    }

    private bool closureDone = false;

    /** This function computes the closure and also updates the kind so that
      * any time there is a move to this state, it can go on epsilon to a
      * new state in the epsilon moves that might have a lower kind of token
      * number for the same length.
    */

    private void EpsilonClosure()
    {
        int i = 0;

        if (closureDone || mark[id])
            return;

        mark[id] = true;

        // Recursively do closure
        for (i = 0; i < epsilonMoves.Count; i++)
            epsilonMoves[i].EpsilonClosure();

        
        foreach(NfaState nfas1 in epsilonMoves)
        {
            for (i = 0; i < nfas1.epsilonMoves.Count; i++)
            {
                var nfas2 = nfas1.epsilonMoves[i];
                if (nfas2.UsefulState() && !epsilonMoves.Contains(nfas2))
                {
                    InsertInOrder(epsilonMoves, nfas2);
                    done = false;
                }
            }

            if (kind > nfas1.kind)
                kind = nfas1.kind;
        }

        if (HasTransitions() && !epsilonMoves.Contains(this))
            InsertInOrder(epsilonMoves, this);
    }

    private bool UsefulState()
    {
        return isFinal || HasTransitions();
    }

    public bool HasTransitions()
    {
        return (asciiMoves[0] != 0L || asciiMoves[1] != 0L ||
                (charMoves != null && charMoves[0] != 0) ||
                (rangeMoves != null && rangeMoves[0] != 0));
    }

    void MergeMoves(NfaState other)
    {
        // Warning : This function does not merge epsilon moves
        if (asciiMoves == other.asciiMoves)
        {
            JavaCCErrors.SemanticError("Bug in JavaCC : Please send " +
                      "a report along with the input that caused this. Thank you.");
            throw new Error();
        }

        asciiMoves[0] = asciiMoves[0] | other.asciiMoves[0];
        asciiMoves[1] = asciiMoves[1] | other.asciiMoves[1];

        if (other.charMoves != null)
        {
            if (charMoves == null)
                charMoves = other.charMoves;
            else
            {
                char[] tmpCharMoves = new char[charMoves.Length +
                                                  other.charMoves.Length];
                Array.Copy(charMoves, 0, tmpCharMoves, 0, charMoves.Length);
                charMoves = tmpCharMoves;

                for (int i = 0; i < other.charMoves.Length; i++)
                    AddChar(other.charMoves[i]);
            }
        }

        if (other.rangeMoves != null)
        {
            if (rangeMoves == null)
                rangeMoves = other.rangeMoves;
            else
            {
                char[] tmpRangeMoves = new char[rangeMoves.Length +
                                                         other.rangeMoves.Length];
                Array.Copy(rangeMoves, 0, tmpRangeMoves,
                                                            0, rangeMoves.Length);
                rangeMoves = tmpRangeMoves;
                for (int i = 0; i < other.rangeMoves.Length; i += 2)
                    AddRange(other.rangeMoves[i], other.rangeMoves[i + 1]);
            }
        }

        if (other.kind < kind)
            kind = other.kind;

        if (other.kindToPrint < kindToPrint)
            kindToPrint = other.kindToPrint;

        isFinal |= other.isFinal;
    }

    static NfaState CreateEquivState(List<NfaState> states)
    {
        NfaState newState = ((NfaState)states[0]).CreateClone();

        newState.next = new NfaState();

        InsertInOrder(newState.next.epsilonMoves,
                             ((NfaState)states[0]).next);

        for (int i = 1; i < states.Count; i++)
        {
            NfaState tmp2 = ((NfaState)states[i]);

            if (tmp2.kind < newState.kind)
                newState.kind = tmp2.kind;

            newState.isFinal |= tmp2.isFinal;

            InsertInOrder(newState.next.epsilonMoves, tmp2.next);
        }

        return newState;
    }

    private NfaState GetEquivalentRunTimeState()
    {
    Outer:
        for (int i = allStates.Count; i-- > 0;)
        {
            NfaState other = (NfaState)allStates[i];

            if (this != other && other.stateName != -1 &&
                kindToPrint == other.kindToPrint &&
                asciiMoves[0] == other.asciiMoves[0] &&
                asciiMoves[1] == other.asciiMoves[1] &&
                EqualCharArr(charMoves, other.charMoves) &&
                EqualCharArr(rangeMoves, other.rangeMoves))
            {
                if (next == other.next)
                    return other;
                else if (next != null && other.next != null)
                {
                    if (next.epsilonMoves.Count == other.next.epsilonMoves.Count)
                    {
                        for (int j = 0; j < next.epsilonMoves.Count; j++)
                            if (next.epsilonMoves[j] !=
                                  other.next.epsilonMoves[j])
                                goto OuterExit;

                        return other;
                    }
                }
            }
        OuterExit:
            ;
        }

        return null;
    }

    // generates code (without outputting it) and returns the name used.
    public void GenerateCode()
    {
        if (stateName != -1)
            return;

        if (next != null)
        {
            next.GenerateCode();
            if (next.kind != int.MaxValue)
                kindToPrint = next.kind;
        }

        if (stateName == -1 && HasTransitions())
        {
            NfaState tmp = GetEquivalentRunTimeState();

            if (tmp != null)
            {
                stateName = tmp.stateName;
                //????
                //tmp.inNextOf += inNextOf;
                //????
                dummy = true;
                return;
            }

            stateName = generatedStates++;
            indexedAllStates.Add(this);
            GenerateNextStatesCode();
        }
    }

    public static void ComputeClosures()
    {
        for (int i = allStates.Count; i-- > 0;)
        {
            NfaState tmp = allStates[i];

            if (!tmp.closureDone)
                tmp.OptimizeEpsilonMoves(true);
        }

        for (int i = 0; i < allStates.Count; i++)
        {
            NfaState tmp = allStates[i];

            if (!tmp.closureDone)
                tmp.OptimizeEpsilonMoves(false);
        }

        for (int i = 0; i < allStates.Count; i++)
        {
            NfaState tmp = (NfaState)allStates[i];
            tmp.epsilonMoveArray = new NfaState[tmp.epsilonMoves.Count];
            tmp.epsilonMoves.CopyTo(tmp.epsilonMoveArray);
        }
    }

    void OptimizeEpsilonMoves(bool optReqd)
    {
        int i;

        // First do epsilon closure
        done = false;
        while (!done)
        {
            if (mark == null || mark.Length < allStates.Count)
                mark = new bool[allStates.Count];

            for (i = allStates.Count; i-- > 0;)
                mark[i] = false;

            done = true;
            EpsilonClosure();
        }

        for (i = allStates.Count; i-- > 0;)
            allStates[i].closureDone =
                                     mark[allStates[i].id];

        // Warning : The following piece of code is just an optimization.
        // in case of trouble, just remove this piece.

        bool sometingOptimized = true;

        NfaState newState = null;
        NfaState tmp1, tmp2;
        int j;
        List<NfaState> equivStates = null;

        while (sometingOptimized)
        {
            sometingOptimized = false;
            for (i = 0; optReqd && i < epsilonMoves.Count; i++)
            {
                if ((tmp1 = (NfaState)epsilonMoves[i]).HasTransitions())
                {
                    for (j = i + 1; j < epsilonMoves.Count; j++)
                    {
                        if ((tmp2 = (NfaState)epsilonMoves[j]).
                                                                 HasTransitions() &&
                            (tmp1.asciiMoves[0] == tmp2.asciiMoves[0] &&
                             tmp1.asciiMoves[1] == tmp2.asciiMoves[1] &&
                             EqualCharArr(tmp1.charMoves, tmp2.charMoves) &&
                             EqualCharArr(tmp1.rangeMoves, tmp2.rangeMoves)))
                        {
                            if (equivStates == null)
                            {
                                equivStates = new ();
                                equivStates.Add(tmp1);
                            }

                            InsertInOrder(equivStates, tmp2);
                            epsilonMoves.RemoveAt(j--);
                        }
                    }
                }

                if (equivStates != null)
                {
                    sometingOptimized = true;
                    string tmp = "";
                    for (int l = 0; l < equivStates.Count; l++)
                        tmp += equivStates[l].id + ", ";

                    if ((newState = (NfaState)equivStatesTable.get(tmp)) == null)
                    {
                        newState = CreateEquivState(equivStates);
                        equivStatesTable.Add(tmp, newState);
                    }

                    epsilonMoves.RemoveAt(i--);
                    epsilonMoves.Add(newState);
                    equivStates = null;
                    newState = null;
                }
            }

            for (i = 0; i < epsilonMoves.Count; i++)
            {
                //if ((tmp1 = (NfaState)epsilonMoves.elementAt(i)).next == null)
                //continue;
                tmp1 = (NfaState)epsilonMoves[i];

                for (j = i + 1; j < epsilonMoves.Count; j++)
                {
                    tmp2 = (NfaState)epsilonMoves[j];

                    if (tmp1.next == tmp2.next)
                    {
                        if (newState == null)
                        {
                            newState = tmp1.CreateClone();
                            newState.next = tmp1.next;
                            sometingOptimized = true;
                        }

                        newState.MergeMoves(tmp2);
                        epsilonMoves.removeElementAt(j--);
                    }
                }

                if (newState != null)
                {
                    epsilonMoves.removeElementAt(i--);
                    epsilonMoves.Add(newState);
                    newState = null;
                }
            }
        }

        // End Warning

        // Generate an array of states for epsilon moves (not vector)
        if (epsilonMoves.Count > 0)
        {
            for (i = 0; i < epsilonMoves.Count; i++)
                // Since we are doing a closure, just epsilon moves are unnecessary
                if (((NfaState)epsilonMoves[i]).HasTransitions())
                    usefulEpsilonMoves++;
                else
                    epsilonMoves.RemoveAt(i--);
        }
    }

    void GenerateNextStatesCode()
    {
        if (next.usefulEpsilonMoves > 0)
            next.GetEpsilonMovesString();
    }

    string GetEpsilonMovesString()
    {
        int[] stateNames = new int[usefulEpsilonMoves];
        int cnt = 0;

        if (epsilonMovesString != null)
            return epsilonMovesString;

        if (usefulEpsilonMoves > 0)
        {
            NfaState tempState;
            epsilonMovesString = "{ ";
            for (int i = 0; i < epsilonMoves.Count; i++)
            {
                if ((tempState = (NfaState)epsilonMoves[i]).
                                                         HasTransitions())
                {
                    if (tempState.stateName == -1)
                        tempState.GenerateCode();

                    ((NfaState)indexedAllStates.get(tempState.stateName)).inNextOf++;
                    stateNames[cnt] = tempState.stateName;
                    epsilonMovesString += tempState.stateName + ", ";
                    if (cnt++ > 0 && cnt % 16 == 0)
                        epsilonMovesString += "\n";
                }
            }

            epsilonMovesString += "};";
        }

        usefulEpsilonMoves = cnt;
        if (epsilonMovesString != null &&
            allNextStates.get(epsilonMovesString) == null)
        {
            int[] statesToPut = new int[usefulEpsilonMoves];

            Array.Copy(stateNames, 0, statesToPut, 0, cnt);
            allNextStates.Add(epsilonMovesString, statesToPut);
        }

        return epsilonMovesString;
    }

    public static bool CanStartNfaUsingAscii(char c)
    {
        if (c >= 128)
            throw new Error("JavaCC Bug: Please send mail to sankar@cs.stanford.edu");

        string s = LexGen.initialState.GetEpsilonMovesString();

        if (s == null || s == ("null;"))
            return false;

        int[] states = (int[])allNextStates.get(s);

        for (int i = 0; i < states.Length; i++)
        {
            NfaState tmp = (NfaState)indexedAllStates.get(states[i]);

            if ((tmp.asciiMoves[c / 64] & (1L << c % 64)) != 0L)
                return true;
        }

        return false;
    }

    bool CanMoveUsingChar(char c)
    {
        int i;

        if (onlyChar == 1)
            return c == matchSingleChar;

        if (c < 128)
            return ((asciiMoves[c / 64] & (1UL << c % 64)) != 0L);

        // Just check directly if there is a move for this char
        if (charMoves != null && charMoves[0] != 0)
        {
            for (i = 0; i < charMoves.Length; i++)
            {
                if (c == charMoves[i])
                    return true;
                else if (c < charMoves[i] || charMoves[i] == 0)
                    break;
            }
        }


        // For ranges, iterate through the table to see if the current char
        // is in some range
        if (rangeMoves != null && rangeMoves[0] != 0)
            for (i = 0; i < rangeMoves.Length; i += 2)
                if (c >= rangeMoves[i] && c <= rangeMoves[i + 1])
                    return true;
                else if (c < rangeMoves[i] || rangeMoves[i] == 0)
                    break;

        //return (nextForNegatedList != null);
        return false;
    }

    public int getFirstValidPos(string s, int i, int len)
    {
        if (onlyChar == 1)
        {
            char c = matchSingleChar;
            while (c != s[i] && ++i < len) ;
            return i;
        }

        do
        {
            if (CanMoveUsingChar(s[i]))
                return i;
        } while (++i < len);

        return i;
    }

    public int MoveFrom(char c, List<NfaState> newStates)
    {
        if (CanMoveUsingChar(c))
        {
            for (int i = next.epsilonMoves.Count; i-- > 0;)
                InsertInOrder(newStates, next.epsilonMoves[i]);

            return kindToPrint;
        }

        return int.MaxValue;
    }

    public static int MoveFromSet(char c, List<NfaState> states, List<NfaState> newStates)
    {
        int tmp;
        int retVal = int.MaxValue;

        for (int i = states.Count; i-- > 0;)
            if (retVal >
                (tmp = ((NfaState)states[i]).MoveFrom(c, newStates)))
                retVal = tmp;

        return retVal;
    }

    public static int moveFromSetForRegEx(char c, NfaState[] states, NfaState[] newStates, int round)
    {
        int start = 0;
        int sz = states.Length;

        for (int i = 0; i < sz; i++)
        {
            NfaState tmp1, tmp2;

            if ((tmp1 = states[i]) == null)
                break;

            if (tmp1.CanMoveUsingChar(c))
            {
                if (tmp1.kindToPrint != int.MaxValue)
                {
                    newStates[start] = null;
                    return 1;
                }

                NfaState[] v = tmp1.next.epsilonMoveArray;
                for (int j = v.Length; j-- > 0;)
                {
                    if ((tmp2 = v[j]).round != round)
                    {
                        tmp2.round = round;
                        newStates[start++] = tmp2;
                    }
                }
            }
        }

        newStates[start] = null;
        return int.MaxValue;
    }

    static List allBitVectors = new ();

    /* This function generates the bit vectors of low and hi bytes for common
       bit vectors and returns those that are not common with anything (in
       loBytes) and returns an array of indices that can be used to generate
       the function names for char matching using the common bit vectors.
       It also generates code to match a char with the common bit vectors.
       (Need a better comment). */

    static int[] tmpIndices = new int[512]; // 2 * 256
    void GenerateNonAsciiMoves(CodeGenerator codeGenerator)
    {
        int i = 0, j = 0;
        char hiByte;
        int cnt = 0;
        ulong[][] loBytes = new long[256][4];

        if ((charMoves == null || charMoves[0] == 0) &&
            (rangeMoves == null || rangeMoves[0] == 0))
            return;

        if (charMoves != null)
        {
            for (i = 0; i < charMoves.Length; i++)
            {
                if (charMoves[i] == 0)
                    break;

                hiByte = (char)(charMoves[i] >> 8);
                loBytes[hiByte][(charMoves[i] & 0xff) / 64] |=
                                  (1UL << ((charMoves[i] & 0xff) % 64));
            }
        }

        if (rangeMoves != null)
        {
            for (i = 0; i < rangeMoves.Length; i += 2)
            {
                if (rangeMoves[i] == 0)
                    break;

                char c, r;

                r = (char)(rangeMoves[i + 1] & 0xff);
                hiByte = (char)(rangeMoves[i] >> 8);

                if (hiByte == (char)(rangeMoves[i + 1] >> 8))
                {
                    for (c = (char)(rangeMoves[i] & 0xff); c <= r; c++)
                        loBytes[hiByte][c / 64] |= (1UL << (c % 64));

                    continue;
                }

                for (c = (char)(rangeMoves[i] & 0xff); c <= 0xff; c++)
                    loBytes[hiByte][c / 64] |= (1UL << (c % 64));

                while (++hiByte < (char)(rangeMoves[i + 1] >> 8))
                {
                    loBytes[hiByte][0] |= 0xffffffffffffffffL;
                    loBytes[hiByte][1] |= 0xffffffffffffffffL;
                    loBytes[hiByte][2] |= 0xffffffffffffffffL;
                    loBytes[hiByte][3] |= 0xffffffffffffffffL;
                }

                for (c = 0; c <= r; c++)
                    loBytes[hiByte][c / 64] |= (1UL << (c % 64));
            }
        }

        long[] common = null;
        bool[] done = new bool[256];

        for (i = 0; i <= 255; i++)
        {
            if (done[i] ||
                (done[i] =
                 loBytes[i][0] == 0 &&
                 loBytes[i][1] == 0 &&
                 loBytes[i][2] == 0 &&
                 loBytes[i][3] == 0))
                continue;

            for (j = i + 1; j < 256; j++)
            {
                if (done[j])
                    continue;

                if (loBytes[i][0] == loBytes[j][0] &&
                    loBytes[i][1] == loBytes[j][1] &&
                    loBytes[i][2] == loBytes[j][2] &&
                    loBytes[i][3] == loBytes[j][3])
                {
                    done[j] = true;
                    if (common == null)
                    {
                        done[i] = true;
                        common = new long[4];
                        common[i / 64] |= (1L << (i % 64));
                    }

                    common[j / 64] |= (1L << (j % 64));
                }
            }

            if (common != null)
            {
                int ind;
                string tmp; 

                tmp = "{\n   0x" + Convert.ToString(common[0],16) + "L, " +
                        "0x" + Convert.ToString(common[1], 16) + "L, " +
                        "0x" + Convert.ToString(common[2], 16) + "L, " +
                        "0x" + Convert.ToString(common[3], 16) + "L\n};";
                if ((ind = (int)lohiByteTab.get(tmp)) == null)
                {
                    allBitVectors.Add(tmp);

                    if (!AllBitsSet(tmp))
                    {
                        if (CodeGenerator.IsJavaLanguage())
                        {
                            codeGenerator.GenCodeLine("static final " + Options.GetLongType() + "[] jjbitVec" + lohiByteCnt + " = " + tmp);
                        }
                        else
                        {
                            codeGenerator.SwitchToStaticsFile();
                            codeGenerator.GenCodeLine("static const " + Options.GetLongType() + " jjbitVec" + lohiByteCnt + "[] = " + tmp);
                        }
                    }
                    lohiByteTab.Add(tmp, ind = (lohiByteCnt++));
                }

                tmpIndices[cnt++] = ind;

                tmp = "{\n   0x" + Long.toHexString(loBytes[i][0]) + "L, " +
                        "0x" + Long.toHexString(loBytes[i][1]) + "L, " +
                        "0x" + Long.toHexString(loBytes[i][2]) + "L, " +
                        "0x" + Long.toHexString(loBytes[i][3]) + "L\n};";
                if ((ind = (int)lohiByteTab.get(tmp)) == null)
                {
                    allBitVectors.Add(tmp);

                    if (!AllBitsSet(tmp))
                        if (CodeGenerator.IsJavaLanguage())
                        {
                            codeGenerator.GenCodeLine("static final " + Options.GetLongType() + "[] jjbitVec" + lohiByteCnt + " = " + tmp);
                        }
                        else
                        {
                            codeGenerator.SwitchToStaticsFile();
                            codeGenerator.GenCodeLine("static const " + Options.GetLongType() + " jjbitVec" + lohiByteCnt + "[] = " + tmp);
                            codeGenerator.SwitchToMainFile();
                        }
                    lohiByteTab.Add(tmp, ind = (lohiByteCnt++));
                }

                tmpIndices[cnt++] = ind;

                common = null;
            }
        }

        nonAsciiMoveIndices = new int[cnt];
        Array.Copy(tmpIndices, 0, nonAsciiMoveIndices, 0, cnt);

        /*
              Console.println("state : " + stateName + " cnt : " + cnt);
              while (cnt > 0)
              {
                 Console.Write(nonAsciiMoveIndices[cnt - 1] + ", " + nonAsciiMoveIndices[cnt - 2] + ", ");
                 cnt -= 2;
              }
              Console.println("");
        */

        for (i = 0; i < 256; i++)
        {
            if (done[i])
                loBytes[i] = null;
            else
            {
                //Console.Write(i + ", ");
                string tmp;
                int ind;

                tmp = "{\n   0x" + Long.toHexString(loBytes[i][0]) + "L, " +
                        "0x" + Long.toHexString(loBytes[i][1]) + "L, " +
                        "0x" + Long.toHexString(loBytes[i][2]) + "L, " +
                        "0x" + Long.toHexString(loBytes[i][3]) + "L\n};";
                if ((ind = (int)lohiByteTab.get(tmp)) == null)
                {
                    allBitVectors.Add(tmp);


                    if (!AllBitsSet(tmp))
                        if (CodeGenerator.IsJavaLanguage())
                        {
                            codeGenerator.GenCodeLine("static final " + Options.GetLongType() + "[] jjbitVec" + lohiByteCnt + " = " + tmp);
                        }
                        else
                        {
                            codeGenerator.SwitchToStaticsFile();
                            codeGenerator.GenCodeLine("static const " + Options.GetLongType() + " jjbitVec" + lohiByteCnt + "[] = " + tmp);
                        }
                    lohiByteTab.Add(tmp, ind = (lohiByteCnt++));
                }

                loByteVec ??= new Vector();

                loByteVec.Add((i));
                loByteVec.Add(ind);
            }
        }
        //Console.println("");
        UpdateDuplicateNonAsciiMoves();
    }

    private void UpdateDuplicateNonAsciiMoves()
    {
        for (int i = 0; i < nonAsciiTableForMethod.Count; i++)
        {
            NfaState tmp = (NfaState)nonAsciiTableForMethod[i];
            if (EqualLoByteVectors(loByteVec, tmp.loByteVec) &&
                EqualNonAsciiMoveIndices(nonAsciiMoveIndices, tmp.nonAsciiMoveIndices))
            {
                nonAsciiMethod = i;
                return;
            }
        }

        nonAsciiMethod = nonAsciiTableForMethod.Count;
        nonAsciiTableForMethod.Add(this);
    }

    private static bool EqualLoByteVectors(List vec1, List vec2)
    {
        if (vec1 == null || vec2 == null)
            return false;

        if (vec1 == vec2)
            return true;

        if (vec1.Count != vec2.Count)
            return false;

        for (int i = 0; i < vec1.Count; i++)
        {
            if (((int)vec1[i]).intValue() !=
                ((int)vec2[i]).intValue())
                return false;
        }

        return true;
    }

    private static bool EqualNonAsciiMoveIndices(int[] moves1, int[] moves2)
    {
        if (moves1 == moves2)
            return true;

        if (moves1 == null || moves2 == null)
            return false;

        if (moves1.Length != moves2.Length)
            return false;

        for (int i = 0; i < moves1.Length; i++)
        {
            if (moves1[i] != moves2[i])
                return false;
        }

        return true;
    }

    static string allBits = "{\n   0xffffffffffffffffL, " +
                     "0xffffffffffffffffL, " +
                     "0xffffffffffffffffL, " +
                     "0xffffffffffffffffL\n};";

    static bool AllBitsSet(string bitVec)
    {
        return bitVec == (allBits);
    }

    public static int AddStartStateSet(string stateSetString)
    {
        return AddCompositeStateSet(stateSetString, true);
    }

    private static int AddCompositeStateSet(string stateSetString, bool starts)
    {
        int stateNameToReturn;

        if ((stateNameToReturn = (int)stateNameForComposite.get(stateSetString)) != null)
            return stateNameToReturn.intValue();

        int toRet = 0;
        int[] nameSet = (int[])allNextStates.get(stateSetString);

        if (!starts)
            stateBlockTable.Add(stateSetString, stateSetString);

        if (nameSet == null)
            throw new Error("JavaCC Bug: Please file a bug at: https://github.com/javacc/javacc/issues");

        if (nameSet.Length == 1)
        {
            stateNameToReturn = (nameSet[0]);
            stateNameForComposite.Add(stateSetString, stateNameToReturn);
            return nameSet[0];
        }

        for (int i = 0; i < nameSet.Length; i++)
        {
            if (nameSet[i] == -1)
                continue;

            NfaState st = (NfaState)indexedAllStates.get(nameSet[i]);
            st.isComposite = true;
            st.compositeStates = nameSet;
        }

        while (toRet < nameSet.Length &&
               (starts && ((NfaState)indexedAllStates.get(nameSet[toRet])).inNextOf > 1))
            toRet++;

        Enumeration<string> e = compositeStateTable.keys();
        string s;
        while (e.hasMoreElements())
        {
            s = (String)e.nextElement();
            if (s != (stateSetString) && Intersect(stateSetString, s))
            {
                int[] other = (int[])compositeStateTable.get(s);

                while (toRet < nameSet.Length &&
                       ((starts && ((NfaState)indexedAllStates.get(nameSet[toRet])).inNextOf > 1) ||
                        ElemOccurs(nameSet[toRet], other) >= 0))
                    toRet++;
            }
        }

        int tmp;

        if (toRet >= nameSet.Length)
        {
            if (dummyStateIndex == -1)
                tmp = dummyStateIndex = generatedStates;
            else
                tmp = ++dummyStateIndex;

            // TODO(sreeni) : Fix this
            if (Options.GetTokenManagerCodeGenerator() != null)
            {
                NfaState dummyState = new NfaState();
                dummyState.isComposite = true;
                dummyState.compositeStates = nameSet;
                dummyState.stateName = tmp;
            }
        }
        else
            tmp = nameSet[toRet];

        stateNameToReturn = (tmp);
        stateNameForComposite.Add(stateSetString, stateNameToReturn);
        compositeStateTable.Add(stateSetString, nameSet);

        return tmp;
    }

    private static int StateNameForComposite(string stateSetString)
    {
        return ((int)stateNameForComposite.get(stateSetString)).intValue();
    }

    public static int InitStateName()
    {
        string s = LexGen.initialState.GetEpsilonMovesString();

        if (LexGen.initialState.usefulEpsilonMoves != 0)
            return StateNameForComposite(s);
        return -1;
    }

    public int GenerateInitMoves(CodeGenerator codeGenerator)
    {
        GetEpsilonMovesString();

        epsilonMovesString ??= "null;";

        return AddStartStateSet(epsilonMovesString);
    }

    static Dictionary tableToDump = new Dictionary();
    static List orderedStateSet = new ();

    static int lastIndex = 0;
    private static int[] GetStateSetIndicesForUse(string arrayString)
    {
        int[] ret;
        int[] set = (int[])allNextStates.get(arrayString);

        if ((ret = (int[])tableToDump.get(arrayString)) == null)
        {
            ret = new int[2];
            ret[0] = lastIndex;
            ret[1] = lastIndex + set.Length - 1;  
            lastIndex += set.Length;
            tableToDump.Add(arrayString, ret);
            orderedStateSet.Add(set);
        }

        return ret;
    }

    public static void DumpStateSets(CodeGenerator codeGenerator)
    {
        int cnt = 0;

        if (CodeGenerator.IsJavaLanguage())
        {
            codeGenerator.GenCode("static final int[] jjnextStates = {");
        }
        else
        {
            codeGenerator.SwitchToStaticsFile();
            codeGenerator.GenCode("static const int jjnextStates[] = {");
        }
        if (orderedStateSet.Count > 0)
            for (int i = 0; i < orderedStateSet.Count; i++)
            {
                int[] set = (int[])orderedStateSet[i];

                for (int j = 0; j < set.Length; j++)
                {
                    if (cnt++ % 16 == 0)
                        codeGenerator.GenCode("\n   ");

                    codeGenerator.GenCode(set[j] + ", ");
                }
            }
        else
            codeGenerator.GenCode("0");

        codeGenerator.GenCodeLine("\n};");
        if (!CodeGenerator.IsJavaLanguage())
        {
            codeGenerator.SwitchToMainFile();
        }
    }

    public static string GetStateSetString(int[] states)
    {
        string retVal = "{ ";
        for (int i = 0; i < states.Length;)
        {
            retVal += states[i] + ", ";

            if (i++ > 0 && i % 16 == 0)
                retVal += "\n";
        }

        retVal += "};";
        allNextStates.Add(retVal, states);
        return retVal;
    }

    public static string GetStateSetString(List<NfaState> states)
    {
        if (states == null || states.Count == 0)
            return "null;";

        int[] set = new int[states.Count];
        string retVal = "{ ";
        for (int i = 0; i < states.Count;)
        {
            int k;
            retVal += (k = ((NfaState)states[i]).stateName) + ", ";
            set[i] = k;

            if (i++ > 0 && i % 16 == 0)
                retVal += "\n";
        }

        retVal += "};";
        allNextStates.Add(retVal, set);
        return retVal;
    }

    static int NumberOfBitsSet(long l)
    {
        int ret = 0;
        for (int i = 0; i < 63; i++)
            if (((l >> i) & 1L) != 0L)
                ret++;

        return ret;
    }

    static int OnlyOneBitSet(long l)
    {
        int oneSeen = -1;
        for (int i = 0; i < 64; i++)
            if (((l >> i) & 1L) != 0L)
            {
                if (oneSeen >= 0)
                    return -1;
                oneSeen = i;
            }

        return oneSeen;
    }

    private static int ElemOccurs(int elem, int[] arr)
    {
        for (int i = arr.Length; i-- > 0;)
            if (arr[i] == elem)
                return i;

        return -1;
    }

    private bool FindCommonBlocks()
    {
        if (next == null || next.usefulEpsilonMoves <= 1)
            return false;

        stateDone ??= new bool[generatedStates];

        string set = next.epsilonMovesString;

        int[] nameSet = (int[])allNextStates.get(set);

        if (nameSet.Length <= 2 || compositeStateTable.get(set) != null)
            return false;

        int i;
        int[] freq = new int[nameSet.Length];
        bool[] live = new bool[nameSet.Length];
        int[] count = new int[allNextStates.Count];

        for (i = 0; i < nameSet.Length; i++)
        {
            if (nameSet[i] != -1)
            {
                live[i] = !stateDone[nameSet[i]];
                if (live[i])
                    count[0]++;
            }
        }

        int j, blockLen = 0, commonFreq = 0;
        Enumeration e = allNextStates.keys();
        bool needUpdate;

        while (e.hasMoreElements())
        {
            int[] tmpSet = (int[])allNextStates.get(e.nextElement());
            if (tmpSet == nameSet)
                continue;

            needUpdate = false;
            for (j = 0; j < nameSet.Length; j++)
            {
                if (nameSet[j] == -1)
                    continue;

                if (live[j] && ElemOccurs(nameSet[j], tmpSet) >= 0)
                {
                    if (!needUpdate)
                    {
                        needUpdate = true;
                        commonFreq++;
                    }

                    count[freq[j]]--;
                    count[commonFreq]++;
                    freq[j] = commonFreq;
                }
            }

            if (needUpdate)
            {
                int foundFreq = -1;
                blockLen = 0;

                for (j = 0; j <= commonFreq; j++)
                    if (count[j] > blockLen)
                    {
                        foundFreq = j;
                        blockLen = count[j];
                    }

                if (blockLen <= 1)
                    return false;

                for (j = 0; j < nameSet.Length; j++)
                    if (nameSet[j] != -1 && freq[j] != foundFreq)
                    {
                        live[j] = false;
                        count[freq[j]]--;
                    }
            }
        }

        if (blockLen <= 1)
            return false;

        int[] commonBlock = new int[blockLen];
        int cnt = 0;
        //Console.println("Common Block for " + set + " :");
        for (i = 0; i < nameSet.Length; i++)
        {
            if (live[i])
            {
                if (((NfaState)indexedAllStates.get(nameSet[i])).isComposite)
                    return false;

                stateDone[nameSet[i]] = true;
                commonBlock[cnt++] = nameSet[i];
                //Console.Write(nameSet[i] + ", ");
            }
        }

        //Console.println("");

        string s = GetStateSetString(commonBlock);
        e = allNextStates.keys();

    Outer:
        while (e.hasMoreElements())
        {
            int at;
            bool firstOne = true;
            string stringToFix;
            int[] setToFix = (int[])allNextStates.get(stringToFix = (String)e.nextElement());

            if (setToFix == commonBlock)
                continue;

            for (int k = 0; k < cnt; k++)
            {
                if ((at = ElemOccurs(commonBlock[k], setToFix)) >= 0)
                {
                    if (!firstOne)
                        setToFix[at] = -1;
                    firstOne = false;
                }
                else
                    continue Outer;
            }

            if (stateSetsToFix.get(stringToFix) == null)
                stateSetsToFix.Add(stringToFix, setToFix);
        }

        next.usefulEpsilonMoves -= blockLen - 1;
        AddCompositeStateSet(s, false);
        return true;
    }

    private bool CheckNextOccursTogether()
    {
        if (next == null || next.usefulEpsilonMoves <= 1)
            return true;

        string set = next.epsilonMovesString;

        int[] nameSet = (int[])allNextStates.get(set);

        if (nameSet.Length == 1 || compositeStateTable.get(set) != null ||
            stateSetsToFix.get(set) != null)
            return false;

        int i;
        Dictionary occursIn = new Dictionary();
        NfaState tmp = (NfaState)allStates.get(nameSet[0]);

        for (i = 1; i < nameSet.Length; i++)
        {
            NfaState tmp1 = (NfaState)allStates.get(nameSet[i]);

            if (tmp.inNextOf != tmp1.inNextOf)
                return false;
        }

        int isPresent, j;
        Enumeration e = allNextStates.keys();
        while (e.hasMoreElements())
        {
            string s;
            int[] tmpSet = (int[])allNextStates.get(s = (String)e.nextElement());

            if (tmpSet == nameSet)
                continue;

            isPresent = 0;
            for (j = 0; j < nameSet.Length; j++)
            {
                if (ElemOccurs(nameSet[j], tmpSet) >= 0)
                    isPresent++;
                else if (isPresent > 0)
                    return false;
            }

            if (isPresent == j)
            {
                if (tmpSet.Length > nameSet.Length)
                    occursIn.Add(s, tmpSet);

                //May not need. But safe.
                if (compositeStateTable.get(s) != null ||
                    stateSetsToFix.get(s) != null)
                    return false;
            }
            else if (isPresent != 0)
                return false;
        }

        e = occursIn.keys();
        while (e.hasMoreElements())
        {
            string s;
            int[] setToFix = (int[])occursIn.get(s = (String)e.nextElement());

            if (stateSetsToFix.get(s) == null)
                stateSetsToFix.Add(s, setToFix);

            for (int k = 0; k < setToFix.Length; k++)
                if (ElemOccurs(setToFix[k], nameSet) > 0)  // Not >= since need the first one (0)
                    setToFix[k] = -1;
        }

        next.usefulEpsilonMoves = 1;
        AddCompositeStateSet(next.epsilonMovesString, false);
        return true;
    }

    private static void FixStateSets()
    {
        Dictionary fixedSets = new Dictionary();
        Enumeration e = stateSetsToFix.keys();
        int[] tmp = new int[generatedStates];
        int i;

        while (e.hasMoreElements())
        {
            string s;
            int[] toFix = (int[])stateSetsToFix.get(s = (String)e.nextElement());
            int cnt = 0;

            //Console.Write("Fixing : ");
            for (i = 0; i < toFix.Length; i++)
            {
                //Console.Write(toFix[i] + ", ");
                if (toFix[i] != -1)
                    tmp[cnt++] = toFix[i];
            }

            int[] _fixed = new int[cnt];
            Array.Copy(tmp, 0, _fixed, 0, cnt);
            fixedSets.Add(s, _fixed);
            allNextStates.Add(s, _fixed);
            //Console.println(" as " + GetStateSetString(fixed));
        }

        for (i = 0; i < allStates.Count; i++)
        {
            NfaState tmpState = (NfaState)allStates[i];
            int[] newSet;

            if (tmpState.next == null || tmpState.next.usefulEpsilonMoves == 0)
                continue;

            /*if (compositeStateTable.get(tmpState.next.epsilonMovesString) != null)
               tmpState.next.usefulEpsilonMoves = 1;
            else*/
            if ((newSet = (int[])fixedSets.get(tmpState.next.epsilonMovesString)) != null)
                tmpState.FixNextStates(newSet);
        }
    }

    private void FixNextStates(int[] newSet)
    {
        next.usefulEpsilonMoves = newSet.Length;
        //next.epsilonMovesString = GetStateSetString(newSet);
    }

    private static bool Intersect(string set1, string set2)
    {
        if (set1 == null || set2 == null)
            return false;

        int[] nameSet1 = (int[])allNextStates.get(set1);
        int[] nameSet2 = (int[])allNextStates.get(set2);

        if (nameSet1 == null || nameSet2 == null)
            return false;

        if (nameSet1 == nameSet2)
            return true;

        for (int i = nameSet1.Length; i-- > 0;)
            for (int j = nameSet2.Length; j-- > 0;)
                if (nameSet1[i] == nameSet2[j])
                    return true;

        return false;
    }

    private static void DumpHeadForCase(CodeGenerator codeGenerator, int byteNum)
    {
        if (byteNum == 0)
        {
            codeGenerator.GenCodeLine("         " + Options.GetLongType() + " l = 1L << curChar;");
            if (!CodeGenerator.IsJavaLanguage())
            {
                codeGenerator.GenCodeLine("         (void)l;");
            }
        }
        else if (byteNum == 1)
        {
            codeGenerator.GenCodeLine("         " + Options.GetLongType() + " l = 1L << (curChar & 077);");
            if (!CodeGenerator.IsJavaLanguage())
            {
                codeGenerator.GenCodeLine("         (void)l;");
            }
        }
        else
        {
            if (Options.GetJavaUnicodeEscape() || unicodeWarningGiven)
            {
                codeGenerator.GenCodeLine("         int hiByte = (curChar >> 8);");
                codeGenerator.GenCodeLine("         int i1 = hiByte >> 6;");
                codeGenerator.GenCodeLine("         " + Options.GetLongType() + " l1 = 1L << (hiByte & 077);");
            }

            codeGenerator.GenCodeLine("         int i2 = (curChar & 0xff) >> 6;");
            codeGenerator.GenCodeLine("         " + Options.GetLongType() + " l2 = 1L << (curChar & 077);");
        }

        //codeGenerator.GenCodeLine("         MatchLoop: do");
        codeGenerator.GenCodeLine("         do");
        codeGenerator.GenCodeLine("         {");

        codeGenerator.GenCodeLine("            switch(jjstateSet[--i])");
        codeGenerator.GenCodeLine("            {");
    }

    private static Vector PartitionStatesSetForAscii(int[] states, int byteNum)
    {
        int[] cardinalities = new int[states.Length];
        Vector original = new Vector();
        Vector partition = new Vector();
        NfaState tmp;

        original.setSize(states.Length);
        int cnt = 0;
        for (int i = 0; i < states.Length; i++)
        {
            tmp = (NfaState)allStates.get(states[i]);

            if (tmp.asciiMoves[byteNum] != 0L)
            {
                int j;
                int p = NumberOfBitsSet(tmp.asciiMoves[byteNum]);

                for (j = 0; j < i; j++)
                    if (cardinalities[j] <= p)
                        break;

                for (int k = i; k > j; k--)
                    cardinalities[k] = cardinalities[k - 1];

                cardinalities[j] = p;

                original.insertElementAt(tmp, j);
                cnt++;
            }
        }

        original.setSize(cnt);

        while (original.Count > 0)
        {
            tmp = (NfaState)original[0];
            original.removeElement(tmp);

            long bitVec = tmp.asciiMoves[byteNum];
            List<NfaState> subSet = new();
            subSet.Add(tmp);

            for (int j = 0; j < original.Count; j++)
            {
                NfaState tmp1 = (NfaState)original[j];

                if ((tmp1.asciiMoves[byteNum] & bitVec) == 0L)
                {
                    bitVec |= tmp1.asciiMoves[byteNum];
                    subSet.Add(tmp1);
                    original.removeElementAt(j--);
                }
            }

            partition.Add(subSet);
        }

        return partition;
    }

    private string PrintNoBreak(CodeGenerator codeGenerator, int byteNum, bool[] dumped)
    {
        if (inNextOf != 1)
            throw new Error("JavaCC Bug: Please send mail to sankar@cs.stanford.edu");

        dumped[stateName] = true;

        if (byteNum >= 0)
        {
            if (asciiMoves[byteNum] != 0L)
            {
                codeGenerator.GenCodeLine("               case " + stateName + ":");
                DumpAsciiMoveForCompositeState(codeGenerator, byteNum, false);
                return "";
            }
        }
        else if (nonAsciiMethod != -1)
        {
            codeGenerator.GenCodeLine("               case " + stateName + ":");
            DumpNonAsciiMoveForCompositeState(codeGenerator);
            return "";
        }

        return ("               case " + stateName + ":\n");
    }

    private static void DumpCompositeStatesAsciiMoves(CodeGenerator codeGenerator,
                                 string key, int byteNum, bool[] dumped)
    {
        int i;

        int[] nameSet = (int[])allNextStates.get(key);

        if (nameSet.Length == 1 || dumped[StateNameForComposite(key)])
            return;

        NfaState toBePrinted = null;
        int neededStates = 0;
        NfaState tmp;
        NfaState stateForCase = null;
        string toPrint = "";
        bool stateBlock = (stateBlockTable.get(key) != null);

        for (i = 0; i < nameSet.Length; i++)
        {
            tmp = (NfaState)allStates.get(nameSet[i]);

            if (tmp.asciiMoves[byteNum] != 0L)
            {
                if (neededStates++ == 1)
                    break;
                else
                    toBePrinted = tmp;
            }
            else
                dumped[tmp.stateName] = true;

            if (tmp.stateForCase != null)
            {
                if (stateForCase != null)
                    throw new Error("JavaCC Bug: Please send mail to sankar@cs.stanford.edu : ");

                stateForCase = tmp.stateForCase;
            }
        }

        if (stateForCase != null)
            toPrint = stateForCase.PrintNoBreak(codeGenerator, byteNum, dumped);

        if (neededStates == 0)
        {
            if (stateForCase != null && toPrint == (""))
                codeGenerator.GenCodeLine("                  break;");
            return;
        }

        if (neededStates == 1)
        {
            //if (byteNum == 1)
            //Console.println(toBePrinted.stateName + " is the only state for "
            //+ key + " ; and key is : " + StateNameForComposite(key));

            if (!toPrint == (""))
                codeGenerator.GenCode(toPrint);

            codeGenerator.GenCodeLine("               case " + StateNameForComposite(key) + ":");

            if (!dumped[toBePrinted.stateName] && !stateBlock && toBePrinted.inNextOf > 1)
                codeGenerator.GenCodeLine("               case " + toBePrinted.stateName + ":");

            dumped[toBePrinted.stateName] = true;
            toBePrinted.DumpAsciiMove(codeGenerator, byteNum, dumped);
            return;
        }

        List partition = PartitionStatesSetForAscii(nameSet, byteNum);

        if (!toPrint == (""))
            codeGenerator.GenCode(toPrint);

        int keyState = StateNameForComposite(key);
        codeGenerator.GenCodeLine("               case " + keyState + ":");
        if (keyState < generatedStates)
            dumped[keyState] = true;

        for (i = 0; i < partition.Count; i++)
        {
            List subSet = (List)partition[i];

            for (int j = 0; j < subSet.Count; j++)
            {
                tmp = (NfaState)subSet[j];

                if (stateBlock)
                    dumped[tmp.stateName] = true;
                tmp.DumpAsciiMoveForCompositeState(codeGenerator, byteNum, j != 0);
            }
        }

        if (stateBlock)
            codeGenerator.GenCodeLine("                  break;");
        else
            codeGenerator.GenCodeLine("                  break;");
    }

    private bool selfLoop()
    {
        if (next == null || next.epsilonMovesString == null)
            return false;

        int[] set = (int[])allNextStates.get(next.epsilonMovesString);
        return ElemOccurs(stateName, set) >= 0;
    }

    private void DumpAsciiMoveForCompositeState(CodeGenerator codeGenerator, int byteNum, bool elseNeeded)
    {
        bool nextIntersects = selfLoop();

        for (int j = 0; j < allStates.Count; j++)
        {
            NfaState temp1 = (NfaState)allStates[j];

            if (this == temp1 || temp1.stateName == -1 || temp1.dummy ||
                stateName == temp1.stateName || temp1.asciiMoves[byteNum] == 0L)
                continue;

            if (!nextIntersects && Intersect(temp1.next.epsilonMovesString,
                                            next.epsilonMovesString))
            {
                nextIntersects = true;
                break;
            }
        }

        //Console.println(stateName + " \'s nextIntersects : " + nextIntersects);
        string prefix = "";
        if (asciiMoves[byteNum] != 0xffffffffffffffffL)
        {
            int oneBit = OnlyOneBitSet(asciiMoves[byteNum]);

            if (oneBit != -1)
                codeGenerator.GenCodeLine("                  " + (elseNeeded ? "else " : "") + "if (curChar == " +
                        (64 * byteNum + oneBit) + ")");
            else
                codeGenerator.GenCodeLine("                  " + (elseNeeded ? "else " : "") +
                        "if ((0x" + Long.toHexString(asciiMoves[byteNum]) + "L & l) != 0L)");
            prefix = "   ";
        }

        if (kindToPrint != int.MaxValue)
        {
            if (asciiMoves[byteNum] != 0xffffffffffffffffL)
            {
                codeGenerator.GenCodeLine("                  {");
            }

            codeGenerator.GenCodeLine(prefix + "                  if (kind > " + kindToPrint + ")");
            codeGenerator.GenCodeLine(prefix + "                     kind = " + kindToPrint + ";");
        }

        if (next != null && next.usefulEpsilonMoves > 0)
        {
            int[] stateNames = (int[])allNextStates.get(
                                             next.epsilonMovesString);
            if (next.usefulEpsilonMoves == 1)
            {
                int name = stateNames[0];

                if (nextIntersects)
                    codeGenerator.GenCodeLine(prefix + "                  { jjCheckNAdd(" + name + "); }");
                else
                    codeGenerator.GenCodeLine(prefix + "                  jjstateSet[jjnewStateCnt++] = " + name + ";");
            }
            else if (next.usefulEpsilonMoves == 2 && nextIntersects)
            {
                codeGenerator.GenCodeLine(prefix + "                  { jjCheckNAddTwoStates(" +
                   stateNames[0] + ", " + stateNames[1] + "); }");
            }
            else
            {
                int[] indices = GetStateSetIndicesForUse(next.epsilonMovesString);
                bool notTwo = (indices[0] + 1 != indices[1]);

                if (nextIntersects)
                {
                    codeGenerator.GenCode(prefix + "                  { jjCheckNAddStates(" + indices[0]);
                    if (notTwo)
                    {
                        jjCheckNAddStatesDualNeeded = true;
                        codeGenerator.GenCode(", " + indices[1]);
                    }
                    else
                    {
                        jjCheckNAddStatesUnaryNeeded = true;
                    }
                    codeGenerator.GenCodeLine("); }");
                }
                else
                    codeGenerator.GenCodeLine(prefix + "                  { jjAddStates(" +
                                          indices[0] + ", " + indices[1] + "); }");
            }
        }

        if (asciiMoves[byteNum] != 0xffffffffffffffffL && kindToPrint != int.MaxValue)
            codeGenerator.GenCodeLine("                  }");
    }

    private void DumpAsciiMove(CodeGenerator codeGenerator, int byteNum, bool[] dumped)
    {
        bool nextIntersects = selfLoop() && isComposite;
        bool onlyState = true;

        for (int j = 0; j < allStates.Count; j++)
        {
            NfaState temp1 = (NfaState)allStates[j];

            if (this == temp1 || temp1.stateName == -1 || temp1.dummy ||
                stateName == temp1.stateName || temp1.asciiMoves[byteNum] == 0L)
                continue;

            if (onlyState && (asciiMoves[byteNum] & temp1.asciiMoves[byteNum]) != 0L)
                onlyState = false;

            if (!nextIntersects && Intersect(temp1.next.epsilonMovesString,
                                            next.epsilonMovesString))
                nextIntersects = true;

            if (!dumped[temp1.stateName] && !temp1.isComposite &&
                asciiMoves[byteNum] == temp1.asciiMoves[byteNum] &&
                kindToPrint == temp1.kindToPrint &&
                (next.epsilonMovesString == temp1.next.epsilonMovesString ||
                 (next.epsilonMovesString != null &&
                  temp1.next.epsilonMovesString != null &&
                  next.epsilonMovesString == (
                               temp1.next.epsilonMovesString))))
            {
                dumped[temp1.stateName] = true;
                codeGenerator.GenCodeLine("               case " + temp1.stateName + ":");
            }
        }

        //if (onlyState)
        //nextIntersects = false;

        int oneBit = OnlyOneBitSet(asciiMoves[byteNum]);
        if (asciiMoves[byteNum] != 0xffffffffffffffffL)
        {
            if ((next == null || next.usefulEpsilonMoves == 0) &&
                kindToPrint != int.MaxValue)
            {
                string kindCheck = "";

                if (!onlyState)
                    kindCheck = " && kind > " + kindToPrint;

                if (oneBit != -1)
                    codeGenerator.GenCodeLine("                  if (curChar == " +
                       (64 * byteNum + oneBit) + kindCheck + ")");
                else
                    codeGenerator.GenCodeLine("                  if ((0x" +
                        Long.toHexString(asciiMoves[byteNum]) +
                        "L & l) != 0L" + kindCheck + ")");

                codeGenerator.GenCodeLine("                     kind = " + kindToPrint + ";");

                if (onlyState)
                    codeGenerator.GenCodeLine("                  break;");
                else
                    codeGenerator.GenCodeLine("                  break;");

                return;
            }
        }

        string prefix = "";
        if (kindToPrint != int.MaxValue)
        {

            if (oneBit != -1)
            {
                codeGenerator.GenCodeLine("                  if (curChar != " +
                        (64 * byteNum + oneBit) + ")");
                codeGenerator.GenCodeLine("                     break;");
            }
            else if (asciiMoves[byteNum] != 0xffffffffffffffffL)
            {
                codeGenerator.GenCodeLine("                  if ((0x" + Long.toHexString(asciiMoves[byteNum]) + "L & l) == 0L)");
                codeGenerator.GenCodeLine("                     break;");
            }

            if (onlyState)
            {
                codeGenerator.GenCodeLine("                  kind = " + kindToPrint + ";");
            }
            else
            {
                codeGenerator.GenCodeLine("                  if (kind > " + kindToPrint + ")");
                codeGenerator.GenCodeLine("                     kind = " + kindToPrint + ";");
            }
        }
        else
        {
            if (oneBit != -1)
            {
                codeGenerator.GenCodeLine("                  if (curChar == " +
                        (64 * byteNum + oneBit) + ")");
                prefix = "   ";
            }
            else if (asciiMoves[byteNum] != 0xffffffffffffffffL)
            {
                codeGenerator.GenCodeLine("                  if ((0x" + Long.toHexString(asciiMoves[byteNum]) + "L & l) != 0L)");
                prefix = "   ";
            }
        }

        if (next != null && next.usefulEpsilonMoves > 0)
        {
            int[] stateNames = (int[])allNextStates.get(
                                             next.epsilonMovesString);
            if (next.usefulEpsilonMoves == 1)
            {
                int name = stateNames[0];
                if (nextIntersects)
                    codeGenerator.GenCodeLine(prefix + "                  { jjCheckNAdd(" + name + "); }");
                else
                    codeGenerator.GenCodeLine(prefix + "                  jjstateSet[jjnewStateCnt++] = " + name + ";");
            }
            else if (next.usefulEpsilonMoves == 2 && nextIntersects)
            {
                codeGenerator.GenCodeLine(prefix + "                  { jjCheckNAddTwoStates(" +
                   stateNames[0] + ", " + stateNames[1] + "); }");
            }
            else
            {
                int[] indices = GetStateSetIndicesForUse(next.epsilonMovesString);
                bool notTwo = (indices[0] + 1 != indices[1]);

                if (nextIntersects)
                {
                    codeGenerator.GenCode(prefix + "                  { jjCheckNAddStates(" + indices[0]);
                    if (notTwo)
                    {
                        jjCheckNAddStatesDualNeeded = true;
                        codeGenerator.GenCode(", " + indices[1]);
                    }
                    else
                    {
                        jjCheckNAddStatesUnaryNeeded = true;
                    }
                    codeGenerator.GenCodeLine("); }");
                }
                else
                    codeGenerator.GenCodeLine(prefix + "                  { jjAddStates(" +
                                          indices[0] + ", " + indices[1] + "); }");
            }
        }

        if (onlyState)
            codeGenerator.GenCodeLine("                  break;");
        else
            codeGenerator.GenCodeLine("                  break;");
    }

    private static void DumpAsciiMoves(CodeGenerator codeGenerator, int byteNum)
    {
        bool[] dumped = new bool[Math.Max(generatedStates, dummyStateIndex + 1)];
        Enumeration e = compositeStateTable.keys();

        DumpHeadForCase(codeGenerator, byteNum);

        while (e.hasMoreElements())
            DumpCompositeStatesAsciiMoves(codeGenerator, (String)e.nextElement(), byteNum, dumped);

        for (int i = 0; i < allStates.Count; i++)
        {
            NfaState temp = (NfaState)allStates[i];

            if (dumped[temp.stateName] || temp.lexState != LexGen.lexStateIndex ||
                !temp.HasTransitions() || temp.dummy ||
                temp.stateName == -1)
                continue;

            string toPrint = "";

            if (temp.stateForCase != null)
            {
                if (temp.inNextOf == 1)
                    continue;

                if (dumped[temp.stateForCase.stateName])
                    continue;

                toPrint = (temp.stateForCase.PrintNoBreak(codeGenerator, byteNum, dumped));

                if (temp.asciiMoves[byteNum] == 0L)
                {
                    if (toPrint == (""))
                        codeGenerator.GenCodeLine("                  break;");

                    continue;
                }
            }

            if (temp.asciiMoves[byteNum] == 0L)
                continue;

            if (!toPrint == (""))
                codeGenerator.GenCode(toPrint);

            dumped[temp.stateName] = true;
            codeGenerator.GenCodeLine("               case " + temp.stateName + ":");
            temp.DumpAsciiMove(codeGenerator, byteNum, dumped);
        }

        if (byteNum != 0 && byteNum != 1)
        {
            codeGenerator.GenCodeLine("               default : if (i1 == 0 || l1 == 0 || i2 == 0 ||  l2 == 0) break; else break;");
        }
        else
        {
            codeGenerator.GenCodeLine("               default : break;");
        }

        codeGenerator.GenCodeLine("            }");
        codeGenerator.GenCodeLine("         } while(i != startsAt);");
    }

    private static void DumpCompositeStatesNonAsciiMoves(CodeGenerator codeGenerator,
                                       string key, bool[] dumped)
    {
        int i;
        int[] nameSet = (int[])allNextStates.get(key);

        if (nameSet.Length == 1 || dumped[StateNameForComposite(key)])
            return;

        NfaState toBePrinted = null;
        int neededStates = 0;
        NfaState tmp;
        NfaState stateForCase = null;
        string toPrint = "";
        bool stateBlock = (stateBlockTable.get(key) != null);

        for (i = 0; i < nameSet.Length; i++)
        {
            tmp = (NfaState)allStates.get(nameSet[i]);

            if (tmp.nonAsciiMethod != -1)
            {
                if (neededStates++ == 1)
                    break;
                else
                    toBePrinted = tmp;
            }
            else
                dumped[tmp.stateName] = true;

            if (tmp.stateForCase != null)
            {
                if (stateForCase != null)
                    throw new Error("JavaCC Bug: Please send mail to sankar@cs.stanford.edu : ");

                stateForCase = tmp.stateForCase;
            }
        }

        if (stateForCase != null)
            toPrint = stateForCase.PrintNoBreak(codeGenerator, -1, dumped);

        if (neededStates == 0)
        {
            if (stateForCase != null && toPrint == (""))
                codeGenerator.GenCodeLine("                  break;");

            return;
        }

        if (neededStates == 1)
        {
            if (!toPrint == (""))
                codeGenerator.GenCode(toPrint);

            codeGenerator.GenCodeLine("               case " + StateNameForComposite(key) + ":");

            if (!dumped[toBePrinted.stateName] && !stateBlock && toBePrinted.inNextOf > 1)
                codeGenerator.GenCodeLine("               case " + toBePrinted.stateName + ":");

            dumped[toBePrinted.stateName] = true;
            toBePrinted.DumpNonAsciiMove(codeGenerator, dumped);
            return;
        }

        if (!toPrint == (""))
            codeGenerator.GenCode(toPrint);

        int keyState = StateNameForComposite(key);
        codeGenerator.GenCodeLine("               case " + keyState + ":");
        if (keyState < generatedStates)
            dumped[keyState] = true;

        for (i = 0; i < nameSet.Length; i++)
        {
            tmp = (NfaState)allStates.get(nameSet[i]);

            if (tmp.nonAsciiMethod != -1)
            {
                if (stateBlock)
                    dumped[tmp.stateName] = true;
                tmp.DumpNonAsciiMoveForCompositeState(codeGenerator);
            }
        }

        if (stateBlock)
            codeGenerator.GenCodeLine("                  break;");
        else
            codeGenerator.GenCodeLine("                  break;");
    }

    private void DumpNonAsciiMoveForCompositeState(CodeGenerator codeGenerator)
    {
        bool nextIntersects = selfLoop();
        for (int j = 0; j < allStates.Count; j++)
        {
            NfaState temp1 = (NfaState)allStates[j];

            if (this == temp1 || temp1.stateName == -1 || temp1.dummy ||
                stateName == temp1.stateName || (temp1.nonAsciiMethod == -1))
                continue;

            if (!nextIntersects && Intersect(temp1.next.epsilonMovesString,
                                            next.epsilonMovesString))
            {
                nextIntersects = true;
                break;
            }
        }

        if (!Options.GetJavaUnicodeEscape() && !unicodeWarningGiven)
        {
            if (loByteVec != null && loByteVec.Count > 1)
                codeGenerator.GenCodeLine("                  if ((jjbitVec" +
                 ((int)loByteVec.get(1)).intValue() + "[i2" +
                    "] & l2) != 0L)");
        }
        else
        {
            codeGenerator.GenCodeLine("                  if (jjCanMove_" + nonAsciiMethod +
                                                   "(hiByte, i1, i2, l1, l2))");
        }

        if (kindToPrint != int.MaxValue)
        {
            codeGenerator.GenCodeLine("                  {");
            codeGenerator.GenCodeLine("                     if (kind > " + kindToPrint + ")");
            codeGenerator.GenCodeLine("                        kind = " + kindToPrint + ";");
        }

        if (next != null && next.usefulEpsilonMoves > 0)
        {
            int[] stateNames = (int[])allNextStates.get(
                                             next.epsilonMovesString);
            if (next.usefulEpsilonMoves == 1)
            {
                int name = stateNames[0];
                if (nextIntersects)
                    codeGenerator.GenCodeLine("                     { jjCheckNAdd(" + name + "); }");
                else
                    codeGenerator.GenCodeLine("                     jjstateSet[jjnewStateCnt++] = " + name + ";");
            }
            else if (next.usefulEpsilonMoves == 2 && nextIntersects)
            {
                codeGenerator.GenCodeLine("                     { jjCheckNAddTwoStates(" +
                   stateNames[0] + ", " + stateNames[1] + "); }");
            }
            else
            {
                int[] indices = GetStateSetIndicesForUse(next.epsilonMovesString);
                bool notTwo = (indices[0] + 1 != indices[1]);

                if (nextIntersects)
                {
                    codeGenerator.GenCode("                     { jjCheckNAddStates(" + indices[0]);
                    if (notTwo)
                    {
                        jjCheckNAddStatesDualNeeded = true;
                        codeGenerator.GenCode(", " + indices[1]);
                    }
                    else
                    {
                        jjCheckNAddStatesUnaryNeeded = true;
                    }
                    codeGenerator.GenCodeLine("); }");
                }
                else
                    codeGenerator.GenCodeLine("                     { jjAddStates(" + indices[0] + ", " + indices[1] + "); }");
            }
        }

        if (kindToPrint != int.MaxValue)
            codeGenerator.GenCodeLine("                  }");
    }

    private void DumpNonAsciiMove(CodeGenerator codeGenerator, bool[] dumped)
    {
        bool nextIntersects = selfLoop() && isComposite;

        for (int j = 0; j < allStates.Count; j++)
        {
            NfaState temp1 = (NfaState)allStates[j];

            if (this == temp1 || temp1.stateName == -1 || temp1.dummy ||
                stateName == temp1.stateName || (temp1.nonAsciiMethod == -1))
                continue;

            if (!nextIntersects && Intersect(temp1.next.epsilonMovesString,
                                            next.epsilonMovesString))
                nextIntersects = true;

            if (!dumped[temp1.stateName] && !temp1.isComposite &&
                nonAsciiMethod == temp1.nonAsciiMethod &&
                kindToPrint == temp1.kindToPrint &&
                (next.epsilonMovesString == temp1.next.epsilonMovesString ||
                 (next.epsilonMovesString != null &&
                  temp1.next.epsilonMovesString != null &&
                  next.epsilonMovesString == (temp1.next.epsilonMovesString))))
            {
                dumped[temp1.stateName] = true;
                codeGenerator.GenCodeLine("               case " + temp1.stateName + ":");
            }
        }

        if (next == null || next.usefulEpsilonMoves <= 0)
        {
            string kindCheck = " && kind > " + kindToPrint;

            if (!Options.GetJavaUnicodeEscape() && !unicodeWarningGiven)
            {
                if (loByteVec != null && loByteVec.Count > 1)
                    codeGenerator.GenCodeLine("                  if ((jjbitVec" +
                     ((int)loByteVec.get(1)).intValue() + "[i2" +
                        "] & l2) != 0L" + kindCheck + ")");
            }
            else
            {
                codeGenerator.GenCodeLine("                  if (jjCanMove_" + nonAsciiMethod +
                                  "(hiByte, i1, i2, l1, l2)" + kindCheck + ")");
            }
            codeGenerator.GenCodeLine("                     kind = " + kindToPrint + ";");
            codeGenerator.GenCodeLine("                  break;");
            return;
        }

        string prefix = "   ";
        if (kindToPrint != int.MaxValue)
        {
            if (!Options.GetJavaUnicodeEscape() && !unicodeWarningGiven)
            {
                if (loByteVec != null && loByteVec.Count > 1)
                {
                    codeGenerator.GenCodeLine("                  if ((jjbitVec" +
                     ((int)loByteVec.get(1)).intValue() + "[i2" +
                     "] & l2) == 0L)");
                    codeGenerator.GenCodeLine("                     break;");
                }
            }
            else
            {
                codeGenerator.GenCodeLine("                  if (!jjCanMove_" + nonAsciiMethod +
                                                          "(hiByte, i1, i2, l1, l2))");
                codeGenerator.GenCodeLine("                     break;");
            }

            codeGenerator.GenCodeLine("                  if (kind > " + kindToPrint + ")");
            codeGenerator.GenCodeLine("                     kind = " + kindToPrint + ";");
            prefix = "";
        }
        else if (!Options.GetJavaUnicodeEscape() && !unicodeWarningGiven)
        {
            if (loByteVec != null && loByteVec.Count > 1)
                codeGenerator.GenCodeLine("                  if ((jjbitVec" +
                 ((int)loByteVec.get(1)).intValue() + "[i2" +
                    "] & l2) != 0L)");
        }
        else
        {
            codeGenerator.GenCodeLine("                  if (jjCanMove_" + nonAsciiMethod +
                                                      "(hiByte, i1, i2, l1, l2))");
        }

        if (next != null && next.usefulEpsilonMoves > 0)
        {
            int[] stateNames = (int[])allNextStates.get(
                                             next.epsilonMovesString);
            if (next.usefulEpsilonMoves == 1)
            {
                int name = stateNames[0];
                if (nextIntersects)
                    codeGenerator.GenCodeLine(prefix + "                  { jjCheckNAdd(" + name + "); }");
                else
                    codeGenerator.GenCodeLine(prefix + "                  jjstateSet[jjnewStateCnt++] = " + name + ";");
            }
            else if (next.usefulEpsilonMoves == 2 && nextIntersects)
            {
                codeGenerator.GenCodeLine(prefix + "                  { jjCheckNAddTwoStates(" +
                   stateNames[0] + ", " + stateNames[1] + "); }");
            }
            else
            {
                int[] indices = GetStateSetIndicesForUse(next.epsilonMovesString);
                bool notTwo = (indices[0] + 1 != indices[1]);

                if (nextIntersects)
                {
                    codeGenerator.GenCode(prefix + "                  { jjCheckNAddStates(" + indices[0]);
                    if (notTwo)
                    {
                        jjCheckNAddStatesDualNeeded = true;
                        codeGenerator.GenCode(", " + indices[1]);
                    }
                    else
                    {
                        jjCheckNAddStatesUnaryNeeded = true;
                    }
                    codeGenerator.GenCodeLine("); }");
                }
                else
                    codeGenerator.GenCodeLine(prefix + "                  { jjAddStates(" + indices[0] + ", " + indices[1] + "); }");
            }
        }

        codeGenerator.GenCodeLine("                  break;");
    }

    public static void DumpCharAndRangeMoves(CodeGenerator codeGenerator)
    {
        bool[] dumped = new bool[Math.Max(generatedStates, dummyStateIndex + 1)];
        Enumeration e = compositeStateTable.keys();
        int i;

        DumpHeadForCase(codeGenerator, -1);

        while (e.hasMoreElements())
            DumpCompositeStatesNonAsciiMoves(codeGenerator, (String)e.nextElement(), dumped);

        for (i = 0; i < allStates.Count; i++)
        {
            NfaState temp = (NfaState)allStates[i];

            if (temp.stateName == -1 || dumped[temp.stateName] || temp.lexState != LexGen.lexStateIndex ||
                !temp.HasTransitions() || temp.dummy)
                continue;

            string toPrint = "";

            if (temp.stateForCase != null)
            {
                if (temp.inNextOf == 1)
                    continue;

                if (dumped[temp.stateForCase.stateName])
                    continue;

                toPrint = (temp.stateForCase.PrintNoBreak(codeGenerator, -1, dumped));

                if (temp.nonAsciiMethod == -1)
                {
                    if (toPrint == (""))
                        codeGenerator.GenCodeLine("                  break;");

                    continue;
                }
            }

            if (temp.nonAsciiMethod == -1)
                continue;

            if (!toPrint == (""))
                codeGenerator.GenCode(toPrint);

            dumped[temp.stateName] = true;
            //Console.println("case : " + temp.stateName);
            codeGenerator.GenCodeLine("               case " + temp.stateName + ":");
            temp.DumpNonAsciiMove(codeGenerator, dumped);
        }


        if (Options.GetJavaUnicodeEscape() || unicodeWarningGiven)
        {
            codeGenerator.GenCodeLine("               default : if (i1 == 0 || l1 == 0 || i2 == 0 ||  l2 == 0) break; else break;");
        }
        else
        {
            codeGenerator.GenCodeLine("               default : break;");
        }
        codeGenerator.GenCodeLine("            }");
        codeGenerator.GenCodeLine("         } while(i != startsAt);");
    }

    public static void DumpNonAsciiMoveMethods(CodeGenerator codeGenerator)
    {
        if (!Options.GetJavaUnicodeEscape() && !unicodeWarningGiven)
            return;

        if (nonAsciiTableForMethod.Count <= 0)
            return;

        for (int i = 0; i < nonAsciiTableForMethod.Count; i++)
        {
            NfaState tmp = (NfaState)nonAsciiTableForMethod[i];
            tmp.DumpNonAsciiMoveMethod(codeGenerator);
        }
    }

    void DumpNonAsciiMoveMethod(CodeGenerator codeGenerator)
    {
        int j;
        if (CodeGenerator.IsJavaLanguage())
        {
            codeGenerator.GenCodeLine("private static final " + Options.GetBooleanType() + " jjCanMove_" + nonAsciiMethod +
                           "(int hiByte, int i1, int i2, " + Options.GetLongType() + " l1, " + Options.GetLongType() + " l2)");
        }
        else
        {
            codeGenerator.GenerateMethodDefHeader("" + Options.GetBooleanType() + "", LexGen.tokMgrClassName, "jjCanMove_" + nonAsciiMethod +
                           "(int hiByte, int i1, int i2, " + Options.GetLongType() + " l1, " + Options.GetLongType() + " l2)");
        }
        codeGenerator.GenCodeLine("{");
        codeGenerator.GenCodeLine("   switch(hiByte)");
        codeGenerator.GenCodeLine("   {");

        if (loByteVec != null && loByteVec.Count > 0)
        {
            for (j = 0; j < loByteVec.Count; j += 2)
            {
                codeGenerator.GenCodeLine("      case " +
                             ((int)loByteVec[j]).intValue() + ":");
                if (!AllBitsSet((String)allBitVectors.get(
                     ((int)loByteVec.get(j + 1)).intValue())))
                {
                    codeGenerator.GenCodeLine("         return ((jjbitVec" +
                     ((int)loByteVec.get(j + 1)).intValue() + "[i2" +
                        "] & l2) != 0L);");
                }
                else
                    codeGenerator.GenCodeLine("            return true;");
            }
        }

        codeGenerator.GenCodeLine("      default :");

        if (nonAsciiMoveIndices != null &&
            (j = nonAsciiMoveIndices.Length) > 0)
        {
            do
            {
                if (!AllBitsSet((String)allBitVectors.get(
                                   nonAsciiMoveIndices[j - 2])))
                    codeGenerator.GenCodeLine("         if ((jjbitVec" + nonAsciiMoveIndices[j - 2] +
                                 "[i1] & l1) != 0L)");
                if (!AllBitsSet((String)allBitVectors.get(
                                   nonAsciiMoveIndices[j - 1])))
                {
                    codeGenerator.GenCodeLine("            if ((jjbitVec" + nonAsciiMoveIndices[j - 1] +
                                 "[i2] & l2) == 0L)");
                    codeGenerator.GenCodeLine("               return false;");
                    codeGenerator.GenCodeLine("            else");
                }
                codeGenerator.GenCodeLine("            return true;");
            }
            while ((j -= 2) > 0);
        }

        codeGenerator.GenCodeLine("         return false;");
        codeGenerator.GenCodeLine("   }");
        codeGenerator.GenCodeLine("}");
    }

    private static void ReArrange()
    {
        List v = allStates;
        allStates = new List(Collections.nCopies(generatedStates, null));

        if (allStates.Count != generatedStates) throw new Error("What??");

        for (int j = 0; j < v.Count; j++)
        {
            NfaState tmp = (NfaState)v[j];
            if (tmp.stateName != -1 && !tmp.dummy)
                allStates[tmp.stateName]= tmp;
        }
    }

    //private static bool boilerPlateDumped = false;
    public static void PrintBoilerPlate(CodeGenerator codeGenerator)
    {
        codeGenerator.GenCodeLine((Options.GetStatic() ? "static " : "") + "private void " +
                     "jjCheckNAdd(int state)");
        codeGenerator.GenCodeLine("{");
        codeGenerator.GenCodeLine("   if (jjrounds[state] != jjround)");
        codeGenerator.GenCodeLine("   {");
        codeGenerator.GenCodeLine("      jjstateSet[jjnewStateCnt++] = state;");
        codeGenerator.GenCodeLine("      jjrounds[state] = jjround;");
        codeGenerator.GenCodeLine("   }");
        codeGenerator.GenCodeLine("}");

        codeGenerator.GenCodeLine((Options.GetStatic() ? "static " : "") + "private void " +
                      "jjAddStates(int start, int end)");
        codeGenerator.GenCodeLine("{");
        codeGenerator.GenCodeLine("   do {");
        codeGenerator.GenCodeLine("      jjstateSet[jjnewStateCnt++] = jjnextStates[start];");
        codeGenerator.GenCodeLine("   } while (start++ != end);");
        codeGenerator.GenCodeLine("}");

        codeGenerator.GenCodeLine((Options.GetStatic() ? "static " : "") + "private void " +
                      "jjCheckNAddTwoStates(int state1, int state2)");
        codeGenerator.GenCodeLine("{");
        codeGenerator.GenCodeLine("   jjCheckNAdd(state1);");
        codeGenerator.GenCodeLine("   jjCheckNAdd(state2);");
        codeGenerator.GenCodeLine("}");
        codeGenerator.GenCodeLine("");

        if (jjCheckNAddStatesDualNeeded)
        {
            codeGenerator.GenCodeLine((Options.GetStatic() ? "static " : "") + "private void " +
                         "jjCheckNAddStates(int start, int end)");
            codeGenerator.GenCodeLine("{");
            codeGenerator.GenCodeLine("   do {");
            codeGenerator.GenCodeLine("      jjCheckNAdd(jjnextStates[start]);");
            codeGenerator.GenCodeLine("   } while (start++ != end);");
            codeGenerator.GenCodeLine("}");
            codeGenerator.GenCodeLine("");
        }

        if (jjCheckNAddStatesUnaryNeeded)
        {
            codeGenerator.GenCodeLine((Options.GetStatic() ? "static " : "") + "private void " +
                      "jjCheckNAddStates(int start)");
            codeGenerator.GenCodeLine("{");
            codeGenerator.GenCodeLine("   jjCheckNAdd(jjnextStates[start]);");
            codeGenerator.GenCodeLine("   jjCheckNAdd(jjnextStates[start + 1]);");
            codeGenerator.GenCodeLine("}");
            codeGenerator.GenCodeLine("");
        }
    }

    //private static bool boilerPlateDumped = false;
    public static void PrintBoilerPlateCPP(CodeGenerator codeGenerator)
    {
        codeGenerator.SwitchToIncludeFile();
        codeGenerator.GenCodeLine("#define jjCheckNAdd(state)\\");
        codeGenerator.GenCodeLine("{\\");
        codeGenerator.GenCodeLine("   if (jjrounds[state] != jjround)\\");
        codeGenerator.GenCodeLine("   {\\");
        codeGenerator.GenCodeLine("      jjstateSet[jjnewStateCnt++] = state;\\");
        codeGenerator.GenCodeLine("      jjrounds[state] = jjround;\\");
        codeGenerator.GenCodeLine("   }\\");
        codeGenerator.GenCodeLine("}");

        codeGenerator.GenCodeLine("#define jjAddStates(start, end)\\");
        codeGenerator.GenCodeLine("{\\");
        codeGenerator.GenCodeLine("   for (int x = start; x <= end; x++) {\\");
        codeGenerator.GenCodeLine("      jjstateSet[jjnewStateCnt++] = jjnextStates[x];\\");
        codeGenerator.GenCodeLine("   } /*while (start++ != end);*/\\");
        codeGenerator.GenCodeLine("}");

        codeGenerator.GenCodeLine("#define jjCheckNAddTwoStates(state1, state2)\\");
        codeGenerator.GenCodeLine("{\\");
        codeGenerator.GenCodeLine("   jjCheckNAdd(state1);\\");
        codeGenerator.GenCodeLine("   jjCheckNAdd(state2);\\");
        codeGenerator.GenCodeLine("}");
        codeGenerator.GenCodeLine("");

        if (jjCheckNAddStatesDualNeeded)
        {
            codeGenerator.GenCodeLine("#define jjCheckNAddStates(start, end)\\");
            codeGenerator.GenCodeLine("{\\");
            codeGenerator.GenCodeLine("   for (int x = start; x <= end; x++) {\\");
            codeGenerator.GenCodeLine("      jjCheckNAdd(jjnextStates[x]);\\");
            codeGenerator.GenCodeLine("   } /*while (start++ != end);*/\\");
            codeGenerator.GenCodeLine("}");
            codeGenerator.GenCodeLine("");
        }

        if (jjCheckNAddStatesUnaryNeeded)
        {
            codeGenerator.GenCodeLine("#define jjCheckNAddStates(start)\\");
            codeGenerator.GenCodeLine("{\\");
            codeGenerator.GenCodeLine("   jjCheckNAdd(jjnextStates[start]);\\");
            codeGenerator.GenCodeLine("   jjCheckNAdd(jjnextStates[start + 1]);\\");
            codeGenerator.GenCodeLine("}");
            codeGenerator.GenCodeLine("");
        }
        codeGenerator.SwitchToMainFile();
    }

    private static void FindStatesWithNoBreak()
    {
        Dictionary printed = new Dictionary();
        bool[] put = new bool[generatedStates];
        int cnt = 0;
        int i, j, foundAt = 0;

    Outer:
        for (j = 0; j < allStates.Count; j++)
        {
            NfaState stateForCase = null;
            NfaState tmpState = (NfaState)allStates[j];

            if (tmpState.stateName == -1 || tmpState.dummy || !tmpState.UsefulState() ||
                tmpState.next == null || tmpState.next.usefulEpsilonMoves < 1)
                continue;

            string s = tmpState.next.epsilonMovesString;

            if (compositeStateTable.get(s) != null || printed.get(s) != null)
                continue;

            printed.Add(s, s);
            int[] nexts = (int[])allNextStates.get(s);

            if (nexts.Length == 1)
                continue;

            int state = cnt;
            //Console.println("State " + tmpState.stateName + " : " + s);
            for (i = 0; i < nexts.Length; i++)
            {
                if ((state = nexts[i]) == -1)
                    continue;

                NfaState tmp = (NfaState)allStates.get(state);

                if (!tmp.isComposite && tmp.inNextOf == 1)
                {
                    if (put[state])
                        throw new Error("JavaCC Bug: Please send mail to sankar@cs.stanford.edu");

                    foundAt = i;
                    cnt++;
                    stateForCase = tmp;
                    put[state] = true;

                    //Console.Write(state + " : " + tmp.inNextOf + ", ");
                    break;
                }
            }
            //Console.println("");

            if (stateForCase == null)
                continue;

            for (i = 0; i < nexts.Length; i++)
            {
                if ((state = nexts[i]) == -1)
                    continue;

                NfaState tmp = (NfaState)allStates.get(state);

                if (!put[state] && tmp.inNextOf > 1 && !tmp.isComposite && tmp.stateForCase == null)
                {
                    cnt++;
                    nexts[i] = -1;
                    put[state] = true;

                    (nexts[foundAt], nexts[0]) = (nexts[0], nexts[foundAt]);
                    tmp.stateForCase = stateForCase;
                    stateForCase.stateForCase = tmp;
                    stateSetsToFix.Add(s, nexts);

                    //Console.println("For : " + s + "; " + stateForCase.stateName +
                    //" and " + tmp.stateName);

                    continue Outer;
                }
            }

            for (i = 0; i < nexts.Length; i++)
            {
                if ((state = nexts[i]) == -1)
                    continue;

                NfaState tmp = (NfaState)allStates.get(state);
                if (tmp.inNextOf <= 1)
                    put[state] = false;
            }
        }
    }

    static int[][] kinds;
    static int[][][] statesForState;
    public static void DumpMoveNfa(CodeGenerator codeGenerator)
    {
        //if (!boilerPlateDumped)
        //   PrintBoilerPlate(codeGenerator);

        //boilerPlateDumped = true;
        int i;
        int[] kindsForStates = null;

        if (kinds == null)
        {
            kinds = new int[LexGen.maxLexStates][];
            statesForState = new int[LexGen.maxLexStates][][];
        }

        ReArrange();

        for (i = 0; i < allStates.Count; i++)
        {
            NfaState temp = (NfaState)allStates[i];

            if (temp.lexState != LexGen.lexStateIndex ||
                !temp.HasTransitions() || temp.dummy ||
                temp.stateName == -1)
                continue;

            if (kindsForStates == null)
            {
                kindsForStates = new int[generatedStates];
                statesForState[LexGen.lexStateIndex] = new int[Math.Max(generatedStates, dummyStateIndex + 1)][];
            }

            kindsForStates[temp.stateName] = temp.lookingFor;
            statesForState[LexGen.lexStateIndex][temp.stateName] = temp.compositeStates;

            temp.GenerateNonAsciiMoves(codeGenerator);
        }

        Enumeration e = stateNameForComposite.keys();

        while (e.hasMoreElements())
        {
            string s = (String)e.nextElement();
            int state = ((int)stateNameForComposite.get(s)).intValue();

            if (state >= generatedStates)
                statesForState[LexGen.lexStateIndex][state] = (int[])allNextStates.get(s);
        }

        if (stateSetsToFix.Count != 0)
            FixStateSets();

        kinds[LexGen.lexStateIndex] = kindsForStates;

        if (CodeGenerator.IsJavaLanguage())
        {
            codeGenerator.GenCodeLine((Options.GetStatic() ? "static " : "") + "private int " +
                        "jjMoveNfa" + LexGen.lexStateSuffix + "(int startState, int curPos)");
        }
        else
        {
            codeGenerator.GenerateMethodDefHeader("int", LexGen.tokMgrClassName, "jjMoveNfa" + LexGen.lexStateSuffix + "(int startState, int curPos)");
        }
        codeGenerator.GenCodeLine("{");
        if (generatedStates == 0)
        {
            codeGenerator.GenCodeLine("   return curPos;");
            codeGenerator.GenCodeLine("}");
            return;
        }

        if (LexGen.mixed[LexGen.lexStateIndex])
        {
            codeGenerator.GenCodeLine("   int strKind = jjmatchedKind;");
            codeGenerator.GenCodeLine("   int strPos = jjmatchedPos;");
            codeGenerator.GenCodeLine("   int seenUpto;");
            if (CodeGenerator.IsJavaLanguage())
            {
                codeGenerator.GenCodeLine("   input_stream.backup(seenUpto = curPos + 1);");
                codeGenerator.GenCodeLine("   try { curChar = input_stream.readChar(); }");
                codeGenerator.GenCodeLine("   catch(java.io.IOException e) { throw new Error(\"Internal Error\"); }");
            }
            else
            {
                codeGenerator.GenCodeLine("   input_stream->backup(seenUpto = curPos + 1);");
                codeGenerator.GenCodeLine("   assert(!input_stream->endOfInput());");
                codeGenerator.GenCodeLine("   curChar = input_stream->readChar();");
            }
            codeGenerator.GenCodeLine("   curPos = 0;");
        }

        codeGenerator.GenCodeLine("   int startsAt = 0;");
        codeGenerator.GenCodeLine("   jjnewStateCnt = " + generatedStates + ";");
        codeGenerator.GenCodeLine("   int i = 1;");
        codeGenerator.GenCodeLine("   jjstateSet[0] = startState;");

        if (Options.GetDebugTokenManager())
        {
            if (CodeGenerator.IsJavaLanguage())
            {
                codeGenerator.GenCodeLine("      debugStream.println(\"   Starting NFA to match one of : \" + " +
                        "jjKindsForStateVector(curLexState, jjstateSet, 0, 1));");
            }
            else
            {
                codeGenerator.GenCodeLine("      fprintf(debugStream, \"   Starting NFA to match one of : %s\\n\", jjKindsForStateVector(curLexState, jjstateSet, 0, 1).c_str());");
            }
        }

        if (Options.GetDebugTokenManager())
        {
            if (CodeGenerator.IsJavaLanguage())
            {
                codeGenerator.GenCodeLine("      debugStream.println(" + (LexGen.maxLexStates > 1 ?
                        "\"<\" + lexStateNames[curLexState] + \">\" + " :
                        "") + "\"Current character : \" + " + Options.getTokenMgrErrorClass() +
                        ".addEscapes(String.valueOf(curChar)) + \" (\" + (int)curChar + \") " + "at line \" + input_stream.getEndLine() + \" column \" + input_stream.getEndColumn());");
            }
            else
            {
                codeGenerator.GenCodeLine("   fprintf(debugStream, " +
                   "\"<%s>Current character : %c(%d) at line %d column %d\\n\"," +
                   "addUnicodeEscapes(lexStateNames[curLexState]).c_str(), curChar, (int)curChar, " +
                   "input_stream->getEndLine(), input_stream->getEndColumn());");
            }
        }

        codeGenerator.GenCodeLine("   int kind = 0x" + Convert.ToString(int.MaxValue,16) + ";");
        codeGenerator.GenCodeLine("   for (;;)");
        codeGenerator.GenCodeLine("   {");
        codeGenerator.GenCodeLine("      if (++jjround == 0x" + Convert.ToString(int.MaxValue,16) + ")");
        codeGenerator.GenCodeLine("         ReInitRounds();");
        codeGenerator.GenCodeLine("      if (curChar < 64)");
        codeGenerator.GenCodeLine("      {");

        DumpAsciiMoves(codeGenerator, 0);

        codeGenerator.GenCodeLine("      }");

        codeGenerator.GenCodeLine("      else if (curChar < 128)");

        codeGenerator.GenCodeLine("      {");

        DumpAsciiMoves(codeGenerator, 1);

        codeGenerator.GenCodeLine("      }");

        codeGenerator.GenCodeLine("      else");
        codeGenerator.GenCodeLine("      {");

        DumpCharAndRangeMoves(codeGenerator);

        codeGenerator.GenCodeLine("      }");

        codeGenerator.GenCodeLine("      if (kind != 0x" + Convert.ToString(int.MaxValue,16) + ")");
        codeGenerator.GenCodeLine("      {");
        codeGenerator.GenCodeLine("         jjmatchedKind = kind;");
        codeGenerator.GenCodeLine("         jjmatchedPos = curPos;");
        codeGenerator.GenCodeLine("         kind = 0x" + Convert.ToString(int.MaxValue,16) + ";");
        codeGenerator.GenCodeLine("      }");
        codeGenerator.GenCodeLine("      ++curPos;");

        if (Options.GetDebugTokenManager())
        {
            if (CodeGenerator.IsJavaLanguage())
            {
                codeGenerator.GenCodeLine("      if (jjmatchedKind != 0 && jjmatchedKind != 0x" +
                      Convert.ToString(int.MaxValue,16) + ")");
                codeGenerator.GenCodeLine("         debugStream.println(" +
                       "\"   Currently matched the first \" + (jjmatchedPos + 1) + \" characters as" +
                       " a \" + tokenImage[jjmatchedKind] + \" token.\");");
            }
            else
            {
                codeGenerator.GenCodeLine("      if (jjmatchedKind != 0 && jjmatchedKind != 0x" +
                      Convert.ToString(int.MaxValue,16) + ")");
                codeGenerator.GenCodeLine("   fprintf(debugStream, \"   Currently matched the first %d characters as a \\\"%s\\\" token.\\n\",  (jjmatchedPos + 1),  addUnicodeEscapes(tokenImage[jjmatchedKind]).c_str());");
            }
        }

        if (CodeGenerator.IsJavaLanguage())
        {
            codeGenerator.GenCodeLine("      if ((i = jjnewStateCnt) == (startsAt = " +
                       generatedStates + " - (jjnewStateCnt = startsAt)))");
        }
        else
        {
            codeGenerator.GenCodeLine("      if ((i = jjnewStateCnt), (jjnewStateCnt = startsAt), (i == (startsAt = " +
                       generatedStates + " - startsAt)))");
        }
        if (LexGen.mixed[LexGen.lexStateIndex])
            codeGenerator.GenCodeLine("         break;");
        else
            codeGenerator.GenCodeLine("         return curPos;");

        if (Options.GetDebugTokenManager())
        {
            if (CodeGenerator.IsJavaLanguage())
            {
                codeGenerator.GenCodeLine("      debugStream.println(\"   Possible kinds of longer matches : \" + " +
                       "jjKindsForStateVector(curLexState, jjstateSet, startsAt, i));");
            }
            else
            {
                codeGenerator.GenCodeLine("      fprintf(debugStream, \"   Possible kinds of longer matches : %s\\n\", jjKindsForStateVector(curLexState, jjstateSet, startsAt, i).c_str());");
            }
        }

        if (CodeGenerator.IsJavaLanguage())
        {
            codeGenerator.GenCodeLine("      try { curChar = input_stream.readChar(); }");
        }
        else
        {
            if (LexGen.mixed[LexGen.lexStateIndex])
            {
                codeGenerator.GenCodeLine("      if (input_stream->endOfInput()) { break; }");
            }
            else
            {
                codeGenerator.GenCodeLine("      if (input_stream->endOfInput()) { return curPos; }");
            }
            codeGenerator.GenCodeLine("      curChar = input_stream->readChar();");
        }

        if (LexGen.mixed[LexGen.lexStateIndex])
        {
            if (CodeGenerator.IsJavaLanguage())
            {
                codeGenerator.GenCodeLine("      catch(java.io.IOException e) { break; }");
            }
        }
        else
        {
            if (CodeGenerator.IsJavaLanguage())
            {
                codeGenerator.GenCodeLine("      catch(java.io.IOException e) { return curPos; }");
            }
        }

        if (Options.GetDebugTokenManager())
        {
            if (CodeGenerator.IsJavaLanguage())
            {
                codeGenerator.GenCodeLine("      debugStream.println(" + (LexGen.maxLexStates > 1 ?
                        "\"<\" + lexStateNames[curLexState] + \">\" + " :
                        "") + "\"Current character : \" + " + Options.getTokenMgrErrorClass() +
                        ".addEscapes(String.valueOf(curChar)) + \" (\" + (int)curChar + \") " +
                        "at line \" + input_stream.getEndLine() + \" column \" + input_stream.getEndColumn());");
            }
            else
            {
                codeGenerator.GenCodeLine("   fprintf(debugStream, " +
                   "\"<%s>Current character : %c(%d) at line %d column %d\\n\"," +
                   "addUnicodeEscapes(lexStateNames[curLexState]).c_str(), curChar, (int)curChar, " +
                   "input_stream->getEndLine(), input_stream->getEndColumn());");
            }
        }

        codeGenerator.GenCodeLine("   }");

        if (LexGen.mixed[LexGen.lexStateIndex])
        {
            codeGenerator.GenCodeLine("   if (jjmatchedPos > strPos)");
            codeGenerator.GenCodeLine("      return curPos;");
            codeGenerator.GenCodeLine("");
            if (CodeGenerator.IsJavaLanguage())
            {
                codeGenerator.GenCodeLine("   int toRet = Math.max(curPos, seenUpto);");
            }
            else
            {
                codeGenerator.GenCodeLine("   int toRet = MAX(curPos, seenUpto);");
            }
            codeGenerator.GenCodeLine("");
            codeGenerator.GenCodeLine("   if (curPos < toRet)");
            if (CodeGenerator.IsJavaLanguage())
            {
                codeGenerator.GenCodeLine("      for (i = toRet - Math.min(curPos, seenUpto); i-- > 0; )");
                codeGenerator.GenCodeLine("         try { curChar = input_stream.readChar(); }");
                codeGenerator.GenCodeLine("         catch(java.io.IOException e) { " +
                        "throw new Error(\"Internal Error : Please send a bug report.\"); }");
            }
            else
            {
                codeGenerator.GenCodeLine("      for (i = toRet - MIN(curPos, seenUpto); i-- > 0; )");
                codeGenerator.GenCodeLine("        {  assert(!input_stream->endOfInput());");
                codeGenerator.GenCodeLine("           curChar = input_stream->readChar(); }");
            }
            codeGenerator.GenCodeLine("");
            codeGenerator.GenCodeLine("   if (jjmatchedPos < strPos)");
            codeGenerator.GenCodeLine("   {");
            codeGenerator.GenCodeLine("      jjmatchedKind = strKind;");
            codeGenerator.GenCodeLine("      jjmatchedPos = strPos;");
            codeGenerator.GenCodeLine("   }");
            codeGenerator.GenCodeLine("   else if (jjmatchedPos == strPos && jjmatchedKind > strKind)");
            codeGenerator.GenCodeLine("      jjmatchedKind = strKind;");
            codeGenerator.GenCodeLine("");
            codeGenerator.GenCodeLine("   return toRet;");
        }

        codeGenerator.GenCodeLine("}");
        allStates.Clear();
    }

    public static void DumpStatesForStateCPP(CodeGenerator codeGenerator)
    {
        if (statesForState == null)
        {
            //assert(false) : "This should never be null.";
            codeGenerator.GenCodeLine("null;");
            return;
        }

        codeGenerator.SwitchToStaticsFile();
        for (int i = 0; i < LexGen.maxLexStates; i++)
        {
            if (statesForState[i] == null)
            {
                continue;
            }

            for (int j = 0; j < statesForState[i].Length; j++)
            {
                int[] stateSet = statesForState[i][j];

                codeGenerator.GenCode("const int stateSet_" + i + "_" + j + "[" +
                           LexGen.stateSetSize + "] = ");
                if (stateSet == null)
                {
                    codeGenerator.GenCodeLine("   { " + j + " };");
                    continue;
                }

                codeGenerator.GenCode("   { ");

                for (int k = 0; k < stateSet.Length; k++)
                    codeGenerator.GenCode(stateSet[k] + ", ");

                codeGenerator.GenCodeLine("};");
            }

        }

        for (int i = 0; i < LexGen.maxLexStates; i++)
        {
            codeGenerator.GenCodeLine("const int *stateSet_" + i + "[] = {");
            if (statesForState[i] == null)
            {
                codeGenerator.GenCodeLine(" NULL, ");
                codeGenerator.GenCodeLine("};");
                continue;
            }

            for (int j = 0; j < statesForState[i].Length; j++)
            {
                codeGenerator.GenCode("stateSet_" + i + "_" + j + ",");
            }
            codeGenerator.GenCodeLine("};");
        }

        codeGenerator.GenCode("const int** statesForState[] = { ");
        for (int i = 0; i < LexGen.maxLexStates; i++)
        {
            codeGenerator.GenCodeLine("stateSet_" + i + ", ");
        }

        codeGenerator.GenCodeLine("\n};");
        codeGenerator.SwitchToMainFile();
    }


    public static void DumpStatesForState(CodeGenerator codeGenerator)
    {
        codeGenerator.GenCode("protected static final int[][][] statesForState = ");

        if (statesForState == null)
        {
            //assert(false) : "This should never be null.";
            codeGenerator.GenCodeLine("null;");
            return;
        }
        else
        {
            codeGenerator.GenCodeLine("{");
        }

        for (int i = 0; i < LexGen.maxLexStates; i++)
        {
            if (statesForState[i] == null)
            {
                codeGenerator.GenCodeLine(" {},");
                continue;
            }

            codeGenerator.GenCodeLine(" {");

            for (int j = 0; j < statesForState[i].Length; j++)
            {
                int[] stateSet = statesForState[i][j];

                if (stateSet == null)
                {
                    codeGenerator.GenCodeLine("   { " + j + " },");
                    continue;
                }

                codeGenerator.GenCode("   { ");

                for (int k = 0; k < stateSet.Length; k++)
                    codeGenerator.GenCode(stateSet[k] + ", ");

                codeGenerator.GenCodeLine("},");
            }

            codeGenerator.GenCodeLine("},");
        }

        codeGenerator.GenCodeLine("\n};");
    }

    public static void DumpStatesForKind(CodeGenerator codeGenerator)
    {
        if (CodeGenerator.IsJavaLanguage())
        {
            DumpStatesForState(codeGenerator);
        }
        else
        {
            DumpStatesForStateCPP(codeGenerator);
        }
        bool moreThanOne = false;
        int cnt = 0;

        if (CodeGenerator.IsJavaLanguage())
        {
            codeGenerator.GenCode("protected static final int[][] kindForState = ");
        }
        else
        {
            codeGenerator.SwitchToStaticsFile();
            codeGenerator.GenCode("static const int kindForState[" + LexGen.stateSetSize + "][" + LexGen.stateSetSize + "] = ");
        }

        if (kinds == null)
        {
            codeGenerator.GenCodeLine("null;");
            return;
        }
        else
            codeGenerator.GenCodeLine("{");

        for (int i = 0; i < kinds.Length; i++)
        {
            if (moreThanOne)
                codeGenerator.GenCodeLine(",");
            moreThanOne = true;

            if (kinds[i] == null)
                codeGenerator.GenCodeLine("{}");
            else
            {
                cnt = 0;
                codeGenerator.GenCode("{ ");
                for (int j = 0; j < kinds[i].Length; j++)
                {
                    if (cnt % 15 == 0)
                        codeGenerator.GenCode("\n  ");
                    else if (cnt > 1)
                        codeGenerator.GenCode(" ");

                    codeGenerator.GenCode(kinds[i][j]);
                    codeGenerator.GenCode(", ");

                }

                codeGenerator.GenCode("}");
            }
        }
        codeGenerator.GenCodeLine("\n};");
        codeGenerator.SwitchToMainFile();
    }

    public static void reInit()
    {
        unicodeWarningGiven = false;
        generatedStates = 0;
        idCnt = 0;
        lohiByteCnt = 0;
        dummyStateIndex = -1;
        done = false;
        mark = null;
        stateDone = null;
        allStates = new();
        indexedAllStates = new();
        nonAsciiTableForMethod = new();
        equivStatesTable = new();
        allNextStates = new();
        lohiByteTab = new();
        stateNameForComposite = new();
        compositeStateTable = new();
        stateBlockTable = new();
        stateSetsToFix = new();
        allBitVectors = new();
        tmpIndices = new int[512];
        allBits = "{\n   0xffffffffffffffffL, " +
                      "0xffffffffffffffffL, " +
                      "0xffffffffffffffffL, " +
                      "0xffffffffffffffffL\n};";
        tableToDump = new ();
        orderedStateSet = new ();
        lastIndex = 0;
        //boilerPlateDumped = false; 
        jjCheckNAddStatesUnaryNeeded = false;
        jjCheckNAddStatesDualNeeded = false;
        kinds = null;
        statesForState = null;
    }

    private static readonly Dictionary<int, NfaState> initialStates =
        new Dictionary<int, NfaState>();
    private static readonly Dictionary<int, List<NfaState>> statesForLexicalState =
        new Dictionary<int, List<NfaState>>();
    private static readonly Dictionary<int, int> nfaStateOffset =
        new Dictionary<int, int>();
    private static readonly Dictionary<int, int> matchAnyChar =
        new Dictionary<int, int>();
    public static void UpdateNfaData(
        int maxState, int startStateName, int lexicalStateIndex,
        int matchAnyCharKind)
    {
        // Cleanup the state set.
        HashSet<int> done = new HashSet<int>();
        List<NfaState> cleanStates = new();
        NfaState startState = null;
        for (int i = 0; i < allStates.Count; i++)
        {
            NfaState tmp = (NfaState)allStates[i];
            if (tmp.stateName == -1) continue;
            if (done.Contains(tmp.stateName)) continue;
            done.Add(tmp.stateName);
            cleanStates.Add(tmp);
            if (tmp.stateName == startStateName)
            {
                startState = tmp;
            }
        }

        initialStates.Add(lexicalStateIndex, startState);
        statesForLexicalState.Add(lexicalStateIndex, cleanStates);
        nfaStateOffset.Add(lexicalStateIndex, maxState);
        if (matchAnyCharKind > 0)
        {
            matchAnyChar.Add(lexicalStateIndex, matchAnyCharKind);
        }
        else
        {
            matchAnyChar.Add(lexicalStateIndex, int.MaxValue);
        }
    }

    public static void BuildTokenizerData(TokenizerData tokenizerData)
    {
        NfaState[] cleanStates;
        List<NfaState> cleanStateList = new();
        foreach (int l in statesForLexicalState.keySet())
        {
            int offset = nfaStateOffset[l];
            List<NfaState> states = statesForLexicalState[l];
            for (int i = 0; i < states.Count; i++)
            {
                NfaState state = states[i];
                if (state.stateName == -1) continue;
                states[i].stateName += offset;
            }
            cleanStateList.addAll(states);
        }
        cleanStates = new NfaState[cleanStateList.Count];
        foreach (NfaState s in cleanStateList)
        {
            //assert(cleanStates[s.stateName] == null);
            cleanStates[s.stateName] = s;
            HashSet<char> chars = new HashSet<char>();
            for (int c = 0; c <= char.MaxValue; c++)
            {
                if (s.CanMoveUsingChar((char)c))
                {
                    chars.Add((char)c);
                }
            }
            HashSet<int> nextStates = new HashSet<int>();
            if (s.next != null)
            {
                foreach (NfaState next in s.next.epsilonMoveArray)
                {
                    nextStates.Add(next.stateName);
                }
            }
            HashSet<int> composite = new HashSet<int>();
            if (s.isComposite)
            {
                foreach (int c in s.compositeStates) composite.Add(c);
            }
            tokenizerData.AddNfaState(
                s.stateName, chars, nextStates, composite, s.kindToPrint);
        }
        var initStates = new Dictionary<int, int>();
        foreach (int l in initialStates.Keys)
        {
            if (initialStates[l] == null)
            {
                initStates.Add(l, -1);
            }
            else
            {
                initStates.Add(l, initialStates[l].stateName);
            }
        }
        tokenizerData.SetInitialStates(initStates);
        tokenizerData.SetWildcardKind(matchAnyChar);
    }

    public static NfaState GetNfaState(int index)
    {
        if (index == -1) return null;
        foreach (NfaState s in allStates)
        {
            if (s.stateName == index) return s;
        }
        //assert(false);
        return null;
    }
}
