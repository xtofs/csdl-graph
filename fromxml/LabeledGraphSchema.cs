namespace SemanticGraph;

internal class LabeledGraphSchema : IReadOnlyDictionary<string, (string[] Attributes, string[] Children)>
{
    private readonly Dictionary<String, (string[] Attributes, string[] Children)> dictionary = [];

    public (string[] Attributes, string[] Children) this[string key]
    {
        get => dictionary[key];
        set => dictionary[key] = value;
    }

    public IEnumerable<string> Keys => dictionary.Keys;

    public IEnumerable<(string[] Attributes, string[] Children)> Values => dictionary.Values;

    public int Count => dictionary.Count;

    public bool ContainsKey(string key) => dictionary.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, (string[] Attributes, string[] Children)>> GetEnumerator() => dictionary.GetEnumerator();

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out (string[] Attributes, string[] Children) value) => dictionary.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => dictionary.GetEnumerator();

    public void Load(string[] Names, XElement x, Graph graph)
    {
        Load(Names, x, graph, null);
    }

    private void Load(string[] Names, XElement x, Graph graph, int? parentId)
    {
        if (Names.Contains(x.Name.LocalName))
        {
            var (attributes, children) = this[x.Name.LocalName];

            var attrs = attributes.Where(a => x.Attribute(a) != null).Select(a => (a, x.Attribute(a)?.Value ?? "?"));
            var id = graph.AddNode(x.Name.LocalName, attrs);

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
}
