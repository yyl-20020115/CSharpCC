namespace CSharpCC.CCTree;

public interface TreeParserVisitor
{
    public object Visit(SimpleNode node, object data);
    public object Visit(ASTGrammar node, object data);
    public object Visit(ASTCompilationUnit node, object data);
    public object Visit(ASTProductions node, object data);
    public object Visit(ASTLHS node, object data);
    public object Visit(ASTOptions node, object data);
    public object Visit(ASTOptionBinding node, object data);
    public object Visit(ASTJavacode node, object data);
    public object Visit(ASTJavacodeBody node, object data);
    public object Visit(ASTBNF node, object data);
    public object Visit(ASTBNFDeclaration node, object data);
    public object Visit(ASTBNFNodeScope node, object data);
    public object Visit(ASTRE node, object data);
    public object Visit(ASTTokenDecls node, object data);
    public object Visit(ASTRESpec node, object data);
    public object Visit(ASTBNFChoice node, object data);
    public object Visit(ASTBNFSequence node, object data);
    public object Visit(ASTBNFLookahead node, object data);
    public object Visit(ASTExpansionNodeScope node, object data);
    public object Visit(ASTBNFAction node, object data);
    public object Visit(ASTBNFZeroOrOne node, object data);
    public object Visit(ASTBNFTryBlock node, object data);
    public object Visit(ASTBNFNonTerminal node, object data);
    public object Visit(ASTBNFAssignment node, object data);
    public object Visit(ASTBNFOneOrMore node, object data);
    public object Visit(ASTBNFZeroOrMore node, object data);
    public object Visit(ASTBNFParenthesized node, object data);
    public object Visit(ASTREStringLiteral node, object data);
    public object Visit(ASTRENamed node, object data);
    public object Visit(ASTREReference node, object data);
    public object Visit(ASTREEOF node, object data);
    public object Visit(ASTREChoice node, object data);
    public object Visit(ASTRESequence node, object data);
    public object Visit(ASTREOneOrMore node, object data);
    public object Visit(ASTREZeroOrMore node, object data);
    public object Visit(ASTREZeroOrOne node, object data);
    public object Visit(ASTRRepetitionRange node, object data);
    public object Visit(ASTREParenthesized node, object data);
    public object Visit(ASTRECharList node, object data);
    public object Visit(ASTCharDescriptor node, object data);
    public object Visit(ASTNodeDescriptor node, object data);
    public object Visit(ASTNodeDescriptorExpression node, object data);
    public object Visit(ASTPrimaryExpression node, object data);
    public object Visit(TreeNode node, object data);
}   
