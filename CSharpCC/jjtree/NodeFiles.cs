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
using System.Text;

namespace org.javacc.jjtree;



public static class NodeFiles
{
    /**
     * ID of the latest version (of JJTree) in which one of the Node classes
     * was modified.
     */
    static readonly string nodeVersion = Version.majorDotMinor;

    static HashSet<Node> nodesGenerated = new ();

    static void ensure(IO io, string nodeType)
    {
        string file = System.IO.Path.Combine(JJTreeOptions.getJJTreeOutputDirectory(), nodeType + ".java");

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
        if (!(nodeType == ("Node") || JJTreeOptions.getBuildNodeFiles()))
        {
            return;
        }

        if (file.exists() && nodesGenerated.Contains(file.getName()))
        {
            return;
        }

        try
        {
            String[] options = new String[] { "MULTI", "NODE_USES_PARSER", "VISITOR", "TRACK_TOKENS", "NODE_PREFIX", "NODE_EXTENDS", "NODE_FACTORY", Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
            OutputFile outputFile = new OutputFile(file, nodeVersion, options);
            outputFile.setToolName("JJTree");

            nodesGenerated.Add(file);

            if (!outputFile.needToWrite)
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

            outputFile.close();

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
        if (JJTreeGlobals.nodePackageName != (""))
        {
            ostr.WriteLine("package " + JJTreeGlobals.nodePackageName + ";");
            ostr.WriteLine();
            if (JJTreeGlobals.nodePackageName != (JJTreeGlobals.packageName))
            {
                ostr.WriteLine("import " + JJTreeGlobals.packageName + ".*;");
                ostr.WriteLine();
            }

        }
    }


    public static string nodeConstants()
    {
        return JJTreeGlobals.parserName + "TreeConstants";
    }

    public static void generateTreeConstants_java()
    {
        string name = nodeConstants();
        string file = System.IO.Path.Combine(JJTreeOptions.getJJTreeOutputDirectory(), name + ".java");

        try
        {
            OutputFile outputFile = new OutputFile(file);
            TextWriter ostr = outputFile.getPrintWriter();

            var nodeIds = ASTNodeDescriptor.getNodeIds();
            var nodeNames = ASTNodeDescriptor.getNodeNames();

            generatePrologue(ostr);
            ostr.WriteLine("public interface " + name);
            ostr.WriteLine("{");

            for (int i = 0; i < nodeIds.Count; ++i)
            {
                string n = (String)nodeIds.get(i);
                ostr.WriteLine("  public int " + n + " = " + i + ";");
            }

            ostr.WriteLine();
            ostr.WriteLine();

            ostr.WriteLine("  public String[] jjtNodeName = {");
            for (int i = 0; i < nodeNames.Count; ++i)
            {
                string n = (String)nodeNames.get(i);
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


    static string visitorClass()
    {
        return JJTreeGlobals.parserName + "Visitor";
    }

    static void generateVisitor_java()
    {
        if (!JJTreeOptions.getVisitor())
        {
            return;
        }

        string name = visitorClass();
        string file = new File(JJTreeOptions.getJJTreeOutputDirectory(), name + ".java");

        try
        {
            OutputFile outputFile = new OutputFile(file);
            TextWriter ostr = outputFile.getPrintWriter();

            List nodeNames = ASTNodeDescriptor.getNodeNames();

            generatePrologue(ostr);
            ostr.WriteLine("public interface " + name);
            ostr.WriteLine("{");

            string ve = mergeVisitorException();

            string argumentType = "Object";
            if (!JJTreeOptions.getVisitorDataType() == (""))
            {
                argumentType = JJTreeOptions.getVisitorDataType();
            }

            ostr.WriteLine("  public " + JJTreeOptions.getVisitorReturnType() + " visit(SimpleNode node, " + argumentType + " data)" +
                ve + ";");
            if (JJTreeOptions.getMulti())
            {
                for (int i = 0; i < nodeNames.Count; ++i)
                {
                    string n = (String)nodeNames.get(i);
                    if (n == ("void"))
                    {
                        continue;
                    }
                    string nodeType = JJTreeOptions.getNodePrefix() + n;
                    ostr.WriteLine("  public " + JJTreeOptions.getVisitorReturnType() + " " + getVisitMethodName(nodeType) +
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

    static string defaultVisitorClass()
    {
        return JJTreeGlobals.parserName + "DefaultVisitor";
    }

    private static string getVisitMethodName(string className)
    {
        var sb = new StringBuilder("visit");
        if (JJTreeOptions.booleanValue("VISITOR_METHOD_NAME_INCLUDES_TYPE_NAME"))
        {
            sb.Append(Character.ToUpper(className.charAt(0)));
            for (int i = 1; i < className.Length; i++)
            {
                sb.Append(className.charAt(i));
            }
        }

        return sb.ToString();
    }

    static void generateDefaultVisitor_java()
    {
        if (!JJTreeOptions.getVisitor())
        {
            return;
        }

        string className = defaultVisitorClass();
        File file = new File(JJTreeOptions.getJJTreeOutputDirectory(), className + ".java");

        try
        {
            OutputFile outputFile = new OutputFile(file);
            TextWriter ostr = outputFile.getPrintWriter();

            List nodeNames = ASTNodeDescriptor.getNodeNames();

            generatePrologue(ostr);
            ostr.WriteLine("public class " + className + " implements " + visitorClass() + "{");

            string ve = mergeVisitorException();

            string argumentType = "Object";
            if (!JJTreeOptions.getVisitorDataType() == (""))
            {
                argumentType = JJTreeOptions.getVisitorDataType().Trim();
            }

            string returnType = JJTreeOptions.getVisitorReturnType().Trim();
            bool isVoidReturnType = "void" == (returnType);

            ostr.WriteLine("  public " + returnType + " defaultVisit(SimpleNode node, " + argumentType + " data)" +
                ve + "{");
            ostr.WriteLine("    node.childrenAccept(this, data);");
            ostr.print("    return");
            if (!isVoidReturnType)
            {
                if (returnType == (argumentType))
                    ostr.print(" data");
                else if ("boolean" == (returnType))
                    ostr.print(" false");
                else if ("int" == (returnType))
                    ostr.print(" 0");
                else if ("long" == (returnType))
                    ostr.print(" 0L");
                else if ("double" == (returnType))
                    ostr.print(" 0.0d");
                else if ("float" == (returnType))
                    ostr.print(" 0.0f");
                else if ("short" == (returnType))
                    ostr.print(" 0");
                else if ("byte" == (returnType))
                    ostr.print(" 0");
                else if ("char" == (returnType))
                    ostr.print(" '\u0000'");
                else
                    ostr.print(" null");
            }
            ostr.WriteLine(";");
            ostr.WriteLine("  }");

            ostr.WriteLine("  public " + returnType + " visit(SimpleNode node, " + argumentType + " data)" +
                ve + "{");
            ostr.WriteLine("    " + (isVoidReturnType ? "" : "return ") + "defaultVisit(node, data);");
            ostr.WriteLine("  }");

            if (JJTreeOptions.getMulti())
            {
                for (int i = 0; i < nodeNames.Count; ++i)
                {
                    string n = (String)nodeNames.get(i);
                    if (n == ("void"))
                    {
                        continue;
                    }
                    string nodeType = JJTreeOptions.getNodePrefix() + n;
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
        string ve = JJTreeOptions.getVisitorException();
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
        options.Add(Options.NONUSER_OPTION__PARSER_NAME, JJTreeGlobals.parserName);

        OutputFileGenerator generator = new OutputFileGenerator(
            "/templates/Node.template", options);

        generator.generate(ostr);

        ostr.Close();
    }


    private static void generateSimpleNode_java(OutputFile outputFile)
    {
        TextWriter ostr = outputFile.getPrintWriter();

        generatePrologue(ostr);

        Dictionary options = new Dictionary(Options.getOptions());
        options.Add(Options.NONUSER_OPTION__PARSER_NAME, JJTreeGlobals.parserName);
        options.Add("VISITOR_RETURN_TYPE_VOID", Boolean.valueOf(JJTreeOptions.getVisitorReturnType() == ("void")));

        OutputFileGenerator generator = new OutputFileGenerator(
            "/templates/SimpleNode.template", options);

        generator.generate(ostr);

        ostr.Close();
    }


    private static void generateMULTINode_java(OutputFile outputFile, string nodeType)
    {
        TextWriter ostr = outputFile.getPrintWriter();

        generatePrologue(ostr);

        Dictionary options = new Dictionary(Options.getOptions());
        options.Add(Options.NONUSER_OPTION__PARSER_NAME, JJTreeGlobals.parserName);
        options.Add("NODE_TYPE", nodeType);
        options.Add("VISITOR_RETURN_TYPE_VOID", Boolean.valueOf(JJTreeOptions.getVisitorReturnType() == ("void")));

        OutputFileGenerator generator = new OutputFileGenerator(
            "/templates/MultiNode.template", options);

        generator.generate(ostr);

        ostr.Close();
    }

}
