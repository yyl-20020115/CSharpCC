namespace CSharpCC.CCTree;

public class ASTBNFSequence : TreeNode
{
    public ASTBNFSequence(int id) : base(id) { }

    public ASTBNFSequence(CCTreeParser p, int id) : base(p, id) { }

    public override object Accept(TreeParserVisitor visitor, object data) 
        => visitor.Visit(this, data);
}