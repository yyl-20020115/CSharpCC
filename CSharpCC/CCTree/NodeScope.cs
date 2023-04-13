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


public class NodeScope
{
    public ASTProduction production;
    public ASTNodeDescriptor nodeDescriptor;

    public string closedVar;
    public string exceptionVar;
    public string nodeVar;
    public int scopeNumber;

    public NodeScope(ASTProduction p, ASTNodeDescriptor n)
    {
        production = p;

        if (n == null)
        {
            string nm = production.Name;
            if (CCTreeOptions.GetNodeDefaultVoid())
            {
                nm = "void";
            }
            nodeDescriptor = ASTNodeDescriptor.Indefinite(nm);
        }
        else
        {
            nodeDescriptor = n;
        }

        scopeNumber = production.GetNodeScopeNumber(this);
        nodeVar = ConstructVariable("n");
        closedVar = ConstructVariable("c");
        exceptionVar = ConstructVariable("e");
    }


    public bool IsVoid => nodeDescriptor.IsVoid;


    public ASTNodeDescriptor NodeDescriptor => nodeDescriptor;


    public string NodeDescriptorText => nodeDescriptor.GetDescriptor();


    public string NodeVariable => nodeVar;


    private string ConstructVariable(string id)
    {
        string s = "000" + scopeNumber;
        return "jjt" + id + s[^3..];
    }


    public bool UsesCloseNodeVar => true;

    public static NodeScope GetEnclosingNodeScope(Node node)
    {
        if (node is ASTBNFDeclaration declaration)
        {
            return declaration.NodeScope;
        }
        for (var n = node.Parent; n != null; n = n.Parent)
        {
            if (n is ASTBNFDeclaration declaration2)
            {
                return declaration2.NodeScope;
            }
            else if (n is ASTBNFNodeScope scope)
            {
                return scope.NodeScope;
            }
            else if (n is ASTExpansionNodeScope scope1)
            {
                return scope1.NodeScope;
            }
        }
        return null;
    }
}
