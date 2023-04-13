namespace CSharpCC.CCTree;

public class ASTBNFNonTerminal : TreeNode
{
    public ASTBNFNonTerminal(int id) : base(id) { }

    public ASTBNFNonTerminal(CCTreeParser p, int id) : base(p, id) { }
    public override object Accept(TreeParserVisitor visitor, object data) 
        => visitor.Visit(this, data);
}
