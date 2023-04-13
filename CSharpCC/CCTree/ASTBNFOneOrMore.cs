namespace CSharpCC.CCTree;

public class ASTBNFOneOrMore : TreeNode
{
    public ASTBNFOneOrMore(int id) : base(id) { }
    public ASTBNFOneOrMore(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data)
        => visitor.Visit(this, data);
}
