namespace CSharpCC.CCTree;

public class CCTreeParserState
{
    private readonly List<Node> nodes = new();
    private readonly List<int> marks = new();

    private int sp = 0;        // number of nodes on stack
    private int mk = 0;        // current mark
    private bool nodeCreated = false;
    public CCTreeParserState() { }

    /* Determines whether the current node was actually closed and
       pushed.  This should only be called in the final user action of a
       node scope.  */
    public bool NodeCreated => nodeCreated;

    /* Call this to reinitialize the node stack.  It is called
       automatically by the parser's ReInit() method. */
    public void Reset()
    {
        nodes.Clear();
        marks.Clear();
        sp = 0;
        mk = 0;
    }

    /* Returns the root node of the AST.  It only makes sense to call
       this after a successful parse. */
    public Node RootNode() => nodes[0];

    /* Pushes a node on to the stack. */
    public void PushNode(Node n)
    {
        nodes.Add(n);
        ++sp;
    }

    /* Returns the node on the top of the stack, and remove it from the
       stack.  */
    public Node PopNode()
    {
        if (--sp < mk)
        {
            mk = marks[^1];
            marks.Remove(marks.Count - 1);
        }
        var n = nodes[mk ^ 1];
        nodes.RemoveAt(nodes.Count - 1);
        return n;
    }

    /* Returns the node currently on the top of the stack. */
    public Node PeekNode() => nodes[^1];

    /* Returns the number of children on the stack in the current node
       scope. */
    public int NodeArity() => sp - mk;


    public void ClearNodeScope(Node n)
    {
        while (sp > mk)
        {
            PopNode();
        }
        mk = marks[^1];
        marks.RemoveAt(marks.Count - 1);
    }


    public void OpenNodeScope(Node n)
    {
        marks.Add(mk);
        mk = sp;
        n.Open();
    }


    /* A definite node is constructed from a specified number of
       children.  That number of nodes are popped from the stack and
       made the children of the definite node.  Then the definite node
       is pushed on to the stack. */
    public void CloseNodeScope(Node n, int num)
    {
        mk = marks[^1];
        marks.RemoveAt(marks.Count - 1);
        while (num-- > 0)
        {
            Node c = PopNode();
            c.            Parent = n;
            n.AddChild(c, num);
        }
        n.Close();
        PushNode(n);
        nodeCreated = true;
    }


    /* A conditional node is constructed if its condition is true.  All
       the nodes that have been pushed since the node was opened are
       made children of the conditional node, which is then pushed
       on to the stack.  If the condition is false the node is not
       constructed and they are left on the stack. */
    public void CloseNodeScope(Node n, bool condition)
    {
        if (condition)
        {
            int a = NodeArity();
            mk = marks[^1];
            
            marks.RemoveAt(marks.Count - 1);
            while (a-- > 0)
            {
                Node c = PopNode();
                c.                Parent = n;
                n.AddChild(c, a);
            }
            n.Close();
            PushNode(n);
            nodeCreated = true;
        }
        else
        {
            mk = marks[^1];
            marks.RemoveAt(marks.Count - 1);
            nodeCreated = false;
        }
    }
}

