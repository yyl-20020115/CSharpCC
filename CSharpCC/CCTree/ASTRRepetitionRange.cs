namespace CSharpCC.CCTree;

public class ASTRRepetitionRange : TreeNode
{
    public ASTRRepetitionRange(int id) : base(id) { }
    public ASTRRepetitionRange(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data)
        => visitor.Visit(this, data);
}
