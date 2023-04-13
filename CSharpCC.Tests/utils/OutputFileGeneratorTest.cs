using CSharpCC.Parser;
using CSharpCC.Utils;

namespace CSharpCC.Tests;

[TestClass]
public class OutputFileGeneratorTest : CSharpCCTestCase
{

    [TestMethod]
    public void TestStringBuffer()
    {
        Options.Init();

        //JavaResourceTemplateLocationImpl impl = new JavaResourceTemplateLocationImpl();
        var generator = new OutputFileGenerator(
            "test.txt", new Dictionary<string, object>());

        var stringWriter = new StringWriter();
        generator.Generate(stringWriter);

        Assert.IsTrue(stringWriter.ToString().Contains("StringBuilder"));
        Assert.IsFalse(stringWriter.ToString().Contains("StringBuilder"));
    }

    [TestMethod]
    public void TestStringBuilder()
    {
        Options.Init();
        Options.SetCmdLineOption(Options.USEROPTION__GENERATE_STRING_BUILDER);

        //JavaResourceTemplateLocationImpl impl = new JavaResourceTemplateLocationImpl();
        OutputFileGenerator generator = new OutputFileGenerator(
            "test.txt", new Dictionary<string, object>());

        var writer = new StringWriter();
        generator.Generate(writer);

        Assert.IsTrue(writer.ToString().Contains("StringBuilder"));
        Assert.IsFalse(writer.ToString().Contains("StringBuilder"));
    }
}
