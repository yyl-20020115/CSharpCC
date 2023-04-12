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
using org.javacc.jjtree;

namespace org.javacc.parser;



/**
 * Generates the Constants file.
 */
public class OtherFilesGen : JavaCCGlobals, JavaCCParserConstants
{

    private static readonly string CONSTANTS_FILENAME_SUFFIX = "Constants.java";

    static public void start(bool isJavaModern)
    {

        JavaResourceTemplateLocations templateLoc = isJavaModern ? JavaFiles.RESOURCES_JAVA_MODERN : JavaFiles.RESOURCES_JAVA_CLASSIC;

        Token t = null;

        if (JavaCCErrors.get_error_count() != 0) throw new MetaParseException();

        // Added this if condition -- 2012/10/17 -- cba
        if (Options.isGenerateBoilerplateCode())
        {

            if (isJavaModern)
            {
                JavaFiles.gen_JavaModernFiles();
            }

            JavaFiles.gen_TokenMgrError(templateLoc);
            JavaFiles.gen_ParseException(templateLoc);
            JavaFiles.gen_Token(templateLoc);
        }


        if (Options.getUserTokenManager())
        {
            // CBA -- I think that Token managers are unique so will always be generated
            JavaFiles.gen_TokenManager(templateLoc);
        }
        else if (Options.getUserCharStream())
        {
            // Added this if condition -- 2012/10/17 -- cba
            if (Options.isGenerateBoilerplateCode())
            {
                JavaFiles.gen_CharStream(templateLoc);
            }
        }
        else
        {
            // Added this if condition -- 2012/10/17 -- cba

            if (Options.isGenerateBoilerplateCode())
            {
                if (Options.getJavaUnicodeEscape())
                {
                    JavaFiles.gen_JavaCharStream(templateLoc);
                }
                else
                {
                    JavaFiles.gen_SimpleCharStream(templateLoc);
                }
            }
        }

        try
        {
            ostr = new TextWriter(
                      new BufferedWriter(
                         new FileWriter(
                           new File(Options.getOutputDirectory(), cu_name + CONSTANTS_FILENAME_SUFFIX)
                         ),
                         8192
                      )
                   );
        }
        catch (IOException e)
        {
            JavaCCErrors.semantic_error("Could not open file " + cu_name + "Constants.java for writing.");
            throw new Error();
        }

        List<string> tn = new(toolNames);
        tn.Add(toolName);
        ostr.WriteLine("/* " + getIdString(tn, cu_name + CONSTANTS_FILENAME_SUFFIX) + " */");

        if (cu_to_insertion_point_1.Count != 0 &&
            ((Token)cu_to_insertion_point_1[0]).kind == PACKAGE
           )
        {
            for (int i = 1; i < cu_to_insertion_point_1.Count; i++)
            {
                if (((Token)cu_to_insertion_point_1[i]).kind == SEMICOLON)
                {
                    printTokenSetup((Token)(cu_to_insertion_point_1[0]));
                    for (int j = 0; j <= i; j++)
                    {
                        t = (Token)(cu_to_insertion_point_1[j]);
                        printToken(t, ostr);
                    }
                    printTrailingComments(t, ostr);
                    ostr.WriteLine("");
                    ostr.WriteLine("");
                    break;
                }
            }
        }
        ostr.WriteLine("");
        ostr.WriteLine("/**");
        ostr.WriteLine(" * Token literal values and constants.");
        ostr.WriteLine(" * Generated by org.javacc.parser.OtherFilesGen#start()");
        ostr.WriteLine(" */");

        if (Options.getSupportClassVisibilityPublic())
        {
            ostr.print("public ");
        }
        ostr.WriteLine("interface " + cu_name + "Constants {");
        ostr.WriteLine("");

        RegularExpression re;
        ostr.WriteLine("  /** End of File. */");
        ostr.WriteLine("  int EOF = 0;");
        for (Iterator<RegularExpression> it = ordered_named_tokens.iterator(); it.hasNext();)
        {
            re = it.next();
            ostr.WriteLine("  /** RegularExpression Id. */");
            ostr.WriteLine("  int " + re.label + " = " + re.ordinal + ";");
        }
        ostr.WriteLine("");
        if (!Options.getUserTokenManager() && Options.getBuildTokenManager())
        {
            for (int i = 0; i < Main.lg.lexStateName.Length; i++)
            {
                ostr.WriteLine("  /** Lexical state. */");
                ostr.WriteLine("  int " + LexGen.lexStateName[i] + " = " + i + ";");
            }
            ostr.WriteLine("");
        }
        ostr.WriteLine("  /** Literal token values. */");
        ostr.WriteLine("  String[] tokenImage = {");
        ostr.WriteLine("    \"<EOF>\",");

        for (Iterator<TokenProduction> it = rexprlist.iterator(); it.hasNext();)
        {
            TokenProduction tp = (TokenProduction)(it.next());
            List<RegExprSpec> respecs = tp.respecs;
            for (Iterator<RegExprSpec> it2 = respecs.iterator(); it2.hasNext();)
            {
                RegExprSpec res = (RegExprSpec)(it2.next());
                re = res.rexp;
                ostr.print("    ");
                if (re is RStringLiteral)
                {
                    ostr.WriteLine("\"\\\"" + add_escapes(add_escapes(((RStringLiteral)re).image)) + "\\\"\",");
                }
                else if (!re.label == (""))
                {
                    ostr.WriteLine("\"<" + re.label + ">\",");
                }
                else
                {
                    if (re.tpContext.kind == TokenProduction.TOKEN)
                    {
                        JavaCCErrors.warning(re, "Consider giving this non-string token a label for better error reporting.");
                    }
                    ostr.WriteLine("\"<token of kind " + re.ordinal + ">\",");
                }

            }
        }
        ostr.WriteLine("  };");
        ostr.WriteLine("");
        ostr.WriteLine("}");

        ostr.Close();

    }

    static private TextWriter ostr;

    public static void reInit()
    {
        ostr = null;
    }

}
