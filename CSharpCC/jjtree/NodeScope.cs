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


public class NodeScope
{
    public ASTProduction production;
    public ASTNodeDescriptor node_descriptor;

    public string closedVar;
    public string exceptionVar;
    public string nodeVar;
    public int scopeNumber;

    public NodeScope(ASTProduction p, ASTNodeDescriptor n)
    {
        production = p;

        if (n == null)
        {
            string nm = production.name;
            if (JJTreeOptions.getNodeDefaultVoid())
            {
                nm = "void";
            }
            node_descriptor = ASTNodeDescriptor.Indefinite(nm);
        }
        else
        {
            node_descriptor = n;
        }

        scopeNumber = production.GetNodeScopeNumber(this);
        nodeVar = constructVariable("n");
        closedVar = constructVariable("c");
        exceptionVar = constructVariable("e");
    }


    public bool isVoid()
    {
        return node_descriptor.IsVoid;
    }


    public ASTNodeDescriptor getNodeDescriptor()
    {
        return node_descriptor;
    }


    public string getNodeDescriptorText()
    {
        return node_descriptor.GetDescriptor();
    }


    public string getNodeVariable()
    {
        return nodeVar;
    }


    private string constructVariable(string id)
    {
        string s = "000" + scopeNumber;
        return "jjt" + id + s[^3..];
    }


    public bool usesCloseNodeVar()
    {
        return true;
    }

    public static NodeScope getEnclosingNodeScope(Node node)
    {
        if (node is ASTBNFDeclaration)
        {
            return ((ASTBNFDeclaration)node).NodeScope;
        }
        for (Node n = node.jjtGetParent(); n != null; n = n.jjtGetParent())
        {
            if (n is ASTBNFDeclaration)
            {
                return ((ASTBNFDeclaration)n).NodeScope;
            }
            else if (n is ASTBNFNodeScope)
            {
                return ((ASTBNFNodeScope)n).NodeScope;
            }
            else if (n is ASTExpansionNodeScope)
            {
                return ((ASTExpansionNodeScope)n).NodeScope;
            }
        }
        return null;
    }

}

/*end*/
