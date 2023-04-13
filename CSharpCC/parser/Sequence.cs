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

namespace org.javacc.parser;


/**
 * Describes expansions that are sequences of expansion
 * units.  (c1 c2 ...)
 */

public class Sequence : Expansion
{

    /**
     * The list of units in this expansion sequence.  Each
     * List component will narrow to Expansion.
     */
    public List<Expansion> units = new();

    public Sequence() { }

    public Sequence(Token token, Lookahead lookahead)
    {
        this.SetLine(token.beginLine);
        this.SetColumn(token.beginColumn);
        this.units.Add(lookahead);
    }


    public StringBuilder dump(int indent, HashSet<Expansion> alreadyDumped)
    {
        if (alreadyDumped.Contains(this))
        {
            return base.Dump(0, alreadyDumped).Insert(0, '[').Append(']').Insert(0, DumpPrefix(indent));
        }

        alreadyDumped.Add(this);
        var sb = base.Dump(indent, alreadyDumped);
        foreach (Expansion next in units)
        {
            sb.Append(eol).Append(next.Dump(indent + 1, alreadyDumped));
        }
        return sb;
    }
}
