using CSharpCC.Parser;
using System.IO;

namespace CSharpCC.Utils;


public class OutputFileGeneratorTest : CSharpCCTestCase
{

  public void testStringBuffer() {
    Options.Init();

    //JavaResourceTemplateLocationImpl impl = new JavaResourceTemplateLocationImpl();
    OutputFileGenerator generator = new OutputFileGenerator(
        "test.txt", new Dictionary<string,object>());
    
    TextWriter stringWriter = new StringWriter();
    generator.Generate(stringWriter);

    Assert.IsTrue(stringWriter.ToString().Contains("StringBuilder"));
    Assert.IsFalse(stringWriter.ToString().Contains("StringBuilder"));
  }

  public void testStringBuilder() {
    Options.Init();
    Options.SetCmdLineOption(Options.USEROPTION__GENERATE_STRING_BUILDER);

    //JavaResourceTemplateLocationImpl impl = new JavaResourceTemplateLocationImpl();
    OutputFileGenerator generator = new OutputFileGenerator(
        "test.txt", new Dictionary<string, object>());

    TextWriter writer = new StringWriter();
    generator.Generate(writer);

    Assert.IsTrue(writer.ToString().Contains("StringBuilder"));
    Assert.IsFalse(writer.ToString().Contains("StringBuilder"));
  }
}
