namespace graf;

public class Graph
{
    record Node(string Label) { }

    record Edge(int Source, int Target, string Label, string? Color) { }

    private readonly List<Node> vertices = [];

    private readonly List<Edge> edges = [];

    internal int AddNode(string name)
    {
        vertices.Add(new Node(name));
        return vertices.Count - 1;
    }

    public int AddEdge(int src, int dst, string label, string? color = null)
    {
        edges.Add(new Edge(src, dst, label, color));
        return edges.Count - 1;
    }

    public void WriteAsMermaid(TextWriter writer)
    {
        writer.WriteLine("graph");
        foreach (var (i, v) in vertices.Enumerate())
        {
            writer.WriteLine("{0}[{1}]", i, v.Label);
        }
        foreach (var edge in edges)
        {
            if (edge.Label == "has")
            {
                writer.WriteLine("{0}-->{2}", edge.Source, edge.Label, edge.Target);
            }
            else
            {
                writer.WriteLine("{0}-.{1}.->{2}", edge.Source, edge.Label, edge.Target);
            }
        }

        var groups = edges.Enumerate().Where(g => g.Item2.Color != null).GroupBy(e => e.Item2.Color, e => e.Item1);
        foreach (var group in groups)
        {
            writer.WriteLine("linkStyle {0} stroke:{1},color:{1};", string.Join(",", group), group.Key);
        }
    }
}
