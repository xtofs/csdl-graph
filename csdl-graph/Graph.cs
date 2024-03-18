using System.Diagnostics;

namespace csdlGraph;

public sealed class Graph(NodeNameFunc CreateNodeName)
{
    private readonly List<Node> nodes = [];

    internal int AddNode(string Label, IReadOnlyDictionary<string, string> properties)
    {
        nodes.Add(new Node(Label, properties));
        return nodes.Count - 1;
    }

    public void AddEdge(int source, int target, string label)
    {
        // Debug.Assert(nodes.Count < target); // ensure target exists
        nodes[source].Adjacent.Add((label, target));
    }


    public string NodeName(int id) => NodeName(nodes[id]);

    public string NodeName(Node node)
    {
        return node.Name ??= CreateNodeName(node.Label, node.Properties, node.Adjacent.Select(a => (a.Label, nodes[a.Target])));
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
        w.WriteLine("# csdl");
        w.WriteLine("```mermaid");
        w.WriteLine("graph");
        foreach (var (i, node) in nodes.WidthIndex())
        {
            var name = this.NodeName(node);
            // name = name == null ? $"{node.Label}" : $"{name}: {node.Label}";
            w.WriteLine("n{0}[{1}]", i, $"{name}: {node.Label}");
        }
        foreach (var (i, node) in nodes.WidthIndex())
        {
            foreach (var edge in node.Adjacent)
            {
                if (edge.Label == "contains")
                {
                    w.WriteLine("n{0}-->n{1}", i, edge.Target);
                }
                else
                {
                    w.WriteLine("n{0}-. {1} .-> n{2}", i, edge.Label, edge.Target);
                }
            }
        }
        w.WriteLine("```");
    }

    public static Graph LoadGraph(LabeledPropertyGraphSchema schema, NodeNameFunc getNodeName, params string[] paths)
    {
        var builder = new XmlCsdlLoader(schema, getNodeName);

        var xmls = paths.Select(path => (System.IO.Path.Combine(Environment.CurrentDirectory, path), XElement.Load(path, LoadOptions.SetLineInfo)));
        foreach (var (path, xml) in xmls)
        {
            builder.Load(["Schema"], xml, path);
        }

        builder.Resolve();

        return builder.Graph;
    }
}


public delegate string NodeNameFunc(string label, IReadOnlyDictionary<string, string> properties, IEnumerable<(string Label, Node Node)> adjacent);

public record Node(string Label, IReadOnlyDictionary<string, string> Properties, List<(string Label, int Target)> Adjacent)
{
    public Node(string Label, IReadOnlyDictionary<string, string> Properties) : this(Label, Properties, [])
    { }

    internal string? Name { get; set; }
}
