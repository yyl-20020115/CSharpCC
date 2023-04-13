namespace CSharpCC.CCTree;

public class ASTRENamed : TreeNode
{
    public ASTRENamed(int id) : base(id) { }
    public ASTRENamed(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data)
        => visitor.Visit(this, data);
}
