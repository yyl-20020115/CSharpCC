namespace CSharpCC.CCTree;

public class ASTBNFZeroOrOne : TreeNode
{
    public ASTBNFZeroOrOne(int id) : base(id) { }
    public ASTBNFZeroOrOne(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data) 
        => visitor.Visit(this, data);
}