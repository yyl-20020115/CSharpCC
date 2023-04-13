namespace CSharpCC.CCTree;

public class ASTREEOF : TreeNode
{
    public ASTREEOF(int id) : base(id) { }
    public ASTREEOF(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data)
        => visitor.Visit(this, data);
}
