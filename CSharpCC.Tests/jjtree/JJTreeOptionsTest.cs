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
        JJTreeOptions.init();
        JavaCCErrors.reInit();

        Assert.AreEqual(("."), JJTreeOptions.getOutputDirectory());
        Assert.AreEqual(("."), JJTreeOptions.getJJTreeOutputDirectory());

        Options.setInputFileOption(null, null, Options.USEROPTION__OUTPUT_DIRECTORY,
        "test/output");
        Assert.AreEqual(("test/output"), JJTreeOptions.getOutputDirectory());
        Assert.AreEqual(("test/output"), JJTreeOptions.getJJTreeOutputDirectory());

        Options.setInputFileOption(null, null, "JJTREE_OUTPUT_DIRECTORY",
                "test/jjtreeoutput");
        Assert.AreEqual(("test/output"), JJTreeOptions.getOutputDirectory());
        Assert.AreEqual(("test/jjtreeoutput"), JJTreeOptions.getJJTreeOutputDirectory());

        Assert.AreEqual(0, JavaCCErrors.get_warning_count());
        Assert.AreEqual(0, JavaCCErrors.get_error_count());
        Assert.AreEqual(0, JavaCCErrors.get_parse_error_count());
        Assert.AreEqual(0, JavaCCErrors.get_semantic_error_count());
    }

    public void testNodeFactory() {
      JJTreeOptions.init();
      JavaCCErrors.reInit();

      Assert.AreEqual(0, JavaCCErrors.get_warning_count());
      Assert.AreEqual(0, JavaCCErrors.get_error_count());
      JJTreeOptions.setInputFileOption(null, null, "NODE_FACTORY", false);
      Assert.AreEqual(JJTreeOptions.getNodeFactory(), "");

      JJTreeOptions.init();
      JJTreeOptions.setInputFileOption(null, null, "NODE_FACTORY", true);
      Assert.AreEqual(JJTreeOptions.getNodeFactory(), "*");

      JJTreeOptions.init();
      JJTreeOptions.setInputFileOption(null, null, "NODE_FACTORY", "mypackage.MyNode");
      Assert.AreEqual(JJTreeOptions.getNodeFactory(), "mypackage.MyNode");

      Assert.AreEqual(0, JavaCCErrors.get_warning_count());

      Assert.AreEqual(0, JavaCCErrors.get_error_count());
      Assert.AreEqual(0, JavaCCErrors.get_parse_error_count());
      Assert.AreEqual(0, JavaCCErrors.get_semantic_error_count());
    }

    public void testNodeClass() {
      JJTreeOptions.init();
      JavaCCErrors.reInit();

      Assert.AreEqual(0, JavaCCErrors.get_warning_count());
      Assert.AreEqual(0, JavaCCErrors.get_error_count());

      Assert.AreEqual("", JJTreeOptions.getNodeClass());
      // Need some functional tests, as well.
    }

    public void testValidate() {
      JJTreeOptions.init();
      JavaCCErrors.reInit();

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=Object");
      JJTreeOptions.validate();
      Assert.AreEqual(1, JavaCCErrors.get_warning_count());

      JJTreeOptions.init();
      JavaCCErrors.reInit();

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=Object");
      JJTreeOptions.setCmdLineOption("VISITOR=true");
      JJTreeOptions.validate();
      Assert.AreEqual(0, JavaCCErrors.get_warning_count());

      JJTreeOptions.init();
      JavaCCErrors.reInit();

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=Object");
      JJTreeOptions.validate();
      Assert.AreEqual(1, JavaCCErrors.get_warning_count());
    }

    public void testValidateReturnType() {
      JJTreeOptions.init();
      JavaCCErrors.reInit();

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=String");
      JJTreeOptions.validate();
      Assert.AreEqual(1, JavaCCErrors.get_warning_count());

      JJTreeOptions.init();
      JavaCCErrors.reInit();

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=String");
      JJTreeOptions.setCmdLineOption("VISITOR=true");
      JJTreeOptions.validate();
      Assert.AreEqual(0, JavaCCErrors.get_warning_count());

      JJTreeOptions.init();
      JavaCCErrors.reInit();

      JJTreeOptions.setCmdLineOption("VISITOR_DATA_TYPE=String");
      JJTreeOptions.validate();
      Assert.AreEqual(1, JavaCCErrors.get_warning_count());
    }
  }
