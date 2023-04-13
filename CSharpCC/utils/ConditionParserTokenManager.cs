using CSharpCC.CCTree;
using System.Text;

namespace CSharpCC.Utils;

/** Token Manager. */
public class ConditionParserTokenManager : ConditionParserConstants
{

    /** Debug output. */
    public TextWriter debugStream = Console.Out;
    /** Set debug output. */
    public void SetDebugStream(TextWriter ds) => debugStream = ds;
    private int ccStopStringLiteralDfa_0(int pos, long active0)
    {
        switch (pos)
        {
            case 0:
                if ((active0 & 0x180000L) != 0L)
                {
                    ccmatchedKind = 21;
                    return 5;
                }
                if ((active0 & 0x140L) != 0L)
                    return 2;
                return -1;
            case 1:
                if ((active0 & 0x100L) != 0L)
                    return 0;
                if ((active0 & 0x180000L) != 0L)
                {
                    ccmatchedKind = 21;
                    ccmatchedPos = 1;
                    return 5;
                }
                return -1;
            case 2:
                if ((active0 & 0x180000L) != 0L)
                {
                    ccmatchedKind = 21;
                    ccmatchedPos = 2;
                    return 5;
                }
                return -1;
            case 3:
                if ((active0 & 0x100000L) != 0L)
                {
                    ccmatchedKind = 21;
                    ccmatchedPos = 3;
                    return 5;
                }
                if ((active0 & 0x80000L) != 0L)
                    return 5;
                return -1;
            default:
                return -1;
        }
    }
    private int ccStartNfa_0(int pos, long active0)
    {
        return ccMoveNfa_0(ccStopStringLiteralDfa_0(pos, active0), pos + 1);
    }
    private int ccStopAtPos(int pos, int kind)
    {
        ccmatchedKind = kind;
        ccmatchedPos = pos;
        return pos + 1;
    }
    private int ccMoveStringLiteralDfa0_0() => (int)curChar switch
    {
        33 => ccStopAtPos(0, 15),
        38 => ccMoveStringLiteralDfa1_0(0x20000L),
        40 => ccStopAtPos(0, 13),
        41 => ccStopAtPos(0, 14),
        47 => ccMoveStringLiteralDfa1_0(0x140L),
        102 => ccMoveStringLiteralDfa1_0(0x100000L),
        116 => ccMoveStringLiteralDfa1_0(0x80000L),
        124 => ccMoveStringLiteralDfa1_0(0x10000L),
        126 => ccStopAtPos(0, 18),
        _ => ccMoveNfa_0(3, 0),
    };
    private int ccMoveStringLiteralDfa1_0(long active0)
    {
        try { curChar = input_stream.readChar(); }
        catch (IOException e)
        {
            ccStopStringLiteralDfa_0(0, active0);
            return 1;
        }
        switch ((int)curChar)
        {
            case 38:
                if ((active0 & 0x20000L) != 0L)
                    return ccStopAtPos(1, 17);
                break;
            case 42:
                if ((active0 & 0x100L) != 0L)
                    return ccStartNfaWithStates_0(1, 8, 0);
                break;
            case 47:
                if ((active0 & 0x40L) != 0L)
                    return ccStopAtPos(1, 6);
                break;
            case 97:
                return ccMoveStringLiteralDfa2_0(active0, 0x100000L);
            case 114:
                return ccMoveStringLiteralDfa2_0(active0, 0x80000L);
            case 124:
                if ((active0 & 0x10000L) != 0L)
                    return ccStopAtPos(1, 16);
                break;
            default:
                break;
        }
        return ccStartNfa_0(0, active0);
    }
    private int ccMoveStringLiteralDfa2_0(long old0, long active0)
    {
        if (((active0 &= old0)) == 0L)
            return ccStartNfa_0(0, old0);
        try { curChar = input_stream.readChar(); }
        catch (IOException e)
        {
            ccStopStringLiteralDfa_0(1, active0);
            return 2;
        }
        switch ((int)curChar)
        {
            case 108:
                return ccMoveStringLiteralDfa3_0(active0, 0x100000L);
            case 117:
                return ccMoveStringLiteralDfa3_0(active0, 0x80000L);
            default:
                break;
        }
        return ccStartNfa_0(1, active0);
    }
    private int ccMoveStringLiteralDfa3_0(long old0, long active0)
    {
        if (((active0 &= old0)) == 0L)
            return ccStartNfa_0(1, old0);
        try { curChar = input_stream.readChar(); }
        catch (IOException e)
        {
            ccStopStringLiteralDfa_0(2, active0);
            return 3;
        }
        switch ((int)curChar)
        {
            case 101:
                if ((active0 & 0x80000L) != 0L)
                    return ccStartNfaWithStates_0(3, 19, 5);
                break;
            case 115:
                return ccMoveStringLiteralDfa4_0(active0, 0x100000L);
            default:
                break;
        }
        return ccStartNfa_0(2, active0);
    }
    private int ccMoveStringLiteralDfa4_0(long old0, long active0)
    {
        if (((active0 &= old0)) == 0L)
            return ccStartNfa_0(2, old0);
        try { curChar = input_stream.readChar(); }
        catch (IOException e)
        {
            ccStopStringLiteralDfa_0(3, active0);
            return 4;
        }
        switch ((int)curChar)
        {
            case 101:
                if ((active0 & 0x100000L) != 0L)
                    return ccStartNfaWithStates_0(4, 20, 5);
                break;
            default:
                break;
        }
        return ccStartNfa_0(3, active0);
    }
    private int ccStartNfaWithStates_0(int pos, int kind, int state)
    {
        ccmatchedKind = kind;
        ccmatchedPos = pos;
        try { curChar = input_stream.readChar(); }
        catch (IOException e) { return pos + 1; }
        return ccMoveNfa_0(state, pos + 1);
    }
    static readonly long[] ccbitVec0 = {
        unchecked((long)0xfffffffffffffffeL),
        unchecked((long)0xffffffffffffffffL),
        unchecked((long)0xffffffffffffffffL),
        unchecked((long)0xffffffffffffffffL)
    };
    static readonly long[] ccbitVec2 = {
        0x0L, 0x0L, unchecked((long)0xffffffffffffffffL), unchecked((long)0xffffffffffffffffL)
    };
    private int ccMoveNfa_0(int startState, int curPos)
    {
        int startsAt = 0;
        ccnewStateCnt = 6;
        int i = 1;
        ccstateSet[0] = startState;
        int kind = 0x7fffffff;
        for (; ; )
        {
            if (++ccround == 0x7fffffff)
                ReInitRounds();
            if (curChar < 64)
            {
                long l = 1L << curChar;
                do
                {
                    switch (ccstateSet[--i])
                    {
                        case 3:
                            if (curChar == 36)
                            {
                                if (kind > 21)
                                    kind = 21;
                                ccCheckNAdd(5);
                            }
                            else if (curChar == 47)
                                ccstateSet[ccnewStateCnt++] = 2;
                            break;
                        case 0:
                            if (curChar == 42)
                                ccstateSet[ccnewStateCnt++] = 1;
                            break;
                        case 1:
                            if ((unchecked((long)0xffff7fffffffffffL) & l) != 0L && kind > 7)
                                kind = 7;
                            break;
                        case 2:
                            if (curChar == 42)
                                ccstateSet[ccnewStateCnt++] = 0;
                            break;
                        case 4:
                            if (curChar != 36)
                                break;
                            if (kind > 21)
                                kind = 21;
                            ccCheckNAdd(5);
                            break;
                        case 5:
                            if ((0x3ff001000000000L & l) == 0L)
                                break;
                            if (kind > 21)
                                kind = 21;
                            ccCheckNAdd(5);
                            break;
                        default: break;
                    }
                } while (i != startsAt);
            }
            else if (curChar < 128)
            {
                long l = 1L << (curChar & 077);
                do
                {
                    switch (ccstateSet[--i])
                    {
                        case 3:
                        case 5:
                            if ((0x7fffffe87fffffeL & l) == 0L)
                                break;
                            if (kind > 21)
                                kind = 21;
                            ccCheckNAdd(5);
                            break;
                        case 1:
                            if (kind > 7)
                                kind = 7;
                            break;
                        default: break;
                    }
                } while (i != startsAt);
            }
            else
            {
                int hiByte = (int)(curChar >> 8);
                int i1 = hiByte >> 6;
                long l1 = 1L << (hiByte & 077);
                int i2 = (curChar & 0xff) >> 6;
                long l2 = 1L << (curChar & 077);
                do
                {
                    switch (ccstateSet[--i])
                    {
                        case 1:
                            if (ccCanMove_0(hiByte, i1, i2, l1, l2) && kind > 7)
                                kind = 7;
                            break;
                        default: break;
                    }
                } while (i != startsAt);
            }
            if (kind != 0x7fffffff)
            {
                ccmatchedKind = kind;
                ccmatchedPos = curPos;
                kind = 0x7fffffff;
            }
            ++curPos;
            if ((i = ccnewStateCnt) == (startsAt = 6 - (ccnewStateCnt = startsAt)))
                return curPos;
            try { curChar = input_stream.readChar(); }
            catch (IOException e) { return curPos; }
        }
    }
    private int ccMoveStringLiteralDfa0_3() => (int)curChar switch
    {
        42 => ccMoveStringLiteralDfa1_3(0x800L),
        _ => 1,
    };
    private int ccMoveStringLiteralDfa1_3(long active0)
    {
        try { curChar = input_stream.readChar(); }
        catch (IOException e)
        {
            return 1;
        }
        switch ((int)curChar)
        {
            case 47:
                if ((active0 & 0x800L) != 0L)
                    return ccStopAtPos(1, 11);
                break;
            default:
                return 2;
        }
        return 2;
    }
    private int ccMoveStringLiteralDfa0_1()
    {
        return ccMoveNfa_1(0, 0);
    }
    private int ccMoveNfa_1(int startState, int curPos)
    {
        int startsAt = 0;
        ccnewStateCnt = 3;
        int i = 1;
        ccstateSet[0] = startState;
        int kind = 0x7fffffff;
        for (; ; )
        {
            if (++ccround == 0x7fffffff)
                ReInitRounds();
            if (curChar < 64)
            {
                long l = 1L << curChar;
                do
                {
                    switch (ccstateSet[--i])
                    {
                        case 0:
                            if ((0x2400L & l) != 0L)
                            {
                                if (kind > 9)
                                    kind = 9;
                            }
                            if (curChar == 13)
                                ccstateSet[ccnewStateCnt++] = 1;
                            break;
                        case 1:
                            if (curChar == 10 && kind > 9)
                                kind = 9;
                            break;
                        case 2:
                            if (curChar == 13)
                                ccstateSet[ccnewStateCnt++] = 1;
                            break;
                        default: break;
                    }
                } while (i != startsAt);
            }
            else if (curChar < 128)
            {
                long l = 1L << (curChar & 077);
                do
                {
                    switch (ccstateSet[--i])
                    {
                        default: break;
                    }
                } while (i != startsAt);
            }
            else
            {
                int hiByte = (int)(curChar >> 8);
                int i1 = hiByte >> 6;
                long l1 = 1L << (hiByte & 077);
                int i2 = (curChar & 0xff) >> 6;
                long l2 = 1L << (curChar & 077);
                do
                {
                    switch (ccstateSet[--i])
                    {
                        default: break;
                    }
                } while (i != startsAt);
            }
            if (kind != 0x7fffffff)
            {
                ccmatchedKind = kind;
                ccmatchedPos = curPos;
                kind = 0x7fffffff;
            }
            ++curPos;
            if ((i = ccnewStateCnt) == (startsAt = 3 - (ccnewStateCnt = startsAt)))
                return curPos;
            try { curChar = input_stream.readChar(); }
            catch (IOException e) { return curPos; }
        }
    }
    private int ccMoveStringLiteralDfa0_2() => (int)curChar switch
    {
        42 => ccMoveStringLiteralDfa1_2(0x400L),
        _ => 1,
    };
    private int ccMoveStringLiteralDfa1_2(long active0)
    {
        try { curChar = input_stream.readChar(); }
        catch (IOException)
        {
            return 1;
        }
        switch ((int)curChar)
        {
            case 47:
                if ((active0 & 0x400L) != 0L)
                    return ccStopAtPos(1, 10);
                break;
            default:
                return 2;
        }
        return 2;
    }
    static readonly int[] ccnextStates = Array.Empty<int>();
    private static bool ccCanMove_0(int hiByte, int i1, int i2, long l1, long l2)
    {
        switch (hiByte)
        {
            case 0:
                return ((ccbitVec2[i2] & l2) != 0L);
            default:
                if ((ccbitVec0[i1] & l1) != 0L)
                    return true;
                return false;
        }
    }

    /** Token literal values. */
    public static readonly string[] ccstrLiteralImages = {
        "", null, null, null, null, null, null, null, null, null, null, null, null,
        "\x28", "\x29", "\x21", "\x7c\x7c", "\x26\x26", "\x7e", "\x74\x72\x75\x65",
        "\x66\x61\x6c\x73\x65", null, null, null, };

    /** Lexer state names. */
    public static readonly string[] lexStateNames = {
       "DEFAULT",
       "IN_SINGLE_LINE_COMMENT",
       "IN_FORMAL_COMMENT",
       "IN_MULTI_LINE_COMMENT",
    };

    /** Lex State array. */
    public static readonly int[] ccnewLexState = {
   -1, -1, -1, -1, -1, -1, 1, 2, 3, 0, 0, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
};
    static readonly long[] cctoToken = {
   0x3fe001L,
};
    static readonly long[] cctoSkip = {
   0xe3eL,
};
    static readonly long[] cctoSpecial = {
   0xe00L,
};
    static readonly long[] cctoMore = {
   0x11c0L,
};
    protected CSharpCharStream input_stream;
    private readonly int[] ccrounds = new int[6];
    private readonly int[] ccstateSet = new int[12];
    private readonly StringBuilder jjimage = new();
    private StringBuilder image;
    private int ccimageLen;
    private int lengthOfMatch;
    protected char curChar;
    /** Constructor. */
    public ConditionParserTokenManager(CSharpCharStream stream)
    {
        if (CSharpCharStream.staticFlag)
            throw new Error("ERROR: Cannot use a static CharStream class with a non-static lexical analyzer.");
        input_stream = stream;
    }

    /** Constructor. */
    public ConditionParserTokenManager(CSharpCharStream stream, int lexState)
        : this(stream)
    {
        SwitchTo(lexState);
    }

    /** Reinitialise parser. */
    public void ReInit(CSharpCharStream stream)
    {
        ccmatchedPos = ccnewStateCnt = 0;
        curLexState = defaultLexState;
        input_stream = stream;
        ReInitRounds();
    }
    private void ReInitRounds()
    {
        int i;
        ccround = unchecked((int)0x80000001);
        for (i = 6; i-- > 0;)
            ccrounds[i] = unchecked((int)0x80000000);
    }

    /** Reinitialise parser. */
    public void ReInit(CSharpCharStream stream, int lexState)
    {
        ReInit(stream);
        SwitchTo(lexState);
    }

    /** Switch to specified lex state. */
    public void SwitchTo(int lexState)
    {
        if (lexState >= 4 || lexState < 0)
            throw new TokenMgrError("Error: Ignoring invalid lexical state : " + lexState + ". State unchanged.", TokenMgrError.INVALID_LEXICAL_STATE);
        else
            curLexState = lexState;
    }

    protected Token ccFillToken()
    {
        Token t;
        String curTokenImage;
        int beginLine;
        int endLine;
        int beginColumn;
        int endColumn;
        String im = ccstrLiteralImages[ccmatchedKind];
        curTokenImage = im ?? input_stream.GetImage();
        beginLine = input_stream.getBeginLine();
        beginColumn = input_stream.getBeginColumn();
        endLine = input_stream.getEndLine();
        endColumn = input_stream.getEndColumn();
        t = Token.NewToken(ccmatchedKind, curTokenImage);

        t.beginLine = beginLine;
        t.endLine = endLine;
        t.beginColumn = beginColumn;
        t.endColumn = endColumn;

        return t;
    }

    int curLexState = 0;
    readonly int defaultLexState = 0;
    int ccnewStateCnt;
    int ccround;
    int ccmatchedPos;
    int ccmatchedKind;

    /** Get the next Token. */
    public Token GetNextToken()
    {
        Token specialToken = null;
        Token matchedToken;
        int curPos = 0;

    EOFLoop:
        for (; ; )
        {
            try
            {
                curChar = input_stream.BeginToken();
            }
            catch (IOException e)
            {
                ccmatchedKind = 0;
                matchedToken = ccFillToken();
                matchedToken.specialToken = specialToken;
                return matchedToken;
            }
            image = jjimage;
            image.Capacity = 0;
            ccimageLen = 0;

            for (; ; )
            {
                switch (curLexState)
                {
                    case 0:
                        try
                        {
                            input_stream.backup(0);
                            while (curChar <= 32 && (0x100003600L & (1L << curChar)) != 0L)
                                curChar = input_stream.BeginToken();
                        }
                        catch (IOException e1) { goto EOFLoop; }
                        ccmatchedKind = 0x7fffffff;
                        ccmatchedPos = 0;
                        curPos = ccMoveStringLiteralDfa0_0();
                        break;
                    case 1:
                        ccmatchedKind = 0x7fffffff;
                        ccmatchedPos = 0;
                        curPos = ccMoveStringLiteralDfa0_1();
                        if (ccmatchedPos == 0 && ccmatchedKind > 12)
                        {
                            ccmatchedKind = 12;
                        }
                        break;
                    case 2:
                        ccmatchedKind = 0x7fffffff;
                        ccmatchedPos = 0;
                        curPos = ccMoveStringLiteralDfa0_2();
                        if (ccmatchedPos == 0 && ccmatchedKind > 12)
                        {
                            ccmatchedKind = 12;
                        }
                        break;
                    case 3:
                        ccmatchedKind = 0x7fffffff;
                        ccmatchedPos = 0;
                        curPos = ccMoveStringLiteralDfa0_3();
                        if (ccmatchedPos == 0 && ccmatchedKind > 12)
                        {
                            ccmatchedKind = 12;
                        }
                        break;
                }
                if (ccmatchedKind != 0x7fffffff)
                {
                    if (ccmatchedPos + 1 < curPos)
                        input_stream.backup(curPos - ccmatchedPos - 1);
                    if ((cctoToken[ccmatchedKind >> 6] & (1L << (ccmatchedKind & 077))) != 0L)
                    {
                        matchedToken = ccFillToken();
                        matchedToken.specialToken = specialToken;
                        if (ccnewLexState[ccmatchedKind] != -1)
                            curLexState = ccnewLexState[ccmatchedKind];
                        return matchedToken;
                    }
                    else if ((cctoSkip[ccmatchedKind >> 6] & (1L << (ccmatchedKind & 077))) != 0L)
                    {
                        if ((cctoSpecial[ccmatchedKind >> 6] & (1L << (ccmatchedKind & 077))) != 0L)
                        {
                            matchedToken = ccFillToken();
                            if (specialToken == null)
                                specialToken = matchedToken;
                            else
                            {
                                matchedToken.specialToken = specialToken;
                                specialToken = (specialToken.next = matchedToken);
                            }
                            SkipLexicalActions(matchedToken);
                        }
                        else
                            SkipLexicalActions(null);
                        if (ccnewLexState[ccmatchedKind] != -1)
                            curLexState = ccnewLexState[ccmatchedKind];
                        goto EOFLoop;
                    }
                    MoreLexicalActions();
                    if (ccnewLexState[ccmatchedKind] != -1)
                        curLexState = ccnewLexState[ccmatchedKind];
                    curPos = 0;
                    ccmatchedKind = 0x7fffffff;
                    try
                    {
                        curChar = input_stream.readChar();
                        continue;
                    }
                    catch (IOException e1) { }
                }
                int error_line = input_stream.getEndLine();
                int error_column = input_stream.getEndColumn();
                String error_after = null;
                bool EOFSeen = false;
                try { input_stream.readChar(); input_stream.backup(1); }
                catch (IOException e1)
                {
                    EOFSeen = true;
                    error_after = curPos <= 1 ? "" : input_stream.GetImage();
                    if (curChar == '\n' || curChar == '\r')
                    {
                        error_line++;
                        error_column = 0;
                    }
                    else
                        error_column++;
                }
                if (!EOFSeen)
                {
                    input_stream.backup(1);
                    error_after = curPos <= 1 ? "" : input_stream.GetImage();
                }
                throw new TokenMgrError(EOFSeen, curLexState, error_line, error_column, error_after, curChar, TokenMgrError.LEXICAL_ERROR);
            }
        }
    }

    void SkipLexicalActions(Token matchedToken)
    {
        switch (ccmatchedKind)
        {
            default:
                break;
        }
    }
    void MoreLexicalActions()
    {
        ccimageLen += (lengthOfMatch = ccmatchedPos + 1);
        switch (ccmatchedKind)
        {
            case 7:
                image.Append(input_stream.GetSuffix(ccimageLen));
                ccimageLen = 0;
                input_stream.backup(1);
                break;
            default:
                break;
        }
    }
    private void ccCheckNAdd(int state)
    {
        if (ccrounds[state] != ccround)
        {
            ccstateSet[ccnewStateCnt++] = state;
            ccrounds[state] = ccround;
        }
    }
    private void ccAddStates(int start, int end)
    {
        do
        {
            ccstateSet[ccnewStateCnt++] = ccnextStates[start];
        } while (start++ != end);
    }
    private void ccCheckNAddTwoStates(int state1, int state2)
    {
        ccCheckNAdd(state1);
        ccCheckNAdd(state2);
    }
}
