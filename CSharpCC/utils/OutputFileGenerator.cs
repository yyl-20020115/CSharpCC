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
    public void generate(TextWriter _out)
    {
        InputStream _is = getClass().getResourceAsStream(templateName);
        if (_is == null)
            throw new IOException("Invalid template name: " + templateName);
        BufferedReader _in = new BufferedReader(new InputStreamReader(_is));
        process(_in, _out, false);
    }

    private string peekLine(TextReader _in)
    {
        if (currentLine == null)
            currentLine = _in.ReadLine();

        return currentLine;
    }

    private string getLine(TextReader _in)
    {
        string line = currentLine;
        currentLine = null;

        if (line == null)
            _in.ReadLine();

        return line;
    }

    private bool evaluate(string condition)
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

    private string substitute(string text)
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
                value = substituteWithDefault(variableExpression[..i], variableExpression[(i + 2)..]);
                break;
            }
            else if (ch == '?')
            {
                value = substituteWithConditional(variableExpression[..i], variableExpression[(i + 1)..]);
                break;
            }
            else if (ch != '_' && !Character.isJavaIdentifierPart(ch))
            {
                throw new IOException("Invalid variable in " + text);
            }
        }

        if (value == null)
        {
            value = substituteWithDefault(variableExpression, "");
        }

        return text[..startPos] + value + text[endPos..];
    }

    /**
     * @param substring
     * @param defaultValue
     * @return
     * @throws IOException
     */
    private string substituteWithConditional(string variableName, string values)
    {
        // Split values into true and false values.

        int pos = values.IndexOf(':');
        if (pos == -1)
            throw new IOException("No ':' separator in " + values);

        if (evaluate(variableName))
            return substitute(values[..pos]);
        else
            return substitute(values[(pos + 1)..]);
    }

    /**
     * @param variableName
     * @param defaultValue
     * @return
     */
    private string substituteWithDefault(string variableName, string defaultValue)
    {
        Object obj = options.get(variableName.Trim());
        if (obj == null || obj.ToString().Length == 0)
            return substitute(defaultValue);

        return obj.ToString();
    }

    private void write(TextWriter _out, string text)
    {
        while (text.IndexOf("${") != -1)
        {
            text = substitute(text);
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

    private void process(TextReader _in, TextWriter _out, bool ignoring)
    {
        //    _out.WriteLine("*** process ignore=" + ignoring + " : " + peekLine(in));
        while (peekLine(_in) != null)
        {
            if (peekLine(_in).Trim().StartsWith("#if"))
            {
                processIf(_in, _out, ignoring);
            }
            else if (peekLine(_in).Trim().StartsWith("#"))
            {
                break;
            }
            else
            {
                string line = getLine(_in);
                if (!ignoring) write(_out, line);
            }
        }

        _out.Flush();
    }

    private void processIf(TextReader _in, TextWriter _out, bool ignoring)
    {
        string line = getLine(_in).Trim();
        //assert line.Trim().startsWith("#if");
        bool foundTrueCondition = false;

        bool condition = evaluate(line[3..].Trim());
        while (true)
        {
            process(_in, _out, ignoring || foundTrueCondition || !condition);
            foundTrueCondition |= condition;

            if (peekLine(_in) == null || !peekLine(_in).Trim().StartsWith("#elif"))
                break;

            condition = evaluate(getLine(_in).Trim()[5..].Trim());
        }

        if (peekLine(_in) != null && peekLine(_in).Trim().StartsWith("#else"))
        {
            getLine(_in);   // Discard the #else line
            process(_in, _out, ignoring || foundTrueCondition);
        }

        line = getLine(_in);

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

        new OutputFileGenerator(args[0], map).generate(new StreamWriter(args[1]));
    }

    public static void generateFromTemplate(
        string template, Dictionary<String, Object> options,
        string outputFileName)
    {
        OutputFileGenerator gen = new OutputFileGenerator(template, options);
        gen.generate(new StringWriter());
        TextWriter fw = null;
        try
        {
            fw = new StreamWriter(outputFileName);

            fw.Write(fw.ToString());
        }
        finally
        {
            if (fw != null)
            {
                fw.Close();
            }
        }
    }
}
