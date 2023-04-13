using System.Text;

namespace CSharpCC.Utils;

/**
 * This exception is thrown when parse errors are encountered.
 * You can explicitly create objects of this exception type by
 * calling the method generateParseException in the generated
 * parser.
 *
 * You can modify this class to customize your error reporting
 * mechanisms so long as you retain the public fields.
 */
public class ParseException : Exception
{
    /**
     * This constructor is used by the method "generateParseException"
     * in the generated parser.  Calling this constructor generates
     * a new object of this type with the fields "currentToken",
     * "expectedTokenSequences", and "tokenImage" set.
     */
    public ParseException(Token currentTokenVal,
                          int[][] expectedTokenSequencesVal,
                          String[] tokenImageVal
                         )
          : base(Initialise(currentTokenVal, expectedTokenSequencesVal, tokenImageVal))
    {
        currentToken = currentTokenVal;
        expectedTokenSequences = expectedTokenSequencesVal;
        tokenImage = tokenImageVal;
    }

    /**
     * The following constructors are for use by you for whatever
     * purpose you can think of.  Constructing the exception in this
     * manner makes the exception behave in the normal way - i.e., as
     * documented in the class "Throwable".  The fields "errorToken",
     * "expectedTokenSequences", and "tokenImage" do not contain
     * relevant information.  The JavaCC generated code does not use
     * these constructors.
     */

    public ParseException()
        :base()
    {
    }

    /** Constructor with message. */
    public ParseException(String message)
        : base(message)
    {
    }


    /**
     * This is the last token that has been consumed successfully.  If
     * this object has been created due to a parse error, the token
     * followng this token will (therefore) be the first error token.
     */
    public Token currentToken;

    /**
     * Each entry in this array is an array of integers.  Each array
     * of integers represents a sequence of tokens (by their ordinal
     * values) that is expected at this point of the parse.
     */
    public int[][] expectedTokenSequences;

    /**
     * This is a reference to the "tokenImage" array of the generated
     * parser within which the parse error occurred.  This array is
     * defined in the generated ...Constants interface.
     */
    public String[] tokenImage;

    /**
     * It uses "currentToken" and "expectedTokenSequences" to generate a parse
     * error message and returns it.  If this object has been created
     * due to a parse error, and you do not catch it (it gets thrown
     * from the parser) the correct error message
     * gets displayed.
     */
    private static string Initialise(Token currentToken,
                             int[][] expectedTokenSequences,
                             string[] tokenImage)
    {
        var eol = Environment.NewLine;// System.getProperty("line.separator", "\n");
        var expected = new StringBuilder();
        int maxSize = 0;
        for (int i = 0; i < expectedTokenSequences.Length; i++)
        {
            if (maxSize < expectedTokenSequences[i].Length)
            {
                maxSize = expectedTokenSequences[i].Length;
            }
            for (int j = 0; j < expectedTokenSequences[i].Length; j++)
            {
                expected.Append(tokenImage[expectedTokenSequences[i][j]]).Append(' ');
            }
            if (expectedTokenSequences[i][^1] != 0)
            {
                expected.Append("...");
            }
            expected.Append(eol).Append("    ");
        }
        var retval = "Encountered \"";
        var tok = currentToken.next;
        for (int i = 0; i < maxSize; i++)
        {
            if (i != 0) retval += " ";
            if (tok.kind == 0)
            {
                retval += tokenImage[0];
                break;
            }
            retval += " " + tokenImage[tok.kind];
            retval += " \"";
            retval += AddEscapes(tok.image);
            retval += " \"";
            tok = tok.next;
        }
        retval += "\" at line " + currentToken.next.beginLine + ", column " + currentToken.next.beginColumn;
        retval += "." + eol;
        if (expectedTokenSequences.Length == 1)
        {
            retval += "Was expecting:" + eol + "    ";
        }
        else
        {
            retval += "Was expecting one of:" + eol + "    ";
        }
        retval += expected.ToString();
        return retval;
    }

    /**
     * The end of line string for this machine.
     */
    protected string eol = Environment.NewLine;// System.getProperty("line.separator", "\n");

    /**
     * Used to convert raw characters to their escaped version
     * when these raw version cannot be used as part of an ASCII
     * string literal.
     */
    private static string AddEscapes(string str)
    {
        var retval = new StringBuilder();
        char ch;
        for (int i = 0; i < str.Length; i++)
        {
            switch (str[i])
            {
                case '\0':
                    continue;
                case '\b':
                    retval.Append("\\b");
                    continue;
                case '\t':
                    retval.Append("\\t");
                    continue;
                case '\n':
                    retval.Append("\\n");
                    continue;
                case '\f':
                    retval.Append("\\f");
                    continue;
                case '\r':
                    retval.Append("\\r");
                    continue;
                case '\"':
                    retval.Append("\\\"");
                    continue;
                case '\'':
                    retval.Append("\\\'");
                    continue;
                case '\\':
                    retval.Append("\\\\");
                    continue;
                default:
                    if ((ch = str[i]) < 0x20 || ch > 0x7e)
                    {
                        var s = "0000" + Convert.ToString(ch, 16);
                        retval.Append("\\u" + s[^4..]);
                    }
                    else
                    {
                        retval.Append(ch);
                    }
                    continue;
            }
        }
        return retval.ToString();
    }

}
