using CSharpCC.CCTree;
using CSharpCC.Parser;

namespace CSharpCC.Tests;

/**
 * Test the JJTree-specific options.
 *
 * @author Kees Jan Koster &lt;kjkoster@kjkoster.org&gt;
 */
[TestClass]
public class CCTreeOptionsTest : CSharpCCTestCase
{
    [TestMethod]
    public void TestOutputDirectory()
    {
        CCTreeOptions.Init();
        CSharpCCErrors.ReInit();

        Assert.AreEqual(("."), CCTreeOptions.GetOutputDirectory());
        Assert.AreEqual(("."), CCTreeOptions.GetJJTreeOutputDirectory());

        Options.SetInputFileOption(null, null, Options.USEROPTION__OUTPUT_DIRECTORY,
        "test/output");
        Assert.AreEqual(("test/output"), CCTreeOptions.GetOutputDirectory());
        Assert.AreEqual(("test/output"), CCTreeOptions.GetJJTreeOutputDirectory());

        Options.SetInputFileOption(null, null, "JJTREE_OUTPUT_DIRECTORY",
                "test/jjtreeoutput");
        Assert.AreEqual(("test/output"), CCTreeOptions.GetOutputDirectory());
        Assert.AreEqual(("test/jjtreeoutput"), CCTreeOptions.GetJJTreeOutputDirectory());

        Assert.AreEqual(0, CSharpCCErrors.WarningCount);
        Assert.AreEqual(0, CSharpCCErrors.ErrorCount);
        Assert.AreEqual(0, CSharpCCErrors.ParseErrorCount);
        Assert.AreEqual(0, CSharpCCErrors.GetSemanticErrorCount());
    }
    [TestMethod]
    public void TestNodeFactory()
    {
        CCTreeOptions.Init();
        CSharpCCErrors.ReInit();

        Assert.AreEqual(0, CSharpCCErrors.WarningCount);
        Assert.AreEqual(0, CSharpCCErrors.ErrorCount);
        CCTreeOptions.SetInputFileOption(null, null, "NODE_FACTORY", false);
        Assert.AreEqual(CCTreeOptions.GetNodeFactory(), "");

        CCTreeOptions.Init();
        CCTreeOptions.SetInputFileOption(null, null, "NODE_FACTORY", true);
        Assert.AreEqual(CCTreeOptions.GetNodeFactory(), "*");

        CCTreeOptions.Init();
        CCTreeOptions.SetInputFileOption(null, null, "NODE_FACTORY", "mypackage.MyNode");
        Assert.AreEqual(CCTreeOptions.GetNodeFactory(), "mypackage.MyNode");

        Assert.AreEqual(0, CSharpCCErrors.WarningCount);

        Assert.AreEqual(0, CSharpCCErrors.ErrorCount);
        Assert.AreEqual(0, CSharpCCErrors.ParseErrorCount);
        Assert.AreEqual(0, CSharpCCErrors.GetSemanticErrorCount());
    }
    [TestMethod]
    public void TestNodeClass()
    {
        CCTreeOptions.Init();
        CSharpCCErrors.ReInit();

        Assert.AreEqual(0, CSharpCCErrors.WarningCount);
        Assert.AreEqual(0, CSharpCCErrors.ErrorCount);

        Assert.AreEqual("", CCTreeOptions.GetNodeClass());
        // Need some functional tests, as well.
    }
    [TestMethod]
    public void TestValidate()
    {
        CCTreeOptions.Init();
        CSharpCCErrors.ReInit();

        CCTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=Object");
        CCTreeOptions.Validate();
        Assert.AreEqual(1, CSharpCCErrors.WarningCount);

        CCTreeOptions.Init();
        CSharpCCErrors.ReInit();

        CCTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=Object");
        CCTreeOptions.SetCmdLineOption("VISITOR=true");
        CCTreeOptions.Validate();
        Assert.AreEqual(0, CSharpCCErrors.WarningCount);

        CCTreeOptions.Init();
        CSharpCCErrors.ReInit();

        CCTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=Object");
        CCTreeOptions.Validate();
        Assert.AreEqual(1, CSharpCCErrors.WarningCount);
    }
    [TestMethod]
    public void TestValidateReturnType()
    {
        CCTreeOptions.Init();
        CSharpCCErrors.ReInit();

        CCTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=String");
        CCTreeOptions.Validate();
        Assert.AreEqual(1, CSharpCCErrors.WarningCount);

        CCTreeOptions.Init();
        CSharpCCErrors.ReInit();

        CCTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=String");
        CCTreeOptions.SetCmdLineOption("VISITOR=true");
        CCTreeOptions.Validate();
        Assert.AreEqual(0, CSharpCCErrors.WarningCount);

        CCTreeOptions.Init();
        CSharpCCErrors.ReInit();

        CCTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=String");
        CCTreeOptions.Validate();
        Assert.AreEqual(1, CSharpCCErrors.WarningCount);
    }
}
