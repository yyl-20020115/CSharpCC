using CSharpCC.Parser;

namespace CSharpCC.CCTree;




/**
 * Test the JJTree-specific options.
 *
 * @author Kees Jan Koster &lt;kjkoster@kjkoster.org&gt;
 */
public class CCTreeOptionsTest : CSharpCCTestCase
{
    public void testOutputDirectory() {
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

        Assert.AreEqual(0, CSharpCCErrors.GetWarningCount());
        Assert.AreEqual(0, CSharpCCErrors.GetErrorCount());
        Assert.AreEqual(0, CSharpCCErrors.GetParseErrorCount());
        Assert.AreEqual(0, CSharpCCErrors.GetSemanticErrorCount());
    }

    public void testNodeFactory() {
      CCTreeOptions.Init();
      CSharpCCErrors.ReInit();

      Assert.AreEqual(0, CSharpCCErrors.GetWarningCount());
      Assert.AreEqual(0, CSharpCCErrors.GetErrorCount());
      CCTreeOptions.SetInputFileOption(null, null, "NODE_FACTORY", false);
      Assert.AreEqual(CCTreeOptions.GetNodeFactory(), "");

      CCTreeOptions.Init();
      CCTreeOptions.SetInputFileOption(null, null, "NODE_FACTORY", true);
      Assert.AreEqual(CCTreeOptions.GetNodeFactory(), "*");

      CCTreeOptions.Init();
      CCTreeOptions.SetInputFileOption(null, null, "NODE_FACTORY", "mypackage.MyNode");
      Assert.AreEqual(CCTreeOptions.GetNodeFactory(), "mypackage.MyNode");

      Assert.AreEqual(0, CSharpCCErrors.GetWarningCount());

      Assert.AreEqual(0, CSharpCCErrors.GetErrorCount());
      Assert.AreEqual(0, CSharpCCErrors.GetParseErrorCount());
      Assert.AreEqual(0, CSharpCCErrors.GetSemanticErrorCount());
    }

    public void testNodeClass() {
      CCTreeOptions.Init();
      CSharpCCErrors.ReInit();

      Assert.AreEqual(0, CSharpCCErrors.GetWarningCount());
      Assert.AreEqual(0, CSharpCCErrors.GetErrorCount());

      Assert.AreEqual("", CCTreeOptions.GetNodeClass());
      // Need some functional tests, as well.
    }

    public void testValidate() {
      CCTreeOptions.Init();
      CSharpCCErrors.ReInit();

      CCTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=Object");
      CCTreeOptions.Validate();
      Assert.AreEqual(1, CSharpCCErrors.GetWarningCount());

      CCTreeOptions.Init();
      CSharpCCErrors.ReInit();

      CCTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=Object");
      CCTreeOptions.SetCmdLineOption("VISITOR=true");
      CCTreeOptions.Validate();
      Assert.AreEqual(0, CSharpCCErrors.GetWarningCount());

      CCTreeOptions.Init();
      CSharpCCErrors.ReInit();

      CCTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=Object");
      CCTreeOptions.Validate();
      Assert.AreEqual(1, CSharpCCErrors.GetWarningCount());
    }

    public void testValidateReturnType() {
      CCTreeOptions.Init();
      CSharpCCErrors.ReInit();

      CCTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=String");
      CCTreeOptions.Validate();
      Assert.AreEqual(1, CSharpCCErrors.GetWarningCount());

      CCTreeOptions.Init();
      CSharpCCErrors.ReInit();

      CCTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=String");
      CCTreeOptions.SetCmdLineOption("VISITOR=true");
      CCTreeOptions.Validate();
      Assert.AreEqual(0, CSharpCCErrors.GetWarningCount());

      CCTreeOptions.Init();
      CSharpCCErrors.ReInit();

      CCTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=String");
      CCTreeOptions.Validate();
      Assert.AreEqual(1, CSharpCCErrors.GetWarningCount());
    }
  }
