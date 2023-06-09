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
namespace CSharpCC.CCTree;

public class ASTNodeDescriptor : TreeNode
{
    public ASTNodeDescriptor(int id) : base(id) { }
    private bool faked = false;

    public static ASTNodeDescriptor Indefinite(string s)
    {
        var nd = new ASTNodeDescriptor(CCTreeParserTreeConstants.JJTNODEDESCRIPTOR)
        {
            name = s,
            faked = true
        };

        nd.SetNodeIdValue();
        return nd;
    }

    public static List<string> NodeIds = new();
    public static List<string> NodeNames = new();
    public static Dictionary<string, string> NodeSeen = new();

    public static List<string> GetNodeIds() => NodeIds;

    public static List<string> GetNodeNames() => NodeNames;

    public void SetNodeIdValue()
    {
        var k = GetNodeId();
        if (!NodeSeen.ContainsKey(k))
        {
            NodeSeen.Add(k, k);
            NodeNames.Add(name);
            NodeIds.Add(k);
        }
    }

    public string GetNodeId() => "JJT" + name.ToUpper().Replace('.', '_');


    public string name;
    public bool isGT;
    public ASTNodeDescriptorExpression expression;


    public bool IsVoid => name == ("void");

    public override string ToString() => faked ? "(faked) " + name : base.ToString() + ": " + name;


    public string GetDescriptor() => expression == null ? name : $"#{name}({(isGT ? ">" : "")}{ExpressionText()})";

    public string GetNodeType() => CCTreeOptions.GetMulti() ? CCTreeOptions.GetNodePrefix() + name : "SimpleNode";

    public string GetNodeName() => name;

    public string OpenNode(string nodeVar) => $"jjtree.openNodeScope({nodeVar});";


    public string ExpressionText()
    {
        if (expression.FirstToken.Image == (")") &&
          expression.          LastToken.Image == ("("))
        {
            return "true";
        }

        string s = "";
        Token t = expression.FirstToken;
        while (true)
        {
            s += " " + t.Image;
            if (t == expression.LastToken)
            {
                break;
            }
            t = t.Next;
        }
        return s;
    }


    public string CloseNode(string nodeVar)
    {
        if (expression == null)
        {
            return "jjtree.closeNodeScope(" + nodeVar + ", true);";
        }
        else if (isGT)
        {
            return "jjtree.closeNodeScope(" + nodeVar + ", jjtree.nodeArity() >" +
                ExpressionText() + ");";
        }
        else
        {
            return "jjtree.closeNodeScope(" + nodeVar + ", " +
                ExpressionText() + ");";
        }
    }


    public override string TranslateImage(Token t) => WhiteOut(t);

    public override object Accept(TreeParserVisitor visitor, object data) 
        => visitor.Visit(this, data);

}