namespace CSharpCC.Parser;

public interface ParserCodeGenerator
{
    /**
     * Generate the code for the parser. Note that the code generator just
     * produces a buffer.
     */
    void GenerateCode(CodeGenerator codeGenerator, ParserData tokenizerData);

    /**
     * Complete the code generation and save any output file(s).
     */
    void Finish(CodeGenerator codeGenerator, ParserData tokenizerData);
}
