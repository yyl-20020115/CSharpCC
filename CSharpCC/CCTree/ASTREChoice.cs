namespace CSharpCC.CCTree;

public class ASTREChoice : TreeNode
{
    public ASTREChoice(int id) : base(id) { }
    public ASTREChoice(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data)
       => visitor.Visit(this, data);   
}
