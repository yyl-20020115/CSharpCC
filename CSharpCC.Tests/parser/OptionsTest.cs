namespace org.javacc.parser;



/**
 * Test cases to prod at the valitity of Options a little.
 *
 * @author Kees Jan Koster &lt;kjkoster@kjkoster.org&gt;
 */
public class OptionsTest : CSharpCCTestCase
{
    public void testDefaults()
    {
        Options.Init();
        JavaCCErrors.reInit();

        //        Assert.AreEqual(49, Options.optionValues.Count);

        Assert.AreEqual(true, Options.getBuildParser());
        Assert.AreEqual(true, Options.getBuildTokenManager());
        Assert.AreEqual(false, Options.getCacheTokens());
        Assert.AreEqual(false, Options.getCommonTokenAction());
        Assert.AreEqual(false, Options.getDebugLookahead());
        Assert.AreEqual(false, Options.getDebugParser());
        Assert.AreEqual(false, Options.getDebugTokenManager());
        Assert.AreEqual(true, Options.getErrorReporting());
        Assert.AreEqual(false, Options.getForceLaCheck());
        Assert.AreEqual(false, Options.getIgnoreCase());
        Assert.AreEqual(false, Options.getJavaUnicodeEscape());
        Assert.AreEqual(true, Options.getKeepLineColumn());
        Assert.AreEqual(true, Options.getSanityCheck());
        Assert.AreEqual(true, Options.getStatic());
        Assert.AreEqual(false, Options.getUnicodeInput());
        Assert.AreEqual(false, Options.getUserCharStream());
        Assert.AreEqual(false, Options.getUserTokenManager());
        Assert.AreEqual(false, Options.getTokenManagerUsesParser());

        Assert.AreEqual(2, Options.getChoiceAmbiguityCheck());
        Assert.AreEqual(1, Options.getLookahead());
        Assert.AreEqual(1, Options.getOtherAmbiguityCheck());

        Assert.AreEqual("1.5", Options.getJdkVersion());
        Assert.AreEqual(".", Options.getOutputDirectory());
        Assert.AreEqual("", Options.getTokenExtends());
        Assert.AreEqual("", Options.getTokenFactory());
        //Assert.AreEqual(System.getProperties().get("file.encoding"), Options.getGrammarEncoding());

        Assert.AreEqual(0, JavaCCErrors.get_warning_count());
        Assert.AreEqual(0, JavaCCErrors.get_error_count());
        Assert.AreEqual(0, JavaCCErrors.get_parse_error_count());
        Assert.AreEqual(0, JavaCCErrors.get_semantic_error_count());
    }

    public void testSetBooleanOption()
    {
        Options.Init();
        JavaCCErrors.reInit();

        Assert.AreEqual(true, Options.getStatic());
        Options.setCmdLineOption("-NOSTATIC");
        Assert.AreEqual(false, Options.getStatic());

        Assert.AreEqual(false, Options.getJavaUnicodeEscape());
        Options.setCmdLineOption("-JAVA_UNICODE_ESCAPE:true");
        Assert.AreEqual(true, Options.getJavaUnicodeEscape());

        Assert.AreEqual(true, Options.getSanityCheck());
        Options.setCmdLineOption("-SANITY_CHECK=false");
        Assert.AreEqual(false, Options.getSanityCheck());

        Assert.AreEqual(0, JavaCCErrors.get_warning_count());
        Assert.AreEqual(0, JavaCCErrors.get_error_count());
        Assert.AreEqual(0, JavaCCErrors.get_parse_error_count());
        Assert.AreEqual(0, JavaCCErrors.get_semantic_error_count());
    }


    public void testIntBooleanOption()
    {
        Options.Init();
        JavaCCErrors.reInit();

        Assert.AreEqual(1, Options.getLookahead());
        Options.setCmdLineOption("LOOKAHEAD=2");
        Assert.AreEqual(2, Options.getLookahead());
        Assert.AreEqual(0, JavaCCErrors.get_warning_count());
        Options.setCmdLineOption("LOOKAHEAD=0");
        Assert.AreEqual(2, Options.getLookahead());
        Assert.AreEqual(0, JavaCCErrors.get_warning_count());
        Options.setInputFileOption(null, null, Options.USEROPTION__LOOKAHEAD, (0));
        Assert.AreEqual(2, Options.getLookahead());
        Assert.AreEqual(1, JavaCCErrors.get_warning_count());

        Assert.AreEqual(0, JavaCCErrors.get_error_count());
        Assert.AreEqual(0, JavaCCErrors.get_parse_error_count());
        Assert.AreEqual(0, JavaCCErrors.get_semantic_error_count());
    }

    public void testSetStringOption()
    {
        Options.Init();
        JavaCCErrors.reInit();

        Assert.AreEqual("", Options.getTokenExtends());
        Options.setCmdLineOption("-TOKEN_EXTENDS=java.lang.Object");
        Assert.AreEqual("java.lang.Object", Options.getTokenExtends());
        Options.setInputFileOption(null, null, Options.USEROPTION__TOKEN_EXTENDS, "Object");
        // File option does not override cmd line
        Assert.AreEqual("java.lang.Object", Options.getTokenExtends());

        Options.Init();
        JavaCCErrors.reInit();

        Options.setInputFileOption(null, null, Options.USEROPTION__TOKEN_EXTENDS, "Object");
        Assert.AreEqual("Object", Options.getTokenExtends());
        Options.setCmdLineOption("-TOKEN_EXTENDS=java.lang.Object");
        Assert.AreEqual("java.lang.Object", Options.getTokenExtends());
    }

    public void testSetNonexistentOption()
    {
        Options.Init();
        JavaCCErrors.reInit();

        Assert.AreEqual(0, JavaCCErrors.get_warning_count());
        Options.setInputFileOption(null, null, "NONEXISTENTOPTION", true);
        Assert.AreEqual(1, JavaCCErrors.get_warning_count());

        Assert.AreEqual(0, JavaCCErrors.get_error_count());
        Assert.AreEqual(0, JavaCCErrors.get_parse_error_count());
        Assert.AreEqual(0, JavaCCErrors.get_semantic_error_count());
    }

    public void testSetWrongTypeForOption()
    {
        Options.Init();
        JavaCCErrors.reInit();

        Assert.AreEqual(0, JavaCCErrors.get_warning_count());
        Assert.AreEqual(0, JavaCCErrors.get_error_count());
        Options.setInputFileOption(null, null, Options.USEROPTION__STATIC, (8));
        Assert.AreEqual(1, JavaCCErrors.get_warning_count());

        Assert.AreEqual(0, JavaCCErrors.get_error_count());
        Assert.AreEqual(0, JavaCCErrors.get_parse_error_count());
        Assert.AreEqual(0, JavaCCErrors.get_semantic_error_count());
    }

    public void testNormalize()
    {
        Options.Init();
        JavaCCErrors.reInit();

        Assert.AreEqual(false, Options.getDebugLookahead());
        Assert.AreEqual(false, Options.getDebugParser());

        Options.setCmdLineOption("-DEBUG_LOOKAHEAD=TRUE");
        Options.normalize();

        Assert.AreEqual(true, Options.getDebugLookahead());
        Assert.AreEqual(true, Options.getDebugParser());

        Assert.AreEqual(0, JavaCCErrors.get_warning_count());
        Assert.AreEqual(0, JavaCCErrors.get_error_count());
        Assert.AreEqual(0, JavaCCErrors.get_parse_error_count());
        Assert.AreEqual(0, JavaCCErrors.get_semantic_error_count());
    }

    public void testOptionsString()
    {
        Options.Init();
        JavaCCErrors.reInit();

        Options.setCmdLineOption("-STATIC=False");
        Options.setCmdLineOption("-IGNORE_CASE=True");
        String[] options = {
           Options.USEROPTION__STATIC,
           Options.USEROPTION__IGNORE_CASE
        };
        string optionString = Options.getOptionsString(options);
        Assert.AreEqual("STATIC=false,IGNORE_CASE=true", optionString);
    }
}
