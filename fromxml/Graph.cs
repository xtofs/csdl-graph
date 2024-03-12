namespace SemanticGraph;



public sealed class Graph
{
    private readonly List<NodeInfo> nodes = [];
    private readonly List<EdgeInfo> edges = [];

    internal int AddNode(string name, IEnumerable<(string, string)> attributes)
    {
        nodes.Add(new NodeInfo(name, attributes));
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

    public void WriteTo(TextWriter w)
    {
        w.WriteLine("```mermaid");
        w.WriteLine("graph");
        foreach (var (i, node) in nodes.Enumerate())
        {
            var name = node.Properties.Where(a => a.Name == "Name").Select(a => $"{a.Value}: {node.Label}").FirstOrDefault() ?? node.Label;
            w.WriteLine("n{0}[{1}]", i, name);
        }
        foreach (var (i, edge) in edges.Enumerate())
        {
            if (edge.Label == "contains")
            {
                w.WriteLine("n{0}-->n{1}", edge.Source, edge.target);
            }
            else
            {
                w.WriteLine("n{0}-. {1} .-> n{2}", edge.Source, edge.Label, edge.target);
            }
        }
        w.WriteLine("```");

    }
}

internal record struct NodeInfo(string Label, IEnumerable<(string Name, string Value)> Properties)
{
    public static implicit operator (string Label, IEnumerable<(string, string)> Attributes)(NodeInfo value)
    {
        return (value.Label, value.Properties);
    }

    public static implicit operator NodeInfo((string Label, IEnumerable<(string, string)> Attributes) value)
    {
        return new NodeInfo(value.Label, value.Attributes);
    }
}

internal record struct EdgeInfo(int Source, int target, string Label)
{
    public static implicit operator (int Source, int target, string Label)(EdgeInfo value)
    {
        return (value.Source, value.target, value.Label);
    }

    public static implicit operator EdgeInfo((int Source, int target, string Label) value)
    {
        return new EdgeInfo(value.Source, value.target, value.Label);
    }
}