namespace CSharpCC.CCTree;

public class ASTREZeroOrOne : TreeNode
{
    public ASTREZeroOrOne(int id) : base(id) { }
    public ASTREZeroOrOne(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data)
        => visitor.Visit(this, data);
}
