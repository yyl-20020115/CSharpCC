namespace CSharpCC.CCTree;

public class ASTRESpec : TreeNode
{
    public ASTRESpec(int id) : base(id) { }
    public ASTRESpec(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data) 
        => visitor.Visit(this, data);
}
