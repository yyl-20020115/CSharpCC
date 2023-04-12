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

using org.javacc.parser;

namespace org.javacc.jjtree;


public class JJTree
{

    private IO io;

    private void Print(string s)
    {
        io.GetMsg().WriteLine(s);
    }

    private void HelpMessage()
    {
        Print("Usage:");
        Print("    jjtree option-settings inputfile");
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
        Print("JJTree also accepts JavaCC options, which it inserts into the generated file.");
        Print("");

        Print("EXAMPLES:");
        Print("    jjtree -STATIC=false mygrammar.jjt");
        Print("");
        Print("ABOUT JJTree:");
        Print("    JJTree is a preprocessor for JavaCC that inserts actions into a");
        Print("    JavaCC grammar to build parse trees for the input.");
        Print("");
        Print("    For more information, see the online JJTree documentation at ");
        Print("    https://javacc.dev.java.net/doc/JJTree.html ");
        Print("");
    }

    /**
     * A main program that exercises the parser.
     */
    public int main(string[] args)
    {

        // initialize static state for allowing repeat runs without exiting
        ASTNodeDescriptor.NodeIds = new ();
        ASTNodeDescriptor.NodeNames = new ();
        ASTNodeDescriptor.nodeSeen = new ();
        org.javacc.parser.Main.reInitAll();

        JavaCCGlobals.bannerLine("Tree Builder", "");

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
                Print("(type \"jjtree\" with no arguments for help)");
            }

            string fn = args[args.Length - 1];

            if (JJTreeOptions.isOption(fn))
            {
                Print("Last argument \"" + fn + "\" is not a filename");
                return 1;
            }
            for (int arg = 0; arg < args.Length - 1; arg++)
            {
                if (!JJTreeOptions.isOption(args[arg]))
                {
                    Print("Argument \"" + args[arg] + "\" must be an option setting.");
                    return 1;
                }
                JJTreeOptions.setCmdLineOption(args[arg]);
            }

            JJTreeOptions.Validate();

            try
            {
                io.SetInput(fn);
            }
            catch (JJTreeIOException ioe)
            {
                Print("Error setting input: " + ioe.Message);
                return 1;
            }
            Print("Reading from file " + io.GetInputFileName() + " . . .");

            JJTreeGlobals.ToolList = JavaCCGlobals.getToolNames(fn);
            JJTreeGlobals.ToolList.Add("JJTree");

            try
            {
                var parser = new JJTreeParser(io.GetIn());
                parser.javacc_input();

                ASTGrammar root = (ASTGrammar)parser.jjtree.RootNode();
                if (bool.TryParse(Environment.GetEnvironmentVariable("jjtree-dump"),out var b) && b)
                {
                    root.Dump(" ");
                }
                try
                {
                    io.SetOutput();
                }
                catch (JJTreeIOException ioe)
                {
                    Print("Error setting output: " + ioe.Message);
                    return 1;
                }
                root.Generate(io);
                io.GetOut().Close();

                string outputLanguage = JJTreeOptions.getOutputLanguage();

                // TODO :: Not yet tested this in GWT/Modern mode (disabled by default in 6.1)

                if (JJTreeOptions.isOutputLanguageJava())
                {
                    NodeFiles.generateTreeConstants_java();
                    NodeFiles.generateVisitor_java();
                    NodeFiles.generateDefaultVisitor_java();
                    JJTreeState.GenerateTreeStateJava();
                }
                else if (JJTreeOptions.isOutputLanguageCpp())
                {
                    CPPNodeFiles.GenerateTreeConstants();
                    CPPNodeFiles.GenerateVisitors();
                    //CPPNodeFiles.generateDefaultVisitor();
                    CPPJJTreeState.GenerateTreeState();
                    //CPPNodeFiles.generateJJTreeH();
                }
                else
                {
                    Print("Unsupported JJTree output language : " + outputLanguage);
                    return 1;
                }

                Print("Annotated grammar generated successfully in " +
                      io.GetOutputFileName());

            }
            catch (ParseException pe)
            {
                Print("Error parsing input: " + pe.ToString());
                return 1;
            }
            catch (Exception e)
            {
                Print("Error parsing input: " + e.ToString());
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
     * Initialize for JJTree
     */
    private void InitializeOptions()
    {
        JJTreeOptions.Init();
        JJTreeGlobals.Initialize();
    }


}

/*end*/
