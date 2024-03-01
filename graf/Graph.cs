namespace graf;

public class Graph
{
    record Vertex(string Label) { }

    record Edge(int Source, int Target, string Label) { }

    private readonly List<Vertex> vertices = [];

    private readonly List<Edge> edges = [];

    internal int AddVertex(string name)
    {
        vertices.Add(new Vertex(name));
        return vertices.Count - 1;
    }

    public void AddEdge(int src, int dst, string label)
    {
        edges.Add(new Edge(src, dst, label));
    }

    public void ToMermaid()
    {
        using var file = File.CreateText("foo.md");
        file.WriteLine("```mermaid");
        file.WriteLine("graph");
        foreach (var (i, v) in vertices.Enumerate())
        {
            file.WriteLine("{0}[{1}]", i, v.Label);
        }
        foreach (var edge in edges)
        {
            if (edge.Label == "has")
            {
                file.WriteLine("{0}-->{2}", edge.Source, edge.Label, edge.Target);
            }
            else
            {
                file.WriteLine("{0}-.{1}.->{2}", edge.Source, edge.Label, edge.Target);
            }
        }
        file.WriteLine("```");
    }
}
