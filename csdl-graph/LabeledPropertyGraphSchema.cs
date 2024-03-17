
namespace graf;

public class LabeledPropertyGraphSchema : IEnumerable
{
    private readonly Dictionary<String, NodeDef> dictionary = [];

    public LabeledPropertyGraphSchema(Func<string, IReadOnlyDictionary<string, string>, string?> getNodeName)
    {
        GetNodeName = getNodeName;
    }

    public Func<string, IReadOnlyDictionary<string, string>, string?> GetNodeName { get; }

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

            foreach (var (i, r) in def.References.WidthIndex())
            {
                if (i == 0)
                {
                    w.WriteLine();
                    w.WriteLine("    // references (associations) to other nodes");
                }

                w.WriteLine("    {0}: Box<{1}>;", r.Name, string.Join("|", r.Types));
            }

            // var multi = def.Contained?.Length > 1;
            foreach (var (i, c) in def.Contained.WidthIndex())
            {
                if (i == 0)
                {
                    w.WriteLine();
                    w.WriteLine("    // child (contained) nodes");
                }

                w.WriteLine("    {0}: Collection<{1}>;",
                    c.Name,
                    // multi && i == 0 ? "@default " : "",
                    string.Join("|", c.Types));
            }
            w.WriteLine("}");
            w.WriteLine();
        }

        w.WriteLine(new string('\n', 10));
        w.WriteLine("type Box<T> = any");
        w.WriteLine("type Collection<T> = any");
    }

    public override string ToString()
    {
        var writer = new StringWriter();
        Display(writer);
        return writer.ToString();
    }
}

public readonly record struct NodeDef(
    Property[] Properties,
    Reference[] References,
    Reference[] Contained
)
{

    public static implicit operator (Property[] Properties, Reference[] References, Reference[] Children)(NodeDef value)
    {
        return (value.Properties ?? [], value.References ?? [], value.Contained ?? []);
    }

    public static implicit operator NodeDef((Property[] Properties, Reference[] References, Reference[] Children) value)
    {
        return new NodeDef(value.Properties, value.References, value.Children);
    }
}

public enum Primitive { String, Bool, Int }

public static class PrimitiveExtensions
{
    public static string Name(this Primitive primitive) => primitive switch
    {
        Primitive.String => "string",
        Primitive.Bool => "boolean",
        Primitive.Int => "number",
        _ => throw new InvalidDataException($"{primitive} is an unkonw Primitive value"),
    };
}

public record struct Reference(string Name, string[] Types)
{
    public static implicit operator (string Name, string[] Types)(Reference value)
    {
        return (value.Name, value.Types);
    }

    public static implicit operator Reference((string Name, string[] Types) value)
    {
        return new Reference(value.Name, value.Types);
    }


}

public record struct Property(string Name, Primitive Type)
{
    public static implicit operator (string Name, Primitive Type)(Property value)
    {
        return (value.Name, value.Type);
    }

    public static implicit operator Property((string Name, Primitive Type) value)
    {
        return new Property(value.Name, value.Type);
    }

    public static implicit operator Property(string Name)
    {
        return new Property(Name, Primitive.String);
    }
}