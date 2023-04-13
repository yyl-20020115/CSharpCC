namespace CSharpCC.CCTree;

public class ASTBNFTryBlock : TreeNode
{
    public ASTBNFTryBlock(int id) : base(id) { }
    public ASTBNFTryBlock(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data) 
        => visitor.Visit(this, data);
}
