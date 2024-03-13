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
            w.WriteLine("{0} {1}", name, def);
        }
    }

    public override string ToString()
    {
        var writer = new StringWriter();
        Display(writer);
        return writer.ToString();
    }
}

public record struct TypeDef(string[] Attributes, Reference[] References, string[] Children)
{
    public static implicit operator (string[] Attributes, Reference[] References, string[] Children)(TypeDef value)
    {
        return (value.Attributes, value.References, value.Children);
    }

    public static implicit operator TypeDef((string[] Attributes, Reference[] References, string[] Children) value)
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
