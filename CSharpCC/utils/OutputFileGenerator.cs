/* Copyright (c) 2008, Paul Cager.
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

using org.javacc.parser;

namespace org.javacc.utils;



/**
 * Generates boiler-plate files from templates. Only very basic
 * template processing is supplied - if we need something more
 * sophisticated I suggest we use a third-party library.
 *
 * @author paulcager
 * @since 4.2
 */
public class OutputFileGenerator
{

    /**
     * @param templateName the name of the template. E.g.
     *        "/templates/Token.template".
     * @param options the processing options in force, such
     *        as "STATIC=yes"
     */
    public OutputFileGenerator(string templateName, Dictionary<string,object> options)
    {
        this.templateName = templateName;
        this.options = options;
    }

    private string templateName;
    private Dictionary<string, object> options;

    private string currentLine;

    /**
     * Generate the output file.
     * @param out
     * @throws IOException
     */
    public void Generate(TextWriter _out)
    {
        //NOTICE:should use template name as input file
        Process(Console.In, _out, false);
    }

    private string PeekLine(TextReader _in) => currentLine ??= _in.ReadLine();

    private string GetLine(TextReader _in)
    {
        string line = currentLine;
        currentLine = null;

        if (line == null)
            _in.ReadLine();

        return line;
    }

    private bool Evaluate(string condition)
    {
        condition = condition.Trim();

        try
        {
            return new ConditionParser(new StringReader(condition)).CompilationUnit(options);
        }
        catch (ParseException e)
        {
            return false;
        }
    }
    public static bool IsJavaIdentifierPart(char t)
    {
        return t=='_'||char.IsLetterOrDigit(t);
    }
    private string Substitute(string text)
    {
        int startPos;

        if ((startPos = text.IndexOf("${")) == -1)
        {
            return text;
        }

        // Find matching "}".
        int braceDepth = 1;
        int endPos = startPos + 2;

        while (endPos < text.Length && braceDepth > 0)
        {
            if (text[endPos] == '{')
                braceDepth++;
            else if (text[endPos] == '}')
                braceDepth--;

            endPos++;
        }

        if (braceDepth != 0)
            throw new IOException("Mismatched \"{}\" in template string: " + text);

        string variableExpression = text.Substring(startPos + 2, endPos - 1);

        // Find the end of the variable name
        string value = null;

        for (int i = 0; i < variableExpression.Length; i++)
        {
            char ch = variableExpression[i];

            if (ch == ':' && i < variableExpression.Length - 1 && variableExpression[i + 1] == '-')
            {
                value = SubstituteWithDefault(variableExpression[..i], variableExpression[(i + 2)..]);
                break;
            }
            else if (ch == '?')
            {
                value = SubstituteWithConditional(variableExpression[..i], variableExpression[(i + 1)..]);
                break;
            }
            else if (ch != '_' && !IsJavaIdentifierPart(ch))
            {
                throw new IOException("Invalid variable in " + text);
            }
        }

        value ??= SubstituteWithDefault(variableExpression, "");

        return text[..startPos] + value + text[endPos..];
    }

    /**
     * @param substring
     * @param defaultValue
     * @return
     * @throws IOException
     */
    private string SubstituteWithConditional(string variableName, string values)
    {
        // Split values into true and false values.

        int pos = values.IndexOf(':');
        if (pos == -1)
            throw new IOException("No ':' separator in " + values);

        if (Evaluate(variableName))
            return Substitute(values[..pos]);
        else
            return Substitute(values[(pos + 1)..]);
    }

    /**
     * @param variableName
     * @param defaultValue
     * @return
     */
    private string SubstituteWithDefault(string variableName, string defaultValue)
    {
        if (!options.TryGetValue(variableName.Trim(),out var obj) || obj.ToString().Length == 0)
            return Substitute(defaultValue);

        return obj.ToString();
    }

    private void Write(TextWriter _out, string text)
    {
        while (text.IndexOf("${") != -1)
        {
            text = Substitute(text);
        }

        if (Options.isOutputLanguageJava() && Options.getGenerateStringBuilder())
        {
            text = text.Replace("StringBuilder", "StringBuilder");
        }

        // TODO :: Added by Sreenivas on 12 June 2013 for 6.0 release, merged in to 6.1 release for sake of compatibility by cainsley ... This needs to be removed urgently!!!
        if (text.StartsWith("\\#"))
        { // Hack to escape # for C++
            text = text[1..];
        }
        _out.WriteLine(text);
    }

    private void Process(TextReader _in, TextWriter _out, bool ignoring)
    {
        //    _out.WriteLine("*** process ignore=" + ignoring + " : " + peekLine(in));
        while (PeekLine(_in) != null)
        {
            if (PeekLine(_in).Trim().StartsWith("#if"))
            {
                ProcessIf(_in, _out, ignoring);
            }
            else if (PeekLine(_in).Trim().StartsWith("#"))
            {
                break;
            }
            else
            {
                string line = GetLine(_in);
                if (!ignoring) Write(_out, line);
            }
        }

        _out.Flush();
    }

    private void ProcessIf(TextReader _in, TextWriter _out, bool ignoring)
    {
        string line = GetLine(_in).Trim();
        //assert line.Trim().StartsWith("#if");
        bool foundTrueCondition = false;

        bool condition = Evaluate(line[3..].Trim());
        while (true)
        {
            Process(_in, _out, ignoring || foundTrueCondition || !condition);
            foundTrueCondition |= condition;

            if (PeekLine(_in) == null || !PeekLine(_in).Trim().StartsWith("#elif"))
                break;

            condition = Evaluate(GetLine(_in).Trim()[5..].Trim());
        }

        if (PeekLine(_in) != null && PeekLine(_in).Trim().StartsWith("#else"))
        {
            GetLine(_in);   // Discard the #else line
            Process(_in, _out, ignoring || foundTrueCondition);
        }

        line = GetLine(_in);

        if (line == null)
            throw new IOException("Missing \"#fi\"");

        if (!line.Trim().StartsWith("#fi"))
            throw new IOException("Expected \"#fi\", got: " + line);
    }


    public static void main(String[] args)
    {
        Dictionary<string, object> map = new ();
        map.Add("falseArg", false);
        map.Add("trueArg", true);
        map.Add("stringValue", "someString");

        new OutputFileGenerator(args[0], map).Generate(new StreamWriter(args[1]));
    }

    public static void GenerateFromTemplate(
        string template, Dictionary<String, Object> options,
        string outputFileName)
    {
        OutputFileGenerator gen = new OutputFileGenerator(template, options);
        gen.Generate(new StringWriter());
        TextWriter fw = null;
        try
        {
            fw = new StreamWriter(outputFileName);

            fw.Write(fw.ToString());
        }
        finally
        {
            fw?.Close();
        }
    }
}
