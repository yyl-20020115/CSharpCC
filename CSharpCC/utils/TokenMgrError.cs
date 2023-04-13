namespace CSharpCC.Utils;

/** Token Manager Error. */
public class TokenMgrError : Exception
{
    /*
     * Ordinals for various reasons why an Error of this type can be thrown.
     */

    /**
     * Lexical error occurred.
     */
    public const int LEXICAL_ERROR = 0;

    /**
     * An attempt was made to create a second instance of a static token manager.
     */
    public const int STATIC_LEXER_ERROR = 1;

    /**
     * Tried to change to an invalid lexical state.
     */
    public const int INVALID_LEXICAL_STATE = 2;

    /**
     * Detected (and bailed out of) an infinite loop in the token manager.
     */
    public const int LOOP_DETECTED = 3;

    /**
     * Indicates the reason why the exception is thrown. It will have
     * one of the above 4 values.
     */
    public int errorCode;

    /**
     * Returns a detailed message for the Error when it is thrown by the
     * token manager to indicate a lexical error.
     * Parameters :
     *    EOFSeen     : indicates if EOF caused the lexical error
     *    curLexState : lexical state in which this error occurred
     *    errorLine   : line number when the error occurred
     *    errorColumn : column number when the error occurred
     *    errorAfter  : prefix that was seen before this error occurred
     *    curchar     : the offending character
     * Note: You can customize the lexical error message by modifying this method.
     */
    protected static string LexicalError(bool EOFSeen, int lexState, int errorLine, int errorColumn, string errorAfter, char curChar) 
        => $"Lexical error at line {errorLine}, column {errorColumn}.  Encountered: {(EOFSeen ? "<EOF> " : "\"" + StringEscapeHelpers.AddEscapes(curChar.ToString()) + "\"" + " (" + (int)curChar + "), ")}after : \"{StringEscapeHelpers.AddEscapes(errorAfter)}\"";

    /**
     * You can also modify the body of this method to customize your error messages.
     * For example, cases like LOOP_DETECTED and INVALID_LEXICAL_STATE are not
     * of end-users concern, so you can return something like :
     *
     *     "Internal Error : Please file a bug report .... "
     *
     * from this method for such cases in the release version of your parser.
     */
    /*
     * Constructors of various flavors follow.
     */

    /** No arg constructor. */
    public TokenMgrError() { }

    /** Constructor with message and reason. */
    public TokenMgrError(string message, int reason)
        : base(message) => errorCode = reason;

    /** Full Constructor. */
    public TokenMgrError(bool EOFSeen, int lexState, int errorLine, int errorColumn, string errorAfter, char curChar, int reason)
        : this(LexicalError(EOFSeen, lexState, errorLine, errorColumn, errorAfter, curChar), reason) { }
}
