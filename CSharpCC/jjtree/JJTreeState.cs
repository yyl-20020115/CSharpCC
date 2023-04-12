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



/**
 * Generate the State of a tree.
 */
public static class JJTreeState
{
    public static void InsertParserMembers(IO io)
    {
        string s;

        if (JJTreeOptions.getStatic())
        {
            s = "static ";
        }
        else
        {
            s = "";
        }

        io.Println();
        io.Println("  protected " + s + NameState() +
             " jjtree = new " + NameState() + "();");
        io.Println();
    }


    private static string NameState()
    {
        return "JJT" + JJTreeGlobals.ParserName + "State";
    }


    public static void GenerateTreeStateJava()
    {
        string file = System.IO.Path.Combine(JJTreeOptions.GetJJTreeOutputDirectory(), NameState() + ".java");

        try
        {
            OutputFile outputFile = new OutputFile(file);
            TextWriter ostr = outputFile.getPrintWriter();
            NodeFiles.generatePrologue(ostr);
            InsertState(ostr);
            outputFile.Close();
        }
        catch (IOException e)
        {
            throw new Error(e.ToString());
        }
    }


    private static void InsertState(TextWriter ostr)
    {
        ostr.WriteLine("public class " + NameState() + " {");

        if (!JJTreeOptions.getGenerateGenerics())
            ostr.WriteLine("  private java.util.List nodes;");
        else
            ostr.WriteLine("  private java.util.List<Node> nodes;");

        if (!JJTreeOptions.getGenerateGenerics())
            ostr.WriteLine("  private java.util.List marks;");
        else
            ostr.WriteLine("  private java.util.List<Integer> marks;");

        ostr.WriteLine("");
        ostr.WriteLine("  private int sp;        // number of nodes on stack");
        ostr.WriteLine("  private int mk;        // current mark");
        ostr.WriteLine("  private boolean node_created;");
        ostr.WriteLine("");
        ostr.WriteLine("  public " + NameState() + "() {");

        if (!JJTreeOptions.getGenerateGenerics())
            ostr.WriteLine("    nodes = new java.util.ArrayList();");
        else
            ostr.WriteLine("    nodes = new java.util.ArrayList<Node>();");

        if (!JJTreeOptions.getGenerateGenerics())
            ostr.WriteLine("    marks = new java.util.ArrayList();");
        else
            ostr.WriteLine("    marks = new java.util.ArrayList<Integer>();");

        ostr.WriteLine("    sp = 0;");
        ostr.WriteLine("    mk = 0;");
        ostr.WriteLine("  }");
        ostr.WriteLine("");
        ostr.WriteLine("  /* Determines whether the current node was actually closed and");
        ostr.WriteLine("     pushed.  This should only be called in the final user action of a");
        ostr.WriteLine("     node scope.  */");
        ostr.WriteLine("  public boolean nodeCreated() {");
        ostr.WriteLine("    return node_created;");
        ostr.WriteLine("  }");
        ostr.WriteLine("");
        ostr.WriteLine("  /* Call this to reinitialize the node stack.  It is called");
        ostr.WriteLine("     automatically by the parser's ReInit() method. */");
        ostr.WriteLine("  public void reset() {");
        ostr.WriteLine("    nodes.clear();");
        ostr.WriteLine("    marks.clear();");
        ostr.WriteLine("    sp = 0;");
        ostr.WriteLine("    mk = 0;");
        ostr.WriteLine("  }");
        ostr.WriteLine("");
        ostr.WriteLine("  /* Returns the root node of the AST.  It only makes sense to call");
        ostr.WriteLine("     this after a successful parse. */");
        ostr.WriteLine("  public Node rootNode() {");
        if (!JJTreeOptions.getGenerateGenerics())
            ostr.WriteLine("    return (Node)nodes.get(0);");
        else
            ostr.WriteLine("    return nodes.get(0);");
        ostr.WriteLine("  }");
        ostr.WriteLine("");
        ostr.WriteLine("  /* Pushes a node on to the stack. */");
        ostr.WriteLine("  public void pushNode(Node n) {");
        ostr.WriteLine("    nodes.add(n);");
        ostr.WriteLine("    ++sp;");
        ostr.WriteLine("  }");
        ostr.WriteLine("");
        ostr.WriteLine("  /* Returns the node on the top of the stack, and remove it from the");
        ostr.WriteLine("     stack.  */");
        ostr.WriteLine("  public Node popNode() {");
        ostr.WriteLine("    if (--sp < mk) {");
        if (!JJTreeOptions.getGenerateGenerics())
            ostr.WriteLine("      mk = ((Integer)marks.remove(marks.size()-1)).intValue();");
        else
            ostr.WriteLine("      mk = marks.remove(marks.size()-1);");
        ostr.WriteLine("    }");
        if (!JJTreeOptions.getGenerateGenerics())
            ostr.WriteLine("    return (Node)nodes.remove(nodes.size()-1);");
        else
            ostr.WriteLine("    return nodes.remove(nodes.size()-1);");
        ostr.WriteLine("  }");
        ostr.WriteLine("");
        ostr.WriteLine("  /* Returns the node currently on the top of the stack. */");
        ostr.WriteLine("  public Node peekNode() {");
        if (!JJTreeOptions.getGenerateGenerics())
            ostr.WriteLine("    return (Node)nodes.get(nodes.size()-1);");
        else
            ostr.WriteLine("    return nodes.get(nodes.size()-1);");
        ostr.WriteLine("  }");
        ostr.WriteLine("");
        ostr.WriteLine("  /* Returns the number of children on the stack in the current node");
        ostr.WriteLine("     scope. */");
        ostr.WriteLine("  public int nodeArity() {");
        ostr.WriteLine("    return sp - mk;");
        ostr.WriteLine("  }");
        ostr.WriteLine("");
        ostr.WriteLine("");
        ostr.WriteLine("  public void clearNodeScope(Node n) {");
        ostr.WriteLine("    while (sp > mk) {");
        ostr.WriteLine("      popNode();");
        ostr.WriteLine("    }");
        if (!JJTreeOptions.getGenerateGenerics())
            ostr.WriteLine("    mk = ((Integer)marks.remove(marks.size()-1)).intValue();");
        else
            ostr.WriteLine("    mk = marks.remove(marks.size()-1);");
        ostr.WriteLine("  }");
        ostr.WriteLine("");
        ostr.WriteLine("");
        ostr.WriteLine("  public void openNodeScope(Node n) {");
        if (!JJTreeOptions.getGenerateGenerics())
            ostr.WriteLine("    marks.add(Integer.valueOf(mk));");
        else
            ostr.WriteLine("    marks.add(mk);");
        ostr.WriteLine("    mk = sp;");
        ostr.WriteLine("    n.jjtOpen();");
        ostr.WriteLine("  }");
        ostr.WriteLine("");
        ostr.WriteLine("");
        ostr.WriteLine("  /* A definite node is constructed from a specified number of");
        ostr.WriteLine("     children.  That number of nodes are popped from the stack and");
        ostr.WriteLine("     made the children of the definite node.  Then the definite node");
        ostr.WriteLine("     is pushed on to the stack. */");
        ostr.WriteLine("  public void closeNodeScope(Node n, int num) {");
        if (!JJTreeOptions.getGenerateGenerics())
            ostr.WriteLine("    mk = ((Integer)marks.remove(marks.size()-1)).intValue();");
        else
            ostr.WriteLine("    mk = marks.remove(marks.size()-1);");
        ostr.WriteLine("    while (num-- > 0) {");
        ostr.WriteLine("      Node c = popNode();");
        ostr.WriteLine("      c.jjtSetParent(n);");
        ostr.WriteLine("      n.jjtAddChild(c, num);");
        ostr.WriteLine("    }");
        ostr.WriteLine("    n.jjtClose();");
        ostr.WriteLine("    pushNode(n);");
        ostr.WriteLine("    node_created = true;");
        ostr.WriteLine("  }");
        ostr.WriteLine("");
        ostr.WriteLine("");
        ostr.WriteLine("  /* A conditional node is constructed if its condition is true.  All");
        ostr.WriteLine("     the nodes that have been pushed since the node was opened are");
        ostr.WriteLine("     made children of the conditional node, which is then pushed");
        ostr.WriteLine("     on to the stack.  If the condition is false the node is not");
        ostr.WriteLine("     constructed and they are left on the stack. */");
        ostr.WriteLine("  public void closeNodeScope(Node n, boolean condition) {");
        ostr.WriteLine("    if (condition) {");
        ostr.WriteLine("      int a = nodeArity();");
        if (!JJTreeOptions.getGenerateGenerics())
            ostr.WriteLine("      mk = ((Integer)marks.remove(marks.size()-1)).intValue();");
        else
            ostr.WriteLine("      mk = marks.remove(marks.size()-1);");
        ostr.WriteLine("      while (a-- > 0) {");
        ostr.WriteLine("        Node c = popNode();");
        ostr.WriteLine("        c.jjtSetParent(n);");
        ostr.WriteLine("        n.jjtAddChild(c, a);");
        ostr.WriteLine("      }");
        ostr.WriteLine("      n.jjtClose();");
        ostr.WriteLine("      pushNode(n);");
        ostr.WriteLine("      node_created = true;");
        ostr.WriteLine("    } else {");
        if (!JJTreeOptions.getGenerateGenerics())
            ostr.WriteLine("      mk = ((Integer)marks.remove(marks.size()-1)).intValue();");
        else
            ostr.WriteLine("      mk = marks.remove(marks.size()-1);");
        ostr.WriteLine("      node_created = false;");
        ostr.WriteLine("    }");
        ostr.WriteLine("  }");
        ostr.WriteLine("}");
    }

}

/*end*/
