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
            else if (node is OneOrMore more)
            {
                PreOrderWalk(more.expansion, opObj);
            }
            else if (node is ZeroOrMore more1)
            {
                PreOrderWalk(more1.expansion, opObj);
            }
            else if (node is ZeroOrOne one)
            {
                PreOrderWalk(one.expansion, opObj);
            }
            else if (node is Lookahead lookahead)
            {
                var nested_e = lookahead.GetLaExpansion();
                if (!(nested_e is Sequence && (Expansion)(((Sequence)nested_e).units[0]) == node))
                {
                    PreOrderWalk(nested_e, opObj);
                }
            }
            else if (node is TryBlock block)
            {
                PreOrderWalk(block.exp, opObj);
            }
            else if (node is RChoice choice1)
            {
                foreach(Expansion e in choice1.getChoices())
                {
                    PreOrderWalk(e, opObj);
                }
            }
            else if (node is RSequence sequence1)
            {
                foreach(Expansion s in sequence1.units)
                {
                    PreOrderWalk(s, opObj);
                }
            }
            else if (node is ROneOrMore more2)
            {
                PreOrderWalk(more2.regexpr, opObj);
            }
            else if (node is RZeroOrMore more3)
            {
                PreOrderWalk(more3.regexpr, opObj);
            }
            else if (node is RZeroOrOne one1)
            {
                PreOrderWalk(one1.regexpr, opObj);
            }
            else if (node is RRepetitionRange range)
            {
                PreOrderWalk(range.regexpr, opObj);
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
            if (node is Choice choice)
            {
                foreach(Expansion c in choice.GetChoices())
                {
                    PostOrderWalk(c, opObj);
                }
            }
            else if (node is Sequence sequence)
            {
                foreach(var u in sequence.units)
                {
                    PostOrderWalk(u, opObj);
                }
            }
            else if (node is OneOrMore more)
            {
                PostOrderWalk(more.expansion, opObj);
            }
            else if (node is ZeroOrMore more1)
            {
                PostOrderWalk(more1.expansion, opObj);
            }
            else if (node is ZeroOrOne one)
            {
                PostOrderWalk(one.expansion, opObj);
            }
            else if (node is Lookahead lookahead)
            {
                Expansion nested_e = lookahead.GetLaExpansion();
                if (!(nested_e is Sequence && (Expansion)(((Sequence)nested_e).units[0]) == node))
                {
                    PostOrderWalk(nested_e, opObj);
                }
            }
            else if (node is TryBlock block)
            {
                PostOrderWalk(block.exp, opObj);
            }
            else if (node is RChoice choice1)
            {
                foreach(Expansion cs in choice1.getChoices())
                {
                    PostOrderWalk(cs, opObj);
                }
            }
            else if (node is RSequence sequence1)
            {
                foreach(Expansion ru in sequence1.units)
                {
                    PostOrderWalk(ru, opObj);
                }
            }
            else if (node is ROneOrMore more3)
            {
                PostOrderWalk(more3.regexpr, opObj);
            }
            else if (node is RZeroOrMore more2)
            {
                PostOrderWalk(more2.regexpr, opObj);
            }
            else if (node is RZeroOrOne one1)
            {
                PostOrderWalk(one1.regexpr, opObj);
            }
            else if (node is RRepetitionRange range)
            {
                PostOrderWalk(range.regexpr, opObj);
            }
        }
        opObj.Action(node);
    }

}
