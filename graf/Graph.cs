namespace graf;

public class Graph
{
    record Node(string Label) { }

    record Edge(int Source, int Target, string Label) { }

    private readonly List<Node> vertices = [];

    private readonly List<Edge> edges = [];

    internal int AddNode(string name)
    {
        vertices.Add(new Node(name));
        return vertices.Count - 1;
    }

    public void AddEdge(int src, int dst, string label)
    {
        edges.Add(new Edge(src, dst, label));
    }

    public void WriteAsMermaidMarkdown(TextWriter writer)
    {
        writer.WriteLine("```mermaid");
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
        writer.WriteLine("```");
    }
}
