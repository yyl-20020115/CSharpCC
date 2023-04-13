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

public static class CPPNodeFiles
{

    private static readonly List<string> HeadersForJJTreeH = new();
    /**
     * ID of the latest version (of JJTree) in which one of the Node classes
     * was modified.
     */
    static readonly string NodeVersion = Version.MajorDotMinor;

    static readonly HashSet<string> NodesToGenerate = new();

    public static void AddType(string type)
    {
        if (type != ("Node") && type != ("SimpleNode"))
        {
            NodesToGenerate.Add(type);
        }
    }

    public static string NodeIncludeFile() => Path.Combine(CCTreeOptions.GetJJTreeOutputDirectory(), "Node.h");

    public static string SimpleNodeIncludeFile() => Path.Combine(CCTreeOptions.GetJJTreeOutputDirectory(), "SimpleNode.h");

    public static string SimpleNodeCodeFile() => Path.Combine(CCTreeOptions.GetJJTreeOutputDirectory(), "SimpleNode.cc");

    public static string JjtreeIncludeFile() => Path.Combine(CCTreeOptions.GetJJTreeOutputDirectory(), CCTreeGlobals.ParserName + "Tree.h");

    public static string JjtreeImplFile() => Path.Combine(CCTreeOptions.GetJJTreeOutputDirectory(), CCTreeGlobals.ParserName + "Tree.cc");

    public static string JjtreeIncludeFile(string s) => Path.Combine(CCTreeOptions.GetJJTreeOutputDirectory(), s + ".h");

    public static string JjtreeImplFile(string s) => Path.Combine(CCTreeOptions.GetJJTreeOutputDirectory(), s + ".cc");

    public static string JjtreeASTIncludeFile(string ASTNode) => Path.Combine(CCTreeOptions.GetJJTreeOutputDirectory(), ASTNode + ".h");

    public static string JjtreeASTCodeFile(string ASTNode) => Path.Combine(CCTreeOptions.GetJJTreeOutputDirectory(), ASTNode + ".cc");

    private static string VisitorIncludeFile() => Path.Combine(CCTreeOptions.GetJJTreeOutputDirectory(), VisitorClass() + ".h");

    public static void GenerateTreeClasses()
    {
        GenerateNodeHeader();
        generateSimpleNodeHeader();
        GenerateSimpleNodeCode();
        GenerateMultiTreeInterface();
        GenerateMultiTreeImpl();
        GenerateOneTreeInterface();
        //    generateOneTreeImpl();
    }

    private static void GenerateNodeHeader()
    {
        var file = NodeIncludeFile();
        OutputFile outputFile = null;

        try
        {
            var options = new string[] { "MULTI", "NODE_USES_PARSER", "VISITOR", "TRACK_TOKENS", "NODE_PREFIX", "NODE_EXTENDS", "NODE_FACTORY", Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
            outputFile = new OutputFile(file, NodeVersion, options);
            outputFile.SetToolName("JJTree");

            if (File.Exists(file) && !outputFile.NeedToWrite)
            {
                return;
            }

            Dictionary<string, object> optionMap = new(Options.getOptions())
            {
                { Options.NONUSER_OPTION__PARSER_NAME, CCTreeGlobals.ParserName },
                { "VISITOR_RETURN_TYPE", GetVisitorReturnType() },
                { "VISITOR_DATA_TYPE", GetVisitorArgumentType() },
                { "VISITOR_RETURN_TYPE_VOID", (GetVisitorReturnType() == ("void")).ToString() }
            };
            GenerateFile(outputFile, "/templates/cpp/Node.h.template", optionMap, false);
        }
        catch (IOException e)
        {
            throw new Error(e.ToString());
        }
        finally
        {
            if (outputFile != null) { try { outputFile.Close(); } catch (IOException ioe) { } }
        }
    }
    private static void generateSimpleNodeHeader()
    {
        string file = (SimpleNodeIncludeFile());
        OutputFile outputFile = null;

        try
        {
            string[] options = new string[] { "MULTI", "NODE_USES_PARSER", "VISITOR", "TRACK_TOKENS", "NODE_PREFIX", "NODE_EXTENDS", "NODE_FACTORY", Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
            outputFile = new OutputFile(file, NodeVersion, options);
            outputFile.SetToolName("JJTree");

            if (File.Exists(file) && !outputFile.NeedToWrite)
            {
                return;
            }

            Dictionary<string, object> optionMap = new(Options.getOptions())
            {
                { Options.NONUSER_OPTION__PARSER_NAME, CCTreeGlobals.ParserName },
                { "VISITOR_RETURN_TYPE", GetVisitorReturnType() },
                { "VISITOR_DATA_TYPE", GetVisitorArgumentType() },
                { "VISITOR_RETURN_TYPE_VOID", (GetVisitorReturnType() == ("void")).ToString() }
            };
            GenerateFile(outputFile, "/templates/cpp/SimpleNode.h.template", optionMap, false);
        }
        catch (IOException e)
        {
            throw new Error(e.ToString());
        }
        finally
        {
            if (outputFile != null) { try { outputFile.Close(); } catch (IOException ioe) { } }
        }
    }
    private static void GenerateSimpleNodeCode()
    {
        string file = (SimpleNodeCodeFile());
        OutputFile outputFile = null;

        try
        {
            string[] options = new string[] { "MULTI", "NODE_USES_PARSER", "VISITOR", "TRACK_TOKENS", "NODE_PREFIX", "NODE_EXTENDS", "NODE_FACTORY", Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
            outputFile = new OutputFile(file, NodeVersion, options);
            outputFile.SetToolName("JJTree");

            if (File.Exists(file) && !outputFile.NeedToWrite)
            {
                return;
            }

            Dictionary<string, object> optionMap = new(Options.getOptions())
            {
                { Options.NONUSER_OPTION__PARSER_NAME, CCTreeGlobals.ParserName },
                { "VISITOR_RETURN_TYPE", GetVisitorReturnType() },
                { "VISITOR_DATA_TYPE", GetVisitorArgumentType() },
                { "VISITOR_RETURN_TYPE_VOID", (GetVisitorReturnType() == ("void")).ToString() }
            };
            GenerateFile(outputFile, "/templates/cpp/SimpleNode.cc.template", optionMap, false);
        }
        catch (IOException e)
        {
            throw new Error(e.ToString());
        }
        finally
        {
            if (outputFile != null) { try { outputFile.Close(); } catch (IOException ioe) { } }
        }
    }

    private static void GenerateMultiTreeInterface()
    {
        OutputFile outputFile = null;

        try
        {
            foreach (string node in NodesToGenerate)
            {
                string file = (JjtreeIncludeFile(node));
                string[] options = new string[] { "MULTI", "NODE_USES_PARSER", "VISITOR", "TRACK_TOKENS", "NODE_PREFIX", "NODE_EXTENDS", "NODE_FACTORY", Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
                outputFile = new OutputFile(file, NodeVersion, options);
                outputFile.SetToolName("JJTree");

                if (File.Exists(file) && !outputFile.NeedToWrite)
                {
                    return;
                }

                Dictionary<string, object> optionMap = new(Options.getOptions())
                {
                    { Options.NONUSER_OPTION__PARSER_NAME, CCTreeGlobals.ParserName },
                    { "VISITOR_RETURN_TYPE", GetVisitorReturnType() },
                    { "VISITOR_DATA_TYPE", GetVisitorArgumentType() },
                    { "VISITOR_RETURN_TYPE_VOID", (GetVisitorReturnType() == ("void")).ToString() }
                };

                TextWriter ostr = outputFile.GetPrintWriter();
                optionMap.Add("NODE_TYPE", node);
                GenerateFile(outputFile, "/templates/cpp/MultiNodeInterface.template", optionMap, false);

            }
        }
        catch (IOException e)
        {
            throw new Error(e.ToString());
        }
        finally
        {
            if (outputFile != null) { try { outputFile.Close(); } catch (IOException ioe) { } }
        }
    }

    private static void GenerateMultiTreeImpl()
    {
        OutputFile outputFile = null;

        try
        {
            foreach (string node in NodesToGenerate)
            {
                string file = (JjtreeImplFile(node));
                string[] options = new string[] { "MULTI", "NODE_USES_PARSER", "VISITOR", "TRACK_TOKENS", "NODE_PREFIX", "NODE_EXTENDS", "NODE_FACTORY", Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
                outputFile = new OutputFile(file, NodeVersion, options);
                outputFile.SetToolName("JJTree");

                if (File.Exists(file) && !outputFile.NeedToWrite)
                {
                    return;
                }

                Dictionary<string, object> optionMap = new(Options.getOptions())
                {
                    { Options.NONUSER_OPTION__PARSER_NAME, CCTreeGlobals.ParserName },
                    { "VISITOR_RETURN_TYPE", GetVisitorReturnType() },
                    { "VISITOR_DATA_TYPE", GetVisitorArgumentType() },
                    { "VISITOR_RETURN_TYPE_VOID", (GetVisitorReturnType() == ("void")).ToString() }
                };

                TextWriter ostr = outputFile.GetPrintWriter();
                optionMap.Add("NODE_TYPE", node);
                GenerateFile(outputFile, "/templates/cpp/MultiNodeImpl.template", optionMap, false);

            }
        }
        catch (IOException e)
        {
            throw new Error(e.ToString());
        }
        finally
        {
            if (outputFile != null) { try { outputFile.Close(); } catch (IOException ioe) { } }
        }
    }


    private static void GenerateOneTreeInterface()
    {
        string file = (JjtreeIncludeFile());
        OutputFile outputFile = null;

        try
        {
            string[] options = new string[] { "MULTI", "NODE_USES_PARSER", "VISITOR", "TRACK_TOKENS", "NODE_PREFIX", "NODE_EXTENDS", "NODE_FACTORY", Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
            outputFile = new OutputFile(file, NodeVersion, options);
            outputFile.SetToolName("JJTree");

            if (File.Exists(file) && !outputFile.NeedToWrite)
            {
                return;
            }

            Dictionary<string, object> optionMap = new(Options.getOptions())
            {
                { Options.NONUSER_OPTION__PARSER_NAME, CCTreeGlobals.ParserName },
                { "VISITOR_RETURN_TYPE", GetVisitorReturnType() },
                { "VISITOR_DATA_TYPE", GetVisitorArgumentType() },
                { "VISITOR_RETURN_TYPE_VOID", (GetVisitorReturnType() == ("void")).ToString() }
            };

            TextWriter ostr = outputFile.GetPrintWriter();
            string includeName = file.Replace('.', '_').ToUpper();
            ostr.WriteLine("#ifndef " + includeName);
            ostr.WriteLine("#define " + includeName);
            ostr.WriteLine("#include \"SimpleNode.h\"");
            foreach (string s in NodesToGenerate)
            {
                ostr.WriteLine("#include \"" + s + ".h\"");
            }
            ostr.WriteLine("#endif");
        }
        catch (IOException e)
        {
            throw new Error(e.ToString());
        }
        finally
        {
            if (outputFile != null) { try { outputFile.Close(); } catch (IOException ioe) { } }
        }
    }

    private static void GenerateOneTreeImpl()
    {
        string file = (JjtreeImplFile());
        OutputFile outputFile = null;

        try
        {
            var options = new string[] { "MULTI", "NODE_USES_PARSER", "VISITOR", "TRACK_TOKENS", "NODE_PREFIX", "NODE_EXTENDS", "NODE_FACTORY", Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC };
            outputFile = new OutputFile(file, NodeVersion, options);
            outputFile.SetToolName("JJTree");

            if (File.Exists(file) && !outputFile.NeedToWrite)
            {
                return;
            }

            Dictionary<string, object> optionMap = new(Options.getOptions())
            {
                { Options.NONUSER_OPTION__PARSER_NAME, CCTreeGlobals.ParserName },
                { "VISITOR_RETURN_TYPE", GetVisitorReturnType() },
                { "VISITOR_DATA_TYPE", GetVisitorArgumentType() },
                { "VISITOR_RETURN_TYPE_VOID", (GetVisitorReturnType() == ("void")).ToString() }
            };
            GenerateFile(outputFile, "/templates/cpp/TreeImplHeader.template", optionMap, false);

            bool hasNamespace = CCTreeOptions.StringValue(Options.USEROPTION__CPP_NAMESPACE).Length > 0;
            if (hasNamespace)
            {
                outputFile.GetPrintWriter().WriteLine("namespace " + CCTreeOptions.StringValue("NAMESPACE_OPEN"));
            }

            foreach (string s in NodesToGenerate)
            {
                optionMap.Add("NODE_TYPE", s);
                GenerateFile(outputFile, "/templates/cpp/MultiNodeImpl.template", optionMap, false);
            }

            if (hasNamespace)
            {
                outputFile.GetPrintWriter().WriteLine(CCTreeOptions.StringValue("NAMESPACE_CLOSE"));
            }
        }
        catch (IOException e)
        {
            throw new Error(e.ToString());
        }
        finally
        {
            if (outputFile != null) { try { outputFile.Close(); } catch (IOException ioe) { } }
        }
    }

    static void GeneratePrologue(TextWriter ostr)
    {
        // Output the node's namespace name?
    }


    static string NodeConstants()
    {
        return CCTreeGlobals.ParserName + "TreeConstants";
    }

    public static void GenerateTreeConstants()
    {
        string name = NodeConstants();
        string file = Path.Combine(CCTreeOptions.GetJJTreeOutputDirectory(), name + ".h");
        HeadersForJJTreeH.Add(file);

        try
        {
            OutputFile outputFile = new(file);
            TextWriter ostr = outputFile.GetPrintWriter();

            List<string> nodeIds = ASTNodeDescriptor.GetNodeIds();
            List<string> nodeNames = ASTNodeDescriptor.GetNodeNames();

            GeneratePrologue(ostr);
            ostr.WriteLine("#ifndef " + file.Replace('.', '_').ToUpper());
            ostr.WriteLine("#define " + file.Replace('.', '_').ToUpper());

            ostr.WriteLine("\n#include \"JavaCC.h\"");
            bool hasNamespace = CCTreeOptions.StringValue(Options.USEROPTION__CPP_NAMESPACE).Length > 0;
            if (hasNamespace)
            {
                ostr.WriteLine("namespace " + CCTreeOptions.StringValue("NAMESPACE_OPEN"));
            }
            ostr.WriteLine("enum {");
            for (int i = 0; i < nodeIds.Count; ++i)
            {
                string n = (string)nodeIds[i];
                ostr.WriteLine("  " + n + " = " + i + ",");
            }

            ostr.WriteLine("};");
            ostr.WriteLine();

            for (int i = 0; i < nodeNames.Count; ++i)
            {
                ostr.WriteLine("  static JJChar jjtNodeName_arr_" + i + "[] = ");
                var n = nodeNames[i];
                //ostr.WriteLine("    (JJChar*)\"" + n + "\",");
                OtherFilesGenCPP.PrintCharArray(ostr, n);
                ostr.WriteLine(";");
            }
            ostr.WriteLine("  static JJString jjtNodeName[] = {");
            for (int i = 0; i < nodeNames.Count; i++)
            {
                ostr.WriteLine("jjtNodeName_arr_" + i + ", ");
            }
            ostr.WriteLine("  };");

            if (hasNamespace)
            {
                ostr.WriteLine(CCTreeOptions.StringValue("NAMESPACE_CLOSE"));
            }


            ostr.WriteLine("#endif");
            ostr.Close();

        }
        catch (IOException e)
        {
            throw new Error(e.ToString());
        }
    }


    static string VisitorClass()
    {
        return CCTreeGlobals.ParserName + "Visitor";
    }

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

    private static string GetVisitorArgumentType()
    {
        string ret = CCTreeOptions.StringValue("VISITOR_DATA_TYPE");
        return ret == null || ret == ("") || ret == ("object") ? "void *" : ret;
    }

    private static string GetVisitorReturnType()
    {
        string ret = CCTreeOptions.StringValue("VISITOR_RETURN_TYPE");
        return ret == null || ret == ("") || ret == ("object") ? "void " : ret;
    }

    public static void GenerateVisitors()
    {
        if (!CCTreeOptions.GetVisitor())
        {
            return;
        }

        try
        {
            string name = VisitorClass();
            string file = (VisitorIncludeFile());
            OutputFile outputFile = new(file);
            TextWriter ostr = outputFile.GetPrintWriter();

            GeneratePrologue(ostr);
            ostr.WriteLine("#ifndef " + file.Replace('.', '_').ToUpper());
            ostr.WriteLine("#define " + file.Replace('.', '_').ToUpper());
            ostr.WriteLine("\n#include \"JavaCC.h\"");
            ostr.WriteLine("#include \"" + CCTreeGlobals.ParserName + "Tree.h" + "\"");

            bool hasNamespace = CCTreeOptions.StringValue(Options.USEROPTION__CPP_NAMESPACE).Length > 0;
            if (hasNamespace)
            {
                ostr.WriteLine("namespace " + CCTreeOptions.StringValue("NAMESPACE_OPEN"));
            }

            GenerateVisitorInterface(ostr);
            GenerateDefaultVisitor(ostr);

            if (hasNamespace)
            {
                ostr.WriteLine(CCTreeOptions.StringValue("NAMESPACE_CLOSE"));
            }

            ostr.WriteLine("#endif");
            ostr.Close();
        }
        catch (IOException ioe)
        {
            throw new Error(ioe.ToString());
        }
    }

    private static void GenerateVisitorInterface(TextWriter ostr)
    {
        string name = VisitorClass();
        List<string> nodeNames = ASTNodeDescriptor.GetNodeNames();

        ostr.WriteLine("class " + name);
        ostr.WriteLine("{");

        string argumentType = GetVisitorArgumentType();
        string returnType = GetVisitorReturnType();
        if (CCTreeOptions.GetVisitorDataType() != (""))
        {
            argumentType = CCTreeOptions.GetVisitorDataType();
        }
        ostr.WriteLine("  public:");

        ostr.WriteLine("  virtual " + returnType + " visit(const SimpleNode *node, " + argumentType + " data) = 0;");
        if (CCTreeOptions.GetMulti())
        {
            for (int i = 0; i < nodeNames.Count; ++i)
            {
                string n = (string)nodeNames[i];
                if (n == ("void"))
                {
                    continue;
                }
                string nodeType = CCTreeOptions.GetNodePrefix() + n;
                ostr.WriteLine("  virtual " + returnType + " " + GetVisitMethodName(nodeType) + "(const " + nodeType +
                    " *node, " + argumentType + " data) = 0;");
            }
        }

        ostr.WriteLine("  virtual ~" + name + "() { }");
        ostr.WriteLine("};");
    }

    static string DefaultVisitorClass() => CCTreeGlobals.ParserName + "DefaultVisitor";

    private static void GenerateDefaultVisitor(TextWriter ostr)
    {
        string className = DefaultVisitorClass();
        List<string> nodeNames = ASTNodeDescriptor.GetNodeNames();

        ostr.WriteLine("class " + className + " : public " + VisitorClass() + " {");

        string argumentType = GetVisitorArgumentType();
        string ret = GetVisitorReturnType();

        ostr.WriteLine("public:");
        ostr.WriteLine("  virtual " + ret + " defaultVisit(const SimpleNode *node, " + argumentType + " data) = 0;");
        //ostr.WriteLine("    node->childrenAccept(this, data);");
        //ostr.WriteLine("    return" + (ret.Trim()==("void") ? "" : " data") + ";");
        //ostr.WriteLine("  }");

        ostr.WriteLine("  virtual " + ret + " visit(const SimpleNode *node, " + argumentType + " data) {");
        ostr.WriteLine("    " + (ret.Trim() == ("void") ? "" : "return ") + "defaultVisit(node, data);");
        ostr.WriteLine("}");

        if (CCTreeOptions.GetMulti())
        {
            for (int i = 0; i < nodeNames.Count; ++i)
            {
                string n = (string)nodeNames[i];
                if (n == ("void"))
                {
                    continue;
                }
                string nodeType = CCTreeOptions.GetNodePrefix() + n;
                ostr.WriteLine("  virtual " + ret + " " + GetVisitMethodName(nodeType) + "(const " + nodeType +
                    " *node, " + argumentType + " data) {");
                ostr.WriteLine("    " + (ret.Trim() == ("void") ? "" : "return ") + "defaultVisit(node, data);");
                ostr.WriteLine("  }");
            }
        }
        ostr.WriteLine("  ~" + className + "() { }");
        ostr.WriteLine("};");
    }

    public static void GenerateFile(OutputFile outputFile, string template, Dictionary<string, object> options)
    {
        GenerateFile(outputFile, template, options, true);
    }

    public static void GenerateFile(OutputFile outputFile, string template, Dictionary<string, object> options, bool close)
    {
        var ostr = outputFile.GetPrintWriter();
        GeneratePrologue(ostr);
        OutputFileGenerator generator;
        generator = new OutputFileGenerator(template, options);
        generator.Generate(ostr);
        if (close) ostr.Close();
    }
}
