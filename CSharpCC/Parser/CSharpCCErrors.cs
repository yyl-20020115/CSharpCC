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
using CSharpCC.CCTree;

namespace CSharpCC.Parser;

/**
 * Output error messages and keep track of totals.
 */
public static class CSharpCCErrors
{
    private static int pec = 0, sec = 0, wc = 0;

    private static void PrintLocationInfo(object node)
    {
        switch (node)
        {
            case NormalProduction n:
                {
                    Console.Error.Write("Line " + n.Line + ", Column " + n.Column + ": ");
                    break;
                }

            case TokenProduction n:
                {
                    Console.Error.Write("Line " + n.Line + ", Column " + n.Column + ": ");
                    break;
                }

            case Expansion n:
                {
                    Console.Error.Write("Line " + n.Line + ", Column " + n.Column + ": ");
                    break;
                }

            case CharacterRange n:
                {
                    Console.Error.Write("Line " + n.Line + ", Column " + n.Column + ": ");
                    break;
                }

            case SingleCharacter n:
                {
                    Console.Error.Write("Line " + n.Line + ", Column " + n.Column + ": ");
                    break;
                }

            case Token t:
                {
                    Console.Error.Write("Line " + t.beginLine + ", Column " + t.beginColumn + ": ");
                    break;
                }
        }
    }

    public static void ParseError(object node, string mess)
    {
        Console.Error.Write("Error: ");
        PrintLocationInfo(node);
        Console.Error.WriteLine(mess);
        pec++;
    }

    public static void ParseError(string mess)
    {
        Console.Error.Write("Error: ");
        Console.Error.WriteLine(mess);
        pec++;
    }

    public static int ParseErrorCount => pec;

    public static void SemanticError(object node, string mess)
    {
        Console.Error.Write("Error: ");
        PrintLocationInfo(node);
        Console.Error.WriteLine(mess);
        sec++;
    }

    public static void SemanticError(string mess)
    {
        Console.Error.Write("Error: ");
        Console.Error.WriteLine(mess);
        sec++;
    }

    public static int GetSemanticErrorCount() => sec;

    public static void Warning(object node, string mess)
    {
        Console.Error.Write("Warning: ");
        PrintLocationInfo(node);
        Console.Error.WriteLine(mess);
        wc++;
    }

    public static void Warning(string mess)
    {
        Console.Error.Write("Warning: ");
        Console.Error.WriteLine(mess);
        wc++;
    }

    public static int WarningCount => wc;

    public static int ErrorCount => pec + sec;

    public static void ReInit()
    {
        pec = 0;
        sec = 0;
        wc = 0;
    }

    public static void Fatal(string message)
    {
        Console.Error.WriteLine("Fatal Error: " + message);
        throw new RuntimeException("Fatal Error: " + message);
    }
}
