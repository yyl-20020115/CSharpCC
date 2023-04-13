namespace CSharpCC.Parser;


public class ParserData
{
    public enum LookaheadType
    {
        TOKEN,
        PRODUCTION,
        SEQUENCE,
        CHOICE,
        ZERORORMORE
    }

    public class LookaheadInfo
    {
        public LookaheadType lokaheadType = new();
        public List<int> data = new();
    }

    public readonly Dictionary<int, List<LookaheadInfo>> Lookaheads =new();
}
