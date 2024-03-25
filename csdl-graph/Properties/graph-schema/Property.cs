
namespace Csdl.Graph;

public record Property(string Name, PropertyType Type)
{
    public static implicit operator (string Name, PropertyType Type)(Property value)
    {
        return (value.Name, value.Type);
    }

    public static implicit operator Property((string Name, PropertyType Type) value)
    {
        return new Property(value.Name, value.Type);
    }

    public static implicit operator Property(string Name)
    {
        return new Property(Name, PropertyType.String);
    }
}

