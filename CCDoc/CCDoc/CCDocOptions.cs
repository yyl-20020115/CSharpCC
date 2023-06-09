/* Copyright (c) 2005-2006, Kees Jan Koster kjkoster@kjkoster.org
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

namespace CSharpCC.CCDoc;


/**
 * The options, specific to JJDoc.
 *
 * @author Kees Jan Koster &lt;kjkoster@kjkoster.org&gt;
 */
public class CCDocOptions : Options
{

    /**
     * Limit subclassing to derived classes.
     */
    protected CCDocOptions()
        : base() { }

    /**
     * Initialize the options.
     */
    public new static void Init()
    {
        Options.Init();

        Options.OptionValues.Add("ONE_TABLE", true);
        Options.OptionValues.Add("TEXT", false);
        Options.OptionValues.Add("XTEXT", false);
        Options.OptionValues.Add("BNF", false);

        Options.OptionValues.Add("OUTPUT_FILE", "");
        Options.OptionValues.Add("CSS", "");
    }

    /**
     * Find the one table value.
     *
     * @return The requested one table value.
     */
    public static bool GetOneTable() => BooleanValue("ONE_TABLE");

    /**
     * Find the CSS value.
     *
     * @return The requested CSS value.
     */
    public static string GetCSS() => StringValue("CSS");

    /**
     * Find the text value.
     *
     * @return The requested text value.
     */
    public static bool GetText() => BooleanValue("TEXT");

    public static bool GetXText() => BooleanValue("XTEXT");

    /**
     * Find the BNF value.
     *
     * @return The requested text value.
     */
    public static bool GetBNF() => BooleanValue("BNF");

    /**
     * Find the output file value.
     *
     * @return The requested output value.
     */
    public static string GetOutputFile() => StringValue("OUTPUT_FILE");
}
