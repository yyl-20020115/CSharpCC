using CSharpCC.Parser;

namespace CSharpCC.Tests;

/**
 * Test cases to prod at the valitity of Options a little.
 *
 * @author Kees Jan Koster &lt;kjkoster@kjkoster.org&gt;
 */
[TestClass]
public class OptionsTest : CSharpCCTestCase
{
    [TestMethod]
    public void TestDefaults()
    {
        Options.Init();
        CSharpCCErrors.ReInit();

        //        Assert.AreEqual(49, Options.optionValues.Count);

        Assert.AreEqual(true, Options.GetBuildParser());
        Assert.AreEqual(true, Options.GetBuildTokenManager());
        Assert.AreEqual(false, Options.GetCacheTokens());
        Assert.AreEqual(false, Options.GetCommonTokenAction());
        Assert.AreEqual(false, Options.GetDebugLookahead());
        Assert.AreEqual(false, Options.GetDebugParser());
        Assert.AreEqual(false, Options.GetDebugTokenManager());
        Assert.AreEqual(true, Options.GetErrorReporting());
        Assert.AreEqual(false, Options.GetForceLaCheck());
        Assert.AreEqual(false, Options.GetIgnoreCase());
        Assert.AreEqual(false, Options.GetJavaUnicodeEscape());
        Assert.AreEqual(true, Options.GetKeepLineColumn());
        Assert.AreEqual(true, Options.GetSanityCheck());
        Assert.AreEqual(true, Options.GetStatic());
        Assert.AreEqual(false, Options.GetUnicodeInput());
        Assert.AreEqual(false, Options.GetUserCharStream());
        Assert.AreEqual(false, Options.GetUserTokenManager());
        Assert.AreEqual(false, Options.GetTokenManagerUsesParser());

        Assert.AreEqual(2, Options.GetChoiceAmbiguityCheck());
        Assert.AreEqual(1, Options.GetLookahead());
        Assert.AreEqual(1, Options.GetOtherAmbiguityCheck());

        Assert.AreEqual("1.5", Options.GetJdkVersion());
        Assert.AreEqual(".", Options.GetOutputDirectory());
        Assert.AreEqual("", Options.GetTokenExtends());
        Assert.AreEqual("", Options.GetTokenFactory());
        //Assert.AreEqual(System.getProperties().get("file.encoding"), Options.getGrammarEncoding());

        Assert.AreEqual(0, CSharpCCErrors.GetWarningCount());
        Assert.AreEqual(0, CSharpCCErrors.GetErrorCount());
        Assert.AreEqual(0, CSharpCCErrors.GetParseErrorCount());
        Assert.AreEqual(0, CSharpCCErrors.GetSemanticErrorCount());
    }
    [TestMethod]
    public void TestSetBooleanOption()
    {
        Options.Init();
        CSharpCCErrors.ReInit();

        Assert.AreEqual(true, Options.GetStatic());
        Options.SetCmdLineOption("-NOSTATIC");
        Assert.AreEqual(false, Options.GetStatic());

        Assert.AreEqual(false, Options.GetJavaUnicodeEscape());
        Options.SetCmdLineOption("-JAVA_UNICODE_ESCAPE:true");
        Assert.AreEqual(true, Options.GetJavaUnicodeEscape());

        Assert.AreEqual(true, Options.GetSanityCheck());
        Options.SetCmdLineOption("-SANITY_CHECK=false");
        Assert.AreEqual(false, Options.GetSanityCheck());

        Assert.AreEqual(0, CSharpCCErrors.GetWarningCount());
        Assert.AreEqual(0, CSharpCCErrors.GetErrorCount());
        Assert.AreEqual(0, CSharpCCErrors.GetParseErrorCount());
        Assert.AreEqual(0, CSharpCCErrors.GetSemanticErrorCount());
    }

    [TestMethod]
    public void TestIntBooleanOption()
    {
        Options.Init();
        CSharpCCErrors.ReInit();

        Assert.AreEqual(1, Options.GetLookahead());
        Options.SetCmdLineOption("LOOKAHEAD=2");
        Assert.AreEqual(2, Options.GetLookahead());
        Assert.AreEqual(0, CSharpCCErrors.GetWarningCount());
        Options.SetCmdLineOption("LOOKAHEAD=0");
        Assert.AreEqual(2, Options.GetLookahead());
        Assert.AreEqual(0, CSharpCCErrors.GetWarningCount());
        Options.SetInputFileOption(null, null, Options.USEROPTION__LOOKAHEAD, (0));
        Assert.AreEqual(2, Options.GetLookahead());
        Assert.AreEqual(1, CSharpCCErrors.GetWarningCount());

        Assert.AreEqual(0, CSharpCCErrors.GetErrorCount());
        Assert.AreEqual(0, CSharpCCErrors.GetParseErrorCount());
        Assert.AreEqual(0, CSharpCCErrors.GetSemanticErrorCount());
    }
    [TestMethod]
    public void TestSetStringOption()
    {
        Options.Init();
        CSharpCCErrors.ReInit();

        Assert.AreEqual("", Options.GetTokenExtends());
        Options.SetCmdLineOption("-TOKEN_EXTENDS=java.lang.Object");
        Assert.AreEqual("java.lang.Object", Options.GetTokenExtends());
        Options.SetInputFileOption(null, null, Options.USEROPTION__TOKEN_EXTENDS, "Object");
        // File option does not override cmd line
        Assert.AreEqual("java.lang.Object", Options.GetTokenExtends());

        Options.Init();
        CSharpCCErrors.ReInit();

        Options.SetInputFileOption(null, null, Options.USEROPTION__TOKEN_EXTENDS, "Object");
        Assert.AreEqual("Object", Options.GetTokenExtends());
        Options.SetCmdLineOption("-TOKEN_EXTENDS=java.lang.Object");
        Assert.AreEqual("java.lang.Object", Options.GetTokenExtends());
    }
    [TestMethod]
    public void TestSetNonexistentOption()
    {
        Options.Init();
        CSharpCCErrors.ReInit();

        Assert.AreEqual(0, CSharpCCErrors.GetWarningCount());
        Options.SetInputFileOption(null, null, "NONEXISTENTOPTION", true);
        Assert.AreEqual(1, CSharpCCErrors.GetWarningCount());

        Assert.AreEqual(0, CSharpCCErrors.GetErrorCount());
        Assert.AreEqual(0, CSharpCCErrors.GetParseErrorCount());
        Assert.AreEqual(0, CSharpCCErrors.GetSemanticErrorCount());
    }
    [TestMethod]
    public void TestSetWrongTypeForOption()
    {
        Options.Init();
        CSharpCCErrors.ReInit();

        Assert.AreEqual(0, CSharpCCErrors.GetWarningCount());
        Assert.AreEqual(0, CSharpCCErrors.GetErrorCount());
        Options.SetInputFileOption(null, null, Options.USEROPTION__STATIC, (8));
        Assert.AreEqual(1, CSharpCCErrors.GetWarningCount());

        Assert.AreEqual(0, CSharpCCErrors.GetErrorCount());
        Assert.AreEqual(0, CSharpCCErrors.GetParseErrorCount());
        Assert.AreEqual(0, CSharpCCErrors.GetSemanticErrorCount());
    }
    [TestMethod]
    public void TestNormalize()
    {
        Options.Init();
        CSharpCCErrors.ReInit();

        Assert.AreEqual(false, Options.GetDebugLookahead());
        Assert.AreEqual(false, Options.GetDebugParser());

        Options.SetCmdLineOption("-DEBUG_LOOKAHEAD=TRUE");
        Options.Normalize();

        Assert.AreEqual(true, Options.GetDebugLookahead());
        Assert.AreEqual(true, Options.GetDebugParser());

        Assert.AreEqual(0, CSharpCCErrors.GetWarningCount());
        Assert.AreEqual(0, CSharpCCErrors.GetErrorCount());
        Assert.AreEqual(0, CSharpCCErrors.GetParseErrorCount());
        Assert.AreEqual(0, CSharpCCErrors.GetSemanticErrorCount());
    }
    [TestMethod]
    public void TestOptionsString()
    {
        Options.Init();
        CSharpCCErrors.ReInit();

        Options.SetCmdLineOption("-STATIC=False");
        Options.SetCmdLineOption("-IGNORE_CASE=True");
        string[] options = {
           Options.USEROPTION__STATIC,
           Options.USEROPTION__IGNORE_CASE
        };
        string optionString = Options.GetOptionsString(options);
        Assert.AreEqual("STATIC=false,IGNORE_CASE=true", optionString);
    }
}
