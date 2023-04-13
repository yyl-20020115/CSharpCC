// Copyright 2011 Google Inc. All Rights Reserved.
// Author: sreeni@google.com (Sreeni Viswanadha)

using org.javacc.utils;
using System.Text;

namespace org.javacc.parser;


public class CodeGenerator : JavaCCGlobals
{
    protected StringBuilder mainBuffer = new ();
    protected StringBuilder includeBuffer = new ();
    protected StringBuilder staticsBuffer = new ();
    protected StringBuilder outputBuffer;
    public CodeGenerator()
    {
        this.outputBuffer = mainBuffer;
    }
    public void GenStringLiteralArrayCPP(string varName, String[] arr)
    {
        // First generate char array vars
        for (int i = 0; i < arr.Length; i++)
        {
            GenCodeLine("static const JJChar " + varName + "_arr_" + i + "[] = ");
            GenStringLiteralInCPP(arr[i]);
            GenCodeLine(";");
        }

        GenCodeLine("static const JJString " + varName + "[] = {");
        for (int i = 0; i < arr.Length; i++)
        {
            GenCodeLine(varName + "_arr_" + i + ", ");
        }
        GenCodeLine("};");
    }
    public void GenStringLiteralInCPP(string s)
    {
        // string literals in CPP become char arrays
        outputBuffer.Append('{');
        for (int i = 0; i < s.Length; i++)
        {
            outputBuffer.Append("0x" + Convert.ToString((int)s[i],16) + ", ");
        }
        outputBuffer.Append("0}");
    }
    public void GenCodeLine(params object[] code)
    {
        GenCode(code);
        GenCode("\n");
    }

    public void GenCode(params object[] code)
    {
        foreach (object s in code)
        {
            outputBuffer.Append(s);
        }
    }

    public void SaveOutput(string fileName)
    {
        if (!IsJavaLanguage())
        {
            string incfilePath = fileName.Replace(".cc", ".h");
            string incfileName = (incfilePath);
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
            SaveOutput(incfilePath, includeBuffer);
        }

        mainBuffer.Insert(0, "/* " + fileName + " */\n");
        SaveOutput(fileName, mainBuffer);
    }

    private static bool IsHexDigit(char c) 
        => (c >= '0' && c <= '9') ||
               (c >= 'a' && c <= 'f') ||
               (c >= 'A' && c <= 'F');

    // HACK
    private void FixupLongLiterals(StringBuilder sb)
    {
        for (int i = 0; i < sb.Length - 1; i++)
        {
            int beg = i;
            char c1 = sb[i];
            char c2 = sb[i + 1];
            if (char.IsDigit(c1) || (c1 == '0' && c2 == 'x'))
            {
                i += c1 == '0' ? 2 : 1;
                while (IsHexDigit(sb[i])) i++;
                if (sb[i] == 'L')
                {
                    sb.Insert(i, "UL");
                }
                i++;
            }
        }
    }

    public void SaveOutput(string fileName, StringBuilder sb)
    {
        TextWriter fw = null;
        if (!IsJavaLanguage())
        {
            FixupLongLiterals(sb);
        }
        try
        {
            fw = new StreamWriter(fileName);

            fw.Write(sb.ToString());
        }
        catch (IOException ioe)
        {
            JavaCCErrors.Fatal("Could not create output file: " + fileName);
        }
        finally
        {
            if (fw != null)
            {
                fw.Close();
            }
        }
    }

    protected new int cline = 0, ccol = 0;

    public void PrintsTokenSetup(Token t)
    {
        Token tt = t;

        while (tt.specialToken != null)
        {
            tt = tt.specialToken;
        }

        cline = tt.beginLine;
        ccol = tt.beginColumn;
    }

    public void PrintTokenList(List<Token> list)
    {
        Token? t = null;
        foreach (var t2 in list)
        {
            PrintToken(t = t2);
        }
        if (t != null)
            PrintTrailingComments(t);
    }

    public new void PrintTokenOnly(Token t)
    {
        GenCode(GetStringForTokenOnly(t));
    }

    protected string GetStringForTokenOnly(Token t)
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
            char last = t.image[^1];
            if (last == '\n' || last == '\r')
            {
                cline++; ccol = 1;
            }
        }

        return retval;
    }

    public new void PrintToken(Token t)
    {
        GenCode(GetStringToPrint(t));
    }

    public string GetStringToPrint(Token t)
    {
        string retval = "";
        Token tt = t.specialToken;
        if (tt != null)
        {
            while (tt.specialToken != null) tt = tt.specialToken;
            while (tt != null)
            {
                retval += GetStringForTokenOnly(tt);
                tt = tt.next;
            }
        }

        return retval + GetStringForTokenOnly(t);
    }

    public new void PrintLeadingComments(Token t)
    {
        GenCode(GetLeadingComments(t));
    }

    public string GetLeadingComments(Token t)
    {
        string retval = "";
        if (t.specialToken == null) return retval;
        Token tt = t.specialToken;
        while (tt.specialToken != null) tt = tt.specialToken;
        while (tt != null)
        {
            retval += GetStringForTokenOnly(tt);
            tt = tt.next;
        }
        if (ccol != 1 && cline != t.beginLine)
        {
            retval += "\n";
            cline++; ccol = 1;
        }

        return retval;
    }

    public new void PrintTrailingComments(Token t)
    {
        outputBuffer.Append(GetTrailingComments(t));
    }

    public string GetTrailingComments(Token t)
    {
        if (t.next == null) return "";
        return GetLeadingComments(t.next);
    }

    /**
     * for testing
     */
    public string GetGeneratedCode()
    {
        return outputBuffer.ToString() + "\n";
    }

    /**
     * Generate annotation. @XX syntax for java, comments in C++
     */
    public void GenAnnotation(string ann)
    {
        if (Options.isOutputLanguageJava())
        {
            GenCode("@" + ann);
        }
        else if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
        { // For now, it's only C++ for now
            GenCode("/*" + ann + "*/");
        }
        else
        {
            throw new Exception("Unknown language : " + Options.getOutputLanguage());
        }
    }

    /**
     * Generate a modifier
     */
    public void GenModifier(string mod)
    {
        string origMod = mod.ToLower();
        if (IsJavaLanguage())
        {
            GenCode(mod);
        }
        else
        { // For now, it's only C++ for now
            if (origMod == ("public") || origMod == ("private"))
            {
                GenCode(origMod + ": ");
            }
            // we don't care about other mods for now.
        }
    }

    /**
     * Generate a class with a given name, an array of superclass and
     * another array of super interfaes
     */
    public void GenClassStart(string mod, string name, String[] superClasses, String[] superInterfaces)
    {
        bool _isJavaLanguage = IsJavaLanguage();
        if (_isJavaLanguage && mod != null)
        {
            GenModifier(mod);
        }
        GenCode("class " + name);
        if (_isJavaLanguage)
        {
            if (superClasses.Length == 1 && superClasses[0] != null)
            {
                GenCode(" extends " + superClasses[0]);
            }
            if (superInterfaces.Length != 0)
            {
                GenCode(" implements ");
            }
        }
        else
        {
            if (superClasses.Length > 0 || superInterfaces.Length > 0)
            {
                GenCode(" : ");
            }

            GenCommaSeperatedString(superClasses);
        }

        GenCommaSeperatedString(superInterfaces);
        GenCodeLine(" {");
        if (Options.getOutputLanguage() == (Options.OUTPUT_LANGUAGE__CPP))
        {
            GenCodeLine("public:");
        }
    }

    private void GenCommaSeperatedString(String[] strings)
    {
        for (int i = 0; i < strings.Length; i++)
        {
            if (i > 0)
            {
                GenCode(", ");
            }

            GenCode(strings[i]);
        }
    }

    public static bool IsJavaLanguage()
    {
        // TODO :: CBA --  Require Unification of output language specific processing into a single Enum class
        return Options.isOutputLanguageJava();
    }

    public void SwitchToMainFile()
    {
        outputBuffer = mainBuffer;
    }

    public void SwitchToStaticsFile()
    {
        if (!IsJavaLanguage())
        {
            outputBuffer = staticsBuffer;
        }
    }

    public void SwitchToIncludeFile()
    {
        if (!IsJavaLanguage())
        {
            outputBuffer = includeBuffer;
        }
    }

    public void GenerateMethodDefHeader(string modsAndRetType, string className, string nameAndParams)
    {
        GenerateMethodDefHeader(modsAndRetType, className, nameAndParams, null);
    }

    public void GenerateMethodDefHeader(string qualifiedModsAndRetType, string className, string nameAndParams, string exceptions)
    {
        // for C++, we generate the signature in the header file and body in main file
        if (IsJavaLanguage())
        {
            GenCode(qualifiedModsAndRetType + " " + nameAndParams);
            if (exceptions != null)
            {
                GenCode(" throws " + exceptions);
            }
            GenCodeLine("");
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
                modsAndRetType = qualifiedModsAndRetType[(i + 1)..];

            if (modsAndRetType != null)
            {
                i = modsAndRetType.LastIndexOf("virtual");
                if (i >= 0)
                    modsAndRetType = modsAndRetType[(i + "virtual".Length)..];
            }
            i = qualifiedModsAndRetType.LastIndexOf("virtual");
            if (i >= 0)
                qualifiedModsAndRetType = qualifiedModsAndRetType[(i + "virtual".Length)..];
            mainBuffer.Append("\n" + qualifiedModsAndRetType + " " +
                                 GetClassQualifier(className) + nameAndParams);
            //if (exceptions != null)
            //mainBuffer.Append(" throw( " + exceptions + ")");
            SwitchToMainFile();
        }
    }

    protected static string GetClassQualifier(string className)
    {
        return className == null ? "" : className + "::";
    }

    public static string GetCharStreamName()
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
    protected void WriteTemplate(string name, Dictionary<String, object> options, params object[] additionalOptions)
    {

        // options.Add("", .valueOf(maxOrdinal));


        for (int i = 0; i < additionalOptions.Length; i++)
        {
            object o = additionalOptions[i];

            if (o is Dictionary<string, object> d)
            {
                foreach(var v in d)
                {
                    options.Add(v.Key, v.Value);
                }
            }
            else
            {
                if (i == additionalOptions.Length - 1)
                    throw new ArgumentException("Must supply pairs of [name value] args");

                options.Add((String)o, additionalOptions[i + 1]);
                i++;
            }
        }

        var gen = new OutputFileGenerator(name, options);
        var sw = new StringWriter();
        gen.Generate(sw);
        sw.Close();
        GenCode(sw.ToString());
    }
}
