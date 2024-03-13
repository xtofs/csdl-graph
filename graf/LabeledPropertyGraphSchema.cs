namespace graf;

public class LabeledPropertyGraphSchema : IEnumerable
{
    private readonly Dictionary<String, TypeDef> dictionary = [];

    public TypeDef this[string key]
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
            foreach (var p in def.Properties)
            {
                w.WriteLine("    {0}: string", p);
            }
            foreach (var r in def.References)
            {
                w.WriteLine("    {0}: Box<{1}>", r.Name, string.Join("|", r.Types));
            }

            foreach (var (i, r) in def.Children.Enumerate())
            {
                w.WriteLine("    {0}{1}: Collection<{2}>",
                    i == 0 ? "default " : "",
                    r.Name,
                    string.Join("|", r.Types));
            }
            w.WriteLine("}");
        }
    }

    public override string ToString()
    {
        var writer = new StringWriter();
        Display(writer);
        return writer.ToString();
    }
}

public record struct TypeDef(
    string[] Properties,
    Reference[] References,
    Reference[] Children)
{
    public static implicit operator (string[] Attributes, Reference[] References, Reference[] Children)(TypeDef value)
    {
        return (value.Properties, value.References, value.Children);
    }

    public static implicit operator TypeDef((string[] Attributes, Reference[] References, Reference[] Children) value)
    {
        return new TypeDef(value.Attributes, value.References, value.Children);
    }
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
