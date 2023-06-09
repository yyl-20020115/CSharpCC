/* Copyright (c) 2006, Tim Pizey
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

namespace CSharpCC.Tests;

/**
 * @author timp
 * @since 7 Mar 2007
 *
 */
[TestClass]
public class CCDocMainTest : CSharpCCTestCase
{
    /**
     * @param name
     */
    public CCDocMainTest(string name)
        : base(name) { }

    /**
     * {@inheritDoc}
     * @see junit.framework.TestCase#setUp()
     */
    protected void SetUp()
    {
        //base.setUp();
    }

    /**
     * {@inheritDoc}
     * @see junit.framework.TestCase#tearDown()
     */
    protected void TearDown()
    {
        //base.tearDown();
    }

    /**
     * Test method for {@link org.javacc.jjdoc.JJDocMain#main(java.lang.String[])}.
     */
    public void TestMain()
    {
    }

    /**
     * Test method for {@link org.javacc.jjdoc.JJDocMain#mainProgram(java.lang.String[])}.
     */
    [TestMethod]
    public void TestMainProgramHTML()
    {
        int result = CCDoc.CCDocMain.MainProgram(
            new string[] { "-OUTPUT_FILE:" + GetCCDocOutputDirectory() + "JavaCC.html", GetCCInputDirectory() + "JavaCC.jj" });
        Assert.AreEqual(0, result);
    }

    /**
     * Test method for {@link org.javacc.jjdoc.JJDocMain#mainProgram(java.lang.String[])}.
     */
    [TestMethod]
    public void TestMainProgramText()
    {
        Assert.AreEqual(0, CCDoc.CCDocMain.MainProgram(new string[] {"-OUTPUT_FILE:" + GetCCDocOutputDirectory() + "JavaCC.txt",
            "-TEXT:true",GetCCInputDirectory() + "JavaCC.jj"}));
    }

}
