using System.Diagnostics.CodeAnalysis;

namespace CSharpCC.Utils;


/**
 *
 *
 * @author Chris Ainsley
 *
 */
public class OptionInfo : IComparable<OptionInfo>
{
    readonly string name;
    readonly OptionType type;
    readonly object @default;

    public OptionInfo(string name, OptionType type, object default1)
    {
        this.name = name;
        this.type = type;
        @default = default1;
    }

    public string Name => name;

    public OptionType OptionType => type;

    public object Default => @default;

    public override int GetHashCode()
    {
        const int prime = 31;
        int result = 1;
        result = prime * result
                + ((@default == null) ? 0 : @default.GetHashCode());
        result = prime * result + ((name == null) ? 0 : name.GetHashCode());
        result = prime * result + ((type == null) ? 0 : type.GetHashCode());
        return result;
    }

    //@Override
    public override bool Equals([NotNull] object? obj)
    {
        if (this == obj)
            return true;
        if (obj == null)
            return false;
        if (this.GetType() != obj.GetType())
            return false;
        OptionInfo other = (OptionInfo)obj;
        if (@default == null)
        {
            if (other.@default != null)
                return false;
        }
        else if (@default != (other.@default))
            return false;
        if (name == null)
        {
            if (other.name != null)
                return false;
        }
        else if (name != (other.name))
            return false;
        if (type != other.type)
            return false;
        return true;
    }

    public int CompareTo(OptionInfo? o) => name.CompareTo(o?.name);


}
