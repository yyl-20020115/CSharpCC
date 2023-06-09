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

using CSharpCC.Parser;
using CSharpCC.Utils;
using System.Text;

namespace CSharpCC.CCTree;

public static class NodeFiles
{
    /**
     * ID of the latest version (of JJTree) in which one of the Node classes
     * was modified.
     */
    static readonly string NodeVersion = Version.MajorDotMinor;

    static HashSet<string> NodesGenerated = new();

    public static void Ensure(IO io, string nodeType)
    {
        string file = System.IO.Path.Combine(CCTreeOptions.GetJJTreeOutputDirectory(), nodeType + ".java");

        if (nodeType == ("Node"))
        {
        }
        else if (nodeType == ("SimpleNode"))
        {
            Ensure(io, "Node");
        }
        else
        {
            Ensure(io, "SimpleNode");
        }

        /* Only build the node file if we're dealing with Node.java, or
           the NODE_BUILD_FILES option is set. */
        if (!(nodeType == ("Node") || CCTreeOptions.GetBuildNodeFiles()))
        {
            return;
        }

        if (File.Exists(file) && NodesGenerated.Contains(file))
        {
            return;
        }

        try
        {
            string[] options = new string[] { "MULTI", "NODE_USES_PARSER", "VISITOR", "TRACK_TOKENS", "NODE_PREFIX", "NODE_EXTENDS", "NODE_FACTORY", Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
            OutputFile outputFile = new OutputFile(file, NodeVersion, options);
            outputFile.SetToolName("JJTree");

            NodesGenerated.Add(file);

            if (!outputFile.NeedToWrite)
            {
                return;
            }

            if (nodeType == ("Node"))
            {
                GenerateNodeJava(outputFile);
            }
            else if (nodeType == ("SimpleNode"))
            {
                GenerateSimpleNodeJava(outputFile);
            }
            else
            {
                GenerateMULTINodeJava(outputFile, nodeType);
            }

            outputFile.Close();

        }
        catch (IOException e)
        {
            throw new Error(e.ToString());
        }
    }


    public static void GeneratePrologue(TextWriter ostr)
    {
        // Output the node's package name. JJTreeGlobals.nodePackageName
        // will be the value of NODE_PACKAGE in OPTIONS; if that wasn't set it
        // will default to the parser's package name.
        // If the package names are different we will need to import classes
        // from the parser's package.
        if (CCTreeGlobals.NodePackageName != (""))
        {
            ostr.WriteLine("package " + CCTreeGlobals.NodePackageName + ";");
            ostr.WriteLine();
            if (CCTreeGlobals.NodePackageName != (CCTreeGlobals.PackageName))
            {
                ostr.WriteLine("import " + CCTreeGlobals.PackageName + ".*;");
                ostr.WriteLine();
            }

        }
    }


    public static string NodeConstants => CCTreeGlobals.ParserName + "TreeConstants";

    public static void GenerateTreeConstantsJava()
    {
        string name = NodeConstants;
        string file = System.IO.Path.Combine(CCTreeOptions.GetJJTreeOutputDirectory(), name + ".java");

        try
        {
            OutputFile outputFile = new OutputFile(file);
            TextWriter ostr = outputFile.GetPrintWriter();

            var nodeIds = ASTNodeDescriptor.GetNodeIds();
            var nodeNames = ASTNodeDescriptor.GetNodeNames();

            GeneratePrologue(ostr);
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


    public static string VisitorClass => CCTreeGlobals.ParserName + "Visitor";

    public static void GenerateVisitorJava()
    {
        if (!CCTreeOptions.GetVisitor())
        {
            return;
        }

        string name = VisitorClass;
        string file = System.IO.Path.Combine(CCTreeOptions.GetJJTreeOutputDirectory(), name + ".java");

        try
        {
            var outputFile = new OutputFile(file);
            var ostr = outputFile.GetPrintWriter();

            var nodeNames = ASTNodeDescriptor.GetNodeNames();

            GeneratePrologue(ostr);
            ostr.WriteLine("public interface " + name);
            ostr.WriteLine("{");

            string ve = MergeVisitorException();

            string argumentType = "Object";
            if (CCTreeOptions.GetVisitorDataType() != (""))
            {
                argumentType = CCTreeOptions.GetVisitorDataType();
            }

            ostr.WriteLine("  public " + CCTreeOptions.GetVisitorReturnType() + " visit(SimpleNode node, " + argumentType + " data)" +
                ve + ";");
            if (CCTreeOptions.GetMulti())
            {
                for (int i = 0; i < nodeNames.Count; ++i)
                {
                    string n = (String)nodeNames[i];
                    if (n == ("void"))
                    {
                        continue;
                    }
                    string nodeType = CCTreeOptions.GetNodePrefix() + n;
                    ostr.WriteLine("  public " + CCTreeOptions.GetVisitorReturnType() + " " + GetVisitMethodName(nodeType) +
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

    public static string DefaultVisitorClass => CCTreeGlobals.ParserName + "DefaultVisitor";

    private static string GetVisitMethodName(string className)
    {
        var sb = new StringBuilder("visit");
        if (CCTreeOptions.BooleanValue("VISITOR_METHOD_NAME_INCLUDES_TYPE_NAME"))
        {
            sb.Append(char.ToUpper(className[0]));
            for (int i = 1; i < className.Length; i++)
            {
                sb.Append(className[i]);
            }
        }

        return sb.ToString();
    }

    public static void GenerateDefaultVisitorJava()
    {
        if (!CCTreeOptions.GetVisitor())
        {
            return;
        }

        string className = DefaultVisitorClass;
        string file = System.IO.Path.Combine(CCTreeOptions.GetJJTreeOutputDirectory(), className + ".java");

        try
        {
            OutputFile outputFile = new OutputFile(file);
            TextWriter ostr = outputFile.GetPrintWriter();

            var nodeNames = ASTNodeDescriptor.GetNodeNames();

            GeneratePrologue(ostr);
            ostr.WriteLine("public class " + className + " implements " + VisitorClass + "{");

            string ve = MergeVisitorException();

            string argumentType = "Object";
            if (CCTreeOptions.GetVisitorDataType() != (""))
            {
                argumentType = CCTreeOptions.GetVisitorDataType().Trim();
            }

            string returnType = CCTreeOptions.GetVisitorReturnType().Trim();
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

            if (CCTreeOptions.GetMulti())
            {
                for (int i = 0; i < nodeNames.Count; ++i)
                {
                    string n = (String)nodeNames[i];
                    if (n == ("void"))
                    {
                        continue;
                    }
                    string nodeType = CCTreeOptions.GetNodePrefix() + n;
                    ostr.WriteLine("  public " + returnType + " " + GetVisitMethodName(nodeType) +
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

    private static string MergeVisitorException()
    {
        string ve = CCTreeOptions.GetVisitorException();
        if ("" != (ve))
        {
            ve = " throws " + ve;
        }
        return ve;
    }


    private static void GenerateNodeJava(OutputFile outputFile)
    {
        TextWriter ostr = outputFile.GetPrintWriter();

        GeneratePrologue(ostr);

        var options = new Dictionary<string, object>(Options.getOptions());
        options.Add(Options.NONUSER_OPTION__PARSER_NAME, CCTreeGlobals.ParserName);

        OutputFileGenerator generator = new OutputFileGenerator(
            "/templates/Node.template", options);

        generator.Generate(ostr);

        ostr.Close();
    }


    private static void GenerateSimpleNodeJava(OutputFile outputFile)
    {
        TextWriter ostr = outputFile.GetPrintWriter();

        GeneratePrologue(ostr);

        var options = new Dictionary<string, object>(Options.getOptions());
        options.Add(Options.NONUSER_OPTION__PARSER_NAME, CCTreeGlobals.ParserName);
        options.Add("VISITOR_RETURN_TYPE_VOID", (CCTreeOptions.GetVisitorReturnType() == ("void")));

        OutputFileGenerator generator = new OutputFileGenerator(
            "/templates/SimpleNode.template", options);

        generator.Generate(ostr);

        ostr.Close();
    }


    private static void GenerateMULTINodeJava(OutputFile outputFile, string nodeType)
    {
        var ostr = outputFile.GetPrintWriter();

        GeneratePrologue(ostr);

        var options = new Dictionary<string, object>(Options.getOptions());
        options.Add(Options.NONUSER_OPTION__PARSER_NAME, CCTreeGlobals.ParserName);
        options.Add("NODE_TYPE", nodeType);
        options.Add("VISITOR_RETURN_TYPE_VOID", (CCTreeOptions.GetVisitorReturnType() == ("void")));

        var generator = new OutputFileGenerator(
            "/templates/MultiNode.template", options);

        generator.Generate(ostr);

        ostr.Close();
    }
}
