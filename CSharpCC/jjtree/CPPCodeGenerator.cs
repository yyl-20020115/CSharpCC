// Copyright 2011 Google Inc. All Rights Reserved.
// Author: sreeni@google.com (Sreeni Viswanadha)

using org.javacc.parser;

namespace org.javacc.jjtree;



public class CPPCodeGenerator : DefaultJJTreeVisitor
{
    public override Object DefaultVisit(SimpleNode node, Object data)
    {
        visit((JJTreeNode)node, data);
        return null;
    }

    public override Object Visit(ASTGrammar node, Object data)
    {
        IO io = (IO)data;
        io.Println("/*@bgen(jjtree) " +
            JavaCCGlobals.getIdString(JJTreeGlobals.ToolList,
            io.GetOutputFileName()) +
             (JJTreeOptions.booleanValue(Options.USEROPTION__CPP_IGNORE_ACTIONS) ? "" : " */"));
        io.Print((JJTreeOptions.booleanValue(Options.USEROPTION__CPP_IGNORE_ACTIONS) ? "" : "/*") + "@egen*/");

        return node.childrenAccept(this, io);
    }

    public override Object Visit(ASTBNFAction node, Object data)
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

        return visit((JJTreeNode)node, io);
    }

    public override Object Visit(ASTBNFDeclaration node, Object data)
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

        return visit((JJTreeNode)node, io);
    }

    public override Object Visit(ASTBNFNodeScope node, Object data)
    {
        IO io = (IO)data;
        if (node.NodeScope.IsVoid)
        {
            return visit((JJTreeNode)node, io);
        }

        string indent = GetIndentation(node.expansion_unit);

        OpenJJTreeComment(io, node.NodeScope.NodeDescriptor.GetDescriptor());
        io.Println();
        TryExpansionUnit(node.NodeScope, io, indent, node.expansion_unit);
        return null;
    }

    public override Object Visit(ASTCompilationUnit node, Object data)
    {
        IO io = (IO)data;
        Token t = node.GetFirstToken();
        while (true)
        {
            node.Print(t, io);
            if (t == node.GetLastToken()) break;
            if (t.kind == JJTreeParserConstants._PARSER_BEGIN)
            {
                // eat PARSER_BEGIN "(" <ID> ")"
                node.Print(t.next, io);
                node.Print(t.next.next, io);
                node.Print(t = t.next.next.next, io);
            }

            t = t.next;
        }
        return null;
    }

    public override Object Visit(ASTExpansionNodeScope node, Object data)
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

    public override Object Visit(ASTJavacodeBody node, Object data)
    {
        IO io = (IO)data;
        if (node.NodeScope.IsVoid)
        {
            return visit((JJTreeNode)node, io);
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

    public override Object Visit(ASTLHS node, Object data)
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

    public override Object Visit(JJTreeNode node, Object data)
    {
        IO io = (IO)data;
        /* Some productions do not consume any tokens.  In that case their
           first and last tokens are a bit strange. */
        if (node.GetLastToken().next == node.GetFirstToken())
        {
            return null;
        }

        Token t1 = node.GetFirstToken();
        Token t = new Token();
        t.next = t1;
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


    private static void OpenJJTreeComment(IO io, string arg)
    {
        if (arg != null)
        {
            io.Print("/*@bgen(jjtree) " + arg + (JJTreeOptions.booleanValue(Options.USEROPTION__CPP_IGNORE_ACTIONS) ? "" : " */"));
        }
        else
        {
            io.Print("/*@bgen(jjtree)" + (JJTreeOptions.booleanValue(Options.USEROPTION__CPP_IGNORE_ACTIONS) ? "" : "*/"));
        }
    }


    private static void CloseJJTreeComment(IO io)
    {
        io.Print((JJTreeOptions.booleanValue(Options.USEROPTION__CPP_IGNORE_ACTIONS) ? "" : "/*") + "@egen*/");
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

        CPPNodeFiles.AddType(type);

        io.Print(indent + nodeClass + " *" + ns.nodeVar + " = ");
        string p = JJTreeOptions.getStatic() ? "null" : "this";
        string parserArg = JJTreeOptions.GetNodeUsesParser() ? (p + ", ") : "";

        if (JJTreeOptions.GetNodeFactory() == ("*"))
        {
            // Old-style multiple-implementations.
            io.Println("(" + nodeClass + "*)" + nodeClass + "::jjtCreate(" + parserArg +
                ns.node_descriptor.GetNodeId() + ");");
        }
        else if (JJTreeOptions.GetNodeFactory().Length > 0)
        {
            io.Println("(" + nodeClass + "*)nodeFactory->jjtCreate(" + parserArg +
             ns.node_descriptor.GetNodeId() + ");");
        }
        else
        {
            io.Println("new " + nodeClass + "(" + parserArg + ns.node_descriptor.GetNodeId() + ");");
        }

        if (ns.UsesCloseNodeVar)
        {
            io.Println(indent + "bool " + ns.closedVar + " = true;");
        }
        io.Println(indent + ns.node_descriptor.OpenNode(ns.nodeVar));
        if (JJTreeOptions.GetNodeScopeHook())
        {
            io.Println(indent + "jjtreeOpenNodeScope(" + ns.nodeVar + ");");
        }

        if (JJTreeOptions.GetTrackTokens())
        {
            io.Println(indent + ns.nodeVar + "->jjtSetFirstToken(getToken(1));");
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
            io.Println(indent + "if (jjtree.nodeCreated()) {");
            io.Println(indent + " jjtreeCloseNodeScope(" + ns.nodeVar + ");");
            io.Println(indent + "}");
        }

        if (JJTreeOptions.GetTrackTokens())
        {
            io.Println(indent + ns.nodeVar + "->jjtSetLastToken(getToken(0));");
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


    private void InsertCatchBlocks(NodeScope ns, IO io, object thrown_names,
           string indent)
    {
        string thrown;
        //if (thrown_names.hasMoreElements()) {
        io.Println(indent + "} catch (...) {"); // " +  ns.exceptionVar + ") {");

        if (ns.UsesCloseNodeVar)
        {
            io.Println(indent + "  if (" + ns.closedVar + ") {");
            io.Println(indent + "    jjtree.clearNodeScope(" + ns.nodeVar + ");");
            io.Println(indent + "    " + ns.closedVar + " = false;");
            io.Println(indent + "  } else {");
            io.Println(indent + "    jjtree.popNode();");
            io.Println(indent + "  }");
        }
        //}

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

        Enumeration thrown_names = ns.production.throws_list.elements();
        InsertCatchBlocks(ns, io, thrown_names, indent);

        io.Println(indent + "} {");
        if (ns.UsesCloseNodeVar)
        {
            io.Println(indent + "  if (" + ns.closedVar + ") {");
            InsertCloseNodeCode(ns, io, indent + "    ", true);
            io.Println(indent + "  }");
        }
        io.Println(indent + "}");
        CloseJJTreeComment(io);
    }


    private static void FindThrown(NodeScope ns, Dictionary thrown_set,
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
                Enumeration e = prod.throws_list.elements();
                while (e.hasMoreElements())
                {
                    string t = (String)e.nextElement();
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

        Dictionary thrown_set = new Dictionary();
        FindThrown(ns, thrown_set, expansion_unit);
        Enumeration thrown_names = thrown_set.elements();
        InsertCatchBlocks(ns, io, thrown_names, indent);

        io.Println(indent + "} {");
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
