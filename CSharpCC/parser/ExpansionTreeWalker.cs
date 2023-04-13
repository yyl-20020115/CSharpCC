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
namespace org.javacc.parser;


/**
 * A set of routines that walk down the Expansion tree in
 * various ways.
 */
public static class ExpansionTreeWalker
{
    /**
     * Visits the nodes of the tree rooted at "node" in pre-order.
     * i.e., it executes opObj.action first and then visits the
     * children.
     */
    public static void PreOrderWalk(Expansion node, TreeWalkerOp opObj)
    {
        opObj.Action(node);
        if (opObj.GoDeeper(node))
        {
            if (node is Choice choice)
            {
                foreach(var expansion in choice.GetChoices())
                {
                    PreOrderWalk(expansion, opObj);
                }
            }
            else if (node is Sequence sequence)
            {
                foreach(var expansion in sequence.units)
                {
                    PreOrderWalk(expansion, opObj);
                }
            }
            else if (node is OneOrMore)
            {
                PreOrderWalk(((OneOrMore)node).expansion, opObj);
            }
            else if (node is ZeroOrMore)
            {
                PreOrderWalk(((ZeroOrMore)node).expansion, opObj);
            }
            else if (node is ZeroOrOne)
            {
                PreOrderWalk(((ZeroOrOne)node).expansion, opObj);
            }
            else if (node is Lookahead)
            {
                Expansion nested_e = ((Lookahead)node).getLaExpansion();
                if (!(nested_e is Sequence && (Expansion)(((Sequence)nested_e).units[0]) == node))
                {
                    PreOrderWalk(nested_e, opObj);
                }
            }
            else if (node is TryBlock)
            {
                PreOrderWalk(((TryBlock)node).exp, opObj);
            }
            else if (node is RChoice)
            {
                for (Iterator it = ((RChoice)node).getChoices().iterator(); it.hasNext();)
                {
                    PreOrderWalk((Expansion)it.next(), opObj);
                }
            }
            else if (node is RSequence)
            {
                for (Iterator it = ((RSequence)node).units.iterator(); it.hasNext();)
                {
                    PreOrderWalk((Expansion)it.next(), opObj);
                }
            }
            else if (node is ROneOrMore)
            {
                PreOrderWalk(((ROneOrMore)node).regexpr, opObj);
            }
            else if (node is RZeroOrMore)
            {
                PreOrderWalk(((RZeroOrMore)node).regexpr, opObj);
            }
            else if (node is RZeroOrOne)
            {
                PreOrderWalk(((RZeroOrOne)node).regexpr, opObj);
            }
            else if (node is RRepetitionRange)
            {
                PreOrderWalk(((RRepetitionRange)node).regexpr, opObj);
            }
        }
    }

    /**
     * Visits the nodes of the tree rooted at "node" in post-order.
     * i.e., it visits the children first and then executes
     * opObj.action.
     */
    static void PostOrderWalk(Expansion node, TreeWalkerOp opObj)
    {
        if (opObj.GoDeeper(node))
        {
            if (node is Choice)
            {
                for (Iterator it = ((Choice)node).GetChoices().iterator(); it.hasNext();)
                {
                    PostOrderWalk((Expansion)it.next(), opObj);
                }
            }
            else if (node is Sequence)
            {
                for (Iterator it = ((Sequence)node).units.iterator(); it.hasNext();)
                {
                    PostOrderWalk((Expansion)it.next(), opObj);
                }
            }
            else if (node is OneOrMore)
            {
                PostOrderWalk(((OneOrMore)node).expansion, opObj);
            }
            else if (node is ZeroOrMore)
            {
                PostOrderWalk(((ZeroOrMore)node).expansion, opObj);
            }
            else if (node is ZeroOrOne)
            {
                PostOrderWalk(((ZeroOrOne)node).expansion, opObj);
            }
            else if (node is Lookahead)
            {
                Expansion nested_e = ((Lookahead)node).getLaExpansion();
                if (!(nested_e is Sequence && (Expansion)(((Sequence)nested_e).units[0]) == node))
                {
                    PostOrderWalk(nested_e, opObj);
                }
            }
            else if (node is TryBlock)
            {
                PostOrderWalk(((TryBlock)node).exp, opObj);
            }
            else if (node is RChoice)
            {
                for (Iterator it = ((RChoice)node).getChoices().iterator(); it.hasNext();)
                {
                    PostOrderWalk((Expansion)it.next(), opObj);
                }
            }
            else if (node is RSequence)
            {
                for (Iterator it = ((RSequence)node).units.iterator(); it.hasNext();)
                {
                    PostOrderWalk((Expansion)it.next(), opObj);
                }
            }
            else if (node is ROneOrMore)
            {
                PostOrderWalk(((ROneOrMore)node).regexpr, opObj);
            }
            else if (node is RZeroOrMore)
            {
                PostOrderWalk(((RZeroOrMore)node).regexpr, opObj);
            }
            else if (node is RZeroOrOne)
            {
                PostOrderWalk(((RZeroOrOne)node).regexpr, opObj);
            }
            else if (node is RRepetitionRange)
            {
                PostOrderWalk(((RRepetitionRange)node).regexpr, opObj);
            }
        }
        opObj.Action(node);
    }

}
