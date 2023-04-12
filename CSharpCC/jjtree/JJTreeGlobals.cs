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
namespace org.javacc.jjtree;


public class JJTreeGlobals
{
    public static void initialize()
    {
        toolList = new();
        parserName = null;
        packageName = "";
        parserImplements = null;
        parserClassBodyStart = null;
        parserImports = null;
        productions = new Dictionary();

        jjtreeOptions = new HashSet();
        jjtreeOptions.Add("JJTREE_OUTPUT_DIRECTORY");
        jjtreeOptions.Add("MULTI");
        jjtreeOptions.Add("NODE_PREFIX");
        jjtreeOptions.Add("NODE_PACKAGE");
        jjtreeOptions.Add("NODE_EXTENDS");
        jjtreeOptions.Add("NODE_CLASS");
        jjtreeOptions.Add("NODE_STACK_SIZE");
        jjtreeOptions.Add("NODE_DEFAULT_VOID");
        jjtreeOptions.Add("OUTPUT_FILE");
        jjtreeOptions.Add("CHECK_DEFINITE_NODE");
        jjtreeOptions.Add("NODE_SCOPE_HOOK");
        jjtreeOptions.Add("TRACK_TOKENS");
        jjtreeOptions.Add("NODE_FACTORY");
        jjtreeOptions.Add("NODE_USES_PARSER");
        jjtreeOptions.Add("BUILD_NODE_FILES");
        jjtreeOptions.Add("VISITOR");
        jjtreeOptions.Add("VISITOR_EXCEPTION");
        jjtreeOptions.Add("VISITOR_DATA_TYPE");
        jjtreeOptions.Add("VISITOR_RETURN_TYPE");
        jjtreeOptions.Add("VISITOR_METHOD_NAME_INCLUDES_TYPE_NAME");
        jjtreeOptions.Add("NODE_INCLUDES");
    }

    static JJTreeGlobals()
    {
        initialize();
    }

    /**
     * This set stores the JJTree-specific options that should not be
     * passed down to JavaCC
     */
    private static HashSet<string> jjtreeOptions;

    public static bool isOptionJJTreeOnly(string optionName)
    {
        return jjtreeOptions.Contains(optionName.ToUpper());
    }

    public static List<string> toolList = new ();

    /**
     * Use this like className.
     **/
    public static string parserName;

    /**
     * The package that the parser lives in.  If the grammar doesn't
     * specify a package it is the empty string.
     **/
    public static string packageName = "";

    /**
     * The package the node files live in. If the NODE_PACKAGE option
     * is not set, then this defaults to packageName.
     **/
    public static string nodePackageName = "";

    /** The <code>implements</code> token of the parser class.  If the
     * parser doesn't have one then it is the first "{" of the parser
     * class body.
     **/
    public static Token parserImplements;

    /** The first token of the parser class body (the <code>{</code>).
     * The JJTree state is inserted after this token.
     **/
    public static Token parserClassBodyStart;

    /** The first token of the <code>import</code> list, or the
     * position where such a list should be inserted. The import
     * for the Node Package is inserted after this token.
     **/
    public static Token parserImports;

    /**
     * This is mapping from production names to ASTProduction objects.
     **/
    public static Dictionary productions = new Dictionary();

}

/*end*/
