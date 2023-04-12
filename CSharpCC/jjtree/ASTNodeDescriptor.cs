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
namespace org.javacc.jjtree;


public class ASTNodeDescriptor : JJTreeNode
{
    public ASTNodeDescriptor(int id) : base(id)
    {
    }

    private bool faked = false;

    public static ASTNodeDescriptor indefinite(string s)
    {
        var nd = new ASTNodeDescriptor(JJTreeParserTreeConstants.JJTNODEDESCRIPTOR)
        {
            name = s,
            faked = true
        };

        nd.setNodeIdValue();
        return nd;
    }


    public static List<String> nodeIds = new();
    public static List<String> nodeNames = new();
    public static Dictionary<String, String> nodeSeen = new ();

    public static List<String> getNodeIds()
    {
        return nodeIds;
    }

    public static List<String> getNodeNames()
    {
        return nodeNames;
    }

    public void setNodeIdValue()
    {
        var k = getNodeId();
        if (!nodeSeen.ContainsKey(k))
        {
            nodeSeen.Add(k, k);
            nodeNames.Add(name);
            nodeIds.Add(k);
        }
    }

    public string getNodeId()
    {
        return "JJT" + name.ToUpper().Replace('.', '_');
    }


    public string name;
    public bool isGT;
    public ASTNodeDescriptorExpression expression;


    public bool isVoid()
    {
        return name == ("void");
    }

    public override string ToString()
    {
        return faked ? "(faked) " + name : base.ToString() + ": " + name;
    }


    public string getDescriptor()
    {
        return expression == null ? name : "#" + name + "(" + (isGT ? ">" : "") + expression_text() + ")";
    }

    public string getNodeType()
    {
        return JJTreeOptions.getMulti() ? JJTreeOptions.getNodePrefix() + name : "SimpleNode";
    }


    public string getNodeName()
    {
        return name;
    }


    public string openNode(string nodeVar)
    {
        return "jjtree.openNodeScope(" + nodeVar + ");";
    }


    public string expression_text()
    {
        if (expression.getFirstToken().image == (")") &&
          expression.getLastToken().image == ("("))
        {
            return "true";
        }

        string s = "";
        Token t = expression.getFirstToken();
        while (true)
        {
            s += " " + t.image;
            if (t == expression.getLastToken())
            {
                break;
            }
            t = t.next;
        }
        return s;
    }


    public string closeNode(string nodeVar)
    {
        if (expression == null)
        {
            return "jjtree.closeNodeScope(" + nodeVar + ", true);";
        }
        else if (isGT)
        {
            return "jjtree.closeNodeScope(" + nodeVar + ", jjtree.nodeArity() >" +
                expression_text() + ");";
        }
        else
        {
            return "jjtree.closeNodeScope(" + nodeVar + ", " +
                expression_text() + ");";
        }
    }


    public override string translateImage(Token t)
    {
        return whiteOut(t);
    }

    /** Accept the visitor. **/
    public override object jjtAccept(JJTreeParserVisitor visitor, object data)
    {
        return visitor.visit(this, data);
    }

}

/*end*/
