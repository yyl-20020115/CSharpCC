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

namespace org.javacc.utils;



/**
 * Generates boiler-plate files from templates. Only very basic
 * template processing is supplied - if we need something more
 * sophisticated I suggest we use a third-party library.
 *
 * @author paulcager
 * @since 4.2
 */
public class OutputFileGenerator {

  /**
   * @param templateName the name of the template. E.g.
   *        "/templates/Token.template".
   * @param options the processing options in force, such
   *        as "STATIC=yes"
   */
  public OutputFileGenerator(string templateName, Dictionary options) {
    this.templateName = templateName;
    this.options = options;
  }

  private string templateName;
  private Dictionary options;

  private string currentLine;

  /**
   * Generate the output file.
   * @param out
   * @throws IOException
   */
  public void generate(TextWriter _out)
  {
    InputStream is = getClass().getResourceAsStream(templateName);
    if (is == null)
      throw new IOException("Invalid template name: " + templateName);
    BufferedReader in = new BufferedReader(new InputStreamReader(is));
    process(in, _out, false);
  }

  private string peekLine(BufferedReader in)
  {
    if (currentLine == null)
      currentLine = in.readLine();

    return currentLine;
  }

  private string getLine(BufferedReader in)
  {
    string line = currentLine;
    currentLine = null;

    if (line == null)
      in.readLine();

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

    if ( (startPos = text.indexOf("${")) == -1)
    {
      return text;
    }

    // Find matching "}".
    int braceDepth = 1;
    int endPos = startPos + 2;

    while ( endPos < text.Length && braceDepth > 0)
    {
      if (text.charAt(endPos) == '{')
        braceDepth++;
      else if (text.charAt(endPos) == '}')
        braceDepth--;

      endPos++;
    }

    if (braceDepth != 0)
      throw new IOException("Mismatched \"{}\" in template string: " + text);

    string variableExpression = text.substring(startPos + 2, endPos - 1);

    // Find the end of the variable name
    string value = null;

    for (int i = 0; i < variableExpression.Length; i++)
    {
      char ch = variableExpression.charAt(i);

      if (ch == ':' && i < variableExpression.Length - 1 && variableExpression.charAt(i+1) == '-' )
      {
        value = substituteWithDefault(variableExpression.substring(0, i), variableExpression.substring(i + 2));
        break;
      }
      else if (ch == '?')
      {
        value = substituteWithConditional(variableExpression.substring(0, i), variableExpression.substring(i + 1));
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

    return text.substring(0, startPos) + value + text.substring(endPos);
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

    int pos = values.indexOf(':');
    if (pos == -1)
      throw new IOException("No ':' separator in " + values);

    if (evaluate(variableName))
      return substitute(values.substring(0, pos));
    else
      return substitute(values.substring(pos + 1));
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
    while ( text.indexOf("${") != -1)
    {
    text = substitute(text);
    }

    if (Options.isOutputLanguageJava() && Options.getGenerateStringBuilder()) {
        text = text.Replace("StringBuilder", "StringBuilder");
    }

    // TODO :: Added by Sreenivas on 12 June 2013 for 6.0 release, merged in to 6.1 release for sake of compatibility by cainsley ... This needs to be removed urgently!!!
    if (text.startsWith("\\#")) { // Hack to escape # for C++
      text = text.substring(1);
    }
    _out.WriteLine(text);
  }

  private void process(BufferedReader in, TextWriter _out, bool ignoring)
      {
    //    _out.WriteLine("*** process ignore=" + ignoring + " : " + peekLine(in));
    while ( peekLine(in) != null)
    {
      if (peekLine(in).Trim().startsWith("#if"))
      {
        processIf(in, out, ignoring);
      }
      else if (peekLine(in).Trim().startsWith("#"))
      {
        break;
      }
      else
      {
        string line = getLine(in);
        if (!ignoring) write(_out, line);
      }
    }

    _out.flush();
  }

  private void processIf(BufferedReader in, TextWriter out, bool ignoring) 
  {
        string line = getLine(in).Trim();
    assert line.Trim().startsWith("#if");
    bool foundTrueCondition = false;

    bool condition = evaluate(line.substring(3).Trim());
    while (true)
    {
      process(in, _out, ignoring || foundTrueCondition || !condition);
      foundTrueCondition |= condition;

      if (peekLine(in) == null || !peekLine(in).Trim().startsWith("#elif"))
        break;

      condition = evaluate(getLine(in).Trim().substring(5).Trim());
    }

        if (peekLine(in) != null && peekLine(in).Trim().startsWith("#else"))
        {
          getLine(in);   // Discard the #else line
      process(in, _out, ignoring || foundTrueCondition);
        }

        line = getLine(in);

        if (line == null)
          throw new IOException("Missing \"#fi\"");

        if (!line.Trim().startsWith("#fi"))
          throw new IOException("Expected \"#fi\", got: " + line);
      }


  public static void main(String[] args) throws Exception
  {
    Dictionary map = new Dictionary();
    map.Add("falseArg", false);
    map.Add("trueArg", true);
    map.Add("stringValue", "someString");

    new OutputFileGenerator(args[0], map).generate(new TextWriter(args[1]));
  }

  public static void generateFromTemplate(
      string template, Dictionary<String, Object> options,
      string outputFileName) {
    OutputFileGenerator gen = new OutputFileGenerator(template, options);
    StringWriter sw = new StringWriter();
    gen.generate(new TextWriter(sw));
    sw.Close();
    TextWriter fw = null;
    try {
      File tmp = new File(outputFileName);
      fw = new TextWriter(
              new BufferedWriter(
              new FileWriter(tmp),
              8092
          )
      );

      fw.print(sw.ToString());
    } finally {
      if (fw != null) {
        fw.Close();
      }
    }
  }
}
