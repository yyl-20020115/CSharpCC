namespace CSharpCC.CCTree;

public class ASTBNFZeroOrMore : TreeNode
{
    public ASTBNFZeroOrMore(int id) : base(id) { }
    public ASTBNFZeroOrMore(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data) 
        => visitor.Visit(this, data);
}
