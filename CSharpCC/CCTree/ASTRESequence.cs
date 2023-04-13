namespace CSharpCC.CCTree;

public class ASTRESequence : TreeNode
{
    public ASTRESequence(int id) : base(id) { }
    public ASTRESequence(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data)
        => visitor.Visit(this, data);
}
