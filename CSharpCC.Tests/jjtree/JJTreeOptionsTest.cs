using org.javacc.parser;

namespace org.javacc.jjtree;




/**
 * Test the JJTree-specific options.
 *
 * @author Kees Jan Koster &lt;kjkoster@kjkoster.org&gt;
 */
public class JJTreeOptionsTest : CSharpCCTestCase
{
    public void testOutputDirectory() {
        JJTreeOptions.Init();
        JavaCCErrors.ReInit();

        Assert.AreEqual(("."), JJTreeOptions.GetOutputDirectory());
        Assert.AreEqual(("."), JJTreeOptions.GetJJTreeOutputDirectory());

        Options.SetInputFileOption(null, null, Options.USEROPTION__OUTPUT_DIRECTORY,
        "test/output");
        Assert.AreEqual(("test/output"), JJTreeOptions.GetOutputDirectory());
        Assert.AreEqual(("test/output"), JJTreeOptions.GetJJTreeOutputDirectory());

        Options.SetInputFileOption(null, null, "JJTREE_OUTPUT_DIRECTORY",
                "test/jjtreeoutput");
        Assert.AreEqual(("test/output"), JJTreeOptions.GetOutputDirectory());
        Assert.AreEqual(("test/jjtreeoutput"), JJTreeOptions.GetJJTreeOutputDirectory());

        Assert.AreEqual(0, JavaCCErrors.GetWarningCount());
        Assert.AreEqual(0, JavaCCErrors.GetErrorCount());
        Assert.AreEqual(0, JavaCCErrors.GetParseErrorCount());
        Assert.AreEqual(0, JavaCCErrors.GetSemanticErrorCount());
    }

    public void testNodeFactory() {
      JJTreeOptions.Init();
      JavaCCErrors.ReInit();

      Assert.AreEqual(0, JavaCCErrors.GetWarningCount());
      Assert.AreEqual(0, JavaCCErrors.GetErrorCount());
      JJTreeOptions.SetInputFileOption(null, null, "NODE_FACTORY", false);
      Assert.AreEqual(JJTreeOptions.GetNodeFactory(), "");

      JJTreeOptions.Init();
      JJTreeOptions.SetInputFileOption(null, null, "NODE_FACTORY", true);
      Assert.AreEqual(JJTreeOptions.GetNodeFactory(), "*");

      JJTreeOptions.Init();
      JJTreeOptions.SetInputFileOption(null, null, "NODE_FACTORY", "mypackage.MyNode");
      Assert.AreEqual(JJTreeOptions.GetNodeFactory(), "mypackage.MyNode");

      Assert.AreEqual(0, JavaCCErrors.GetWarningCount());

      Assert.AreEqual(0, JavaCCErrors.GetErrorCount());
      Assert.AreEqual(0, JavaCCErrors.GetParseErrorCount());
      Assert.AreEqual(0, JavaCCErrors.GetSemanticErrorCount());
    }

    public void testNodeClass() {
      JJTreeOptions.Init();
      JavaCCErrors.ReInit();

      Assert.AreEqual(0, JavaCCErrors.GetWarningCount());
      Assert.AreEqual(0, JavaCCErrors.GetErrorCount());

      Assert.AreEqual("", JJTreeOptions.GetNodeClass());
      // Need some functional tests, as well.
    }

    public void testValidate() {
      JJTreeOptions.Init();
      JavaCCErrors.ReInit();

      JJTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=Object");
      JJTreeOptions.Validate();
      Assert.AreEqual(1, JavaCCErrors.GetWarningCount());

      JJTreeOptions.Init();
      JavaCCErrors.ReInit();

      JJTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=Object");
      JJTreeOptions.SetCmdLineOption("VISITOR=true");
      JJTreeOptions.Validate();
      Assert.AreEqual(0, JavaCCErrors.GetWarningCount());

      JJTreeOptions.Init();
      JavaCCErrors.ReInit();

      JJTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=Object");
      JJTreeOptions.Validate();
      Assert.AreEqual(1, JavaCCErrors.GetWarningCount());
    }

    public void testValidateReturnType() {
      JJTreeOptions.Init();
      JavaCCErrors.ReInit();

      JJTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=String");
      JJTreeOptions.Validate();
      Assert.AreEqual(1, JavaCCErrors.GetWarningCount());

      JJTreeOptions.Init();
      JavaCCErrors.ReInit();

      JJTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=String");
      JJTreeOptions.SetCmdLineOption("VISITOR=true");
      JJTreeOptions.Validate();
      Assert.AreEqual(0, JavaCCErrors.GetWarningCount());

      JJTreeOptions.Init();
      JavaCCErrors.ReInit();

      JJTreeOptions.SetCmdLineOption("VISITOR_DATA_TYPE=String");
      JJTreeOptions.Validate();
      Assert.AreEqual(1, JavaCCErrors.GetWarningCount());
    }
  }
