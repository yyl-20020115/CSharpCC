/* Generated By:JJTree: Do not edit this line. ASTBNFAssignment.java Version 4.3 */
/* JavaCCOptions:MULTI=true,NODE_USES_PARSER=false,VISITOR=true,TRACK_TOKENS=false,NODE_PREFIX=AST,NODE_EXTENDS=,NODE_FACTORY=,SUPPORT_CLASS_VISIBILITY_PUBLIC=true */
namespace CSharpCC.CCTree;

public class ASTBNFAssignment : TreeNode
{
    public ASTBNFAssignment(int id) : base(id)
    {
    }

    public ASTBNFAssignment(CCTreeParser p, int id) : base(p, id)
    {
    }


    /** Accept the visitor. **/
    public override object Accept(TreeParserVisitor visitor, object data)
        => visitor.Visit(this, data);
}