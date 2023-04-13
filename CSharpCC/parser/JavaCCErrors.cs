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
using org.javacc.jjtree;

namespace org.javacc.parser;

/**
 * Output error messages and keep track of totals.
 */
public static class JavaCCErrors
{
    private static int parse_error_count = 0, semantic_error_count = 0, warning_count = 0;

    private static void PrintLocationInfo(object node)
    {
        if (node is NormalProduction)
        {

            NormalProduction n = (NormalProduction)node;
            Console.Error.Write("Line " + n.getLine() + ", Column " + n.getColumn() + ": ");
        }
        else if (node is TokenProduction)
        {
            TokenProduction n = (TokenProduction)node;
            Console.Error.Write("Line " + n.getLine() + ", Column " + n.getColumn() + ": ");
        }
        else if (node is Expansion)
        {
            Expansion n = (Expansion)node;
            Console.Error.Write("Line " + n.GetLine() + ", Column " + n.GetColumn() + ": ");
        }
        else if (node is CharacterRange)
        {
            CharacterRange n = (CharacterRange)node;
            Console.Error.Write("Line " + n.GetLine() + ", Column " + n.GetColumn() + ": ");
        }
        else if (node is SingleCharacter)
        {
            SingleCharacter n = (SingleCharacter)node;
            Console.Error.Write("Line " + n.getLine() + ", Column " + n.getColumn() + ": ");
        }
        else if (node is Token)
        {
            Token t = (Token)node;
            Console.Error.Write("Line " + t.beginLine + ", Column " + t.beginColumn + ": ");
        }
    }

    public static void parse_error(object node, string mess)
    {
        Console.Error.Write("Error: ");
        PrintLocationInfo(node);
        Console.Error.WriteLine(mess);
        parse_error_count++;
    }

    public static void parse_error(string mess)
    {
        Console.Error.Write("Error: ");
        Console.Error.WriteLine(mess);
        parse_error_count++;
    }

    public static int get_parse_error_count()
    {
        return parse_error_count;
    }

    public static void semantic_error(object node, string mess)
    {
        Console.Error.Write("Error: ");
        PrintLocationInfo(node);
        Console.Error.WriteLine(mess);
        semantic_error_count++;
    }

    public static void semantic_error(string mess)
    {
        Console.Error.Write("Error: ");
        Console.Error.WriteLine(mess);
        semantic_error_count++;
    }

    public static int get_semantic_error_count()
    {
        return semantic_error_count;
    }

    public static void warning(object node, string mess)
    {
        Console.Error.Write("Warning: ");
        PrintLocationInfo(node);
        Console.Error.WriteLine(mess);
        warning_count++;
    }

    public static void warning(string mess)
    {
        Console.Error.Write("Warning: ");
        Console.Error.WriteLine(mess);
        warning_count++;
    }

    public static int get_warning_count()
    {
        return warning_count;
    }

    public static int get_error_count()
    {
        return parse_error_count + semantic_error_count;
    }

    public static void reInit()
    {
        parse_error_count = 0;
        semantic_error_count = 0;
        warning_count = 0;
    }

    public static void fatal(string message)
    {
        Console.Error.WriteLine("Fatal Error: " + message);
        throw new RuntimeException("Fatal Error: " + message);
    }
}
