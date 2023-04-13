namespace CSharpCC.CCTree;

public class Error : Exception
{
    public Error() : base() { }
    public Error(string? message) : base(message) { }
}
