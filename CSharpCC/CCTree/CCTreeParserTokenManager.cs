using System.Text;

namespace CSharpCC.CCTree;

/** Token Manager. */
public class CCTreeParserTokenManager : CCTreeParserConstants
{

    public TextWriter DebugStream = Console.Out;
    public void SetDebugStream(TextWriter ds) => DebugStream = ds;
    private int CCStopStringLiteralDfa_0(int pos, ulong active0, ulong active1, ulong active2)
    {
        switch (pos)
        {
            case 0:
                if ((active0 & 0xa0000L) != 0L || (active1 & 0x202000000000000L) != 0L)
                    return 2;
                if ((active1 & 0x20000000L) != 0L || (active2 & 0x8L) != 0L)
                    return 8;
                if ((active0 & 0xffffffffff000ffeL) != 0L || (active1 & 0x3ffL) != 0L || (active2 & 0x436L) != 0L)
                {
                    ccmatchedKind = 140;
                    return 32;
                }
                return -1;
            case 1:
                if ((active0 & 0x80000L) != 0L)
                    return 0;
                if ((active0 & 0xffffdfe7ff000ffeL) != 0L || (active1 & 0x3ffL) != 0L || (active2 & 0x436L) != 0L)
                {
                    if (ccmatchedPos != 1)
                    {
                        ccmatchedKind = 140;
                        ccmatchedPos = 1;
                    }
                    return 32;
                }
                if ((active0 & 0x201800000000L) != 0L)
                    return 32;
                return -1;
            case 2:
                if ((active0 & 0xffd9d7f7ff0007feL) != 0L || (active1 & 0x3bfL) != 0L || (active2 & 0x436L) != 0L)
                {
                    if (ccmatchedPos != 2)
                    {
                        ccmatchedKind = 140;
                        ccmatchedPos = 2;
                    }
                    return 32;
                }
                if ((active0 & 0x26080000000800L) != 0L || (active1 & 0x40L) != 0L)
                    return 32;
                return -1;
            case 3:
                if ((active0 & 0xff95c7d7a70004feL) != 0L || (active1 & 0x31dL) != 0L || (active2 & 0x432L) != 0L)
                {
                    ccmatchedKind = 140;
                    ccmatchedPos = 3;
                    return 32;
                }
                if ((active0 & 0x48102058000300L) != 0L || (active1 & 0xa2L) != 0L || (active2 & 0x4L) != 0L)
                    return 32;
                return -1;
            case 4:
                if ((active0 & 0xaf95c056030000beL) != 0L || (active1 & 0x111L) != 0L || (active2 & 0x432L) != 0L)
                {
                    if (ccmatchedPos != 4)
                    {
                        ccmatchedKind = 140;
                        ccmatchedPos = 4;
                    }
                    return 32;
                }
                if ((active0 & 0x50000781a4000440L) != 0L || (active1 & 0x20cL) != 0L)
                    return 32;
                return -1;
            case 5:
                if ((active0 & 0x3854246030004beL) != 0L || (active1 & 0x111L) != 0L || (active2 & 0x32L) != 0L)
                {
                    ccmatchedKind = 140;
                    ccmatchedPos = 5;
                    return 32;
                }
                if ((active0 & 0xac10801000000000L) != 0L || (active1 & 0x8L) != 0L || (active2 & 0x400L) != 0L)
                    return 32;
                return -1;
            case 6:
                if ((active0 & 0x2054002010004beL) != 0L || (active1 & 0x111L) != 0L || (active2 & 0x32L) != 0L)
                {
                    ccmatchedKind = 140;
                    ccmatchedPos = 6;
                    return 32;
                }
                if ((active0 & 0x180024402000000L) != 0L)
                    return 32;
                return -1;
            case 7:
                if ((active0 & 0x20540000000049eL) != 0L || (active1 & 0x11L) != 0L)
                {
                    ccmatchedKind = 140;
                    ccmatchedPos = 7;
                    return 32;
                }
                if ((active0 & 0x201000020L) != 0L || (active1 & 0x100L) != 0L || (active2 & 0x32L) != 0L)
                    return 32;
                return -1;
            case 8:
                if ((active0 & 0x140000000049cL) != 0L || (active1 & 0x1L) != 0L)
                {
                    ccmatchedKind = 140;
                    ccmatchedPos = 8;
                    return 32;
                }
                if ((active0 & 0x204000000000002L) != 0L || (active1 & 0x10L) != 0L)
                    return 32;
                return -1;
            case 9:
                if ((active0 & 0x48cL) != 0L || (active1 & 0x1L) != 0L)
                {
                    ccmatchedKind = 140;
                    ccmatchedPos = 9;
                    return 32;
                }
                if ((active0 & 0x1400000000010L) != 0L)
                    return 32;
                return -1;
            case 10:
                if ((active0 & 0x488L) != 0L || (active1 & 0x1L) != 0L)
                {
                    ccmatchedKind = 140;
                    ccmatchedPos = 10;
                    return 32;
                }
                if ((active0 & 0x4L) != 0L)
                    return 32;
                return -1;
            case 11:
                if ((active0 & 0x480L) != 0L)
                {
                    ccmatchedKind = 140;
                    ccmatchedPos = 11;
                    return 32;
                }
                if ((active0 & 0x8L) != 0L || (active1 & 0x1L) != 0L)
                    return 32;
                return -1;
            case 12:
                if ((active0 & 0x400L) != 0L)
                {
                    ccmatchedKind = 140;
                    ccmatchedPos = 12;
                    return 32;
                }
                if ((active0 & 0x80L) != 0L)
                    return 32;
                return -1;
            case 13:
                if ((active0 & 0x400L) != 0L)
                {
                    ccmatchedKind = 140;
                    ccmatchedPos = 13;
                    return 32;
                }
                return -1;
            default:
                return -1;
        }
    }
    private int CCStartNfa_0(int pos, ulong active0, ulong active1, ulong active2)
    {
        return CCMoveNfa_0(CCStopStringLiteralDfa_0(pos, active0, active1, active2), pos + 1);
    }
    private int CCStopAtPos(int pos, int kind)
    {
        ccmatchedKind = kind;
        ccmatchedPos = pos;
        return pos + 1;
    }
    private int CCMoveStringLiteralDfa0_0()
    {
        switch ((int)curChar)
        {
            case 9:
                return CCStopAtPos(0, 13);
            case 10:
                return CCStopAtPos(0, 14);
            case 12:
                return CCStopAtPos(0, 16);
            case 13:
                return CCStopAtPos(0, 15);
            case 32:
                return CCStopAtPos(0, 12);
            case 33:
                ccmatchedKind = 97;
                return CCMoveStringLiteralDfa1_0(0x0L, 0x20000000000L, 0x0L);
            case 35:
                return CCStopAtPos(0, 94);
            case 37:
                ccmatchedKind = 117;
                return CCMoveStringLiteralDfa1_0(0x0L, 0x2000000000000000L, 0x0L);
            case 38:
                ccmatchedKind = 114;
                return CCMoveStringLiteralDfa1_0(0x0L, 0x400080000000000L, 0x0L);
            case 40:
                return CCStopAtPos(0, 85);
            case 41:
                return CCStopAtPos(0, 86);
            case 42:
                ccmatchedKind = 112;
                return CCMoveStringLiteralDfa1_0(0x0L, 0x100000000000000L, 0x0L);
            case 43:
                ccmatchedKind = 110;
                return CCMoveStringLiteralDfa1_0(0x0L, 0x40100000000000L, 0x0L);
            case 44:
                return CCStopAtPos(0, 92);
            case 45:
                ccmatchedKind = 111;
                return CCMoveStringLiteralDfa1_0(0x0L, 0x80200000000000L, 0x0L);
            case 46:
                ccmatchedKind = 93;
                return CCMoveStringLiteralDfa1_0(0x0L, 0x0L, 0x8L);
            case 47:
                ccmatchedKind = 113;
                return CCMoveStringLiteralDfa1_0(0xa0000L, 0x200000000000000L, 0x0L);
            case 58:
                ccmatchedKind = 100;
                return CCMoveStringLiteralDfa1_0(0x0L, 0x2000000000L, 0x0L);
            case 59:
                return CCStopAtPos(0, 91);
            case 60:
                ccmatchedKind = 96;
                return CCMoveStringLiteralDfa1_0(0x0L, 0x8000000000L, 0x240L);
            case 61:
                ccmatchedKind = 95;
                return CCMoveStringLiteralDfa1_0(0x0L, 0x4000000000L, 0x0L);
            case 62:
                ccmatchedKind = 128;
                return CCMoveStringLiteralDfa1_0(0x0L, 0xc000010000000000L, 0x180L);
            case 63:
                return CCStopAtPos(0, 99);
            case 64:
                return CCStopAtPos(0, 139);
            case 69:
                return CCMoveStringLiteralDfa1_0(0x800L, 0x0L, 0x0L);
            case 73:
                return CCMoveStringLiteralDfa1_0(0x4L, 0x0L, 0x0L);
            case 74:
                return CCMoveStringLiteralDfa1_0(0x20L, 0x0L, 0x0L);
            case 76:
                return CCMoveStringLiteralDfa1_0(0x2L, 0x0L, 0x0L);
            case 77:
                return CCMoveStringLiteralDfa1_0(0x100L, 0x0L, 0x0L);
            case 80:
                return CCMoveStringLiteralDfa1_0(0x18L, 0x0L, 0x0L);
            case 83:
                return CCMoveStringLiteralDfa1_0(0x280L, 0x0L, 0x0L);
            case 84:
                return CCMoveStringLiteralDfa1_0(0x440L, 0x0L, 0x0L);
            case 91:
                return CCStopAtPos(0, 89);
            case 93:
                return CCStopAtPos(0, 90);
            case 94:
                ccmatchedKind = 116;
                return CCMoveStringLiteralDfa1_0(0x0L, 0x1000000000000000L, 0x0L);
            case 97:
                return CCMoveStringLiteralDfa1_0(0x1000000L, 0x0L, 0x400L);
            case 98:
                return CCMoveStringLiteralDfa1_0(0xe000000L, 0x0L, 0x0L);
            case 99:
                return CCMoveStringLiteralDfa1_0(0x3f0000000L, 0x0L, 0x0L);
            case 100:
                return CCMoveStringLiteralDfa1_0(0x1c00000000L, 0x0L, 0x0L);
            case 101:
                return CCMoveStringLiteralDfa1_0(0x6000000000L, 0x0L, 0x4L);
            case 102:
                return CCMoveStringLiteralDfa1_0(0xf8000000000L, 0x0L, 0x0L);
            case 103:
                return CCMoveStringLiteralDfa1_0(0x100000000000L, 0x0L, 0x0L);
            case 105:
                return CCMoveStringLiteralDfa1_0(0x7e00000000000L, 0x0L, 0x0L);
            case 108:
                return CCMoveStringLiteralDfa1_0(0x8000000000000L, 0x0L, 0x0L);
            case 110:
                return CCMoveStringLiteralDfa1_0(0x70000000000000L, 0x0L, 0x0L);
            case 112:
                return CCMoveStringLiteralDfa1_0(0x780000000000000L, 0x0L, 0x0L);
            case 114:
                return CCMoveStringLiteralDfa1_0(0x800000000000000L, 0x0L, 0x0L);
            case 115:
                return CCMoveStringLiteralDfa1_0(0xf000000000000000L, 0x1L, 0x2L);
            case 116:
                return CCMoveStringLiteralDfa1_0(0x0L, 0x7eL, 0x30L);
            case 118:
                return CCMoveStringLiteralDfa1_0(0x0L, 0x180L, 0x0L);
            case 119:
                return CCMoveStringLiteralDfa1_0(0x0L, 0x200L, 0x0L);
            case 123:
                return CCStopAtPos(0, 87);
            case 124:
                ccmatchedKind = 115;
                return CCMoveStringLiteralDfa1_0(0x0L, 0x800040000000000L, 0x0L);
            case 125:
                return CCStopAtPos(0, 88);
            case 126:
                return CCStopAtPos(0, 98);
            default:
                return CCMoveNfa_0(3, 0);
        }
    }
    private int CCMoveStringLiteralDfa1_0(ulong active0, ulong active1, ulong active2)
    {
        try { curChar = InputStream.ReadChar(); }
        catch (IOException)
        {
            CCStopStringLiteralDfa_0(0, active0, active1, active2);
            return 1;
        }
        switch ((int)curChar)
        {
            case 38:
                if ((active1 & 0x80000000000L) != 0L)
                    return CCStopAtPos(1, 107);
                break;
            case 42:
                if ((active0 & 0x80000L) != 0L)
                    return CCStartNfaWithStates_0(1, 19, 0);
                break;
            case 43:
                if ((active1 & 0x100000000000L) != 0L)
                    return CCStopAtPos(1, 108);
                break;
            case 45:
                if ((active1 & 0x200000000000L) != 0L)
                    return CCStopAtPos(1, 109);
                break;
            case 46:
                return CCMoveStringLiteralDfa2_0(active0, 0L, active1, 0L, active2, 0x8L);
            case 47:
                if ((active0 & 0x20000L) != 0L)
                    return CCStopAtPos(1, 17);
                break;
            case 58:
                if ((active1 & 0x2000000000L) != 0L)
                    return CCStopAtPos(1, 101);
                break;
            case 60:
                if ((active2 & 0x200L) != 0L)
                {
                    ccmatchedKind = 137;
                    ccmatchedPos = 1;
                }
                return CCMoveStringLiteralDfa2_0(active0, 0L, active1, 0L, active2, 0x40L);
            case 61:
                if ((active1 & 0x4000000000L) != 0L)
                    return CCStopAtPos(1, 102);
                else if ((active1 & 0x8000000000L) != 0L)
                    return CCStopAtPos(1, 103);
                else if ((active1 & 0x10000000000L) != 0L)
                    return CCStopAtPos(1, 104);
                else if ((active1 & 0x20000000000L) != 0L)
                    return CCStopAtPos(1, 105);
                else if ((active1 & 0x40000000000000L) != 0L)
                    return CCStopAtPos(1, 118);
                else if ((active1 & 0x80000000000000L) != 0L)
                    return CCStopAtPos(1, 119);
                else if ((active1 & 0x100000000000000L) != 0L)
                    return CCStopAtPos(1, 120);
                else if ((active1 & 0x200000000000000L) != 0L)
                    return CCStopAtPos(1, 121);
                else if ((active1 & 0x400000000000000L) != 0L)
                    return CCStopAtPos(1, 122);
                else if ((active1 & 0x800000000000000L) != 0L)
                    return CCStopAtPos(1, 123);
                else if ((active1 & 0x1000000000000000L) != 0L)
                    return CCStopAtPos(1, 124);
                else if ((active1 & 0x2000000000000000L) != 0L)
                    return CCStopAtPos(1, 125);
                break;
            case 62:
                if ((active1 & 0x8000000000000000L) != 0L)
                {
                    ccmatchedKind = 127;
                    ccmatchedPos = 1;
                }
                return CCMoveStringLiteralDfa2_0(active0, 0L, active1, 0x4000000000000000L, active2, 0x180L);
            case 65:
                return CCMoveStringLiteralDfa2_0(active0, 0x38L, active1, 0L, active2, 0L);
            case 71:
                return CCMoveStringLiteralDfa2_0(active0, 0x4L, active1, 0L, active2, 0L);
            case 75:
                return CCMoveStringLiteralDfa2_0(active0, 0x200L, active1, 0L, active2, 0L);
            case 79:
                return CCMoveStringLiteralDfa2_0(active0, 0xd42L, active1, 0L, active2, 0L);
            case 80:
                return CCMoveStringLiteralDfa2_0(active0, 0x80L, active1, 0L, active2, 0L);
            case 97:
                return CCMoveStringLiteralDfa2_0(active0, 0x90008030000000L, active1, 0L, active2, 0L);
            case 98:
                return CCMoveStringLiteralDfa2_0(active0, 0x1000000L, active1, 0L, active2, 0L);
            case 101:
                return CCMoveStringLiteralDfa2_0(active0, 0x820000400000000L, active1, 0L, active2, 0x10L);
            case 102:
                if ((active0 & 0x200000000000L) != 0L)
                    return CCStartNfaWithStates_0(1, 45, 32);
                break;
            case 104:
                return CCMoveStringLiteralDfa2_0(active0, 0x1000000040000000L, active1, 0x20eL, active2, 0L);
            case 105:
                return CCMoveStringLiteralDfa2_0(active0, 0x30000000000L, active1, 0L, active2, 0L);
            case 108:
                return CCMoveStringLiteralDfa2_0(active0, 0x42080000000L, active1, 0L, active2, 0L);
            case 109:
                return CCMoveStringLiteralDfa2_0(active0, 0xc00000000000L, active1, 0L, active2, 0L);
            case 110:
                return CCMoveStringLiteralDfa2_0(active0, 0x7000000000000L, active1, 0L, active2, 0x4L);
            case 111:
                if ((active0 & 0x800000000L) != 0L)
                {
                    ccmatchedKind = 35;
                    ccmatchedPos = 1;
                }
                return CCMoveStringLiteralDfa2_0(active0, 0x8181302000000L, active1, 0x180L, active2, 0L);
            case 114:
                return CCMoveStringLiteralDfa2_0(active0, 0x300000004000000L, active1, 0x70L, active2, 0L);
            case 115:
                return CCMoveStringLiteralDfa2_0(active0, 0L, active1, 0L, active2, 0x400L);
            case 116:
                return CCMoveStringLiteralDfa2_0(active0, 0x2000000000000000L, active1, 0L, active2, 0x2L);
            case 117:
                return CCMoveStringLiteralDfa2_0(active0, 0x4440000000000000L, active1, 0L, active2, 0L);
            case 119:
                return CCMoveStringLiteralDfa2_0(active0, 0x8000000000000000L, active1, 0L, active2, 0L);
            case 120:
                return CCMoveStringLiteralDfa2_0(active0, 0x4000000000L, active1, 0L, active2, 0L);
            case 121:
                return CCMoveStringLiteralDfa2_0(active0, 0x8000000L, active1, 0x1L, active2, 0x20L);
            case 124:
                if ((active1 & 0x40000000000L) != 0L)
                    return CCStopAtPos(1, 106);
                break;
            default:
                break;
        }
        return CCStartNfa_0(0, active0, active1, active2);
    }
    private int CCMoveStringLiteralDfa2_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return CCStartNfa_0(0, old0, old1, old2);
        try { curChar = InputStream.ReadChar(); }
        catch (IOException)
        {
            CCStopStringLiteralDfa_0(1, active0, active1, active2);
            return 2;
        }
        switch ((int)curChar)
        {
            case 46:
                if ((active2 & 0x8L) != 0L)
                    return CCStopAtPos(2, 131);
                break;
            case 61:
                if ((active2 & 0x40L) != 0L)
                    return CCStopAtPos(2, 134);
                else if ((active2 & 0x80L) != 0L)
                    return CCStopAtPos(2, 135);
                break;
            case 62:
                if ((active1 & 0x4000000000000000L) != 0L)
                {
                    ccmatchedKind = 126;
                    ccmatchedPos = 2;
                }
                return CCMoveStringLiteralDfa3_0(active0, 0L, active1, 0L, active2, 0x100L);
            case 69:
                return CCMoveStringLiteralDfa3_0(active0, 0x80L, active1, 0L, active2, 0L);
            case 70:
                if ((active0 & 0x800L) != 0L)
                    return CCStartNfaWithStates_0(2, 11, 32);
                break;
            case 73:
                return CCMoveStringLiteralDfa3_0(active0, 0x200L, active1, 0L, active2, 0L);
            case 75:
                return CCMoveStringLiteralDfa3_0(active0, 0x440L, active1, 0L, active2, 0L);
            case 78:
                return CCMoveStringLiteralDfa3_0(active0, 0x4L, active1, 0L, active2, 0L);
            case 79:
                return CCMoveStringLiteralDfa3_0(active0, 0x2L, active1, 0L, active2, 0L);
            case 82:
                return CCMoveStringLiteralDfa3_0(active0, 0x118L, active1, 0L, active2, 0L);
            case 86:
                return CCMoveStringLiteralDfa3_0(active0, 0x20L, active1, 0L, active2, 0L);
            case 97:
                return CCMoveStringLiteralDfa3_0(active0, 0x20000000c0000000L, active1, 0x10L, active2, 0L);
            case 98:
                return CCMoveStringLiteralDfa3_0(active0, 0x400000000000000L, active1, 0L, active2, 0L);
            case 99:
                return CCMoveStringLiteralDfa3_0(active0, 0x80000000000000L, active1, 0L, active2, 0L);
            case 101:
                return CCMoveStringLiteralDfa3_0(active0, 0x4000000L, active1, 0L, active2, 0L);
            case 102:
                return CCMoveStringLiteralDfa3_0(active0, 0x400000000L, active1, 0L, active2, 0L);
            case 105:
                return CCMoveStringLiteralDfa3_0(active0, 0x8100000000000000L, active1, 0x282L, active2, 0L);
            case 108:
                return CCMoveStringLiteralDfa3_0(active0, 0x40008000000000L, active1, 0x100L, active2, 0L);
            case 109:
                return CCMoveStringLiteralDfa3_0(active0, 0L, active1, 0L, active2, 0x10L);
            case 110:
                return CCMoveStringLiteralDfa3_0(active0, 0x8030300000000L, active1, 0x1L, active2, 0L);
            case 111:
                return CCMoveStringLiteralDfa3_0(active0, 0x1200040002000000L, active1, 0L, active2, 0L);
            case 112:
                return CCMoveStringLiteralDfa3_0(active0, 0x4000c00000000000L, active1, 0L, active2, 0x20L);
            case 114:
                if ((active0 & 0x80000000000L) != 0L)
                    return CCStartNfaWithStates_0(2, 43, 32);
                return CCMoveStringLiteralDfa3_0(active0, 0L, active1, 0xcL, active2, 0x2L);
            case 115:
                return CCMoveStringLiteralDfa3_0(active0, 0x1002011000000L, active1, 0L, active2, 0x400L);
            case 116:
                if ((active0 & 0x2000000000000L) != 0L)
                {
                    ccmatchedKind = 49;
                    ccmatchedPos = 2;
                }
                return CCMoveStringLiteralDfa3_0(active0, 0x814104028000000L, active1, 0L, active2, 0L);
            case 117:
                return CCMoveStringLiteralDfa3_0(active0, 0x1000000000L, active1, 0x20L, active2, 0x4L);
            case 119:
                if ((active0 & 0x20000000000000L) != 0L)
                    return CCStartNfaWithStates_0(2, 53, 32);
                break;
            case 121:
                if ((active1 & 0x40L) != 0L)
                    return CCStartNfaWithStates_0(2, 70, 32);
                break;
            default:
                break;
        }
        return CCStartNfa_0(1, active0, active1, active2);
    }
    private int CCMoveStringLiteralDfa3_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return CCStartNfa_0(1, old0, old1, old2);
        try { curChar = InputStream.ReadChar(); }
        catch (IOException)
        {
            CCStopStringLiteralDfa_0(2, active0, active1, active2);
            return 3;
        }
        switch ((int)curChar)
        {
            case 61:
                if ((active2 & 0x100L) != 0L)
                    return CCStopAtPos(3, 136);
                break;
            case 65:
                return CCMoveStringLiteralDfa4_0(active0, 0x20L, active1, 0L, active2, 0L);
            case 67:
                return CCMoveStringLiteralDfa4_0(active0, 0x80L, active1, 0L, active2, 0L);
            case 69:
                if ((active0 & 0x100L) != 0L)
                    return CCStartNfaWithStates_0(3, 8, 32);
                return CCMoveStringLiteralDfa4_0(active0, 0x440L, active1, 0L, active2, 0L);
            case 75:
                return CCMoveStringLiteralDfa4_0(active0, 0x2L, active1, 0L, active2, 0L);
            case 79:
                return CCMoveStringLiteralDfa4_0(active0, 0x4L, active1, 0L, active2, 0L);
            case 80:
                if ((active0 & 0x200L) != 0L)
                    return CCStartNfaWithStates_0(3, 9, 32);
                break;
            case 83:
                return CCMoveStringLiteralDfa4_0(active0, 0x18L, active1, 0L, active2, 0L);
            case 97:
                return CCMoveStringLiteralDfa4_0(active0, 0x70404000000L, active1, 0x100L, active2, 0L);
            case 98:
                return CCMoveStringLiteralDfa4_0(active0, 0x1000000000L, active1, 0L, active2, 0L);
            case 99:
                return CCMoveStringLiteralDfa4_0(active0, 0x20000000L, active1, 0x1L, active2, 0L);
            case 100:
                if ((active1 & 0x80L) != 0L)
                    return CCStartNfaWithStates_0(3, 71, 32);
                break;
            case 101:
                if ((active0 & 0x8000000L) != 0L)
                    return CCStartNfaWithStates_0(3, 27, 32);
                else if ((active0 & 0x10000000L) != 0L)
                    return CCStartNfaWithStates_0(3, 28, 32);
                else if ((active0 & 0x2000000000L) != 0L)
                    return CCStartNfaWithStates_0(3, 37, 32);
                else if ((active1 & 0x20L) != 0L)
                    return CCStartNfaWithStates_0(3, 69, 32);
                return CCMoveStringLiteralDfa4_0(active0, 0x4004004000000000L, active1, 0L, active2, 0x420L);
            case 103:
                if ((active0 & 0x8000000000000L) != 0L)
                    return CCStartNfaWithStates_0(3, 51, 32);
                break;
            case 105:
                return CCMoveStringLiteralDfa4_0(active0, 0x10000000000000L, active1, 0L, active2, 0x2L);
            case 107:
                return CCMoveStringLiteralDfa4_0(active0, 0x80000000000000L, active1, 0L, active2, 0L);
            case 108:
                if ((active0 & 0x40000000000000L) != 0L)
                    return CCStartNfaWithStates_0(3, 54, 32);
                return CCMoveStringLiteralDfa4_0(active0, 0x400400002000000L, active1, 0x200L, active2, 0L);
            case 109:
                if ((active2 & 0x4L) != 0L)
                    return CCStartNfaWithStates_0(3, 130, 32);
                break;
            case 110:
                return CCMoveStringLiteralDfa4_0(active0, 0L, active1, 0x10L, active2, 0L);
            case 111:
                if ((active0 & 0x100000000000L) != 0L)
                    return CCStartNfaWithStates_0(3, 44, 32);
                return CCMoveStringLiteralDfa4_0(active0, 0x800000000000L, active1, 0xcL, active2, 0L);
            case 112:
                return CCMoveStringLiteralDfa4_0(active0, 0L, active1, 0L, active2, 0x10L);
            case 114:
                if ((active0 & 0x40000000L) != 0L)
                    return CCStartNfaWithStates_0(3, 30, 32);
                return CCMoveStringLiteralDfa4_0(active0, 0x1000000000000000L, active1, 0L, active2, 0L);
            case 115:
                if ((active1 & 0x2L) != 0L)
                    return CCStartNfaWithStates_0(3, 65, 32);
                return CCMoveStringLiteralDfa4_0(active0, 0x8180000000L, active1, 0L, active2, 0L);
            case 116:
                return CCMoveStringLiteralDfa4_0(active0, 0xa201000201000000L, active1, 0L, active2, 0L);
            case 117:
                return CCMoveStringLiteralDfa4_0(active0, 0x800000000000000L, active1, 0L, active2, 0L);
            case 118:
                return CCMoveStringLiteralDfa4_0(active0, 0x100000000000000L, active1, 0L, active2, 0L);
            default:
                break;
        }
        return CCStartNfa_0(2, active0, active1, active2);
    }
    private int CCMoveStringLiteralDfa4_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return CCStartNfa_0(2, old0, old1, old2);
        try { curChar = InputStream.ReadChar(); }
        catch (IOException)
        {
            CCStopStringLiteralDfa_0(3, active0, active1, active2);
            return 4;
        }
        switch ((int)curChar)
        {
            case 65:
                return CCMoveStringLiteralDfa5_0(active0, 0x2L, active1, 0L, active2, 0L);
            case 67:
                return CCMoveStringLiteralDfa5_0(active0, 0x20L, active1, 0L, active2, 0L);
            case 69:
                return CCMoveStringLiteralDfa5_0(active0, 0x18L, active1, 0L, active2, 0L);
            case 73:
                return CCMoveStringLiteralDfa5_0(active0, 0x80L, active1, 0L, active2, 0L);
            case 78:
                if ((active0 & 0x40L) != 0L)
                {
                    ccmatchedKind = 6;
                    ccmatchedPos = 4;
                }
                return CCMoveStringLiteralDfa5_0(active0, 0x400L, active1, 0L, active2, 0L);
            case 82:
                return CCMoveStringLiteralDfa5_0(active0, 0x4L, active1, 0L, active2, 0L);
            case 97:
                return CCMoveStringLiteralDfa5_0(active0, 0x181000000000000L, active1, 0L, active2, 0L);
            case 99:
                return CCMoveStringLiteralDfa5_0(active0, 0x8000000000000000L, active1, 0L, active2, 0x2L);
            case 101:
                if ((active0 & 0x8000000000L) != 0L)
                    return CCStartNfaWithStates_0(4, 39, 32);
                else if ((active1 & 0x200L) != 0L)
                    return CCStartNfaWithStates_0(4, 73, 32);
                return CCMoveStringLiteralDfa5_0(active0, 0x200400002000000L, active1, 0L, active2, 0L);
            case 104:
                if ((active0 & 0x20000000L) != 0L)
                    return CCStartNfaWithStates_0(4, 29, 32);
                return CCMoveStringLiteralDfa5_0(active0, 0L, active1, 0x1L, active2, 0L);
            case 105:
                return CCMoveStringLiteralDfa5_0(active0, 0x2400000200000000L, active1, 0L, active2, 0L);
            case 107:
                if ((active0 & 0x4000000L) != 0L)
                    return CCStartNfaWithStates_0(4, 26, 32);
                break;
            case 108:
                if ((active0 & 0x10000000000L) != 0L)
                {
                    ccmatchedKind = 40;
                    ccmatchedPos = 4;
                }
                return CCMoveStringLiteralDfa5_0(active0, 0x21000000000L, active1, 0L, active2, 0x10L);
            case 110:
                return CCMoveStringLiteralDfa5_0(active0, 0x4000000000L, active1, 0L, active2, 0x20L);
            case 114:
                if ((active0 & 0x4000000000000000L) != 0L)
                    return CCStartNfaWithStates_0(4, 62, 32);
                return CCMoveStringLiteralDfa5_0(active0, 0x804800001000000L, active1, 0L, active2, 0x400L);
            case 115:
                if ((active0 & 0x80000000L) != 0L)
                    return CCStartNfaWithStates_0(4, 31, 32);
                return CCMoveStringLiteralDfa5_0(active0, 0L, active1, 0x10L, active2, 0L);
            case 116:
                if ((active0 & 0x100000000L) != 0L)
                    return CCStartNfaWithStates_0(4, 32, 32);
                else if ((active0 & 0x40000000000L) != 0L)
                    return CCStartNfaWithStates_0(4, 42, 32);
                else if ((active0 & 0x1000000000000000L) != 0L)
                    return CCStartNfaWithStates_0(4, 60, 32);
                return CCMoveStringLiteralDfa5_0(active0, 0L, active1, 0x100L, active2, 0L);
            case 117:
                return CCMoveStringLiteralDfa5_0(active0, 0x400000000L, active1, 0L, active2, 0L);
            case 118:
                return CCMoveStringLiteralDfa5_0(active0, 0x10000000000000L, active1, 0L, active2, 0L);
            case 119:
                if ((active1 & 0x4L) != 0L)
                {
                    ccmatchedKind = 66;
                    ccmatchedPos = 4;
                }
                return CCMoveStringLiteralDfa5_0(active0, 0L, active1, 0x8L, active2, 0L);
            default:
                break;
        }
        return CCStartNfa_0(3, active0, active1, active2);
    }
    private int CCMoveStringLiteralDfa5_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return CCStartNfa_0(3, old0, old1, old2);
        try { curChar = InputStream.ReadChar(); }
        catch (IOException)
        {
            CCStopStringLiteralDfa_0(4, active0, active1, active2);
            return 5;
        }
        switch ((int)curChar)
        {
            case 65:
                return CCMoveStringLiteralDfa6_0(active0, 0x80L, active1, 0L, active2, 0L);
            case 69:
                return CCMoveStringLiteralDfa6_0(active0, 0x4L, active1, 0L, active2, 0L);
            case 72:
                return CCMoveStringLiteralDfa6_0(active0, 0x2L, active1, 0L, active2, 0L);
            case 79:
                return CCMoveStringLiteralDfa6_0(active0, 0x20L, active1, 0L, active2, 0L);
            case 82:
                return CCMoveStringLiteralDfa6_0(active0, 0x18L, active1, 0L, active2, 0L);
            case 95:
                return CCMoveStringLiteralDfa6_0(active0, 0x400L, active1, 0L, active2, 0L);
            case 97:
                return CCMoveStringLiteralDfa6_0(active0, 0x3000000L, active1, 0L, active2, 0x30L);
            case 99:
                if ((active0 & 0x400000000000000L) != 0L)
                    return CCStartNfaWithStates_0(5, 58, 32);
                else if ((active0 & 0x2000000000000000L) != 0L)
                    return CCStartNfaWithStates_0(5, 61, 32);
                return CCMoveStringLiteralDfa6_0(active0, 0x200000000000000L, active1, 0L, active2, 0L);
            case 100:
                return CCMoveStringLiteralDfa6_0(active0, 0x4000000000L, active1, 0L, active2, 0L);
            case 101:
                if ((active0 & 0x1000000000L) != 0L)
                    return CCStartNfaWithStates_0(5, 36, 32);
                else if ((active0 & 0x10000000000000L) != 0L)
                    return CCStartNfaWithStates_0(5, 52, 32);
                break;
            case 102:
                return CCMoveStringLiteralDfa6_0(active0, 0x4000000000000L, active1, 0L, active2, 0L);
            case 103:
                return CCMoveStringLiteralDfa6_0(active0, 0x80000000000000L, active1, 0L, active2, 0L);
            case 104:
                if ((active0 & 0x8000000000000000L) != 0L)
                    return CCStartNfaWithStates_0(5, 63, 32);
                break;
            case 105:
                return CCMoveStringLiteralDfa6_0(active0, 0L, active1, 0x110L, active2, 0L);
            case 108:
                return CCMoveStringLiteralDfa6_0(active0, 0x20400000000L, active1, 0L, active2, 0L);
            case 109:
                return CCMoveStringLiteralDfa6_0(active0, 0x400000000000L, active1, 0L, active2, 0L);
            case 110:
                if ((active0 & 0x800000000000000L) != 0L)
                    return CCStartNfaWithStates_0(5, 59, 32);
                return CCMoveStringLiteralDfa6_0(active0, 0x1000200000000L, active1, 0L, active2, 0L);
            case 114:
                return CCMoveStringLiteralDfa6_0(active0, 0L, active1, 0x1L, active2, 0L);
            case 115:
                if ((active1 & 0x8L) != 0L)
                    return CCStartNfaWithStates_0(5, 67, 32);
                break;
            case 116:
                if ((active0 & 0x800000000000L) != 0L)
                    return CCStartNfaWithStates_0(5, 47, 32);
                else if ((active2 & 0x400L) != 0L)
                    return CCStartNfaWithStates_0(5, 138, 32);
                return CCMoveStringLiteralDfa6_0(active0, 0x100000000000000L, active1, 0L, active2, 0x2L);
            default:
                break;
        }
        return CCStartNfa_0(4, active0, active1, active2);
    }
    private int CCMoveStringLiteralDfa6_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return CCStartNfa_0(4, old0, old1, old2);
        try { curChar = InputStream.ReadChar(); }
        catch (IOException)
        {
            CCStopStringLiteralDfa_0(5, active0, active1, active2);
            return 6;
        }
        switch ((int)curChar)
        {
            case 68:
                return CCMoveStringLiteralDfa7_0(active0, 0x20L, active1, 0L, active2, 0L);
            case 69:
                return CCMoveStringLiteralDfa7_0(active0, 0x2L, active1, 0L, active2, 0L);
            case 76:
                return CCMoveStringLiteralDfa7_0(active0, 0x80L, active1, 0L, active2, 0L);
            case 77:
                return CCMoveStringLiteralDfa7_0(active0, 0x400L, active1, 0L, active2, 0L);
            case 95:
                return CCMoveStringLiteralDfa7_0(active0, 0x1cL, active1, 0L, active2, 0L);
            case 97:
                return CCMoveStringLiteralDfa7_0(active0, 0x4000000000000L, active1, 0L, active2, 0L);
            case 99:
                return CCMoveStringLiteralDfa7_0(active0, 0x1000001000000L, active1, 0L, active2, 0L);
            case 101:
                if ((active0 & 0x80000000000000L) != 0L)
                    return CCStartNfaWithStates_0(6, 55, 32);
                else if ((active0 & 0x100000000000000L) != 0L)
                    return CCStartNfaWithStates_0(6, 56, 32);
                return CCMoveStringLiteralDfa7_0(active0, 0x400000000000L, active1, 0x10L, active2, 0L);
            case 102:
                return CCMoveStringLiteralDfa7_0(active0, 0L, active1, 0L, active2, 0x2L);
            case 108:
                return CCMoveStringLiteralDfa7_0(active0, 0L, active1, 0x100L, active2, 0L);
            case 109:
                return CCMoveStringLiteralDfa7_0(active0, 0L, active1, 0L, active2, 0x20L);
            case 110:
                if ((active0 & 0x2000000L) != 0L)
                    return CCStartNfaWithStates_0(6, 25, 32);
                break;
            case 111:
                return CCMoveStringLiteralDfa7_0(active0, 0L, active1, 0x1L, active2, 0L);
            case 115:
                if ((active0 & 0x4000000000L) != 0L)
                    return CCStartNfaWithStates_0(6, 38, 32);
                break;
            case 116:
                if ((active0 & 0x400000000L) != 0L)
                    return CCStartNfaWithStates_0(6, 34, 32);
                return CCMoveStringLiteralDfa7_0(active0, 0x200000000000000L, active1, 0L, active2, 0x10L);
            case 117:
                return CCMoveStringLiteralDfa7_0(active0, 0x200000000L, active1, 0L, active2, 0L);
            case 121:
                if ((active0 & 0x20000000000L) != 0L)
                    return CCStartNfaWithStates_0(6, 41, 32);
                break;
            default:
                break;
        }
        return CCStartNfa_0(5, active0, active1, active2);
    }
    private int CCMoveStringLiteralDfa7_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return CCStartNfa_0(5, old0, old1, old2);
        try { curChar = InputStream.ReadChar(); }
        catch (IOException)
        {
            CCStopStringLiteralDfa_0(6, active0, active1, active2);
            return 7;
        }
        switch ((int)curChar)
        {
            case 65:
                return CCMoveStringLiteralDfa8_0(active0, 0x2L, active1, 0L, active2, 0L);
            case 66:
                return CCMoveStringLiteralDfa8_0(active0, 0x8L, active1, 0L, active2, 0L);
            case 67:
                return CCMoveStringLiteralDfa8_0(active0, 0x4L, active1, 0L, active2, 0L);
            case 69:
                if ((active0 & 0x20L) != 0L)
                    return CCStartNfaWithStates_0(7, 5, 32);
                return CCMoveStringLiteralDfa8_0(active0, 0x10L, active1, 0L, active2, 0L);
            case 71:
                return CCMoveStringLiteralDfa8_0(active0, 0x400L, active1, 0L, active2, 0L);
            case 95:
                return CCMoveStringLiteralDfa8_0(active0, 0x80L, active1, 0L, active2, 0L);
            case 99:
                return CCMoveStringLiteralDfa8_0(active0, 0x4000000000000L, active1, 0L, active2, 0L);
            case 101:
                if ((active0 & 0x200000000L) != 0L)
                    return CCStartNfaWithStates_0(7, 33, 32);
                else if ((active1 & 0x100L) != 0L)
                    return CCStartNfaWithStates_0(7, 72, 32);
                else if ((active2 & 0x10L) != 0L)
                    return CCStartNfaWithStates_0(7, 132, 32);
                else if ((active2 & 0x20L) != 0L)
                    return CCStartNfaWithStates_0(7, 133, 32);
                return CCMoveStringLiteralDfa8_0(active0, 0x201000000000000L, active1, 0L, active2, 0L);
            case 110:
                return CCMoveStringLiteralDfa8_0(active0, 0x400000000000L, active1, 0x11L, active2, 0L);
            case 112:
                if ((active2 & 0x2L) != 0L)
                    return CCStartNfaWithStates_0(7, 129, 32);
                break;
            case 116:
                if ((active0 & 0x1000000L) != 0L)
                    return CCStartNfaWithStates_0(7, 24, 32);
                break;
            default:
                break;
        }
        return CCStartNfa_0(6, active0, active1, active2);
    }
    private int CCMoveStringLiteralDfa8_0(ulong old0, ulong active0, ulong old1, ulong active1, ulong old2, ulong active2)
    {
        if (((active0 &= old0) | (active1 &= old1) | (active2 &= old2)) == 0L)
            return CCStartNfa_0(6, old0, old1, old2);
        try { curChar = InputStream.ReadChar(); }
        catch (IOException)
        {
            CCStopStringLiteralDfa_0(7, active0, active1, 0L);
            return 8;
        }
        switch ((int)curChar)
        {
            case 65:
                return CCMoveStringLiteralDfa9_0(active0, 0x4L, active1, 0L);
            case 68:
                if ((active0 & 0x2L) != 0L)
                    return CCStartNfaWithStates_0(8, 1, 32);
                break;
            case 69:
                return CCMoveStringLiteralDfa9_0(active0, 0x8L, active1, 0L);
            case 78:
                return CCMoveStringLiteralDfa9_0(active0, 0x10L, active1, 0L);
            case 82:
                return CCMoveStringLiteralDfa9_0(active0, 0x400L, active1, 0L);
            case 84:
                return CCMoveStringLiteralDfa9_0(active0, 0x80L, active1, 0L);
            case 100:
                if ((active0 & 0x200000000000000L) != 0L)
                    return CCStartNfaWithStates_0(8, 57, 32);
                break;
            case 101:
                if ((active0 & 0x4000000000000L) != 0L)
                    return CCStartNfaWithStates_0(8, 50, 32);
                break;
            case 105:
                return CCMoveStringLiteralDfa9_0(active0, 0L, active1, 0x1L);
            case 111:
                return CCMoveStringLiteralDfa9_0(active0, 0x1000000000000L, active1, 0L);
            case 116:
                if ((active1 & 0x10L) != 0L)
                    return CCStartNfaWithStates_0(8, 68, 32);
                return CCMoveStringLiteralDfa9_0(active0, 0x400000000000L, active1, 0L);
            default:
                break;
        }
        return CCStartNfa_0(7, active0, active1, 0L);
    }
    private int CCMoveStringLiteralDfa9_0(ulong old0, ulong active0, ulong old1, ulong active1)
    {
        if (((active0 &= old0) | (active1 &= old1)) == 0L)
            return CCStartNfa_0(7, old0, old1, 0L);
        try { curChar = InputStream.ReadChar(); }
        catch (IOException)
        {
            CCStopStringLiteralDfa_0(8, active0, active1, 0L);
            return 9;
        }
        switch ((int)curChar)
        {
            case 68:
                if ((active0 & 0x10L) != 0L)
                    return CCStartNfaWithStates_0(9, 4, 32);
                break;
            case 71:
                return CCMoveStringLiteralDfa10_0(active0, 0x8L, active1, 0L);
            case 79:
                return CCMoveStringLiteralDfa10_0(active0, 0x80L, active1, 0L);
            case 83:
                return CCMoveStringLiteralDfa10_0(active0, 0x4L, active1, 0L);
            case 95:
                return CCMoveStringLiteralDfa10_0(active0, 0x400L, active1, 0L);
            case 102:
                if ((active0 & 0x1000000000000L) != 0L)
                    return CCStartNfaWithStates_0(9, 48, 32);
                break;
            case 115:
                if ((active0 & 0x400000000000L) != 0L)
                    return CCStartNfaWithStates_0(9, 46, 32);
                break;
            case 122:
                return CCMoveStringLiteralDfa10_0(active0, 0L, active1, 0x1L);
            default:
                break;
        }
        return CCStartNfa_0(8, active0, active1, 0L);
    }
    private int CCMoveStringLiteralDfa10_0(ulong old0, ulong active0, ulong old1, ulong active1)
    {
        if (((active0 &= old0) | (active1 &= old1)) == 0L)
            return CCStartNfa_0(8, old0, old1, 0L);
        try { curChar = InputStream.ReadChar(); }
        catch (IOException)
        {
            CCStopStringLiteralDfa_0(9, active0, active1, 0L);
            return 10;
        }
        switch ((int)curChar)
        {
            case 68:
                return CCMoveStringLiteralDfa11_0(active0, 0x400L, active1, 0L);
            case 69:
                if ((active0 & 0x4L) != 0L)
                    return CCStartNfaWithStates_0(10, 2, 32);
                break;
            case 73:
                return CCMoveStringLiteralDfa11_0(active0, 0x8L, active1, 0L);
            case 75:
                return CCMoveStringLiteralDfa11_0(active0, 0x80L, active1, 0L);
            case 101:
                return CCMoveStringLiteralDfa11_0(active0, 0L, active1, 0x1L);
            default:
                break;
        }
        return CCStartNfa_0(9, active0, active1, 0L);
    }
    private int CCMoveStringLiteralDfa11_0(ulong old0, ulong active0, ulong old1, ulong active1)
    {
        if (((active0 &= old0) | (active1 &= old1)) == 0L)
            return CCStartNfa_0(9, old0, old1, 0L);
        try { curChar = InputStream.ReadChar(); }
        catch (IOException)
        {
            CCStopStringLiteralDfa_0(10, active0, active1, 0L);
            return 11;
        }
        switch ((int)curChar)
        {
            case 69:
                return CCMoveStringLiteralDfa12_0(active0, 0x480L, active1, 0L);
            case 78:
                if ((active0 & 0x8L) != 0L)
                    return CCStartNfaWithStates_0(11, 3, 32);
                break;
            case 100:
                if ((active1 & 0x1L) != 0L)
                    return CCStartNfaWithStates_0(11, 64, 32);
                break;
            default:
                break;
        }
        return CCStartNfa_0(10, active0, active1, 0L);
    }
    private int CCMoveStringLiteralDfa12_0(ulong old0, ulong active0, ulong old1, ulong active1)
    {
        if (((active0 &= old0) | (active1 &= old1)) == 0L)
            return CCStartNfa_0(10, old0, old1, 0L);
        try { curChar = InputStream.ReadChar(); }
        catch (IOException)
        {
            CCStopStringLiteralDfa_0(11, active0, 0L, 0L);
            return 12;
        }
        switch ((int)curChar)
        {
            case 67:
                return CCMoveStringLiteralDfa13_0(active0, 0x400L);
            case 78:
                if ((active0 & 0x80L) != 0L)
                    return CCStartNfaWithStates_0(12, 7, 32);
                break;
            default:
                break;
        }
        return CCStartNfa_0(11, active0, 0L, 0L);
    }
    private int CCMoveStringLiteralDfa13_0(ulong old0, ulong active0)
    {
        if (((active0 &= old0)) == 0L)
            return CCStartNfa_0(11, old0, 0L, 0L);
        try { curChar = InputStream.ReadChar(); }
        catch (IOException)
        {
            CCStopStringLiteralDfa_0(12, active0, 0L, 0L);
            return 13;
        }
        switch ((int)curChar)
        {
            case 76:
                return CCMoveStringLiteralDfa14_0(active0, 0x400L);
            default:
                break;
        }
        return CCStartNfa_0(12, active0, 0L, 0L);
    }
    private int CCMoveStringLiteralDfa14_0(ulong old0, ulong active0)
    {
        if (((active0 &= old0)) == 0L)
            return CCStartNfa_0(12, old0, 0L, 0L);
        try { curChar = InputStream.ReadChar(); }
        catch (IOException)
        {
            CCStopStringLiteralDfa_0(13, active0, 0L, 0L);
            return 14;
        }
        switch ((int)curChar)
        {
            case 83:
                if ((active0 & 0x400L) != 0L)
                    return CCStartNfaWithStates_0(14, 10, 32);
                break;
            default:
                break;
        }
        return CCStartNfa_0(13, active0, 0L, 0L);
    }
    private int CCStartNfaWithStates_0(int pos, int kind, int state)
    {
        ccmatchedKind = kind;
        ccmatchedPos = pos;
        try { curChar = InputStream.ReadChar(); }
        catch (IOException) { return pos + 1; }
        return CCMoveNfa_0(state, pos + 1);
    }
    static readonly ulong[] ccbitVec0 = {
   0xfffffffffffffffeL, 0xffffffffffffffffL, 0xffffffffffffffffL, 0xffffffffffffffffL
};
    static readonly ulong[] ccbitVec2 = {
   0x0L, 0x0L, 0xffffffffffffffffL, 0xffffffffffffffffL
};
    static readonly ulong[] ccbitVec3 = {
   0xfff0000000200002L, 0xffffffffffffdfffL, 0xfffff00f7fffffffL, 0x12000000007fffffL
};
    static readonly ulong[] ccbitVec4 = {
   0x0L, 0x0L, 0x420043c00000000L, 0xff7fffffff7fffffL
};
    static readonly ulong[] ccbitVec5 = {
   0xffffcffffffffL, 0xffffffffffff0000L, 0xf9ff3fffffffffffL, 0x401f00030003L
};
    static readonly ulong[] ccbitVec6 = {
   0x0L, 0x400000000000000L, 0xfffffffbffffd740L, 0xffffffcff7fffL
};
    static readonly ulong[] ccbitVec7 = {
   0xffffffffffffffffL, 0xffffffffffffffffL, 0xfffffffffffff003L, 0x33fffffffff199fL
};
    static readonly ulong[] ccbitVec8 = {
   0xfffe000000000000L, 0xfffffffe027fffffL, 0xffL, 0x707ffffff0000L
};
    static readonly ulong[] ccbitVec9 = {
   0x7fffffe00000000L, 0xfffe0000000007ffL, 0xffffffffffffffffL, 0x1c000060002fffffL
};
    static readonly ulong[] ccbitVec10 = {
   0x1ffffffd0000L, 0x0L, 0x3fffffffffL, 0x0L
};
    static readonly ulong[] ccbitVec11 = {
   0x23ffffffffffffe0L, 0x3ff010000L, 0x3c5fdfffff99fe0L, 0xf0003b0000000L
};
    static readonly ulong[] ccbitVec12 = {
   0x36dfdfffff987e0L, 0x1c00005e000000L, 0x23edfdfffffbafe0L, 0x100010000L
};
    static readonly ulong[] ccbitVec13 = {
   0x23cdfdfffff99fe0L, 0x3b0000000L, 0x3bfc718d63dc7e0L, 0x0L
};
    static readonly ulong[] ccbitVec14 = {
   0x3effdfffffddfe0L, 0x300000000L, 0x3effdfffffddfe0L, 0x340000000L
};
    static readonly ulong[] ccbitVec15 = {
   0x3fffdfffffddfe0L, 0x300000000L, 0x2ffbfffffc7fffe0L, 0x7fL
};
    static readonly ulong[] ccbitVec16 = {
   0x800dfffffffffffeL, 0x7fL, 0x200decaefef02596L, 0x3000005fL
};
    static readonly ulong[] ccbitVec17 = {
   0x1L, 0x7fffffffeffL, 0xf00L, 0x0L
};
    static readonly ulong[] ccbitVec18 = {
   0x6fbffffffffL, 0x3f0000L, 0xffffffff00000000L, 0x7fffffffff003fL
};
    static readonly ulong[] ccbitVec19 = {
   0xffffffffffffffffL, 0xffffffff83ffffffL, 0xffffff07ffffffffL, 0x3ffffffffffffffL
};
    static readonly ulong[] ccbitVec20 = {
   0xffffffffffffff7fL, 0xffffffff3d7f3d7fL, 0x7f3d7fffffff3d7fL, 0xffff7fffff7f7f3dL
};
    static readonly ulong[] ccbitVec21 = {
   0xffffffff7f3d7fffL, 0x7ffff7fL, 0xffffffff00000000L, 0x1fffffffffffffL
};
    static readonly ulong[] ccbitVec22 = {
   0xffffffffffffffffL, 0x7f9fffffffffffL, 0xffffffff07fffffeL, 0x7ffffffffffL
};
    static readonly ulong[] ccbitVec23 = {
   0x0L, 0x0L, 0xfffffffffffffL, 0x8000000L
};
    static readonly ulong[] ccbitVec24 = {
   0xffffffff00000000L, 0xffffffffffffffL, 0x1ffffffffffL, 0x0L
};
    static readonly ulong[] ccbitVec25 = {
   0xffffffffffffffffL, 0xffffffffffffffffL, 0xffffffff0fffffffL, 0x3ffffffffffffffL
};
    static readonly ulong[] ccbitVec26 = {
   0xffffffff3f3fffffL, 0x3fffffffaaff3f3fL, 0x5fdfffffffffffffL, 0x1fdc1fff0fcf1fdcL
};
    static readonly ulong[] ccbitVec27 = {
   0x8000000000000000L, 0x8000000000000001L, 0xffff00000000L, 0x0L
};
    static readonly ulong[] ccbitVec28 = {
   0x3fbbd503e2ffc84L, 0xffffffff00000000L, 0xfL, 0x0L
};
    static readonly ulong[] ccbitVec29 = {
   0x73e03fe000000e0L, 0xfffffffffffffffeL, 0xfffffffe601fffffL, 0x7fffffffffffffffL
};
    static readonly ulong[] ccbitVec30 = {
   0xfffe1fffffffffe0L, 0xffffffffffffffffL, 0xffffff00007fffL, 0x0L
};
    static readonly ulong[] ccbitVec31 = {
   0xffffffffffffffffL, 0xffffffffffffffffL, 0x3fffffffffffffL, 0x0L
};
    static readonly ulong[] ccbitVec32 = {
   0xffffffffffffffffL, 0xffffffffffffffffL, 0x3fffffffffL, 0x0L
};
    static readonly ulong[] ccbitVec33 = {
   0xffffffffffffffffL, 0xffffffffffffffffL, 0x1fffL, 0x0L
};
    static readonly ulong[] ccbitVec34 = {
   0xffffffffffffffffL, 0xffffffffffffffffL, 0xfffffffffL, 0x0L
};
    static readonly ulong[] ccbitVec35 = {
   0x3fffffffffffL, 0x0L, 0x0L, 0x0L
};
    static readonly ulong[] ccbitVec36 = {
   0x5f7ffdffa0f8007fL, 0xffffffffffffffdbL, 0x3ffffffffffffL, 0xfffffffffff80000L
};
    static readonly ulong[] ccbitVec37 = {
   0x3fffffffffffffffL, 0xffffffffffff0000L, 0xfffffffffffcffffL, 0xfff0000000000ffL
};
    static readonly ulong[] ccbitVec38 = {
   0x18000000000000L, 0xffd702000000e000L, 0xffffffffffffffffL, 0x1fffffffffffffffL
};
    static readonly ulong[] ccbitVec39 = {
   0x87fffffe00000010L, 0xffffffe007fffffeL, 0x7fffffffffffffffL, 0x631cfcfcfcL
};
    static readonly ulong[] ccbitVec40 = {
   0x0L, 0x0L, 0x420043cffffffffL, 0xff7fffffff7fffffL
};
    static readonly ulong[] ccbitVec41 = {
   0xffffffffffffffffL, 0x400000700007fffL, 0xfffffffbffffd740L, 0xffffffcff7fffL
};
    static readonly ulong[] ccbitVec42 = {
   0xffffffffffffffffL, 0xffffffffffffffffL, 0xfffffffffffff07bL, 0x33fffffffff199fL
};
    static readonly ulong[] ccbitVec43 = {
   0xfffe000000000000L, 0xfffffffe027fffffL, 0xbbfffffbfffe00ffL, 0x707ffffff0016L
};
    static readonly ulong[] ccbitVec44 = {
   0x7fffffe00000000L, 0xffff03ff003fffffL, 0xffffffffffffffffL, 0x1fff3dff9fefffffL
};
    static readonly ulong[] ccbitVec45 = {
   0xffff1fffffff8000L, 0x7ffL, 0x1ffffffffffffL, 0x0L
};
    static readonly ulong[] ccbitVec46 = {
   0xf3ffffffffffffeeL, 0xffcfff1f3fffL, 0xd3c5fdfffff99feeL, 0xfffcfb080399fL
};
    static readonly ulong[] ccbitVec47 = {
   0xd36dfdfffff987e4L, 0x1fffc05e003987L, 0xf3edfdfffffbafeeL, 0xffc100013bbfL
};
    static readonly ulong[] ccbitVec48 = {
   0xf3cdfdfffff99feeL, 0xffc3b0c0398fL, 0xc3bfc718d63dc7ecL, 0xff8000803dc7L
};
    static readonly ulong[] ccbitVec49 = {
   0xc3effdfffffddfeeL, 0xffc300603ddfL, 0xc3effdfffffddfecL, 0xffc340603ddfL
};
    static readonly ulong[] ccbitVec50 = {
   0xc3fffdfffffddfecL, 0xffc300803dcfL, 0x2ffbfffffc7fffecL, 0xc0000ff5f847fL
};
    static readonly ulong[] ccbitVec51 = {
   0x87fffffffffffffeL, 0x3ff7fffL, 0x3bffecaefef02596L, 0x33ff3f5fL
};
    static readonly ulong[] ccbitVec52 = {
   0xc2a003ff03000001L, 0xfffe07fffffffeffL, 0x1ffffffffeff0fdfL, 0x40L
};
    static readonly ulong[] ccbitVec53 = {
   0x3c7f6fbffffffffL, 0x3ff03ffL, 0xffffffff00000000L, 0x7fffffffff003fL
};
    static readonly ulong[] ccbitVec54 = {
   0xffffffff7f3d7fffL, 0x3fe0007ffff7fL, 0xffffffff00000000L, 0x1fffffffffffffL
};
    static readonly ulong[] ccbitVec55 = {
   0x0L, 0x0L, 0xffffffffffffffffL, 0x3ff080fffffL
};
    static readonly ulong[] ccbitVec56 = {
   0xffffffff03ff7800L, 0xffffffffffffffL, 0x3ffffffffffL, 0x0L
};
    static readonly ulong[] ccbitVec57 = {
   0x80007c000000f000L, 0x8000fc0000000001L, 0xffff00000000L, 0x21fff0000L
};
    static readonly ulong[] ccbitVec58 = {
   0x73efffe000000e0L, 0xfffffffffffffffeL, 0xfffffffe661fffffL, 0x7fffffffffffffffL
};
    static readonly ulong[] ccbitVec59 = {
   0x5f7ffdffe0f8007fL, 0xffffffffffffffdbL, 0x3ffffffffffffL, 0xfffffffffff80000L
};
    static readonly ulong[] ccbitVec60 = {
   0x18000f00000000L, 0xffd702000000e000L, 0xffffffffffffffffL, 0x9fffffffffffffffL
};
    static readonly ulong[] ccbitVec61 = {
   0x87fffffe03ff0010L, 0xffffffe007fffffeL, 0x7fffffffffffffffL, 0xe0000631cfcfcfcL
};
    private int CCMoveNfa_0(int startState, int curPos)
    {
        int startsAt = 0;
        ccnewStateCnt = 65;
        int i = 1;
        ccstateSet[0] = startState;
        int kind = 0x7fffffff;
        for (; ; )
        {
            if (++ccround == 0x7fffffff)
                ReInitRounds();
            if (curChar < 64)
            {
                ulong l = 1UL << curChar;
                do
                {
                    switch (ccstateSet[--i])
                    {
                        case 3:
                            if ((0x3ff000000000000L & l) != 0L)
                                ccCheckNAddStates(0, 6);
                            else if (curChar == 36)
                            {
                                if (kind > 140)
                                    kind = 140;
                                ccCheckNAdd(32);
                            }
                            else if (curChar == 34)
                                ccCheckNAddStates(7, 9);
                            else if (curChar == 39)
                                ccAddStates(10, 11);
                            else if (curChar == 46)
                                ccCheckNAdd(8);
                            else if (curChar == 47)
                                ccstateSet[ccnewStateCnt++] = 2;
                            if ((0x3fe000000000000L & l) != 0L)
                            {
                                if (kind > 74)
                                    kind = 74;
                                ccCheckNAddTwoStates(5, 6);
                            }
                            else if (curChar == 48)
                            {
                                if (kind > 74)
                                    kind = 74;
                                ccCheckNAddStates(12, 16);
                            }
                            break;
                        case 0:
                            if (curChar == 42)
                                ccstateSet[ccnewStateCnt++] = 1;
                            break;
                        case 1:
                            if ((0xffff7fffffffffffL & l) != 0L && kind > 18)
                                kind = 18;
                            break;
                        case 2:
                            if (curChar == 42)
                                ccstateSet[ccnewStateCnt++] = 0;
                            break;
                        case 4:
                            if ((0x3fe000000000000L & l) == 0L)
                                break;
                            if (kind > 74)
                                kind = 74;
                            ccCheckNAddTwoStates(5, 6);
                            break;
                        case 5:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 74)
                                kind = 74;
                            ccCheckNAddTwoStates(5, 6);
                            break;
                        case 7:
                            if (curChar == 46)
                                ccCheckNAdd(8);
                            break;
                        case 8:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 78)
                                kind = 78;
                            ccCheckNAddStates(17, 19);
                            break;
                        case 10:
                            if ((0x280000000000L & l) != 0L)
                                ccCheckNAdd(11);
                            break;
                        case 11:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 78)
                                kind = 78;
                            ccCheckNAddTwoStates(11, 12);
                            break;
                        case 13:
                            if (curChar == 39)
                                ccAddStates(10, 11);
                            break;
                        case 14:
                            if ((0xffffff7fffffdbffL & l) != 0L)
                                ccCheckNAdd(15);
                            break;
                        case 15:
                            if (curChar == 39 && kind > 83)
                                kind = 83;
                            break;
                        case 17:
                            if ((0x8400000000L & l) != 0L)
                                ccCheckNAdd(15);
                            break;
                        case 18:
                            if ((0xff000000000000L & l) != 0L)
                                ccCheckNAddTwoStates(19, 15);
                            break;
                        case 19:
                            if ((0xff000000000000L & l) != 0L)
                                ccCheckNAdd(15);
                            break;
                        case 20:
                            if ((0xf000000000000L & l) != 0L)
                                ccstateSet[ccnewStateCnt++] = 21;
                            break;
                        case 21:
                            if ((0xff000000000000L & l) != 0L)
                                ccCheckNAdd(19);
                            break;
                        case 22:
                            if (curChar == 34)
                                ccCheckNAddStates(7, 9);
                            break;
                        case 23:
                            if ((0xfffffffbffffdbffL & l) != 0L)
                                ccCheckNAddStates(7, 9);
                            break;
                        case 25:
                            if ((0x8400000000L & l) != 0L)
                                ccCheckNAddStates(7, 9);
                            break;
                        case 26:
                            if (curChar == 34 && kind > 84)
                                kind = 84;
                            break;
                        case 27:
                            if ((0xff000000000000L & l) != 0L)
                                ccCheckNAddStates(20, 23);
                            break;
                        case 28:
                            if ((0xff000000000000L & l) != 0L)
                                ccCheckNAddStates(7, 9);
                            break;
                        case 29:
                            if ((0xf000000000000L & l) != 0L)
                                ccstateSet[ccnewStateCnt++] = 30;
                            break;
                        case 30:
                            if ((0xff000000000000L & l) != 0L)
                                ccCheckNAdd(28);
                            break;
                        case 31:
                            if (curChar != 36)
                                break;
                            if (kind > 140)
                                kind = 140;
                            ccCheckNAdd(32);
                            break;
                        case 32:
                            if ((0x3ff00100fffc1ffL & l) == 0L)
                                break;
                            if (kind > 140)
                                kind = 140;
                            ccCheckNAdd(32);
                            break;
                        case 33:
                            if ((0x3ff000000000000L & l) != 0L)
                                ccCheckNAddStates(0, 6);
                            break;
                        case 34:
                            if ((0x3ff000000000000L & l) != 0L)
                                ccCheckNAddStates(24, 26);
                            break;
                        case 36:
                            if ((0x280000000000L & l) != 0L)
                                ccCheckNAdd(37);
                            break;
                        case 37:
                            if ((0x3ff000000000000L & l) != 0L)
                                ccCheckNAddTwoStates(37, 12);
                            break;
                        case 38:
                            if ((0x3ff000000000000L & l) != 0L)
                                ccCheckNAddTwoStates(38, 39);
                            break;
                        case 40:
                            if ((0x280000000000L & l) != 0L)
                                ccCheckNAdd(41);
                            break;
                        case 41:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 78)
                                kind = 78;
                            ccCheckNAddTwoStates(41, 12);
                            break;
                        case 42:
                            if ((0x3ff000000000000L & l) != 0L)
                                ccCheckNAddTwoStates(42, 43);
                            break;
                        case 43:
                            if (curChar != 46)
                                break;
                            if (kind > 78)
                                kind = 78;
                            ccCheckNAddStates(27, 29);
                            break;
                        case 44:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 78)
                                kind = 78;
                            ccCheckNAddStates(27, 29);
                            break;
                        case 46:
                            if ((0x280000000000L & l) != 0L)
                                ccCheckNAdd(47);
                            break;
                        case 47:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 78)
                                kind = 78;
                            ccCheckNAddTwoStates(47, 12);
                            break;
                        case 48:
                            if (curChar != 48)
                                break;
                            if (kind > 74)
                                kind = 74;
                            ccCheckNAddStates(12, 16);
                            break;
                        case 50:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 74)
                                kind = 74;
                            ccCheckNAddTwoStates(50, 6);
                            break;
                        case 51:
                            if ((0xff000000000000L & l) == 0L)
                                break;
                            if (kind > 74)
                                kind = 74;
                            ccCheckNAddTwoStates(51, 6);
                            break;
                        case 53:
                            if ((0x3ff000000000000L & l) != 0L)
                                ccAddStates(30, 31);
                            break;
                        case 54:
                            if (curChar == 46)
                                ccCheckNAdd(55);
                            break;
                        case 55:
                            if ((0x3ff000000000000L & l) != 0L)
                                ccCheckNAddTwoStates(55, 56);
                            break;
                        case 57:
                            if ((0x280000000000L & l) != 0L)
                                ccCheckNAdd(58);
                            break;
                        case 58:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 78)
                                kind = 78;
                            ccCheckNAddTwoStates(58, 12);
                            break;
                        case 60:
                            if ((0x3ff000000000000L & l) != 0L)
                                ccCheckNAddStates(32, 34);
                            break;
                        case 61:
                            if (curChar == 46)
                                ccCheckNAdd(62);
                            break;
                        case 63:
                            if ((0x280000000000L & l) != 0L)
                                ccCheckNAdd(64);
                            break;
                        case 64:
                            if ((0x3ff000000000000L & l) == 0L)
                                break;
                            if (kind > 78)
                                kind = 78;
                            ccCheckNAddTwoStates(64, 12);
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
                    switch (ccstateSet[--i])
                    {
                        case 3:
                            if ((0x7fffffe87fffffeL & l) == 0L)
                                break;
                            if (kind > 140)
                                kind = 140;
                            ccCheckNAdd(32);
                            break;
                        case 1:
                            if (kind > 18)
                                kind = 18;
                            break;
                        case 6:
                            if ((0x100000001000L & l) != 0L && kind > 74)
                                kind = 74;
                            break;
                        case 9:
                            if ((0x2000000020L & l) != 0L)
                                ccAddStates(35, 36);
                            break;
                        case 12:
                            if ((0x5000000050L & l) != 0L && kind > 78)
                                kind = 78;
                            break;
                        case 14:
                            if ((0xffffffffefffffffL & l) != 0L)
                                ccCheckNAdd(15);
                            break;
                        case 16:
                            if (curChar == 92)
                                ccAddStates(37, 39);
                            break;
                        case 17:
                            if ((0x14404410000000L & l) != 0L)
                                ccCheckNAdd(15);
                            break;
                        case 23:
                            if ((0xffffffffefffffffL & l) != 0L)
                                ccCheckNAddStates(7, 9);
                            break;
                        case 24:
                            if (curChar == 92)
                                ccAddStates(40, 42);
                            break;
                        case 25:
                            if ((0x14404410000000L & l) != 0L)
                                ccCheckNAddStates(7, 9);
                            break;
                        case 32:
                            if ((0x87fffffe87fffffeL & l) == 0L)
                                break;
                            if (kind > 140)
                                kind = 140;
                            ccCheckNAdd(32);
                            break;
                        case 35:
                            if ((0x2000000020L & l) != 0L)
                                ccAddStates(43, 44);
                            break;
                        case 39:
                            if ((0x2000000020L & l) != 0L)
                                ccAddStates(45, 46);
                            break;
                        case 45:
                            if ((0x2000000020L & l) != 0L)
                                ccAddStates(47, 48);
                            break;
                        case 49:
                            if ((0x100000001000000L & l) != 0L)
                                ccCheckNAdd(50);
                            break;
                        case 50:
                            if ((0x7e0000007eL & l) == 0L)
                                break;
                            if (kind > 74)
                                kind = 74;
                            ccCheckNAddTwoStates(50, 6);
                            break;
                        case 52:
                            if ((0x100000001000000L & l) != 0L)
                                ccCheckNAddTwoStates(53, 54);
                            break;
                        case 53:
                            if ((0x7e0000007eL & l) != 0L)
                                ccCheckNAddTwoStates(53, 54);
                            break;
                        case 55:
                            if ((0x7e0000007eL & l) != 0L)
                                ccAddStates(49, 50);
                            break;
                        case 56:
                            if ((0x1000000010000L & l) != 0L)
                                ccAddStates(51, 52);
                            break;
                        case 59:
                            if ((0x100000001000000L & l) != 0L)
                                ccCheckNAdd(60);
                            break;
                        case 60:
                            if ((0x7e0000007eL & l) != 0L)
                                ccCheckNAddStates(32, 34);
                            break;
                        case 62:
                            if ((0x1000000010000L & l) != 0L)
                                ccAddStates(53, 54);
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
                    switch (ccstateSet[--i])
                    {
                        case 3:
                            if (!ccCanMove_1(hiByte, i1, i2, l1, l2))
                                break;
                            if (kind > 140)
                                kind = 140;
                            ccCheckNAdd(32);
                            break;
                        case 1:
                            if (ccCanMove_0(hiByte, i1, i2, l1, l2) && kind > 18)
                                kind = 18;
                            break;
                        case 14:
                            if (ccCanMove_0(hiByte, i1, i2, l1, l2))
                                ccstateSet[ccnewStateCnt++] = 15;
                            break;
                        case 23:
                            if (ccCanMove_0(hiByte, i1, i2, l1, l2))
                                ccAddStates(7, 9);
                            break;
                        case 32:
                            if (!ccCanMove_2(hiByte, i1, i2, l1, l2))
                                break;
                            if (kind > 140)
                                kind = 140;
                            ccCheckNAdd(32);
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
            if ((i = ccnewStateCnt) == (startsAt = 65 - (ccnewStateCnt = startsAt)))
                return curPos;
            try { curChar = InputStream.ReadChar(); }
            catch (IOException) { return curPos; }
        }
    }
    private int ccMoveStringLiteralDfa0_3() => (int)curChar switch
    {
        42 => ccMoveStringLiteralDfa1_3(0x400000L),
        _ => 1,
    };
    private int ccMoveStringLiteralDfa1_3(ulong active0)
    {
        try { curChar = InputStream.ReadChar(); }
        catch (IOException)
        {
            return 1;
        }
        switch ((int)curChar)
        {
            case 47:
                if ((active0 & 0x400000L) != 0L)
                    return CCStopAtPos(1, 22);
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
                ulong l = 1UL << curChar;
                do
                {
                    switch (ccstateSet[--i])
                    {
                        case 0:
                            if ((0x2400L & l) != 0L)
                            {
                                if (kind > 20)
                                    kind = 20;
                            }
                            if (curChar == 13)
                                ccstateSet[ccnewStateCnt++] = 1;
                            break;
                        case 1:
                            if (curChar == 10 && kind > 20)
                                kind = 20;
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
                ulong l = 1UL << (curChar & 077);
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
                ulong l1 = 1UL << (hiByte & 077);
                int i2 = (curChar & 0xff) >> 6;
                ulong l2 = 1UL << (curChar & 077);
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
            try { curChar = InputStream.ReadChar(); }
            catch (IOException) { return curPos; }
        }
    }
    private int ccMoveStringLiteralDfa0_2() => (int)curChar switch
    {
        42 => ccMoveStringLiteralDfa1_2(0x200000L),
        _ => 1,
    };
    private int ccMoveStringLiteralDfa1_2(ulong active0)
    {
        try { curChar = InputStream.ReadChar(); }
        catch (IOException)
        {
            return 1;
        }
        switch ((int)curChar)
        {
            case 47:
                if ((active0 & 0x200000L) != 0L)
                    return CCStopAtPos(1, 21);
                break;
            default:
                return 2;
        }
        return 2;
    }
    static readonly int[] ccnextStates = {
   34, 35, 12, 38, 39, 42, 43, 23, 24, 26, 14, 16, 49, 51, 6, 52,
   59, 8, 9, 12, 23, 24, 28, 26, 34, 35, 12, 44, 45, 12, 53, 54,
   60, 61, 62, 10, 11, 17, 18, 20, 25, 27, 29, 36, 37, 40, 41, 46,
   47, 55, 56, 57, 58, 63, 64,
};
    private static bool ccCanMove_0(int hiByte, int i1, int i2, ulong l1, ulong l2)
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
    private static bool ccCanMove_1(int hiByte, int i1, int i2, ulong l1, ulong l2)
    {
        switch (hiByte)
        {
            case 0:
                return ((ccbitVec4[i2] & l2) != 0L);
            case 2:
                return ((ccbitVec5[i2] & l2) != 0L);
            case 3:
                return ((ccbitVec6[i2] & l2) != 0L);
            case 4:
                return ((ccbitVec7[i2] & l2) != 0L);
            case 5:
                return ((ccbitVec8[i2] & l2) != 0L);
            case 6:
                return ((ccbitVec9[i2] & l2) != 0L);
            case 7:
                return ((ccbitVec10[i2] & l2) != 0L);
            case 9:
                return ((ccbitVec11[i2] & l2) != 0L);
            case 10:
                return ((ccbitVec12[i2] & l2) != 0L);
            case 11:
                return ((ccbitVec13[i2] & l2) != 0L);
            case 12:
                return ((ccbitVec14[i2] & l2) != 0L);
            case 13:
                return ((ccbitVec15[i2] & l2) != 0L);
            case 14:
                return ((ccbitVec16[i2] & l2) != 0L);
            case 15:
                return ((ccbitVec17[i2] & l2) != 0L);
            case 16:
                return ((ccbitVec18[i2] & l2) != 0L);
            case 17:
                return ((ccbitVec19[i2] & l2) != 0L);
            case 18:
                return ((ccbitVec20[i2] & l2) != 0L);
            case 19:
                return ((ccbitVec21[i2] & l2) != 0L);
            case 20:
                return ((ccbitVec0[i2] & l2) != 0L);
            case 22:
                return ((ccbitVec22[i2] & l2) != 0L);
            case 23:
                return ((ccbitVec23[i2] & l2) != 0L);
            case 24:
                return ((ccbitVec24[i2] & l2) != 0L);
            case 30:
                return ((ccbitVec25[i2] & l2) != 0L);
            case 31:
                return ((ccbitVec26[i2] & l2) != 0L);
            case 32:
                return ((ccbitVec27[i2] & l2) != 0L);
            case 33:
                return ((ccbitVec28[i2] & l2) != 0L);
            case 48:
                return ((ccbitVec29[i2] & l2) != 0L);
            case 49:
                return ((ccbitVec30[i2] & l2) != 0L);
            case 77:
                return ((ccbitVec31[i2] & l2) != 0L);
            case 159:
                return ((ccbitVec32[i2] & l2) != 0L);
            case 164:
                return ((ccbitVec33[i2] & l2) != 0L);
            case 215:
                return ((ccbitVec34[i2] & l2) != 0L);
            case 250:
                return ((ccbitVec35[i2] & l2) != 0L);
            case 251:
                return ((ccbitVec36[i2] & l2) != 0L);
            case 253:
                return ((ccbitVec37[i2] & l2) != 0L);
            case 254:
                return ((ccbitVec38[i2] & l2) != 0L);
            case 255:
                return ((ccbitVec39[i2] & l2) != 0L);
            default:
                if ((ccbitVec3[i1] & l1) != 0L)
                    return true;
                return false;
        }
    }
    private static bool ccCanMove_2(int hiByte, int i1, int i2, ulong l1, ulong l2)
    {
        switch (hiByte)
        {
            case 0:
                return ((ccbitVec40[i2] & l2) != 0L);
            case 2:
                return ((ccbitVec5[i2] & l2) != 0L);
            case 3:
                return ((ccbitVec41[i2] & l2) != 0L);
            case 4:
                return ((ccbitVec42[i2] & l2) != 0L);
            case 5:
                return ((ccbitVec43[i2] & l2) != 0L);
            case 6:
                return ((ccbitVec44[i2] & l2) != 0L);
            case 7:
                return ((ccbitVec45[i2] & l2) != 0L);
            case 9:
                return ((ccbitVec46[i2] & l2) != 0L);
            case 10:
                return ((ccbitVec47[i2] & l2) != 0L);
            case 11:
                return ((ccbitVec48[i2] & l2) != 0L);
            case 12:
                return ((ccbitVec49[i2] & l2) != 0L);
            case 13:
                return ((ccbitVec50[i2] & l2) != 0L);
            case 14:
                return ((ccbitVec51[i2] & l2) != 0L);
            case 15:
                return ((ccbitVec52[i2] & l2) != 0L);
            case 16:
                return ((ccbitVec53[i2] & l2) != 0L);
            case 17:
                return ((ccbitVec19[i2] & l2) != 0L);
            case 18:
                return ((ccbitVec20[i2] & l2) != 0L);
            case 19:
                return ((ccbitVec54[i2] & l2) != 0L);
            case 20:
                return ((ccbitVec0[i2] & l2) != 0L);
            case 22:
                return ((ccbitVec22[i2] & l2) != 0L);
            case 23:
                return ((ccbitVec55[i2] & l2) != 0L);
            case 24:
                return ((ccbitVec56[i2] & l2) != 0L);
            case 30:
                return ((ccbitVec25[i2] & l2) != 0L);
            case 31:
                return ((ccbitVec26[i2] & l2) != 0L);
            case 32:
                return ((ccbitVec57[i2] & l2) != 0L);
            case 33:
                return ((ccbitVec28[i2] & l2) != 0L);
            case 48:
                return ((ccbitVec58[i2] & l2) != 0L);
            case 49:
                return ((ccbitVec30[i2] & l2) != 0L);
            case 77:
                return ((ccbitVec31[i2] & l2) != 0L);
            case 159:
                return ((ccbitVec32[i2] & l2) != 0L);
            case 164:
                return ((ccbitVec33[i2] & l2) != 0L);
            case 215:
                return ((ccbitVec34[i2] & l2) != 0L);
            case 250:
                return ((ccbitVec35[i2] & l2) != 0L);
            case 251:
                return ((ccbitVec59[i2] & l2) != 0L);
            case 253:
                return ((ccbitVec37[i2] & l2) != 0L);
            case 254:
                return ((ccbitVec60[i2] & l2) != 0L);
            case 255:
                return ((ccbitVec61[i2] & l2) != 0L);
            default:
                if ((ccbitVec3[i1] & l1) != 0L)
                    return true;
                return false;
        }
    }

    /** Token literal values. */
    public static readonly String[] ccstrLiteralImages = {
        "",
        "\x4c\x4f\x4f\x4b\x41\x48\x45\x41\x44",
        "\x49\x47\x4e\x4f\x52\x45\x5f\x43\x41\x53\x45",
        "\x50\x41\x52\x53\x45\x52\x5f\x42\x45\x47\x49\x4e",
        "\x50\x41\x52\x53\x45\x52\x5f\x45\x4e\x44",
        "\x4a\x41\x56\x41\x43\x4f\x44\x45",
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
        "\x61\x62\x73\x74\x72\x61\x63\x74",
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
        "\x23",
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
        "\x73\x74\x72\x69\x63\x74\x66\x70",
        "\x65\x6e\x75\x6d",
        "\x2e\x2e\x2e",
        "\x74\x65\x6d\x70\x6c\x61\x74\x65",
        "\x74\x79\x70\x65\x6e\x61\x6d\x65",
        "\x3c\x3c\x3d",
        "\x3e\x3e\x3d",
        "\x3e\x3e\x3e\x3d",
        "\x3c\x3c",
        "\x61\x73\x73\x65\x72\x74",
        "\x40",
        null,
        null,
        null,
        };

    /** Lexer state names. */
    public static readonly string[] lexStateNames = {
       "DEFAULT",
       "IN_SINGLE_LINE_COMMENT",
       "IN_FORMAL_COMMENT",
       "IN_MULTI_LINE_COMMENT",
    };

    /** Lex State array. */
    public static readonly int[] ccnewLexState = {
   -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1, 2, 3, 0, 0, 0, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
   -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
};
    static readonly ulong[] cctoToken = {
   unchecked((ulong)0xffffffffff000fffL), unchecked((ulong)0xfffffffffff847ffL), 0x1fffL,
};
    static readonly ulong[] cctoSkip = {
   0x71f000L, 0x0L, 0x0L,
};
    static readonly ulong[] cctoSpecial = {
   0x71f000L, 0x0L, 0x0L,
};
    static readonly ulong[] cctoMore = {
   0x8e0000L, 0x0L, 0x0L,
};
    protected CSharpCharStream InputStream;
    private readonly int[] ccrounds = new int[65];
    private readonly int[] ccstateSet = new int[130];
    private readonly StringBuilder ccimage = new ();
    private StringBuilder image;// = jjimage;
    private int ccimageLen;
    private int lengthOfMatch;
    protected char curChar;
    /** Constructor. */
    public CCTreeParserTokenManager(CSharpCharStream stream)
    {
        if (CSharpCharStream.StaticFlag)
            throw new Error("ERROR: Cannot use a static CharStream class with a non-static lexical analyzer.");
        InputStream = stream;
    }

    /** Constructor. */
    public CCTreeParserTokenManager(CSharpCharStream stream, int lexState)
        : this(stream)
    {
        SwitchTo(lexState);
    }

    /** Reinitialise parser. */
    public void ReInit(CSharpCharStream stream)
    {
        ccmatchedPos = ccnewStateCnt = 0;
        curLexState = defaultLexState;
        InputStream = stream;
        ReInitRounds();
    }
    private void ReInitRounds()
    {
        int i;
        ccround = unchecked((int)0x80000001);
        for (i = 65; i-- > 0;)
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

    protected Token CCFillToken()
    {
        Token t;
        String curTokenImage;
        int beginLine;
        int endLine;
        int beginColumn;
        int endColumn;
        string im = ccstrLiteralImages[ccmatchedKind];
        curTokenImage = im ?? InputStream.GetImage();
        beginLine = InputStream.BeginLine;
        beginColumn = InputStream.BeginColumn;
        endLine = InputStream.EndLine;
        endColumn = InputStream.EndColumn;
        t = Token.NewToken(ccmatchedKind, curTokenImage);

        t.BeginLine = beginLine;
        t.EndLine = endLine;
        t.BeginColumn = beginColumn;
        t.EndColumn = endColumn;

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
                curChar = InputStream.BeginToken();
            }
            catch (IOException)
            {
                ccmatchedKind = 0;
                matchedToken = CCFillToken();
                matchedToken.SpecialToken = specialToken;
                return matchedToken;
            }
            image = ccimage;
            image.Capacity=0;
            ccimageLen = 0;

            for (; ; )
            {
                switch (curLexState)
                {
                    case 0:
                        ccmatchedKind = 0x7fffffff;
                        ccmatchedPos = 0;
                        curPos = CCMoveStringLiteralDfa0_0();
                        break;
                    case 1:
                        ccmatchedKind = 0x7fffffff;
                        ccmatchedPos = 0;
                        curPos = ccMoveStringLiteralDfa0_1();
                        if (ccmatchedPos == 0 && ccmatchedKind > 23)
                        {
                            ccmatchedKind = 23;
                        }
                        break;
                    case 2:
                        ccmatchedKind = 0x7fffffff;
                        ccmatchedPos = 0;
                        curPos = ccMoveStringLiteralDfa0_2();
                        if (ccmatchedPos == 0 && ccmatchedKind > 23)
                        {
                            ccmatchedKind = 23;
                        }
                        break;
                    case 3:
                        ccmatchedKind = 0x7fffffff;
                        ccmatchedPos = 0;
                        curPos = ccMoveStringLiteralDfa0_3();
                        if (ccmatchedPos == 0 && ccmatchedKind > 23)
                        {
                            ccmatchedKind = 23;
                        }
                        break;
                }
                if (ccmatchedKind != 0x7fffffff)
                {
                    if (ccmatchedPos + 1 < curPos)
                        InputStream.Backup(curPos - ccmatchedPos - 1);
                    if ((cctoToken[ccmatchedKind >> 6] & (1UL << (ccmatchedKind & 077))) != 0L)
                    {
                        matchedToken = CCFillToken();
                        matchedToken.SpecialToken = specialToken;
                        TokenLexicalActions(matchedToken);
                        if (ccnewLexState[ccmatchedKind] != -1)
                            curLexState = ccnewLexState[ccmatchedKind];
                        return matchedToken;
                    }
                    else if ((cctoSkip[ccmatchedKind >> 6] & (1UL << (ccmatchedKind & 077))) != 0L)
                    {
                        if ((cctoSpecial[ccmatchedKind >> 6] & (1UL << (ccmatchedKind & 077))) != 0L)
                        {
                            matchedToken = CCFillToken();
                            if (specialToken == null)
                                specialToken = matchedToken;
                            else
                            {
                                matchedToken.SpecialToken = specialToken;
                                specialToken = (specialToken.Next = matchedToken);
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
                        curChar = InputStream.ReadChar();
                        continue;
                    }
                    catch (IOException e1) { }
                }
                int error_line = InputStream.EndLine;
                int error_column = InputStream.EndColumn;
                String error_after = null;
                bool EOFSeen = false;
                try { InputStream.ReadChar(); InputStream.Backup(1); }
                catch (IOException)
                {
                    EOFSeen = true;
                    error_after = curPos <= 1 ? "" : InputStream.GetImage();
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
                    InputStream.Backup(1);
                    error_after = curPos <= 1 ? "" : InputStream.GetImage();
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
            case 18:
                image.Append(InputStream.GetSuffix(ccimageLen));
                ccimageLen = 0;
                InputStream.Backup(1);
                break;
            default:
                break;
        }
    }
    void TokenLexicalActions(Token matchedToken)
    {
        switch (ccmatchedKind)
        {
            case 126:
                image.Append(ccstrLiteralImages[126]);
                lengthOfMatch = ccstrLiteralImages[126].Length;
                matchedToken.Kind = GT;
                ((Token.GTToken)matchedToken).RealKind = RUNSIGNEDSHIFT_VALUE;
                InputStream.Backup(2);
                matchedToken.Image = ">";
                break;
            case 127:
                image.Append(ccstrLiteralImages[127]);
                lengthOfMatch = ccstrLiteralImages[127].Length;
                matchedToken.Kind = GT;
                ((Token.GTToken)matchedToken).RealKind = RSIGNEDSHIFT_VALUE;
                InputStream.Backup(1);
                matchedToken.Image = ">";
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

    private void ccCheckNAddStates(int start, int end)
    {
        do
        {
            ccCheckNAdd(ccnextStates[start]);
        } while (start++ != end);
    }

}
