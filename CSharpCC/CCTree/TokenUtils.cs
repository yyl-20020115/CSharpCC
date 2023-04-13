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
using CSharpCC.Parser;
using System.Text;

namespace CSharpCC.CCTree;


/**
 * Utilities for manipulating Tokens.
 */
public static class TokenUtils
{
    public static void Print(Token t, IO io, string _in, string _out)
    {
        Token tt = t.SpecialToken;
        if (tt != null)
        {
            while (tt.SpecialToken != null) tt = tt.SpecialToken;
            while (tt != null)
            {
                io.Write(AddUnicodeEscapes(tt.Image));
                tt = tt.Next;
            }
        }
        string i = t.Image;
        if (_in != null && i == (_in)) {
            i = _out;
        }
        io.Write(AddUnicodeEscapes(i));
    }

    public static string AddUnicodeEscapes(string str)
    {
        var buff = new StringBuilder(str.Length);
        char ch;
        for (int i = 0; i < str.Length; i++)
        {
            ch = str[i];
            if ((ch < 0x20 || ch > 0x7e) && ch != '\t' && ch != '\n' && ch != '\r' && ch != '\f')
            {
                string s = "0000" + Convert.ToString(ch, 16);
                buff.Append("\\u" + s[^4..]);
            }
            else
            {
                buff.Append(ch);
            }
        }
        return buff.ToString();
    }


    public static bool HasTokens(TreeNode n) => n.LastToken.Next != n.FirstToken;

    public static string RemoveEscapesAndQuotes(Token t, string str)
    {
        string retval = "";
        int index = 1;
        char ch, ch1;
        int ordinal;
        while (index < str.Length - 1)
        {
            if (str[index] != '\\')
            {
                retval += str[index]; index++;
                continue;
            }
            index++;
            ch = str[index];
            if (ch == 'b')
            {
                retval += '\b'; index++;
                continue;
            }
            if (ch == 't')
            {
                retval += '\t'; index++;
                continue;
            }
            if (ch == 'n')
            {
                retval += '\n'; index++;
                continue;
            }
            if (ch == 'f')
            {
                retval += '\f'; index++;
                continue;
            }
            if (ch == 'r')
            {
                retval += '\r'; index++;
                continue;
            }
            if (ch == '"')
            {
                retval += '\"'; index++;
                continue;
            }
            if (ch == '\'')
            {
                retval += '\''; index++;
                continue;
            }
            if (ch == '\\')
            {
                retval += '\\'; index++;
                continue;
            }
            if (ch >= '0' && ch <= '7')
            {
                ordinal = ((int)ch) - ((int)'0'); index++;
                ch1 = str[index];
                if (ch1 >= '0' && ch1 <= '7')
                {
                    ordinal = ordinal * 8 + ((int)ch1) - ((int)'0'); index++;
                    ch1 = str[index];
                    if (ch <= '3' && ch1 >= '0' && ch1 <= '7')
                    {
                        ordinal = ordinal * 8 + ((int)ch1) - ((int)'0'); index++;
                    }
                }
                retval += (char)ordinal;
                continue;
            }
            if (ch == 'u')
            {
                index++; ch = str[index];
                if (Hexchar(ch))
                {
                    ordinal = Hexval(ch);
                    index++; ch = str[index];
                    if (Hexchar(ch))
                    {
                        ordinal = ordinal * 16 + Hexval(ch);
                        index++; ch = str[index];
                        if (Hexchar(ch))
                        {
                            ordinal = ordinal * 16 + Hexval(ch);
                            index++; ch = str[index];
                            if (Hexchar(ch))
                            {
                                ordinal = ordinal * 16 + Hexval(ch);
                                index++;
                                continue;
                            }
                        }
                    }
                }
                CSharpCCErrors.ParseError(t, "Encountered non-hex character '" + ch +
                    "' at position " + index + " of string - Unicode escape must have 4 hex digits after it.");
                return retval;
            }
            CSharpCCErrors.ParseError(t, "Illegal escape sequence '\\" + ch + "' at position " + index + " of string.");
            return retval;
        }
        return retval;
    }

    private static bool Hexchar(char ch)
    {
        if (ch >= '0' && ch <= '9') return true;
        if (ch >= 'A' && ch <= 'F') return true;
        if (ch >= 'a' && ch <= 'f') return true;
        return false;
    }

    private static int Hexval(char ch)
    {
        if (ch >= '0' && ch <= '9') return ((int)ch) - ((int)'0');
        if (ch >= 'A' && ch <= 'F') return ((int)ch) - ((int)'A') + 10;
        return ((int)ch) - ((int)'a') + 10;
    }

}

/*end*/
