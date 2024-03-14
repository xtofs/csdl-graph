namespace graf;

public sealed class Graph
{
    private readonly List<NodeInfo> nodes = [];
    private readonly List<EdgeInfo> edges = [];

    internal int AddNode(string Label, string? Name, IReadOnlyDictionary<string, string> attributes)
    {
        nodes.Add(new NodeInfo(Label, Name, attributes));
        return nodes.Count - 1;
    }

    public void AddEdge(int source, int target, string label)
    {
        edges.Add(new EdgeInfo(source, target, label));
    }

    public override string ToString()
    {
        var w = new StringWriter();
        WriteTo(w, (l, ps) => $"{ps["Name"]}:{l}");
        return w.ToString();
    }

    public void WriteTo(string path, Func<string, IReadOnlyDictionary<string, string>, string?> getName)
    {
        using var w = File.CreateText(path);
        this.WriteTo(w, getName);
        Console.WriteLine("finished writing {0}", path);
    }

    public void WriteTo(TextWriter w, Func<string, IReadOnlyDictionary<string, string>, string?> getName)
    {
        w.WriteLine("```mermaid");
        w.WriteLine("graph");
        foreach (var (i, node) in nodes.WidthIndex())
        {
            var name = getName(node.Label, node.Properties);
            name = name == null ? $"{node.Label}" : $"{name}: {node.Label}";
            w.WriteLine("n{0}[{1}]", i, name);
        }
        foreach (var (i, edge) in edges.WidthIndex())
        {
            if (edge.Label == "contains")
            {
                w.WriteLine("n{0}-->n{1}", edge.Source, edge.Target);
            }
            else
            {
                w.WriteLine("n{0}-. {1} .-> n{2}", edge.Source, edge.Label, edge.Target);
            }
        }
        w.WriteLine("```");
    }



    public static Graph LoadGraph(string path, LabeledPropertyGraphSchema schema, Func<string, IReadOnlyDictionary<string, string>, string?> getNodeName)
    {
        var xml = XElement.Load(path, LoadOptions.SetLineInfo);

        var builder = new XmlCsdlLoader(schema, getNodeName);
        builder.Load(["Schema"], xml, null, ImmutableList<string>.Empty);

        foreach (var x in builder.NameTable)
        {
            Console.WriteLine("{0} : {1}", x.Key, x.Value);
        }
        builder.Resolve();

        return builder.Graph;
    }
}

internal record struct NodeInfo(string Label, string? Name, IReadOnlyDictionary<string, string> Properties)
{
    public static implicit operator (string Label, string? Name, IReadOnlyDictionary<string, string> Attributes)(NodeInfo value)
    {
        return (value.Label, value.Name, value.Properties);
    }

    public static implicit operator NodeInfo((string Label, string? Name, IReadOnlyDictionary<string, string> Attributes) value)
    {
        return new NodeInfo(value.Label, value.Name, value.Attributes);
    }
}

internal record struct EdgeInfo(int Source, int Target, string Label)
{
    public static implicit operator (int Source, int Target, string Label)(EdgeInfo value)
    {
        return (value.Source, value.Target, value.Label);
    }

    public static implicit operator EdgeInfo((int Source, int target, string Label) value)
    {
        return new EdgeInfo(value.Source, value.target, value.Label);
    }
}