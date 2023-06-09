namespace CSharpCC.CCTree;

public class DefaultCCTreeVisitor : TreeParserVisitor
{
    public virtual object DefaultVisit(SimpleNode node, object data) => node.ChildrenAccept(this, data);
    public virtual object Visit(SimpleNode node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTGrammar node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTCompilationUnit node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTProductions node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTLHS node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTOptions node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTOptionBinding node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTJavacode node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTJavacodeBody node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTBNF node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTBNFDeclaration node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTBNFNodeScope node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTRE node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTTokenDecls node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTRESpec node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTBNFChoice node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTBNFSequence node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTBNFLookahead node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTExpansionNodeScope node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTBNFAction node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTBNFZeroOrOne node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTBNFTryBlock node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTBNFNonTerminal node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTBNFAssignment node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTBNFOneOrMore node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTBNFZeroOrMore node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTBNFParenthesized node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTREStringLiteral node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTRENamed node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTREReference node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTREEOF node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTREChoice node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTRESequence node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTREOneOrMore node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTREZeroOrMore node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTREZeroOrOne node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTRRepetitionRange node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTREParenthesized node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTRECharList node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTCharDescriptor node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTNodeDescriptor node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTNodeDescriptorExpression node, object data) => DefaultVisit(node, data);
    public virtual object Visit(ASTPrimaryExpression node, object data) => DefaultVisit(node, data);
    public virtual object Visit(TreeNode node, object data) => DefaultVisit(node, data);

}
