/* Copyright (c) 2006, Sun Microsystems, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *     * Redistributions of source code must retain the above copyright notice,
 *       this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Sun Microsystems, Inc. nor the names of its
 *       contributors may be used to endorse or promote products derived from
 *       this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */
using System.Text;

namespace CSharpCC.CCTree;

public class TreeNode : SimpleNode
{

    private int ordinal;

    public TreeNode(int id) : base(id) { }

    public TreeNode(CCTreeParser p, int i) : this(i) { }

    public static Node Create(int id) => new TreeNode(id);

    public override void AddChild(Node n, int i)
    {
        base.AddChild(n, i);
        (n as TreeNode).Ordinal = i;
    }

    public int Ordinal { get => ordinal; set => ordinal = value; }


    /*****************************************************************
     *
     * The following is added manually to enhance all tree nodes with
     * attributes that store the first and last tokens corresponding to
     * each node, as well as to print the tokens back to the specified
     * output stream.
     *
     *****************************************************************/

    private Token first, last;

    public Token FirstToken { get => first;
        set => first = value; }

    public Token LastToken { get => last;
        set => last = value; }

    public virtual string TranslateImage(Token t) => t.Image;

    public string WhiteOut(Token t)
    {
        var sb = new StringBuilder(t.Image.Length);

        for (int i = 0; i < t.Image.Length; ++i)
        {
            char ch = t.Image[i];
            if (ch != '\t' && ch != '\n' && ch != '\r' && ch != '\f')
            {
                sb.Append(' ');
            }
            else
            {
                sb.Append(ch);
            }
        }

        return sb.ToString();
    }

    /* Indicates whether the token should be replaced by white space or
       replaced with the actual node variable. */
    private bool whitingOut = false;

    public void Print(Token t, IO io)
    {
        Token tt = t.SpecialToken;
        if (tt != null)
        {
            while (tt.SpecialToken != null) tt = tt.SpecialToken;
            while (tt != null)
            {
                io.Write(TokenUtils.AddUnicodeEscapes(TranslateImage(tt)));
                tt = tt.Next;
            }
        }

        /* If we're within a node scope we modify the source in the
           following ways:

           1) we rename all references to `jjtThis' to be references to
           the actual node variable.

           2) we replace all calls to `jjtree.currentNode()' with
           references to the node variable. */

        var s = NodeScope.GetEnclosingNodeScope(this);
        if (s == null)
        {
            /* Not within a node scope so we don't need to modify the
               source. */
            io.Write(TokenUtils.AddUnicodeEscapes(TranslateImage(t)));
            return;
        }

        if (t.Image == ("jjtThis"))
        {
            io.Write(s.NodeVariable);
            return;
        }
        else if (t.Image == ("jjtree"))
        {
            if (t.Next.Image == ("."))
            {
                if (t.Next.Next.Image == ("currentNode"))
                {
                    if (t.Next.Next.Next.Image == ("("))
                    {
                        if (t.Next.Next.Next.Next.Image == (")"))
                        {
                            /* Found `jjtree.currentNode()' so go into white _out
                               mode.  We'll stay in this mode until we find the
                               closing parenthesis. */
                            whitingOut = true;
                        }
                    }
                }
            }
        }
        if (whitingOut)
        {
            if (t.Image == ("jjtree"))
            {
                io.Write(s.NodeVariable);
                io.Write(" ");
            }
            else if (t.Image == (")"))
            {
                io.Write(" ");
                whitingOut = false;
            }
            else
            {
                for (int i = 0; i < t.Image.Length; ++i)
                {
                    io.Write(" ");
                }
            }
            return;
        }

        io.Write(TokenUtils.AddUnicodeEscapes(TranslateImage(t)));
    }
}
