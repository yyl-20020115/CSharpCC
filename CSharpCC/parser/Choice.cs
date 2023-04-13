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

namespace CSharpCC.Parser;


/**
 * Describes expansions where one of many choices
 * is taken (c1|c2|...).
 */

public class Choice : Expansion
{

    /**
     * The list of choices of this expansion unit.  Each
     * List component will narrow to ExpansionUnit.
     */
    private List<Expansion> choices = new();

    public Choice() { }

    public Choice(Token token)
    {
        this.        Line = token.beginLine;
        this.        Column = token.beginColumn;
    }

    public Choice(Expansion expansion)
    {
        this.        Line = expansion.Line;
        this.        Column = expansion.Column;
        this.GetChoices().Add(expansion);
    }

    /**
     * @param choices the choices to set
     */
    public void SetChoices(List<Expansion> choices)
    {
        this.choices = choices;
    }

    /**
     * @return the choices
     */
    public List<Expansion> GetChoices()
    {
        return choices;
    }

    public override StringBuilder Dump(int indent, HashSet<Expansion> alreadyDumped)
    {
        var sb = base.Dump(indent, alreadyDumped);
        if (alreadyDumped.Contains(this))
            return sb;
        alreadyDumped.Add(this);
        foreach(Expansion next in GetChoices())
        {
            sb.Append(eol).Append(next.Dump(indent + 1, alreadyDumped));
        }
        return sb;
    }
}
