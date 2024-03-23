namespace Csdl.Graph;

public sealed class Graph()
{
    internal readonly List<Node> nodes = [];

    internal int AddNode(string Label, IReadOnlyDictionary<string, string> properties)
    {
        nodes.Add(new Node(Label, properties));
        return nodes.Count - 1;
    }

    internal int AddChildNode(int parentId, string Label, IReadOnlyDictionary<string, string> properties)
    {
        var id = this.AddNode(Label, properties);
        this.AddEdge(parentId, id, "$contains");
        this.AddEdge(id, parentId, "$contained");
        return id;
    }

    public void AddEdge(int source, int target, string label)
    {
        // Debug.Assert(nodes.Count < target); // ensure target exists
        nodes[source].Adjacent.Add((label, target));
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
        w.WriteLine();
        w.WriteLine("```mermaid");
        w.WriteLine("graph");
        foreach (var (i, node) in nodes.WidthIndex().Where(n => n.Item.Label != "$ROOT"))
        {
            var name = node.Name == null ? $"{node.Label}" : $"{node.Name}: {node.Label}";
            w.WriteLine("n{0}[{1}]", i, name);
        }
        foreach (var (i, node) in nodes.WidthIndex().Where(n => n.Item.Label != "$ROOT"))
        {
            foreach (var (Label, Target) in node.Adjacent.Where(lnk => lnk.Label != "$contained"))
            {
                if (Label == "$contains")
                {
                    w.WriteLine("n{0}-->n{1}", i, Target);
                }
                else
                {
                    w.WriteLine("n{0}-. {1} .-> n{2}", i, Label, Target);
                }
            }
        }
        w.WriteLine("```");
    }

    public static Graph LoadGraph(LabeledPropertyGraphSchema schema, params string[] paths)
    {
        var xmls = paths.Select(path => (System.IO.Path.Combine(Environment.CurrentDirectory, path), XElement.Load(path, LoadOptions.SetLineInfo)));

        return XmlCsdlGraphBuilder.FromXml(schema, xmls);
    }
}

public record Node(string Label, IReadOnlyDictionary<string, string> Properties, List<(string Label, int Target)> Adjacent)
{
    public Node(string Label, IReadOnlyDictionary<string, string> Properties) : this(Label, Properties, [])
    { }

    internal string? Name { get; set; }
}
