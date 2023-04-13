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
using CSharpCC.CCTree;
using CSharpCC.Utils;

namespace CSharpCC.Parser;

/**
 * Generates the Constants file.
 */
public class OtherFilesGen : CSharpCCGlobals
{

    private static readonly string CONSTANTS_FILENAME_SUFFIX = "Constants.java";

    static public void Start(bool isJavaModern)
    {

        var templateLoc = isJavaModern ? CSharpFiles.RESOURCES_JAVA_MODERN : CSharpFiles.RESOURCES_JAVA_CLASSIC;

        Token t = null;

        if (CSharpCCErrors.ErrorCount != 0) throw new MetaParseException();

        // Added this if condition -- 2012/10/17 -- cba
        if (Options.IsGenerateBoilerplateCode())
        {

            if (isJavaModern)
            {
                CSharpFiles.GenJavaModernFiles();
            }

            CSharpFiles.GenTokenMgrError(templateLoc);
            CSharpFiles.GenParseException(templateLoc);
            CSharpFiles.GenToken(templateLoc);
        }


        if (Options.GetUserTokenManager())
        {
            // CBA -- I think that Token managers are unique so will always be generated
            CSharpFiles.GenTokenManager(templateLoc);
        }
        else if (Options.GetUserCharStream())
        {
            // Added this if condition -- 2012/10/17 -- cba
            if (Options.IsGenerateBoilerplateCode())
            {
                CSharpFiles.GenCharStream(templateLoc);
            }
        }
        else
        {
            // Added this if condition -- 2012/10/17 -- cba

            if (Options.IsGenerateBoilerplateCode())
            {
                if (Options.GetJavaUnicodeEscape()) 
                { 
                    CSharpFiles.GenJavaCharStream(templateLoc);
                }
                else
                {
                    CSharpFiles.GenSimpleCharStream(templateLoc);
                }
            }
        }

        try
        {
            ostr = new StreamWriter(
                           System.IO.Path.Combine(Options.GetOutputDirectory(), CuName + CONSTANTS_FILENAME_SUFFIX)
                   );
        }
        catch (IOException e)
        {
            CSharpCCErrors.SemanticError("Could not open file " + CuName + "Constants.java for writing.");
            throw new Error();
        }

        List<string> tn = new(ToolNames)
        {
            ToolName
        };
        ostr.WriteLine("/* " + GetIdString(tn, CuName + CONSTANTS_FILENAME_SUFFIX) + " */");

        if (CuToInsertionPoint1.Count != 0 &&
            ((Token)CuToInsertionPoint1[0]).kind == PACKAGE
           )
        {
            for (int i = 1; i < CuToInsertionPoint1.Count; i++)
            {
                if (((Token)CuToInsertionPoint1[i]).kind == SEMICOLON)
                {
                    PrintTokenSetup((Token)(CuToInsertionPoint1[0]));
                    for (int j = 0; j <= i; j++)
                    {
                        t = (Token)(CuToInsertionPoint1[j]);
                        PrintToken(t, ostr);
                    }
                    PrintTrailingComments(t, ostr);
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

        if (Options.GetSupportClassVisibilityPublic())
        {
            ostr.Write("public ");
        }
        ostr.WriteLine("interface " + CuName + "Constants {");
        ostr.WriteLine("");

        ostr.WriteLine("  /** End of File. */");
        ostr.WriteLine("  int EOF = 0;");
        foreach(var re2 in OrderedNamedToken)
        {
            var re = re2;
            ostr.WriteLine("  /** RegularExpression Id. */");
            ostr.WriteLine("  int " + re.label + " = " + re.ordinal + ";");
        }
        ostr.WriteLine("");
        if (!Options.GetUserTokenManager() && Options.GetBuildTokenManager())
        {
            for (int i = 0; i < LexGen.lexStateName.Length; i++)
            {
                ostr.WriteLine("  /** Lexical state. */");
                ostr.WriteLine("  int " + LexGen.lexStateName[i] + " = " + i + ";");
            }
            ostr.WriteLine("");
        }
        ostr.WriteLine("  /** Literal token values. */");
        ostr.WriteLine("  String[] tokenImage = {");
        ostr.WriteLine("    \"<EOF>\",");

        foreach(var tp in RegexpList)
        {
            foreach(var res in tp.respecs)
            {
                var re = res.Rexp;
                ostr.Write("    ");
                if (re is RStringLiteral literal)
                { 
                    ostr.WriteLine("\"\\\"" + StringEscapeHelpers.AddEscapes(
                        StringEscapeHelpers.AddEscapes(literal.image)) + "\\\"\",");
                }
                else if (re.label != (""))
                { 
                    ostr.WriteLine("\"<" + re.label + ">\",");
                }
                else
                {
                    if (re.tpContext.kind == TokenProduction.TOKEN)
                    {
                        CSharpCCErrors.Warning(re, "Consider giving this non-string token a label for better error reporting.");
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

    public static new void ReInit()
    {
        ostr = null;
    }

}
