namespace graf;

public sealed record Element(string? Name, string Type) : IEnumerable
{
    public Element(string Type) : this(null, Type) { }

    public IReadOnlyCollection<Element> Nodes => nodes;

    private readonly List<Element> nodes = [];
    private readonly Dictionary<string, string> attributes = [];

    public void Add(Element node) { nodes.Add(node); }

    // for collection initializer
    IEnumerator IEnumerable.GetEnumerator() => Enumerable.Empty<Element>().GetEnumerator();

    public String this[string name]
    {
        set => attributes.Add(name, value);
    }

    private bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.AppendFormat("Name={0}", Name);
        builder.AppendFormat(", Type={0}", Type);
        foreach (var attrib in attributes)
        {
            builder.AppendFormat(", {0}={1}", attrib.Key, attrib.Value);
        }

        builder.AppendJoinIfAny(", Nodes=[", nodes, ",", "]");
        return true;
    }

    public XElement ToXml()
    {
        var element = new XElement(Type);
        if (Name != null) { element.Add(new XAttribute("Name", Name)); }
        foreach (var attrib in attributes)
        {
            element.Add(new XAttribute(attrib.Key, attrib.Value));
        }
        foreach (var item in nodes)
        {
            element.Add(item.ToXml());
        }
        return element;
    }

    private IEnumerable<Element> AllDescendants()
    {
        var queue = new Queue<Element>();
        queue.Enqueue(this);

        while (queue.TryDequeue(out var node))
        {
            yield return node;
            foreach (var child in node.Nodes)
            {
                queue.Enqueue(child);
            }
        }
    }

    public Graph ToGraph()
    {
        var graph = new Graph();
        var index = new Dictionary<Element, int>(ReferenceEqualityComparer.Instance);
        foreach (var node in this.AllDescendants())
        {
            var label = $"{(string.IsNullOrWhiteSpace(node.Name) ? "" : $"{node.Name}: ")}{node.Type}";
            var ix = graph.AddNode(label);
            index.Add(node, ix);
        }

        foreach (var node in index.Keys)
        {
            foreach (var child in node.Nodes)
            {
                graph.AddEdge(index[node], index[child], "has");
            }

            foreach (var (key, value) in node.attributes)
            {
                if (this.TryFind(value, out var ty))
                {
                    Console.WriteLine($"found associated node {value}");
                    graph.AddEdge(index[node], index[ty], key.ToLower());
                }
                else
                {
                    Console.WriteLine($"couldn't find associated node {value}");
                }
            }
        }
        return graph;
    }

    private bool TryFind(string name, [MaybeNullWhen(false)] out Element node)
    {
        return TryFind(name.Split('.'), out node);
    }

    private bool TryFind(Span<string> name, [MaybeNullWhen(false)] out Element node)
    {
        var current = this;
        while (name.Length > 0)
        {
            var first = name[0];

            var ix = current.nodes.FindIndex(n => n.Name.Equals(first, StringComparison.InvariantCultureIgnoreCase));
            if (ix < 0)
            {
                node = default; return false;
            }
            current = current.nodes[ix];
            name = name[1..];
        }
        node = current;
        return true;
    }

    public void WriteSchemaXml(string path)
    {
        var xml = this.ToXml();
        using var schema = File.CreateText(path);
        schema.WriteLine(xml);
    }

    public void WriteGraphMarkdown(string path)
    {
        var graph = this.ToGraph();
        using var file = File.CreateText(path);
        graph.WriteAsMermaidMarkdown(file);
    }
}
