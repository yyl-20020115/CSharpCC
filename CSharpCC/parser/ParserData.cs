namespace org.javacc.parser;


public class ParserData {
  public enum LookaheadType {
    TOKEN,
    PRODUCTION,
    SEQUENCE,
    CHOICE,
    ZERORORMORE
  }

  public static class LookaheadInfo {
    public LookaheadType lokaheadType;
    public List<Integer> data;
  }

  Dictionary<Integer, List<LookaheadInfo>> lookaheads;
}
