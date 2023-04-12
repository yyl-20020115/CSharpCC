// Copyright 2011 Google Inc. All Rights Reserved.
// Author: sreeni@google.com (Sreeni Viswanadha)

using org.javacc.utils;
using System.Text;

namespace org.javacc.parser;




public class CodeGenerator : JavaCCParserConstants
{
    protected StringBuilder mainBuffer = new StringBuilder();
    protected StringBuilder includeBuffer = new StringBuilder();
    protected StringBuilder staticsBuffer = new StringBuilder();
    protected StringBuilder outputBuffer = mainBuffer;

    public void genStringLiteralArrayCPP(string varName, String[] arr)
    {
        // First generate char array vars
        for (int i = 0; i < arr.Length; i++)
        {
            genCodeLine("static const JJChar " + varName + "_arr_" + i + "[] = ");
            genStringLiteralInCPP(arr[i]);
            genCodeLine(";");
        }

        genCodeLine("static const JJString " + varName + "[] = {");
        for (int i = 0; i < arr.Length; i++)
        {
            genCodeLine(varName + "_arr_" + i + ", ");
        }
        genCodeLine("};");
    }
    public void genStringLiteralInCPP(string s)
    {
        // string literals in CPP become char arrays
        outputBuffer.Append("{");
        for (int i = 0; i < s.Length; i++)
        {
            outputBuffer.Append("0x" + int.toHexString((int)s[i]) + ", ");
        }
        outputBuffer.Append("0}");
    }
    public void genCodeLine(params object[] code)
    {
        genCode(code);
        genCode("\n");
    }

    public void genCode(params object[] code)
    {
        foreach (object s in code)
        {
            outputBuffer.Append(s);
        }
    }

    public void saveOutput(string fileName)
    {
        if (!isJavaLanguage())
        {
            string incfilePath = fileName.Replace(".cc", ".h");
            string incfileName = new File(incfilePath);
            includeBuffer.Insert(0, "#define " + incfileName.Replace('.', '_').ToUpper() + "\n");
            includeBuffer.Insert(0, "#ifndef " + incfileName.Replace('.', '_').ToUpper() + "\n");

            // dump the statics into the main file with the code.
            mainBuffer.Insert(0, staticsBuffer);

            // Finally enclose the whole thing in the namespace, if specified.
            if (Options.stringValue(Options.USEROPTION__CPP_NAMESPACE).Length > 0)
            {
                mainBuffer.Insert(0, "namespace " + Options.stringValue("NAMESPACE_OPEN") + "\n");
                mainBuffer.Append(Options.stringValue("NAMESPACE_CLOSE") + "\n");
                includeBuffer.Append(Options.stringValue("NAMESPACE_CLOSE") + "\n");
            }

            if (jjtreeGenerated)
            {
                mainBuffer.Insert(0, "#include \"SimpleNode.h\"\n");
            }
            if (Options.getTokenManagerUsesParser())
                mainBuffer.Insert(0, "#include \"" + cu_name + ".h\"\n");
            mainBuffer.Insert(0, "#include \"TokenMgrError.h\"\n");
            mainBuffer.Insert(0, "#include \"" + incfileName + "\"\n");
            includeBuffer.Append("#endif\n");
            saveOutput(incfilePath, includeBuffer);
        }

        mainBuffer.Insert(0, "/* " + new File(fileName) + " */\n");
        saveOutput(fileName, mainBuffer);
    }

    private static bool isHexDigit(char c)
    {
        return (c >= '0' && c <= '9') ||
               (c >= 'a' && c <= 'f') ||
               (c >= 'A' && c <= 'F');
    }

    // HACK
    private void fixupLongLiterals(StringBuilder sb)
    {
        for (int i = 0; i < sb.Length - 1; i++)
        {
            int beg = i;
            char c1 = sb[i];
            char c2 = sb[i + 1];
            if (char.IsDigit(c1) || (c1 == '0' && c2 == 'x'))
            {
                i += c1 == '0' ? 2 : 1;
                while (isHexDigit(sb[i])) i++;
                if (sb[i] == 'L')
                {
                    sb.Insert(i, "UL");
                }
                i++;
            }
        }
    }

    public void saveOutput(string fileName, StringBuilder sb)
    {
        TextWriter fw = null;
        if (!isJavaLanguage())
        {
            fixupLongLiterals(sb);
        }
        try
        {
            File tmp = new File(fileName);
            fw = new TextWriter(
                    new BufferedWriter(
                    new FileWriter(tmp),
                    8092
                )
            );

            fw.print(sb.ToString());
        }
        catch (IOException ioe)
        {
            JavaCCErrors.fatal("Could not create output file: " + fileName);
        }
        finally
        {
            if (fw != null)
            {
                fw.Close();
            }
        }
    }

    protected int cline, ccol;

    public void printTokenSetup(Token t)
    {
        Token tt = t;

        while (tt.specialToken != null)
        {
            tt = tt.specialToken;
        }

        cline = tt.beginLine;
        ccol = tt.beginColumn;
    }

    public void printTokenList(List<Token> list)
    {
        Token t = null;
        for (Iterator<Token> it = list.iterator(); it.hasNext();)
        {
            t = (Token)it.next();
            printToken(t);
        }

        if (t != null)
            printTrailingComments(t);
    }

    public void printTokenOnly(Token t)
    {
        genCode(getStringForTokenOnly(t));
    }

    protected string getStringForTokenOnly(Token t)
    {
        string retval = "";
        for (; cline < t.beginLine; cline++)
        {
            retval += "\n"; ccol = 1;
        }
        for (; ccol < t.beginColumn; ccol++)
        {
            retval += " ";
        }
        if (t.kind == JavaCCParserConstants.STRING_LITERAL ||
            t.kind == JavaCCParserConstants.CHARACTER_LITERAL)
            retval += addUnicodeEscapes(t.image);
        else
            retval += t.image;
        cline = t.endLine;
        ccol = t.endColumn + 1;
        if (t.image.Length > 0)
        {
            char last = t.image.charAt(t.image.Length - 1);
            if (last == '\n' || last == '\r')
            {
                cline++; ccol = 1;
            }
        }

        return retval;
    }

    public void printToken(Token t)
    {
        genCode(getStringToPrint(t));
    }

    public string getStringToPrint(Token t)
    {
        string retval = "";
        Token tt = t.specialToken;
        if (tt != null)
        {
            while (tt.specialToken != null) tt = tt.specialToken;
            while (tt != null)
            {
                retval += getStringForTokenOnly(tt);
                tt = tt.next;
            }
        }

        return retval + getStringForTokenOnly(t);
    }

    public void printLeadingComments(Token t)
    {
        genCode(getLeadingComments(t));
    }

    protected string getLeadingComments(Token t)
    {
        string retval = "";
        if (t.specialToken == null) return retval;
        Token tt = t.specialToken;
        while (tt.specialToken != null) tt = tt.specialToken;
        while (tt != null)
        {
            retval += getStringForTokenOnly(tt);
            tt = tt.next;
        }
        if (ccol != 1 && cline != t.beginLine)
        {
            retval += "\n";
            cline++; ccol = 1;
        }

        return retval;
    }

    public void printTrailingComments(Token t)
    {
        outputBuffer.Append(getTrailingComments(t));
    }

    public string getTrailingComments(Token t)
    {
        if (t.next == null) return "";
        return getLeadingComments(t.next);
    }

    /**
     * for testing
     */
    public string getGeneratedCode()
    {
        return outputBuffer.ToString() + "\n";
    }

    /**
     * Generate annotation. @XX syntax for java, comments in C++
     */
    public void genAnnotation(string ann)
    {
        if (Options.isOutputLanguageJava())
        {
            genCode("@" + ann);
        }
        else if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
        { // For now, it's only C++ for now
            genCode("/*" + ann + "*/");
        }
        else
        {
            throw new Exception("Unknown language : " + Options.getOutputLanguage());
        }
    }

    /**
     * Generate a modifier
     */
    public void genModifier(string mod)
    {
        string origMod = mod.ToLower(Locale.ENGLISH);
        if (isJavaLanguage())
        {
            genCode(mod);
        }
        else
        { // For now, it's only C++ for now
            if (origMod == ("public") || origMod == ("private"))
            {
                genCode(origMod + ": ");
            }
            // we don't care about other mods for now.
        }
    }

    /**
     * Generate a class with a given name, an array of superclass and
     * another array of super interfaes
     */
    public void genClassStart(string mod, string name, String[] superClasses, String[] superInterfaces)
    {
        bool _isJavaLanguage = isJavaLanguage();
        if (_isJavaLanguage && mod != null)
        {
            genModifier(mod);
        }
        genCode("class " + name);
        if (_isJavaLanguage)
        {
            if (superClasses.Length == 1 && superClasses[0] != null)
            {
                genCode(" extends " + superClasses[0]);
            }
            if (superInterfaces.Length != 0)
            {
                genCode(" implements ");
            }
        }
        else
        {
            if (superClasses.Length > 0 || superInterfaces.Length > 0)
            {
                genCode(" : ");
            }

            genCommaSeperatedString(superClasses);
        }

        genCommaSeperatedString(superInterfaces);
        genCodeLine(" {");
        if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
        {
            genCodeLine("public:");
        }
    }

    private void genCommaSeperatedString(String[] strings)
    {
        for (int i = 0; i < strings.Length; i++)
        {
            if (i > 0)
            {
                genCode(", ");
            }

            genCode(strings[i]);
        }
    }

    public bool isJavaLanguage()
    {
        // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
        return Options.isOutputLanguageJava();
    }

    public void switchToMainFile()
    {
        outputBuffer = mainBuffer;
    }

    public void switchToStaticsFile()
    {
        if (!isJavaLanguage())
        {
            outputBuffer = staticsBuffer;
        }
    }

    public void switchToIncludeFile()
    {
        if (!isJavaLanguage())
        {
            outputBuffer = includeBuffer;
        }
    }

    public void generateMethodDefHeader(string modsAndRetType, string className, string nameAndParams)
    {
        generateMethodDefHeader(modsAndRetType, className, nameAndParams, null);
    }

    public void generateMethodDefHeader(string qualifiedModsAndRetType, string className, string nameAndParams, string exceptions)
    {
        // for C++, we generate the signature in the header file and body in main file
        if (isJavaLanguage())
        {
            genCode(qualifiedModsAndRetType + " " + nameAndParams);
            if (exceptions != null)
            {
                genCode(" throws " + exceptions);
            }
            genCodeLine("");
        }
        else
        {
            includeBuffer.Append(qualifiedModsAndRetType + " " + nameAndParams);
            //if (exceptions != null)
            //includeBuffer.Append(" throw(" + exceptions + ")");
            includeBuffer.Append(";\n");

            string modsAndRetType = null;
            int i = qualifiedModsAndRetType.LastIndexOf(':');
            if (i >= 0)
                modsAndRetType = qualifiedModsAndRetType.substring(i + 1);

            if (modsAndRetType != null)
            {
                i = modsAndRetType.LastIndexOf("virtual");
                if (i >= 0)
                    modsAndRetType = modsAndRetType.substring(i + "virtual".Length);
            }
            i = qualifiedModsAndRetType.LastIndexOf("virtual");
            if (i >= 0)
                qualifiedModsAndRetType = qualifiedModsAndRetType.substring(i + "virtual".Length);
            mainBuffer.Append("\n" + qualifiedModsAndRetType + " " +
                                 getClassQualifier(className) + nameAndParams);
            //if (exceptions != null)
            //mainBuffer.Append(" throw( " + exceptions + ")");
            switchToMainFile();
        }
    }

    protected string getClassQualifier(string className)
    {
        return className == null ? "" : className + "::";
    }

    public static string getCharStreamName()
    {
        if (Options.getUserCharStream())
        {
            return "CharStream";
        }
        else
        {
            return Options.getJavaUnicodeEscape() ? "JavaCharStream"
                                                  : "SimpleCharStream";
        }
    }
    protected void writeTemplate(string name, Dictionary<String, object> options, params object[] additionalOptions)
    {

        // options.Add("", .valueOf(maxOrdinal));


        for (int i = 0; i < additionalOptions.Length; i++)
        {
            object o = additionalOptions[i];

            if (o is Dictionary<string, object>)
            {
                options.putAll((Dictionary<String, object>)o);
            }
            else
            {
                if (i == additionalOptions.Length - 1)
                    throw new IllegalArgumentException("Must supply pairs of [name value] args");

                options.Add((String)o, additionalOptions[i + 1]);
                i++;
            }
        }

        OutputFileGenerator gen = new OutputFileGenerator(name, options);
        StringWriter sw = new StringWriter();
        gen.generate(new TextWriter(sw));
        sw.Close();
        genCode(sw.ToString());
    }
}
