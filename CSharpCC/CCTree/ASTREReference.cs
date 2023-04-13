namespace CSharpCC.CCTree;

public class ASTREReference : TreeNode
{
    public ASTREReference(int id) : base(id) { }
    public ASTREReference(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data)
        => visitor.Visit(this, data);
}
