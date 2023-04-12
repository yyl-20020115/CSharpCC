// Copyright 2011 Google Inc. All Rights Reserved.
// Author: sreeni@google.com (Sreeni Viswanadha)


using org.javacc.jjtree;
using org.javacc.utils;
using System.Text;

namespace org.javacc.parser;



/**
 * Generate CharStream, TokenManager and Exceptions.
 */
public class CPPFiles : JavaCCGlobals
{
    /**
    * ID of the latest version (of JavaCC) in which one of the CharStream classes
    * or the CharStream interface is modified.
    */
    static readonly string charStreamVersion = Version.MajorDotMinor;

    /**
     * ID of the latest version (of JavaCC) in which the TokenManager interface is modified.
     */
    static readonly string tokenManagerVersion = Version.MajorDotMinor;

    /**
     * ID of the latest version (of JavaCC) in which the Token class is modified.
     */
    static readonly string tokenVersion = Version.MajorDotMinor;

    /**
     * ID of the latest version (of JavaCC) in which the ParseException class is
     * modified.
     */
    static readonly string parseExceptionVersion = Version.MajorDotMinor;

    /**
     * ID of the latest version (of JavaCC) in which the TokenMgrError class is
     * modified.
     */
    static readonly string tokenMgrErrorVersion = Version.MajorDotMinor;

    /**
     * Replaces all backslahes with double backslashes.
     */
    static string replaceBackslash(string str)
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
    static double getVersion(string fileName)
    {
        string commentHeader = "/* " + getIdString(toolName, fileName) + " Version ";
        string file = System.IO.Path.Combine(Options.getOutputDirectory(), replaceBackslash(fileName));

        if (!file.exists())
        {
            // Has not yet been created, so it must be up to date.
            try
            {
                string majorVersion = Version.VersionNumber.replaceAll("[^0-9.]+.*", "");
                return Double.parseDouble(majorVersion);
            }
            catch (NumberFormatException e)
            {
                return 0.0; // Should never happen
            }
        }

        BufferedReader reader = null;
        try
        {
            reader = new BufferedReader(new FileReader(file));
            string str;
            double version = 0.0;

            // Although the version comment should be the first line, sometimes the
            // user might have put comments before it.
            while ((str = reader.readLine()) != null)
            {
                if (str.StartsWith(commentHeader))
                {
                    str = str.substring(commentHeader.Length);
                    int pos = str.IndexOf(' ');
                    if (pos >= 0) str = str.substring(0, pos);
                    if (str.Length > 0)
                    {
                        try
                        {
                            version = Double.parseDouble(str);
                        }
                        catch (NumberFormatException nfe)
                        {
                            // Ignore - leave version as 0.0
                        }
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

    private static void genFile(string name, string version, String[] parameters)
    {
        File file = new File(Options.getOutputDirectory(), name);
        try
        {
            OutputFile outputFile = new OutputFile(file, version, parameters);

            if (!outputFile.NeedToWrite)
            {
                return;
            }

            TextWriter ostr = outputFile.getPrintWriter();
            OutputFileGenerator generator = new OutputFileGenerator(
                "/templates/cpp/" + name + ".template", Options.getOptions());
            generator.generate(ostr);
            ostr.Close();
        }
        catch (IOException e)
        {
            Console.Error.WriteLine("Failed to create file: " + file + e);
            JavaCCErrors.semantic_error("Could not open file: " + file + " for writing.");
            throw new Error();
        }
    }

    public static void gen_CharStream()
    {
        String[] parameters = new String[] { Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
        genFile("CharStream.h", charStreamVersion, parameters);
        genFile("CharStream.cc", charStreamVersion, parameters);
    }

    public static void gen_ParseException()
    {
        String[] parameters = new String[] { Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
        genFile("ParseException.h", parseExceptionVersion, parameters);
        genFile("ParseException.cc", parseExceptionVersion, parameters);
    }

    public static void gen_TokenMgrError()
    {
        String[] parameters = new String[] { Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
        genFile("TokenMgrError.h", tokenMgrErrorVersion, parameters);
        genFile("TokenMgrError.cc", tokenMgrErrorVersion, parameters);
    }

    public static void gen_Token()
    {
        String[] parameters = new String[] { Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC, Options.USEROPTION__CPP_TOKEN_INCLUDE, Options.USEROPTION__TOKEN_SUPER_CLASS };
        genFile("Token.h", tokenMgrErrorVersion, parameters);
        genFile("Token.cc", tokenMgrErrorVersion, parameters);
    }

    public static void gen_TokenManager()
    {
        String[] parameters = new String[] { Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
        genFile("TokenManager.h", tokenManagerVersion, parameters);
    }

    public static void gen_JavaCCDefs()
    {
        String[] parameters = new String[] { Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
        genFile("JavaCC.h", tokenManagerVersion, parameters);
    }

    public static void gen_ErrorHandler()
    {
        String[] parameters = new String[] { Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC, Options.USEROPTION__BUILD_PARSER, Options.USEROPTION__BUILD_TOKEN_MANAGER };
        genFile("ErrorHandler.h", parseExceptionVersion, parameters);
    }

    public static void reInit()
    {
    }

}
