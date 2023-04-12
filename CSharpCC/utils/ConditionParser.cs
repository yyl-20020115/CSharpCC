/* Generated By:JavaCC: Do not edit this line. ConditionParser.java */
using org.javacc.jjtree;

namespace org.javacc.utils;

public class ConditionParser : ConditionParserConstants
{
    private Dictionary<String, Object> options;

    public static void main(params string[] args)
    {
        test("F", false);
        test("T", true);
        test("F || T", true);
        test("T || F", true);
        test("T || will not be compiled )", true);
        test("F && T", false);
        test("T && T", true);
        test("unknown", false);
    }

    private static void test(String input, bool expectedValue)
    {
        ConditionParser cp = new ConditionParser(new StringReader(input));
        Dictionary<String, Object> values = new Dictionary<String, Object>();
        values.Add("F", false);
        values.Add("T", true);
        bool value = cp.CompilationUnit(values);
        Console.WriteLine(input + " = " + value);
        if (value != expectedValue)
            throw new RuntimeException();
    }

    public bool CompilationUnit(Dictionary<String, Object> options)
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
    label_1:
        while (true)
        {
            switch ((jj_ntk == -1) ? jj_ntk() : jj_ntk)
            {
                case SC_OR:
                    ;
                    break;
                default:
                    jj_la1[0] = jj_gen;
                    break label_1;
            }
            jj_consume_token(SC_OR);
            value = ConditionalAndExpression();
            if (value) { if (true) return true; }
        }
        { if (true) return false; }
        throw new Error("Missing return statement in function");
    }

    public bool ConditionalAndExpression()
    {
        bool value;
        value = UnaryExpressionNotPlusMinus();
        if (!value) { if (true) return false; }
    label_2:
        while (true)
        {
            switch ((jj_ntk == -1) ? jj_ntk() : jj_ntk)
            {
                case SC_AND:
                    ;
                    break;
                default:
                    jj_la1[1] = jj_gen;
                    break label_2;
            }
            jj_consume_token(SC_AND);
            value = UnaryExpressionNotPlusMinus();
            if (!value) { if (true) return false; }
        }
        { if (true) return true; }
        throw new Error("Missing return statement in function");
    }

    public bool UnaryExpressionNotPlusMinus()
    {
        bool value;
        switch ((jj_ntk == -1) ? jj_ntk() : jj_ntk)
        {
            case BANG:
            case 18:
                switch ((jj_ntk == -1) ? jj_ntk() : jj_ntk)
                {
                    case 18:
                        jj_consume_token(18);
                        break;
                    case BANG:
                        jj_consume_token(BANG);
                        break;
                    default:
                        jj_la1[2] = jj_gen;
                        jj_consume_token(-1);
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
                jj_la1[3] = jj_gen;
                jj_consume_token(-1);
                throw new ParseException();
        }
        throw new Error("Missing return statement in function");
    }

    public bool UnaryExpression()
    {
        bool value;
        switch ((jj_ntk == -1) ? jj_ntk() : jj_ntk)
        {
            case TRUE:
            case FALSE:
                value = Literal();
                { if (true) return value; }
                break;
            case LPAREN:
                jj_consume_token(LPAREN);
                value = ConditionalExpression();
                jj_consume_token(RPAREN);
                { if (true) return value; }
                break;
            case IDENTIFIER:
                value = Name();
                { if (true) return value; }
                break;
            default:
                jj_la1[4] = jj_gen;
                jj_consume_token(-1);
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
        switch ((jj_ntk == -1) ? jj_ntk() : jj_ntk)
        {
            case TRUE:
                jj_consume_token(TRUE);
                { if (true) return true; }
                break;
            case FALSE:
                jj_consume_token(FALSE);
                { if (true) return false; }
                break;
            default:
                jj_la1[5] = jj_gen;
                jj_consume_token(-1);
                throw new ParseException();
        }
        throw new Error("Missing return statement in function");
    }

    public bool Name()
    {
        String name;
        jj_consume_token(IDENTIFIER);
        name = getToken(0).image.trim();

        Object obj = options.get(name);

        if (obj is Boolean)
        {
            { if (true) return ((Boolean)obj).booleanValue(); }
        }
        else if (obj is String)
        {
            String s = ((String)obj).trim();
            { if (true) return s.Length > 0 && !s.equalsIgnoreCase("false") && !s.equalsIgnoreCase("no"); }
        }

        { if (true) return false; }
        throw new Error("Missing return statement in function");
    }

    /** Generated Token Manager. */
    public ConditionParserTokenManager token_source;
    JavaCharStream jj_input_stream;
    /** Current token. */
    public Token token;
    /** Next token. */
    public Token jj_nt;
    private int jj_ntk;
    private int jj_gen;
    private int[] jj_la1 = new int[6];
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
    public ConditionParser(java.io.InputStream stream)
    {
        this(stream, null);
    }
    /** Constructor with InputStream and supplied encoding */
    public ConditionParser(java.io.InputStream stream, String encoding)
    {
        try { jj_input_stream = new JavaCharStream(stream, encoding, 1, 1); } catch (java.io.UnsupportedEncodingException e) { throw new RuntimeException(e); }
        token_source = new ConditionParserTokenManager(jj_input_stream);
        token = new Token();
        jj_ntk = -1;
        jj_gen = 0;
        for (int i = 0; i < 6; i++) jj_la1[i] = -1;
    }

    /** Reinitialise. */
    public void ReInit(java.io.InputStream stream)
    {
        ReInit(stream, null);
    }
    /** Reinitialise. */
    public void ReInit(java.io.InputStream stream, String encoding)
    {
        try { jj_input_stream.ReInit(stream, encoding, 1, 1); } catch (java.io.UnsupportedEncodingException e) { throw new RuntimeException(e); }
        token_source.ReInit(jj_input_stream);
        token = new Token();
        jj_ntk = -1;
        jj_gen = 0;
        for (int i = 0; i < 6; i++) jj_la1[i] = -1;
    }

    /** Constructor. */
    public ConditionParser(TextReader stream)
    {
        jj_input_stream = new JavaCharStream(stream, 1, 1);
        token_source = new ConditionParserTokenManager(jj_input_stream);
        token = new Token();
        jj_ntk = -1;
        jj_gen = 0;
        for (int i = 0; i < 6; i++) jj_la1[i] = -1;
    }

    /** Reinitialise. */
    public void ReInit(TextReader stream)
    {
        jj_input_stream.ReInit(stream, 1, 1);
        token_source.ReInit(jj_input_stream);
        token = new Token();
        jj_ntk = -1;
        jj_gen = 0;
        for (int i = 0; i < 6; i++) jj_la1[i] = -1;
    }

    /** Constructor with generated Token Manager. */
    public ConditionParser(ConditionParserTokenManager tm)
    {
        token_source = tm;
        token = new Token();
        jj_ntk = -1;
        jj_gen = 0;
        for (int i = 0; i < 6; i++) jj_la1[i] = -1;
    }

    /** Reinitialise. */
    public void ReInit(ConditionParserTokenManager tm)
    {
        token_source = tm;
        token = new Token();
        jj_ntk = -1;
        jj_gen = 0;
        for (int i = 0; i < 6; i++) jj_la1[i] = -1;
    }

    private Token jj_consume_token(int kind)
    {
        Token oldToken;
        if ((oldToken = token).next != null) token = token.next;
        else token = token.next = token_source.getNextToken();
        jj_ntk = -1;
        if (token.kind == kind)
        {
            jj_gen++;
            return token;
        }
        token = oldToken;
        jj_kind = kind;
        throw generateParseException();
    }


    /** Get the next Token. */
    public Token getNextToken()
    {
        if (token.next != null) token = token.next;
        else token = token.next = token_source.getNextToken();
        jj_ntk = -1;
        jj_gen++;
        return token;
    }

    /** Get the specific Token. */
    public Token getToken(int index)
    {
        Token t = token;
        for (int i = 0; i < index; i++)
        {
            if (t.next != null) t = t.next;
            else t = t.next = token_source.getNextToken();
        }
        return t;
    }

    private int jj_ntk()
    {
        if ((jj_nt = token.next) == null)
            return (jj_ntk = (token.next = token_source.getNextToken()).kind);
        else
            return (jj_ntk = jj_nt.kind);
    }

    private List<int[]> jj_expentries = new();
    private int[] jj_expentry;
    private int jj_kind = -1;

    /** Generate ParseException. */
    public ParseException generateParseException()
    {
        jj_expentries.Clear();
        bool[] la1tokens = new bool[24];
        if (jj_kind >= 0)
        {
            la1tokens[jj_kind] = true;
            jj_kind = -1;
        }
        for (int i = 0; i < 6; i++)
        {
            if (jj_la1[i] == jj_gen)
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
                jj_expentry = new int[1];
                jj_expentry[0] = i;
                jj_expentries.add(jj_expentry);
            }
        }
        int[][] exptokseq = new int[jj_expentries.Count][];
        for (int i = 0; i < jj_expentries.Count; i++)
        {
            exptokseq[i] = jj_expentries.get(i);
        }
        return new ParseException(token, exptokseq, tokenImage);
    }

    /** Enable tracing. */
    public void enable_tracing()
    {
    }

    /** Disable tracing. */
    public void disable_tracing()
    {
    }

}
