namespace CSharpCC.CCTree;

/* All AST nodes must implement this interface.  It provides basic
   machinery for constructing the parent and child relationships
   between nodes. */

public interface Node
{

    /** This method is called after the node has been made the current
      node.  It indicates that child nodes can now be added to it. */
    void Open();

    /** This method is called after all the child nodes have been
      added. */
    void Close();

    /** This pair of methods are used to inform the node of its
  parent. */
    Node Parent { get; set; }

    /** This method tells the node to add its argument to the node's
      list of children.  */
    void AddChild(Node n, int i);

    /** This method returns a child node.  The children are numbered
       from zero, left to right. */
    Node GetChild(int i);

    /** Return the number of children the node has. */
    int ChildrenCount { get; }

    /** Accept the visitor. **/
    object Accept(TreeParserVisitor visitor, object data);
}
