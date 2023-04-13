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



using org.javacc.parser;
using System.Security;

namespace org.javacc.jjdoc;


/**
 * Main class.
 */
public class JJDocMain : JJDocGlobals
{
    private static void HelpMessage()
    {
        Info("");
        Info("    jjdoc option-settings - (to read from standard input)");
        Info("OR");
        Info("    jjdoc option-settings inputfile (to read from a file)");
        Info("");
        Info("WHERE");
        Info("    \"option-settings\" is a sequence of settings separated by spaces.");
        Info("");

        Info("Each option setting must be of one of the following forms:");
        Info("");
        Info("    -optionname=value (e.g., -TEXT=false)");
        Info("    -optionname:value (e.g., -TEXT:false)");
        Info("    -optionname       (equivalent to -optionname=true.  e.g., -TEXT)");
        Info("    -NOoptionname     (equivalent to -optionname=false. e.g., -NOTEXT)");
        Info("");
        Info("Option settings are not case-sensitive, so one can say \"-nOtExT\" instead");
        Info("of \"-NOTEXT\".  Option values must be appropriate for the corresponding");
        Info("option, and must be either an integer, boolean or string value.");
        Info("");
        Info("The string valued options are:");
        Info("");
        Info("    OUTPUT_FILE");
        Info("    CSS");
        Info("");
        Info("The boolean valued options are:");
        Info("");
        Info("    ONE_TABLE              (default true)");
        Info("    TEXT                   (default false)");
        Info("    BNF                    (default false)");
        Info("");

        Info("");
        Info("EXAMPLES:");
        Info("    jjdoc -ONE_TABLE=false mygrammar.jj");
        Info("    jjdoc - < mygrammar.jj");
        Info("");
        Info("ABOUT JJDoc:");
        Info("    JJDoc generates JavaDoc documentation from JavaCC grammar files.");
        Info("");
        Info("    For more information, see the online JJDoc documentation at");
        Info("    https://javacc.dev.java.net/doc/JJDoc.html");
    }

    /**
     * A main program that exercises the parser.
     */
    public static void Main(string[] args)
    {
        int errorcode = mainProgram(args);
        Environment.Exit(errorcode);
    }

    /**
     * The method to call to exercise the parser from other Java programs.
     * It returns an error code.  See how the main program above uses
     * this method.
     */
    public static int mainProgram(string[] args)
    {

        javacc.parser.MainParser.ReInitAll();
        JJDocOptions.Init();

        bannerLine("Documentation Generator", "0.1.4");

        JavaCCParser parser = null;
        if (args.Length == 0)
        {
            HelpMessage();
            return 1;
        }
        else
        {
            Info("(type \"jjdoc\" with no arguments for help)");
        }


        if (JJDocOptions.isOption(args[^1]))
        {
            Error("Last argument \"" + args[^1] + "\" is not a filename or \"-\".  ");
            return 1;
        }
        for (int arg = 0; arg < args.Length - 1; arg++)
        {
            if (!JJDocOptions.isOption(args[arg]))
            {
                Error("Argument \"" + args[arg] + "\" must be an option setting.  ");
                return 1;
            }
            JJDocOptions.setCmdLineOption(args[arg]);
        }

        if (args[^1] == ("-"))
        {
            Info("Reading from standard input . . .");
            parser = new JavaCCParser(Console.In);
            JJDocGlobals.input_file = "standard input";
            JJDocGlobals.output_file = "standard output";
        }
        else
        {
            Info("Reading from file " + args[args.Length - 1] + " . . .");
            try
            {
                string fp = (args[^1]);
                if (!fp.exists())
                {
                    Error("File " + args[args.Length - 1] + " not found.");
                    return 1;
                }
                if (fp.isDirectory())
                {
                    Error(args[args.Length - 1] + " is a directory. Please use a valid file name.");
                    return 1;
                }
                JJDocGlobals.input_file = fp.getName();
                parser = new JavaCCParser(new BufferedReader(new InputStreamReader(new FileInputStream(args[args.Length - 1]), JJDocOptions.getGrammarEncoding())));
            }
            catch (SecurityException se)
            {
                Error("Security violation while trying to open " + args[args.Length - 1]);
                return 1;
            }
            catch (FileNotFoundException e)
            {
                Error("File " + args[args.Length - 1] + " not found.");
                return 1;
            }
        }
        try
        {

            parser.javacc_input();
            JJDoc.Start();

            if (JavaCCErrors.get_error_count() == 0)
            {
                if (JavaCCErrors.get_warning_count() == 0)
                {
                    Info("Grammar documentation generated successfully in " + JJDocGlobals.output_file);
                }
                else
                {
                    Info("Grammar documentation generated with 0 errors and "
                                       + JavaCCErrors.get_warning_count() + " warnings.");
                }
                return 0;
            }
            else
            {
                Error("Detected " + JavaCCErrors.get_error_count() + " errors and "
                                   + JavaCCErrors.get_warning_count() + " warnings.");
                return (JavaCCErrors.get_error_count() == 0) ? 0 : 1;
            }
        }
        catch (org.javacc.parser.MetaParseException e)
        {
            Error(e.ToString());
            Error("Detected " + JavaCCErrors.get_error_count() + " errors and "
                               + JavaCCErrors.get_warning_count() + " warnings.");
            return 1;
        }
        catch (org.javacc.parser.ParseException e)
        {
            Error(e.ToString());
            Error("Detected " + (JavaCCErrors.get_error_count() + 1) + " errors and "
                               + JavaCCErrors.get_warning_count() + " warnings.");
            return 1;
        }
    }
}
