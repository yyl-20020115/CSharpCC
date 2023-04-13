namespace CSharpCC.CCTree;

public class ASTBNFParenthesized : TreeNode
{
    public ASTBNFParenthesized(int id) : base(id) { }
    public ASTBNFParenthesized(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data) 
        => visitor.Visit(this, data);
}