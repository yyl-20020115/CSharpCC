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
 * This package contains data created as a result of parsing and semanticizing
 * a JavaCC input file.  This data is what is used by the back-ends of JavaCC as
 * well as any other back-end of JavaCC related tools such as JJTree.
 */
public class JavaCCGlobals : JavaCCParserConstants
{

    /**
     * string that identifies the JavaCC generated files.
     */
    public const string toolName = "JavaCC";

    /**
     * The name of the grammar file being processed.
     */
    static public string fileName;

    /**
     * The name of the original file (before processing by JJTree).
     * Currently this is the same as fileName.
     */
    static public string origFileName;

    /**
     * Set to true if this file has been processed by JJTree.
     */
    static public bool jjtreeGenerated;

    /**
     * The list of tools that have participated in generating the
     * input grammar file.
     */
    static public List<string> toolNames;

    /**
     * This prints the banner line when the various tools are invoked.  This
     * takes as argument the tool's full name and its version.
     */
    static public void bannerLine(string fullName, string ver)
    {
        Console.Write("Java Compiler Compiler Version " + Version.VersionNumber + " (" + fullName);
        if (ver != (""))
        {
            Console.Write(" Version " + ver);
        }
        Console.WriteLine(")");
    }

    /**
     * The name of the parser class (what appears in PARSER_BEGIN and PARSER_END).
     */
    static public string cu_name;

    /**
     * This is a list of tokens that appear after "PARSER_BEGIN(name)" all the
     * way until (but not including) the opening brace "{" of the class "name".
     */
    static public List<Token> cu_to_insertion_point_1 = new ();

    /**
     * This is the list of all tokens that appear after the tokens in
     * "cu_to_insertion_point_1" and until (but not including) the closing brace "}"
     * of the class "name".
     */
    static public List<Token> cu_to_insertion_point_2 = new ();

    /**
     * This is the list of all tokens that appear after the tokens in
     * "cu_to_insertion_point_2" and until "PARSER_END(name)".
     */
    static public List<Token> cu_from_insertion_point_2 = new ();

    /**
     * A list of all grammar productions - normal and JAVACODE - in the order
     * they appear in the input file.  Each entry here will be a subclass of
     * "NormalProduction".
     */
    static public List<NormalProduction> bnfproductions = new ();

    /**
     * A symbol table of all grammar productions - normal and JAVACODE.  The
     * symbol table is indexed by the name of the left hand side non-terminal.
     * Its contents are of type "NormalProduction".
     */
    static public Dictionary<string,NormalProduction> production_table = new ();

    /**
     * A mapping of lexical state strings to their integer internal representation.
     * Integers are stored as java.lang.int's.
     */
    static public Dictionary<String, int> lexstate_S2I = new ();

    /**
     * A mapping of the internal integer representations of lexical states to
     * their strings.  Integers are stored as java.lang.int's.
     */
    static public Dictionary<int, String> lexstate_I2S = new ();

    /**
     * The declarations to be inserted into the TokenManager class.
     */
    static public List token_mgr_decls;

    /**
     * The list of all TokenProductions from the input file.  This list includes
     * implicit TokenProductions that are created for uses of regular expressions
     * within BNF productions.
     */
    static public List<TokenProduction> rexprlist = new ();

    /**
     * The total number of distinct tokens.  This is therefore one more than the
     * largest assigned token ordinal.
     */
    static public int tokenCount;

    /**
     * This is a symbol table that contains all named tokens (those that are
     * defined with a label).  The index to the table is the image of the label
     * and the contents of the table are of type "RegularExpression".
     */
    static public Dictionary named_tokens_table = new ();

    /**
     * Contains the same entries as "named_tokens_table", but this is an ordered
     * list which is ordered by the order of appearance in the input file.
     */
    static public List<RegularExpression> ordered_named_tokens = new ();

    /**
     * A mapping of ordinal values (represented as objects of type "int") to
     * the corresponding labels (of type "String").  An entry exists for an ordinal
     * value only if there is a labeled token corresponding to this entry.
     * If there are multiple labels representing the same ordinal value, then
     * only one label is stored.
     */
    static public Dictionary<int, String> names_of_tokens = new ();

    /**
     * A mapping of ordinal values (represented as objects of type "int") to
     * the corresponding RegularExpression's.
     */
    static public Dictionary<int, RegularExpression> rexps_of_tokens = new Dictionary<int, RegularExpression>();

    /**
     * This is a three-level symbol table that contains all simple tokens (those
     * that are defined using a single string (with or without a label).  The index
     * to the first level table is a lexical state which maps to a second level
     * hashtable.  The index to the second level hashtable is the string of the
     * simple token converted to upper case, and this maps to a third level hashtable.
     * This third level hashtable contains the actual string of the simple token
     * and maps it to its RegularExpression.
     */
    static public Dictionary simple_tokens_table = new Dictionary();

    /**
     * maskindex, jj2index, maskVals are variables that are shared between
     * ParseEngine and ParseGen.
     */
    static protected int maskindex = 0;
    static protected int jj2index = 0;
    public static bool lookaheadNeeded;
    public static protected List maskVals = new ();

    public static Action actForEof;
    public static string nextStateForEof;
    public static Token otherLanguageDeclTokenBeg;
    public static Token otherLanguageDeclTokenEnd;


    // Some general purpose utilities follow.

    /**
     * Returns the identifying string for the file name, given a toolname
     * used to generate it.
     */
    public static string GetIdString(string toolName, string fileName)
    {
        List<string> toolNames = new ();
        toolNames.Add(toolName);
        return GetIdString(toolNames, fileName);
    }

    /**
     * Returns the identifying string for the file name, given a set of tool
     * names that are used to generate it.
     */
    public static string GetIdString(List<string> toolNames, string fileName)
    {
        int i;
        string toolNamePrefix = "Generated By:";

        for (i = 0; i < toolNames.Count - 1; i++)
            toolNamePrefix += (String)toolNames[i] + "&";
        toolNamePrefix += (String)toolNames[i] + ":";

        if (toolNamePrefix.Length > 200)
        {
            Console.WriteLine("Tool names too long.");
            throw new Error();
        }

        return toolNamePrefix + " Do not edit this line. " + addUnicodeEscapes(fileName);
    }

    /**
     * Returns true if tool name passed is one of the tool names returned
     * by getToolNames(fileName).
     */
    public static bool IsGeneratedBy(string toolName, string fileName)
    {
        List<string> v = GetToolNames(fileName);

        for (int i = 0; i < v.Count; i++)
            if (toolName == (v[i]))
                return true;

        return false;
    }

    private static List<string> MakeToolNameList(string str)
    {
        List<string> retVal = new ();

        int limit1 = str.IndexOf('\n');
        if (limit1 == -1) limit1 = 1000;
        int limit2 = str.IndexOf('\r');
        if (limit2 == -1) limit2 = 1000;
        int limit = (limit1 < limit2) ? limit1 : limit2;

        string tmp;
        if (limit == 1000)
        {
            tmp = str;
        }
        else
        {
            tmp = str[..limit];
        }

        if (tmp.IndexOf(':') == -1)
            return retVal;

        tmp = tmp[(tmp.IndexOf(':') + 1)..];

        if (tmp.IndexOf(':') == -1)
            return retVal;

        tmp = tmp[..tmp.IndexOf(':')];

        int i = 0, j = 0;

        while (j < tmp.Length && (i = tmp.IndexOf('&', j)) != -1)
        {
            retVal.Add(tmp[j..i]);
            j = i + 1;
        }

        if (j < tmp.Length)
            retVal.Add(tmp[j..]);

        return retVal;
    }

    /**
     * Returns a List of names of the tools that have been used to generate
     * the given file.
     */
    public static List<string> GetToolNames(string fileName)
    {
        char[] buf = new char[256];
        StreamReader stream = null;
        int read, total = 0;

        try
        {
            stream = new StreamReader(fileName);

            for (; ; )
                if ((read = stream.Read(buf, total, buf.Length - total)) != -1)
                {
                    if ((total += read) == buf.Length)
                        break;
                }
                else
                    break;

            return MakeToolNameList(new String(buf, 0, total));
        }
        catch (FileNotFoundException e1)
        {
        }
        catch (IOException e2)
        {
            if (total > 0)
                return MakeToolNameList(new String(buf, 0, total));
        }
        finally
        {
            if (stream != null)
                try { stream.Close(); }
                catch (Exception e3) { }
        }

        return new ();
    }

    public static void createOutputDir(string outputDir)
    {
        if (!Directory.Exists(outputDir))
        {
            JavaCCErrors.Warning("Output directory \"" + outputDir + "\" does not exist. Creating the directory.");

            if (!outputDir.mkdirs())
            {
                JavaCCErrors.SemanticError("Cannot create the output directory : " + outputDir);
                return;
            }
        }

        if (!outputDir.isDirectory())
        {
            JavaCCErrors.SemanticError("\"" + outputDir + " is not a valid output directory.");
            return;
        }

        if (!outputDir.canWrite())
        {
            JavaCCErrors.SemanticError("Cannot write to the output output directory : \"" + outputDir + "\"");
            return;
        }
    }

    static public string staticOpt()
    {
        if (Options.getStatic())
        {
            return "static ";
        }
        else
        {
            return "";
        }
    }

    static public string add_escapes(string str)
    {
        string retval = "";
        char ch;
        for (int i = 0; i < str.Length; i++)
        {
            ch = str[i];
            if (ch == '\b')
            {
                retval += "\\b";
            }
            else if (ch == '\t')
            {
                retval += "\\t";
            }
            else if (ch == '\n')
            {
                retval += "\\n";
            }
            else if (ch == '\f')
            {
                retval += "\\f";
            }
            else if (ch == '\r')
            {
                retval += "\\r";
            }
            else if (ch == '\"')
            {
                retval += "\\\"";
            }
            else if (ch == '\'')
            {
                retval += "\\\'";
            }
            else if (ch == '\\')
            {
                retval += "\\\\";
            }
            else if (ch < 0x20 || ch > 0x7e)
            {
                string s = "0000" + Convert.ToString(ch, 16);
                retval += "\\u" + s[^4..];
            }
            else
            {
                retval += ch;
            }
        }
        return retval;
    }

    static public string addUnicodeEscapes(string str)
    {

        if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
        {
            return str;
        }
        else if (Options.isOutputLanguageJava())
        {
            string retval = "";
            char ch;
            for (int i = 0; i < str.Length; i++)
            {
                ch = str[i];
                if (ch < 0x20 || ch > 0x7e /*|| ch == '\\' -- cba commented out 20140305*/ )
                {
                    string s = "0000" + Convert.ToString(ch, 16);
                    retval += "\\u" + s[^4..];
                }
                else
                {
                    retval += ch;
                }
            }
            return retval;
        }
        else
        {
            // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
            throw new Exception("Unhandled Output Language : " + Options.getOutputLanguage());
        }
    }

    static protected int cline, ccol;

    static protected void PrintTokenSetup(Token t)
    {
        Token tt = t;
        while (tt.specialToken != null) tt = tt.specialToken;
        cline = tt.beginLine;
        ccol = tt.beginColumn;
    }

    static protected void PrintTokenOnly(Token t, TextWriter ostr)
    {
        for (; cline < t.beginLine; cline++)
        {
            ostr.WriteLine(""); ccol = 1;
        }
        for (; ccol < t.beginColumn; ccol++)
        {
            ostr.Write(" ");
        }
        if (t.kind == JavaCCParserConstants.STRING_LITERAL ||
            t.kind == JavaCCParserConstants.CHARACTER_LITERAL)
            ostr.Write(addUnicodeEscapes(t.image));
        else
            ostr.Write(t.image);
        cline = t.endLine;
        ccol = t.endColumn + 1;
        char last = t.image[^1];
        if (last == '\n' || last == '\r')
        {
            cline++; ccol = 1;
        }
    }

    static protected void PrintToken(Token t, TextWriter ostr)
    {
        Token tt = t.specialToken;
        if (tt != null)
        {
            while (tt.specialToken != null) tt = tt.specialToken;
            while (tt != null)
            {
                PrintTokenOnly(tt, ostr);
                tt = tt.next;
            }
        }
        PrintTokenOnly(t, ostr);
    }

    static protected void PrintTokenList(List<Token> list, TextWriter ostr)
    {
        Token? t = null;
        foreach (var t2 in list)
        {
            PrintToken(t = t2, ostr);
        }
        if (t != null)
            PrintTrailingComments(t, ostr);
    }

    static protected void PrintLeadingComments(Token t, TextWriter ostr)
    {
        if (t.specialToken == null) return;
        Token tt = t.specialToken;
        while (tt.specialToken != null) tt = tt.specialToken;
        while (tt != null)
        {
            PrintTokenOnly(tt, ostr);
            tt = tt.next;
        }
        if (ccol != 1 && cline != t.beginLine)
        {
            ostr.WriteLine();
            cline++; ccol = 1;
        }
    }

    static protected void PrintTrailingComments(Token t, TextWriter ostr)
    {
        if (t.next == null) return;
        PrintLeadingComments(t.next);
    }

    static protected string PrintTokenOnly(Token t)
    {
        string retval = "";
        for (; cline < t.beginLine; cline++)
        {
            retval += "\n"; ccol = 1;
        }
        for (; ccol < t.beginColumn; ccol++)
        {
            retval += " ";
        }
        if (t.kind == JavaCCParserConstants.STRING_LITERAL ||
            t.kind == JavaCCParserConstants.CHARACTER_LITERAL)
            retval += addUnicodeEscapes(t.image);
        else
            retval += t.image;
        cline = t.endLine;
        ccol = t.endColumn + 1;
        char last = t.image[^1];
        if (last == '\n' || last == '\r')
        {
            cline++; ccol = 1;
        }
        return retval;
    }

    static protected string PrintToken(Token t)
    {
        string retval = "";
        Token tt = t.specialToken;
        if (tt != null)
        {
            while (tt.specialToken != null) tt = tt.specialToken;
            while (tt != null)
            {
                retval += PrintTokenOnly(tt);
                tt = tt.next;
            }
        }
        retval += PrintTokenOnly(t);
        return retval;
    }

    static protected string PrintLeadingComments(Token t)
    {
        string retval = "";
        if (t.specialToken == null) return retval;
        Token tt = t.specialToken;
        while (tt.specialToken != null) tt = tt.specialToken;
        while (tt != null)
        {
            retval += PrintTokenOnly(tt);
            tt = tt.next;
        }
        if (ccol != 1 && cline != t.beginLine)
        {
            retval += "\n";
            cline++; ccol = 1;
        }
        return retval;
    }

    static protected string PrintTrailingComments(Token t)
    {
        if (t.next == null) return "";
        return PrintLeadingComments(t.next);
    }

    public static void ReInit()
    {
        fileName = null;
        origFileName = null;
        jjtreeGenerated = false;
        toolNames = null;
        cu_name = null;
        cu_to_insertion_point_1 = new ();
        cu_to_insertion_point_2 = new ();
        cu_from_insertion_point_2 = new ();
        bnfproductions = new ();
        production_table = new ();
        lexstate_S2I = new ();
        lexstate_I2S = new ();
        token_mgr_decls = null;
        rexprlist = new ();
        tokenCount = 0;
        named_tokens_table = new ();
        ordered_named_tokens = new ();
        names_of_tokens = new ();
        rexps_of_tokens = new ();
        simple_tokens_table = new ();
        maskindex = 0;
        jj2index = 0;
        maskVals = new ();
        cline = 0;
        ccol = 0;
        actForEof = null;
        nextStateForEof = null;
    }


    static string GetFileExtension(string language)
    {
        string lang = Options.getOutputLanguage();
        // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
        if (Options.isOutputLanguageJava())
        {
            return ".java";
        }
        else if (lang.ToLower() == (Options.OUTPUT_LANGUAGE__CPP))
        {
            return ".cc";
        }

        //assert(false);
        return null;
    }

}
