/* Generated By:JJTree: Do not edit this line. ASTREChoice.java Version 4.3 */
/* JavaCCOptions:MULTI=true,NODE_USES_PARSER=false,VISITOR=true,TRACK_TOKENS=false,NODE_PREFIX=AST,NODE_EXTENDS=,NODE_FACTORY=,SUPPORT_CLASS_VISIBILITY_PUBLIC=true */
namespace org.javacc.jjtree;

public class ASTREChoice : JJTreeNode
{
    public ASTREChoice(int id) : base(id)
    {
    }

    public ASTREChoice(JJTreeParser p, int id) : base(p, id)
    {
    }


    /** Accept the visitor. **/
    public override object jjtAccept(JJTreeParserVisitor visitor, object data)
    {
        return visitor.visit(this, data);
    }
}
/* JavaCC - OriginalChecksum=ca959d0dc576808b2ba5ef90e0c68c74 (do not edit this line) */
