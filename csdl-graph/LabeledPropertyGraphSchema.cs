
namespace csdlGraph;

public class LabeledPropertyGraphSchema : IEnumerable
{
    private readonly Dictionary<String, NodeDef> dictionary = [];

    public LabeledPropertyGraphSchema()
    {
    }

    public NodeDef this[string key]
    {
        get => dictionary[key];
        set => dictionary[key] = value;
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        dictionary.GetEnumerator();

    public void Display(TextWriter w)
    {
        foreach (var (name, def) in dictionary)
        {
            w.WriteLine("class {0} {{", name);

            foreach (var (i, p) in def.Properties.WidthIndex())
            {
                if (i == 0)
                {
                    w.WriteLine("    // properties (of primitive values)");
                }

                w.WriteLine("    {0}: {1};", p.Name, p.Type.Name());
            }

            foreach (var (i, assoc) in def.Associations.WidthIndex())
            {
                if (i == 0)
                {
                    w.WriteLine();
                    w.WriteLine("    // references (associations) to other nodes");
                }
                switch (assoc)
                {
                    case Reference reference:
                        w.WriteLine("    {0}: Reference<{1}>;", assoc.Name, string.Join("|", reference.TypeAlternatives));
                        break;

                    case PathReference path:
                        w.WriteLine("    {0}: PathReference<{1}>;", assoc.Name, string.Join(",", path.Types));
                        break;
                }
            }

            // var multi = def.Contained?.Length > 1;
            foreach (var (i, c) in def.Elements.WidthIndex())
            {
                if (i == 0)
                {
                    w.WriteLine();
                    w.WriteLine("    // child (contained) nodes");
                }

                w.WriteLine("    {0}: Collection<{1}>;",
                    c.Name,
                    string.Join("|", c.TypeAlternatives));
            }

            w.WriteLine("}");
            w.WriteLine();
        }

        // w.WriteLine(new string('\n', 10));
        // w.WriteLine("type Box<T> = any");
        // w.WriteLine("type Collection<T> = any");
    }

    public override string ToString()
    {
        var writer = new StringWriter();
        Display(writer);
        return writer.ToString();
    }
}

public sealed class NodeDef
{
    public Property[] Properties { get; init; } = [];
    public Association[] Associations { get; init; } = [];
    public Element[] Elements { get; init; } = [];


    public void Deconstruct(out Property[] properties, out Association[] associations, out Element[] elements)
    {
        properties = Properties;
        associations = Associations;
        elements = Elements;
    }
}

public sealed record Element(string Name, string[] TypeAlternatives)
{
    public static implicit operator (string Name, string[] TypeAlternatives)(Element value)
    {
        return (value.Name, value.TypeAlternatives);
    }

    public static implicit operator Element((string Name, string[] TypeAlternatives) value)
    {
        return new Element(value.Name, value.TypeAlternatives);
    }
}

public abstract record Association(string Name);

public sealed record Reference(string Name, int? RelativeTo, string[] TypeAlternatives) : Association(Name)
{

    public Reference(string Name, string[] TypeAlternatives) : this(Name, null, TypeAlternatives) { }

    public static implicit operator (string Name, string[] Types)(Reference value)
    {
        return (value.Name, value.TypeAlternatives);
    }

    public static implicit operator Reference((string Name, string[] Alternatives) value)
    {
        return new Reference(value.Name, null, value.Alternatives);
    }

    public static implicit operator Reference((string Name, int RelativeTo, string[] Alternatives) value)
    {
        return new Reference(value.Name, value.RelativeTo, value.Alternatives);
    }
}

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

public enum PropertyType { String, Bool, Int,
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