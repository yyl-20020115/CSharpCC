using System.Text;

namespace CSharpCC.Utils;

public static class StringEscapeHelpers
{
    /**
     * Replaces unprintable characters by their escaped (or unicode escaped)
     * equivalents in the given string
     */
    public static string AddEscapes(string str)
    {
        var retval = new StringBuilder();
        char ch;
        for (int i = 0; i < str.Length; i++)
        {
            switch (str[i])
            {
                case '\0':
                    continue;
                case '\b':
                    retval.Append("\\b");
                    continue;
                case '\t':
                    retval.Append("\\t");
                    continue;
                case '\n':
                    retval.Append("\\n");
                    continue;
                case '\f':
                    retval.Append("\\f");
                    continue;
                case '\r':
                    retval.Append("\\r");
                    continue;
                case '\"':
                    retval.Append("\\\"");
                    continue;
                case '\'':
                    retval.Append("\\\'");
                    continue;
                case '\\':
                    retval.Append("\\\\");
                    continue;
                default:
                    if ((ch = str[i]) < 0x20 || ch > 0x7e)
                    {
                        var s = "0000" + Convert.ToString(ch, 16);
                        retval.Append("\\u" + s[^4..]);
                    }
                    else
                    {
                        retval.Append(ch);
                    }
                    continue;
            }
        }
        return retval.ToString();
    }

}