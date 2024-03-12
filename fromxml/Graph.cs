
namespace SemanticGraph;



public sealed class Graph
{
    private readonly List<NodeInfo> nodes = [];
    private readonly List<EdgeInfo> edges = [];

    internal int AddNode(string name, IReadOnlyDictionary<string, string> attributes)
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
        WriteTo(w, (l, ps) => $"{ps["Name"]}:{l}");
        return w.ToString();
    }

    public void WriteTo(string path, Func<string, IReadOnlyDictionary<string, string>, string?> format)
    {
        using var w = File.CreateText(path);
        this.WriteTo(w, format);
        Console.WriteLine("finished writing {0}", path);
    }

    public void WriteTo(TextWriter w, Func<string, IReadOnlyDictionary<string, string>, string?> format)
    {
        w.WriteLine("```mermaid");
        w.WriteLine("graph");
        foreach (var (i, node) in nodes.Enumerate())
        {
            var name = format(node.Label, node.Properties);
            name = name == null ? node.Label : $"{name}: {node.Label}";
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

internal record struct NodeInfo(string Label, IReadOnlyDictionary<string, string> Properties)
{
    public static implicit operator (string Label, IReadOnlyDictionary<string, string> Attributes)(NodeInfo value)
    {
        return (value.Label, value.Properties);
    }

    public static implicit operator NodeInfo((string Label, IReadOnlyDictionary<string, string> Attributes) value)
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