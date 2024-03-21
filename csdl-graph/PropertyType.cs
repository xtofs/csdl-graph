
namespace Csdl.Graph;

public enum PropertyType
{
    String, Bool, Int,
    Path
}


public static class PropertyTypeExtensions
{
    public static string Name(this PropertyType type) => type switch
    {
        PropertyType.String => "string",
        PropertyType.Bool => "boolean",
        PropertyType.Int => "number",
        _ => throw new InvalidDataException($"{type} is an unkonw Primitive value"),
    };
}