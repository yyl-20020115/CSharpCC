namespace CSharpCC.CCTree;

public class ASTPrimaryExpression : TreeNode
{
    public ASTPrimaryExpression(int id) : base(id) { }
    public ASTPrimaryExpression(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data) 
        => visitor.Visit(this, data);
}
