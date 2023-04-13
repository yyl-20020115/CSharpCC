namespace CSharpCC.CCTree;

public class SimpleNode : Node
{
    protected Node parent;
    protected Node[] children;
    protected int id;
    protected object value;
    protected CCTreeParser parser;

    public SimpleNode(int i) => id = i;

    public SimpleNode(CCTreeParser p, int i)
        : this(i) => parser = p;

    public virtual void Open() { }

    public virtual void Close() { }

    public virtual Node Parent { get => parent; set => parent = value; }

    public virtual void AddChild(Node n, int i)
    {
        if (children == null)
        {
            children = new Node[i + 1];
        }
        else if (i >= children.Length)
        {
            var c = new Node[i + 1];
            Array.Copy(children, 0, c, 0, children.Length);
            children = c;
        }
        children[i] = n;
    }

    public virtual Node GetChild(int i) => children[i];

    public virtual int ChildrenCount => (children == null) ? 0 : children.Length;

    public virtual object Value { get => value; set => this.value = value; }

    /** Accept the visitor. **/
    public virtual object Accept(TreeParserVisitor visitor, object data) 
        => visitor.Visit(this, data);

    /** Accept the visitor. **/
    public virtual object ChildrenAccept(TreeParserVisitor visitor, object data)
    {
        if (children != null)
        {
            for (int i = 0; i < children.Length; ++i)
            {
                children[i].Accept(visitor, data);
            }
        }
        return data;
    }

    /* You can override these two methods in subclasses of SimpleNode to
       customize the way the node appears when the tree is dumped.  If
       your output uses more than one line you should override
       toString(String), otherwise overriding toString() is probably all
       you need to do. */

    public override string ToString() => CCTreeParserTreeConstants.jjtNodeName[id];
    public virtual string ToString(string prefix) => prefix + ToString();

    /* Override this method if you want to customize how the node dumps
       out its children. */

    public void Dump(string prefix)
    {
        Console.WriteLine(ToString(prefix));
        if (children != null)
        {
            for (int i = 0; i < children.Length; ++i)
            {
                var n = children[i] as SimpleNode;
                n?.Dump(prefix + " ");
            }
        }
    }
}

