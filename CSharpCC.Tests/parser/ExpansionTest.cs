/* Copyright (c) 2007, Sun Microsystems, Inc.
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


public class ExpansionTest: CSharpCCTestCase
{

    private Token t;
    private Expansion e;

    public void setUp() {
        t = new Token();
        t.beginColumn = 2;
        t.beginLine = 3;
        e = new Expansion();
        e.setColumn(5);
        e.setLine(6);
    }
    public void testZeroOrOneConstructor() {
        ZeroOrOne zoo = new ZeroOrOne(t, e);
        Assert.AreEqual(t.beginColumn, zoo.getColumn());
        Assert.AreEqual(t.beginLine, zoo.getLine());
        Assert.AreEqual(e, zoo.expansion);
        Assert.AreSame(e.parent, zoo);
    }

    public void testZeroOrMoreConstructor() {
        ZeroOrMore zom = new ZeroOrMore(t, e);
        Assert.AreEqual(t.beginColumn, zom.getColumn());
        Assert.AreEqual(t.beginLine, zom.getLine());
        Assert.AreEqual(e, zom.expansion);
        Assert.AreEqual(e.parent, zom);
    }

    public void testRZeroOrMoreConstructor() {
        RegularExpression r = new RChoice();
        RZeroOrMore rzom = new RZeroOrMore(t, r);
        Assert.AreEqual(t.beginColumn, rzom.getColumn());
        Assert.AreEqual(t.beginLine, rzom.getLine());
        Assert.AreEqual(r, rzom.regexpr);
    }

    public void testROneOrMoreConstructor() {
        RegularExpression r = new RChoice();
        ROneOrMore room = new ROneOrMore(t, r);
        Assert.AreEqual(t.beginColumn, room.getColumn());
        Assert.AreEqual(t.beginLine, room.getLine());
        Assert.AreEqual(r, room.regexpr);
    }

    public void testOneOrMoreConstructor() {
        Expansion rce = new RChoice();
        OneOrMore oom = new OneOrMore(t, rce);
        Assert.AreEqual(t.beginColumn, oom.getColumn());
        Assert.AreEqual(t.beginLine, oom.getLine());
        Assert.AreEqual(rce, oom.expansion);
        Assert.AreEqual(rce.parent, oom);
    }


    public void testRStringLiteralConstructor() {
        RStringLiteral r = new RStringLiteral(t, "hey");
        Assert.AreEqual(t.beginColumn, r.getColumn());
        Assert.AreEqual(t.beginLine, r.getLine());
        Assert.AreEqual("hey", r.image);
    }

    public void testChoiceConstructor() {
        Choice c = new Choice(t);
        Assert.AreEqual(t.beginColumn, c.getColumn());
        Assert.AreEqual(t.beginLine, c.getLine());
        c = new Choice(e);
        Assert.AreEqual(e.getColumn(), c.getColumn());
        Assert.AreEqual(e.getLine(), c.getLine());
        Assert.AreSame(e, c.getChoices()[0]);
    }

    public void testRJustNameConstructor() {
        RJustName r = new RJustName(t, "hey");
        Assert.AreEqual(t.beginColumn, r.getColumn());
        Assert.AreEqual(t.beginLine, r.getLine());
        Assert.AreEqual("hey", r.label);
    }

    public void testSequenceConstructor() {
        Lookahead la = new Lookahead();
        Sequence s = new Sequence(t, la);
        Assert.AreEqual(t.beginColumn, s.getColumn());
        Assert.AreEqual(t.beginLine, s.getLine());
        Assert.AreSame(la, s.units[0]);
    }
}
