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

using org.javacc.parser;

namespace org.javacc.jjtree;



/**
 * The JJTree-specific options.
 *
 * @author Kees Jan Koster &lt;kjkoster@kjkoster.org&gt;
 */
public class JJTreeOptions : Options
{

    /**
     * Limit subclassing to derived classes.
     */
    protected JJTreeOptions():base()
    {
    }

    /**
     * Initialize the JJTree-specific options.
     */
    public new static void init()
    {
        Options.Init();


        Options.optionValues.Add("MULTI", false);
        Options.optionValues.Add("NODE_DEFAULT_VOID", false);
        Options.optionValues.Add("NODE_SCOPE_HOOK", false);
        Options.optionValues.Add("NODE_USES_PARSER", false);
        Options.optionValues.Add("BUILD_NODE_FILES", true);
        Options.optionValues.Add("VISITOR", false);
        Options.optionValues.Add("VISITOR_METHOD_NAME_INCLUDES_TYPE_NAME", false);
        Options.optionValues.Add("TRACK_TOKENS", false);

        Options.optionValues.Add("NODE_PREFIX", "AST");
        Options.optionValues.Add("NODE_PACKAGE", "");
        Options.optionValues.Add("NODE_EXTENDS", "");
        Options.optionValues.Add("NODE_CLASS", "");
        Options.optionValues.Add("NODE_FACTORY", "");
        Options.optionValues.Add("NODE_INCLUDES", "");
        Options.optionValues.Add("OUTPUT_FILE", "");
        Options.optionValues.Add("VISITOR_DATA_TYPE", "");
        Options.optionValues.Add("VISITOR_RETURN_TYPE", "Object");
        Options.optionValues.Add("VISITOR_EXCEPTION", "");

        Options.optionValues.Add("JJTREE_OUTPUT_DIRECTORY", "");


        // TODO :: 2013/07/23 -- This appears to be a duplicate from the parent class
        Options.optionValues.Add(Options.USEROPTION__JDK_VERSION, "1.5");

        // Also appears to be a duplicate
        Options.optionValues.Add(Options.USEROPTION__CPP_NAMESPACE, "");

        // Also appears to be a duplicate
        Options.optionValues.Add(Options.USEROPTION__CPP_IGNORE_ACTIONS, false);
    }

    /**
     *  Check options for consistency
     */
    public static void validate()
    {
        if (!getVisitor())
        {
            if (getVisitorDataType().Length > 0)
            {
                JavaCCErrors.warning("VISITOR_DATA_TYPE option will be ignored since VISITOR is false");
            }
            if (getVisitorReturnType().Length > 0 && getVisitorReturnType() != ("Object"))
            {
                JavaCCErrors.warning("VISITOR_RETURN_TYPE option will be ignored since VISITOR is false");
            }
            if (getVisitorException().Length > 0)
            {
                JavaCCErrors.warning("VISITOR_EXCEPTION option will be ignored since VISITOR is false");
            }
        }
    }


    /**
     * Find the multi value.
     *
     * @return The requested multi value.
     */
    public static bool getMulti()
    {
        return booleanValue("MULTI");
    }

    /**
     * Find the node default void value.
     *
     * @return The requested node default void value.
     */
    public static bool getNodeDefaultVoid()
    {
        return booleanValue("NODE_DEFAULT_VOID");
    }

    /**
     * Find the node scope hook value.
     *
     * @return The requested node scope hook value.
     */
    public static bool getNodeScopeHook()
    {
        return booleanValue("NODE_SCOPE_HOOK");
    }

    /**
     * Find the node factory value.
     *
     * @return The requested node factory value.
     */
    public static string getNodeFactory()
    {
        return stringValue("NODE_FACTORY");
    }

    /**
     * Find the node uses parser value.
     *
     * @return The requested node uses parser value.
     */
    public static bool getNodeUsesParser()
    {
        return booleanValue("NODE_USES_PARSER");
    }

    /**
     * Find the build node files value.
     *
     * @return The requested build node files value.
     */
    public static bool getBuildNodeFiles()
    {
        return booleanValue("BUILD_NODE_FILES");
    }

    /**
     * Find the visitor value.
     *
     * @return The requested visitor value.
     */
    public static bool getVisitor()
    {
        return booleanValue("VISITOR");
    }

    /**
     * Find the trackTokens value.
     *
     * @return The requested trackTokens value.
     */
    public static bool getTrackTokens()
    {
        return booleanValue("TRACK_TOKENS");
    }

    /**
     * Find the node prefix value.
     *
     * @return The requested node prefix value.
     */
    public static string getNodePrefix()
    {
        return stringValue("NODE_PREFIX");
    }

    /**
     * Find the node super class name.
     *
     * @return The requested node super class
     */
    public static string getNodeExtends()
    {
        return stringValue("NODE_EXTENDS");
    }

    /**
     * Find the node class name.
     *
     * @return The requested node class
     */
    public static string getNodeClass()
    {
        return stringValue("NODE_CLASS");
    }

    /**
     * Find the node package value.
     *
     * @return The requested node package value.
     */
    public static string getNodePackage()
    {
        return stringValue("NODE_PACKAGE");
    }

    /**
     * Find the output file value.
     *
     * @return The requested output file value.
     */
    public static string getOutputFile()
    {
        return stringValue("OUTPUT_FILE");
    }

    /**
     * Find the visitor exception value
     *
     * @return The requested visitor exception value.
     */
    public static string getVisitorException()
    {
        return stringValue("VISITOR_EXCEPTION");
    }

    /**
     * Find the visitor data type value
     *
     * @return The requested visitor data type value.
     */
    public static string getVisitorDataType()
    {
        return stringValue("VISITOR_DATA_TYPE");
    }

    /**
     * Find the visitor return type value
     *
     * @return The requested visitor return type value.
     */
    public static string getVisitorReturnType()
    {
        return stringValue("VISITOR_RETURN_TYPE");
    }

    /**
     * Find the output directory to place the generated <code>.jj</code> files
     * into. If none is configured, use the value of
     * <code>getOutputDirectory()</code>.
     *
     * @return The requested JJTree output directory
     */
    public static string getJJTreeOutputDirectory()
    {
        string dirName = stringValue("JJTREE_OUTPUT_DIRECTORY");
        string dir;
        if ("" == (dirName))
        {
            dir = getOutputDirectory();
        }
        else
        {
            dir = (dirName);
        }

        return dir;
    }
}
