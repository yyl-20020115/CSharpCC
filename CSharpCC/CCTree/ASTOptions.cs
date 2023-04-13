namespace CSharpCC.CCTree;

public class ASTOptions : TreeNode
{
    public ASTOptions(int id) : base(id) { }

    public ASTOptions(CCTreeParser p, int id) : base(p, id) { }

    public override object Accept(TreeParserVisitor visitor, object data)
        => visitor.Visit(this, data);
}
