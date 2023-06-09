namespace CSharpCC.Parser;

public interface TokenManagerCodeGenerator
{
    /**
     * Generate the code for the token manager. Note that the code generator just
     * produces a buffer.
     */
    void GenerateCode(TokenizerData tokenizerData);

    /**
     * Complete the code generation and save any output file(s).
     */
    void Finish(TokenizerData tokenizerData);
}
