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
namespace CSharpCC.CCTree;


public class CCTreeGlobals
{
    public static void Initialize()
    {
        ToolList = new();
        ParserName = null;
        PackageName = "";
        ParserImplements = null;
        ParserClassBodyStart = null;
        ParserImports = null;
        Productions = new ();

        cctreeOptions = new()
        {
            "JJTREE_OUTPUT_DIRECTORY",
            "MULTI",
            "NODE_PREFIX",
            "NODE_PACKAGE",
            "NODE_EXTENDS",
            "NODE_CLASS",
            "NODE_STACK_SIZE",
            "NODE_DEFAULT_VOID",
            "OUTPUT_FILE",
            "CHECK_DEFINITE_NODE",
            "NODE_SCOPE_HOOK",
            "TRACK_TOKENS",
            "NODE_FACTORY",
            "NODE_USES_PARSER",
            "BUILD_NODE_FILES",
            "VISITOR",
            "VISITOR_EXCEPTION",
            "VISITOR_DATA_TYPE",
            "VISITOR_RETURN_TYPE",
            "VISITOR_METHOD_NAME_INCLUDES_TYPE_NAME",
            "NODE_INCLUDES"
        };
    }

    static CCTreeGlobals()
    {
        Initialize();
    }

    /**
     * This set stores the JJTree-specific options that should not be
     * passed down to JavaCC
     */
    private static HashSet<string> cctreeOptions;

    public static bool IsOptionJJTreeOnly(string optionName)
    {
        return cctreeOptions.Contains(optionName.ToUpper());
    }

    public static List<string> ToolList = new();

    /**
     * Use this like className.
     **/
    public static string ParserName;

    /**
     * The package that the parser lives in.  If the grammar doesn't
     * specify a package it is the empty string.
     **/
    public static string PackageName = "";

    /**
     * The package the node files live in. If the NODE_PACKAGE option
     * is not set, then this defaults to packageName.
     **/
    public static string NodePackageName = "";

    /** The <code>implements</code> token of the parser class.  If the
     * parser doesn't have one then it is the first "{" of the parser
     * class body.
     **/
    public static Token ParserImplements;

    /** The first token of the parser class body (the <code>{</code>).
     * The JJTree state is inserted after this token.
     **/
    public static Token ParserClassBodyStart;

    /** The first token of the <code>import</code> list, or the
     * position where such a list should be inserted. The import
     * for the Node Package is inserted after this token.
     **/
    public static Token ParserImports;

    /**
     * This is mapping from production names to ASTProduction objects.
     **/
    public static Dictionary<string,ASTProduction> Productions = new ();

}

/*end*/
