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
namespace CSharpCC.Parser;


/**
 * Generate the parser.
 */
public class ParseGen : CodeGenerator
{

    public void Start(bool isJavaModernMode)
    {

        Token t = null;

        if (CSharpCCErrors.GetErrorCount() != 0)
        {
            throw new MetaParseException();
        }

        if (Options.GetBuildParser())
        {
            List<string> tn = new(ToolNames)
            {
                ToolName
            };

            // This is the first line generated -- the the comment line at the top of the generated parser
            GenCodeLine("/* " + GetIdString(tn, CuName + ".java") + " */");

            bool implementsExists = false;
            //bool extendsExists = false;

            if (cu_to_insertion_point_1.Count != 0)
            {
                var firstToken = cu_to_insertion_point_1[0];
                PrintTokenSetup(firstToken);
                this.ccol = 1;
                foreach (var t2 in cu_to_insertion_point_1)
                {
                    t = t2;
                    if (t.kind == IMPLEMENTS)
                    {
                        implementsExists = true;
                    }
                    else if (t.kind == CLASS)
                    {
                        implementsExists = false;
                    }

                    PrintToken(t);
                }
            }

            if (implementsExists)
            {
                GenCode(", ");
            }
            else
            {
                GenCode(" implements ");
            }
            GenCode(CuName + "Constants ");
            if (cu_to_insertion_point_2.Count != 0)
            {
                CSharpCCGlobals.PrintTokenSetup((cu_to_insertion_point_2[0]));
                foreach (var t2 in cu_to_insertion_point_2)
                {
                    PrintToken(t = t2);
                }
            }

            GenCodeLine("");
            GenCodeLine("");

            new ParseEngine().build(this);

            if (Options.GetStatic())
            {
                GenCodeLine("  static private " + Options.GetBooleanType()
                        + " jj_initialized_once = false;");
            }
            if (Options.GetUserTokenManager())
            {
                GenCodeLine("  /** User defined Token Manager. */");
                GenCodeLine("  " + StaticOpt() + "public TokenManager token_source;");
            }
            else
            {
                GenCodeLine("  /** Generated Token Manager. */");
                GenCodeLine("  " + StaticOpt() + "public " + CuName + "TokenManager token_source;");
                if (!Options.GetUserCharStream())
                {
                    if (Options.GetJavaUnicodeEscape())
                    {
                        GenCodeLine("  " + StaticOpt() + "JavaCharStream jj_input_stream;");
                    }
                    else
                    {
                        GenCodeLine("  " + StaticOpt() + "SimpleCharStream jj_input_stream;");
                    }
                }
            }
            GenCodeLine("  /** Current token. */");
            GenCodeLine("  " + StaticOpt() + "public Token token;");
            GenCodeLine("  /** Next token. */");
            GenCodeLine("  " + StaticOpt() + "public Token jj_nt;");
            if (!Options.GetCacheTokens())
            {
                GenCodeLine("  " + StaticOpt() + "private int jj_ntk;");
            }
            if (Options.GetDepthLimit() > 0)
            {
                GenCodeLine("  " + StaticOpt() + "private int jj_depth;");
            }
            if (jj2index != 0)
            {
                GenCodeLine("  " + StaticOpt() + "private Token jj_scanpos, jj_lastpos;");
                GenCodeLine("  " + StaticOpt() + "private int jj_la;");
                if (lookaheadNeeded)
                {
                    GenCodeLine("  /** Whether we are looking ahead. */");
                    GenCodeLine("  " + StaticOpt() + "private " + Options.GetBooleanType()
                            + " jj_lookingAhead = false;");
                    GenCodeLine("  " + StaticOpt() + "private " + Options.GetBooleanType()
                            + " jj_semLA;");
                }
            }
            if (Options.GetErrorReporting())
            {
                GenCodeLine("  " + StaticOpt() + "private int jj_gen;");
                GenCodeLine("  " + StaticOpt() + "private int[] jj_la1 = new int["
                        + maskindex + "];");
                int tokenMaskSize = (tokenCount - 1) / 32 + 1;
                for (int i = 0; i < tokenMaskSize; i++)
                {
                    GenCodeLine("  static private int[] jj_la1_" + i + ";");
                }
                GenCodeLine("  static {");
                for (int i = 0; i < tokenMaskSize; i++)
                {
                    GenCodeLine("	   jj_la1_init_" + i + "();");
                }
                GenCodeLine("	}");
                for (int i = 0; i < tokenMaskSize; i++)
                {
                    GenCodeLine("	private static void jj_la1_init_" + i + "() {");
                    GenCode("	   jj_la1_" + i + " = new int[] {");
                    foreach (var tokenMask in maskVals)
                    {
                        GenCode("0x" + Convert.ToString(tokenMask[i], 16) + ",");
                    }
                    GenCodeLine("};");
                    GenCodeLine("	}");
                }
            }
            if (jj2index != 0 && Options.GetErrorReporting())
            {
                GenCodeLine("  " + StaticOpt() + "final private JJCalls[] jj_2_rtns = new JJCalls["
                        + jj2index + "];");
                GenCodeLine("  " + StaticOpt() + "private " + Options.GetBooleanType()
                        + " jj_rescan = false;");
                GenCodeLine("  " + StaticOpt() + "private int jj_gc = 0;");
            }
            GenCodeLine("");

            if (Options.GetDebugParser())
            {
                GenCodeLine("  {");
                GenCodeLine("      enable_tracing();");
                GenCodeLine("  }");
            }

            if (!Options.GetUserTokenManager())
            {
                if (Options.GetUserCharStream())
                {
                    GenCodeLine("  /** Constructor with user supplied CharStream. */");
                    GenCodeLine("  public " + CuName + "(CharStream stream) {");
                    if (Options.GetStatic())
                    {
                        GenCodeLine("	 if (jj_initialized_once) {");
                        GenCodeLine("	   System.out.println(\"ERROR: Second call to constructor of static parser.  \");");
                        GenCodeLine("	   System.out.println(\"	   You must either use ReInit() "
                                + "or set the JavaCC option STATIC to false\");");
                        GenCodeLine("	   System.out.println(\"	   during parser generation.\");");
                        GenCodeLine("	   throw new " + (Options.IsLegacyExceptionHandling() ? "Error" : "RuntimeException") + "();");
                        GenCodeLine("	 }");
                        GenCodeLine("	 jj_initialized_once = true;");
                    }
                    if (Options.GetTokenManagerUsesParser())
                    {
                        GenCodeLine("	 token_source = new " + CuName
                                + "TokenManager(this, stream);");
                    }
                    else
                    {
                        GenCodeLine("	 token_source = new " + CuName + "TokenManager(stream);");
                    }
                    GenCodeLine("	 token = new Token();");
                    if (Options.GetCacheTokens())
                    {
                        GenCodeLine("	 token.next = jj_nt = token_source.getNextToken();");
                    }
                    else
                    {
                        GenCodeLine("	 jj_ntk = -1;");
                    }
                    if (Options.GetDepthLimit() > 0)
                    {
                        GenCodeLine("    jj_depth = -1;");
                    }
                    if (Options.GetErrorReporting())
                    {
                        GenCodeLine("	 jj_gen = 0;");
                        if (maskindex > 0)
                        {
                            GenCodeLine("	 for (int i = 0; i < " + maskindex
                                    + "; i++) jj_la1[i] = -1;");
                        }
                        if (jj2index != 0)
                        {
                            GenCodeLine("	 for (int i = 0; i < jj_2_rtns.length; i++) jj_2_rtns[i] = new JJCalls();");
                        }
                    }
                    GenCodeLine("  }");
                    GenCodeLine("");
                    GenCodeLine("  /** Reinitialise. */");
                    GenCodeLine("  " + StaticOpt() + "public void ReInit(CharStream stream) {");

                    if (Options.IsTokenManagerRequiresParserAccess())
                    {
                        GenCodeLine("	 token_source.ReInit(this,stream);");
                    }
                    else
                    {
                        GenCodeLine("	 token_source.ReInit(stream);");
                    }


                    GenCodeLine("	 token = new Token();");
                    if (Options.GetCacheTokens())
                    {
                        GenCodeLine("	 token.next = jj_nt = token_source.getNextToken();");
                    }
                    else
                    {
                        GenCodeLine("	 jj_ntk = -1;");
                    }
                    if (Options.GetDepthLimit() > 0)
                    {
                        GenCodeLine("    jj_depth = -1;");
                    }
                    if (lookaheadNeeded)
                    {
                        GenCodeLine("	 jj_lookingAhead = false;");
                    }
                    if (JjtreeGenerated)
                    {
                        GenCodeLine("	 jjtree.reset();");
                    }
                    if (Options.GetErrorReporting())
                    {
                        GenCodeLine("	 jj_gen = 0;");
                        if (maskindex > 0)
                        {
                            GenCodeLine("	 for (int i = 0; i < " + maskindex
                                    + "; i++) jj_la1[i] = -1;");
                        }
                        if (jj2index != 0)
                        {
                            GenCodeLine("	 for (int i = 0; i < jj_2_rtns.length; i++) jj_2_rtns[i] = new JJCalls();");
                        }
                    }
                    GenCodeLine("  }");
                }
                else
                {

                    if (!isJavaModernMode)
                    {
                        GenCodeLine("  /** Constructor with InputStream. */");
                        GenCodeLine("  public " + CuName + "(java.io.InputStream stream) {");
                        GenCodeLine("	  this(stream, null);");
                        GenCodeLine("  }");
                        GenCodeLine("  /** Constructor with InputStream and supplied encoding */");
                        GenCodeLine("  public " + CuName
                                + "(java.io.InputStream stream, String encoding) {");
                        if (Options.GetStatic())
                        {
                            GenCodeLine("	 if (jj_initialized_once) {");
                            GenCodeLine("	   System.out.println(\"ERROR: Second call to constructor of static parser.  \");");
                            GenCodeLine("	   System.out.println(\"	   You must either use ReInit() or "
                                    + "set the JavaCC option STATIC to false\");");
                            GenCodeLine("	   System.out.println(\"	   during parser generation.\");");
                            GenCodeLine("	   throw new " + (Options.IsLegacyExceptionHandling() ? "Error" : "RuntimeException") + "();");
                            GenCodeLine("	 }");
                            GenCodeLine("	 jj_initialized_once = true;");
                        }

                        if (Options.GetJavaUnicodeEscape())
                        {
                            if (!Options.GetGenerateChainedException())
                            {
                                GenCodeLine("	 try { jj_input_stream = new JavaCharStream(stream, encoding, 1, 1); } "
                                        + "catch(java.io.UnsupportedEncodingException e) {"
                                        + " throw new RuntimeException(e.Message); }");
                            }
                            else
                            {
                                GenCodeLine("	 try { jj_input_stream = new JavaCharStream(stream, encoding, 1, 1); } "
                                        + "catch(java.io.UnsupportedEncodingException e) { throw new RuntimeException(e); }");
                            }
                        }
                        else
                        {
                            if (!Options.GetGenerateChainedException())
                            {
                                GenCodeLine("	 try { jj_input_stream = new SimpleCharStream(stream, encoding, 1, 1); } "
                                        + "catch(java.io.UnsupportedEncodingException e) { "
                                        + "throw new RuntimeException(e.Message); }");
                            }
                            else
                            {
                                GenCodeLine("	 try { jj_input_stream = new SimpleCharStream(stream, encoding, 1, 1); } "
                                        + "catch(java.io.UnsupportedEncodingException e) { throw new RuntimeException(e); }");
                            }
                        }
                        if (Options.GetTokenManagerUsesParser() && !Options.GetStatic())
                        {
                            GenCodeLine("	 token_source = new " + CuName
                                    + "TokenManager(this, jj_input_stream);");
                        }
                        else
                        {
                            GenCodeLine("	 token_source = new " + CuName
                                    + "TokenManager(jj_input_stream);");
                        }
                        GenCodeLine("	 token = new Token();");
                        if (Options.GetCacheTokens())
                        {
                            GenCodeLine("	 token.next = jj_nt = token_source.getNextToken();");
                        }
                        else
                        {
                            GenCodeLine("	 jj_ntk = -1;");
                        }
                        if (Options.GetDepthLimit() > 0)
                        {
                            GenCodeLine("    jj_depth = -1;");
                        }
                        if (Options.GetErrorReporting())
                        {
                            GenCodeLine("	 jj_gen = 0;");
                            if (maskindex > 0)
                            {
                                GenCodeLine("	 for (int i = 0; i < " + maskindex
                                        + "; i++) jj_la1[i] = -1;");
                            }
                            if (jj2index != 0)
                            {
                                GenCodeLine("	 for (int i = 0; i < jj_2_rtns.length; i++) jj_2_rtns[i] = new JJCalls();");
                            }
                        }
                        GenCodeLine("  }");
                        GenCodeLine("");

                        GenCodeLine("  /** Reinitialise. */");
                        GenCodeLine("  " + StaticOpt()
                                + "public void ReInit(java.io.InputStream stream) {");
                        GenCodeLine("	  ReInit(stream, null);");
                        GenCodeLine("  }");

                        GenCodeLine("  /** Reinitialise. */");
                        GenCodeLine("  "
                                + StaticOpt()
                                + "public void ReInit(java.io.InputStream stream, String encoding) {");
                        if (!Options.GetGenerateChainedException())
                        {
                            GenCodeLine("	 try { jj_input_stream.ReInit(stream, encoding, 1, 1); } "
                                    + "catch(java.io.UnsupportedEncodingException e) { "
                                    + "throw new RuntimeException(e.Message); }");
                        }
                        else
                        {
                            GenCodeLine("	 try { jj_input_stream.ReInit(stream, encoding, 1, 1); } "
                                    + "catch(java.io.UnsupportedEncodingException e) { throw new RuntimeException(e); }");
                        }

                        if (Options.IsTokenManagerRequiresParserAccess())
                        {
                            GenCodeLine("	 token_source.ReInit(this,jj_input_stream);");
                        }
                        else
                        {
                            GenCodeLine("	 token_source.ReInit(jj_input_stream);");
                        }

                        GenCodeLine("	 token = new Token();");
                        if (Options.GetCacheTokens())
                        {
                            GenCodeLine("	 token.next = jj_nt = token_source.getNextToken();");
                        }
                        else
                        {
                            GenCodeLine("	 jj_ntk = -1;");
                        }
                        if (Options.GetDepthLimit() > 0)
                        {
                            GenCodeLine("    jj_depth = -1;");
                        }
                        if (JjtreeGenerated)
                        {
                            GenCodeLine("	 jjtree.reset();");
                        }
                        if (Options.GetErrorReporting())
                        {
                            GenCodeLine("	 jj_gen = 0;");
                            GenCodeLine("	 for (int i = 0; i < " + maskindex
                                    + "; i++) jj_la1[i] = -1;");
                            if (jj2index != 0)
                            {
                                GenCodeLine("	 for (int i = 0; i < jj_2_rtns.length; i++) jj_2_rtns[i] = new JJCalls();");
                            }
                        }
                        GenCodeLine("  }");
                        GenCodeLine("");

                    }

                    string readerInterfaceName = isJavaModernMode ? "Provider" : "java.io.Reader";
                    string stringReaderClass = isJavaModernMode ? "StringProvider"
                            : "java.io.StringReader";


                    GenCodeLine("  /** Constructor. */");
                    GenCodeLine("  public " + CuName + "(" + readerInterfaceName + " stream) {");
                    if (Options.GetStatic())
                    {
                        GenCodeLine("	 if (jj_initialized_once) {");
                        GenCodeLine("	   System.out.println(\"ERROR: Second call to constructor of static parser. \");");
                        GenCodeLine("	   System.out.println(\"	   You must either use ReInit() or "
                                + "set the JavaCC option STATIC to false\");");
                        GenCodeLine("	   System.out.println(\"	   during parser generation.\");");
                        GenCodeLine("	   throw new " + (Options.IsLegacyExceptionHandling() ? "Error" : "RuntimeException") + "();");
                        GenCodeLine("	 }");
                        GenCodeLine("	 jj_initialized_once = true;");
                    }
                    if (Options.GetJavaUnicodeEscape())
                    {
                        GenCodeLine("	 jj_input_stream = new JavaCharStream(stream, 1, 1);");
                    }
                    else
                    {
                        GenCodeLine("	 jj_input_stream = new SimpleCharStream(stream, 1, 1);");
                    }
                    if (Options.GetTokenManagerUsesParser() && !Options.GetStatic())
                    {
                        GenCodeLine("	 token_source = new " + CuName
                                + "TokenManager(this, jj_input_stream);");
                    }
                    else
                    {
                        GenCodeLine("	 token_source = new " + CuName
                                + "TokenManager(jj_input_stream);");
                    }
                    GenCodeLine("	 token = new Token();");
                    if (Options.GetCacheTokens())
                    {
                        GenCodeLine("	 token.next = jj_nt = token_source.getNextToken();");
                    }
                    else
                    {
                        GenCodeLine("	 jj_ntk = -1;");
                    }
                    if (Options.GetDepthLimit() > 0)
                    {
                        GenCodeLine("    jj_depth = -1;");
                    }
                    if (Options.GetErrorReporting())
                    {
                        GenCodeLine("	 jj_gen = 0;");
                        if (maskindex > 0)
                        {
                            GenCodeLine("	 for (int i = 0; i < " + maskindex
                                    + "; i++) jj_la1[i] = -1;");
                        }
                        if (jj2index != 0)
                        {
                            GenCodeLine("	 for (int i = 0; i < jj_2_rtns.length; i++) jj_2_rtns[i] = new JJCalls();");
                        }
                    }
                    GenCodeLine("  }");
                    GenCodeLine("");

                    // Add-in a string based constructor because its convenient (modern only to prevent regressions)
                    if (isJavaModernMode)
                    {
                        GenCodeLine("  /** Constructor. */");
                        GenCodeLine("  public " + CuName
                                + "(String dsl), " + Options.getTokenMgrErrorClass() + " {");
                        GenCodeLine("	   this(new " + stringReaderClass + "(dsl));");
                        GenCodeLine("  }");
                        GenCodeLine("");

                        GenCodeLine("  public void ReInit(String s) {");
                        GenCodeLine("	  ReInit(new " + stringReaderClass + "(s));");
                        GenCodeLine("  }");

                    }


                    GenCodeLine("  /** Reinitialise. */");
                    GenCodeLine("  " + StaticOpt() + "public void ReInit(" + readerInterfaceName
                            + " stream) {");
                    if (Options.GetJavaUnicodeEscape())
                    {
                        GenCodeLine("	if (jj_input_stream == null) {");
                        GenCodeLine("	   jj_input_stream = new JavaCharStream(stream, 1, 1);");
                        GenCodeLine("	} else {");
                        GenCodeLine("	   jj_input_stream.ReInit(stream, 1, 1);");
                        GenCodeLine("	}");
                    }
                    else
                    {
                        GenCodeLine("	if (jj_input_stream == null) {");
                        GenCodeLine("	   jj_input_stream = new SimpleCharStream(stream, 1, 1);");
                        GenCodeLine("	} else {");
                        GenCodeLine("	   jj_input_stream.ReInit(stream, 1, 1);");
                        GenCodeLine("	}");
                    }

                    GenCodeLine("	if (token_source == null) {");

                    if (Options.GetTokenManagerUsesParser() && !Options.GetStatic())
                    {
                        GenCodeLine(" token_source = new " + CuName + "TokenManager(this, jj_input_stream);");
                    }
                    else
                    {
                        GenCodeLine(" token_source = new " + CuName + "TokenManager(jj_input_stream);");
                    }

                    GenCodeLine("	}");
                    GenCodeLine("");

                    if (Options.IsTokenManagerRequiresParserAccess())
                    {
                        GenCodeLine("	 token_source.ReInit(this,jj_input_stream);");
                    }
                    else
                    {
                        GenCodeLine("	 token_source.ReInit(jj_input_stream);");
                    }

                    GenCodeLine("	 token = new Token();");
                    if (Options.GetCacheTokens())
                    {
                        GenCodeLine("	 token.next = jj_nt = token_source.getNextToken();");
                    }
                    else
                    {
                        GenCodeLine("	 jj_ntk = -1;");
                    }
                    if (Options.GetDepthLimit() > 0)
                    {
                        GenCodeLine("    jj_depth = -1;");
                    }
                    if (JjtreeGenerated)
                    {
                        GenCodeLine("	 jjtree.reset();");
                    }
                    if (Options.GetErrorReporting())
                    {
                        GenCodeLine("	 jj_gen = 0;");
                        if (maskindex > 0)
                        {
                            GenCodeLine("	 for (int i = 0; i < " + maskindex
                                    + "; i++) jj_la1[i] = -1;");
                        }
                        if (jj2index != 0)
                        {
                            GenCodeLine("	 for (int i = 0; i < jj_2_rtns.length; i++) jj_2_rtns[i] = new JJCalls();");
                        }
                    }
                    GenCodeLine("  }");

                }
            }
            GenCodeLine("");
            if (Options.GetUserTokenManager())
            {
                GenCodeLine("  /** Constructor with user supplied Token Manager. */");
                GenCodeLine("  public " + CuName + "(TokenManager tm) {");
            }
            else
            {
                GenCodeLine("  /** Constructor with generated Token Manager. */");
                GenCodeLine("  public " + CuName + "(" + CuName + "TokenManager tm) {");
            }
            if (Options.GetStatic())
            {
                GenCodeLine("	 if (jj_initialized_once) {");
                GenCodeLine("	   System.out.println(\"ERROR: Second call to constructor of static parser. \");");
                GenCodeLine("	   System.out.println(\"	   You must either use ReInit() or "
                        + "set the JavaCC option STATIC to false\");");
                GenCodeLine("	   System.out.println(\"	   during parser generation.\");");
                GenCodeLine("	   throw new " + (Options.IsLegacyExceptionHandling() ? "Error" : "RuntimeException") + "();");
                GenCodeLine("	 }");
                GenCodeLine("	 jj_initialized_once = true;");
            }
            GenCodeLine("	 token_source = tm;");
            GenCodeLine("	 token = new Token();");
            if (Options.GetCacheTokens())
            {
                GenCodeLine("	 token.next = jj_nt = token_source.getNextToken();");
            }
            else
            {
                GenCodeLine("	 jj_ntk = -1;");
            }
            if (Options.GetDepthLimit() > 0)
            {
                GenCodeLine("    jj_depth = -1;");
            }
            if (Options.GetErrorReporting())
            {
                GenCodeLine("	 jj_gen = 0;");
                if (maskindex > 0)
                {
                    GenCodeLine("	 for (int i = 0; i < " + maskindex + "; i++) jj_la1[i] = -1;");
                }
                if (jj2index != 0)
                {
                    GenCodeLine("	 for (int i = 0; i < jj_2_rtns.length; i++) jj_2_rtns[i] = new JJCalls();");
                }
            }
            GenCodeLine("  }");
            GenCodeLine("");
            if (Options.GetUserTokenManager())
            {
                GenCodeLine("  /** Reinitialise. */");
                GenCodeLine("  public void ReInit(TokenManager tm) {");
            }
            else
            {
                GenCodeLine("  /** Reinitialise. */");
                GenCodeLine("  public void ReInit(" + CuName + "TokenManager tm) {");
            }
            GenCodeLine("	 token_source = tm;");
            GenCodeLine("	 token = new Token();");
            if (Options.GetCacheTokens())
            {
                GenCodeLine("	 token.next = jj_nt = token_source.getNextToken();");
            }
            else
            {
                GenCodeLine("	 jj_ntk = -1;");
            }
            if (Options.GetDepthLimit() > 0)
            {
                GenCodeLine("    jj_depth = -1;");
            }
            if (JjtreeGenerated)
            {
                GenCodeLine("	 jjtree.reset();");
            }
            if (Options.GetErrorReporting())
            {
                GenCodeLine("	 jj_gen = 0;");
                if (maskindex > 0)
                {
                    GenCodeLine("	 for (int i = 0; i < " + maskindex + "; i++) jj_la1[i] = -1;");
                }
                if (jj2index != 0)
                {
                    GenCodeLine("	 for (int i = 0; i < jj_2_rtns.length; i++) jj_2_rtns[i] = new JJCalls();");
                }
            }
            GenCodeLine("  }");
            GenCodeLine("");
            GenCodeLine("  " + StaticOpt()
                    + "private Token jj_consume_token(int kind) throws ParseException {");
            if (Options.GetCacheTokens())
            {
                GenCodeLine("	 Token oldToken = token;");
                GenCodeLine("	 if ((token = jj_nt).next != null) jj_nt = jj_nt.next;");
                GenCodeLine("	 else jj_nt = jj_nt.next = token_source.getNextToken();");
            }
            else
            {
                GenCodeLine("	 Token oldToken;");
                GenCodeLine("	 if ((oldToken = token).next != null) token = token.next;");
                GenCodeLine("	 else token = token.next = token_source.getNextToken();");
                GenCodeLine("	 jj_ntk = -1;");
            }
            GenCodeLine("	 if (token.kind == kind) {");
            if (Options.GetErrorReporting())
            {
                GenCodeLine("	   jj_gen++;");
                if (jj2index != 0)
                {
                    GenCodeLine("	   if (++jj_gc > 100) {");
                    GenCodeLine("		 jj_gc = 0;");
                    GenCodeLine("		 for (int i = 0; i < jj_2_rtns.length; i++) {");
                    GenCodeLine("		   JJCalls c = jj_2_rtns[i];");
                    GenCodeLine("		   while (c != null) {");
                    GenCodeLine("			 if (c.gen < jj_gen) c.first = null;");
                    GenCodeLine("			 c = c.next;");
                    GenCodeLine("		   }");
                    GenCodeLine("		 }");
                    GenCodeLine("	   }");
                }
            }
            if (Options.GetDebugParser())
            {
                GenCodeLine("	   trace_token(token, \"\");");
            }
            GenCodeLine("	   return token;");
            GenCodeLine("	 }");
            if (Options.GetCacheTokens())
            {
                GenCodeLine("	 jj_nt = token;");
            }
            GenCodeLine("	 token = oldToken;");
            if (Options.GetErrorReporting())
            {
                GenCodeLine("	 jj_kind = kind;");
            }
            GenCodeLine("	 throw generateParseException();");
            GenCodeLine("  }");
            GenCodeLine("");
            if (jj2index != 0)
            {
                GenCodeLine("  @SuppressWarnings(\"serial\")");
                GenCodeLine("  static private final class LookaheadSuccess extends " + (Options.IsLegacyExceptionHandling() ? "java.lang.Error" : "java.lang.RuntimeException") + " {");
                GenCodeLine("    @Override");
                GenCodeLine("    public Throwable fillInStackTrace() {");
                GenCodeLine("      return this;");
                GenCodeLine("    }");
                GenCodeLine("  }");
                GenCodeLine("  static private final LookaheadSuccess jj_ls = new LookaheadSuccess();");
                GenCodeLine("  " + StaticOpt() + "private " + Options.GetBooleanType()
                        + " jj_scan_token(int kind) {");
                GenCodeLine("	 if (jj_scanpos == jj_lastpos) {");
                GenCodeLine("	   jj_la--;");
                GenCodeLine("	   if (jj_scanpos.next == null) {");
                GenCodeLine("		 jj_lastpos = jj_scanpos = jj_scanpos.next = token_source.getNextToken();");
                GenCodeLine("	   } else {");
                GenCodeLine("		 jj_lastpos = jj_scanpos = jj_scanpos.next;");
                GenCodeLine("	   }");
                GenCodeLine("	 } else {");
                GenCodeLine("	   jj_scanpos = jj_scanpos.next;");
                GenCodeLine("	 }");
                if (Options.GetErrorReporting())
                {
                    GenCodeLine("	 if (jj_rescan) {");
                    GenCodeLine("	   int i = 0; Token tok = token;");
                    GenCodeLine("	   while (tok != null && tok != jj_scanpos) { i++; tok = tok.next; }");
                    GenCodeLine("	   if (tok != null) jj_add_error_token(kind, i);");
                    if (Options.GetDebugLookahead())
                    {
                        GenCodeLine("	 } else {");
                        GenCodeLine("	   trace_scan(jj_scanpos, kind);");
                    }
                    GenCodeLine("	 }");
                }
                else if (Options.GetDebugLookahead())
                {
                    GenCodeLine("	 trace_scan(jj_scanpos, kind);");
                }
                GenCodeLine("	 if (jj_scanpos.kind != kind) return true;");
                GenCodeLine("	 if (jj_la == 0 && jj_scanpos == jj_lastpos) throw jj_ls;");
                GenCodeLine("	 return false;");
                GenCodeLine("  }");
                GenCodeLine("");
            }
            GenCodeLine("");
            GenCodeLine("/** Get the next Token. */");
            GenCodeLine("  " + StaticOpt() + "final public Token getNextToken() {");
            if (Options.GetCacheTokens())
            {
                GenCodeLine("	 if ((token = jj_nt).next != null) jj_nt = jj_nt.next;");
                GenCodeLine("	 else jj_nt = jj_nt.next = token_source.getNextToken();");
            }
            else
            {
                GenCodeLine("	 if (token.next != null) token = token.next;");
                GenCodeLine("	 else token = token.next = token_source.getNextToken();");
                GenCodeLine("	 jj_ntk = -1;");
            }
            if (Options.GetErrorReporting())
            {
                GenCodeLine("	 jj_gen++;");
            }
            if (Options.GetDebugParser())
            {
                GenCodeLine("	   trace_token(token, \" (in getNextToken)\");");
            }
            GenCodeLine("	 return token;");
            GenCodeLine("  }");
            GenCodeLine("");
            GenCodeLine("/** Get the specific Token. */");
            GenCodeLine("  " + StaticOpt() + "final public Token getToken(int index) {");
            if (lookaheadNeeded)
            {
                GenCodeLine("	 Token t = jj_lookingAhead ? jj_scanpos : token;");
            }
            else
            {
                GenCodeLine("	 Token t = token;");
            }
            GenCodeLine("	 for (int i = 0; i < index; i++) {");
            GenCodeLine("	   if (t.next != null) t = t.next;");
            GenCodeLine("	   else t = t.next = token_source.getNextToken();");
            GenCodeLine("	 }");
            GenCodeLine("	 return t;");
            GenCodeLine("  }");
            GenCodeLine("");
            if (!Options.GetCacheTokens())
            {
                GenCodeLine("  " + StaticOpt() + "private int jj_ntk_f() {");
                GenCodeLine("	 if ((jj_nt=token.next) == null)");
                GenCodeLine("	   return (jj_ntk = (token.next=token_source.getNextToken()).kind);");
                GenCodeLine("	 else");
                GenCodeLine("	   return (jj_ntk = jj_nt.kind);");
                GenCodeLine("  }");
                GenCodeLine("");
            }
            if (Options.GetErrorReporting())
            {
                if (!Options.GetGenerateGenerics())
                {
                    GenCodeLine("  " + StaticOpt()
                            + "private java.util.List jj_expentries = new java.util.ArrayList();");
                }
                else
                {
                    GenCodeLine("  "
                            + StaticOpt()
                            + "private java.util.List<int[]> jj_expentries = new java.util.ArrayList<int[]>();");
                }
                GenCodeLine("  " + StaticOpt() + "private int[] jj_expentry;");
                GenCodeLine("  " + StaticOpt() + "private int jj_kind = -1;");
                if (jj2index != 0)
                {
                    GenCodeLine("  " + StaticOpt() + "private int[] jj_lasttokens = new int[100];");
                    GenCodeLine("  " + StaticOpt() + "private int jj_endpos;");
                    GenCodeLine("");
                    GenCodeLine("  " + StaticOpt()
                            + "private void jj_add_error_token(int kind, int pos) {");
                    GenCodeLine("	 if (pos >= 100) {");
                    GenCodeLine("		return;");
                    GenCodeLine("	 }");
                    GenCodeLine("");
                    GenCodeLine("	 if (pos == jj_endpos + 1) {");
                    GenCodeLine("	   jj_lasttokens[jj_endpos++] = kind;");
                    GenCodeLine("	 } else if (jj_endpos != 0) {");
                    GenCodeLine("	   jj_expentry = new int[jj_endpos];");
                    GenCodeLine("");
                    GenCodeLine("	   for (int i = 0; i < jj_endpos; i++) {");
                    GenCodeLine("		 jj_expentry[i] = jj_lasttokens[i];");
                    GenCodeLine("	   }");
                    GenCodeLine("");
                    if (!Options.GetGenerateGenerics())
                    {
                        GenCodeLine("	   for (java.util.Iterator it = jj_expentries.iterator(); it.hasNext();) {");
                        GenCodeLine("		 int[] oldentry = (int[])(it.next());");
                    }
                    else
                    {
                        GenCodeLine("	   for (int[] oldentry : jj_expentries) {");
                    }

                    GenCodeLine("		 if (oldentry.length == jj_expentry.length) {");
                    GenCodeLine("		   boolean isMatched = true;");
                    GenCodeLine("");
                    GenCodeLine("		   for (int i = 0; i < jj_expentry.length; i++) {");
                    GenCodeLine("			 if (oldentry[i] != jj_expentry[i]) {");
                    GenCodeLine("			   isMatched = false;");
                    GenCodeLine("			   break;");
                    GenCodeLine("			 }");
                    GenCodeLine("");
                    GenCodeLine("		   }");
                    GenCodeLine("		   if (isMatched) {");
                    GenCodeLine("			 jj_expentries.add(jj_expentry);");
                    GenCodeLine("			 break;");
                    GenCodeLine("		   }");
                    GenCodeLine("		 }");
                    GenCodeLine("	   }");
                    GenCodeLine("");
                    GenCodeLine("	   if (pos != 0) {");
                    GenCodeLine("		 jj_lasttokens[(jj_endpos = pos) - 1] = kind;");
                    GenCodeLine("	   }");
                    GenCodeLine("	 }");
                    GenCodeLine("  }");
                }
                GenCodeLine("");
                GenCodeLine("  /** Generate ParseException. */");
                GenCodeLine("  " + StaticOpt() + "public ParseException generateParseException() {");
                GenCodeLine("	 jj_expentries.clear();");
                GenCodeLine("	 " + Options.GetBooleanType() + "[] la1tokens = new "
                        + Options.GetBooleanType() + "[" + tokenCount + "];");
                GenCodeLine("	 if (jj_kind >= 0) {");
                GenCodeLine("	   la1tokens[jj_kind] = true;");
                GenCodeLine("	   jj_kind = -1;");
                GenCodeLine("	 }");
                GenCodeLine("	 for (int i = 0; i < " + maskindex + "; i++) {");
                GenCodeLine("	   if (jj_la1[i] == jj_gen) {");
                GenCodeLine("		 for (int j = 0; j < 32; j++) {");
                for (int i = 0; i < (tokenCount - 1) / 32 + 1; i++)
                {
                    GenCodeLine("		   if ((jj_la1_" + i + "[i] & (1<<j)) != 0) {");
                    GenCode("			 la1tokens[");
                    if (i != 0)
                    {
                        GenCode((32 * i) + "+");
                    }
                    GenCodeLine("j] = true;");
                    GenCodeLine("		   }");
                }
                GenCodeLine("		 }");
                GenCodeLine("	   }");
                GenCodeLine("	 }");
                GenCodeLine("	 for (int i = 0; i < " + tokenCount + "; i++) {");
                GenCodeLine("	   if (la1tokens[i]) {");
                GenCodeLine("		 jj_expentry = new int[1];");
                GenCodeLine("		 jj_expentry[0] = i;");
                GenCodeLine("		 jj_expentries.add(jj_expentry);");
                GenCodeLine("	   }");
                GenCodeLine("	 }");
                if (jj2index != 0)
                {
                    GenCodeLine("	 jj_endpos = 0;");
                    GenCodeLine("	 jj_rescan_token();");
                    GenCodeLine("	 jj_add_error_token(0, 0);");
                }
                GenCodeLine("	 int[][] exptokseq = new int[jj_expentries.size()][];");
                GenCodeLine("	 for (int i = 0; i < jj_expentries.size(); i++) {");
                if (!Options.GetGenerateGenerics())
                {
                    GenCodeLine("	   exptokseq[i] = (int[])jj_expentries.get(i);");
                }
                else
                {
                    GenCodeLine("	   exptokseq[i] = jj_expentries.get(i);");
                }
                GenCodeLine("	 }");


                if (isJavaModernMode)
                {
                    // Add the lexical state onto the exception message
                    GenCodeLine("	 return new ParseException(token, exptokseq, tokenImage, token_source == null ? null : " + CuName + "TokenManager.lexStateNames[token_source.curLexState]);");
                }
                else
                {
                    GenCodeLine("	 return new ParseException(token, exptokseq, tokenImage);");
                }

                GenCodeLine("  }");
            }
            else
            {
                GenCodeLine("  /** Generate ParseException. */");
                GenCodeLine("  " + StaticOpt() + "public ParseException generateParseException() {");
                GenCodeLine("	 Token errortok = token.next;");
                if (Options.GetKeepLineColumn())
                {
                    GenCodeLine("	 int line = errortok.beginLine, column = errortok.beginColumn;");
                }
                GenCodeLine("	 String mess = (errortok.kind == 0) ? tokenImage[0] : errortok.image;");
                if (Options.GetKeepLineColumn())
                {
                    GenCodeLine("	 return new ParseException("
                            + "\"Parse error at line \" + line + \", column \" + column + \".  "
                            + "Encountered: \" + mess);");
                }
                else
                {
                    GenCodeLine("	 return new ParseException(\"Parse error at <unknown location>.  "
                            + "Encountered: \" + mess);");
                }
                GenCodeLine("  }");
            }
            GenCodeLine("");

            GenCodeLine("  " + StaticOpt() + "private " + Options.GetBooleanType()
                    + " trace_enabled;");
            GenCodeLine("");
            GenCodeLine("/** Trace enabled. */");
            GenCodeLine("  " + StaticOpt() + "final public boolean trace_enabled() {");
            GenCodeLine("	 return trace_enabled;");
            GenCodeLine("  }");
            GenCodeLine("");

            if (Options.GetDebugParser())
            {
                GenCodeLine("  " + StaticOpt() + "private int trace_indent = 0;");

                GenCodeLine("/** Enable tracing. */");
                GenCodeLine("  " + StaticOpt() + "final public void enable_tracing() {");
                GenCodeLine("	 trace_enabled = true;");
                GenCodeLine("  }");
                GenCodeLine("");
                GenCodeLine("/** Disable tracing. */");
                GenCodeLine("  " + StaticOpt() + "final public void disable_tracing() {");
                GenCodeLine("	 trace_enabled = false;");
                GenCodeLine("  }");
                GenCodeLine("");
                GenCodeLine("  " + StaticOpt() + "protected void trace_call(String s) {");
                GenCodeLine("	 if (trace_enabled) {");
                GenCodeLine("	   for (int i = 0; i < trace_indent; i++) { System.out.print(\" \"); }");
                GenCodeLine("	   System.out.println(\"Call:	\" + s);");
                GenCodeLine("	 }");
                GenCodeLine("	 trace_indent = trace_indent + 2;");
                GenCodeLine("  }");
                GenCodeLine("");
                GenCodeLine("  " + StaticOpt() + "protected void trace_return(String s) {");
                GenCodeLine("	 trace_indent = trace_indent - 2;");
                GenCodeLine("	 if (trace_enabled) {");
                GenCodeLine("	   for (int i = 0; i < trace_indent; i++) { System.out.print(\" \"); }");
                GenCodeLine("	   System.out.println(\"Return: \" + s);");
                GenCodeLine("	 }");
                GenCodeLine("  }");
                GenCodeLine("");
                GenCodeLine("  " + StaticOpt()
                        + "protected void trace_token(Token t, String where) {");
                GenCodeLine("	 if (trace_enabled) {");
                GenCodeLine("	   for (int i = 0; i < trace_indent; i++) { System.out.print(\" \"); }");
                GenCodeLine("	   System.out.print(\"Consumed token: <\" + tokenImage[t.kind]);");
                GenCodeLine("	   if (t.kind != 0 && !tokenImage[t.kind].equals(\"\\\"\" + t.image + \"\\\"\")) {");
                GenCodeLine("		 System.out.print(\": \\\"\" + " + Options.getTokenMgrErrorClass() + ".addEscapes(" + "t.image) + \"\\\"\");");
                GenCodeLine("	   }");
                GenCodeLine("	   System.out.println(\" at line \" + t.beginLine + "
                        + "\" column \" + t.beginColumn + \">\" + where);");
                GenCodeLine("	 }");
                GenCodeLine("  }");
                GenCodeLine("");
                GenCodeLine("  " + StaticOpt() + "protected void trace_scan(Token t1, int t2) {");
                GenCodeLine("	 if (trace_enabled) {");
                GenCodeLine("	   for (int i = 0; i < trace_indent; i++) { System.out.print(\" \"); }");
                GenCodeLine("	   System.out.print(\"Visited token: <\" + tokenImage[t1.kind]);");
                GenCodeLine("	   if (t1.kind != 0 && !tokenImage[t1.kind].equals(\"\\\"\" + t1.image + \"\\\"\")) {");
                GenCodeLine("		 System.out.print(\": \\\"\" + " + Options.getTokenMgrErrorClass() + ".addEscapes(" + "t1.image) + \"\\\"\");");
                GenCodeLine("	   }");
                GenCodeLine("	   System.out.println(\" at line \" + t1.beginLine + \""
                        + " column \" + t1.beginColumn + \">; Expected token: <\" + tokenImage[t2] + \">\");");
                GenCodeLine("	 }");
                GenCodeLine("  }");
                GenCodeLine("");
            }
            else
            {
                GenCodeLine("  /** Enable tracing. */");
                GenCodeLine("  " + StaticOpt() + "final public void enable_tracing() {");
                GenCodeLine("  }");
                GenCodeLine("");
                GenCodeLine("  /** Disable tracing. */");
                GenCodeLine("  " + StaticOpt() + "final public void disable_tracing() {");
                GenCodeLine("  }");
                GenCodeLine("");
            }

            if (jj2index != 0 && Options.GetErrorReporting())
            {
                GenCodeLine("  " + StaticOpt() + "private void jj_rescan_token() {");
                GenCodeLine("	 jj_rescan = true;");
                GenCodeLine("	 for (int i = 0; i < " + jj2index + "; i++) {");
                GenCodeLine("	   try {");
                GenCodeLine("		 JJCalls p = jj_2_rtns[i];");
                GenCodeLine("");
                GenCodeLine("		 do {");
                GenCodeLine("		   if (p.gen > jj_gen) {");
                GenCodeLine("			 jj_la = p.arg; jj_lastpos = jj_scanpos = p.first;");
                GenCodeLine("			 switch (i) {");
                for (int i = 0; i < jj2index; i++)
                {
                    GenCodeLine("			   case " + i + ": jj_3_" + (i + 1) + "(); break;");
                }
                GenCodeLine("			 }");
                GenCodeLine("		   }");
                GenCodeLine("		   p = p.next;");
                GenCodeLine("		 } while (p != null);");
                GenCodeLine("");
                GenCodeLine("		 } catch(LookaheadSuccess ls) { }");
                GenCodeLine("	 }");
                GenCodeLine("	 jj_rescan = false;");
                GenCodeLine("  }");
                GenCodeLine("");
                GenCodeLine("  " + StaticOpt() + "private void jj_save(int index, int xla) {");
                GenCodeLine("	 JJCalls p = jj_2_rtns[index];");
                GenCodeLine("	 while (p.gen > jj_gen) {");
                GenCodeLine("	   if (p.next == null) { p = p.next = new JJCalls(); break; }");
                GenCodeLine("	   p = p.next;");
                GenCodeLine("	 }");
                GenCodeLine("");
                GenCodeLine("	 p.gen = jj_gen + xla - jj_la; ");
                GenCodeLine("	 p.first = token;");
                GenCodeLine("	 p.arg = xla;");
                GenCodeLine("  }");
                GenCodeLine("");
            }

            if (jj2index != 0 && Options.GetErrorReporting())
            {
                GenCodeLine("  static final class JJCalls {");
                GenCodeLine("	 int gen;");
                GenCodeLine("	 Token first;");
                GenCodeLine("	 int arg;");
                GenCodeLine("	 JJCalls next;");
                GenCodeLine("  }");
                GenCodeLine("");
            }

            if (cu_from_insertion_point_2.Count != 0)
            {
                CSharpCCGlobals.PrintTokenSetup((cu_from_insertion_point_2[0]));
                this.ccol = 1;
                foreach (var t2 in cu_from_insertion_point_2)
                {
                    PrintToken(t = t2);
                }
                PrintTrailingComments(t);
            }
            GenCodeLine("");

            SaveOutput(Options.GetOutputDirectory() + Path.DirectorySeparatorChar + CuName
                    + GetFileExtension(Options.GetOutputLanguage()));

        } // matches "if (Options.getBuildParser())"

    }

    public static void reInit()
    {
        lookaheadNeeded = false;
    }

}
