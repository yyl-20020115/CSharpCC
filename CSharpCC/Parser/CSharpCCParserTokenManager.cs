using CSharpCC.CCTree;
using System.Text;
/**
* This file contains the code for JavaCCParser generated
* by JavaCCParser itself.
*/
namespace CSharpCC.Parser;

/** Token Manager. */
public class CSharpCCParserTokenManager : CSharpCCParserConstants
{
    int[] beginLine = new int[10];
    int[] beginCol = new int[10];
    int depth = 0;
    int size = 10;

    void SaveBeginLineCol(int l, int c)
    {
        if (depth == size)
        {
            size += 5;
            int[] tmpbeginLine = new int[size];
            int[] tmpbeginCol = new int[size];

            Array.Copy(beginLine, 0, beginLine = tmpbeginLine, 0, depth);
            Array.Copy(beginCol, 0, beginCol = tmpbeginCol, 0, depth);
        }

        beginLine[depth] = l;
        beginCol[depth] = c;
        depth++;
    }

    void RestoreBeginLineCol()
    {
        depth--;
        input_stream.AdjustBeginLineColumn(beginLine[depth], beginCol[depth]);
    }

    /** Debug output. */
    public TextWriter debugStream = Console.Out;
    /** Set debug output. */
    public void setDebugStream(TextWriter ds) { debugStream = ds; }
    private int jjStopStringLiteralDfa_0(int pos, ulong active0, ulong active1, ulong active2)
    {
        switch (pos)
        {
            case 0:
                if ((active0 & 0xd40000L) != 0L || (active1 & 0x400000000000000L) != 0L || (active2 & 0x4L) != 0L)
                    return 2;
                if ((active0 & 0xfffffffff0001ffeL) != 0L || (active1 & 0x7ffffL) != 0L || (active2 & 0x1f800L) != 0L)
                {
                    jjmatchedKind = 151;
                    return 35;
                }
                if ((active1 & 0x8000000000L) != 0L || (active2 & 0x20000L) != 0L)
                    return 9;
                return -1;
            case 1:
                if ((active0 & 0xc40000L) != 0L)
                    return 0;
                if ((active0 & 0x8030000000000L) != 0L)
                    return 35;
                if ((active0 & 0xfff7fcfff0001ffeL) != 0L || (active1 & 0x7ffffL) != 0L || (active2 & 0x1f800L) != 0L)
                {
                    if (jjmatchedPos != 1)
                    {
                        jjmatchedKind = 151;
                        jjmatchedPos = 1;
                    }
                    return 35;
                }
                return -1;
            case 2:
                if ((active0 & 0xf675fefff0000ffeL) != 0L || (active1 & 0x7dfffL) != 0L || (active2 & 0x1f800L) != 0L)
                {
                    if (jjmatchedPos != 2)
                    {
                        jjmatchedKind = 151;
                        jjmatchedPos = 2;
                    }
                    return 35;
                }
                if ((active0 & 0x982000000001000L) != 0L || (active1 & 0x2000L) != 0L)
                    return 35;
                return -1;
            case 3:
                if ((active0 & 0x12040c0b00000600L) != 0L || (active1 & 0x5100L) != 0L)
                    return 35;
                if ((active0 & 0xe571f2f4f00009feL) != 0L || (active1 & 0x78effL) != 0L || (active2 & 0x1f800L) != 0L)
                {
                    jjmatchedKind = 151;
                    jjmatchedPos = 3;
                    return 35;
                }
                return -1;
            case 4:
                if ((active0 & 0x1e03480000880L) != 0L || (active1 & 0x10624L) != 0L)
                    return 35;
                if ((active0 & 0xe57012c07000017eL) != 0L || (active1 & 0x688dbL) != 0L || (active2 & 0x1f800L) != 0L)
                {
                    if (jjmatchedPos != 4)
                    {
                        jjmatchedKind = 151;
                        jjmatchedPos = 4;
                    }
                    return 35;
                }
                return -1;
            case 5:
                if ((active0 & 0xe15090c05000097eL) != 0L || (active1 & 0x68890L) != 0L || (active2 & 0x1f800L) != 0L)
                {
                    jjmatchedKind = 151;
                    jjmatchedPos = 5;
                    return 35;
                }
                if ((active0 & 0x420020020000000L) != 0L || (active1 & 0x44bL) != 0L)
                    return 35;
                return -1;
            case 6:
                if ((active0 & 0x815000401000093eL) != 0L || (active1 & 0x68890L) != 0L || (active2 & 0x1f800L) != 0L)
                {
                    jjmatchedKind = 151;
                    jjmatchedPos = 6;
                    return 35;
                }
                if ((active0 & 0x6000908040000040L) != 0L)
                    return 35;
                return -1;
            case 7:
                if ((active0 & 0x4010000020L) != 0L || (active1 & 0x68010L) != 0L)
                    return 35;
                if ((active0 & 0x815000000000091eL) != 0L || (active1 & 0x880L) != 0L || (active2 & 0x1f800L) != 0L)
                {
                    jjmatchedKind = 151;
                    jjmatchedPos = 7;
                    return 35;
                }
                return -1;
            case 8:
                if ((active0 & 0x5000000000091cL) != 0L || (active1 & 0x80L) != 0L || (active2 & 0x1f800L) != 0L)
                {
                    jjmatchedKind = 151;
                    jjmatchedPos = 8;
                    return 35;
                }
                if ((active0 & 0x8100000000000002L) != 0L || (active1 & 0x800L) != 0L)
                    return 35;
                return -1;
            case 9:
                if ((active0 & 0x90cL) != 0L || (active1 & 0x80L) != 0L || (active2 & 0x1f800L) != 0L)
                {
                    jjmatchedKind = 151;
                    jjmatchedPos = 9;
                    return 35;
                }
                if ((active0 & 0x50000000000010L) != 0L)
                    return 35;
                return -1;
            case 10:
                if ((active0 & 0x4L) != 0L)
                    return 35;
                if ((active0 & 0x908L) != 0L || (active1 & 0x80L) != 0L || (active2 & 0x1f800L) != 0L)
                {
                    jjmatchedKind = 151;
                    jjmatchedPos = 10;
                    return 35;
                }
                return -1;
            case 11:
                if ((active0 & 0x900L) != 0L || (active2 & 0x1f800L) != 0L)
                {
                    jjmatchedKind = 151;
                    jjmatchedPos = 11;
                    return 35;
                }
                if ((active0 & 0x8L) != 0L || (active1 & 0x80L) != 0L)
                    return 35;
                return -1;
            case 12:
                if ((active0 & 0x800L) != 0L || (active2 & 0x1f800L) != 0L)
                {
                    jjmatchedKind = 151;
                    jjmatchedPos = 12;
                    return 35;
                }
                if ((active0 & 0x100L) != 0L)
                    return 35;
                return -1;
            case 13:
                if ((active2 & 0x15000L) != 0L)
                    return 35;
                if ((active0 & 0x800L) != 0L || (active2 & 0xa800L) != 0L)
                {
                    jjmatchedKind = 151;
                    jjmatchedPos = 13;
                    return 35;
                }
                return -1;
            case 14:
                if ((active2 & 0xa800L) != 0L)
                {
                    jjmatchedKind = 151;
                    jjmatchedPos = 14;
                    return 35;
                }
                if ((active0 & 0x800L) != 0L)
                    return 35;
                return -1;
            default:
                return -1;
        }
    }
    private int jjStartNfa_0(int pos, ulong active0, ulong active1, ulong active2)
    {
        return jjMoveNfa_0((uint)jjStopStringLiteralDfa_0(pos, active0, active1, active2), pos + 1);
    }
    private int jjStopAtPos(int pos, int kind)
    {
        jjmatchedKind = kind;
        jjmatchedPos = pos;
        return pos + 1;
    }
    private int jjMoveStringLiteralDfa0_0()
    {
        switch ((int)curChar)
        {
            case 33:
                jjmatchedKind = 106;
                return jjMoveStringLiteralDfa1_0(0x0L, 0x4000000000000L, 0x0L);
            case 35:
                return jjStopAtPos(0, 138);
            case 37:
                jjmatchedKind = 126;
                return jjMoveStringLiteralDfa1_0(0x0L, 0x0L, 0x40L);
            case 38:
                jjmatchedKind = 123;
                return jjMoveStringLiteralDfa1_0(0x0L, 0x10000000000000L, 0x8L);
            case 40:
                return jjStopAtPos(0, 95);
            case 41:
                return jjStopAtPos(0, 96);
            case 42:
                jjmatchedKind = 121;
                return jjMoveStringLiteralDfa1_0(0x0L, 0x0L, 0x2L);
            case 43:
                jjmatchedKind = 119;
                return jjMoveStringLiteralDfa1_0(0x0L, 0x8020000000000000L, 0x0L);
            case 44:
                return jjStopAtPos(0, 102);
            case 45:
                jjmatchedKind = 120;
                return jjMoveStringLiteralDfa1_0(0x0L, 0x40000000000000L, 0x1L);
            case 46:
                jjmatchedKind = 103;
                return jjMoveStringLiteralDfa1_0(0x0L, 0x0L, 0x20000L);
            case 47:
                jjmatchedKind = 122;
                return jjMoveStringLiteralDfa1_0(0xd40000L, 0x0L, 0x4L);
            case 58:
                jjmatchedKind = 109;
                return jjMoveStringLiteralDfa1_0(0x0L, 0x400000000000L, 0x0L);
            case 59:
                return jjStopAtPos(0, 101);
            case 60:
                jjmatchedKind = 105;
                return jjMoveStringLiteralDfa1_0(0x0L, 0x1000000000000L, 0x240000L);
            case 61:
                jjmatchedKind = 104;
                return jjMoveStringLiteralDfa1_0(0x0L, 0x800000000000L, 0x0L);
            case 62:
                jjmatchedKind = 137;
                return jjMoveStringLiteralDfa1_0(0x0L, 0x2000000000000L, 0x180180L);
            case 63:
                return jjStopAtPos(0, 108);
            case 64:
                return jjStopAtPos(0, 150);
            case 67:
                return jjMoveStringLiteralDfa1_0(0x40L, 0x0L, 0x0L);
            case 68:
                return jjMoveStringLiteralDfa1_0(0x0L, 0x0L, 0x19800L);
            case 69:
                return jjMoveStringLiteralDfa1_0(0x1000L, 0x0L, 0x0L);
            case 73:
                return jjMoveStringLiteralDfa1_0(0x4L, 0x0L, 0x6000L);
            case 74:
                return jjMoveStringLiteralDfa1_0(0x20L, 0x0L, 0x0L);
            case 76:
                return jjMoveStringLiteralDfa1_0(0x2L, 0x0L, 0x0L);
            case 77:
                return jjMoveStringLiteralDfa1_0(0x200L, 0x0L, 0x0L);
            case 80:
                return jjMoveStringLiteralDfa1_0(0x18L, 0x0L, 0x0L);
            case 83:
                return jjMoveStringLiteralDfa1_0(0x500L, 0x0L, 0x0L);
            case 84:
                return jjMoveStringLiteralDfa1_0(0x880L, 0x0L, 0x0L);
            case 91:
                return jjStopAtPos(0, 99);
            case 93:
                return jjStopAtPos(0, 100);
            case 94:
                jjmatchedKind = 125;
                return jjMoveStringLiteralDfa1_0(0x0L, 0x0L, 0x20L);
            case 97:
                return jjMoveStringLiteralDfa1_0(0x30000000L, 0x0L, 0x0L);
            case 98:
                return jjMoveStringLiteralDfa1_0(0x1c0000000L, 0x0L, 0x0L);
            case 99:
                return jjMoveStringLiteralDfa1_0(0x7e00000000L, 0x0L, 0x0L);
            case 100:
                return jjMoveStringLiteralDfa1_0(0x38000000000L, 0x0L, 0x0L);
            case 101:
                return jjMoveStringLiteralDfa1_0(0x1c0000000000L, 0x0L, 0x0L);
            case 102:
                return jjMoveStringLiteralDfa1_0(0x3e00000000000L, 0x0L, 0x0L);
            case 103:
                return jjMoveStringLiteralDfa1_0(0x4000000000000L, 0x0L, 0x0L);
            case 105:
                return jjMoveStringLiteralDfa1_0(0x1f8000000000000L, 0x0L, 0x0L);
            case 108:
                return jjMoveStringLiteralDfa1_0(0x200000000000000L, 0x0L, 0x0L);
            case 110:
                return jjMoveStringLiteralDfa1_0(0x1c00000000000000L, 0x0L, 0x0L);
            case 112:
                return jjMoveStringLiteralDfa1_0(0xe000000000000000L, 0x1L, 0x0L);
            case 114:
                return jjMoveStringLiteralDfa1_0(0x0L, 0x2L, 0x0L);
            case 115:
                return jjMoveStringLiteralDfa1_0(0x0L, 0xfcL, 0x0L);
            case 116:
                return jjMoveStringLiteralDfa1_0(0x0L, 0x63f00L, 0x0L);
            case 118:
                return jjMoveStringLiteralDfa1_0(0x0L, 0xc000L, 0x0L);
            case 119:
                return jjMoveStringLiteralDfa1_0(0x0L, 0x10000L, 0x0L);
            case 123:
                return jjStopAtPos(0, 97);
            case 124:
                jjmatchedKind = 124;
                return jjMoveStringLiteralDfa1_0(0x0L, 0x8000000000000L, 0x10L);
            case 125:
                return jjStopAtPos(0, 98);
            case 126:
                return jjStopAtPos(0, 107);
            default:
                return jjMoveNfa_0(3, 0);
        }
    }
    private int jjMoveStringLiteralDfa1_0(ulong active0, ulong active1, ulong active2)
    {
        try { curChar = input_stream.ReadChar(); }
        catch (IOException)
        {
            jjStopStringLiteralDfa_0(0, active0, active1, active2);
            return 1;
        }
        switch ((int)curChar)
        {
            case 38:
                if ((active1 & 0x10000000000000L) != 0L)
                    return jjStopAtPos(1, 116);
                break;
            case 42:
                if ((active0 & 0x400000L) != 0L)
                {
                    jjmatchedKind = 22;
                    jjmatchedPos = 1;
                }
                return jjMoveStringLiteralDfa2_0(active0, 0x840000L, active1, 0L, active2, 0L);
            case 43:
                if ((active1 & 0x20000000000000L) != 0L)
                    return jjStopAtPos(1, 117);
                break;
            case 45:
                if ((active1 & 0x40000000000000L) != 0L)
                    return jjStopAtPos(1, 118);
                break;
            case 46:
                return jjMoveStringLiteralDfa2_0(active0, 0L, active1, 0L, active2, 0x20000L);
            case 47:
                if ((active0 & 0x100000L) != 0L)
                    return jjStopAtPos(1, 20);
                break;
            case 58:
                if ((active1 & 0x400000000000L) != 0L)
                    return jjStopAtPos(1, 110);
                break;
            case 60:
                if ((active2 & 0x200000L) != 0L)
                {
                    jjmatchedKind = 149;
                    jjmatchedPos = 1;
                }
                return jjMoveStringLiteralDfa2_0(active0, 0L, active1, 0L, active2, 0x40000L);
            case 61:
                if ((active1 & 0x800000000000L) != 0L)
                    return jjStopAtPos(1, 111);
                else if ((active1 & 0x1000000000000L) != 0L)
                    return jjStopAtPos(1, 112);
                else if ((active1 & 0x2000000000000L) != 0L)
                    return jjStopAtPos(1, 113);
                else if ((active1 & 0x4000000000000L) != 0L)
                    return jjStopAtPos(1, 114);
                else if ((active1 & 0x8000000000000000L) != 0L)
                    return jjStopAtPos(1, 127);
                else if ((active2 & 0x1L) != 0L)
                    return jjStopAtPos(1, 128);
                else if ((active2 & 0x2L) != 0L)
                    return jjStopAtPos(1, 129);
                else if ((active2 & 0x4L) != 0L)
                    return jjStopAtPos(1, 130);
                else if ((active2 & 0x8L) != 0L)
                    return jjStopAtPos(1, 131);
                else if ((active2 & 0x10L) != 0L)
                    return jjStopAtPos(1, 132);
                else if ((active2 & 0x20L) != 0L)
                    return jjStopAtPos(1, 133);
                else if ((active2 & 0x40L) != 0L)
                    return jjStopAtPos(1, 134);
                break;
            case 62:
                if ((active2 & 0x100L) != 0L)
                {
                    jjmatchedKind = 136;
                    jjmatchedPos = 1;
                }
                return jjMoveStringLiteralDfa2_0(active0, 0L, active1, 0L, active2, 0x180080L);
            case 65:
                return jjMoveStringLiteralDfa2_0(active0, 0x38L, active1, 0L, active2, 0L);
            case 67:
                return jjMoveStringLiteralDfa2_0(active0, 0L, active1, 0L, active2, 0x1800L);
            case 69:
                return jjMoveStringLiteralDfa2_0(active0, 0L, active1, 0L, active2, 0x18000L);
            case 71:
                return jjMoveStringLiteralDfa2_0(active0, 0x4L, active1, 0L, active2, 0L);
            case 75:
                return jjMoveStringLiteralDfa2_0(active0, 0x400L, active1, 0L, active2, 0L);
            case 78:
                return jjMoveStringLiteralDfa2_0(active0, 0L, active1, 0L, active2, 0x6000L);
            case 79:
                return jjMoveStringLiteralDfa2_0(active0, 0x1a82L, active1, 0L, active2, 0L);
            case 80:
                return jjMoveStringLiteralDfa2_0(active0, 0x140L, active1, 0L, active2, 0L);
            case 97:
                return jjMoveStringLiteralDfa2_0(active0, 0x2400200600000000L, active1, 0L, active2, 0L);
            case 98:
                return jjMoveStringLiteralDfa2_0(active0, 0x10000000L, active1, 0L, active2, 0L);
            case 101:
                return jjMoveStringLiteralDfa2_0(active0, 0x800008000000000L, active1, 0x20002L, active2, 0L);
            case 102:
                if ((active0 & 0x8000000000000L) != 0L)
                    return jjStartNfaWithStates_0(1, 51, 35);
                break;
            case 104:
                return jjMoveStringLiteralDfa2_0(active0, 0x800000000L, active1, 0x10704L, active2, 0L);
            case 105:
                return jjMoveStringLiteralDfa2_0(active0, 0xc00000000000L, active1, 0L, active2, 0L);
            case 108:
                return jjMoveStringLiteralDfa2_0(active0, 0x1041000000000L, active1, 0L, active2, 0L);
            case 109:
                return jjMoveStringLiteralDfa2_0(active0, 0x30000000000000L, active1, 0L, active2, 0L);
            case 110:
                return jjMoveStringLiteralDfa2_0(active0, 0x1c0080000000000L, active1, 0L, active2, 0L);
            case 111:
                if ((active0 & 0x10000000000L) != 0L)
                {
                    jjmatchedKind = 40;
                    jjmatchedPos = 1;
                }
                return jjMoveStringLiteralDfa2_0(active0, 0x206026040000000L, active1, 0xc000L, active2, 0L);
            case 114:
                return jjMoveStringLiteralDfa2_0(active0, 0xc000000080000000L, active1, 0x3800L, active2, 0L);
            case 115:
                return jjMoveStringLiteralDfa2_0(active0, 0x20000000L, active1, 0L, active2, 0L);
            case 116:
                return jjMoveStringLiteralDfa2_0(active0, 0L, active1, 0x18L, active2, 0L);
            case 117:
                return jjMoveStringLiteralDfa2_0(active0, 0x1000000000000000L, active1, 0x21L, active2, 0L);
            case 119:
                return jjMoveStringLiteralDfa2_0(active0, 0L, active1, 0x40L, active2, 0L);
            case 120:
                return jjMoveStringLiteralDfa2_0(active0, 0x100000000000L, active1, 0L, active2, 0L);
            case 121:
                return jjMoveStringLiteralDfa2_0(active0, 0x100000000L, active1, 0x40080L, active2, 0L);
            case 124:
                if ((active1 & 0x8000000000000L) != 0L)
                    return jjStopAtPos(1, 115);
                break;
            default:
                break;
        }
        return jjStartNfa_0(0, active0, active1, active2);
    }
    private int jjMoveStringLiteralDfa2_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return jjStartNfa_0(0, old0, old1, old2);
        try { curChar = input_stream.ReadChar(); }
        catch (IOException)
        {
            jjStopStringLiteralDfa_0(1, active0, active1, active2);
            return 2;
        }
        switch ((int)curChar)
        {
            case 46:
                if ((active2 & 0x20000L) != 0L)
                    return jjStopAtPos(2, 145);
                break;
            case 61:
                if ((active2 & 0x40000L) != 0L)
                    return jjStopAtPos(2, 146);
                else if ((active2 & 0x80000L) != 0L)
                    return jjStopAtPos(2, 147);
                break;
            case 62:
                if ((active2 & 0x80L) != 0L)
                {
                    jjmatchedKind = 135;
                    jjmatchedPos = 2;
                }
                return jjMoveStringLiteralDfa3_0(active0, 0L, active1, 0L, active2, 0x100000L);
            case 64:
                return jjMoveStringLiteralDfa3_0(active0, 0x840000L, active1, 0L, active2, 0L);
            case 67:
                return jjMoveStringLiteralDfa3_0(active0, 0L, active1, 0L, active2, 0x6000L);
            case 69:
                return jjMoveStringLiteralDfa3_0(active0, 0x100L, active1, 0L, active2, 0L);
            case 70:
                if ((active0 & 0x1000L) != 0L)
                    return jjStartNfaWithStates_0(2, 12, 35);
                return jjMoveStringLiteralDfa3_0(active0, 0L, active1, 0L, active2, 0x18000L);
            case 73:
                return jjMoveStringLiteralDfa3_0(active0, 0x400L, active1, 0L, active2, 0L);
            case 75:
                return jjMoveStringLiteralDfa3_0(active0, 0x880L, active1, 0L, active2, 0L);
            case 76:
                return jjMoveStringLiteralDfa3_0(active0, 0L, active1, 0L, active2, 0x1800L);
            case 78:
                return jjMoveStringLiteralDfa3_0(active0, 0x4L, active1, 0L, active2, 0L);
            case 79:
                return jjMoveStringLiteralDfa3_0(active0, 0x2L, active1, 0L, active2, 0L);
            case 80:
                return jjMoveStringLiteralDfa3_0(active0, 0x40L, active1, 0L, active2, 0L);
            case 82:
                return jjMoveStringLiteralDfa3_0(active0, 0x218L, active1, 0L, active2, 0L);
            case 86:
                return jjMoveStringLiteralDfa3_0(active0, 0x20L, active1, 0L, active2, 0L);
            case 97:
                return jjMoveStringLiteralDfa3_0(active0, 0x1800000000L, active1, 0x808L, active2, 0L);
            case 98:
                return jjMoveStringLiteralDfa3_0(active0, 0L, active1, 0x1L, active2, 0L);
            case 99:
                return jjMoveStringLiteralDfa3_0(active0, 0x2000000000000000L, active1, 0L, active2, 0L);
            case 101:
                return jjMoveStringLiteralDfa3_0(active0, 0x80000000L, active1, 0L, active2, 0L);
            case 102:
                return jjMoveStringLiteralDfa3_0(active0, 0x8000000000L, active1, 0L, active2, 0L);
            case 105:
                return jjMoveStringLiteralDfa3_0(active0, 0x4000000000000000L, active1, 0x14140L, active2, 0L);
            case 108:
                return jjMoveStringLiteralDfa3_0(active0, 0x1000200000000000L, active1, 0x8000L, active2, 0L);
            case 109:
                return jjMoveStringLiteralDfa3_0(active0, 0L, active1, 0x20000L, active2, 0L);
            case 110:
                return jjMoveStringLiteralDfa3_0(active0, 0x200c06000000000L, active1, 0x80L, active2, 0L);
            case 111:
                return jjMoveStringLiteralDfa3_0(active0, 0x8001000040000000L, active1, 0x4L, active2, 0L);
            case 112:
                return jjMoveStringLiteralDfa3_0(active0, 0x30000000000000L, active1, 0x40020L, active2, 0L);
            case 114:
                if ((active0 & 0x2000000000000L) != 0L)
                    return jjStartNfaWithStates_0(2, 49, 35);
                return jjMoveStringLiteralDfa3_0(active0, 0L, active1, 0x610L, active2, 0L);
            case 115:
                return jjMoveStringLiteralDfa3_0(active0, 0x40040230000000L, active1, 0L, active2, 0L);
            case 116:
                if ((active0 & 0x80000000000000L) != 0L)
                {
                    jjmatchedKind = 55;
                    jjmatchedPos = 2;
                }
                return jjMoveStringLiteralDfa3_0(active0, 0x504100500000000L, active1, 0x2L, active2, 0L);
            case 117:
                return jjMoveStringLiteralDfa3_0(active0, 0xa0000000000L, active1, 0x1000L, active2, 0L);
            case 119:
                if ((active0 & 0x800000000000000L) != 0L)
                    return jjStartNfaWithStates_0(2, 59, 35);
                break;
            case 121:
                if ((active1 & 0x2000L) != 0L)
                    return jjStartNfaWithStates_0(2, 77, 35);
                break;
            default:
                break;
        }
        return jjStartNfa_0(1, active0, active1, active2);
    }
    private int jjMoveStringLiteralDfa3_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return jjStartNfa_0(1, old0, old1, old2);
        try { curChar = input_stream.ReadChar(); }
        catch (IOException)
        {
            jjStopStringLiteralDfa_0(2, active0, active1, active2);
            return 3;
        }
        switch ((int)curChar)
        {
            case 61:
                if ((active2 & 0x100000L) != 0L)
                    return jjStopAtPos(3, 148);
                break;
            case 65:
                return jjMoveStringLiteralDfa4_0(active0, 0x20L, active1, 0L, active2, 0L);
            case 67:
                return jjMoveStringLiteralDfa4_0(active0, 0x140L, active1, 0L, active2, 0L);
            case 69:
                if ((active0 & 0x200L) != 0L)
                    return jjStartNfaWithStates_0(3, 9, 35);
                return jjMoveStringLiteralDfa4_0(active0, 0x880L, active1, 0L, active2, 0L);
            case 75:
                return jjMoveStringLiteralDfa4_0(active0, 0x2L, active1, 0L, active2, 0L);
            case 79:
                return jjMoveStringLiteralDfa4_0(active0, 0x4L, active1, 0L, active2, 0L);
            case 80:
                if ((active0 & 0x400L) != 0L)
                    return jjStartNfaWithStates_0(3, 10, 35);
                break;
            case 83:
                return jjMoveStringLiteralDfa4_0(active0, 0x18L, active1, 0L, active2, 0L);
            case 95:
                return jjMoveStringLiteralDfa4_0(active0, 0L, active1, 0L, active2, 0x1f800L);
            case 97:
                return jjMoveStringLiteralDfa4_0(active0, 0x1c08080000000L, active1, 0x8000L, active2, 0L);
            case 98:
                return jjMoveStringLiteralDfa4_0(active0, 0x20000800000L, active1, 0L, active2, 0L);
            case 99:
                return jjMoveStringLiteralDfa4_0(active0, 0x400000000L, active1, 0x80L, active2, 0L);
            case 100:
                if ((active1 & 0x4000L) != 0L)
                    return jjStartNfaWithStates_0(3, 78, 35);
                break;
            case 101:
                if ((active0 & 0x100000000L) != 0L)
                    return jjStartNfaWithStates_0(3, 32, 35);
                else if ((active0 & 0x200000000L) != 0L)
                    return jjStartNfaWithStates_0(3, 33, 35);
                else if ((active0 & 0x40000000000L) != 0L)
                    return jjStartNfaWithStates_0(3, 42, 35);
                else if ((active1 & 0x1000L) != 0L)
                    return jjStartNfaWithStates_0(3, 76, 35);
                return jjMoveStringLiteralDfa4_0(active0, 0x100100020040000L, active1, 0x40020L, active2, 0L);
            case 103:
                if ((active0 & 0x200000000000000L) != 0L)
                    return jjStartNfaWithStates_0(3, 57, 35);
                break;
            case 105:
                return jjMoveStringLiteralDfa4_0(active0, 0x400000000000000L, active1, 0x10L, active2, 0L);
            case 107:
                return jjMoveStringLiteralDfa4_0(active0, 0x2000000000000000L, active1, 0L, active2, 0L);
            case 108:
                if ((active0 & 0x1000000000000000L) != 0L)
                    return jjStartNfaWithStates_0(3, 60, 35);
                return jjMoveStringLiteralDfa4_0(active0, 0x10000040000000L, active1, 0x10001L, active2, 0L);
            case 109:
                if ((active0 & 0x80000000000L) != 0L)
                    return jjStartNfaWithStates_0(3, 43, 35);
                break;
            case 110:
                return jjMoveStringLiteralDfa4_0(active0, 0L, active1, 0x800L, active2, 0L);
            case 111:
                if ((active0 & 0x4000000000000L) != 0L)
                    return jjStartNfaWithStates_0(3, 50, 35);
                return jjMoveStringLiteralDfa4_0(active0, 0x20000000000000L, active1, 0x600L, active2, 0L);
            case 112:
                return jjMoveStringLiteralDfa4_0(active0, 0L, active1, 0x20000L, active2, 0L);
            case 114:
                if ((active0 & 0x800000000L) != 0L)
                    return jjStartNfaWithStates_0(3, 35, 35);
                return jjMoveStringLiteralDfa4_0(active0, 0L, active1, 0x4L, active2, 0L);
            case 115:
                if ((active1 & 0x100L) != 0L)
                    return jjStartNfaWithStates_0(3, 72, 35);
                return jjMoveStringLiteralDfa4_0(active0, 0x203000000000L, active1, 0L, active2, 0L);
            case 116:
                return jjMoveStringLiteralDfa4_0(active0, 0x8040004010000000L, active1, 0x48L, active2, 0L);
            case 117:
                return jjMoveStringLiteralDfa4_0(active0, 0L, active1, 0x2L, active2, 0L);
            case 118:
                return jjMoveStringLiteralDfa4_0(active0, 0x4000000000000000L, active1, 0L, active2, 0L);
            default:
                break;
        }
        return jjStartNfa_0(2, active0, active1, active2);
    }
    private int jjMoveStringLiteralDfa4_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return jjStartNfa_0(2, old0, old1, old2);
        try { curChar = input_stream.ReadChar(); }
        catch (IOException)
        {
            jjStopStringLiteralDfa_0(3, active0, active1, active2);
            return 4;
        }
        switch ((int)curChar)
        {
            case 65:
                return jjMoveStringLiteralDfa5_0(active0, 0x2L, active1, 0L, active2, 0L);
            case 67:
                return jjMoveStringLiteralDfa5_0(active0, 0x20L, active1, 0L, active2, 0L);
            case 69:
                return jjMoveStringLiteralDfa5_0(active0, 0x18L, active1, 0L, active2, 0L);
            case 73:
                return jjMoveStringLiteralDfa5_0(active0, 0x100L, active1, 0L, active2, 0L);
            case 78:
                if ((active0 & 0x80L) != 0L)
                {
                    jjmatchedKind = 7;
                    jjmatchedPos = 4;
                }
                return jjMoveStringLiteralDfa5_0(active0, 0x800L, active1, 0L, active2, 0L);
            case 79:
                return jjMoveStringLiteralDfa5_0(active0, 0x40L, active1, 0L, active2, 0L);
            case 80:
                return jjMoveStringLiteralDfa5_0(active0, 0L, active1, 0L, active2, 0x1f800L);
            case 82:
                return jjMoveStringLiteralDfa5_0(active0, 0x4L, active1, 0L, active2, 0L);
            case 97:
                return jjMoveStringLiteralDfa5_0(active0, 0x6040000000000000L, active1, 0L, active2, 0L);
            case 99:
                return jjMoveStringLiteralDfa5_0(active0, 0L, active1, 0x50L, active2, 0L);
            case 101:
                if ((active0 & 0x200000000000L) != 0L)
                    return jjStartNfaWithStates_0(4, 45, 35);
                else if ((active1 & 0x10000L) != 0L)
                    return jjStartNfaWithStates_0(4, 80, 35);
                return jjMoveStringLiteralDfa5_0(active0, 0x8010000040000000L, active1, 0L, active2, 0L);
            case 103:
                return jjMoveStringLiteralDfa5_0(active0, 0x840000L, active1, 0L, active2, 0L);
            case 104:
                if ((active0 & 0x400000000L) != 0L)
                    return jjStartNfaWithStates_0(4, 34, 35);
                return jjMoveStringLiteralDfa5_0(active0, 0L, active1, 0x80L, active2, 0L);
            case 105:
                return jjMoveStringLiteralDfa5_0(active0, 0x4000000000L, active1, 0x9L, active2, 0L);
            case 107:
                if ((active0 & 0x80000000L) != 0L)
                    return jjStartNfaWithStates_0(4, 31, 35);
                break;
            case 108:
                if ((active0 & 0x400000000000L) != 0L)
                {
                    jjmatchedKind = 46;
                    jjmatchedPos = 4;
                }
                return jjMoveStringLiteralDfa5_0(active0, 0x820000000000L, active1, 0x20000L, active2, 0L);
            case 110:
                return jjMoveStringLiteralDfa5_0(active0, 0x100000000000L, active1, 0x40000L, active2, 0L);
            case 114:
                if ((active1 & 0x20L) != 0L)
                    return jjStartNfaWithStates_0(4, 69, 35);
                return jjMoveStringLiteralDfa5_0(active0, 0x120000030000000L, active1, 0x2L, active2, 0L);
            case 115:
                if ((active0 & 0x1000000000L) != 0L)
                    return jjStartNfaWithStates_0(4, 36, 35);
                return jjMoveStringLiteralDfa5_0(active0, 0L, active1, 0x800L, active2, 0L);
            case 116:
                if ((active0 & 0x2000000000L) != 0L)
                    return jjStartNfaWithStates_0(4, 37, 35);
                else if ((active0 & 0x1000000000000L) != 0L)
                    return jjStartNfaWithStates_0(4, 48, 35);
                else if ((active1 & 0x4L) != 0L)
                    return jjStartNfaWithStates_0(4, 66, 35);
                return jjMoveStringLiteralDfa5_0(active0, 0L, active1, 0x8000L, active2, 0L);
            case 117:
                return jjMoveStringLiteralDfa5_0(active0, 0x8000000000L, active1, 0L, active2, 0L);
            case 118:
                return jjMoveStringLiteralDfa5_0(active0, 0x400000000000000L, active1, 0L, active2, 0L);
            case 119:
                if ((active1 & 0x200L) != 0L)
                {
                    jjmatchedKind = 73;
                    jjmatchedPos = 4;
                }
                return jjMoveStringLiteralDfa5_0(active0, 0L, active1, 0x400L, active2, 0L);
            default:
                break;
        }
        return jjStartNfa_0(3, active0, active1, active2);
    }
    private int jjMoveStringLiteralDfa5_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return jjStartNfa_0(3, old0, old1, old2);
        try { curChar = input_stream.ReadChar(); }
        catch (IOException)
        {
            jjStopStringLiteralDfa_0(4, active0, active1, active2);
            return 5;
        }
        switch ((int)curChar)
        {
            case 65:
                return jjMoveStringLiteralDfa6_0(active0, 0x100L, active1, 0L, active2, 0x1f800L);
            case 68:
                return jjMoveStringLiteralDfa6_0(active0, 0x40L, active1, 0L, active2, 0L);
            case 69:
                return jjMoveStringLiteralDfa6_0(active0, 0x4L, active1, 0L, active2, 0L);
            case 72:
                return jjMoveStringLiteralDfa6_0(active0, 0x2L, active1, 0L, active2, 0L);
            case 79:
                return jjMoveStringLiteralDfa6_0(active0, 0x20L, active1, 0L, active2, 0L);
            case 82:
                return jjMoveStringLiteralDfa6_0(active0, 0x18L, active1, 0L, active2, 0L);
            case 95:
                return jjMoveStringLiteralDfa6_0(active0, 0x800L, active1, 0L, active2, 0L);
            case 97:
                return jjMoveStringLiteralDfa6_0(active0, 0x50000000L, active1, 0x60000L, active2, 0L);
            case 99:
                if ((active1 & 0x1L) != 0L)
                    return jjStartNfaWithStates_0(5, 64, 35);
                else if ((active1 & 0x8L) != 0L)
                    return jjStartNfaWithStates_0(5, 67, 35);
                return jjMoveStringLiteralDfa6_0(active0, 0x8000000000000000L, active1, 0L, active2, 0L);
            case 100:
                return jjMoveStringLiteralDfa6_0(active0, 0x100000000000L, active1, 0L, active2, 0L);
            case 101:
                if ((active0 & 0x20000000000L) != 0L)
                    return jjStartNfaWithStates_0(5, 41, 35);
                else if ((active0 & 0x400000000000000L) != 0L)
                    return jjStartNfaWithStates_0(5, 58, 35);
                return jjMoveStringLiteralDfa6_0(active0, 0x840000L, active1, 0L, active2, 0L);
            case 102:
                return jjMoveStringLiteralDfa6_0(active0, 0x100000000000000L, active1, 0L, active2, 0L);
            case 103:
                return jjMoveStringLiteralDfa6_0(active0, 0x2000000000000000L, active1, 0L, active2, 0L);
            case 104:
                if ((active1 & 0x40L) != 0L)
                    return jjStartNfaWithStates_0(5, 70, 35);
                break;
            case 105:
                return jjMoveStringLiteralDfa6_0(active0, 0L, active1, 0x8800L, active2, 0L);
            case 108:
                return jjMoveStringLiteralDfa6_0(active0, 0x808000000000L, active1, 0L, active2, 0L);
            case 109:
                return jjMoveStringLiteralDfa6_0(active0, 0x10000000000000L, active1, 0L, active2, 0L);
            case 110:
                if ((active1 & 0x2L) != 0L)
                    return jjStartNfaWithStates_0(5, 65, 35);
                return jjMoveStringLiteralDfa6_0(active0, 0x40004000000000L, active1, 0L, active2, 0L);
            case 114:
                return jjMoveStringLiteralDfa6_0(active0, 0L, active1, 0x80L, active2, 0L);
            case 115:
                if ((active1 & 0x400L) != 0L)
                    return jjStartNfaWithStates_0(5, 74, 35);
                break;
            case 116:
                if ((active0 & 0x20000000L) != 0L)
                    return jjStartNfaWithStates_0(5, 29, 35);
                else if ((active0 & 0x20000000000000L) != 0L)
                    return jjStartNfaWithStates_0(5, 53, 35);
                return jjMoveStringLiteralDfa6_0(active0, 0x4000000000000000L, active1, 0x10L, active2, 0L);
            default:
                break;
        }
        return jjStartNfa_0(4, active0, active1, active2);
    }
    private int jjMoveStringLiteralDfa6_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return jjStartNfa_0(4, old0, old1, old2);
        try { curChar = input_stream.ReadChar(); }
        catch (IOException)
        {
            jjStopStringLiteralDfa_0(5, active0, active1, active2);
            return 6;
        }
        switch ((int)curChar)
        {
            case 68:
                return jjMoveStringLiteralDfa7_0(active0, 0x20L, active1, 0L, active2, 0L);
            case 69:
                if ((active0 & 0x40L) != 0L)
                    return jjStartNfaWithStates_0(6, 6, 35);
                return jjMoveStringLiteralDfa7_0(active0, 0x2L, active1, 0L, active2, 0L);
            case 76:
                return jjMoveStringLiteralDfa7_0(active0, 0x100L, active1, 0L, active2, 0L);
            case 77:
                return jjMoveStringLiteralDfa7_0(active0, 0x800L, active1, 0L, active2, 0L);
            case 82:
                return jjMoveStringLiteralDfa7_0(active0, 0L, active1, 0L, active2, 0x1f800L);
            case 95:
                return jjMoveStringLiteralDfa7_0(active0, 0x1cL, active1, 0L, active2, 0L);
            case 97:
                return jjMoveStringLiteralDfa7_0(active0, 0x100000000000000L, active1, 0L, active2, 0L);
            case 99:
                return jjMoveStringLiteralDfa7_0(active0, 0x40000010000000L, active1, 0L, active2, 0L);
            case 101:
                if ((active0 & 0x2000000000000000L) != 0L)
                    return jjStartNfaWithStates_0(6, 61, 35);
                else if ((active0 & 0x4000000000000000L) != 0L)
                    return jjStartNfaWithStates_0(6, 62, 35);
                return jjMoveStringLiteralDfa7_0(active0, 0x10000000000000L, active1, 0x800L, active2, 0L);
            case 102:
                return jjMoveStringLiteralDfa7_0(active0, 0L, active1, 0x10L, active2, 0L);
            case 108:
                return jjMoveStringLiteralDfa7_0(active0, 0L, active1, 0x8000L, active2, 0L);
            case 109:
                return jjMoveStringLiteralDfa7_0(active0, 0L, active1, 0x40000L, active2, 0L);
            case 110:
                if ((active0 & 0x40000000L) != 0L)
                    return jjStartNfaWithStates_0(6, 30, 35);
                return jjMoveStringLiteralDfa7_0(active0, 0x840000L, active1, 0L, active2, 0L);
            case 111:
                return jjMoveStringLiteralDfa7_0(active0, 0L, active1, 0x80L, active2, 0L);
            case 115:
                if ((active0 & 0x100000000000L) != 0L)
                    return jjStartNfaWithStates_0(6, 44, 35);
                break;
            case 116:
                if ((active0 & 0x8000000000L) != 0L)
                    return jjStartNfaWithStates_0(6, 39, 35);
                return jjMoveStringLiteralDfa7_0(active0, 0x8000000000000000L, active1, 0x20000L, active2, 0L);
            case 117:
                return jjMoveStringLiteralDfa7_0(active0, 0x4000000000L, active1, 0L, active2, 0L);
            case 121:
                if ((active0 & 0x800000000000L) != 0L)
                    return jjStartNfaWithStates_0(6, 47, 35);
                break;
            default:
                break;
        }
        return jjStartNfa_0(5, active0, active1, active2);
    }
    private int jjMoveStringLiteralDfa7_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return jjStartNfa_0(5, old0, old1, old2);
        try { curChar = input_stream.ReadChar(); }
        catch (IOException)
        {
            jjStopStringLiteralDfa_0(6, active0, active1, active2);
            return 7;
        }
        switch ((int)curChar)
        {
            case 40:
                return jjMoveStringLiteralDfa8_0(active0, 0x800000L, active1, 0L, active2, 0L);
            case 42:
                return jjMoveStringLiteralDfa8_0(active0, 0x40000L, active1, 0L, active2, 0L);
            case 65:
                return jjMoveStringLiteralDfa8_0(active0, 0x2L, active1, 0L, active2, 0L);
            case 66:
                return jjMoveStringLiteralDfa8_0(active0, 0x8L, active1, 0L, active2, 0L);
            case 67:
                return jjMoveStringLiteralDfa8_0(active0, 0x4L, active1, 0L, active2, 0L);
            case 69:
                if ((active0 & 0x20L) != 0L)
                    return jjStartNfaWithStates_0(7, 5, 35);
                return jjMoveStringLiteralDfa8_0(active0, 0x10L, active1, 0L, active2, 0L);
            case 71:
                return jjMoveStringLiteralDfa8_0(active0, 0x800L, active1, 0L, active2, 0L);
            case 83:
                return jjMoveStringLiteralDfa8_0(active0, 0L, active1, 0L, active2, 0x1f800L);
            case 95:
                return jjMoveStringLiteralDfa8_0(active0, 0x100L, active1, 0L, active2, 0L);
            case 99:
                return jjMoveStringLiteralDfa8_0(active0, 0x100000000000000L, active1, 0L, active2, 0L);
            case 101:
                if ((active0 & 0x4000000000L) != 0L)
                    return jjStartNfaWithStates_0(7, 38, 35);
                else if ((active1 & 0x8000L) != 0L)
                    return jjStartNfaWithStates_0(7, 79, 35);
                else if ((active1 & 0x20000L) != 0L)
                    return jjStartNfaWithStates_0(7, 81, 35);
                else if ((active1 & 0x40000L) != 0L)
                    return jjStartNfaWithStates_0(7, 82, 35);
                return jjMoveStringLiteralDfa8_0(active0, 0x8040000000000000L, active1, 0L, active2, 0L);
            case 110:
                return jjMoveStringLiteralDfa8_0(active0, 0x10000000000000L, active1, 0x880L, active2, 0L);
            case 112:
                if ((active1 & 0x10L) != 0L)
                    return jjStartNfaWithStates_0(7, 68, 35);
                break;
            case 116:
                if ((active0 & 0x10000000L) != 0L)
                    return jjStartNfaWithStates_0(7, 28, 35);
                break;
            default:
                break;
        }
        return jjStartNfa_0(6, active0, active1, active2);
    }
    private int jjMoveStringLiteralDfa8_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return jjStartNfa_0(6, old0, old1, old2);
        try { curChar = input_stream.ReadChar(); }
        catch (IOException)
        {
            jjStopStringLiteralDfa_0(7, active0, active1, active2);
            return 8;
        }
        switch ((int)curChar)
        {
            case 47:
                if ((active0 & 0x40000L) != 0L)
                    return jjStopAtPos(8, 18);
                break;
            case 65:
                return jjMoveStringLiteralDfa9_0(active0, 0x4L, active1, 0L, active2, 0L);
            case 68:
                if ((active0 & 0x2L) != 0L)
                    return jjStartNfaWithStates_0(8, 1, 35);
                break;
            case 69:
                return jjMoveStringLiteralDfa9_0(active0, 0x8L, active1, 0L, active2, 0x1f800L);
            case 78:
                return jjMoveStringLiteralDfa9_0(active0, 0x10L, active1, 0L, active2, 0L);
            case 82:
                return jjMoveStringLiteralDfa9_0(active0, 0x800L, active1, 0L, active2, 0L);
            case 84:
                return jjMoveStringLiteralDfa9_0(active0, 0x100L, active1, 0L, active2, 0L);
            case 100:
                if ((active0 & 0x8000000000000000L) != 0L)
                    return jjStartNfaWithStates_0(8, 63, 35);
                break;
            case 101:
                if ((active0 & 0x100000000000000L) != 0L)
                    return jjStartNfaWithStates_0(8, 56, 35);
                break;
            case 105:
                return jjMoveStringLiteralDfa9_0(active0, 0L, active1, 0x80L, active2, 0L);
            case 106:
                return jjMoveStringLiteralDfa9_0(active0, 0x800000L, active1, 0L, active2, 0L);
            case 111:
                return jjMoveStringLiteralDfa9_0(active0, 0x40000000000000L, active1, 0L, active2, 0L);
            case 116:
                if ((active1 & 0x800L) != 0L)
                    return jjStartNfaWithStates_0(8, 75, 35);
                return jjMoveStringLiteralDfa9_0(active0, 0x10000000000000L, active1, 0L, active2, 0L);
            default:
                break;
        }
        return jjStartNfa_0(7, active0, active1, active2);
    }
    private int jjMoveStringLiteralDfa9_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return jjStartNfa_0(7, old0, old1, old2);
        try { curChar = input_stream.ReadChar(); }
        catch (IOException)
        {
            jjStopStringLiteralDfa_0(8, active0, active1, active2);
            return 9;
        }
        switch ((int)curChar)
        {
            case 68:
                if ((active0 & 0x10L) != 0L)
                    return jjStartNfaWithStates_0(9, 4, 35);
                break;
            case 71:
                return jjMoveStringLiteralDfa10_0(active0, 0x8L, active1, 0L, active2, 0L);
            case 79:
                return jjMoveStringLiteralDfa10_0(active0, 0x100L, active1, 0L, active2, 0L);
            case 82:
                return jjMoveStringLiteralDfa10_0(active0, 0L, active1, 0L, active2, 0x1f800L);
            case 83:
                return jjMoveStringLiteralDfa10_0(active0, 0x4L, active1, 0L, active2, 0L);
            case 95:
                return jjMoveStringLiteralDfa10_0(active0, 0x800L, active1, 0L, active2, 0L);
            case 102:
                if ((active0 & 0x40000000000000L) != 0L)
                    return jjStartNfaWithStates_0(9, 54, 35);
                break;
            case 106:
                return jjMoveStringLiteralDfa10_0(active0, 0x800000L, active1, 0L, active2, 0L);
            case 115:
                if ((active0 & 0x10000000000000L) != 0L)
                    return jjStartNfaWithStates_0(9, 52, 35);
                break;
            case 122:
                return jjMoveStringLiteralDfa10_0(active0, 0L, active1, 0x80L, active2, 0L);
            default:
                break;
        }
        return jjStartNfa_0(8, active0, active1, active2);
    }
    private int jjMoveStringLiteralDfa10_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return jjStartNfa_0(8, old0, old1, old2);
        try { curChar = input_stream.ReadChar(); }
        catch (IOException)
        {
            jjStopStringLiteralDfa_0(9, active0, active1, active2);
            return 10;
        }
        switch ((int)curChar)
        {
            case 68:
                return jjMoveStringLiteralDfa11_0(active0, 0x800L, active1, 0L, active2, 0L);
            case 69:
                if ((active0 & 0x4L) != 0L)
                    return jjStartNfaWithStates_0(10, 2, 35);
                break;
            case 73:
                return jjMoveStringLiteralDfa11_0(active0, 0x8L, active1, 0L, active2, 0L);
            case 75:
                return jjMoveStringLiteralDfa11_0(active0, 0x100L, active1, 0L, active2, 0L);
            case 95:
                return jjMoveStringLiteralDfa11_0(active0, 0L, active1, 0L, active2, 0x1f800L);
            case 101:
                return jjMoveStringLiteralDfa11_0(active0, 0L, active1, 0x80L, active2, 0L);
            case 116:
                return jjMoveStringLiteralDfa11_0(active0, 0x800000L, active1, 0L, active2, 0L);
            default:
                break;
        }
        return jjStartNfa_0(9, active0, active1, active2);
    }
    private int jjMoveStringLiteralDfa11_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return jjStartNfa_0(9, old0, old1, old2);
        try { curChar = input_stream.ReadChar(); }
        catch (IOException)
        {
            jjStopStringLiteralDfa_0(10, active0, active1, active2);
            return 11;
        }
        switch ((int)curChar)
        {
            case 66:
                return jjMoveStringLiteralDfa12_0(active0, 0L, active1, 0L, active2, 0xa800L);
            case 69:
                return jjMoveStringLiteralDfa12_0(active0, 0x900L, active1, 0L, active2, 0x15000L);
            case 78:
                if ((active0 & 0x8L) != 0L)
                    return jjStartNfaWithStates_0(11, 3, 35);
                break;
            case 100:
                if ((active1 & 0x80L) != 0L)
                    return jjStartNfaWithStates_0(11, 71, 35);
                break;
            case 114:
                return jjMoveStringLiteralDfa12_0(active0, 0x800000L, active1, 0L, active2, 0L);
            default:
                break;
        }
        return jjStartNfa_0(10, active0, active1, active2);
    }
    private int jjMoveStringLiteralDfa12_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return jjStartNfa_0(10, old0, old1, old2);
        try { curChar = input_stream.ReadChar(); }
        catch (IOException)
        {
            jjStopStringLiteralDfa_0(11, active0, 0L, active2);
            return 12;
        }
        switch ((int)curChar)
        {
            case 67:
                return jjMoveStringLiteralDfa13_0(active0, 0x800L, active2, 0L);
            case 69:
                return jjMoveStringLiteralDfa13_0(active0, 0L, active2, 0xa800L);
            case 78:
                if ((active0 & 0x100L) != 0L)
                    return jjStartNfaWithStates_0(12, 8, 35);
                return jjMoveStringLiteralDfa13_0(active0, 0L, active2, 0x15000L);
            case 101:
                return jjMoveStringLiteralDfa13_0(active0, 0x800000L, active2, 0L);
            default:
                break;
        }
        return jjStartNfa_0(11, active0, 0L, active2);
    }
    private int jjMoveStringLiteralDfa13_0(ulong old0, ulong active0, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active2 &= old2)) == 0L)
            return jjStartNfa_0(11, old0, 0L, old2);
        try { curChar = input_stream.ReadChar(); }
        catch (IOException)
        {
            jjStopStringLiteralDfa_0(12, active0, 0L, active2);
            return 13;
        }
        switch ((int)curChar)
        {
            case 68:
                if ((active2 & 0x1000L) != 0L)
                    return jjStartNfaWithStates_0(13, 140, 35);
                else if ((active2 & 0x4000L) != 0L)
                    return jjStartNfaWithStates_0(13, 142, 35);
                else if ((active2 & 0x10000L) != 0L)
                    return jjStartNfaWithStates_0(13, 144, 35);
                break;
            case 71:
                return jjMoveStringLiteralDfa14_0(active0, 0L, active2, 0xa800L);
            case 76:
                return jjMoveStringLiteralDfa14_0(active0, 0x800L, active2, 0L);
            case 101:
                if ((active0 & 0x800000L) != 0L)
                    return jjStopAtPos(13, 23);
                break;
            default:
                break;
        }
        return jjStartNfa_0(12, active0, 0L, active2);
    }
    private int jjMoveStringLiteralDfa14_0(ulong old0, ulong active0, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active2 &= old2)) == 0L)
            return jjStartNfa_0(12, old0, 0L, old2);
        try { curChar = input_stream.ReadChar(); }
        catch (IOException)
        {
            jjStopStringLiteralDfa_0(13, active0, 0L, active2);
            return 14;
        }
        switch ((int)curChar)
        {
            case 73:
                return jjMoveStringLiteralDfa15_0(active0, 0L, active2, 0xa800L);
            case 83:
                if ((active0 & 0x800L) != 0L)
                    return jjStartNfaWithStates_0(14, 11, 35);
                break;
            default:
                break;
        }
        return jjStartNfa_0(13, active0, 0L, active2);
    }
    private int jjMoveStringLiteralDfa15_0(ulong old0, ulong active0, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active2 &= old2)) == 0L)
            return jjStartNfa_0(13, old0, 0L, old2);
        try { curChar = input_stream.ReadChar(); }
        catch (IOException)
        {
            jjStopStringLiteralDfa_0(14, 0L, 0L, active2);
            return 15;
        }
        switch ((int)curChar)
        {
            case 78:
                if ((active2 & 0x800L) != 0L)
                    return jjStartNfaWithStates_0(15, 139, 35);
                else if ((active2 & 0x2000L) != 0L)
                    return jjStartNfaWithStates_0(15, 141, 35);
                else if ((active2 & 0x8000L) != 0L)
                    return jjStartNfaWithStates_0(15, 143, 35);
                break;
            default:
                break;
        }
        return jjStartNfa_0(14, 0L, 0L, active2);
    }
    private int jjStartNfaWithStates_0(int pos, int kind, int state)
    {
        jjmatchedKind = kind;
        jjmatchedPos = pos;
        try { curChar = input_stream.ReadChar(); }
        catch (IOException) { return pos + 1; }
        return jjMoveNfa_0((uint)state, pos + 1);
    }
    static readonly ulong[] jjbitVec0 = {
   0xfffffffffffffffeL, 0xffffffffffffffffL, 0xffffffffffffffffL, 0xffffffffffffffffL
};
    static readonly ulong[] jjbitVec2 = {
   0x0L, 0x0L, 0xffffffffffffffffL, 0xffffffffffffffffL
};
    static readonly ulong[] jjbitVec3 = {
   0xfff0000000200002L, 0xffffffffffffdfffL, 0xfffff00f7fffffffL, 0x12000000007fffffL
};
    static readonly ulong[] jjbitVec4 = {
   0x0L, 0x0L, 0x420043c00000000L, 0xff7fffffff7fffffL
};
    static readonly ulong[] jjbitVec5 = {
   0xffffcffffffffL, 0xffffffffffff0000L, 0xf9ff3fffffffffffL, 0x401f00030003L
};
    static readonly ulong[] jjbitVec6 = {
   0x0L, 0x400000000000000L, 0xfffffffbffffd740L, 0xffffffcff7fffL
};
    static readonly ulong[] jjbitVec7 = {
   0xffffffffffffffffL, 0xffffffffffffffffL, 0xfffffffffffff003L, 0x33fffffffff199fL
};
    static readonly ulong[] jjbitVec8 = {
   0xfffe000000000000L, 0xfffffffe027fffffL, 0xffL, 0x707ffffff0000L
};
    static readonly ulong[] jjbitVec9 = {
   0x7fffffe00000000L, 0xfffe0000000007ffL, 0xffffffffffffffffL, 0x1c000060002fffffL
};
    static readonly ulong[] jjbitVec10 = {
   0x1ffffffd0000L, 0x0L, 0x3fffffffffL, 0x0L
};
    static readonly ulong[] jjbitVec11 = {
   0x23ffffffffffffe0L, 0x3ff010000L, 0x3c5fdfffff99fe0L, 0xf0003b0000000L
};
    static readonly ulong[] jjbitVec12 = {
   0x36dfdfffff987e0L, 0x1c00005e000000L, 0x23edfdfffffbafe0L, 0x100010000L
};
    static readonly ulong[] jjbitVec13 = {
   0x23cdfdfffff99fe0L, 0x3b0000000L, 0x3bfc718d63dc7e0L, 0x0L
};
    static readonly ulong[] jjbitVec14 = {
   0x3effdfffffddfe0L, 0x300000000L, 0x3effdfffffddfe0L, 0x340000000L
};
    static readonly ulong[] jjbitVec15 = {
   0x3fffdfffffddfe0L, 0x300000000L, 0x2ffbfffffc7fffe0L, 0x7fL
};
    static readonly ulong[] jjbitVec16 = {
   0x800dfffffffffffeL, 0x7fL, 0x200decaefef02596L, 0x3000005fL
};
    static readonly ulong[] jjbitVec17 = {
   0x1L, 0x7fffffffeffL, 0xf00L, 0x0L
};
    static readonly ulong[] jjbitVec18 = {
   0x6fbffffffffL, 0x3f0000L, 0xffffffff00000000L, 0x7fffffffff003fL
};
    static readonly ulong[] jjbitVec19 = {
   0xffffffffffffffffL, 0xffffffff83ffffffL, 0xffffff07ffffffffL, 0x3ffffffffffffffL
};
    static readonly ulong[] jjbitVec20 = {
   0xffffffffffffff7fL, 0xffffffff3d7f3d7fL, 0x7f3d7fffffff3d7fL, 0xffff7fffff7f7f3dL
};
    static readonly ulong[] jjbitVec21 = {
   0xffffffff7f3d7fffL, 0x7ffff7fL, 0xffffffff00000000L, 0x1fffffffffffffL
};
    static readonly ulong[] jjbitVec22 = {
   0xffffffffffffffffL, 0x7f9fffffffffffL, 0xffffffff07fffffeL, 0x7ffffffffffL
};
    static readonly ulong[] jjbitVec23 = {
   0x0L, 0x0L, 0xfffffffffffffL, 0x8000000L
};
    static readonly ulong[] jjbitVec24 = {
   0xffffffff00000000L, 0xffffffffffffffL, 0x1ffffffffffL, 0x0L
};
    static readonly ulong[] jjbitVec25 = {
   0xffffffffffffffffL, 0xffffffffffffffffL, 0xffffffff0fffffffL, 0x3ffffffffffffffL
};
    static readonly ulong[] jjbitVec26 = {
   0xffffffff3f3fffffL, 0x3fffffffaaff3f3fL, 0x5fdfffffffffffffL, 0x1fdc1fff0fcf1fdcL
};
    static readonly ulong[] jjbitVec27 = {
   0x8000000000000000L, 0x8000000000000001L, 0xffff00000000L, 0x0L
};
    static readonly ulong[] jjbitVec28 = {
   0x3fbbd503e2ffc84L, 0xffffffff00000000L, 0xfL, 0x0L
};
    static readonly ulong[] jjbitVec29 = {
   0x73e03fe000000e0L, 0xfffffffffffffffeL, 0xfffffffe601fffffL, 0x7fffffffffffffffL
};
    static readonly ulong[] jjbitVec30 = {
   0xfffe1fffffffffe0L, 0xffffffffffffffffL, 0xffffff00007fffL, 0x0L
};
    static readonly ulong[] jjbitVec31 = {
   0xffffffffffffffffL, 0xffffffffffffffffL, 0x3fffffffffffffL, 0x0L
};
    static readonly ulong[] jjbitVec32 = {
   0xffffffffffffffffL, 0xffffffffffffffffL, 0x3fffffffffL, 0x0L
};
    static readonly ulong[] jjbitVec33 = {
   0xffffffffffffffffL, 0xffffffffffffffffL, 0x1fffL, 0x0L
};
    static readonly ulong[] jjbitVec34 = {
   0xffffffffffffffffL, 0xffffffffffffffffL, 0xfffffffffL, 0x0L
};
    static readonly ulong[] jjbitVec35 = {
   0x3fffffffffffL, 0x0L, 0x0L, 0x0L
};
    static readonly ulong[] jjbitVec36 = {
   0x5f7ffdffa0f8007fL, 0xffffffffffffffdbL, 0x3ffffffffffffL, 0xfffffffffff80000L
};
    static readonly ulong[] jjbitVec37 = {
   0x3fffffffffffffffL, 0xffffffffffff0000L, 0xfffffffffffcffffL, 0xfff0000000000ffL
};
    static readonly ulong[] jjbitVec38 = {
   0x18000000000000L, 0xffd702000000e000L, 0xffffffffffffffffL, 0x1fffffffffffffffL
};
    static readonly ulong[] jjbitVec39 = {
   0x87fffffe00000010L, 0xffffffe007fffffeL, 0x7fffffffffffffffL, 0x631cfcfcfcL
};
    static readonly ulong[] jjbitVec40 = {
   0x0L, 0x0L, 0x420043cffffffffL, 0xff7fffffff7fffffL
};
    static readonly ulong[] jjbitVec41 = {
   0xffffffffffffffffL, 0x400000700007fffL, 0xfffffffbffffd740L, 0xffffffcff7fffL
};
    static readonly ulong[] jjbitVec42 = {
   0xffffffffffffffffL, 0xffffffffffffffffL, 0xfffffffffffff07bL, 0x33fffffffff199fL
};
    static readonly ulong[] jjbitVec43 = {
   0xfffe000000000000L, 0xfffffffe027fffffL, 0xbbfffffbfffe00ffL, 0x707ffffff0016L
};
    static readonly ulong[] jjbitVec44 = {
   0x7fffffe00000000L, 0xffff03ff003fffffL, 0xffffffffffffffffL, 0x1fff3dff9fefffffL
};
    static readonly ulong[] jjbitVec45 = {
   0xffff1fffffff8000L, 0x7ffL, 0x1ffffffffffffL, 0x0L
};
    static readonly ulong[] jjbitVec46 = {
   0xf3ffffffffffffeeL, 0xffcfff1f3fffL, 0xd3c5fdfffff99feeL, 0xfffcfb080399fL
};
    static readonly ulong[] jjbitVec47 = {
   0xd36dfdfffff987e4L, 0x1fffc05e003987L, 0xf3edfdfffffbafeeL, 0xffc100013bbfL
};
    static readonly ulong[] jjbitVec48 = {
   0xf3cdfdfffff99feeL, 0xffc3b0c0398fL, 0xc3bfc718d63dc7ecL, 0xff8000803dc7L
};
    static readonly ulong[] jjbitVec49 = {
   0xc3effdfffffddfeeL, 0xffc300603ddfL, 0xc3effdfffffddfecL, 0xffc340603ddfL
};
    static readonly ulong[] jjbitVec50 = {
   0xc3fffdfffffddfecL, 0xffc300803dcfL, 0x2ffbfffffc7fffecL, 0xc0000ff5f847fL
};
    static readonly ulong[] jjbitVec51 = {
   0x87fffffffffffffeL, 0x3ff7fffL, 0x3bffecaefef02596L, 0x33ff3f5fL
};
    static readonly ulong[] jjbitVec52 = {
   0xc2a003ff03000001L, 0xfffe07fffffffeffL, 0x1ffffffffeff0fdfL, 0x40L
};
    static readonly ulong[] jjbitVec53 = {
   0x3c7f6fbffffffffL, 0x3ff03ffL, 0xffffffff00000000L, 0x7fffffffff003fL
};
    static readonly ulong[] jjbitVec54 = {
   0xffffffff7f3d7fffL, 0x3fe0007ffff7fL, 0xffffffff00000000L, 0x1fffffffffffffL
};
    static readonly ulong[] jjbitVec55 = {
   0x0L, 0x0L, 0xffffffffffffffffL, 0x3ff080fffffL
};
    static readonly ulong[] jjbitVec56 = {
   0xffffffff03ff7800L, 0xffffffffffffffL, 0x3ffffffffffL, 0x0L
};
    static readonly ulong[] jjbitVec57 = {
   0x80007c000000f000L, 0x8000fc0000000001L, 0xffff00000000L, 0x21fff0000L
};
    static readonly ulong[] jjbitVec58 = {
   0x73efffe000000e0L, 0xfffffffffffffffeL, 0xfffffffe661fffffL, 0x7fffffffffffffffL
};
    static readonly ulong[] jjbitVec59 = {
   0x5f7ffdffe0f8007fL, 0xffffffffffffffdbL, 0x3ffffffffffffL, 0xfffffffffff80000L
};
    static readonly ulong[] jjbitVec60 = {
   0x18000f00000000L, 0xffd702000000e000L, 0xffffffffffffffffL, 0x9fffffffffffffffL
};
    static readonly ulong[] jjbitVec61 = {
   0x87fffffe03ff0010L, 0xffffffe007fffffeL, 0x7fffffffffffffffL, 0xe0000631cfcfcfcL
};
    private int jjMoveNfa_0(uint startState, int curPos)
    {
        int startsAt = 0;
        jjnewStateCnt = 85;
        int i = 1;
        jjstateSet[0] = startState;
        int kind = 0x7fffffff;
        for (; ; )
        {
            if (++jjround == 0x7fffffff)
                ReInitRounds();
            if (curChar < 64)
            {
                ulong l = 1UL << curChar;
                do
                {
                    switch (jjstateSet[--i])
                    {
                        case 3:
                            if ((0x3ff000000000000L & l) != 0L)
                                jjCheckNAddStates(0, 9);
                            else if (curChar == 36)
                            {
                                if (kind > 151)
                                    kind = 151;
                                jjCheckNAdd(35);
                            }
                            else if (curChar == 34)
                                jjCheckNAddStates(10, 12);
                            else if (curChar == 39)
                                jjAddStates(13, 14);
                            else if (curChar == 46)
                                jjCheckNAdd(9);
                            else if (curChar == 47)
                                jjstateSet[jjnewStateCnt++] = 2;
                            if ((0x3fe000000000000L & l) != 0L)
                            {
                                if (kind > 83)
                                    kind = 83;
                                jjCheckNAddStates(15, 17);
                            }
                            else if (curChar == 48)
                            {
                                if (kind > 83)
                                    kind = 83;
                                jjCheckNAddStates(18, 24);
                            }
                            break;
                        case 0:
                            if (curChar == 42)
                                jjstateSet[jjnewStateCnt++] = 1;
                            break;
                        case 1:
                            if ((0xffff7fffffffffffUL & l) != 0L && kind > 21)
                                kind = 21;
                            break;
                        case 2:
                            if (curChar == 42)
                                jjstateSet[jjnewStateCnt++] = 0;
                            break;
                        case 4:
                            if ((0x3fe000000000000L & l) == 0L)
                                break;
                            if (kind > 83)
                                kind = 83;
                            jjCheckNAddStates(15, 17);
                            break;
                        case 6:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 83)
                                kind = 83;
                            jjCheckNAddStates(15, 17);
                            break;
                        case 8:
                            if (curChar == 46)
                                jjCheckNAdd(9);
                            break;
                        case 9:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 88)
                                kind = 88;
                            jjCheckNAddStates(25, 28);
                            break;
                        case 12:
                            if ((0x280000000000L & l) != 0L)
                                jjCheckNAdd(13);
                            break;
                        case 13:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 88)
                                kind = 88;
                            jjCheckNAddStates(29, 31);
                            break;
                        case 16:
                            if (curChar == 39)
                                jjAddStates(13, 14);
                            break;
                        case 17:
                            if ((0xffffff7fffffdbffL & l) != 0L)
                                jjCheckNAdd(18);
                            break;
                        case 18:
                            if (curChar == 39 && kind > 93)
                                kind = 93;
                            break;
                        case 20:
                            if ((0x8400000000L & l) != 0L)
                                jjCheckNAdd(18);
                            break;
                        case 21:
                            if ((0xff000000000000L & l) != 0L)
                                jjCheckNAddTwoStates(22, 18);
                            break;
                        case 22:
                            if ((0xff000000000000L & l) != 0L)
                                jjCheckNAdd(18);
                            break;
                        case 23:
                            if ((0xf000000000000L & l) != 0L)
                                jjstateSet[jjnewStateCnt++] = 24;
                            break;
                        case 24:
                            if ((0xff000000000000L & l) != 0L)
                                jjCheckNAdd(22);
                            break;
                        case 25:
                            if (curChar == 34)
                                jjCheckNAddStates(10, 12);
                            break;
                        case 26:
                            if ((0xfffffffbffffdbffL & l) != 0L)
                                jjCheckNAddStates(10, 12);
                            break;
                        case 28:
                            if ((0x8400000000L & l) != 0L)
                                jjCheckNAddStates(10, 12);
                            break;
                        case 29:
                            if (curChar == 34 && kind > 94)
                                kind = 94;
                            break;
                        case 30:
                            if ((0xff000000000000L & l) != 0L)
                                jjCheckNAddStates(32, 35);
                            break;
                        case 31:
                            if ((0xff000000000000L & l) != 0L)
                                jjCheckNAddStates(10, 12);
                            break;
                        case 32:
                            if ((0xf000000000000L & l) != 0L)
                                jjstateSet[jjnewStateCnt++] = 33;
                            break;
                        case 33:
                            if ((0xff000000000000L & l) != 0L)
                                jjCheckNAdd(31);
                            break;
                        case 34:
                            if (curChar != 36)
                                break;
                            if (kind > 151)
                                kind = 151;
                            jjCheckNAdd(35);
                            break;
                        case 35:
                            if ((0x3ff00100fffc1ffL & l) == 0L)
                                break;
                            if (kind > 151)
                                kind = 151;
                            jjCheckNAdd(35);
                            break;
                        case 36:
                            if ((0x3ff000000000000L & l) != 0L)
                                jjCheckNAddStates(0, 9);
                            break;
                        case 38:
                            if ((0x3ff000000000000L & l) != 0L)
                                jjCheckNAddStates(36, 39);
                            break;
                        case 40:
                            if ((0x280000000000L & l) != 0L)
                                jjCheckNAdd(41);
                            break;
                        case 41:
                            if ((0x3ff000000000000L & l) != 0L)
                                jjCheckNAddStates(40, 42);
                            break;
                        case 44:
                            if ((0x3ff000000000000L & l) != 0L)
                                jjCheckNAddStates(43, 45);
                            break;
                        case 46:
                            if ((0x280000000000L & l) != 0L)
                                jjCheckNAdd(47);
                            break;
                        case 47:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 88)
                                kind = 88;
                            jjCheckNAddStates(46, 48);
                            break;
                        case 50:
                            if ((0x3ff000000000000L & l) != 0L)
                                jjCheckNAddStates(49, 51);
                            break;
                        case 51:
                            if (curChar != 46)
                                break;
                            if (kind > 88)
                                kind = 88;
                            jjCheckNAddStates(52, 54);
                            break;
                        case 52:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 88)
                                kind = 88;
                            jjCheckNAddStates(55, 58);
                            break;
                        case 55:
                            if ((0x280000000000L & l) != 0L)
                                jjCheckNAdd(56);
                            break;
                        case 56:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 88)
                                kind = 88;
                            jjCheckNAddStates(59, 61);
                            break;
                        case 58:
                            if (curChar != 48)
                                break;
                            if (kind > 83)
                                kind = 83;
                            jjCheckNAddStates(18, 24);
                            break;
                        case 60:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 83)
                                kind = 83;
                            jjCheckNAddStates(62, 64);
                            break;
                        case 63:
                            if ((0xff000000000000L & l) == 0L)
                                break;
                            if (kind > 83)
                                kind = 83;
                            jjCheckNAddStates(65, 67);
                            break;
                        case 65:
                            if ((0x3000000000000L & l) == 0L)
                                break;
                            if (kind > 83)
                                kind = 83;
                            jjCheckNAddStates(68, 70);
                            break;
                        case 68:
                            if ((0x3ff000000000000L & l) != 0L)
                                jjAddStates(71, 73);
                            break;
                        case 70:
                            if (curChar == 46)
                                jjCheckNAdd(71);
                            break;
                        case 71:
                            if ((0x3ff000000000000L & l) != 0L)
                                jjCheckNAddStates(74, 76);
                            break;
                        case 74:
                            if ((0x280000000000L & l) != 0L)
                                jjCheckNAdd(75);
                            break;
                        case 75:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 88)
                                kind = 88;
                            jjCheckNAddStates(77, 79);
                            break;
                        case 78:
                            if ((0x3ff000000000000L & l) != 0L)
                                jjCheckNAddStates(80, 83);
                            break;
                        case 80:
                            if (curChar == 46)
                                jjCheckNAdd(81);
                            break;
                        case 82:
                            if ((0x280000000000L & l) != 0L)
                                jjCheckNAdd(83);
                            break;
                        case 83:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 88)
                                kind = 88;
                            jjCheckNAddStates(84, 86);
                            break;
                        default: break;
                    }
                } while (i != startsAt);
            }
            else if (curChar < 128)
            {
                ulong l = 1UL << (curChar & 077);
                do
                {
                    switch (jjstateSet[--i])
                    {
                        case 3:
                            if ((0x7fffffe87fffffeL & l) == 0L)
                                break;
                            if (kind > 151)
                                kind = 151;
                            jjCheckNAdd(35);
                            break;
                        case 1:
                            if (kind > 21)
                                kind = 21;
                            break;
                        case 5:
                            if (curChar == 95)
                                jjAddStates(87, 88);
                            break;
                        case 7:
                            if ((0x100000001000L & l) != 0L && kind > 83)
                                kind = 83;
                            break;
                        case 10:
                            if (curChar == 95)
                                jjAddStates(89, 90);
                            break;
                        case 11:
                            if ((0x2000000020L & l) != 0L)
                                jjCheckNAddTwoStates(12, 13);
                            break;
                        case 14:
                            if (curChar == 95)
                                jjCheckNAddTwoStates(14, 13);
                            break;
                        case 15:
                            if ((0x5000000050L & l) != 0L && kind > 88)
                                kind = 88;
                            break;
                        case 17:
                            if ((0xffffffffefffffffL & l) != 0L)
                                jjCheckNAdd(18);
                            break;
                        case 19:
                            if (curChar == 92)
                                jjAddStates(91, 93);
                            break;
                        case 20:
                            if ((0x14404410000000L & l) != 0L)
                                jjCheckNAdd(18);
                            break;
                        case 26:
                            if ((0xffffffffefffffffL & l) != 0L)
                                jjCheckNAddStates(10, 12);
                            break;
                        case 27:
                            if (curChar == 92)
                                jjAddStates(94, 96);
                            break;
                        case 28:
                            if ((0x14404410000000L & l) != 0L)
                                jjCheckNAddStates(10, 12);
                            break;
                        case 35:
                            if ((0x87fffffe87fffffeL & l) == 0L)
                                break;
                            if (kind > 151)
                                kind = 151;
                            jjCheckNAdd(35);
                            break;
                        case 37:
                            if (curChar == 95)
                                jjAddStates(97, 98);
                            break;
                        case 39:
                            if ((0x2000000020L & l) != 0L)
                                jjCheckNAddTwoStates(40, 41);
                            break;
                        case 42:
                            if (curChar == 95)
                                jjCheckNAddTwoStates(42, 41);
                            break;
                        case 43:
                            if (curChar == 95)
                                jjAddStates(99, 100);
                            break;
                        case 45:
                            if ((0x2000000020L & l) != 0L)
                                jjCheckNAddTwoStates(46, 47);
                            break;
                        case 48:
                            if (curChar == 95)
                                jjCheckNAddTwoStates(48, 47);
                            break;
                        case 49:
                            if (curChar == 95)
                                jjAddStates(101, 102);
                            break;
                        case 53:
                            if (curChar == 95)
                                jjAddStates(103, 104);
                            break;
                        case 54:
                            if ((0x2000000020L & l) != 0L)
                                jjCheckNAddTwoStates(55, 56);
                            break;
                        case 57:
                            if (curChar == 95)
                                jjCheckNAddTwoStates(57, 56);
                            break;
                        case 59:
                            if ((0x100000001000000L & l) != 0L)
                                jjCheckNAdd(60);
                            break;
                        case 60:
                            if ((0x7e0000007eL & l) == 0L)
                                break;
                            if (kind > 83)
                                kind = 83;
                            jjCheckNAddStates(62, 64);
                            break;
                        case 61:
                            if (curChar == 95)
                                jjCheckNAddTwoStates(61, 60);
                            break;
                        case 62:
                            if (curChar == 95)
                                jjAddStates(105, 106);
                            break;
                        case 64:
                            if ((0x400000004L & l) != 0L)
                                jjCheckNAdd(65);
                            break;
                        case 66:
                            if (curChar == 95)
                                jjCheckNAddTwoStates(66, 65);
                            break;
                        case 67:
                            if ((0x100000001000000L & l) != 0L)
                                jjCheckNAddTwoStates(68, 70);
                            break;
                        case 68:
                            if ((0x7e0000007eL & l) != 0L)
                                jjCheckNAddStates(71, 73);
                            break;
                        case 69:
                            if (curChar == 95)
                                jjCheckNAddTwoStates(69, 68);
                            break;
                        case 71:
                            if ((0x7e0000007eL & l) != 0L)
                                jjCheckNAddStates(74, 76);
                            break;
                        case 72:
                            if (curChar == 95)
                                jjCheckNAddTwoStates(72, 71);
                            break;
                        case 73:
                            if ((0x1000000010000L & l) != 0L)
                                jjCheckNAddTwoStates(74, 75);
                            break;
                        case 76:
                            if (curChar == 95)
                                jjCheckNAddTwoStates(76, 75);
                            break;
                        case 77:
                            if ((0x100000001000000L & l) != 0L)
                                jjCheckNAdd(78);
                            break;
                        case 78:
                            if ((0x7e0000007eL & l) != 0L)
                                jjCheckNAddStates(80, 83);
                            break;
                        case 79:
                            if (curChar == 95)
                                jjCheckNAddTwoStates(79, 78);
                            break;
                        case 81:
                            if ((0x1000000010000L & l) != 0L)
                                jjCheckNAddTwoStates(82, 83);
                            break;
                        case 84:
                            if (curChar == 95)
                                jjCheckNAddTwoStates(84, 83);
                            break;
                        default: break;
                    }
                } while (i != startsAt);
            }
            else
            {
                int hiByte = (int)(curChar >> 8);
                int i1 = hiByte >> 6;
                ulong l1 = 1UL << (hiByte & 077);
                int i2 = (curChar & 0xff) >> 6;
                ulong l2 = 1UL << (curChar & 077);
                do
                {
                    switch (jjstateSet[--i])
                    {
                        case 3:
                            if (!jjCanMove_1(hiByte, i1, i2, l1, l2))
                                break;
                            if (kind > 151)
                                kind = 151;
                            jjCheckNAdd(35);
                            break;
                        case 1:
                            if (jjCanMove_0(hiByte, i1, i2, l1, l2) && kind > 21)
                                kind = 21;
                            break;
                        case 17:
                            if (jjCanMove_0(hiByte, i1, i2, l1, l2))
                                jjstateSet[jjnewStateCnt++] = 18;
                            break;
                        case 26:
                            if (jjCanMove_0(hiByte, i1, i2, l1, l2))
                                jjAddStates(10, 12);
                            break;
                        case 35:
                            if (!jjCanMove_2(hiByte, i1, i2, l1, l2))
                                break;
                            if (kind > 151)
                                kind = 151;
                            jjCheckNAdd(35);
                            break;
                        default: break;
                    }
                } while (i != startsAt);
            }
            if (kind != 0x7fffffff)
            {
                jjmatchedKind = kind;
                jjmatchedPos = curPos;
                kind = 0x7fffffff;
            }
            ++curPos;
            if ((i = jjnewStateCnt) == (startsAt = 85 - (jjnewStateCnt = startsAt)))
                return curPos;
            try { curChar = input_stream.ReadChar(); }
            catch (IOException) { return curPos; }
        }
    }
    private int jjMoveStringLiteralDfa0_4() => (int)curChar switch
    {
        42 => jjMoveStringLiteralDfa1_4(0x4000000L),
        _ => 1,
    };
    private int jjMoveStringLiteralDfa1_4(ulong active0)
    {
        try { curChar = input_stream.ReadChar(); }
        catch (IOException)
        {
            return 1;
        }
        switch ((int)curChar)
        {
            case 47:
                if ((active0 & 0x4000000L) != 0L)
                    return jjStopAtPos(1, 26);
                break;
            default:
                return 2;
        }
        return 2;
    }
    private int jjMoveStringLiteralDfa0_2()
    {
        return jjMoveNfa_2(0, 0);
    }
    private int jjMoveNfa_2(uint startState, int curPos)
    {
        int startsAt = 0;
        jjnewStateCnt = 3;
        int i = 1;
        jjstateSet[0] = startState;
        int kind = 0x7fffffff;
        for (; ; )
        {
            if (++jjround == 0x7fffffff)
                ReInitRounds();
            if (curChar < 64)
            {
                long l = 1L << curChar;
                do
                {
                    switch (jjstateSet[--i])
                    {
                        case 0:
                            if ((0x2400L & l) != 0L)
                            {
                                if (kind > 24)
                                    kind = 24;
                            }
                            if (curChar == 13)
                                jjstateSet[jjnewStateCnt++] = 1;
                            break;
                        case 1:
                            if (curChar == 10 && kind > 24)
                                kind = 24;
                            break;
                        case 2:
                            if (curChar == 13)
                                jjstateSet[jjnewStateCnt++] = 1;
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
                    switch (jjstateSet[--i])
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
                    switch (jjstateSet[--i])
                    {
                        default: break;
                    }
                } while (i != startsAt);
            }
            if (kind != 0x7fffffff)
            {
                jjmatchedKind = kind;
                jjmatchedPos = curPos;
                kind = 0x7fffffff;
            }
            ++curPos;
            if ((i = jjnewStateCnt) == (startsAt = 3 - (jjnewStateCnt = startsAt)))
                return curPos;
            try { curChar = input_stream.ReadChar(); }
            catch (IOException) { return curPos; }
        }
    }
    private int jjMoveStringLiteralDfa0_1()
    {
        return 1;
    }
    private int jjMoveStringLiteralDfa0_3() => (int)curChar switch
    {
        42 => jjMoveStringLiteralDfa1_3(0x2000000L),
        _ => 1,
    };
    private int jjMoveStringLiteralDfa1_3(ulong active0)
    {
        try { curChar = input_stream.ReadChar(); }
        catch (IOException)
        {
            return 1;
        }
        switch ((int)curChar)
        {
            case 47:
                if ((active0 & 0x2000000L) != 0L)
                    return jjStopAtPos(1, 25);
                break;
            default:
                return 2;
        }
        return 2;
    }
    static readonly uint[] jjnextStates = {
   37, 38, 39, 15, 43, 44, 45, 49, 50, 51, 26, 27, 29, 17, 19, 5,
   6, 7, 59, 62, 63, 7, 64, 67, 77, 10, 9, 11, 15, 14, 13, 15,
   26, 27, 31, 29, 37, 38, 39, 15, 42, 41, 15, 43, 44, 45, 48, 47,
   15, 49, 50, 51, 52, 54, 15, 53, 52, 54, 15, 57, 56, 15, 61, 60,
   7, 62, 63, 7, 66, 65, 7, 69, 68, 70, 72, 71, 73, 76, 75, 15,
   79, 78, 80, 81, 84, 83, 15, 5, 6, 10, 9, 20, 21, 23, 28, 30,
   32, 37, 38, 43, 44, 49, 50, 53, 52, 62, 63,
};
    private static bool jjCanMove_0(int hiByte, int i1, int i2, ulong l1, ulong l2)
    {
        switch (hiByte)
        {
            case 0:
                return ((jjbitVec2[i2] & l2) != 0L);
            default:
                if ((jjbitVec0[i1] & l1) != 0L)
                    return true;
                return false;
        }
    }
    private static bool jjCanMove_1(int hiByte, int i1, int i2, ulong l1, ulong l2)
    {
        switch (hiByte)
        {
            case 0:
                return ((jjbitVec4[i2] & l2) != 0L);
            case 2:
                return ((jjbitVec5[i2] & l2) != 0L);
            case 3:
                return ((jjbitVec6[i2] & l2) != 0L);
            case 4:
                return ((jjbitVec7[i2] & l2) != 0L);
            case 5:
                return ((jjbitVec8[i2] & l2) != 0L);
            case 6:
                return ((jjbitVec9[i2] & l2) != 0L);
            case 7:
                return ((jjbitVec10[i2] & l2) != 0L);
            case 9:
                return ((jjbitVec11[i2] & l2) != 0L);
            case 10:
                return ((jjbitVec12[i2] & l2) != 0L);
            case 11:
                return ((jjbitVec13[i2] & l2) != 0L);
            case 12:
                return ((jjbitVec14[i2] & l2) != 0L);
            case 13:
                return ((jjbitVec15[i2] & l2) != 0L);
            case 14:
                return ((jjbitVec16[i2] & l2) != 0L);
            case 15:
                return ((jjbitVec17[i2] & l2) != 0L);
            case 16:
                return ((jjbitVec18[i2] & l2) != 0L);
            case 17:
                return ((jjbitVec19[i2] & l2) != 0L);
            case 18:
                return ((jjbitVec20[i2] & l2) != 0L);
            case 19:
                return ((jjbitVec21[i2] & l2) != 0L);
            case 20:
                return ((jjbitVec0[i2] & l2) != 0L);
            case 22:
                return ((jjbitVec22[i2] & l2) != 0L);
            case 23:
                return ((jjbitVec23[i2] & l2) != 0L);
            case 24:
                return ((jjbitVec24[i2] & l2) != 0L);
            case 30:
                return ((jjbitVec25[i2] & l2) != 0L);
            case 31:
                return ((jjbitVec26[i2] & l2) != 0L);
            case 32:
                return ((jjbitVec27[i2] & l2) != 0L);
            case 33:
                return ((jjbitVec28[i2] & l2) != 0L);
            case 48:
                return ((jjbitVec29[i2] & l2) != 0L);
            case 49:
                return ((jjbitVec30[i2] & l2) != 0L);
            case 77:
                return ((jjbitVec31[i2] & l2) != 0L);
            case 159:
                return ((jjbitVec32[i2] & l2) != 0L);
            case 164:
                return ((jjbitVec33[i2] & l2) != 0L);
            case 215:
                return ((jjbitVec34[i2] & l2) != 0L);
            case 250:
                return ((jjbitVec35[i2] & l2) != 0L);
            case 251:
                return ((jjbitVec36[i2] & l2) != 0L);
            case 253:
                return ((jjbitVec37[i2] & l2) != 0L);
            case 254:
                return ((jjbitVec38[i2] & l2) != 0L);
            case 255:
                return ((jjbitVec39[i2] & l2) != 0L);
            default:
                if ((jjbitVec3[i1] & l1) != 0L)
                    return true;
                return false;
        }
    }
    private static bool jjCanMove_2(int hiByte, int i1, int i2, ulong l1, ulong l2)
    {
        switch (hiByte)
        {
            case 0:
                return ((jjbitVec40[i2] & l2) != 0L);
            case 2:
                return ((jjbitVec5[i2] & l2) != 0L);
            case 3:
                return ((jjbitVec41[i2] & l2) != 0L);
            case 4:
                return ((jjbitVec42[i2] & l2) != 0L);
            case 5:
                return ((jjbitVec43[i2] & l2) != 0L);
            case 6:
                return ((jjbitVec44[i2] & l2) != 0L);
            case 7:
                return ((jjbitVec45[i2] & l2) != 0L);
            case 9:
                return ((jjbitVec46[i2] & l2) != 0L);
            case 10:
                return ((jjbitVec47[i2] & l2) != 0L);
            case 11:
                return ((jjbitVec48[i2] & l2) != 0L);
            case 12:
                return ((jjbitVec49[i2] & l2) != 0L);
            case 13:
                return ((jjbitVec50[i2] & l2) != 0L);
            case 14:
                return ((jjbitVec51[i2] & l2) != 0L);
            case 15:
                return ((jjbitVec52[i2] & l2) != 0L);
            case 16:
                return ((jjbitVec53[i2] & l2) != 0L);
            case 17:
                return ((jjbitVec19[i2] & l2) != 0L);
            case 18:
                return ((jjbitVec20[i2] & l2) != 0L);
            case 19:
                return ((jjbitVec54[i2] & l2) != 0L);
            case 20:
                return ((jjbitVec0[i2] & l2) != 0L);
            case 22:
                return ((jjbitVec22[i2] & l2) != 0L);
            case 23:
                return ((jjbitVec55[i2] & l2) != 0L);
            case 24:
                return ((jjbitVec56[i2] & l2) != 0L);
            case 30:
                return ((jjbitVec25[i2] & l2) != 0L);
            case 31:
                return ((jjbitVec26[i2] & l2) != 0L);
            case 32:
                return ((jjbitVec57[i2] & l2) != 0L);
            case 33:
                return ((jjbitVec28[i2] & l2) != 0L);
            case 48:
                return ((jjbitVec58[i2] & l2) != 0L);
            case 49:
                return ((jjbitVec30[i2] & l2) != 0L);
            case 77:
                return ((jjbitVec31[i2] & l2) != 0L);
            case 159:
                return ((jjbitVec32[i2] & l2) != 0L);
            case 164:
                return ((jjbitVec33[i2] & l2) != 0L);
            case 215:
                return ((jjbitVec34[i2] & l2) != 0L);
            case 250:
                return ((jjbitVec35[i2] & l2) != 0L);
            case 251:
                return ((jjbitVec59[i2] & l2) != 0L);
            case 253:
                return ((jjbitVec37[i2] & l2) != 0L);
            case 254:
                return ((jjbitVec60[i2] & l2) != 0L);
            case 255:
                return ((jjbitVec61[i2] & l2) != 0L);
            default:
                if ((jjbitVec3[i1] & l1) != 0L)
                    return true;
                return false;
        }
    }

    /** Token literal values. */  
    public static string[] jjstrLiteralImages = {
        "",
        "\x4c\x4f\x4f\x4b\x41\x48\x45\x41\x44",
        "\x49\x47\x4e\x4f\x52\x45\x5f\x43\x41\x53\x45",
        "\x50\x41\x52\x53\x45\x52\x5f\x42\x45\x47\x49\x4e",
        "\x50\x41\x52\x53\x45\x52\x5f\x45\x4e\x44",
        "\x4a\x41\x56\x41\x43\x4f\x44\x45",
        "\x43\x50\x50\x43\x4f\x44\x45",
        "\x54\x4f\x4b\x45\x4e",
        "\x53\x50\x45\x43\x49\x41\x4c\x5f\x54\x4f\x4b\x45\x4e",
        "\x4d\x4f\x52\x45",
        "\x53\x4b\x49\x50",
        "\x54\x4f\x4b\x45\x4e\x5f\x4d\x47\x52\x5f\x44\x45\x43\x4c\x53",
        "\x45\x4f\x46",
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        "\x61\x62\x73\x74\x72\x61\x63\x74",
        "\x61\x73\x73\x65\x72\x74",
        "\x62\x6f\x6f\x6c\x65\x61\x6e",
        "\x62\x72\x65\x61\x6b",
        "\x62\x79\x74\x65",
        "\x63\x61\x73\x65",
        "\x63\x61\x74\x63\x68",
        "\x63\x68\x61\x72",
        "\x63\x6c\x61\x73\x73",
        "\x63\x6f\x6e\x73\x74",
        "\x63\x6f\x6e\x74\x69\x6e\x75\x65",
        "\x64\x65\x66\x61\x75\x6c\x74",
        "\x64\x6f",
        "\x64\x6f\x75\x62\x6c\x65",
        "\x65\x6c\x73\x65",
        "\x65\x6e\x75\x6d",
        "\x65\x78\x74\x65\x6e\x64\x73",
        "\x66\x61\x6c\x73\x65",
        "\x66\x69\x6e\x61\x6c",
        "\x66\x69\x6e\x61\x6c\x6c\x79",
        "\x66\x6c\x6f\x61\x74",
        "\x66\x6f\x72",
        "\x67\x6f\x74\x6f",
        "\x69\x66",
        "\x69\x6d\x70\x6c\x65\x6d\x65\x6e\x74\x73",
        "\x69\x6d\x70\x6f\x72\x74",
        "\x69\x6e\x73\x74\x61\x6e\x63\x65\x6f\x66",
        "\x69\x6e\x74",
        "\x69\x6e\x74\x65\x72\x66\x61\x63\x65",
        "\x6c\x6f\x6e\x67",
        "\x6e\x61\x74\x69\x76\x65",
        "\x6e\x65\x77",
        "\x6e\x75\x6c\x6c",
        "\x70\x61\x63\x6b\x61\x67\x65",
        "\x70\x72\x69\x76\x61\x74\x65",
        "\x70\x72\x6f\x74\x65\x63\x74\x65\x64",
        "\x70\x75\x62\x6c\x69\x63",
        "\x72\x65\x74\x75\x72\x6e",
        "\x73\x68\x6f\x72\x74",
        "\x73\x74\x61\x74\x69\x63",
        "\x73\x74\x72\x69\x63\x74\x66\x70",
        "\x73\x75\x70\x65\x72",
        "\x73\x77\x69\x74\x63\x68",
        "\x73\x79\x6e\x63\x68\x72\x6f\x6e\x69\x7a\x65\x64",
        "\x74\x68\x69\x73",
        "\x74\x68\x72\x6f\x77",
        "\x74\x68\x72\x6f\x77\x73",
        "\x74\x72\x61\x6e\x73\x69\x65\x6e\x74",
        "\x74\x72\x75\x65",
        "\x74\x72\x79",
        "\x76\x6f\x69\x64",
        "\x76\x6f\x6c\x61\x74\x69\x6c\x65",
        "\x77\x68\x69\x6c\x65",
        "\x74\x65\x6d\x70\x6c\x61\x74\x65",
        "\x74\x79\x70\x65\x6e\x61\x6d\x65",
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        "\x28",
        "\x29",
        "\x7b",
        "\x7d",
        "\x5b",
        "\x5d",
        "\x3b",
        "\x2c",
        "\x2e",
        "\x3d",
        "\x3c",
        "\x21",
        "\x7e",
        "\x3f",
        "\x3a",
        "\x3a\x3a",
        "\x3d\x3d",
        "\x3c\x3d",
        "\x3e\x3d",
        "\x21\x3d",
        "\x7c\x7c",
        "\x26\x26",
        "\x2b\x2b",
        "\x2d\x2d",
        "\x2b",
        "\x2d",
        "\x2a",
        "\x2f",
        "\x26",
        "\x7c",
        "\x5e",
        "\x25",
        "\x2b\x3d",
        "\x2d\x3d",
        "\x2a\x3d",
        "\x2f\x3d",
        "\x26\x3d",
        "\x7c\x3d",
        "\x5e\x3d",
        "\x25\x3d",
        "\x3e\x3e\x3e",
        "\x3e\x3e",
        "\x3e",
        "\x23",
        "\x44\x43\x4c\x5f\x50\x41\x52\x53\x45\x52\x5f\x42\x45\x47\x49\x4e",
        "\x44\x43\x4c\x5f\x50\x41\x52\x53\x45\x52\x5f\x45\x4e\x44",
        "\x49\x4e\x43\x5f\x50\x41\x52\x53\x45\x52\x5f\x42\x45\x47\x49\x4e",
        "\x49\x4e\x43\x5f\x50\x41\x52\x53\x45\x52\x5f\x45\x4e\x44",
        "\x44\x45\x46\x5f\x50\x41\x52\x53\x45\x52\x5f\x42\x45\x47\x49\x4e",
        "\x44\x45\x46\x5f\x50\x41\x52\x53\x45\x52\x5f\x45\x4e\x44",
        "\x2e\x2e\x2e",
        "\x3c\x3c\x3d",
        "\x3e\x3e\x3d",
        "\x3e\x3e\x3e\x3d",
        "\x3c\x3c",
        "\x40",
        null,
        null,
        null,
        null,
        };

    /** Lexer state names. */
    public static String[] lexStateNames = {
       "DEFAULT",
       "AFTER_EGEN",
       "IN_SINGLE_LINE_COMMENT",
       "IN_FORMAL_COMMENT",
       "IN_MULTI_LINE_COMMENT",
    };

    /** Lex State array. */
    public static int[] jjnewLexState = {
   -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1, 0, 2, 3, 4, 4, 0,
   0, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1,
};
    static readonly ulong[] jjtoToken = {
   0xfffffffff0001fffL, 0xffffffffe10fffffL, 0x4ffffffL,
};
    static readonly ulong[] jjtoSkip = {
   0x70fe000L, 0x0L, 0x0L,
};
    static readonly ulong[] jjtoSpecial = {
   0x7000000L, 0x0L, 0x0L,
};
    static readonly ulong[] jjtoMore = {
   0x8f00000L, 0x0L, 0x0L,
};
    protected CSharpCharStream input_stream;
    private readonly uint[] jjrounds = new uint[85];
    private readonly uint[] jjstateSet = new uint[170];
    private readonly StringBuilder jjimage = new();
    private StringBuilder image;
    private int jjimageLen;
    private int lengthOfMatch;
    protected char curChar;
    /** Constructor. */
    public CSharpCCParserTokenManager(CSharpCharStream stream)
    {
        if (CSharpCharStream.StaticFlag)
            throw new Error("ERROR: Cannot use a static CharStream class with a non-static lexical analyzer.");
        input_stream = stream;
    }

    /** Constructor. */
    public CSharpCCParserTokenManager(CSharpCharStream stream, int lexState)
        : this(stream)
    {
        SwitchTo(lexState);
    }

    /** Reinitialise parser. */
    public void ReInit(CSharpCharStream stream)
    {
        jjmatchedPos = jjnewStateCnt = 0;
        curLexState = defaultLexState;
        input_stream = stream;
        ReInitRounds();
    }
    private void ReInitRounds()
    {
        int i;
        jjround = 0x80000001;
        for (i = 85; i-- > 0;)
            jjrounds[i] = 0x80000000;
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
        if (lexState >= 5 || lexState < 0)
            throw new TokenMgrError("Error: Ignoring invalid lexical state : " + lexState + ". State unchanged.", TokenMgrError.INVALID_LEXICAL_STATE);
        else
            curLexState = lexState;
    }

    protected Token jjFillToken()
    {
        Token t;
        String curTokenImage;
        int beginLine;
        int endLine;
        int beginColumn;
        int endColumn;
        String im = jjstrLiteralImages[jjmatchedKind];
        curTokenImage = im ?? input_stream.GetImage();
        beginLine = input_stream.GetBeginLine();
        beginColumn = input_stream.GetBeginColumn();
        endLine = input_stream.GetEndLine();
        endColumn = input_stream.GetEndColumn();
        t = Token.NewToken(jjmatchedKind, curTokenImage);

        t.beginLine = beginLine;
        t.endLine = endLine;
        t.beginColumn = beginColumn;
        t.endColumn = endColumn;

        return t;
    }

    int curLexState = 0;
    readonly int defaultLexState = 0;
    int jjnewStateCnt;
    uint jjround;
    int jjmatchedPos;
    int jjmatchedKind;

    /** Get the next Token. */
    public Token getNextToken()
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
            catch (IOException)
            {
                jjmatchedKind = 0;
                matchedToken = jjFillToken();
                matchedToken.specialToken = specialToken;
                return matchedToken;
            }
            image = jjimage;
            image.Capacity = 0;
            jjimageLen = 0;

            for (; ; )
            {
                switch (curLexState)
                {
                    case 0:
                        try
                        {
                            input_stream.Backup(0);
                            while (curChar <= 32 && (0x100003600L & (1L << curChar)) != 0L)
                                curChar = input_stream.BeginToken();
                        }
                        catch (IOException e1) { goto EOFLoop; }
                        jjmatchedKind = 0x7fffffff;
                        jjmatchedPos = 0;
                        curPos = jjMoveStringLiteralDfa0_0();
                        if (jjmatchedPos == 0 && jjmatchedKind > 154)
                        {
                            jjmatchedKind = 154;
                        }
                        break;
                    case 1:
                        jjmatchedKind = 0x7fffffff;
                        jjmatchedPos = 0;
                        curPos = jjMoveStringLiteralDfa0_1();
                        if (jjmatchedPos == 0 && jjmatchedKind > 19)
                        {
                            jjmatchedKind = 19;
                        }
                        break;
                    case 2:
                        jjmatchedKind = 0x7fffffff;
                        jjmatchedPos = 0;
                        curPos = jjMoveStringLiteralDfa0_2();
                        if (jjmatchedPos == 0 && jjmatchedKind > 27)
                        {
                            jjmatchedKind = 27;
                        }
                        break;
                    case 3:
                        jjmatchedKind = 0x7fffffff;
                        jjmatchedPos = 0;
                        curPos = jjMoveStringLiteralDfa0_3();
                        if (jjmatchedPos == 0 && jjmatchedKind > 27)
                        {
                            jjmatchedKind = 27;
                        }
                        break;
                    case 4:
                        jjmatchedKind = 0x7fffffff;
                        jjmatchedPos = 0;
                        curPos = jjMoveStringLiteralDfa0_4();
                        if (jjmatchedPos == 0 && jjmatchedKind > 27)
                        {
                            jjmatchedKind = 27;
                        }
                        break;
                }
                if (jjmatchedKind != 0x7fffffff)
                {
                    if (jjmatchedPos + 1 < curPos)
                        input_stream.Backup(curPos - jjmatchedPos - 1);
                    if ((jjtoToken[jjmatchedKind >> 6] & (1UL << (jjmatchedKind & 077))) != 0L)
                    {
                        matchedToken = jjFillToken();
                        matchedToken.specialToken = specialToken;
                        TokenLexicalActions(matchedToken);
                        if (jjnewLexState[jjmatchedKind] != -1)
                            curLexState = jjnewLexState[jjmatchedKind];
                        return matchedToken;
                    }
                    else if ((jjtoSkip[jjmatchedKind >> 6] & (1UL << (jjmatchedKind & 077))) != 0L)
                    {
                        if ((jjtoSpecial[jjmatchedKind >> 6] & (1UL << (jjmatchedKind & 077))) != 0L)
                        {
                            matchedToken = jjFillToken();
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
                        if (jjnewLexState[jjmatchedKind] != -1)
                            curLexState = jjnewLexState[jjmatchedKind];
                        goto EOFLoop;
                    }
                    MoreLexicalActions();
                    if (jjnewLexState[jjmatchedKind] != -1)
                        curLexState = jjnewLexState[jjmatchedKind];
                    curPos = 0;
                    jjmatchedKind = 0x7fffffff;
                    try
                    {
                        curChar = input_stream.ReadChar();
                        continue;
                    }
                    catch (IOException e1) { }
                }
                int error_line = input_stream.GetEndLine();
                int error_column = input_stream.GetEndColumn();
                String error_after = null;
                bool EOFSeen = false;
                try { input_stream.ReadChar(); input_stream.Backup(1); }
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
                    input_stream.Backup(1);
                    error_after = curPos <= 1 ? "" : input_stream.GetImage();
                }
                throw new TokenMgrError(EOFSeen, curLexState, error_line, error_column, error_after, curChar, TokenMgrError.LEXICAL_ERROR);
            }
        }
    }

    void SkipLexicalActions(Token matchedToken)
    {
        switch (jjmatchedKind)
        {
            case 19:
                image.Append(input_stream.GetSuffix(jjimageLen + (lengthOfMatch = jjmatchedPos + 1)));
                RestoreBeginLineCol(); input_stream.Backup(1);
                break;
            default:
                break;
        }
    }
    void MoreLexicalActions()
    {
        jjimageLen += (lengthOfMatch = jjmatchedPos + 1);
        switch (jjmatchedKind)
        {
            case 21:
                image.Append(input_stream.GetSuffix(jjimageLen));
                jjimageLen = 0;
                input_stream.Backup(1);
                break;
            case 23:
                image.Append(input_stream.GetSuffix(jjimageLen));
                jjimageLen = 0;
                SaveBeginLineCol(input_stream.GetBeginLine(),
                                 input_stream.GetBeginColumn());
                break;
            default:
                break;
        }
    }
    void TokenLexicalActions(Token matchedToken)
    {
        switch (jjmatchedKind)
        {
            case 135:
                image.Append(jjstrLiteralImages[135]);
                lengthOfMatch = jjstrLiteralImages[135].Length;
                matchedToken.kind = GT;
                ((Token.GTToken)matchedToken).realKind = RUNSIGNEDSHIFT_VALUE;
                input_stream.Backup(2);
                matchedToken.image = ">";
                break;
            case 136:
                image.Append(jjstrLiteralImages[136]);
                lengthOfMatch = jjstrLiteralImages[136].Length;
                matchedToken.kind = GT;
                ((Token.GTToken)matchedToken).realKind = RSIGNEDSHIFT_VALUE;
                input_stream.Backup(1);
                matchedToken.image = ">";
                break;
            default:
                break;
        }
    }
    private void jjCheckNAdd(uint state)
    {
        if (jjrounds[state] != jjround)
        {
            jjstateSet[jjnewStateCnt++] = state;
            jjrounds[state] = jjround;
        }
    }
    private void jjAddStates(int start, int end)
    {
        do
        {
            jjstateSet[jjnewStateCnt++] = jjnextStates[start];
        } while (start++ != end);
    }
    private void jjCheckNAddTwoStates(uint state1, uint state2)
    {
        jjCheckNAdd(state1);
        jjCheckNAdd(state2);
    }

    private void jjCheckNAddStates(int start, int end)
    {
        do
        {
            jjCheckNAdd(jjnextStates[start]);
        } while (start++ != end);
    }

}
