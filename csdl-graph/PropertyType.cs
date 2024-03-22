
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
        _ => throw new InvalidDataException($"{type} is an unknown PropertyType"),
    };

    public static bool IsValid(this PropertyType type, string value) => type switch
    {
        PropertyType.String => true,
        PropertyType.Bool => bool.TryParse(value, out _),
        PropertyType.Int => long.TryParse(value, out _),
        _ => throw new InvalidDataException($"{type} is an unknown PropertyType"),
    };
}