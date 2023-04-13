namespace CSharpCC.CCTree;

public class ASTREStringLiteral : TreeNode
{
    public ASTREStringLiteral(int id) : base(id) { }
    public ASTREStringLiteral(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data) 
        => visitor.Visit(this, data);
}
