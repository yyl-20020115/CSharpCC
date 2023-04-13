namespace CSharpCC.CCTree;

public class ASTRECharList : TreeNode
{
    public ASTRECharList(int id) : base(id) { }

    public ASTRECharList(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data)
        => visitor.Visit(this, data);
}
