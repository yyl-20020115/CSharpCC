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

using CSharpCC.CCTree;
using CSharpCC.Utils;
using System.Text;
using System.Text.RegularExpressions;

namespace CSharpCC.Parser;



/**
 * Generate CharStream, TokenManager and Exceptions.
 */
public partial class CSharpFiles : CSharpCCGlobals
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


    public interface JavaResourceTemplateLocations
    {
        string GetTokenManagerTemplateResourceUrl();
        string GetTokenTemplateResourceUrl();
        string GetTokenMgrErrorTemplateResourceUrl();
        string GetJavaCharStreamTemplateResourceUrl();
        string GetCharStreamTemplateResourceUrl();
        string GetSimpleCharStreamTemplateResourceUrl();
        string GetParseExceptionTemplateResourceUrl();
    }


    public class JavaModernResourceTemplateLocationImpl : JavaResourceTemplateLocations
    {
        public string GetTokenMgrErrorTemplateResourceUrl() =>
            // Same as Java
            "/templates/TokenMgrError.template";
        public string GetCharStreamTemplateResourceUrl() =>
            // Same as Java
            "/templates/CharStream.template";

        public string GetTokenManagerTemplateResourceUrl() =>
            // Same as Java
            "/templates/TokenManager.template";

        public string GetTokenTemplateResourceUrl() =>
            // Same as Java
            "/templates/Token.template";

        public string GetSimpleCharStreamTemplateResourceUrl() => 
            "/templates/gwt/SimpleCharStream.template";


        public string GetJavaCharStreamTemplateResourceUrl() => 
            "/templates/gwt/JavaCharStream.template";


        public string GetParseExceptionTemplateResourceUrl() =>
            "/templates/gwt/ParseException.template";
    }


    public class JavaResourceTemplateLocationImpl : JavaResourceTemplateLocations
    {

        public string GetTokenTemplateResourceUrl() 
            => "/templates/Token.template";
        public string GetTokenManagerTemplateResourceUrl() 
            => "/templates/TokenManager.template";
        public string GetTokenMgrErrorTemplateResourceUrl()
            => "/templates/TokenMgrError.template";
        public string GetJavaCharStreamTemplateResourceUrl() 
            => "/templates/JavaCharStream.template";

        public string GetCharStreamTemplateResourceUrl()
            => "/templates/CharStream.template";
        public string GetSimpleCharStreamTemplateResourceUrl() 
            => "/templates/SimpleCharStream.template";

        public string GetParseExceptionTemplateResourceUrl()
            => "/templates/ParseException.template";

    }

    public static readonly JavaResourceTemplateLocations RESOURCES_JAVA_CLASSIC = new JavaResourceTemplateLocationImpl();
    public static readonly JavaResourceTemplateLocations RESOURCES_JAVA_MODERN = new JavaModernResourceTemplateLocationImpl();


    /**
     * Replaces all backslahes with double backslashes.
     */
    static string ReplaceBackslash(string str)
    {
        StringBuilder builder;
        int i = 0, len = str.Length;

        while (i < len && str[i++] != '\\') ;

        if (i == len)  // No backslash found.
            return str;

        char c;
        builder = new StringBuilder();
        for (i = 0; i < len; i++)
            if ((c = str[i]) == '\\')
                builder.Append("\\\\");
            else
                builder.Append(c);

        return builder.ToString();
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

    static readonly Regex regex = new ("[^0-9.]+.*");
    static readonly Regex versionRegex = new("[^0-9.]+.*");

    static readonly Regex numberRegex = new("(\\d+(\\.\\d+)?).*");
    public static double GetVersion(string fileName)
    {
        string commentHeader = "/* " + GetIdString(ToolName, fileName) + " Version ";
        string file = Path.Combine(Options.GetOutputDirectory(), ReplaceBackslash(fileName));

        if (!File.Exists(file))
        {
            // Has not yet been created, so it must be up to date.
            string majorVersion = versionRegex.Replace(Version.MajorDotMinor, "");
            return double.TryParse(majorVersion, out var d) ? d : 0;
        }

        StreamReader reader = null;
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
                    if (str.Length > 0)
                    {

                        // str can be 4.09
                        // or even 7.0.5
                        // So far we keep only major.minor part
                        // "4 qwerty"-> "4"
                        // "4.09 qwerty" -> "4.09"
                        // "7.0.5 qwerty" -> "7.0"
                        //str = str.replaceAll("(\\d+(\\.\\d+)?).*", "$1");
                        str = numberRegex.Replace(str, "$1");

                        return double.TryParse(str, out version) ? version : 0.0;
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



    public static void GenJavaCharStream(JavaResourceTemplateLocations locations)
    {
        try
        {
            string file = Path.Combine(Options.GetOutputDirectory(), "JavaCharStream.java");
            var outputFile = new OutputFile(file, CharStreamVersion, new string[] { Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC });

            if (!outputFile.NeedToWrite)
            {
                return;
            }

            TextWriter ostr = outputFile.GetPrintWriter();

            if (CuToInsertionPoint1.Count != 0 &&
                CuToInsertionPoint1[0].kind == PACKAGE
            )
            {
                for (int i = 1; i < CuToInsertionPoint1.Count; i++)
                {
                    if (CuToInsertionPoint1[i].kind == SEMICOLON)
                    {
                        cline = CuToInsertionPoint1[0].beginLine;
                        ccol = CuToInsertionPoint1[0].beginColumn;
                        for (int j = 0; j <= i; j++)
                        {
                            PrintToken(CuToInsertionPoint1[j], ostr);
                        }
                        ostr.WriteLine("");
                        ostr.WriteLine("");
                        break;
                    }
                }
            }
            var prefix = Options.GetStatic() ? "static " : "";
            Dictionary<string, object> options = new(Options.getOptions())
            {
                { "PREFIX", prefix }
            };

            var generator = new OutputFileGenerator(
                    locations.GetJavaCharStreamTemplateResourceUrl(), options);

            generator.Generate(ostr);

            ostr.Close();
        }
        catch (IOException e)
        {
            Console.Error.WriteLine("Failed to create JavaCharStream " + e);
            CSharpCCErrors.SemanticError("Could not open file JavaCharStream.java for writing.");
            throw new Error();
        }
    }



    public static void GenSimpleCharStream(JavaResourceTemplateLocations locations)
    {
        try
        {
            string file = Path.Combine(Options.GetOutputDirectory(), "SimpleCharStream.java");
            var outputFile = new OutputFile(file, CharStreamVersion, new string[] { Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC });

            if (!outputFile.NeedToWrite)
            {
                return;
            }

            TextWriter ostr = outputFile.GetPrintWriter();

            if (CuToInsertionPoint1.Count != 0 &&
                CuToInsertionPoint1[0].kind == PACKAGE
            )
            {
                for (int i = 1; i < CuToInsertionPoint1.Count; i++)
                {
                    if (CuToInsertionPoint1[i].kind == SEMICOLON)
                    {
                        cline = CuToInsertionPoint1[0].beginLine;
                        ccol = CuToInsertionPoint1[0].beginColumn;
                        for (int j = 0; j <= i; j++)
                        {
                            PrintToken(CuToInsertionPoint1[j], ostr);
                        }
                        ostr.WriteLine();
                        ostr.WriteLine();
                        break;
                    }
                }
            }
            string prefix = Options.GetStatic() ? "static " : "";
            var options = new Dictionary<string,object>(Options.getOptions());
            options.Add("PREFIX", prefix);

            OutputFileGenerator generator = new OutputFileGenerator(
                    locations.GetSimpleCharStreamTemplateResourceUrl(), options);

            generator.Generate(ostr);

            ostr.Close();
        }
        catch (IOException e)
        {
            Console.Error.WriteLine("Failed to create SimpleCharStream " + e);
            CSharpCCErrors.SemanticError("Could not open file SimpleCharStream.java for writing.");
            throw new Error();
        }
    }



    public static void GenCharStream(JavaResourceTemplateLocations locations)
    {
        try
        {
            var file = Path.Combine(Options.GetOutputDirectory(), "CharStream.java");
            var outputFile = new OutputFile(file, CharStreamVersion, new string[] { Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC });

            if (!outputFile.NeedToWrite)
            {
                return;
            }

            TextWriter ostr = outputFile.GetPrintWriter();

            if (CuToInsertionPoint1.Count != 0 &&
                CuToInsertionPoint1[0].kind == PACKAGE
            )
            {
                for (int i = 1; i < CuToInsertionPoint1.Count; i++)
                {
                    if (CuToInsertionPoint1[i].kind == SEMICOLON)
                    {
                        cline = CuToInsertionPoint1[0].beginLine;
                        ccol = CuToInsertionPoint1[0].beginColumn;
                        for (int j = 0; j <= i; j++)
                        {
                            PrintToken(CuToInsertionPoint1[j], ostr);
                        }
                        ostr.WriteLine("");
                        ostr.WriteLine("");
                        break;
                    }
                }
            }

            var generator = new OutputFileGenerator(
                    locations.GetCharStreamTemplateResourceUrl(), Options.getOptions());

            generator.Generate(ostr);

            ostr.Close();
        }
        catch (IOException e)
        {
            Console.Error.WriteLine("Failed to create CharStream " + e);
            CSharpCCErrors.SemanticError("Could not open file CharStream.java for writing.");
            throw new Error();
        }
    }



    public static void GenJavaModernFiles()
    {
        GenMiscFile("Provider.java", "/templates/gwt/Provider.template");
        GenMiscFile("StringProvider.java", "/templates/gwt/StringProvider.template");

        // This provides a bridge to standard Java readers.
        GenMiscFile("StreamProvider.java", "/templates/gwt/StreamProvider.template");
    }

    private static void GenMiscFile(string fileName, string templatePath)
    {
        try
        {
            var file = Path.Combine(Options.GetOutputDirectory(), fileName);
            var outputFile = new OutputFile(file, ParseExceptionVersion, new string[] {/* cba -- 2013/07/22 -- previously wired to a typo version of this option -- KEEP_LINE_COL */ Options.USEROPTION__KEEP_LINE_COLUMN });

            if (!outputFile.NeedToWrite)
            {
                return;
            }

            var ostr = outputFile.GetPrintWriter();

            if (CuToInsertionPoint1.Count != 0 &&
                CuToInsertionPoint1[0].kind == PACKAGE
            )
            {
                for (int i = 1; i < CuToInsertionPoint1.Count; i++)
                {
                    if (CuToInsertionPoint1[i].kind == SEMICOLON)
                    {
                        cline = CuToInsertionPoint1[0].beginLine;
                        ccol = CuToInsertionPoint1[0].beginColumn;
                        for (int j = 0; j <= i; j++)
                        {
                            PrintToken(CuToInsertionPoint1[j], ostr);
                        }
                        ostr.WriteLine("");
                        ostr.WriteLine("");
                        break;
                    }
                }
            }

            var generator = new OutputFileGenerator(templatePath, Options.getOptions());

            generator.Generate(ostr);

            ostr.Close();
        }
        catch (IOException e)
        {
            Console.Error.WriteLine("Failed to create " + fileName + " " + e);
            CSharpCCErrors.SemanticError("Could not open file " + fileName + " for writing.");
            throw new Error();
        }
    }


    public static void GenParseException(JavaResourceTemplateLocations locations)
    {
        try
        {
            string file = Path.Combine(Options.GetOutputDirectory(), "ParseException.java");
            OutputFile outputFile = new OutputFile(file, ParseExceptionVersion, new string[] {/* cba -- 2013/07/22 -- previously wired to a typo version of this option -- KEEP_LINE_COL */ Options.USEROPTION__KEEP_LINE_COLUMN });

            if (!outputFile.NeedToWrite)
            {
                return;
            }

            var ostr = outputFile.GetPrintWriter();

            if (CuToInsertionPoint1.Count != 0 &&
                CuToInsertionPoint1[0].kind == PACKAGE
            )
            {
                for (int i = 1; i < CuToInsertionPoint1.Count; i++)
                {
                    if (CuToInsertionPoint1[i].kind == SEMICOLON)
                    {
                        cline = CuToInsertionPoint1[0].beginLine;
                        ccol = CuToInsertionPoint1[0].beginColumn;
                        for (int j = 0; j <= i; j++)
                        {
                            PrintToken(CuToInsertionPoint1[j], ostr);
                        }
                        ostr.WriteLine("");
                        ostr.WriteLine("");
                        break;
                    }
                }
            }

            var generator = new OutputFileGenerator(
                    locations.GetParseExceptionTemplateResourceUrl(), Options.getOptions());

            generator.Generate(ostr);

            ostr.Close();
        }
        catch (IOException e)
        {
            Console.Error.WriteLine("Failed to create ParseException " + e);
            CSharpCCErrors.SemanticError("Could not open file ParseException.java for writing.");
            throw new Error();
        }
    }



    public static void GenTokenMgrError(JavaResourceTemplateLocations locations)
    {
        bool isLegacyExceptionHandling = Options.IsLegacyExceptionHandling();
        string filename = isLegacyExceptionHandling ? "TokenMgrError.java" : "TokenMgrException.java";
        try
        {

            string file = Path.Combine(Options.GetOutputDirectory(), filename);
            OutputFile outputFile = new OutputFile(file, TokenMgrErrorVersion, Array.Empty<string>());

            if (!outputFile.NeedToWrite)
            {
                return;
            }

            TextWriter ostr = outputFile.GetPrintWriter();

            if (CuToInsertionPoint1.Count != 0 &&
                CuToInsertionPoint1[0].kind == PACKAGE
            )
            {
                for (int i = 1; i < CuToInsertionPoint1.Count; i++)
                {
                    if (CuToInsertionPoint1[i].kind == SEMICOLON)
                    {
                        cline = CuToInsertionPoint1[0].beginLine;
                        ccol = CuToInsertionPoint1[0].beginColumn;
                        for (int j = 0; j <= i; j++)
                        {
                            PrintToken(CuToInsertionPoint1[j], ostr);
                        }
                        ostr.WriteLine("");
                        ostr.WriteLine("");
                        break;
                    }
                }
            }



            var generator = new OutputFileGenerator(locations.GetTokenMgrErrorTemplateResourceUrl(), Options.getOptions());

            generator.Generate(ostr);

            ostr.Close();


        }
        catch (IOException e)
        {
            Console.Error.WriteLine("Failed to create " + filename + " " + e);
            CSharpCCErrors.SemanticError("Could not open file " + filename + " for writing.");
            throw new Error();
        }
    }



    public static void GenToken(JavaResourceTemplateLocations locations)
    {
        try
        {
            string file = Path.Combine(Options.GetOutputDirectory(), "Token.java");
            OutputFile outputFile = new OutputFile(file, TokenVersion, new string[] { Options.USEROPTION__TOKEN_EXTENDS, Options.USEROPTION__KEEP_LINE_COLUMN, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC });

            if (!outputFile.NeedToWrite)
            {
                return;
            }

            TextWriter ostr = outputFile.GetPrintWriter();

            if (CuToInsertionPoint1.Count != 0 &&
                CuToInsertionPoint1[0].kind == PACKAGE
            )
            {
                for (int i = 1; i < CuToInsertionPoint1.Count; i++)
                {
                    if (CuToInsertionPoint1[i].kind == SEMICOLON)
                    {
                        cline = CuToInsertionPoint1[0].beginLine;
                        ccol = CuToInsertionPoint1[0].beginColumn;
                        for (int j = 0; j <= i; j++)
                        {
                            PrintToken(CuToInsertionPoint1[j], ostr);
                        }
                        ostr.WriteLine("");
                        ostr.WriteLine("");
                        break;
                    }
                }
            }

            var generator = new OutputFileGenerator(
                    locations.GetTokenTemplateResourceUrl(), Options.getOptions());

            generator.Generate(ostr);

            ostr.Close();
        }
        catch (IOException e)
        {
            Console.Error.WriteLine("Failed to create Token " + e);
            CSharpCCErrors.SemanticError("Could not open file Token.java for writing.");
            throw new Error();
        }
    }



    public static void GenTokenManager(JavaResourceTemplateLocations locations)
    {
        try
        {
            string file = Path.Combine(Options.GetOutputDirectory(), "TokenManager.java");
            OutputFile outputFile = new OutputFile(file, TokenManagerVersion, new string[] { Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC });

            if (!outputFile.NeedToWrite)
            {
                return;
            }

            TextWriter ostr = outputFile.GetPrintWriter();

            if (CuToInsertionPoint1.Count != 0 &&
                CuToInsertionPoint1[0].kind == PACKAGE
            )
            {
                for (int i = 1; i < CuToInsertionPoint1.Count; i++)
                {
                    if (CuToInsertionPoint1[i].kind == SEMICOLON)
                    {
                        cline = CuToInsertionPoint1[0].beginLine;
                        ccol = CuToInsertionPoint1[0].beginColumn;
                        for (int j = 0; j <= i; j++)
                        {
                            PrintToken(CuToInsertionPoint1[j], ostr);
                        }
                        ostr.WriteLine("");
                        ostr.WriteLine("");
                        break;
                    }
                }
            }

            var generator = new OutputFileGenerator(
                    locations.GetTokenManagerTemplateResourceUrl(), Options.getOptions());

            generator.Generate(ostr);

            ostr.Close();
        }
        catch (IOException e)
        {
            Console.Error.WriteLine("Failed to create TokenManager " + e);
            CSharpCCErrors.SemanticError("Could not open file TokenManager.java for writing.");
            throw new Error();
        }
    }


    public static new void ReInit()
    {
    }
}
