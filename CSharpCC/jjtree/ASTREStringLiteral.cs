/* Generated By:JJTree: Do not edit this line. ASTREStringLiteral.java Version 4.3 */
/* JavaCCOptions:MULTI=true,NODE_USES_PARSER=false,VISITOR=true,TRACK_TOKENS=false,NODE_PREFIX=AST,NODE_EXTENDS=,NODE_FACTORY=,SUPPORT_CLASS_VISIBILITY_PUBLIC=true */
namespace org.javacc.jjtree;

public class ASTREStringLiteral : JJTreeNode
{
    public ASTREStringLiteral(int id) : base(id)
    {
    }

    public ASTREStringLiteral(JJTreeParser p, int id) : base(p, id)
    {
    }


    /** Accept the visitor. **/
    public override object jjtAccept(JJTreeParserVisitor visitor, object data)
    {
        return visitor.visit(this, data);
    }
}
/* JavaCC - OriginalChecksum=af52a185cb960fce4ed0d5ec45889937 (do not edit this line) */
