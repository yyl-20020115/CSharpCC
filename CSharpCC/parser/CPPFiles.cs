// Copyright 2011 Google Inc. All Rights Reserved.
// Author: sreeni@google.com (Sreeni Viswanadha)


using org.javacc.jjtree;
using org.javacc.utils;
using System.Text;
using System.Text.RegularExpressions;

namespace org.javacc.parser;

/**
 * Generate CharStream, TokenManager and Exceptions.
 */
public partial class CPPFiles : JavaCCGlobals
{
    /**
    * ID of the latest version (of JavaCC) in which one of the CharStream classes
    * or the CharStream interface is modified.
    */
    static readonly string CharStreamVersion = Version.MajorDotMinor;

    /**
     * ID of the latest version (of JavaCC) in which the TokenManager interface is modified.
     */
    static readonly string TokenManagerVersion = Version.MajorDotMinor;

    /**
     * ID of the latest version (of JavaCC) in which the Token class is modified.
     */
    static readonly string TokenVersion = Version.MajorDotMinor;

    /**
     * ID of the latest version (of JavaCC) in which the ParseException class is
     * modified.
     */
    static readonly string ParseExceptionVersion = Version.MajorDotMinor;

    /**
     * ID of the latest version (of JavaCC) in which the TokenMgrError class is
     * modified.
     */
    static readonly string TokenMgrErrorVersion = Version.MajorDotMinor;

    /**
     * Replaces all backslahes with double backslashes.
     */
    static string ReplaceBackslash(string str)
    {
        StringBuilder b;
        int i = 0, len = str.Length;

        while (i < len && str[i++] != '\\') ;

        if (i == len)  // No backslash found.
            return str;

        char c;
        b = new StringBuilder();
        for (i = 0; i < len; i++)
            if ((c = str[i]) == '\\')
                b.Append("\\\\");
            else
                b.Append(c);

        return b.ToString();
    }

    /**
     * Read the version from the comment in the specified file.
     * This method does not try to recover from invalid comment syntax, but
     * rather returns version 0.0 (which will always be taken to mean the file
     * is out of date).
     * @param fileName eg Token.java
     * @return The version as a double, eg 4.1
     * @since 4.1
     */
    static Regex versionRegex = MyRegex();
    static double GetVersion(string fileName)
    {
        string commentHeader = "/* " + GetIdString(toolName, fileName) + " Version ";
        string file = Path.Combine(Options.getOutputDirectory(), ReplaceBackslash(fileName));

        if (!File.Exists(file))
        {
            // Has not yet been created, so it must be up to date.
            string majorVersion = versionRegex.Replace(Version.VersionNumber,"");
            return double.TryParse(majorVersion, out var d) ? d : 0.0;
        }

        TextReader reader = null;
        try
        {
            reader = new StreamReader(file);
            string str;
            double version = 0.0;

            // Although the version comment should be the first line, sometimes the
            // user might have put comments before it.
            while ((str = reader.ReadLine()) != null)
            {
                if (str.StartsWith(commentHeader))
                {
                    str = str[commentHeader.Length..];
                    int pos = str.IndexOf(' ');
                    if (pos >= 0) str = str[..pos];
                    if (str.Length > 0&& double.TryParse(str, out version))
                    {
                        //OK    
                    }

                    break;
                }
            }

            return version;
        }
        catch (IOException ioe)
        {
            return 0.0;
        }
        finally
        {
            if (reader != null)
            {
                try { reader.Close(); } catch (IOException e) { }
            }
        }
    }

    private static void GenFile(string name, string version, string[] parameters)
    {
        string file = Path.Combine(Options.getOutputDirectory(), name);
        try
        {
            var outputFile = new OutputFile(file, version, parameters);

            if (!outputFile.NeedToWrite)
            {
                return;
            }

            TextWriter ostr = outputFile.getPrintWriter();
            OutputFileGenerator generator = new OutputFileGenerator(
                "/templates/cpp/" + name + ".template", Options.getOptions());
            generator.Generate(ostr);
            ostr.Close();
        }
        catch (IOException e)
        {
            Console.Error.WriteLine("Failed to create file: " + file + e);
            JavaCCErrors.SemanticError("Could not open file: " + file + " for writing.");
            throw new Error();
        }
    }

    public static void GenCharStream()
    {
        string[] parameters = new string[] { Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
        GenFile("CharStream.h", CharStreamVersion, parameters);
        GenFile("CharStream.cc", CharStreamVersion, parameters);
    }

    public static void GenParseException()
    {
        string[] parameters = new string[] { Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
        GenFile("ParseException.h", ParseExceptionVersion, parameters);
        GenFile("ParseException.cc", ParseExceptionVersion, parameters);
    }

    public static void GenTokenMgrError()
    {
        string[] parameters = new string[] { Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
        GenFile("TokenMgrError.h", TokenMgrErrorVersion, parameters);
        GenFile("TokenMgrError.cc", TokenMgrErrorVersion, parameters);
    }

    public static void GenToken()
    {
        string[] parameters = new string[] { Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC, Options.USEROPTION__CPP_TOKEN_INCLUDE, Options.USEROPTION__TOKEN_SUPER_CLASS };
        GenFile("Token.h", TokenMgrErrorVersion, parameters);
        GenFile("Token.cc", TokenMgrErrorVersion, parameters);
    }

    public static void GenTokenManager()
    {
        string[] parameters = new string[] { Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
        GenFile("TokenManager.h", TokenManagerVersion, parameters);
    }

    public static void GenJavaCCDefs()
    {
        string[] parameters = new string[] { Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
        GenFile("JavaCC.h", TokenManagerVersion, parameters);
    }

    public static void GenErrorHandler()
    {
        string[] parameters = new string[] { Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC, Options.USEROPTION__BUILD_PARSER, Options.USEROPTION__BUILD_TOKEN_MANAGER };
        GenFile("ErrorHandler.h", ParseExceptionVersion, parameters);
    }

    public static new void ReInit()
    {
    }

    [GeneratedRegex("[^0-9.]+.*")]
    private static partial Regex MyRegex();
}
