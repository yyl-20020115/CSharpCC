/**
 * This file contains the code for JavaCCParser generated
 * by JavaCCParser itself.
 */

namespace CSharpCC.Parser;

/**
 * Describes the input token stream.
 */

public class Token
{

    /**
     * An integer that describes the kind of this token.  This numbering
     * system is determined by JavaCCParser, and a table of these numbers is
     * stored in the file ...Constants.java.
     */
    public int kind;

    /** The line number of the first character of this Token. */
    public int beginLine;
    /** The column number of the first character of this Token. */
    public int beginColumn;
    /** The line number of the last character of this Token. */
    public int endLine;
    /** The column number of the last character of this Token. */
    public int endColumn;

    /**
     * The string image of the token.
     */
    public string image;

    /**
     * A reference to the next regular (non-special) token from the input
     * stream.  If this is the last token from the input stream, or if the
     * token manager has not read tokens beyond this one, this field is
     * set to null.  This is true only if this token is also a regular
     * token.  Otherwise, see below for a description of the contents of
     * this field.
     */
    public Token next;

    /**
     * This field is used to access special tokens that occur prior to this
     * token, but after the immediately preceding regular (non-special) token.
     * If there are no such special tokens, this field is set to null.
     * When there are more than one such special token, this field refers
     * to the last of these special tokens, which in turn refers to the next
     * previous special token through its specialToken field, and so on
     * until the first special token (whose specialToken field is null).
     * The next fields of special tokens refer to other special tokens that
     * immediately follow it (without an intervening regular token).  If there
     * is no such token, this field is null.
     */
    public Token specialToken;

    /**
     * An optional attribute value of the Token.
     * Tokens which are not used as syntactic sugar will often contain
     * meaningful values that will be used later on by the compiler or
     * interpreter. This attribute value is often different from the image.
     * Any subclass of Token that actually wants to return a non-null value can
     * override this method as appropriate.
     */
    public object GetValue() => null;

    /**
     * No-argument constructor
     */
    public Token() { }

    //  /**
    //   * Constructs a new token for the specified Image.
    //   */
    //  public Token(int kind)
    //  {
    //    this(kind, null);
    //  }
    //
    /**
     * Constructs a new token for the specified Image and Kind.
     */
    public Token(int kind, string image)
    {
        this.kind = kind;
        this.image = image;
    }

    /**
     * equals
     */
    public override bool Equals(object? o) 
        => o != null && (this == o || (o is string s && s == image));

    /**
     * hashCode
     */
    public override int GetHashCode() => image.GetHashCode();

    /**
     * Returns the image.
     */
    public override string ToString() => image;

    /**
     * Returns a new Token object, by default. However, if you want, you
     * can create and return subclass objects based on the value of ofKind.
     * Simply add the cases to the switch for all those special cases.
     * For example, if you have a subclass of Token called IDToken that
     * you want to create if ofKind is ID, simply add something like :
     *
     *    case MyParserConstants.ID : return new IDToken(ofKind, image);
     *
     * to the following switch statement. Then you can cast matchedToken
     * variable to the appropriate type and use it in your lexical actions.
     */
    public static Token NewToken(int ofKind, string? image) => ofKind switch
    {
        CSharpCCParserConstants.RUNSIGNEDSHIFT_VALUE or CSharpCCParserConstants.RSIGNEDSHIFT_VALUE or CSharpCCParserConstants.GT => new GTToken(ofKind, image),
        _ => new Token(ofKind, image),
    };

    public static Token NewToken(int ofKind) => NewToken(ofKind, null);

    /**
     * Greater than Token.
     */
    public class GTToken : Token
    {
        public GTToken(int kind, string image)
                : base(kind, image) { }

        public int realKind = CSharpCCParserConstants.GT;
    }
}
