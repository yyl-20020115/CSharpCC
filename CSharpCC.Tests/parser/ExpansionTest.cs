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

using CSharpCC.Parser;

namespace CSharpCC.Tests;

[TestClass]
public class ExpansionTest : CSharpCCTestCase
{

    private Token t;
    private Expansion e;

    [TestInitialize]
    public void SetUp()
    {
        t = new Token
        {
            beginColumn = 2,
            beginLine = 3
        };
        e = new Expansion();
        e.        Column = 5;
        e.        Line = 6;
    }
    [TestMethod]
    public void TestZeroOrOneConstructor()
    {
        var zoo = new ZeroOrOne(t, e);
        Assert.AreEqual(t.beginColumn, zoo.Column);
        Assert.AreEqual(t.beginLine, zoo.Line);
        Assert.AreEqual(e, zoo.expansion);
        Assert.AreSame(e.parent, zoo);
    }
    [TestMethod]
    public void TestZeroOrMoreConstructor()
    {
        ZeroOrMore zom = new ZeroOrMore(t, e);
        Assert.AreEqual(t.beginColumn, zom.Column);
        Assert.AreEqual(t.beginLine, zom.Line);
        Assert.AreEqual(e, zom.expansion);
        Assert.AreEqual(e.parent, zom);
    }
    [TestMethod]
    public void TestRZeroOrMoreConstructor()
    {
        RegularExpression r = new RChoice();
        RZeroOrMore rzom = new RZeroOrMore(t, r);
        Assert.AreEqual(t.beginColumn, rzom.Column);
        Assert.AreEqual(t.beginLine, rzom.Line);
        Assert.AreEqual(r, rzom.regexpr);
    }
    [TestMethod]
    public void TestROneOrMoreConstructor()
    {
        RegularExpression r = new RChoice();
        ROneOrMore room = new ROneOrMore(t, r);
        Assert.AreEqual(t.beginColumn, room.Column);
        Assert.AreEqual(t.beginLine, room.Line);
        Assert.AreEqual(r, room.regexpr);
    }
    [TestMethod]
    public void TestOneOrMoreConstructor()
    {
        Expansion rce = new RChoice();
        OneOrMore oom = new OneOrMore(t, rce);
        Assert.AreEqual(t.beginColumn, oom.Column);
        Assert.AreEqual(t.beginLine, oom.Line);
        Assert.AreEqual(rce, oom.expansion);
        Assert.AreEqual(rce.parent, oom);
    }

    [TestMethod]
    public void TestRStringLiteralConstructor()
    {
        RStringLiteral r = new RStringLiteral(t, "hey");
        Assert.AreEqual(t.beginColumn, r.Column);
        Assert.AreEqual(t.beginLine, r.Line);
        Assert.AreEqual("hey", r.image);
    }
    [TestMethod]
    public void TestChoiceConstructor()
    {
        Choice c = new Choice(t);
        Assert.AreEqual(t.beginColumn, c.Column);
        Assert.AreEqual(t.beginLine, c.Line);
        c = new Choice(e);
        Assert.AreEqual(e.Column, c.Column);
        Assert.AreEqual(e.Line, c.Line);
        Assert.AreSame(e, c.GetChoices()[0]);
    }
    [TestMethod]
    public void TestRJustNameConstructor()
    {
        RJustName r = new RJustName(t, "hey");
        Assert.AreEqual(t.beginColumn, r.Column);
        Assert.AreEqual(t.beginLine, r.Line);
        Assert.AreEqual("hey", r.label);
    }
    [TestMethod]
    public void TestSequenceConstructor()
    {
        Lookahead la = new Lookahead();
        Sequence s = new Sequence(t, la);
        Assert.AreEqual(t.beginColumn, s.Column);
        Assert.AreEqual(t.beginLine, s.Line);
        Assert.AreSame(la, s.units[0]);
    }
}
