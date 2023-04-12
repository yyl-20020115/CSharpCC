using System.Diagnostics.CodeAnalysis;

namespace org.javacc.utils;


/**
 *
 *
 * @author Chris Ainsley
 *
 */
public class OptionInfo : IComparable<OptionInfo>
{
    string _name;
    OptionType _type;
    Object _default;

    public OptionInfo(string name, OptionType type, Object default1)
    {
        _name = name;
        _type = type;
        _default = default1;
    }

    public string getName()
    {
        return _name;
    }

    public OptionType getType()
    {
        return _type;
    }

    public object getDefault()
    {
        return _default;
    }

    //@Override
    public override int GetHashCode()
    {
        const int prime = 31;
        int result = 1;
        result = prime * result
                + ((_default == null) ? 0 : _default.GetHashCode());
        result = prime * result + ((_name == null) ? 0 : _name.GetHashCode());
        result = prime * result + ((_type == null) ? 0 : _type.GetHashCode());
        return result;
    }

    //@Override
    public override bool Equals([NotNull] object obj)
    {
        if (this == obj)
            return true;
        if (obj == null)
            return false;
        if (this.GetType() != obj.GetType())
            return false;
        OptionInfo other = (OptionInfo)obj;
        if (_default == null)
        {
            if (other._default != null)
                return false;
        }
        else if (_default != (other._default))
            return false;
        if (_name == null)
        {
            if (other._name != null)
                return false;
        }
        else if (_name != (other._name))
            return false;
        if (_type != other._type)
            return false;
        return true;
    }

    public int CompareTo(OptionInfo? o) => _name.CompareTo(o?._name);


}
