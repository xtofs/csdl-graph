namespace graf;

public sealed class Graph(Func<string, IReadOnlyDictionary<string, string>, string?> getNodeName)
{
    private readonly List<NodeInfo> nodes = [];
    private readonly List<EdgeInfo> edges = [];

    public Func<string, IReadOnlyDictionary<string, string>, string?> GetNodeName { get; } = getNodeName;

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
        WriteTo(w);
        return w.ToString();
    }

    public void WriteTo(string path)
    {
        using var w = File.CreateText(path);
        this.WriteTo(w);
        Console.WriteLine("finished writing {0}", path);
    }

    public void WriteTo(TextWriter w)
    {
        w.WriteLine("```mermaid");
        w.WriteLine("graph");
        foreach (var (i, node) in nodes.WidthIndex())
        {
            var name = GetNodeName(node.Label, node.Properties);
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


    public static Graph LoadGraph(LabeledPropertyGraphSchema schema, params string[] paths)
    {
        var builder = new XmlCsdlLoader(schema, schema.GetNodeName);

        var xmls = paths.Select(path => XElement.Load(path, LoadOptions.SetLineInfo));
        foreach (var xml in xmls)
        {
            builder.Load(["Schema"], xml);
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