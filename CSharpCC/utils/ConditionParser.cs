using CSharpCC.CCTree;
using System.Text;

namespace CSharpCC.Utils;

public class ConditionParser : ConditionParserConstants
{
    private Dictionary<string, object> options;

    public static void main(params string[] args)
    {
        Test("F", false);
        Test("T", true);
        Test("F || T", true);
        Test("T || F", true);
        Test("T || will not be compiled )", true);
        Test("F && T", false);
        Test("T && T", true);
        Test("unknown", false);
    }

    private static void Test(String input, bool expectedValue)
    {
        var cp = new ConditionParser(new StringReader(input));
        Dictionary<String, Object> values = new()
        {
            { "F", false },
            { "T", true }
        };
        bool value = cp.CompilationUnit(values);
        Console.WriteLine(input + " = " + value);
        if (value != expectedValue)
            throw new RuntimeException();
    }

    public bool CompilationUnit(Dictionary<string, object> options)
    {
        bool value;
        this.options = options;
        value = ConditionalExpression();
        { if (true) return value; }
        throw new Error("Missing return statement in function");
    }

    public bool ConditionalExpression()
    {
        bool value;
        value = ConditionalAndExpression();
        if (value) { if (true) return true; }
    //label_1:
        while (true)
        {
            switch ((cc_ntk == -1) ? cc_ntk_() : cc_ntk)
            {
                case SC_OR:
                    ;
                    break;
                default:
                    cc_la1[0] = cc_gen;
                    goto exit_1;// label_1;
            }
            cc_consume_token(SC_OR);
            value = ConditionalAndExpression();
            if (value) { if (true) return true; }
        }
    exit_1:
        { if (true) return false; }
        throw new Error("Missing return statement in function");
    }

    public bool ConditionalAndExpression()
    {
        bool value;
        value = UnaryExpressionNotPlusMinus();
        if (!value) { if (true) return false; }
    //label_2:
        while (true)
        {
            switch ((cc_ntk == -1) ? cc_ntk_() : cc_ntk)
            {
                case SC_AND:
                    ;
                    break;
                default:
                    cc_la1[1] = cc_gen;
                    goto exit_2;// label_2;
            }
            cc_consume_token(SC_AND);
            value = UnaryExpressionNotPlusMinus();
            if (!value) { if (true) return false; }
        }
    exit_2:
        { if (true) return true; }
        throw new Error("Missing return statement in function");
    }

    public bool UnaryExpressionNotPlusMinus()
    {
        bool value;
        switch ((cc_ntk == -1) ? cc_ntk_() : cc_ntk)
        {
            case BANG:
            case 18:
                switch ((cc_ntk == -1) ? cc_ntk_() : cc_ntk)
                {
                    case 18:
                        cc_consume_token(18);
                        break;
                    case BANG:
                        cc_consume_token(BANG);
                        break;
                    default:
                        cc_la1[2] = cc_gen;
                        cc_consume_token(-1);
                        throw new ParseException();
                }
                value = UnaryExpressionNotPlusMinus();
                { if (true) return !value; }
                break;
            case LPAREN:
            case TRUE:
            case FALSE:
            case IDENTIFIER:
                value = UnaryExpression();
                { if (true) return value; }
                break;
            default:
                cc_la1[3] = cc_gen;
                cc_consume_token(-1);
                throw new ParseException();
        }
        throw new Error("Missing return statement in function");
    }

    public bool UnaryExpression()
    {
        bool value;
        switch ((cc_ntk == -1) ? cc_ntk_() : cc_ntk)
        {
            case TRUE:
            case FALSE:
                value = Literal();
                { if (true) return value; }
                break;
            case LPAREN:
                cc_consume_token(LPAREN);
                value = ConditionalExpression();
                cc_consume_token(RPAREN);
                { if (true) return value; }
                break;
            case IDENTIFIER:
                value = Name();
                { if (true) return value; }
                break;
            default:
                cc_la1[4] = cc_gen;
                cc_consume_token(-1);
                throw new ParseException();
        }
        throw new Error("Missing return statement in function");
    }

    public bool Literal()
    {
        bool value;
        value = BooleanLiteral();
        { if (true) return value; }
        throw new Error("Missing return statement in function");
    }

    public bool BooleanLiteral()
    {
        switch ((cc_ntk == -1) ? cc_ntk_() : cc_ntk)
        {
            case TRUE:
                cc_consume_token(TRUE);
                { if (true) return true; }
                break;
            case FALSE:
                cc_consume_token(FALSE);
                { if (true) return false; }
                break;
            default:
                cc_la1[5] = cc_gen;
                cc_consume_token(-1);
                throw new ParseException();
        }
        throw new Error("Missing return statement in function");
    }

    public bool Name()
    {
        String name;
        cc_consume_token(IDENTIFIER);
        name = GetToken(0).image.Trim();

        if (options.TryGetValue(name, out var obj) && obj is bool b)
        {
            { if (true) return b; }
        }
        else if (obj is String s2)
        {
            String s = s2.Trim();
            {
                if (true) return s.Length > 0 && !s.Equals("false", StringComparison.InvariantCultureIgnoreCase)
                        && !s.Equals("no", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        { if (true) return false; }
        throw new Error("Missing return statement in function");
    }

    /** Generated Token Manager. */
    public ConditionParserTokenManager token_source;
    readonly CSharpCharStream jj_input_stream;
    /** Current token. */
    public Token token;
    /** Next token. */
    public Token jj_nt;
    private int cc_ntk;
    private int cc_gen;
    private readonly int[] cc_la1 = new int[6];
    static private int[] jj_la1_0;
    static ConditionParser()
    {
        jj_la1_init_0();
    }
    private static void jj_la1_init_0()
    {
        jj_la1_0 = new int[] { 0x10000, 0x20000, 0x48000, 0x3ca000, 0x382000, 0x180000, };
    }

    /** Constructor with InputStream. */
    public ConditionParser(Stream stream)
        : this(stream, null)
    {
        ;
    }
    /** Constructor with InputStream and supplied encoding */
    public ConditionParser(Stream stream, Encoding encoding)
    {
        try { jj_input_stream = new CSharpCharStream(stream, encoding, 1, 1); } catch (Exception e) { throw new RuntimeException(e.Message, e); }
        token_source = new ConditionParserTokenManager(jj_input_stream);
        token = new Token();
        cc_ntk = -1;
        cc_gen = 0;
        for (int i = 0; i < 6; i++) cc_la1[i] = -1;
    }

    /** Reinitialise. */
    public void ReInit(Stream stream)
    {
        ReInit(stream, null);
    }
    /** Reinitialise. */
    public void ReInit(Stream stream, Encoding encoding)
    {
        try { jj_input_stream.ReInit(stream, encoding, 1, 1); } catch (Exception e) { throw new RuntimeException(e.Message, e); }
        token_source.ReInit(jj_input_stream);
        token = new Token();
        cc_ntk = -1;
        cc_gen = 0;
        for (int i = 0; i < 6; i++) cc_la1[i] = -1;
    }

    /** Constructor. */
    public ConditionParser(TextReader stream)
    {
        jj_input_stream = new CSharpCharStream(stream, 1, 1);
        token_source = new ConditionParserTokenManager(jj_input_stream);
        token = new Token();
        cc_ntk = -1;
        cc_gen = 0;
        for (int i = 0; i < 6; i++) cc_la1[i] = -1;
    }

    /** Reinitialise. */
    public void ReInit(TextReader stream)
    {
        jj_input_stream.ReInit(stream, 1, 1);
        token_source.ReInit(jj_input_stream);
        token = new Token();
        cc_ntk = -1;
        cc_gen = 0;
        for (int i = 0; i < 6; i++) cc_la1[i] = -1;
    }

    /** Constructor with generated Token Manager. */
    public ConditionParser(ConditionParserTokenManager tm)
    {
        token_source = tm;
        token = new Token();
        cc_ntk = -1;
        cc_gen = 0;
        for (int i = 0; i < 6; i++) cc_la1[i] = -1;
    }

    /** Reinitialise. */
    public void ReInit(ConditionParserTokenManager tm)
    {
        token_source = tm;
        token = new Token();
        cc_ntk = -1;
        cc_gen = 0;
        for (int i = 0; i < 6; i++) cc_la1[i] = -1;
    }

    private Token cc_consume_token(int kind)
    {
        Token oldToken;
        if ((oldToken = token).next != null) token = token.next;
        else token = token.next = token_source.GetNextToken();
        cc_ntk = -1;
        if (token.kind == kind)
        {
            cc_gen++;
            return token;
        }
        token = oldToken;
        cc_kind = kind;
        throw GenerateParseException();
    }


    /** Get the next Token. */
    public Token GetNextToken()
    {
        if (token.next != null) token = token.next;
        else token = token.next = token_source.GetNextToken();
        cc_ntk = -1;
        cc_gen++;
        return token;
    }

    /** Get the specific Token. */
    public Token GetToken(int index)
    {
        Token t = token;
        for (int i = 0; i < index; i++)
        {
            if (t.next != null) t = t.next;
            else t = t.next = token_source.GetNextToken();
        }
        return t;
    }

    private int cc_ntk_()
    {
        if ((jj_nt = token.next) == null)
            return (cc_ntk = (token.next = token_source.GetNextToken()).kind);
        else
            return (cc_ntk = jj_nt.kind);
    }

    private readonly List<int[]> cc_expentries = new();
    private int[] cc_expentry;
    private int cc_kind = -1;

    /** Generate ParseException. */
    public ParseException GenerateParseException()
    {
        cc_expentries.Clear();
        bool[] la1tokens = new bool[24];
        if (cc_kind >= 0)
        {
            la1tokens[cc_kind] = true;
            cc_kind = -1;
        }
        for (int i = 0; i < 6; i++)
        {
            if (cc_la1[i] == cc_gen)
            {
                for (int j = 0; j < 32; j++)
                {
                    if ((jj_la1_0[i] & (1 << j)) != 0)
                    {
                        la1tokens[j] = true;
                    }
                }
            }
        }
        for (int i = 0; i < 24; i++)
        {
            if (la1tokens[i])
            {
                cc_expentry = new int[1];
                cc_expentry[0] = i;
                cc_expentries.Add(cc_expentry);
            }
        }
        int[][] exptokseq = new int[cc_expentries.Count][];
        for (int i = 0; i < cc_expentries.Count; i++)
        {
            exptokseq[i] = cc_expentries[i];
        }
        return new ParseException(token, exptokseq, TokenImage);
    }

    public void EnableTracing()
    {
    }

    public void DisableTracing()
    {
    }
}
