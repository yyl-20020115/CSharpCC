namespace CSharpCC.CCTree;

public class ASTREParenthesized : TreeNode
{
    public ASTREParenthesized(int id) : base(id) { }
    public ASTREParenthesized(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data) 
        => visitor.Visit(this, data);
}
