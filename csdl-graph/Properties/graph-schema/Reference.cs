
namespace Csdl.Graph;

/// <summary>
/// Represents a reference in the graph schema.
/// </summary>
public sealed record Reference(string Name, int? RelativeTo, string[] TypeAlternatives) : Association(Name)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Reference"/> class with the specified name and type alternatives.
    /// </summary>
    /// <param name="Name">The name of the reference.</param>
    /// <param name="TypeAlternatives">The type alternatives for the reference.</param>
    public Reference(string Name, string[] TypeAlternatives) : this(Name, null, TypeAlternatives) { }

    /// <summary>
    /// Implicitly converts a <see cref="Reference"/> object to a tuple of name and type alternatives.
    /// </summary>
    /// <param name="value">The <see cref="Reference"/> object to convert.</param>
    /// <returns>A tuple containing the name and type alternatives of the reference.</returns>
    public static implicit operator (string Name, string[] Types)(Reference value)
    {
        return (value.Name, value.TypeAlternatives);
    }

    /// <summary>
    /// Implicitly converts a tuple of name and type alternatives to a <see cref="Reference"/> object.
    /// </summary>
    /// <param name="value">The tuple containing the name and type alternatives.</param>
    /// <returns>A new <see cref="Reference"/> object with the specified name and type alternatives.</returns>
    public static implicit operator Reference((string Name, string[] Alternatives) value)
    {
        return new Reference(value.Name, null, value.Alternatives);
    }

    /// <summary>
    /// Implicitly converts a tuple of name, relative position, and type alternatives to a <see cref="Reference"/> object.
    /// </summary>
    /// <param name="value">The tuple containing the name, relative position, and type alternatives.</param>
    /// <returns>A new <see cref="Reference"/> object with the specified name, relative position, and type alternatives.</returns>
    public static implicit operator Reference((string Name, int RelativeTo, string[] Alternatives) value)
    {
        return new Reference(value.Name, value.RelativeTo, value.Alternatives);
    }
}

