// Copyright 2011 Google Inc. All Rights Reserved.
// Author: sreeni@google.com (Sreeni Viswanadha)

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
using org.javacc.utils;
using System.Text;

namespace org.javacc.parser;


public enum Language
{
    java, cpp
}
/**
 * A class with static state that stores all option information.
 */
public class Options
{

    /**
	 * Limit subclassing to derived classes.
	 */
    protected Options()
    {
    }

    /**
	 * These are options that are not settable by the user themselves, and that
	 * are set indirectly via some configuration of user options
	 */
    public static readonly string NONUSER_OPTION__NAMESPACE_CLOSE = "NAMESPACE_CLOSE";
    public static readonly string NONUSER_OPTION__HAS_NAMESPACE = "HAS_NAMESPACE";
    public static readonly string NONUSER_OPTION__NAMESPACE_OPEN = "NAMESPACE_OPEN";
    public static readonly string NONUSER_OPTION__PARSER_NAME = "PARSER_NAME";
    public static readonly string NONUSER_OPTION__LEGACY_EXCEPTION_HANDLING = "LEGACY_EXCEPTION_HANDLING";

    /**
	 * Options that the user can specify from .javacc file
	 */

    public static readonly string USEROPTION__JAVA_TEMPLATE_TYPE = "JAVA_TEMPLATE_TYPE";
    public static readonly string USEROPTION__GENERATE_BOILERPLATE = "GENERATE_BOILERPLATE";
    public static readonly string USEROPTION__OUTPUT_LANGUAGE = "OUTPUT_LANGUAGE";
    public static readonly string USEROPTION__PARSER_CODE_GENERATOR = "PARSER_CODE_GENERATOR";
    public static readonly string USEROPTION__TOKEN_MANAGER_CODE_GENERATOR = "TOKEN_MANAGER_CODE_GENERATOR";
    public static readonly string USEROPTION__NO_DFA = "NO_DFA";
    public static readonly string USEROPTION__STATIC = "STATIC";
    public static readonly string USEROPTION__LOOKAHEAD = "LOOKAHEAD";
    public static readonly string USEROPTION__IGNORE_CASE = "IGNORE_CASE";
    public static readonly string USEROPTION__UNICODE_INPUT = "UNICODE_INPUT";
    public static readonly string USEROPTION__JAVA_UNICODE_ESCAPE = "JAVA_UNICODE_ESCAPE";
    public static readonly string USEROPTION__ERROR_REPORTING = "ERROR_REPORTING";
    public static readonly string USEROPTION__DEBUG_TOKEN_MANAGER = "DEBUG_TOKEN_MANAGER";
    public static readonly string USEROPTION__DEBUG_LOOKAHEAD = "DEBUG_LOOKAHEAD";
    public static readonly string USEROPTION__DEBUG_PARSER = "DEBUG_PARSER";
    public static readonly string USEROPTION__OTHER_AMBIGUITY_CHECK = "OTHER_AMBIGUITY_CHECK";
    public static readonly string USEROPTION__CHOICE_AMBIGUITY_CHECK = "CHOICE_AMBIGUITY_CHECK";
    public static readonly string USEROPTION__CACHE_TOKENS = "CACHE_TOKENS";
    public static readonly string USEROPTION__COMMON_TOKEN_ACTION = "COMMON_TOKEN_ACTION";
    public static readonly string USEROPTION__FORCE_LA_CHECK = "FORCE_LA_CHECK";
    public static readonly string USEROPTION__SANITY_CHECK = "SANITY_CHECK";
    public static readonly string USEROPTION__BUILD_TOKEN_MANAGER = "BUILD_TOKEN_MANAGER";
    public static readonly string USEROPTION__BUILD_PARSER = "BUILD_PARSER";
    public static readonly string USEROPTION__USER_CHAR_STREAM = "USER_CHAR_STREAM";
    public static readonly string USEROPTION__USER_TOKEN_MANAGER = "USER_TOKEN_MANAGER";
    public static readonly string USEROPTION__JDK_VERSION = "JDK_VERSION";
    public static readonly string USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC = "SUPPORT_CLASS_VISIBILITY_PUBLIC";
    public static readonly string USEROPTION__GENERATE_ANNOTATIONS = "GENERATE_ANNOTATIONS";
    public static readonly string USEROPTION__GENERATE_STRING_BUILDER = "GENERATE_STRING_BUILDER";
    public static readonly string USEROPTION__GENERATE_GENERICS = "GENERATE_GENERICS";
    public static readonly string USEROPTION__GENERATE_CHAINED_EXCEPTION = "GENERATE_CHAINED_EXCEPTION";
    public static readonly string USEROPTION__OUTPUT_DIRECTORY = "OUTPUT_DIRECTORY";
    public static readonly string USEROPTION__KEEP_LINE_COLUMN = "KEEP_LINE_COLUMN";
    public static readonly string USEROPTION__GRAMMAR_ENCODING = "GRAMMAR_ENCODING";
    public static readonly string USEROPTION__TOKEN_FACTORY = "TOKEN_FACTORY";
    public static readonly string USEROPTION__TOKEN_EXTENDS = "TOKEN_EXTENDS";
    public static readonly string USEROPTION__DEPTH_LIMIT = "DEPTH_LIMIT";

    public static readonly string USEROPTION__TOKEN_MANAGER_USES_PARSER = "TOKEN_MANAGER_USES_PARSER";

    public static readonly string USEROPTION__TOKEN_SUPER_CLASS = "TOKEN_SUPER_CLASS";
    public static readonly string USEROPTION__PARSER_SUPER_CLASS = "PARSER_SUPER_CLASS";
    public static readonly string USEROPTION__TOKEN_MANAGER_SUPER_CLASS = "TOKEN_MANAGER_SUPER_CLASS";

    public static readonly string USEROPTION__CPP_NAMESPACE = "NAMESPACE";
    public static readonly string USEROPTION__CPP_IGNORE_ACTIONS = "IGNORE_ACTIONS";
    public static readonly string USEROPTION__CPP_STOP_ON_FIRST_ERROR = "STOP_ON_FIRST_ERROR";
    public static readonly string USEROPTION__CPP_STACK_LIMIT = "STACK_LIMIT";

    public static readonly string USEROPTION__CPP_TOKEN_INCLUDE = "TOKEN_INCLUDE";
    public static readonly string USEROPTION__CPP_PARSER_INCLUDE = "PARSER_INCLUDE";
    public static readonly string USEROPTION__CPP_TOKEN_MANAGER_INCLUDE = "TOKEN_MANAGER_INCLUDE";
    /**
	 * Various constants relating to possible values for certain options
	 */

    public static readonly string OUTPUT_LANGUAGE__CPP = "c++";
    public static readonly string OUTPUT_LANGUAGE__JAVA = "java";



    public static Language language = Language.java;


    /**
	 * 2013/07/22 -- GWT Compliant Output -- no external dependencies on GWT,
	 * but generated code adds loose coupling to IO, for 6.1 release, this is
	 * opt-in, moving forward to 7.0, after thorough testing, this will likely
	 * become the default option with classic being deprecated
	 */
    public static readonly string JAVA_TEMPLATE_TYPE_MODERN = "modern";

    /**
	 * The old style of Java code generation (tight coupling of code to Java IO classes - not GWT compatible)
	 */
    public static readonly string JAVA_TEMPLATE_TYPE_CLASSIC = "classic";


    static readonly HashSet<OptionInfo> userOptions;


    static Options()
    {
        var options = new HashSet<OptionInfo>
        {
            new OptionInfo(USEROPTION__LOOKAHEAD, OptionType.INTEGER, (1)),
            new OptionInfo(USEROPTION__CHOICE_AMBIGUITY_CHECK, OptionType.INTEGER, (2)),
            new OptionInfo(USEROPTION__OTHER_AMBIGUITY_CHECK, OptionType.INTEGER, (1)),
            new OptionInfo(USEROPTION__STATIC, OptionType.BOOLEAN, true),
            new OptionInfo(USEROPTION__PARSER_CODE_GENERATOR, OptionType.STRING, ""),
            new OptionInfo(USEROPTION__TOKEN_MANAGER_CODE_GENERATOR, OptionType.STRING, ""),
            new OptionInfo(USEROPTION__NO_DFA, OptionType.BOOLEAN, false),
            new OptionInfo(USEROPTION__DEBUG_PARSER, OptionType.BOOLEAN, false),

            new OptionInfo(USEROPTION__DEBUG_LOOKAHEAD, OptionType.BOOLEAN, false),
            new OptionInfo(USEROPTION__DEBUG_TOKEN_MANAGER, OptionType.BOOLEAN, false),
            new OptionInfo(USEROPTION__ERROR_REPORTING, OptionType.BOOLEAN, true),
            new OptionInfo(USEROPTION__JAVA_UNICODE_ESCAPE, OptionType.BOOLEAN, false),

            new OptionInfo(USEROPTION__UNICODE_INPUT, OptionType.BOOLEAN, false),
            new OptionInfo(USEROPTION__IGNORE_CASE, OptionType.BOOLEAN, false),
            new OptionInfo(USEROPTION__USER_TOKEN_MANAGER, OptionType.BOOLEAN, false),
            new OptionInfo(USEROPTION__USER_CHAR_STREAM, OptionType.BOOLEAN, false),

            new OptionInfo(USEROPTION__BUILD_PARSER, OptionType.BOOLEAN, true),
            new OptionInfo(USEROPTION__BUILD_TOKEN_MANAGER, OptionType.BOOLEAN, true),
            new OptionInfo(USEROPTION__TOKEN_MANAGER_USES_PARSER, OptionType.BOOLEAN, false),
            new OptionInfo(USEROPTION__SANITY_CHECK, OptionType.BOOLEAN, true),

            new OptionInfo(USEROPTION__FORCE_LA_CHECK, OptionType.BOOLEAN, false),
            new OptionInfo(USEROPTION__COMMON_TOKEN_ACTION, OptionType.BOOLEAN, false),
            new OptionInfo(USEROPTION__CACHE_TOKENS, OptionType.BOOLEAN, false),
            new OptionInfo(USEROPTION__KEEP_LINE_COLUMN, OptionType.BOOLEAN, true),

            new OptionInfo(USEROPTION__GENERATE_CHAINED_EXCEPTION, OptionType.BOOLEAN, false),
            new OptionInfo(USEROPTION__GENERATE_GENERICS, OptionType.BOOLEAN, false),
            new OptionInfo(USEROPTION__GENERATE_BOILERPLATE, OptionType.BOOLEAN, true),
            new OptionInfo(USEROPTION__GENERATE_STRING_BUILDER, OptionType.BOOLEAN, false),

            new OptionInfo(USEROPTION__GENERATE_ANNOTATIONS, OptionType.BOOLEAN, false),
            new OptionInfo(USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC, OptionType.BOOLEAN, true),
            new OptionInfo(USEROPTION__OUTPUT_DIRECTORY, OptionType.STRING, "."),
            new OptionInfo(USEROPTION__JDK_VERSION, OptionType.STRING, "1.5"),

            new OptionInfo(USEROPTION__TOKEN_FACTORY, OptionType.STRING, ""),
            new OptionInfo(USEROPTION__TOKEN_EXTENDS, OptionType.STRING, ""),
            new OptionInfo(USEROPTION__GRAMMAR_ENCODING, OptionType.STRING, ""),
            new OptionInfo(USEROPTION__OUTPUT_LANGUAGE, OptionType.STRING, OUTPUT_LANGUAGE__JAVA),
            new OptionInfo(USEROPTION__JAVA_TEMPLATE_TYPE, OptionType.STRING, JAVA_TEMPLATE_TYPE_CLASSIC),
            new OptionInfo(USEROPTION__CPP_NAMESPACE, OptionType.STRING, ""),

            new OptionInfo(USEROPTION__TOKEN_SUPER_CLASS, OptionType.STRING, null),
            new OptionInfo(USEROPTION__PARSER_SUPER_CLASS, OptionType.STRING, null),
            new OptionInfo(USEROPTION__TOKEN_MANAGER_SUPER_CLASS, OptionType.STRING, null),

            new OptionInfo(USEROPTION__CPP_TOKEN_INCLUDE, OptionType.STRING, ""),
            new OptionInfo(USEROPTION__CPP_PARSER_INCLUDE, OptionType.STRING, ""),
            new OptionInfo(USEROPTION__CPP_TOKEN_MANAGER_INCLUDE, OptionType.STRING, ""),

            new OptionInfo(USEROPTION__CPP_IGNORE_ACTIONS, OptionType.BOOLEAN, false),
            new OptionInfo(USEROPTION__CPP_STOP_ON_FIRST_ERROR, OptionType.BOOLEAN, false),

            new OptionInfo(USEROPTION__DEPTH_LIMIT, OptionType.INTEGER, (0)),
            new OptionInfo(USEROPTION__CPP_STACK_LIMIT, OptionType.STRING, "")
        };

        userOptions = (options);
        language = Language.java;
        supportedJavaTemplateTypes.Add(JAVA_TEMPLATE_TYPE_CLASSIC);
        supportedJavaTemplateTypes.Add(JAVA_TEMPLATE_TYPE_MODERN);
        supportedLanguages.Add(OUTPUT_LANGUAGE__JAVA);
        supportedLanguages.Add(OUTPUT_LANGUAGE__CPP);

    }

    /**
     * A mapping of option names (Strings) to values (Integer, Boolean, String).
     * This table is initialized by the main program. Its contents defines the
     * set of legal options. Its initial values define the default option
     * values, and the option types can be determined from these values too.
     */
    protected static Dictionary<String, object> optionValues = new();

    /**
     * Initialize for JavaCC
     */
    public static void Init()
    {
        optionValues = new Dictionary<String, object>();
        cmdLineSetting = new HashSet<string>();
        inputFileSetting = new HashSet<string>();

        foreach (OptionInfo t in userOptions)
        {
            optionValues.Add(t, t.Default);
        }

        {
            object o = optionValues[(USEROPTION__JAVA_TEMPLATE_TYPE)];
            bool isLegacy = JAVA_TEMPLATE_TYPE_CLASSIC == (o);
            optionValues.Add(NONUSER_OPTION__LEGACY_EXCEPTION_HANDLING, isLegacy);
        }

    }

    /**
     * Convenience method to retrieve integer options.
     */
    public static int intValue(string option)
    {
        return (optionValues.get(option)).intValue();
    }

    /**
     * Convenience method to retrieve bool options.
     */
    public static bool booleanValue(string option)
    {
        return ((Boolean)optionValues.get(option)).booleanValue();
    }

    /**
     * Convenience method to retrieve string options.
     */
    public static string stringValue(string option)
    {
        return (String)optionValues.get(option);
    }


    public static object objectValue(string option)
    {
        return optionValues.get(option);
    }


    public static Dictionary<String, object> getOptions()
    {
        Dictionary<String, object> ret = new Dictionary<String, object>(optionValues);
        return ret;
    }

    /**
     * Keep track of what options were set as a command line argument. We use
     * this to see if the options set from the command line and the ones set in
     * the input files clash in any way.
     */
    private static HashSet<string> cmdLineSetting = null;

    /**
     * Keep track of what options were set from the grammar file. We use this to
     * see if the options set from the command line and the ones set in the
     * input files clash in any way.
     */
    private static HashSet<string> inputFileSetting = null;

    /**
     * Returns a string representation of the specified options of interest.
     * Used when, for example, generating Token.java to record the JavaCC
     * options that were used to generate the file. All of the options must be
     * bool values.
     *
     * @param interestingOptions
     *            the options of interest, eg {Options.USEROPTION__STATIC, Options.USEROPTION__CACHE_TOKENS}
     * @return the string representation of the options, eg
     *         "STATIC=true,CACHE_TOKENS=false"
     */
    public static string getOptionsString(String[] interestingOptions)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < interestingOptions.Length; i++)
        {
            string key = interestingOptions[i];
            sb.Append(key);
            sb.Append('=');
            sb.Append(optionValues.get(key));
            if (i != interestingOptions.Length - 1)
            {
                sb.Append(',');
            }
        }

        return sb.ToString();
    }

    public static string getTokenMgrErrorClass()
    {
        return isOutputLanguageJava() ? (isLegacyExceptionHandling() ? "TokenMgrError"
                : "TokenMgrException")
                : "TokenMgrError";
    }

    /**
     * Determine if a given command line argument might be an option flag.
     * Command line options start with a dash&nbsp;(-).
     *
     * @param opt
     *            The command line argument to examine.
     * @return True when the argument looks like an option flag.
     */
    public static bool isOption(string opt)
    {
        return opt != null && opt.Length > 1 && opt[0] == '-';
    }

    /**
     * Help function to handle cases where the meaning of an option has changed
     * over time. If the user has supplied an option in the old format, it will
     * be converted to the new format.
     *
     * @param name
     *            The name of the option being checked.
     * @param value
     *            The option's value.
     * @return The upgraded value.
     */
    public static object upgradeValue(string name, object value)
    {
        if (name.Equals("NODE_FACTORY", StringComparison.InvariantCultureIgnoreCase)
                && value is bool b)
        {
            if (b)
            {
                value = "*";
            }
            else
            {
                value = "";
            }
        }

        return value;
    }

    public static void setInputFileOption(object nameloc, object valueloc,
            string name, object value)
    {
        string nameUpperCase = name.ToUpper();
        if (!optionValues.ContainsKey(nameUpperCase))
        {
            JavaCCErrors.warning(nameloc, "Bad option name \"" + name
                    + "\".  Option setting will be ignored.");
            return;
        }
        object existingValue = optionValues.get(nameUpperCase);

        value = upgradeValue(name, value);

        if (existingValue != null)
        {

            bool isIndirectProperty = nameUpperCase.equalsIgnoreCase(NONUSER_OPTION__LEGACY_EXCEPTION_HANDLING);

            object o = null;
            if (value is List)
            {
                o = ((List)value)[0];
            }
            else
            {
                o = value;
            }
            bool isValidInteger = (o is Integer && ((Integer)value).intValue() <= 0);
            if (isIndirectProperty || (existingValue.getClass() != object.getClass())
                    || (isValidInteger))
            {
                JavaCCErrors.warning(valueloc, "Bad option value \"" + value
                        + "\" for \"" + name
                        + "\".  Option setting will be ignored.");
                return;
            }

            if (inputFileSetting.Contains(nameUpperCase))
            {
                JavaCCErrors.warning(nameloc, "Duplicate option setting for \""
                        + name + "\" will be ignored.");
                return;
            }

            if (cmdLineSetting.Contains(nameUpperCase))
            {
                if (!existingValue == (value))
                {
                    JavaCCErrors.warning(nameloc, "Command line setting of \"" + name + "\" modifies option value in file.");
                }
                return;
            }
        }

        optionValues.Add(nameUpperCase, value);
        inputFileSetting.Add(nameUpperCase);

        // Special case logic block here for setting indirect flags

        if (nameUpperCase.equalsIgnoreCase(USEROPTION__JAVA_TEMPLATE_TYPE))
        {
            string templateType = (String)value;
            if (!isValidJavaTemplateType(templateType))
            {
                JavaCCErrors.warning(valueloc, "Bad option value \"" + value
                        + "\" for \"" + name
                        + "\".  Option setting will be ignored. Valid options : " + getAllValidJavaTemplateTypes());
                return;
            }

            bool isLegacy = JAVA_TEMPLATE_TYPE_CLASSIC == (templateType);
            optionValues.Add(NONUSER_OPTION__LEGACY_EXCEPTION_HANDLING, isLegacy);
        }
        else

        if (nameUpperCase.equalsIgnoreCase(USEROPTION__OUTPUT_LANGUAGE))
        {
            string outputLanguage = (String)value;
            if (!isValidOutputLanguage(outputLanguage))
            {
                JavaCCErrors.warning(valueloc, "Bad option value \"" + value
                        + "\" for \"" + name
                        + "\".  Option setting will be ignored. Valid options : " + getAllValidLanguages());
                return;
            }
            if (isOutputLanguageJava())
                language = Language.java;
            else if (isOutputLanguageCpp())
                language = Language.cpp;
        }
        else

        if (nameUpperCase.equalsIgnoreCase(USEROPTION__CPP_NAMESPACE))
        {
            processCPPNamespaceOption((String)value);
        }
    }


    private static string getAllValidJavaTemplateTypes()
    {
        return Arrays.toString(supportedJavaTemplateTypes.toArray(new String[supportedJavaTemplateTypes.Count]));
    }

    private static string getAllValidLanguages()
    {
        return Arrays.toString(supportedLanguages.toArray(new String[supportedLanguages.Count]));
    }


    /**
     * Process a single command-line option. The option is parsed and stored in
     * the optionValues map.
     *
     * @param arg
     */
    public static void setCmdLineOption(string arg)
    {
        string s;

        if (arg[0] == '-')
        {
            s = arg.substring(1);
        }
        else
        {
            s = arg;
        }

        string name;
        object Val;

        // Look for the first ":" or "=", which will separate the option name
        // from its value (if any).
        int index1 = s.IndexOf('=');
        int index2 = s.IndexOf(':');
        int index;

        if (index1 < 0)
            index = index2;
        else if (index2 < 0)
            index = index1;
        else if (index1 < index2)
            index = index1;
        else
            index = index2;

        if (index < 0)
        {
            name = s.ToUpper();
            if (optionValues.ContainsKey(name))
            {
                Val = true;
            }
            else if (name.Length() > 2 && name.charAt(0) == 'N'
                    && name[1] == 'O')
            {
                Val = false;
                name = name.substring(2);
            }
            else
            {
                Console.WriteLine("Warning: Bad option \"" + arg
                        + "\" will be ignored.");
                return;
            }
        }
        else
        {
            name = s.substring(0, index).ToUpper();
            if (s.substring(index + 1).equalsIgnoreCase("TRUE"))
            {
                Val = true;
            }
            else if (s.substring(index + 1).equalsIgnoreCase("FALSE"))
            {
                Val = false;
            }
            else
            {
                try
                {
                    int i = Integer.parseInt(s.substring(index + 1));
                    if (i <= 0)
                    {
                        Console.WriteLine("Warning: Bad option value in \""
                                + arg + "\" will be ignored.");
                        return;
                    }
                    Val = (i);
                }
                catch (NumberFormatException e)
                {
                    Val = s.substring(index + 1);
                    if (s.Length > index + 2)
                    {
                        // i.e., there is space for two '"'s in value
                        if (s[index + 1] == '"'
                                && s[^1] == '"')
                        {
                            // remove the two '"'s.
                            Val = s.substring(index + 2, s.Length - 1);
                        }
                    }
                }
            }
        }

        if (!optionValues.ContainsKey(name))
        {
            Console.WriteLine("Warning: Bad option \"" + arg
                    + "\" will be ignored.");
            return;
        }
        object valOrig = optionValues.get(name);
        if (Val.getClass() != valOrig.getClass())
        {
            Console.WriteLine("Warning: Bad option value in \"" + arg
                    + "\" will be ignored.");
            return;
        }
        if (cmdLineSetting.Contains(name))
        {
            Console.WriteLine("Warning: Duplicate option setting \"" + arg
                    + "\" will be ignored.");
            return;
        }

        Val = upgradeValue(name, Val);

        optionValues.Add(name, Val);
        cmdLineSetting.Add(name);
        if (name.equalsIgnoreCase(USEROPTION__CPP_NAMESPACE))
        {
            processCPPNamespaceOption((String)Val);
        }
    }

    public static void normalize()
    {
        if (getDebugLookahead() && !getDebugParser())
        {
            if (cmdLineSetting.Contains(USEROPTION__DEBUG_PARSER)
                    || inputFileSetting.Contains(USEROPTION__DEBUG_PARSER))
            {
                JavaCCErrors
                        .warning("True setting of option DEBUG_LOOKAHEAD overrides "
                                + "false setting of option DEBUG_PARSER.");
            }
            optionValues.Add(USEROPTION__DEBUG_PARSER, true);
        }

        // Now set the "GENERATE" options from the supplied (or default) JDK
        // version.

        optionValues.Add(USEROPTION__GENERATE_CHAINED_EXCEPTION, Boolean.valueOf(jdkVersionAtLeast(1.4)));
        optionValues.Add(USEROPTION__GENERATE_GENERICS, Boolean.valueOf(jdkVersionAtLeast(1.5)));
        optionValues.Add(USEROPTION__GENERATE_STRING_BUILDER, Boolean.valueOf(jdkVersionAtLeast(1.5)));
        optionValues.Add(USEROPTION__GENERATE_ANNOTATIONS, Boolean.valueOf(jdkVersionAtLeast(1.5)));
    }

    /**
     * Find the lookahead setting.
     *
     * @return The requested lookahead value.
     */
    public static int getLookahead()
    {
        return intValue(USEROPTION__LOOKAHEAD);
    }

    /**
     * Find the choice ambiguity check value.
     *
     * @return The requested choice ambiguity check value.
     */
    public static int getChoiceAmbiguityCheck()
    {
        return intValue(USEROPTION__CHOICE_AMBIGUITY_CHECK);
    }

    /**
     * Find the other ambiguity check value.
     *
     * @return The requested other ambiguity check value.
     */
    public static int getOtherAmbiguityCheck()
    {
        return intValue(USEROPTION__OTHER_AMBIGUITY_CHECK);
    }

    /**
     * Find the static value.
     *
     * @return The requested static value.
     */
    public static bool getStatic()
    {
        return booleanValue(USEROPTION__STATIC);
    }
    public static string getParserCodeGenerator()
    {
        string retVal = stringValue(USEROPTION__PARSER_CODE_GENERATOR);
        return retVal == ("") ? null : retVal;
    }
    public static string getTokenManagerCodeGenerator()
    {
        string retVal = stringValue(USEROPTION__TOKEN_MANAGER_CODE_GENERATOR);
        return retVal == ("") ? null : retVal;
    }
    public static bool getNoDfa()
    {
        return booleanValue(USEROPTION__NO_DFA);
    }

    /**
     * Find the debug parser value.
     *
     * @return The requested debug parser value.
     */
    public static bool getDebugParser()
    {
        return booleanValue(USEROPTION__DEBUG_PARSER);
    }

    /**
     * Find the debug lookahead value.
     *
     * @return The requested debug lookahead value.
     */
    public static bool getDebugLookahead()
    {
        return booleanValue(USEROPTION__DEBUG_LOOKAHEAD);
    }

    /**
     * Find the debug tokenmanager value.
     *
     * @return The requested debug tokenmanager value.
     */
    public static bool getDebugTokenManager()
    {
        return booleanValue(USEROPTION__DEBUG_TOKEN_MANAGER);
    }

    /**
     * Find the error reporting value.
     *
     * @return The requested error reporting value.
     */
    public static bool getErrorReporting()
    {
        return booleanValue(USEROPTION__ERROR_REPORTING);
    }

    /**
     * Find the Java unicode escape value.
     *
     * @return The requested Java unicode escape value.
     */
    public static bool getJavaUnicodeEscape()
    {
        return booleanValue(USEROPTION__JAVA_UNICODE_ESCAPE);
    }

    /**
     * Find the unicode input value.
     *
     * @return The requested unicode input value.
     */
    public static bool getUnicodeInput()
    {
        return booleanValue(USEROPTION__UNICODE_INPUT);
    }

    /**
     * Find the ignore case value.
     *
     * @return The requested ignore case value.
     */
    public static bool getIgnoreCase()
    {
        return booleanValue(USEROPTION__IGNORE_CASE);
    }

    /**
     * Find the user tokenmanager value.
     *
     * @return The requested user tokenmanager value.
     */
    public static bool getUserTokenManager()
    {
        return booleanValue(USEROPTION__USER_TOKEN_MANAGER);
    }

    /**
     * Find the user charstream value.
     *
     * @return The requested user charstream value.
     */
    public static bool getUserCharStream()
    {
        return booleanValue(USEROPTION__USER_CHAR_STREAM);
    }

    /**
     * Find the build parser value.
     *
     * @return The requested build parser value.
     */
    public static bool getBuildParser()
    {
        return booleanValue(USEROPTION__BUILD_PARSER);
    }

    /**
     * Find the build token manager value.
     *
     * @return The requested build token manager value.
     */
    public static bool getBuildTokenManager()
    {
        return booleanValue(USEROPTION__BUILD_TOKEN_MANAGER);
    }

    /**
     * Find the token manager uses parser value.
     *
     * @return The requested token manager uses parser value;
     */
    public static bool getTokenManagerUsesParser()
    {
        return booleanValue(USEROPTION__TOKEN_MANAGER_USES_PARSER) && !Options.getStatic();
    }

    /**
     * Find the sanity check value.
     *
     * @return The requested sanity check value.
     */
    public static bool getSanityCheck()
    {
        return booleanValue(USEROPTION__SANITY_CHECK);
    }

    /**
     * Find the force lookahead check value.
     *
     * @return The requested force lookahead value.
     */
    public static bool getForceLaCheck()
    {
        return booleanValue(USEROPTION__FORCE_LA_CHECK);
    }

    /**
     * Find the common token action value.
     *
     * @return The requested common token action value.
     */

    public static bool getCommonTokenAction()
    {
        return booleanValue(USEROPTION__COMMON_TOKEN_ACTION);
    }

    /**
     * Find the cache tokens value.
     *
     * @return The requested cache tokens value.
     */
    public static bool getCacheTokens()
    {
        return booleanValue(USEROPTION__CACHE_TOKENS);
    }

    /**
     * Find the keep line column value.
     *
     * @return The requested keep line column value.
     */
    public static bool getKeepLineColumn()
    {
        return booleanValue(USEROPTION__KEEP_LINE_COLUMN);
    }

    /**
     * Find the JDK version.
     *
     * @return The requested jdk version.
     */
    public static string getJdkVersion()
    {
        return stringValue(USEROPTION__JDK_VERSION);
    }

    /**
     * Should the generated code create Exceptions using a constructor taking a
     * nested exception?
     *
     * @return
     */
    public static bool getGenerateChainedException()
    {
        return booleanValue(USEROPTION__GENERATE_CHAINED_EXCEPTION);
    }

    public static bool isGenerateBoilerplateCode()
    {
        return booleanValue(USEROPTION__GENERATE_BOILERPLATE);
    }

    /**
     * As of 6.1 JavaCC now throws subclasses of {@link Exception} rather
     * than {@link Error} s (by default), as {@link Error} s typically lead to
     * the closing down of the parent VM and are only to be used in extreme
     * circumstances (failure of parsing is generally not regarded as such). If
     * this value is set to true, then then {@link Error}s will be thrown (for
     * compatibility with older .jj files)
     *
     * @return true if throws errors (legacy), false if use
     *         {@link Exception} s (better approach)
     */
    public static bool isLegacyExceptionHandling()
    {
        bool v = booleanValue(NONUSER_OPTION__LEGACY_EXCEPTION_HANDLING);
        return v;
    }

    /**
     * Should the generated code contain Generics?
     *
     * @return
     */
    public static bool getGenerateGenerics()
    {
        return booleanValue(USEROPTION__GENERATE_GENERICS);
    }

    /**
     * Should the generated code use StringBuilder rather than StringBuilder?
     *
     * @return
     */
    public static bool getGenerateStringBuilder()
    {
        return booleanValue(USEROPTION__GENERATE_STRING_BUILDER);
    }

    /**
     * Should the generated code contain Annotations?
     *
     * @return
     */
    public static bool getGenerateAnnotations()
    {
        return booleanValue(USEROPTION__GENERATE_ANNOTATIONS);
    }

    /**
     * Should the generated code class visibility public?
     *
     * @return
     */
    public static bool getSupportClassVisibilityPublic()
    {
        return booleanValue(USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC);
    }

    /**
     * Determine if the output language is at least the specified version.
     *
     * @param version
     *            the version to check against. E.g. <code>1.5</code>
     * @return true if the output version is at least the specified version.
     */
    public static bool jdkVersionAtLeast(double version)
    {
        double jdkVersion = Double.parseDouble(getJdkVersion());

        // Comparing doubles is safe here, as it is two simple assignments.
        return jdkVersion >= version;
    }

    /**
     * Return the Token's superclass.
     *
     * @return The required base class for Token.
     */
    public static string getTokenExtends()
    {
        return stringValue(USEROPTION__TOKEN_EXTENDS);
    }

    // public static string getBoilerplatePackage()
    // {
    // return stringValue(BOILERPLATE_PACKAGE);
    // }

    /**
     * Return the Token's factory class.
     *
     * @return The required factory class for Token.
     */
    public static string getTokenFactory()
    {
        return stringValue(USEROPTION__TOKEN_FACTORY);
    }

    /**
     * Return the file encoding; this will return the file.encoding system
     * property if no value was explicitly set
     *
     * @return The file encoding (e.g., UTF-8, ISO_8859-1, MacRoman)
     */
    public static string getGrammarEncoding()
    {
        if (stringValue(USEROPTION__GRAMMAR_ENCODING) == (""))
        {
            return System.getProperties().getProperty("file.encoding");
        }
        else
        {
            return stringValue(USEROPTION__GRAMMAR_ENCODING);
        }
    }

    /**
     * Find the output directory.
     *
     * @return The requested output directory.
     */
    public static string getOutputDirectory()
    {
        return stringValue(USEROPTION__OUTPUT_DIRECTORY);
    }

    public static string stringBufOrBuild()
    {
        // TODO :: CBA -- Require Unification of output language specific
        // processing into a single Enum class
        if (isOutputLanguageJava() && getGenerateStringBuilder())
        {
            return getGenerateStringBuilder() ? "StringBuilder"
                    : "StringBuilder";
        }
        else if (getOutputLanguage() == (OUTPUT_LANGUAGE__CPP))
        {
            return "StringBuilder";
        }
        else
        {
            throw new Exception(
                    "Output language type not fully implemented : "
                            + getOutputLanguage());
        }
    }

    private static readonly HashSet<string> supportedJavaTemplateTypes = new HashSet<string>();

    private static readonly HashSet<string> supportedLanguages = new HashSet<string>();

    public static bool isValidOutputLanguage(string language)
    {
        return language == null ? false : supportedLanguages.Contains(language.ToLower(Locale.ENGLISH));
    }


    public static bool isValidJavaTemplateType(string type)
    {
        return type == null ? false : supportedJavaTemplateTypes.Contains(type.ToLower(Locale.ENGLISH));
    }

    /**
     * @return the output language. default java
     */
    public static string getOutputLanguage()
    {
        return stringValue(USEROPTION__OUTPUT_LANGUAGE);
    }

    public static string getJavaTemplateType()
    {
        return stringValue(USEROPTION__JAVA_TEMPLATE_TYPE);
    }

    public static void setStringOption(string optionName, string optionValue)
    {
        optionValues.Add(optionName, optionValue);
        if (optionName.equalsIgnoreCase(USEROPTION__CPP_NAMESPACE))
        {
            processCPPNamespaceOption(optionValue);
        }
    }

    public static void processCPPNamespaceOption(string optionValue)
    {
        string ns = optionValue;
        if (ns.Length > 0)
        {
            // We also need to split it.
            StringTokenizer st = new StringTokenizer(ns, "::");
            string expanded_ns = st.nextToken() + " {";
            string ns_close = "}";
            while (st.hasMoreTokens())
            {
                expanded_ns = expanded_ns + "\nnamespace " + st.nextToken()
                        + " {";
                ns_close = ns_close + "\n}";
            }
            optionValues.Add(NONUSER_OPTION__NAMESPACE_OPEN, expanded_ns);
            optionValues.Add(NONUSER_OPTION__HAS_NAMESPACE, true);
            optionValues.Add(NONUSER_OPTION__NAMESPACE_CLOSE, ns_close);
        }
    }

    public static string getLongType()
    {
        // TODO :: CBA -- Require Unification of output language specific
        // processing into a single Enum class
        if (isOutputLanguageJava())
        {
            return "long";
        }
        else if (getOutputLanguage() == (OUTPUT_LANGUAGE__CPP))
        {
            return "unsigned long long";
        }
        else
        {
            throw new Exception("Language type not fully supported : "
                    + getOutputLanguage());
        }
    }

    public static string getBooleanType()
    {
        // TODO :: CBA -- Require Unification of output language specific
        // processing into a single Enum class
        if (isOutputLanguageJava())
        {
            return "boolean";
        }
        else if (getOutputLanguage() == (OUTPUT_LANGUAGE__CPP))
        {
            return "bool";
        }
        else
        {
            throw new Exception("Language type not fully supported : "
                    + getOutputLanguage());
        }
    }

    public static bool isOutputLanguageJava()
    {
        return getOutputLanguage().equalsIgnoreCase(OUTPUT_LANGUAGE__JAVA);
    }

    public static bool isOutputLanguageCpp()
    {
        return getOutputLanguage().equalsIgnoreCase(OUTPUT_LANGUAGE__CPP);
    }

    public static bool isTokenManagerRequiresParserAccess()
    {
        return getTokenManagerUsesParser() && (!getStatic());
    }

    /**
     * Get defined parser recursion depth limit.
     *
     * @return The requested recursion limit.
     */
    public static int getDepthLimit()
    {
        return intValue(USEROPTION__DEPTH_LIMIT);
    }

    /**
     * Get defined parser stack usage limit.
     *
     * @return The requested stack usage limit.
     */
    public static string getStackLimit()
    {
        string limit = stringValue(USEROPTION__CPP_STACK_LIMIT);
        if (limit == ("0"))
        {
            return "";
        }
        else
        {
            return limit;
        }
    }

    /**
     * Gets all the user options (in order)
     * @return
     */
    public static HashSet<OptionInfo> getUserOptions()
    {
        return userOptions;
    }
}
