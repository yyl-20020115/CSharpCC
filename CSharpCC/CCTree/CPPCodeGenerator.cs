// Copyright 2011 Google Inc. All Rights Reserved.
// Author: sreeni@google.com (Sreeni Viswanadha)

using CSharpCC.Parser;

namespace CSharpCC.CCTree;

public class CPPCodeGenerator : DefaultCCTreeVisitor
{
    public override object DefaultVisit(SimpleNode node, object data)
    {
        Visit(node as TreeNode, data);
        return null;
    }

    public override object Visit(ASTGrammar node, object data)
    {
        IO io = (IO)data;
        io.WriteLine("/*@bgen(jjtree) " +
            CSharpCCGlobals.GetIdString(CCTreeGlobals.ToolList,
            io.            OutputFileName) +
             (CCTreeOptions.BooleanValue(Options.USEROPTION__CPP_IGNORE_ACTIONS) ? "" : " */"));
        io.Write((CCTreeOptions.BooleanValue(Options.USEROPTION__CPP_IGNORE_ACTIONS) ? "" : "/*") + "@egen*/");

        return node.ChildrenAccept(this, io);
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

            TreeNode n = node;
            while (true)
            {
                Node p = n.Parent;
                if (p is ASTBNFSequence || p is ASTBNFTryBlock)
                {
                    if (n.Ordinal != p.ChildrenCount - 1)
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
                n = (TreeNode)p;
            }
            if (needClose)
            {
                OpenJJTreeComment(io, null);
                io.WriteLine();
                InsertCloseNodeAction(ns, io, GetIndentation(node));
                CloseJJTreeComment(io);
            }
        }

        return Visit((TreeNode)node, io);
    }

    public override object Visit(ASTBNFDeclaration node, object data)
    {
        IO io = (IO)data;
        if (!node.NodeScope.IsVoid)
        {
            string indent = "";
            if (TokenUtils.HasTokens(node))
            {
                for (int i = 1; i < node.FirstToken.BeginColumn; ++i)
                {
                    indent += " ";
                }
            }
            else
            {
                indent = "  ";
            }

            OpenJJTreeComment(io, node.NodeScope.NodeDescriptorText);
            io.WriteLine();
            InsertOpenNodeCode(node.NodeScope, io, indent);
            CloseJJTreeComment(io);
        }

        return Visit((TreeNode)node, io);
    }

    public override object Visit(ASTBNFNodeScope node, object data)
    {
        IO io = (IO)data;
        if (node.NodeScope.IsVoid)
        {
            return Visit((TreeNode)node, io);
        }

        string indent = GetIndentation(node.expansion_unit);

        OpenJJTreeComment(io, node.NodeScope.NodeDescriptor.GetDescriptor());
        io.WriteLine();
        TryExpansionUnit(node.NodeScope, io, indent, node.expansion_unit);
        return null;
    }

    public override object Visit(ASTCompilationUnit node, object data)
    {
        IO io = (IO)data;
        Token t = node.FirstToken;
        while (true)
        {
            node.Print(t, io);
            if (t == node.LastToken) break;
            if (t.Kind == CCTreeParserConstants._PARSER_BEGIN)
            {
                // eat PARSER_BEGIN "(" <ID> ")"
                node.Print(t.Next, io);
                node.Print(t.Next.Next, io);
                node.Print(t = t.Next.Next.Next, io);
            }

            t = t.Next;
        }
        return null;
    }

    public override object Visit(ASTExpansionNodeScope node, object data)
    {
        IO io = (IO)data;
        string indent = GetIndentation(node.ExpansionUnit);
        OpenJJTreeComment(io, node.NodeScope.NodeDescriptor.GetDescriptor());
        io.WriteLine();
        InsertOpenNodeAction(node.NodeScope, io, indent);
        TryExpansionUnit(node.NodeScope, io, indent, node.ExpansionUnit);

        // Print the "whiteOut" equivalent of the Node descriptor to preserve
        // line numbers in the generated file.
        ((ASTNodeDescriptor)node.GetChild(1)).Accept(this, io);
        return null;
    }

    public override object Visit(ASTJavacodeBody node, object data)
    {
        IO io = (IO)data;
        if (node.NodeScope.IsVoid)
        {
            return Visit((TreeNode)node, io);
        }

        Token first = node.FirstToken;

        string indent = "";
        for (int i = 4; i < first.BeginColumn; ++i)
        {
            indent += " ";
        }

        OpenJJTreeComment(io, node.NodeScope.NodeDescriptorText);
        io.WriteLine();
        InsertOpenNodeCode(node.NodeScope, io, indent);
        TryTokenSequence(node.NodeScope, io, indent, first, node.LastToken);
        return null;
    }

    public override object Visit(ASTLHS node, object data)
    {
        IO io = (IO)data;
        var ns = NodeScope.GetEnclosingNodeScope(node);

        /* Print out all the tokens, converting all references to
           `jjtThis' into the current node variable. */
        Token first = node.FirstToken;
        Token last = node.LastToken;
        for (Token t = first; t != last.Next; t = t.Next)
        {
            TokenUtils.Print(t, io, "jjtThis", ns.NodeVariable);
        }

        return null;
    }

    /* This method prints the tokens corresponding to this node
       recursively calling the print methods of its children.
       Overriding this print method in appropriate nodes gives the
       output the added stuff not in the input.  */

    public override object Visit(TreeNode node, object data)
    {
        IO io = (IO)data;
        /* Some productions do not consume any tokens.  In that case their
           first and last tokens are a bit strange. */
        if (node.LastToken.Next == node.FirstToken)
        {
            return null;
        }

        Token t1 = node.FirstToken;
        Token t = new()
        {
            Next = t1
        };
        TreeNode n;
        for (int ord = 0; ord < node.ChildrenCount; ord++)
        {
            n = (TreeNode)node.GetChild(ord);
            while (true)
            {
                t = t.Next;
                if (t == n.FirstToken) break;
                node.Print(t, io);
            }
            n.Accept(this, io);
            t = n.LastToken;
        }
        while (t != node.LastToken)
        {
            t = t.Next;
            node.Print(t, io);
        }

        return null;
    }


    private static void OpenJJTreeComment(IO io, string arg)
    {
        if (arg != null)
        {
            io.Write("/*@bgen(jjtree) " + arg + (CCTreeOptions.BooleanValue(Options.USEROPTION__CPP_IGNORE_ACTIONS) ? "" : " */"));
        }
        else
        {
            io.Write("/*@bgen(jjtree)" + (CCTreeOptions.BooleanValue(Options.USEROPTION__CPP_IGNORE_ACTIONS) ? "" : "*/"));
        }
    }


    private static void CloseJJTreeComment(IO io)
    {
        io.Write((CCTreeOptions.BooleanValue(Options.USEROPTION__CPP_IGNORE_ACTIONS) ? "" : "/*") + "@egen*/");
    }


    string GetIndentation(TreeNode n)
    {
        return GetIndentation(n, 0);
    }


    string GetIndentation(TreeNode n, int offset)
    {
        string s = "";
        for (int i = offset + 1; i < n.FirstToken.BeginColumn; ++i)
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
        string type = ns.nodeDescriptor.GetNodeType();
        string nodeClass;
        if (CCTreeOptions.GetNodeClass().Length > 0 && !CCTreeOptions.GetMulti())
        {
            nodeClass = CCTreeOptions.GetNodeClass();
        }
        else
        {
            nodeClass = type;
        }

        CPPNodeFiles.AddType(type);

        io.Write(indent + nodeClass + " *" + ns.nodeVar + " = ");
        string p = CCTreeOptions.GetStatic() ? "null" : "this";
        string parserArg = CCTreeOptions.GetNodeUsesParser() ? (p + ", ") : "";

        if (CCTreeOptions.GetNodeFactory() == ("*"))
        {
            // Old-style multiple-implementations.
            io.WriteLine("(" + nodeClass + "*)" + nodeClass + "::jjtCreate(" + parserArg +
                ns.nodeDescriptor.GetNodeId() + ");");
        }
        else if (CCTreeOptions.GetNodeFactory().Length > 0)
        {
            io.WriteLine("(" + nodeClass + "*)nodeFactory->jjtCreate(" + parserArg +
             ns.nodeDescriptor.GetNodeId() + ");");
        }
        else
        {
            io.WriteLine("new " + nodeClass + "(" + parserArg + ns.nodeDescriptor.GetNodeId() + ");");
        }

        if (ns.UsesCloseNodeVar)
        {
            io.WriteLine(indent + "bool " + ns.closedVar + " = true;");
        }
        io.WriteLine(indent + ns.nodeDescriptor.OpenNode(ns.nodeVar));
        if (CCTreeOptions.GetNodeScopeHook())
        {
            io.WriteLine(indent + "jjtreeOpenNodeScope(" + ns.nodeVar + ");");
        }

        if (CCTreeOptions.GetTrackTokens())
        {
            io.WriteLine(indent + ns.nodeVar + "->jjtSetFirstToken(getToken(1));");
        }
    }

    void InsertCloseNodeCode(NodeScope ns, IO io, string indent, bool isFinal)
    {
        string closeNode = ns.nodeDescriptor.CloseNode(ns.nodeVar);
        io.WriteLine(indent + closeNode);
        if (ns.UsesCloseNodeVar && !isFinal)
        {
            io.WriteLine(indent + ns.closedVar + " = false;");
        }
        if (CCTreeOptions.GetNodeScopeHook())
        {
            io.WriteLine(indent + "if (jjtree.nodeCreated()) {");
            io.WriteLine(indent + " jjtreeCloseNodeScope(" + ns.nodeVar + ");");
            io.WriteLine(indent + "}");
        }

        if (CCTreeOptions.GetTrackTokens())
        {
            io.WriteLine(indent + ns.nodeVar + "->jjtSetLastToken(getToken(0));");
        }
    }

    void InsertOpenNodeAction(NodeScope ns, IO io, string indent)
    {
        io.WriteLine(indent + "{");
        InsertOpenNodeCode(ns, io, indent + "  ");
        io.WriteLine(indent + "}");
    }


    void InsertCloseNodeAction(NodeScope ns, IO io, string indent)
    {
        io.WriteLine(indent + "{");
        InsertCloseNodeCode(ns, io, indent + "  ", false);
        io.WriteLine(indent + "}");
    }


    private void InsertCatchBlocks(NodeScope ns, IO io, object thrown_names,
           string indent)
    {
        string thrown;
        //if (thrown_names.hasMoreElements()) {
        io.WriteLine(indent + "} catch (...) {"); // " +  ns.exceptionVar + ") {");

        if (ns.UsesCloseNodeVar)
        {
            io.WriteLine(indent + "  if (" + ns.closedVar + ") {");
            io.WriteLine(indent + "    jjtree.clearNodeScope(" + ns.nodeVar + ");");
            io.WriteLine(indent + "    " + ns.closedVar + " = false;");
            io.WriteLine(indent + "  } else {");
            io.WriteLine(indent + "    jjtree.popNode();");
            io.WriteLine(indent + "  }");
        }
        //}

    }

    void TryTokenSequence(NodeScope ns, IO io, string indent, Token first, Token last)
    {
        io.WriteLine(indent + "try {");
        CloseJJTreeComment(io);

        /* Print out all the tokens, converting all references to
           `jjtThis' into the current node variable. */
        for (Token t = first; t != last.Next; t = t.Next)
        {
            TokenUtils.Print(t, io, "jjtThis", ns.nodeVar);
        }

        OpenJJTreeComment(io, null);
        io.WriteLine();

        var thrown_names = ns.production.ThrowsList;
        InsertCatchBlocks(ns, io, thrown_names, indent);

        io.WriteLine(indent + "} {");
        if (ns.UsesCloseNodeVar)
        {
            io.WriteLine(indent + "  if (" + ns.closedVar + ") {");
            InsertCloseNodeCode(ns, io, indent + "    ", true);
            io.WriteLine(indent + "  }");
        }
        io.WriteLine(indent + "}");
        CloseJJTreeComment(io);
    }


    private static void FindThrown(NodeScope ns, Dictionary<string, string> thrown_set,
        TreeNode expansion_unit)
    {
        if (expansion_unit is ASTBNFNonTerminal)
        {
            /* Should really make the nonterminal explicitly maintain its
               name. */
            string nt = expansion_unit.FirstToken.Image;
            if (CCTreeGlobals.Productions.TryGetValue(nt, out var prod))
            {
                foreach (var t in prod.ThrowsList)
                {
                    thrown_set.Add(t, t);
                }
            }
        }
        for (int i = 0; i < expansion_unit.ChildrenCount; ++i)
        {
            TreeNode n = (TreeNode)expansion_unit.GetChild(i);
            FindThrown(ns, thrown_set, n);
        }
    }


    void TryExpansionUnit(NodeScope ns, IO io, string indent, TreeNode expansion_unit)
    {
        io.WriteLine(indent + "try {");
        CloseJJTreeComment(io);

        expansion_unit.Accept(this, io);

        OpenJJTreeComment(io, null);
        io.WriteLine();

        Dictionary<string,string> thrown_set = new ();
        FindThrown(ns, thrown_set, expansion_unit);
        var thrown_names = thrown_set;
        InsertCatchBlocks(ns, io, thrown_names, indent);

        io.WriteLine(indent + "} {");
        if (ns.UsesCloseNodeVar)
        {
            io.WriteLine(indent + "  if (" + ns.closedVar + ") {");
            InsertCloseNodeCode(ns, io, indent + "    ", true);
            io.WriteLine(indent + "  }");
        }
        io.WriteLine(indent + "}");
        CloseJJTreeComment(io);
    }
}
