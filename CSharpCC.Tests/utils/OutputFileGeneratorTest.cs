using org.javacc.parser;

namespace org.javacc.utils;




public class OutputFileGeneratorTest : CSharpCCTestCase
{

  public void testStringBuffer() {
    Options.init();

    JavaResourceTemplateLocationImpl impl = new JavaResourceTemplateLocationImpl();
    OutputFileGenerator generator = new OutputFileGenerator(
        impl.getParseExceptionTemplateResourceUrl(), new Dictionary<>());
    
    StringWriter stringWriter = new StringWriter();
    TextWriter writer = new TextWriter(stringWriter);
    generator.generate(writer);

    assertTrue(stringWriter.ToString().Contains("StringBuilder"));
    assertFalse(stringWriter.ToString().Contains("StringBuilder"));
  }

  public void testStringBuilder() {
    Options.init();
    Options.setCmdLineOption(Options.USEROPTION__GENERATE_STRING_BUILDER);

    JavaResourceTemplateLocationImpl impl = new JavaResourceTemplateLocationImpl();
    OutputFileGenerator generator = new OutputFileGenerator(
        impl.getParseExceptionTemplateResourceUrl(), new Dictionary<>());

    StringWriter stringWriter = new StringWriter();
    TextWriter writer = new TextWriter(stringWriter);
    generator.generate(writer);

    assertTrue(stringWriter.ToString().Contains("StringBuilder"));
    assertFalse(stringWriter.ToString().Contains("StringBuilder"));
  }
}
