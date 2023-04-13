namespace CSharpCC.CCTree;

public class ASTREOneOrMore : TreeNode
{
    public ASTREOneOrMore(int id) : base(id) { }
    public ASTREOneOrMore(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data) 
        => visitor.Visit(this, data);
}
