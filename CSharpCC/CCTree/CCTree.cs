// Copyright 2011 Google Inc. All Rights Reserved.
// Author: sreeni@google.com (Sreeni Viswanadha)

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

using CSharpCC.Parser;

namespace CSharpCC.CCTree;


public class CCTree
{
    private IO io;

    private void Print(string s)
    {
        io.        Msg.WriteLine(s);
    }

    private void HelpMessage()
    {
        Print("Usage:");
        Print("    cctree option-settings inputfile");
        Print("");
        Print("\"option-settings\" is a sequence of settings separated by spaces.");
        Print("Each option setting must be of one of the following forms:");
        Print("");
        Print("    -optionname=value (e.g., -STATIC=false)");
        Print("    -optionname:value (e.g., -STATIC:false)");
        Print("    -optionname       (equivalent to -optionname=true.  e.g., -STATIC)");
        Print("    -NOoptionname     (equivalent to -optionname=false. e.g., -NOSTATIC)");
        Print("");
        Print("Option settings are not case-sensitive, so one can say \"-nOsTaTiC\" instead");
        Print("of \"-NOSTATIC\".  Option values must be appropriate for the corresponding");
        Print("option, and must be either an integer or a string value.");
        Print("");

        Print("The boolean valued options are:");
        Print("");
        Print("    STATIC                   (default true)");
        Print("    MULTI                    (default false)");
        Print("    NODE_DEFAULT_VOID        (default false)");
        Print("    NODE_SCOPE_HOOK          (default false)");
        Print("    NODE_USES_PARSER         (default false)");
        Print("    BUILD_NODE_FILES         (default true)");
        Print("    TRACK_TOKENS             (default false)");
        Print("    VISITOR                  (default false)");
        Print("");
        Print("The string valued options are:");
        Print("");
        Print("    JDK_VERSION              (default \"1.5\")");
        Print("    NODE_CLASS               (default \"\")");
        Print("    NODE_PREFIX              (default \"AST\")");
        Print("    NODE_PACKAGE             (default \"\")");
        Print("    NODE_EXTENDS             (default \"\")");
        Print("    NODE_FACTORY             (default \"\")");
        Print("    OUTPUT_FILE              (default remove input file suffix, add .jj)");
        Print("    OUTPUT_DIRECTORY         (default \"\")");
        Print("    JJTREE_OUTPUT_DIRECTORY  (default value of OUTPUT_DIRECTORY option)");
        Print("    VISITOR_DATA_TYPE        (default \"\")");
        Print("    VISITOR_RETURN_TYPE      (default \"Object\")");
        Print("    VISITOR_EXCEPTION        (default \"\")");
        Print("");
        Print("CCTree also accepts CSharpCC options, which it inserts into the generated file.");
        Print("");

        Print("EXAMPLES:");
        Print("    cctree -STATIC=false mygrammar.jjt");
        Print("");
        Print("ABOUT CCTree:");
        Print("    CCTree is a preprocessor for JavaCC that inserts actions into a");
        Print("    JavaCC grammar to build parse trees for the input.");
        Print("");
        Print("    For more information, see the online JJTree documentation at ");
        Print("    https://javacc.dev.java.net/doc/JJTree.html ");
        Print("");
    }

    /**
     * A main program that exercises the parser.
     */
    public int Main(string[] args)
    {
        // initialize static state for allowing repeat runs without exiting
        ASTNodeDescriptor.NodeIds = new ();
        ASTNodeDescriptor.NodeNames = new ();
        ASTNodeDescriptor.NodeSeen = new ();
        global::CSharpCC.Parser.MainParser.ReInitAll();

        CSharpCCGlobals.BannerLine("Tree Builder", "");

        io = new IO();

        try
        {

            InitializeOptions();
            if (args.Length == 0)
            {
                Print("");
                HelpMessage();
                return 1;
            }
            else
            {
                Print("(type \"cctree\" with no arguments for help)");
            }

            string fn = args[^1];

            if (CCTreeOptions.IsOption(fn))
            {
                Print($"Last argument \"{fn}\" is not a filename");
                return 1;
            }
            for (int arg = 0; arg < args.Length - 1; arg++)
            {
                if (!CCTreeOptions.IsOption(args[arg]))
                {
                    Print($"Argument \"{args[arg]}\" must be an option setting.");
                    return 1;
                }
                CCTreeOptions.SetCmdLineOption(args[arg]);
            }

            CCTreeOptions.Validate();

            try
            {
                io.SetInput(fn);
            }
            catch (CCTreeIOException ioe)
            {
                Print("Error setting input: " + ioe.Message);
                return 1;
            }
            Print("Reading from file " + io.InputFileName + " . . .");

            CCTreeGlobals.ToolList = CSharpCCGlobals.GetToolNames(fn);
            CCTreeGlobals.ToolList.Add("JJTree");

            try
            {
                var parser = new CCTreeParser(io.In);
                parser.CSharpCCInput();

                ASTGrammar root = (ASTGrammar)parser.cctree.RootNode();
                if (bool.TryParse(Environment.GetEnvironmentVariable("jjtree-dump"),out var b) && b)
                {
                    root.Dump(" ");
                }
                try
                {
                    io.SetOutput();
                }
                catch (CCTreeIOException ioe)
                {
                    Print("Error setting output: " + ioe.Message);
                    return 1;
                }
                root.Generate(io);
                io.                Out.Close();

                string outputLanguage = CCTreeOptions.GetOutputLanguage();

                // TODO :: Not yet tested this in GWT/Modern mode (disabled by default in 6.1)

                if (CCTreeOptions.IsOutputLanguageJava())
                {
                    NodeFiles.GenerateTreeConstantsJava();
                    NodeFiles.GenerateVisitorJava();
                    NodeFiles.GenerateDefaultVisitorJava();
                    CCTreeState.GenerateTreeStateJava();
                }
                else if (CCTreeOptions.IsOutputLanguageCpp())
                {
                    CPPNodeFiles.GenerateTreeConstants();
                    CPPNodeFiles.GenerateVisitors();
                    //CPPNodeFiles.generateDefaultVisitor();
                    CPPCCTreeState.GenerateTreeState();
                    //CPPNodeFiles.generateJJTreeH();
                }
                else
                {
                    Print($"Unsupported CCTree output language : {outputLanguage}");
                    return 1;
                }

                Print($"Annotated grammar generated successfully in {io.OutputFileName}");

            }
            catch (ParseException pe)
            {
                Print($"Error parsing input: {pe}");
                return 1;
            }
            catch (Exception e)
            {
                Print($"Error parsing input: {e}");
                //e.printStackTrace(io.GetMsg());
                return 1;
            }

            return 0;

        }
        finally
        {
            io.CloseAll();
        }
    }

    /**
     * Initialize for CCTree
     */
    private void InitializeOptions()
    {
        CCTreeOptions.Init();
        CCTreeGlobals.Initialize();
    }
}
