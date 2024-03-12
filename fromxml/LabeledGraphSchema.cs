namespace SemanticGraph;

internal class LabeledGraphSchema : IReadOnlyDictionary<string, TypeDef>
{

    private readonly Dictionary<String, TypeDef> dictionary = [];

    public IEnumerable<string> Keys => dictionary.Keys;

    public IEnumerable<TypeDef> Values => dictionary.Values;

    public int Count => dictionary.Count;


    public TypeDef this[string key]
    {
        get => dictionary[key];
        set => dictionary[key] = value;
    }

    public bool ContainsKey(string key) => dictionary.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, TypeDef>> GetEnumerator() =>
        dictionary.GetEnumerator();

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out TypeDef value) =>
        dictionary.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() =>
        dictionary.GetEnumerator();

    public void Load(string[] Names, XElement x, Graph graph)
    {
        Load(Names, x, graph, null);
    }

    private void Load(string[] Names, XElement x, Graph graph, int? parentId)
    {
        if (Names.Contains(x.Name.LocalName))
        {
            var (attributes, _, children) = this[x.Name.LocalName];

            var attrs = from a in attributes
                        let v = x.Attribute(a)
                        where v != null
                        select (a, v.Value);
            var id = graph.AddNode(x.Name.LocalName, attrs.ToDictionary());

            if (parentId != null)
            {
                graph.AddEdge(parentId.Value, id, "contains");
            }
            if (children.Length > 0)
            {
                foreach (var e in x.Elements())
                {
                    Load(children, e, graph, id);
                }
            }
        }
    }



    public Graph LoadGraph(string path)
    {
        var graph = new Graph();
        var xml = XElement.Load(path, LoadOptions.SetLineInfo);
        this.Load(["Schema"], xml, graph);
        return graph;
    }
}

internal record struct TypeDef(string[] Attributes, Reference[] References, string[] Children)
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


internal record struct Reference(string Name, string[] Types)
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