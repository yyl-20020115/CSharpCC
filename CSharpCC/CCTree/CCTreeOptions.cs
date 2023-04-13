// Copyright 2011 Google Inc. All Rights Reserved.
// Author: sreeni@google.com (Sreeni Viswanadha)

/* Copyright (c) 2005-2006, Kees Jan Koster kjkoster@kjkoster.org
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


/**
 * The JJTree-specific options.
 *
 * @author Kees Jan Koster &lt;kjkoster@kjkoster.org&gt;
 */
public class CCTreeOptions : Options
{

    /**
     * Limit subclassing to derived classes.
     */
    protected CCTreeOptions() : base() { }

    /**
     * Initialize the JJTree-specific options.
     */
    public new static void Init()
    {
        Options.Init();
        Options.OptionValues.Add("MULTI", false);
        Options.OptionValues.Add("NODE_DEFAULT_VOID", false);
        Options.OptionValues.Add("NODE_SCOPE_HOOK", false);
        Options.OptionValues.Add("NODE_USES_PARSER", false);
        Options.OptionValues.Add("BUILD_NODE_FILES", true);
        Options.OptionValues.Add("VISITOR", false);
        Options.OptionValues.Add("VISITOR_METHOD_NAME_INCLUDES_TYPE_NAME", false);
        Options.OptionValues.Add("TRACK_TOKENS", false);

        Options.OptionValues.Add("NODE_PREFIX", "AST");
        Options.OptionValues.Add("NODE_PACKAGE", "");
        Options.OptionValues.Add("NODE_EXTENDS", "");
        Options.OptionValues.Add("NODE_CLASS", "");
        Options.OptionValues.Add("NODE_FACTORY", "");
        Options.OptionValues.Add("NODE_INCLUDES", "");
        Options.OptionValues.Add("OUTPUT_FILE", "");
        Options.OptionValues.Add("VISITOR_DATA_TYPE", "");
        Options.OptionValues.Add("VISITOR_RETURN_TYPE", "Object");
        Options.OptionValues.Add("VISITOR_EXCEPTION", "");

        Options.OptionValues.Add("JJTREE_OUTPUT_DIRECTORY", "");


        // TODO :: 2013/07/23 -- This appears to be a duplicate from the parent class
        Options.OptionValues.Add(Options.USEROPTION__JDK_VERSION, "1.5");

        // Also appears to be a duplicate
        Options.OptionValues.Add(Options.USEROPTION__CPP_NAMESPACE, "");

        // Also appears to be a duplicate
        Options.OptionValues.Add(Options.USEROPTION__CPP_IGNORE_ACTIONS, false);
    }

    /**
     *  Check options for consistency
     */
    public static void Validate()
    {
        if (!GetVisitor())
        {
            if (GetVisitorDataType().Length > 0)
            {
                CSharpCCErrors.Warning("VISITOR_DATA_TYPE option will be ignored since VISITOR is false");
            }
            if (GetVisitorReturnType().Length > 0 && GetVisitorReturnType() != ("Object"))
            {
                CSharpCCErrors.Warning("VISITOR_RETURN_TYPE option will be ignored since VISITOR is false");
            }
            if (GetVisitorException().Length > 0)
            {
                CSharpCCErrors.Warning("VISITOR_EXCEPTION option will be ignored since VISITOR is false");
            }
        }
    }


    /**
     * Find the multi value.
     *
     * @return The requested multi value.
     */
    public static bool GetMulti()
    {
        return BooleanValue("MULTI");
    }

    /**
     * Find the node default void value.
     *
     * @return The requested node default void value.
     */
    public static bool GetNodeDefaultVoid()
    {
        return BooleanValue("NODE_DEFAULT_VOID");
    }

    /**
     * Find the node scope hook value.
     *
     * @return The requested node scope hook value.
     */
    public static bool GetNodeScopeHook()
    {
        return BooleanValue("NODE_SCOPE_HOOK");
    }

    /**
     * Find the node factory value.
     *
     * @return The requested node factory value.
     */
    public static string GetNodeFactory()
    {
        return StringValue("NODE_FACTORY");
    }

    /**
     * Find the node uses parser value.
     *
     * @return The requested node uses parser value.
     */
    public static bool GetNodeUsesParser()
    {
        return BooleanValue("NODE_USES_PARSER");
    }

    /**
     * Find the build node files value.
     *
     * @return The requested build node files value.
     */
    public static bool GetBuildNodeFiles()
    {
        return BooleanValue("BUILD_NODE_FILES");
    }

    /**
     * Find the visitor value.
     *
     * @return The requested visitor value.
     */
    public static bool GetVisitor()
    {
        return BooleanValue("VISITOR");
    }

    /**
     * Find the trackTokens value.
     *
     * @return The requested trackTokens value.
     */
    public static bool GetTrackTokens()
    {
        return BooleanValue("TRACK_TOKENS");
    }

    /**
     * Find the node prefix value.
     *
     * @return The requested node prefix value.
     */
    public static string GetNodePrefix()
    {
        return StringValue("NODE_PREFIX");
    }

    /**
     * Find the node super class name.
     *
     * @return The requested node super class
     */
    public static string GetNodeExtends()
    {
        return StringValue("NODE_EXTENDS");
    }

    /**
     * Find the node class name.
     *
     * @return The requested node class
     */
    public static string GetNodeClass()
    {
        return StringValue("NODE_CLASS");
    }

    /**
     * Find the node package value.
     *
     * @return The requested node package value.
     */
    public static string GetNodePackage()
    {
        return StringValue("NODE_PACKAGE");
    }

    /**
     * Find the output file value.
     *
     * @return The requested output file value.
     */
    public static string GetOutputFile()
    {
        return StringValue("OUTPUT_FILE");
    }

    /**
     * Find the visitor exception value
     *
     * @return The requested visitor exception value.
     */
    public static string GetVisitorException()
    {
        return StringValue("VISITOR_EXCEPTION");
    }

    /**
     * Find the visitor data type value
     *
     * @return The requested visitor data type value.
     */
    public static string GetVisitorDataType()
    {
        return StringValue("VISITOR_DATA_TYPE");
    }

    /**
     * Find the visitor return type value
     *
     * @return The requested visitor return type value.
     */
    public static string GetVisitorReturnType()
    {
        return StringValue("VISITOR_RETURN_TYPE");
    }

    /**
     * Find the output directory to place the generated <code>.jj</code> files
     * into. If none is configured, use the value of
     * <code>getOutputDirectory()</code>.
     *
     * @return The requested JJTree output directory
     */
    public static string GetJJTreeOutputDirectory()
    {
        string dirName = StringValue("JJTREE_OUTPUT_DIRECTORY");
        string dir;
        if ("" == (dirName))
        {
            dir = GetOutputDirectory();
        }
        else
        {
            dir = (dirName);
        }

        return dir;
    }
}
