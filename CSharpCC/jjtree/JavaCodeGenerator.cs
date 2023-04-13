// Copyright 2011 Google Inc. All Rights Reserved.
// Author: sreeni@google.com (Sreeni Viswanadha)

using org.javacc.parser;

namespace org.javacc.jjtree;

public class JavaCodeGenerator : DefaultJJTreeVisitor
{
    public override object DefaultVisit(SimpleNode node, object data)
    {
        Visit((JJTreeNode)node, data);
        return null;
    }

    public override object Visit(ASTGrammar node, object data)
    {
        IO io = (IO)data;
        io.Println("/*@bgen(jjtree) " +
            JavaCCGlobals.GetIdString(JJTreeGlobals.ToolList,
            io.GetOutputFileName()) +
             " */");
        io.Print("/*@egen*/");

        return node.childrenAccept(this, io);
    }

    public override object Visit(ASTBNFAction node, object data)
    {
        IO io = (IO)data;
        /* Assume that this action requires an early node close, and then
           try to decide whether this assumption is false.  Do this by
           looking outwards through the enclosing expansion units.  If we
           ever find that we are enclosed in a unit which is not the final
           unit in a sequence we know that an early close is not
           required. */

        NodeScope ns = NodeScope.GetEnclosingNodeScope(node);
        if (ns != null && !ns.IsVoid)
        {
            bool needClose = true;
            Node sp = node.GetScopingParent(ns);

            JJTreeNode n = node;
            while (true)
            {
                Node p = n.jjtGetParent();
                if (p is ASTBNFSequence || p is ASTBNFTryBlock)
                {
                    if (n.GetOrdinal() != p.jjtGetNumChildren() - 1)
                    {
                        /* We're not the final unit in the sequence. */
                        needClose = false;
                        break;
                    }
                }
                else if (p is ASTBNFZeroOrOne ||
                         p is ASTBNFZeroOrMore ||
                         p is ASTBNFOneOrMore)
                {
                    needClose = false;
                    break;
                }
                if (p == sp)
                {
                    /* No more parents to look at. */
                    break;
                }
                n = (JJTreeNode)p;
            }
            if (needClose)
            {
                OpenJJTreeComment(io, null);
                io.Println();
                InsertCloseNodeAction(ns, io, GetIndentation(node));
                CloseJJTreeComment(io);
            }
        }

        return Visit((JJTreeNode)node, io);
    }

    public override object Visit(ASTBNFDeclaration node, object data)
    {
        IO io = (IO)data;
        if (!node.NodeScope.IsVoid)
        {
            string indent = "";
            if (TokenUtils.HasTokens(node))
            {
                for (int i = 1; i < node.GetFirstToken().beginColumn; ++i)
                {
                    indent += " ";
                }
            }
            else
            {
                indent = "  ";
            }

            OpenJJTreeComment(io, node.NodeScope.NodeDescriptorText);
            io.Println();
            InsertOpenNodeCode(node.NodeScope, io, indent);
            CloseJJTreeComment(io);
        }

        return Visit((JJTreeNode)node, io);
    }

    public override object Visit(ASTBNFNodeScope node, object data)
    {
        IO io = (IO)data;
        if (node.NodeScope.IsVoid)
        {
            return Visit((JJTreeNode)node, io);
        }

        string indent = GetIndentation(node.expansion_unit);

        OpenJJTreeComment(io, node.NodeScope.NodeDescriptor.GetDescriptor());
        io.Println();
        TryExpansionUnit(node.NodeScope, io, indent, node.expansion_unit);
        return null;
    }

    public override object Visit(ASTCompilationUnit node, object data)
    {
        IO io = (IO)data;
        Token t = node.GetFirstToken();

        while (true)
        {
            if (t == JJTreeGlobals.ParserImports)
            {

                // If the parser and nodes are in separate packages (NODE_PACKAGE specified in
                // OPTIONS), then generate an import for the node package.
                if (JJTreeGlobals.NodePackageName != ("") && JJTreeGlobals.NodePackageName != (JJTreeGlobals.PackageName))
                {
                    io.GetOut().WriteLine();
                    io.GetOut().WriteLine("import " + JJTreeGlobals.NodePackageName + ".*;");
                }
            }

            if (t == JJTreeGlobals.ParserImplements)
            {
                if (t.image == ("implements"))
                {
                    node.Print(t, io);
                    OpenJJTreeComment(io, null);
                    io.GetOut().Write(" " + NodeFiles.nodeConstants() + ", ");
                    CloseJJTreeComment(io);
                }
                else
                {
                    // t is pointing at the opening brace of the class body.
                    OpenJJTreeComment(io, null);
                    io.GetOut().Write("implements " + NodeFiles.nodeConstants());
                    CloseJJTreeComment(io);
                    node.Print(t, io);
                }
            }
            else
            {
                node.Print(t, io);
            }

            if (t == JJTreeGlobals.ParserClassBodyStart)
            {
                OpenJJTreeComment(io, null);
                JJTreeState.InsertParserMembers(io);
                CloseJJTreeComment(io);
            }

            if (t == node.GetLastToken())
            {
                return null;
            }
            t = t.next;
        }
    }

    public override object Visit(ASTExpansionNodeScope node, object data)
    {
        IO io = (IO)data;
        string indent = GetIndentation(node.ExpansionUnit);
        OpenJJTreeComment(io, node.NodeScope.NodeDescriptor.GetDescriptor());
        io.Println();
        InsertOpenNodeAction(node.NodeScope, io, indent);
        TryExpansionUnit(node.NodeScope, io, indent, node.ExpansionUnit);

        // Print the "whiteOut" equivalent of the Node descriptor to preserve
        // line numbers in the generated file.
        ((ASTNodeDescriptor)node.jjtGetChild(1)).jjtAccept(this, io);
        return null;
    }

    public override object Visit(ASTJavacodeBody node, object data)
    {
        IO io = (IO)data;
        if (node.NodeScope.IsVoid)
        {
            return Visit((JJTreeNode)node, io);
        }

        Token first = node.GetFirstToken();

        string indent = "";
        for (int i = 4; i < first.beginColumn; ++i)
        {
            indent += " ";
        }

        OpenJJTreeComment(io, node.NodeScope.NodeDescriptorText);
        io.Println();
        InsertOpenNodeCode(node.NodeScope, io, indent);
        TryTokenSequence(node.NodeScope, io, indent, first, node.GetLastToken());
        return null;
    }

    public override object Visit(ASTLHS node, object data)
    {
        IO io = (IO)data;
        NodeScope ns = NodeScope.GetEnclosingNodeScope(node);

        /* Print out all the tokens, converting all references to
           `jjtThis' into the current node variable. */
        Token first = node.GetFirstToken();
        Token last = node.GetLastToken();
        for (Token t = first; t != last.next; t = t.next)
        {
            TokenUtils.Print(t, io, "jjtThis", ns.NodeVariable);
        }

        return null;
    }

    /* This method prints the tokens corresponding to this node
       recursively calling the print methods of its children.
       Overriding this print method in appropriate nodes gives the
       output the added stuff not in the input.  */

    public override object Visit(JJTreeNode node, object data)
    {
        IO io = (IO)data;
        /* Some productions do not consume any tokens.  In that case their
           first and last tokens are a bit strange. */
        if (node.GetLastToken().next == node.GetFirstToken())
        {
            return null;
        }

        Token t1 = node.GetFirstToken();
        Token t = new()
        {
            next = t1
        };
        JJTreeNode n;
        for (int ord = 0; ord < node.jjtGetNumChildren(); ord++)
        {
            n = (JJTreeNode)node.jjtGetChild(ord);
            while (true)
            {
                t = t.next;
                if (t == n.GetFirstToken()) break;
                node.Print(t, io);
            }
            n.jjtAccept(this, io);
            t = n.GetLastToken();
        }
        while (t != node.GetLastToken())
        {
            t = t.next;
            node.Print(t, io);
        }

        return null;
    }


    static void OpenJJTreeComment(IO io, string arg)
    {
        if (arg != null)
        {
            io.Print("/*@bgen(jjtree) " + arg + " */");
        }
        else
        {
            io.Print("/*@bgen(jjtree)*/");
        }
    }


    static void CloseJJTreeComment(IO io)
    {
        io.Print("/*@egen*/");
    }


    string GetIndentation(JJTreeNode n)
    {
        return GetIndentation(n, 0);
    }


    string GetIndentation(JJTreeNode n, int offset)
    {
        string s = "";
        for (int i = offset + 1; i < n.GetFirstToken().beginColumn; ++i)
        {
            s += " ";
        }
        return s;
    }

    void InsertOpenNodeDeclaration(NodeScope ns, IO io, string indent)
    {
        InsertOpenNodeCode(ns, io, indent);
    }

    void InsertOpenNodeCode(NodeScope ns, IO io, string indent)
    {
        string type = ns.node_descriptor.GetNodeType();
        string nodeClass;
        if (JJTreeOptions.GetNodeClass().Length > 0 && !JJTreeOptions.GetMulti())
        {
            nodeClass = JJTreeOptions.GetNodeClass();
        }
        else
        {
            nodeClass = type;
        }

        /* Ensure that there is a template definition file for the node
           type. */
        NodeFiles.ensure(io, type);

        io.Print(indent + nodeClass + " " + ns.nodeVar + " = ");
        string p = JJTreeOptions.GetStatic() ? "null" : "this";
        string parserArg = JJTreeOptions.GetNodeUsesParser() ? (p + ", ") : "";

        if (JJTreeOptions.GetNodeFactory() == ("*"))
        {
            // Old-style multiple-implementations.
            io.Println("(" + nodeClass + ")" + nodeClass + ".jjtCreate(" + parserArg +
                ns.node_descriptor.GetNodeId() + ");");
        }
        else if (JJTreeOptions.GetNodeFactory().Length > 0)
        {
            io.Println("(" + nodeClass + ")" + JJTreeOptions.GetNodeFactory() + ".jjtCreate(" + parserArg +
             ns.node_descriptor.GetNodeId() + ");");
        }
        else
        {
            io.Println("new " + nodeClass + "(" + parserArg + ns.node_descriptor.GetNodeId() + ");");
        }

        if (ns.UsesCloseNodeVar)
        {
            io.Println(indent + "boolean " + ns.closedVar + " = true;");
        }
        io.Println(indent + ns.node_descriptor.OpenNode(ns.nodeVar));
        if (JJTreeOptions.GetNodeScopeHook())
        {
            io.Println(indent + "jjtreeOpenNodeScope(" + ns.nodeVar + ");");
        }

        if (JJTreeOptions.GetTrackTokens())
        {
            io.Println(indent + ns.nodeVar + ".jjtSetFirstToken(getToken(1));");
        }
    }


    void InsertCloseNodeCode(NodeScope ns, IO io, string indent, bool isFinal)
    {
        string closeNode = ns.node_descriptor.CloseNode(ns.nodeVar);
        io.Println(indent + closeNode);
        if (ns.UsesCloseNodeVar && !isFinal)
        {
            io.Println(indent + ns.closedVar + " = false;");
        }
        if (JJTreeOptions.GetNodeScopeHook())
        {
            int i = closeNode.LastIndexOf(",");
            io.Println(indent + "if (jjtree.nodeCreated()) {");
            io.Println(indent + " jjtreeCloseNodeScope(" + ns.nodeVar + ");");
            io.Println(indent + "}");
        }

        if (JJTreeOptions.GetTrackTokens())
        {
            io.Println(indent + ns.nodeVar + ".jjtSetLastToken(getToken(0));");
        }
    }


    void InsertOpenNodeAction(NodeScope ns, IO io, string indent)
    {
        io.Println(indent + "{");
        InsertOpenNodeCode(ns, io, indent + "  ");
        io.Println(indent + "}");
    }


    void InsertCloseNodeAction(NodeScope ns, IO io, string indent)
    {
        io.Println(indent + "{");
        InsertCloseNodeCode(ns, io, indent + "  ", false);
        io.Println(indent + "}");
    }


    private void InsertCatchBlocks(NodeScope ns, IO io, List<string> thrown_names,
           string indent)
    {
        if (thrown_names.Count>0)
        {
            io.Println(indent + "} catch (Throwable " + ns.exceptionVar + ") {");

            if (ns.UsesCloseNodeVar)
            {
                io.Println(indent + "  if (" + ns.closedVar + ") {");
                io.Println(indent + "    jjtree.clearNodeScope(" + ns.nodeVar + ");");
                io.Println(indent + "    " + ns.closedVar + " = false;");
                io.Println(indent + "  } else {");
                io.Println(indent + "    jjtree.popNode();");
                io.Println(indent + "  }");
            }

            foreach(var thrown in thrown_names)
            {
                io.Println(indent + "  if (" + ns.exceptionVar + " is " +
                    thrown + ") {");
                io.Println(indent + "    throw (" + thrown + ")" + ns.exceptionVar + ";");
                io.Println(indent + "  }");
            }
            /* This is either an Error or an undeclared Exception.  If it's
               an Error then the cast is good, otherwise we want to force
               the user to declare it by crashing on the bad cast. */
            io.Println(indent + "  throw (Error)" + ns.exceptionVar + ";");
        }

    }


    void TryTokenSequence(NodeScope ns, IO io, string indent, Token first, Token last)
    {
        io.Println(indent + "try {");
        CloseJJTreeComment(io);

        /* Print out all the tokens, converting all references to
           `jjtThis' into the current node variable. */
        for (Token t = first; t != last.next; t = t.next)
        {
            TokenUtils.Print(t, io, "jjtThis", ns.nodeVar);
        }

        OpenJJTreeComment(io, null);
        io.Println();

        var thrown_names = ns.production.throws_list;
        InsertCatchBlocks(ns, io, thrown_names, indent);

        io.Println(indent + "} finally {");
        if (ns.UsesCloseNodeVar)
        {
            io.Println(indent + "  if (" + ns.closedVar + ") {");
            InsertCloseNodeCode(ns, io, indent + "    ", true);
            io.Println(indent + "  }");
        }
        io.Println(indent + "}");
        CloseJJTreeComment(io);
    }


    private static void FindThrown(NodeScope ns, Dictionary<string,string> thrown_set,
        JJTreeNode expansion_unit)
    {
        if (expansion_unit is ASTBNFNonTerminal)
        {
            /* Should really make the nonterminal explicitly maintain its
               name. */
            string nt = expansion_unit.GetFirstToken().image;
            ASTProduction prod = (ASTProduction)JJTreeGlobals.Productions.get(nt);
            if (prod != null)
            {
                foreach(var t in prod.throws_list)
                {
                    thrown_set.Add(t, t);
                }
            }
        }
        for (int i = 0; i < expansion_unit.jjtGetNumChildren(); ++i)
        {
            JJTreeNode n = (JJTreeNode)expansion_unit.jjtGetChild(i);
            FindThrown(ns, thrown_set, n);
        }
    }


    void TryExpansionUnit(NodeScope ns, IO io, string indent, JJTreeNode expansion_unit)
    {
        io.Println(indent + "try {");
        CloseJJTreeComment(io);

        expansion_unit.jjtAccept(this, io);

        OpenJJTreeComment(io, null);
        io.Println();

        Dictionary<string,string> thrown_set = new ();
        FindThrown(ns, thrown_set, expansion_unit);
        InsertCatchBlocks(ns, io, thrown_set.Values.ToList(), indent);

        io.Println(indent + "} finally {");
        if (ns.UsesCloseNodeVar)
        {
            io.Println(indent + "  if (" + ns.closedVar + ") {");
            InsertCloseNodeCode(ns, io, indent + "    ", true);
            io.Println(indent + "  }");
        }
        io.Println(indent + "}");
        CloseJJTreeComment(io);
    }


}
