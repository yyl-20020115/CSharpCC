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

        Assert.AreEqual(("."), JJTreeOptions.getOutputDirectory());
        Assert.AreEqual(("."), JJTreeOptions.GetJJTreeOutputDirectory());

        Options.setInputFileOption(null, null, Options.USEROPTION__OUTPUT_DIRECTORY,
        "test/output");
        Assert.AreEqual(("test/output"), JJTreeOptions.getOutputDirectory());
        Assert.AreEqual(("test/output"), JJTreeOptions.GetJJTreeOutputDirectory());

        Options.setInputFileOption(null, null, "JJTREE_OUTPUT_DIRECTORY",
                "test/jjtreeoutput");
        Assert.AreEqual(("test/output"), JJTreeOptions.getOutputDirectory());
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
      JJTreeOptions.setInputFileOption(null, null, "NODE_FACTORY", false);
      Assert.AreEqual(JJTreeOptions.GetNodeFactory(), "");

      JJTreeOptions.Init();
      JJTreeOptions.setInputFileOption(null, null, "NODE_FACTORY", true);
      Assert.AreEqual(JJTreeOptions.GetNodeFactory(), "*");

      JJTreeOptions.Init();
      JJTreeOptions.setInputFileOption(null, null, "NODE_FACTORY", "mypackage.MyNode");
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

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=Object");
      JJTreeOptions.Validate();
      Assert.AreEqual(1, JavaCCErrors.GetWarningCount());

      JJTreeOptions.Init();
      JavaCCErrors.ReInit();

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=Object");
      JJTreeOptions.setCmdLineOption("VISITOR=true");
      JJTreeOptions.Validate();
      Assert.AreEqual(0, JavaCCErrors.GetWarningCount());

      JJTreeOptions.Init();
      JavaCCErrors.ReInit();

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=Object");
      JJTreeOptions.Validate();
      Assert.AreEqual(1, JavaCCErrors.GetWarningCount());
    }

    public void testValidateReturnType() {
      JJTreeOptions.Init();
      JavaCCErrors.ReInit();

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=String");
      JJTreeOptions.Validate();
      Assert.AreEqual(1, JavaCCErrors.GetWarningCount());

      JJTreeOptions.Init();
      JavaCCErrors.ReInit();

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=String");
      JJTreeOptions.setCmdLineOption("VISITOR=true");
      JJTreeOptions.Validate();
      Assert.AreEqual(0, JavaCCErrors.GetWarningCount());

      JJTreeOptions.Init();
      JavaCCErrors.ReInit();

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=String");
      JJTreeOptions.Validate();
      Assert.AreEqual(1, JavaCCErrors.GetWarningCount());
    }
  }
