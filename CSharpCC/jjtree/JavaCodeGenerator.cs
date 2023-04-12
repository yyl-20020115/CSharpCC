// Copyright 2011 Google Inc. All Rights Reserved.
// Author: sreeni@google.com (Sreeni Viswanadha)

using org.javacc.parser;

namespace org.javacc.jjtree;


public class JavaCodeGenerator : DefaultJJTreeVisitor
{
    public Object defaultVisit(SimpleNode node, Object data)
    {
        visit((JJTreeNode)node, data);
        return null;
    }

    public Object visit(ASTGrammar node, Object data)
    {
        IO io = (IO)data;
        io.println("/*@bgen(jjtree) " +
            JavaCCGlobals.getIdString(JJTreeGlobals.toolList,
            io.getOutputFileName()).getName()) +
             " */");
        io.print("/*@egen*/");

        return node.childrenAccept(this, io);
    }

    public Object visit(ASTBNFAction node, Object data)
    {
        IO io = (IO)data;
        /* Assume that this action requires an early node close, and then
           try to decide whether this assumption is false.  Do this by
           looking outwards through the enclosing expansion units.  If we
           ever find that we are enclosed in a unit which is not the final
           unit in a sequence we know that an early close is not
           required. */

        NodeScope ns = NodeScope.getEnclosingNodeScope(node);
        if (ns != null && !ns.isVoid())
        {
            bool needClose = true;
            Node sp = node.GetScopingParent(ns);

            JJTreeNode n = node;
            while (true)
            {
                Node p = n.jjtGetParent();
                if (p is ASTBNFSequence || p is ASTBNFTryBlock)
                {
                    if (n.getOrdinal() != p.jjtGetNumChildren() - 1)
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
                openJJTreeComment(io, null);
                io.println();
                insertCloseNodeAction(ns, io, getIndentation(node));
                closeJJTreeComment(io);
            }
        }

        return visit((JJTreeNode)node, io);
    }

    public Object visit(ASTBNFDeclaration node, Object data)
    {
        IO io = (IO)data;
        if (!node.node_scope.isVoid())
        {
            string indent = "";
            if (TokenUtils.hasTokens(node))
            {
                for (int i = 1; i < node.getFirstToken().beginColumn; ++i)
                {
                    indent += " ";
                }
            }
            else
            {
                indent = "  ";
            }

            openJJTreeComment(io, node.node_scope.getNodeDescriptorText());
            io.println();
            insertOpenNodeCode(node.node_scope, io, indent);
            closeJJTreeComment(io);
        }

        return visit((JJTreeNode)node, io);
    }

    public Object visit(ASTBNFNodeScope node, Object data)
    {
        IO io = (IO)data;
        if (node.node_scope.isVoid())
        {
            return visit((JJTreeNode)node, io);
        }

        string indent = getIndentation(node.expansion_unit);

        openJJTreeComment(io, node.node_scope.getNodeDescriptor().getDescriptor());
        io.println();
        tryExpansionUnit(node.node_scope, io, indent, node.expansion_unit);
        return null;
    }

    public Object visit(ASTCompilationUnit node, Object data)
    {
        IO io = (IO)data;
        Token t = node.getFirstToken();

        while (true)
        {
            if (t == JJTreeGlobals.parserImports)
            {

                // If the parser and nodes are in separate packages (NODE_PACKAGE specified in
                // OPTIONS), then generate an import for the node package.
                if (JJTreeGlobals.nodePackageName != ("") && JJTreeGlobals.nodePackageName != (JJTreeGlobals.packageName))
                {
                    io.getOut().WriteLine();
                    io.getOut().WriteLine("import " + JJTreeGlobals.nodePackageName + ".*;");
                }
            }

            if (t == JJTreeGlobals.parserImplements)
            {
                if (t.image == ("implements"))
                {
                    node.print(t, io);
                    openJJTreeComment(io, null);
                    io.getOut().Write(" " + NodeFiles.nodeConstants() + ", ");
                    closeJJTreeComment(io);
                }
                else
                {
                    // t is pointing at the opening brace of the class body.
                    openJJTreeComment(io, null);
                    io.getOut().Write("implements " + NodeFiles.nodeConstants());
                    closeJJTreeComment(io);
                    node.print(t, io);
                }
            }
            else
            {
                node.print(t, io);
            }

            if (t == JJTreeGlobals.parserClassBodyStart)
            {
                openJJTreeComment(io, null);
                JJTreeState.insertParserMembers(io);
                closeJJTreeComment(io);
            }

            if (t == node.getLastToken())
            {
                return null;
            }
            t = t.next;
        }
    }

    public Object visit(ASTExpansionNodeScope node, Object data)
    {
        IO io = (IO)data;
        string indent = getIndentation(node.expansion_unit);
        openJJTreeComment(io, node.node_scope.getNodeDescriptor().getDescriptor());
        io.println();
        insertOpenNodeAction(node.node_scope, io, indent);
        tryExpansionUnit(node.node_scope, io, indent, node.expansion_unit);

        // Print the "whiteOut" equivalent of the Node descriptor to preserve
        // line numbers in the generated file.
        ((ASTNodeDescriptor)node.jjtGetChild(1)).jjtAccept(this, io);
        return null;
    }

    public Object visit(ASTJavacodeBody node, Object data)
    {
        IO io = (IO)data;
        if (node.node_scope.isVoid())
        {
            return visit((JJTreeNode)node, io);
        }

        Token first = node.getFirstToken();

        string indent = "";
        for (int i = 4; i < first.beginColumn; ++i)
        {
            indent += " ";
        }

        openJJTreeComment(io, node.node_scope.getNodeDescriptorText());
        io.println();
        insertOpenNodeCode(node.node_scope, io, indent);
        tryTokenSequence(node.node_scope, io, indent, first, node.getLastToken());
        return null;
    }

    public Object visit(ASTLHS node, Object data)
    {
        IO io = (IO)data;
        NodeScope ns = NodeScope.getEnclosingNodeScope(node);

        /* Print out all the tokens, converting all references to
           `jjtThis' into the current node variable. */
        Token first = node.getFirstToken();
        Token last = node.getLastToken();
        for (Token t = first; t != last.next; t = t.next)
        {
            TokenUtils.print(t, io, "jjtThis", ns.getNodeVariable());
        }

        return null;
    }

    /* This method prints the tokens corresponding to this node
       recursively calling the print methods of its children.
       Overriding this print method in appropriate nodes gives the
       output the added stuff not in the input.  */

    public Object visit(JJTreeNode node, Object data)
    {
        IO io = (IO)data;
        /* Some productions do not consume any tokens.  In that case their
           first and last tokens are a bit strange. */
        if (node.getLastToken().next == node.getFirstToken())
        {
            return null;
        }

        Token t1 = node.getFirstToken();
        Token t = new Token();
        t.next = t1;
        JJTreeNode n;
        for (int ord = 0; ord < node.jjtGetNumChildren(); ord++)
        {
            n = (JJTreeNode)node.jjtGetChild(ord);
            while (true)
            {
                t = t.next;
                if (t == n.getFirstToken()) break;
                node.print(t, io);
            }
            n.jjtAccept(this, io);
            t = n.getLastToken();
        }
        while (t != node.getLastToken())
        {
            t = t.next;
            node.print(t, io);
        }

        return null;
    }


    static void openJJTreeComment(IO io, string arg)
    {
        if (arg != null)
        {
            io.print("/*@bgen(jjtree) " + arg + " */");
        }
        else
        {
            io.print("/*@bgen(jjtree)*/");
        }
    }


    static void closeJJTreeComment(IO io)
    {
        io.print("/*@egen*/");
    }


    string getIndentation(JJTreeNode n)
    {
        return getIndentation(n, 0);
    }


    string getIndentation(JJTreeNode n, int offset)
    {
        string s = "";
        for (int i = offset + 1; i < n.getFirstToken().beginColumn; ++i)
        {
            s += " ";
        }
        return s;
    }

    void insertOpenNodeDeclaration(NodeScope ns, IO io, string indent)
    {
        insertOpenNodeCode(ns, io, indent);
    }

    void insertOpenNodeCode(NodeScope ns, IO io, string indent)
    {
        string type = ns.node_descriptor.getNodeType();
        final string nodeClass;
        if (JJTreeOptions.getNodeClass().Length > 0 && !JJTreeOptions.getMulti())
        {
            nodeClass = JJTreeOptions.getNodeClass();
        }
        else
        {
            nodeClass = type;
        }

        /* Ensure that there is a template definition file for the node
           type. */
        NodeFiles.ensure(io, type);

        io.print(indent + nodeClass + " " + ns.nodeVar + " = ");
        string p = JJTreeOptions.getStatic() ? "null" : "this";
        string parserArg = JJTreeOptions.getNodeUsesParser() ? (p + ", ") : "";

        if (JJTreeOptions.getNodeFactory() == ("*"))
        {
            // Old-style multiple-implementations.
            io.println("(" + nodeClass + ")" + nodeClass + ".jjtCreate(" + parserArg +
                ns.node_descriptor.getNodeId() + ");");
        }
        else if (JJTreeOptions.getNodeFactory().Length > 0)
        {
            io.println("(" + nodeClass + ")" + JJTreeOptions.getNodeFactory() + ".jjtCreate(" + parserArg +
             ns.node_descriptor.getNodeId() + ");");
        }
        else
        {
            io.println("new " + nodeClass + "(" + parserArg + ns.node_descriptor.getNodeId() + ");");
        }

        if (ns.usesCloseNodeVar())
        {
            io.println(indent + "boolean " + ns.closedVar + " = true;");
        }
        io.println(indent + ns.node_descriptor.openNode(ns.nodeVar));
        if (JJTreeOptions.getNodeScopeHook())
        {
            io.println(indent + "jjtreeOpenNodeScope(" + ns.nodeVar + ");");
        }

        if (JJTreeOptions.getTrackTokens())
        {
            io.println(indent + ns.nodeVar + ".jjtSetFirstToken(getToken(1));");
        }
    }


    void insertCloseNodeCode(NodeScope ns, IO io, string indent, bool isFinal)
    {
        string closeNode = ns.node_descriptor.closeNode(ns.nodeVar);
        io.println(indent + closeNode);
        if (ns.usesCloseNodeVar() && !isFinal)
        {
            io.println(indent + ns.closedVar + " = false;");
        }
        if (JJTreeOptions.getNodeScopeHook())
        {
            int i = closeNode.LastIndexOf(",");
            io.println(indent + "if (jjtree.nodeCreated()) {");
            io.println(indent + " jjtreeCloseNodeScope(" + ns.nodeVar + ");");
            io.println(indent + "}");
        }

        if (JJTreeOptions.getTrackTokens())
        {
            io.println(indent + ns.nodeVar + ".jjtSetLastToken(getToken(0));");
        }
    }


    void insertOpenNodeAction(NodeScope ns, IO io, string indent)
    {
        io.println(indent + "{");
        insertOpenNodeCode(ns, io, indent + "  ");
        io.println(indent + "}");
    }


    void insertCloseNodeAction(NodeScope ns, IO io, string indent)
    {
        io.println(indent + "{");
        insertCloseNodeCode(ns, io, indent + "  ", false);
        io.println(indent + "}");
    }


    private void insertCatchBlocks(NodeScope ns, IO io, Enumeration thrown_names,
           string indent)
    {
        string thrown;
        if (thrown_names.hasMoreElements())
        {
            io.println(indent + "} catch (Throwable " + ns.exceptionVar + ") {");

            if (ns.usesCloseNodeVar())
            {
                io.println(indent + "  if (" + ns.closedVar + ") {");
                io.println(indent + "    jjtree.clearNodeScope(" + ns.nodeVar + ");");
                io.println(indent + "    " + ns.closedVar + " = false;");
                io.println(indent + "  } else {");
                io.println(indent + "    jjtree.popNode();");
                io.println(indent + "  }");
            }

            while (thrown_names.hasMoreElements())
            {
                thrown = (String)thrown_names.nextElement();
                io.println(indent + "  if (" + ns.exceptionVar + " is " +
                    thrown + ") {");
                io.println(indent + "    throw (" + thrown + ")" + ns.exceptionVar + ";");
                io.println(indent + "  }");
            }
            /* This is either an Error or an undeclared Exception.  If it's
               an Error then the cast is good, otherwise we want to force
               the user to declare it by crashing on the bad cast. */
            io.println(indent + "  throw (Error)" + ns.exceptionVar + ";");
        }

    }


    void tryTokenSequence(NodeScope ns, IO io, string indent, Token first, Token last)
    {
        io.println(indent + "try {");
        closeJJTreeComment(io);

        /* Print out all the tokens, converting all references to
           `jjtThis' into the current node variable. */
        for (Token t = first; t != last.next; t = t.next)
        {
            TokenUtils.print(t, io, "jjtThis", ns.nodeVar);
        }

        openJJTreeComment(io, null);
        io.println();

        Enumeration thrown_names = ns.production.throws_list.elements();
        insertCatchBlocks(ns, io, thrown_names, indent);

        io.println(indent + "} finally {");
        if (ns.usesCloseNodeVar())
        {
            io.println(indent + "  if (" + ns.closedVar + ") {");
            insertCloseNodeCode(ns, io, indent + "    ", true);
            io.println(indent + "  }");
        }
        io.println(indent + "}");
        closeJJTreeComment(io);
    }


    private static void findThrown(NodeScope ns, Hashtable thrown_set,
        JJTreeNode expansion_unit)
    {
        if (expansion_unit is ASTBNFNonTerminal)
        {
            /* Should really make the nonterminal explicitly maintain its
               name. */
            string nt = expansion_unit.getFirstToken().image;
            ASTProduction prod = (ASTProduction)JJTreeGlobals.productions.get(nt);
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
            findThrown(ns, thrown_set, n);
        }
    }


    void tryExpansionUnit(NodeScope ns, IO io, string indent, JJTreeNode expansion_unit)
    {
        io.println(indent + "try {");
        closeJJTreeComment(io);

        expansion_unit.jjtAccept(this, io);

        openJJTreeComment(io, null);
        io.println();

        Hashtable thrown_set = new Hashtable();
        findThrown(ns, thrown_set, expansion_unit);
        Enumeration thrown_names = thrown_set.elements();
        insertCatchBlocks(ns, io, thrown_names, indent);

        io.println(indent + "} finally {");
        if (ns.usesCloseNodeVar())
        {
            io.println(indent + "  if (" + ns.closedVar + ") {");
            insertCloseNodeCode(ns, io, indent + "    ", true);
            io.println(indent + "  }");
        }
        io.println(indent + "}");
        closeJJTreeComment(io);
    }


}
