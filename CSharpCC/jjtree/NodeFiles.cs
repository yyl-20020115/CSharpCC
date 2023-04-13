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
using org.javacc.utils;
using System.Text;

namespace org.javacc.jjtree;



public static class NodeFiles
{
    /**
     * ID of the latest version (of JJTree) in which one of the Node classes
     * was modified.
     */
    static readonly string nodeVersion = Version.MajorDotMinor;

    static HashSet<string> nodesGenerated = new ();

    public static void ensure(IO io, string nodeType)
    {
        string file = System.IO.Path.Combine(JJTreeOptions.GetJJTreeOutputDirectory(), nodeType + ".java");

        if (nodeType == ("Node"))
        {
        }
        else if (nodeType == ("SimpleNode"))
        {
            ensure(io, "Node");
        }
        else
        {
            ensure(io, "SimpleNode");
        }

        /* Only build the node file if we're dealing with Node.java, or
           the NODE_BUILD_FILES option is set. */
        if (!(nodeType == ("Node") || JJTreeOptions.GetBuildNodeFiles()))
        {
            return;
        }

        if (File.Exists(file) && nodesGenerated.Contains(file))
        {
            return;
        }

        try
        {
            String[] options = new String[] { "MULTI", "NODE_USES_PARSER", "VISITOR", "TRACK_TOKENS", "NODE_PREFIX", "NODE_EXTENDS", "NODE_FACTORY", Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
            OutputFile outputFile = new OutputFile(file, nodeVersion, options);
            outputFile.setToolName("JJTree");

            nodesGenerated.Add(file);

            if (!outputFile.NeedToWrite)
            {
                return;
            }

            if (nodeType == ("Node"))
            {
                generateNode_java(outputFile);
            }
            else if (nodeType == ("SimpleNode"))
            {
                generateSimpleNode_java(outputFile);
            }
            else
            {
                generateMULTINode_java(outputFile, nodeType);
            }

            outputFile.Close();

        }
        catch (IOException e)
        {
            throw new Error(e.ToString());
        }
    }


    public static void generatePrologue(TextWriter ostr)
    {
        // Output the node's package name. JJTreeGlobals.nodePackageName
        // will be the value of NODE_PACKAGE in OPTIONS; if that wasn't set it
        // will default to the parser's package name.
        // If the package names are different we will need to import classes
        // from the parser's package.
        if (JJTreeGlobals.NodePackageName != (""))
        {
            ostr.WriteLine("package " + JJTreeGlobals.NodePackageName + ";");
            ostr.WriteLine();
            if (JJTreeGlobals.NodePackageName != (JJTreeGlobals.PackageName))
            {
                ostr.WriteLine("import " + JJTreeGlobals.PackageName + ".*;");
                ostr.WriteLine();
            }

        }
    }


    public static string nodeConstants()
    {
        return JJTreeGlobals.ParserName + "TreeConstants";
    }

    public static void generateTreeConstants_java()
    {
        string name = nodeConstants();
        string file = System.IO.Path.Combine(JJTreeOptions.GetJJTreeOutputDirectory(), name + ".java");

        try
        {
            OutputFile outputFile = new OutputFile(file);
            TextWriter ostr = outputFile.getPrintWriter();

            var nodeIds = ASTNodeDescriptor.GetNodeIds();
            var nodeNames = ASTNodeDescriptor.GetNodeNames();

            generatePrologue(ostr);
            ostr.WriteLine("public interface " + name);
            ostr.WriteLine("{");

            for (int i = 0; i < nodeIds.Count; ++i)
            {
                string n = (String)nodeIds[i];
                ostr.WriteLine("  public int " + n + " = " + i + ";");
            }

            ostr.WriteLine();
            ostr.WriteLine();

            ostr.WriteLine("  public String[] jjtNodeName = {");
            for (int i = 0; i < nodeNames.Count; ++i)
            {
                string n = (String)nodeNames[i];
                ostr.WriteLine("    \"" + n + "\",");
            }
            ostr.WriteLine("  };");

            ostr.WriteLine("}");
            ostr.Close();

        }
        catch (IOException e)
        {
            throw new Error(e.ToString());
        }
    }


    public static string visitorClass()
    {
        return JJTreeGlobals.ParserName + "Visitor";
    }

    public static void generateVisitor_java()
    {
        if (!JJTreeOptions.GetVisitor())
        {
            return;
        }

        string name = visitorClass();
        string file = System.IO.Path.Combine(JJTreeOptions.GetJJTreeOutputDirectory(), name + ".java");

        try
        {
            OutputFile outputFile = new OutputFile(file);
            TextWriter ostr = outputFile.getPrintWriter();

            var nodeNames = ASTNodeDescriptor.GetNodeNames();

            generatePrologue(ostr);
            ostr.WriteLine("public interface " + name);
            ostr.WriteLine("{");

            string ve = mergeVisitorException();

            string argumentType = "Object";
            if (JJTreeOptions.GetVisitorDataType() != (""))
            {
                argumentType = JJTreeOptions.GetVisitorDataType();
            }

            ostr.WriteLine("  public " + JJTreeOptions.GetVisitorReturnType() + " visit(SimpleNode node, " + argumentType + " data)" +
                ve + ";");
            if (JJTreeOptions.GetMulti())
            {
                for (int i = 0; i < nodeNames.Count; ++i)
                {
                    string n = (String)nodeNames[i];
                    if (n == ("void"))
                    {
                        continue;
                    }
                    string nodeType = JJTreeOptions.GetNodePrefix() + n;
                    ostr.WriteLine("  public " + JJTreeOptions.GetVisitorReturnType() + " " + getVisitMethodName(nodeType) +
                    "(" + nodeType +
                        " node, " + argumentType + " data)" + ve + ";");
                }
            }
            ostr.WriteLine("}");
            ostr.Close();

        }
        catch (IOException e)
        {
            throw new Error(e.ToString());
        }
    }

    public static string defaultVisitorClass()
    {
        return JJTreeGlobals.ParserName + "DefaultVisitor";
    }

    private static string getVisitMethodName(string className)
    {
        var sb = new StringBuilder("visit");
        if (JJTreeOptions.BooleanValue("VISITOR_METHOD_NAME_INCLUDES_TYPE_NAME"))
        {
            sb.Append(char.ToUpper(className[0]));
            for (int i = 1; i < className.Length; i++)
            {
                sb.Append(className[i]);
            }
        }

        return sb.ToString();
    }

    public static void generateDefaultVisitor_java()
    {
        if (!JJTreeOptions.GetVisitor())
        {
            return;
        }

        string className = defaultVisitorClass();
        File file = new File(JJTreeOptions.GetJJTreeOutputDirectory(), className + ".java");

        try
        {
            OutputFile outputFile = new OutputFile(file);
            TextWriter ostr = outputFile.getPrintWriter();

            var nodeNames = ASTNodeDescriptor.GetNodeNames();

            generatePrologue(ostr);
            ostr.WriteLine("public class " + className + " implements " + visitorClass() + "{");

            string ve = mergeVisitorException();

            string argumentType = "Object";
            if (!JJTreeOptions.GetVisitorDataType() == (""))
            {
                argumentType = JJTreeOptions.GetVisitorDataType().Trim();
            }

            string returnType = JJTreeOptions.GetVisitorReturnType().Trim();
            bool isVoidReturnType = "void" == (returnType);

            ostr.WriteLine("  public " + returnType + " defaultVisit(SimpleNode node, " + argumentType + " data)" +
                ve + "{");
            ostr.WriteLine("    node.childrenAccept(this, data);");
            ostr.Write("    return");
            if (!isVoidReturnType)
            {
                if (returnType == (argumentType))
                    ostr.Write(" data");
                else if ("boolean" == (returnType))
                    ostr.Write(" false");
                else if ("int" == (returnType))
                    ostr.Write(" 0");
                else if ("long" == (returnType))
                    ostr.Write(" 0L");
                else if ("double" == (returnType))
                    ostr.Write(" 0.0d");
                else if ("float" == (returnType))
                    ostr.Write(" 0.0f");
                else if ("short" == (returnType))
                    ostr.Write(" 0");
                else if ("byte" == (returnType))
                    ostr.Write(" 0");
                else if ("char" == (returnType))
                    ostr.Write(" '\u0000'");
                else
                    ostr.Write(" null");
            }
            ostr.WriteLine(";");
            ostr.WriteLine("  }");

            ostr.WriteLine("  public " + returnType + " visit(SimpleNode node, " + argumentType + " data)" +
                ve + "{");
            ostr.WriteLine("    " + (isVoidReturnType ? "" : "return ") + "defaultVisit(node, data);");
            ostr.WriteLine("  }");

            if (JJTreeOptions.GetMulti())
            {
                for (int i = 0; i < nodeNames.Count; ++i)
                {
                    string n = (String)nodeNames[i];
                    if (n == ("void"))
                    {
                        continue;
                    }
                    string nodeType = JJTreeOptions.GetNodePrefix() + n;
                    ostr.WriteLine("  public " + returnType + " " + getVisitMethodName(nodeType) +
                    "(" + nodeType +
                        " node, " + argumentType + " data)" + ve + "{");
                    ostr.WriteLine("    " + (isVoidReturnType ? "" : "return ") + "defaultVisit(node, data);");
                    ostr.WriteLine("  }");
                }
            }

            ostr.WriteLine("}");
            ostr.Close();

        }
        catch (IOException e)
        {
            throw new Error(e.ToString());
        }
    }

    private static string mergeVisitorException()
    {
        string ve = JJTreeOptions.GetVisitorException();
        if (!"" == (ve))
        {
            ve = " throws " + ve;
        }
        return ve;
    }


    private static void generateNode_java(OutputFile outputFile)
    {
        TextWriter ostr = outputFile.getPrintWriter();

        generatePrologue(ostr);

        Dictionary options = new Dictionary(Options.getOptions());
        options.Add(Options.NONUSER_OPTION__PARSER_NAME, JJTreeGlobals.ParserName);

        OutputFileGenerator generator = new OutputFileGenerator(
            "/templates/Node.template", options);

        generator.Generate(ostr);

        ostr.Close();
    }


    private static void generateSimpleNode_java(OutputFile outputFile)
    {
        TextWriter ostr = outputFile.getPrintWriter();

        generatePrologue(ostr);

        Dictionary options = new Dictionary(Options.getOptions());
        options.Add(Options.NONUSER_OPTION__PARSER_NAME, JJTreeGlobals.ParserName);
        options.Add("VISITOR_RETURN_TYPE_VOID", Boolean.valueOf(JJTreeOptions.GetVisitorReturnType() == ("void")));

        OutputFileGenerator generator = new OutputFileGenerator(
            "/templates/SimpleNode.template", options);

        generator.Generate(ostr);

        ostr.Close();
    }


    private static void generateMULTINode_java(OutputFile outputFile, string nodeType)
    {
        TextWriter ostr = outputFile.getPrintWriter();

        generatePrologue(ostr);

        Dictionary options = new Dictionary(Options.getOptions());
        options.Add(Options.NONUSER_OPTION__PARSER_NAME, JJTreeGlobals.ParserName);
        options.Add("NODE_TYPE", nodeType);
        options.Add("VISITOR_RETURN_TYPE_VOID", Boolean.valueOf(JJTreeOptions.GetVisitorReturnType() == ("void")));

        OutputFileGenerator generator = new OutputFileGenerator(
            "/templates/MultiNode.template", options);

        generator.Generate(ostr);

        ostr.Close();
    }

}
