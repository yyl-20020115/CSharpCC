namespace CSharpCC.CCTree;

public class ASTTokenDecls : TreeNode
{
    public ASTTokenDecls(int id) : base(id) { }
    public ASTTokenDecls(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data) 
        => visitor.Visit(this, data);
}
