namespace CSharpCC.CCTree;

public class ASTREZeroOrMore : TreeNode
{
    public ASTREZeroOrMore(int id) : base(id) { }
    public ASTREZeroOrMore(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data)
        => visitor.Visit(this, data);
}
