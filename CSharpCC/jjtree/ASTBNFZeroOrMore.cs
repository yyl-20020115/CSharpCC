/* Generated By:JJTree: Do not edit this line. ASTBNFZeroOrMore.java Version 4.3 */
/* JavaCCOptions:MULTI=true,NODE_USES_PARSER=false,VISITOR=true,TRACK_TOKENS=false,NODE_PREFIX=AST,NODE_EXTENDS=,NODE_FACTORY=,SUPPORT_CLASS_VISIBILITY_PUBLIC=true */
namespace org.javacc.jjtree;

public class ASTBNFZeroOrMore : JJTreeNode
{
    public ASTBNFZeroOrMore(int id) : base(id)
    {
    }

    public ASTBNFZeroOrMore(JJTreeParser p, int id) : base(p, id)
    {
    }


    /** Accept the visitor. **/
    public override object jjtAccept(JJTreeParserVisitor visitor, object data)
    {
        return visitor.visit(this, data);
    }
}
/* JavaCC - OriginalChecksum=a70fd123353fa842ff20ab0affae1cd5 (do not edit this line) */
