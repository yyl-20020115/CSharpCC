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

namespace org.javacc.jjtree;



static class CPPNodeFiles {
  private CPPNodeFiles() {}

  private static List<String> headersForJJTreeH = new ();
  /**
   * ID of the latest version (of JJTree) in which one of the Node classes
   * was modified.
   */
  static readonly string nodeVersion = Version.majorDotMinor;

  static HashSet<String> nodesToGenerate = new HashSet<String>();

  static void addType(string type) {
    if (!type==("Node") && !type==("SimpleNode")) {
      nodesToGenerate.Add(type);
    }
  }

  public static string nodeIncludeFile() {
	    return new File(JJTreeOptions.getJJTreeOutputDirectory(), "Node.h").getAbsolutePath();
	  }

  public static string simpleNodeIncludeFile() {
	    return new File(JJTreeOptions.getJJTreeOutputDirectory(), "SimpleNode.h").getAbsolutePath();
	  }

  public static string simpleNodeCodeFile() {
	    return new File(JJTreeOptions.getJJTreeOutputDirectory(), "SimpleNode.cc").getAbsolutePath();
	  }

  public static string jjtreeIncludeFile() {
	    return new File(JJTreeOptions.getJJTreeOutputDirectory(), JJTreeGlobals.parserName + "Tree.h").getAbsolutePath();
	  }

  public static string jjtreeImplFile() {
	    return new File(JJTreeOptions.getJJTreeOutputDirectory(), JJTreeGlobals.parserName + "Tree.cc").getAbsolutePath();
	  }

  public static string jjtreeIncludeFile(string s) {
	    return new File(JJTreeOptions.getJJTreeOutputDirectory(), s + ".h").getAbsolutePath();
	  }

  public static string jjtreeImplFile(string s) {
	    return new File(JJTreeOptions.getJJTreeOutputDirectory(),  s + ".cc").getAbsolutePath();
	  }

  public static string jjtreeASTIncludeFile(string ASTNode) {
	    return new File(JJTreeOptions.getJJTreeOutputDirectory(), ASTNode + ".h").getAbsolutePath();
  }

  public static string jjtreeASTCodeFile(string ASTNode) {
    return new File(JJTreeOptions.getJJTreeOutputDirectory(), ASTNode + ".cc").getAbsolutePath();
  }

  private static string visitorIncludeFile() {
    string name = visitorClass();
    return new File(JJTreeOptions.getJJTreeOutputDirectory(), name + ".h").getAbsolutePath();
  }

  public static void generateTreeClasses() {
	generateNodeHeader();
	generateSimpleNodeHeader();
	generateSimpleNodeCode();
	generateMultiTreeInterface();
    generateMultiTreeImpl();
    generateOneTreeInterface();
//    generateOneTreeImpl();
  }

  private static void generateNodeHeader()
  {
    File file = new File(nodeIncludeFile());
    OutputFile outputFile = null;

    try {
      String[] options = new String[] {"MULTI", "NODE_USES_PARSER", "VISITOR", "TRACK_TOKENS", "NODE_PREFIX", "NODE_EXTENDS", "NODE_FACTORY", Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC};
      outputFile = new OutputFile(file, nodeVersion, options);
      outputFile.setToolName("JJTree");

      if (file.exists() && !outputFile.needToWrite) {
        return;
      }

      Dictionary<String, Object> optionMap = new Dictionary<String, Object>(Options.getOptions());
      optionMap.Add(Options.NONUSER_OPTION__PARSER_NAME, JJTreeGlobals.parserName);
      optionMap.Add("VISITOR_RETURN_TYPE", getVisitorReturnType());
      optionMap.Add("VISITOR_DATA_TYPE", getVisitorArgumentType());
      optionMap.Add("VISITOR_RETURN_TYPE_VOID", Boolean.valueOf(getVisitorReturnType()==("void")));
      generateFile(outputFile, "/templates/cpp/Node.h.template", optionMap, false);
    } catch (IOException e) {
      throw new Error(e.ToString());
    }
    finally {
      if (outputFile != null) { try { outputFile.Close();  } catch(IOException ioe) {} }
    }
  }
  private static void generateSimpleNodeHeader()
  {
    File file = new File(simpleNodeIncludeFile());
    OutputFile outputFile = null;

    try {
      String[] options = new String[] {"MULTI", "NODE_USES_PARSER", "VISITOR", "TRACK_TOKENS", "NODE_PREFIX", "NODE_EXTENDS", "NODE_FACTORY", Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC};
      outputFile = new OutputFile(file, nodeVersion, options);
      outputFile.setToolName("JJTree");

      if (file.exists() && !outputFile.needToWrite) {
        return;
      }

      Dictionary<String, Object> optionMap = new Dictionary<String, Object>(Options.getOptions());
      optionMap.Add(Options.NONUSER_OPTION__PARSER_NAME, JJTreeGlobals.parserName);
      optionMap.Add("VISITOR_RETURN_TYPE", getVisitorReturnType());
      optionMap.Add("VISITOR_DATA_TYPE", getVisitorArgumentType());
      optionMap.Add("VISITOR_RETURN_TYPE_VOID", Boolean.valueOf(getVisitorReturnType()==("void")));
      generateFile(outputFile, "/templates/cpp/SimpleNode.h.template", optionMap, false);
    } catch (IOException e) {
      throw new Error(e.ToString());
    }
    finally {
      if (outputFile != null) { try { outputFile.Close();  } catch(IOException ioe) {} }
    }
  }
  private static void generateSimpleNodeCode()
  {
    File file = new File(simpleNodeCodeFile());
    OutputFile outputFile = null;

    try {
      String[] options = new String[] {"MULTI", "NODE_USES_PARSER", "VISITOR", "TRACK_TOKENS", "NODE_PREFIX", "NODE_EXTENDS", "NODE_FACTORY", Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC};
      outputFile = new OutputFile(file, nodeVersion, options);
      outputFile.setToolName("JJTree");

      if (file.exists() && !outputFile.needToWrite) {
        return;
      }

      Dictionary<String, Object> optionMap = new Dictionary<String, Object>(Options.getOptions());
      optionMap.Add(Options.NONUSER_OPTION__PARSER_NAME, JJTreeGlobals.parserName);
      optionMap.Add("VISITOR_RETURN_TYPE", getVisitorReturnType());
      optionMap.Add("VISITOR_DATA_TYPE", getVisitorArgumentType());
      optionMap.Add("VISITOR_RETURN_TYPE_VOID", Boolean.valueOf(getVisitorReturnType()==("void")));
      generateFile(outputFile, "/templates/cpp/SimpleNode.cc.template", optionMap, false);
    } catch (IOException e) {
      throw new Error(e.ToString());
    }
    finally {
      if (outputFile != null) { try { outputFile.Close();  } catch(IOException ioe) {} }
    }
  }

  private static void generateMultiTreeInterface()
  {
    OutputFile outputFile = null;

    try {
        for (Iterator<String> i = nodesToGenerate.iterator(); i.hasNext(); ) {
          string node = (String)i.next();
          File file = new File(jjtreeIncludeFile(node));
	      String[] options = new String[] {"MULTI", "NODE_USES_PARSER", "VISITOR", "TRACK_TOKENS", "NODE_PREFIX", "NODE_EXTENDS", "NODE_FACTORY", Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC};
	      outputFile = new OutputFile(file, nodeVersion, options);
	      outputFile.setToolName("JJTree");

	      if (file.exists() && !outputFile.needToWrite) {
	        return;
	      }

	      Dictionary<String, Object> optionMap = new Dictionary<String, Object>(Options.getOptions());
	      optionMap.Add(Options.NONUSER_OPTION__PARSER_NAME, JJTreeGlobals.parserName);
	      optionMap.Add("VISITOR_RETURN_TYPE", getVisitorReturnType());
	      optionMap.Add("VISITOR_DATA_TYPE", getVisitorArgumentType());
	      optionMap.Add("VISITOR_RETURN_TYPE_VOID", Boolean.valueOf(getVisitorReturnType()==("void")));

          TextWriter ostr = outputFile.getPrintWriter();
          optionMap.Add("NODE_TYPE", node);
          generateFile(outputFile, "/templates/cpp/MultiNodeInterface.template", optionMap, false);

        }
    } catch (IOException e) {
      throw new Error(e.ToString());
    }
    finally {
      if (outputFile != null) { try { outputFile.Close();  } catch(IOException ioe) {} }
    }
  }

  private static void generateMultiTreeImpl()
  {
    OutputFile outputFile = null;

    try {
        for (Iterator<String> i = nodesToGenerate.iterator(); i.hasNext(); ) {
          string node = (String)i.next();
          File file = new File(jjtreeImplFile(node));
	      String[] options = new String[] {"MULTI", "NODE_USES_PARSER", "VISITOR", "TRACK_TOKENS", "NODE_PREFIX", "NODE_EXTENDS", "NODE_FACTORY", Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC};
	      outputFile = new OutputFile(file, nodeVersion, options);
	      outputFile.setToolName("JJTree");

	      if (file.exists() && !outputFile.needToWrite) {
	        return;
	      }

	      Dictionary<String, Object> optionMap = new Dictionary<String, Object>(Options.getOptions());
	      optionMap.Add(Options.NONUSER_OPTION__PARSER_NAME, JJTreeGlobals.parserName);
	      optionMap.Add("VISITOR_RETURN_TYPE", getVisitorReturnType());
	      optionMap.Add("VISITOR_DATA_TYPE", getVisitorArgumentType());
	      optionMap.Add("VISITOR_RETURN_TYPE_VOID", Boolean.valueOf(getVisitorReturnType()==("void")));

          TextWriter ostr = outputFile.getPrintWriter();
          optionMap.Add("NODE_TYPE", node);
          generateFile(outputFile, "/templates/cpp/MultiNodeImpl.template", optionMap, false);

        }
    } catch (IOException e) {
      throw new Error(e.ToString());
    }
    finally {
      if (outputFile != null) { try { outputFile.Close();  } catch(IOException ioe) {} }
    }
  }


  private static void generateOneTreeInterface()
  {
    File file = new File(jjtreeIncludeFile());
    OutputFile outputFile = null;

    try {
      String[] options = new String[] {"MULTI", "NODE_USES_PARSER", "VISITOR", "TRACK_TOKENS", "NODE_PREFIX", "NODE_EXTENDS", "NODE_FACTORY", Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC};
      outputFile = new OutputFile(file, nodeVersion, options);
      outputFile.setToolName("JJTree");

      if (file.exists() && !outputFile.needToWrite) {
        return;
      }

      Dictionary<String, Object> optionMap = new Dictionary<String, Object>(Options.getOptions());
      optionMap.Add(Options.NONUSER_OPTION__PARSER_NAME, JJTreeGlobals.parserName);
      optionMap.Add("VISITOR_RETURN_TYPE", getVisitorReturnType());
      optionMap.Add("VISITOR_DATA_TYPE", getVisitorArgumentType());
      optionMap.Add("VISITOR_RETURN_TYPE_VOID", Boolean.valueOf(getVisitorReturnType()==("void")));

      TextWriter ostr = outputFile.getPrintWriter();
      string includeName = file.getName().Replace('.', '_').ToUpper();
      ostr.WriteLine("#ifndef " + includeName);
      ostr.WriteLine("#define " + includeName);
      ostr.WriteLine("#include \"SimpleNode.h\"");
      for (Iterator<String> i = nodesToGenerate.iterator(); i.hasNext(); ) {
          string s = (String)i.next();
          ostr.WriteLine("#include \"" + s + ".h\"");
      }
      ostr.WriteLine("#endif");
    } catch (IOException e) {
      throw new Error(e.ToString());
    }
    finally {
      if (outputFile != null) { try { outputFile.Close();  } catch(IOException ioe) {} }
    }
  }

  private static void generateOneTreeImpl()
  {
    File file = new File(jjtreeImplFile());
    OutputFile outputFile = null;

    try {
      String[] options = new String[] {"MULTI", "NODE_USES_PARSER", "VISITOR", "TRACK_TOKENS", "NODE_PREFIX", "NODE_EXTENDS", "NODE_FACTORY", Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC};
      outputFile = new OutputFile(file, nodeVersion, options);
      outputFile.setToolName("JJTree");

      if (file.exists() && !outputFile.needToWrite) {
        return;
      }

      Dictionary<String, Object> optionMap = new Dictionary<String, Object>(Options.getOptions());
      optionMap.Add(Options.NONUSER_OPTION__PARSER_NAME, JJTreeGlobals.parserName);
      optionMap.Add("VISITOR_RETURN_TYPE", getVisitorReturnType());
      optionMap.Add("VISITOR_DATA_TYPE", getVisitorArgumentType());
      optionMap.Add("VISITOR_RETURN_TYPE_VOID", Boolean.valueOf(getVisitorReturnType()==("void")));
      generateFile(outputFile, "/templates/cpp/TreeImplHeader.template", optionMap, false);

      bool hasNamespace = JJTreeOptions.stringValue(Options.USEROPTION__CPP_NAMESPACE).Length > 0;
      if (hasNamespace) {
        outputFile.getPrintWriter().WriteLine("namespace " + JJTreeOptions.stringValue("NAMESPACE_OPEN"));
      }

      for (Iterator<String> i = nodesToGenerate.iterator(); i.hasNext(); ) {
        string s = (String)i.next();
        optionMap.Add("NODE_TYPE", s);
        generateFile(outputFile, "/templates/cpp/MultiNodeImpl.template", optionMap, false);
      }

      if (hasNamespace) {
        outputFile.getPrintWriter().WriteLine(JJTreeOptions.stringValue("NAMESPACE_CLOSE"));
      }
    } catch (IOException e) {
      throw new Error(e.ToString());
    }
    finally {
      if (outputFile != null) { try { outputFile.Close();  } catch(IOException ioe) {} }
    }
  }

  static void generatePrologue(TextWriter ostr)
  {
    // Output the node's namespace name?
  }


  static string nodeConstants()
  {
    return JJTreeGlobals.parserName + "TreeConstants";
  }

  static void generateTreeConstants()
  {
    string name = nodeConstants();
    File file = new File(JJTreeOptions.getJJTreeOutputDirectory(), name + ".h");
    headersForJJTreeH.Add(file.getName());

    try {
      OutputFile outputFile = new OutputFile(file);
      TextWriter ostr = outputFile.getPrintWriter();

      List<String> nodeIds = ASTNodeDescriptor.getNodeIds();
      List<String> nodeNames = ASTNodeDescriptor.getNodeNames();

      generatePrologue(ostr);
      ostr.WriteLine("#ifndef " + file.getName().Replace('.', '_').ToUpper());
      ostr.WriteLine("#define " + file.getName().Replace('.', '_').ToUpper());

      ostr.WriteLine("\n#include \"JavaCC.h\"");
      bool hasNamespace = JJTreeOptions.stringValue(Options.USEROPTION__CPP_NAMESPACE).Length > 0;
      if (hasNamespace) {
        ostr.WriteLine("namespace " + JJTreeOptions.stringValue("NAMESPACE_OPEN"));
      }
      ostr.WriteLine("enum {");
      for (int i = 0; i < nodeIds.Count; ++i) {
        string n = (String)nodeIds.get(i);
        ostr.WriteLine("  " + n + " = " + i + ",");
      }

      ostr.WriteLine("};");
      ostr.WriteLine();

      for (int i = 0; i < nodeNames.Count; ++i) {
        ostr.WriteLine("  static JJChar jjtNodeName_arr_" + i + "[] = ");
        string n = (String)nodeNames.get(i);
        //ostr.WriteLine("    (JJChar*)\"" + n + "\",");
        OtherFilesGenCPP.printCharArray(ostr, n);
        ostr.WriteLine(";");
      }
      ostr.WriteLine("  static JJString jjtNodeName[] = {");
      for (int i = 0; i < nodeNames.Count; i++) {
        ostr.WriteLine("jjtNodeName_arr_" + i + ", ");
      }
      ostr.WriteLine("  };");

      if (hasNamespace) {
        ostr.WriteLine(JJTreeOptions.stringValue("NAMESPACE_CLOSE"));
      }


      ostr.WriteLine("#endif");
      ostr.Close();

    } catch (IOException e) {
      throw new Error(e.ToString());
    }
  }


  static string visitorClass()
  {
    return JJTreeGlobals.parserName + "Visitor";
  }

  private static string getVisitMethodName(string className) {
    StringBuilder sb = new StringBuilder("visit");
    if (JJTreeOptions.booleanValue("VISITOR_METHOD_NAME_INCLUDES_TYPE_NAME")) {
      sb.Append(Character.toUpperCase(className.charAt(0)));
      for (int i = 1; i < className.Length; i++) {
        sb.Append(className.charAt(i));
      }
    }

    return sb.ToString();
  }

  private static string getVisitorArgumentType() {
    string ret = JJTreeOptions.stringValue("VISITOR_DATA_TYPE");
    return ret == null || ret==("") || ret==("Object") ? "void *" : ret;
  }

  private static string getVisitorReturnType() {
    string ret = JJTreeOptions.stringValue("VISITOR_RETURN_TYPE");
    return ret == null || ret==("") || ret==("Object") ? "void " : ret;
  }

  static void generateVisitors() {
    if (!JJTreeOptions.getVisitor()) {
      return;
    }

    try {
      string name = visitorClass();
      File file = new File(visitorIncludeFile());
      OutputFile outputFile = new OutputFile(file);
      TextWriter ostr = outputFile.getPrintWriter();

      generatePrologue(ostr);
      ostr.WriteLine("#ifndef " + file.getName().Replace('.', '_').ToUpper());
      ostr.WriteLine("#define " + file.getName().Replace('.', '_').ToUpper());
      ostr.WriteLine("\n#include \"JavaCC.h\"");
      ostr.WriteLine("#include \"" + JJTreeGlobals.parserName + "Tree.h" + "\"");

      bool hasNamespace = JJTreeOptions.stringValue(Options.USEROPTION__CPP_NAMESPACE).Length > 0;
      if (hasNamespace) {
        ostr.WriteLine("namespace " + JJTreeOptions.stringValue("NAMESPACE_OPEN"));
      }

      generateVisitorInterface(ostr);
      generateDefaultVisitor(ostr);

      if (hasNamespace) {
        ostr.WriteLine(JJTreeOptions.stringValue("NAMESPACE_CLOSE"));
      }

      ostr.WriteLine("#endif");
      ostr.Close();
    } catch(IOException ioe) {
      throw new Error(ioe.ToString());
    }
  }

  private static void generateVisitorInterface(TextWriter ostr) {
    string name = visitorClass();
    List<String> nodeNames = ASTNodeDescriptor.getNodeNames();

    ostr.WriteLine("class " + name);
    ostr.WriteLine("{");

    string argumentType = getVisitorArgumentType();
    string returnType = getVisitorReturnType();
    if (!JJTreeOptions.getVisitorDataType()==("")) {
      argumentType = JJTreeOptions.getVisitorDataType();
    }
    ostr.WriteLine("  public:");

    ostr.WriteLine("  virtual " + returnType + " visit(const SimpleNode *node, " + argumentType + " data) = 0;");
    if (JJTreeOptions.getMulti()) {
      for (int i = 0; i < nodeNames.Count; ++i) {
        string n = (String)nodeNames.get(i);
        if (n==("void")) {
          continue;
        }
        string nodeType = JJTreeOptions.getNodePrefix() + n;
        ostr.WriteLine("  virtual " + returnType + " " + getVisitMethodName(nodeType) + "(const " + nodeType +
            " *node, " + argumentType + " data) = 0;");
      }
    }

    ostr.WriteLine("  virtual ~" + name + "() { }");
    ostr.WriteLine("};");
  }

  static string defaultVisitorClass() {
    return JJTreeGlobals.parserName + "DefaultVisitor";
  }

  private static void generateDefaultVisitor(TextWriter ostr) {
    string className = defaultVisitorClass();
    List<String> nodeNames = ASTNodeDescriptor.getNodeNames();

    ostr.WriteLine("class " + className + " : public " + visitorClass() + " {");

    string argumentType = getVisitorArgumentType();
    string ret = getVisitorReturnType();

    ostr.WriteLine("public:");
    ostr.WriteLine("  virtual " + ret + " defaultVisit(const SimpleNode *node, " + argumentType + " data) = 0;");
    //ostr.WriteLine("    node->childrenAccept(this, data);");
    //ostr.WriteLine("    return" + (ret.Trim()==("void") ? "" : " data") + ";");
    //ostr.WriteLine("  }");

    ostr.WriteLine("  virtual " + ret + " visit(const SimpleNode *node, " + argumentType + " data) {");
    ostr.WriteLine("    " + (ret.Trim()==("void") ? "" : "return ") + "defaultVisit(node, data);");
    ostr.WriteLine("}");

    if (JJTreeOptions.getMulti()) {
      for (int i = 0; i < nodeNames.Count; ++i) {
        string n = (String)nodeNames.get(i);
        if (n==("void")) {
          continue;
        }
        string nodeType = JJTreeOptions.getNodePrefix() + n;
        ostr.WriteLine("  virtual " + ret + " " + getVisitMethodName(nodeType) + "(const " + nodeType +
            " *node, " + argumentType + " data) {");
        ostr.WriteLine("    " + (ret.Trim()==("void") ? "" : "return ") + "defaultVisit(node, data);");
        ostr.WriteLine("  }");
      }
    }
    ostr.WriteLine("  ~" + className + "() { }");
    ostr.WriteLine("};");
  }

  public static void generateFile(OutputFile outputFile, string template, Dictionary<String, Object> options)
  {
    generateFile(outputFile, template, options, true);
  }

  public static void generateFile(OutputFile outputFile, string template, Dictionary<String, Object> options, bool close)
  {
    TextWriter ostr = outputFile.getPrintWriter();
    generatePrologue(ostr);
    OutputFileGenerator generator;
    generator = new OutputFileGenerator(template, options);
    generator.generate(ostr);
    if (close) ostr.Close();
  }
}
