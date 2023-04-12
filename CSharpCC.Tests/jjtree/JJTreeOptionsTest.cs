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
        JavaCCErrors.reInit();

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

        Assert.AreEqual(0, JavaCCErrors.get_warning_count());
        Assert.AreEqual(0, JavaCCErrors.get_error_count());
        Assert.AreEqual(0, JavaCCErrors.get_parse_error_count());
        Assert.AreEqual(0, JavaCCErrors.get_semantic_error_count());
    }

    public void testNodeFactory() {
      JJTreeOptions.Init();
      JavaCCErrors.reInit();

      Assert.AreEqual(0, JavaCCErrors.get_warning_count());
      Assert.AreEqual(0, JavaCCErrors.get_error_count());
      JJTreeOptions.setInputFileOption(null, null, "NODE_FACTORY", false);
      Assert.AreEqual(JJTreeOptions.GetNodeFactory(), "");

      JJTreeOptions.Init();
      JJTreeOptions.setInputFileOption(null, null, "NODE_FACTORY", true);
      Assert.AreEqual(JJTreeOptions.GetNodeFactory(), "*");

      JJTreeOptions.Init();
      JJTreeOptions.setInputFileOption(null, null, "NODE_FACTORY", "mypackage.MyNode");
      Assert.AreEqual(JJTreeOptions.GetNodeFactory(), "mypackage.MyNode");

      Assert.AreEqual(0, JavaCCErrors.get_warning_count());

      Assert.AreEqual(0, JavaCCErrors.get_error_count());
      Assert.AreEqual(0, JavaCCErrors.get_parse_error_count());
      Assert.AreEqual(0, JavaCCErrors.get_semantic_error_count());
    }

    public void testNodeClass() {
      JJTreeOptions.Init();
      JavaCCErrors.reInit();

      Assert.AreEqual(0, JavaCCErrors.get_warning_count());
      Assert.AreEqual(0, JavaCCErrors.get_error_count());

      Assert.AreEqual("", JJTreeOptions.GetNodeClass());
      // Need some functional tests, as well.
    }

    public void testValidate() {
      JJTreeOptions.Init();
      JavaCCErrors.reInit();

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=Object");
      JJTreeOptions.Validate();
      Assert.AreEqual(1, JavaCCErrors.get_warning_count());

      JJTreeOptions.Init();
      JavaCCErrors.reInit();

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=Object");
      JJTreeOptions.setCmdLineOption("VISITOR=true");
      JJTreeOptions.Validate();
      Assert.AreEqual(0, JavaCCErrors.get_warning_count());

      JJTreeOptions.Init();
      JavaCCErrors.reInit();

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=Object");
      JJTreeOptions.Validate();
      Assert.AreEqual(1, JavaCCErrors.get_warning_count());
    }

    public void testValidateReturnType() {
      JJTreeOptions.Init();
      JavaCCErrors.reInit();

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=String");
      JJTreeOptions.Validate();
      Assert.AreEqual(1, JavaCCErrors.get_warning_count());

      JJTreeOptions.Init();
      JavaCCErrors.reInit();

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=String");
      JJTreeOptions.setCmdLineOption("VISITOR=true");
      JJTreeOptions.Validate();
      Assert.AreEqual(0, JavaCCErrors.get_warning_count());

      JJTreeOptions.Init();
      JavaCCErrors.reInit();

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=String");
      JJTreeOptions.Validate();
      Assert.AreEqual(1, JavaCCErrors.get_warning_count());
    }
  }
