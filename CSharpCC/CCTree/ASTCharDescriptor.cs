namespace CSharpCC.CCTree;

public class ASTCharDescriptor : TreeNode
{
    public ASTCharDescriptor(int id) : base(id) { }
    public ASTCharDescriptor(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data)
        => visitor.Visit(this, data);
}
