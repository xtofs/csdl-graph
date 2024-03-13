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
            var @base = def.Children.Length != 0 ? $": Collection<{string.Join("|", def.Children)}>" : "";
            w.WriteLine("class {0}{1} {{", name, @base);
            foreach (var p in def.Properties)
            {
                w.WriteLine("    {0}: string", p);
            }
            foreach (var r in def.References)
            {
                w.WriteLine("    {0}: Box<{1}>", r.Name, string.Join("|", r.Types));
            }

            // w.WriteLine("    @children: {0}", string.Join("|", def.Children));
            foreach (var r in def.NamedChildren)
            {
                w.WriteLine("    {0}: Collection<{1}>", r.Name, string.Join("|", r.Types));
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

public record struct TypeDef(string[] Properties, Reference[] References, string[] Children, Reference[] NamedChildren)
{
    public static implicit operator (string[] Attributes, Reference[] References, string[] Children, Reference[] NamedChildren)(TypeDef value)
    {
        return (value.Properties, value.References, value.Children, value.NamedChildren);
    }

    public static implicit operator TypeDef((string[] Attributes, Reference[] References, string[] Children, Reference[] NamedChildren) value)
    {
        return new TypeDef(value.Attributes, value.References, value.Children, value.NamedChildren);
    }

    public static implicit operator TypeDef((string[] Attributes, Reference[] References, string[] Children) value)
    {
        return new TypeDef(value.Attributes, value.References, value.Children, []);
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
