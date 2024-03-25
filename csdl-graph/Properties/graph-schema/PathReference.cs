
namespace Csdl.Graph;

public sealed record PathReference(string Name, int? RelativeTo, string[] Types) : Association(Name)
{
    public static implicit operator (string Name, string[] Types)(PathReference value)
    {
        return (value.Name, value.Types);
    }

    public static implicit operator PathReference((string Name, string[] Types) value)
    {
        return new PathReference(value.Name, null, value.Types);
    }

    public static implicit operator PathReference((string Name, int RelativeTo, string[] Types) value)
    {
        return new PathReference(value.Name, value.RelativeTo, value.Types);
    }
}

