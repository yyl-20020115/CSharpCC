namespace CSharpCC.CCTree;

public class ASTBNFLookahead : TreeNode
{
    public ASTBNFLookahead(int id) : base(id) { }

    public ASTBNFLookahead(CCTreeParser p, int id) : base(p, id) { }

    /** Accept the visitor. **/
    public override object Accept(TreeParserVisitor visitor, object data) 
        => visitor.Visit(this, data);
}
