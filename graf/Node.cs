using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace graf;

public sealed record Node(string? Name, string Type) : IEnumerable
{
    public Node(string Type) : this(null, Type) { }

    public IReadOnlyCollection<Node> Nodes => nodes;

    private readonly List<Node> nodes = [];
    private readonly Dictionary<string, string> attributes = [];

    public void Add(Node node) { nodes.Add(node); }

    // for collection initializer
    IEnumerator IEnumerable.GetEnumerator() => Enumerable.Empty<Node>().GetEnumerator();

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

    private IEnumerable<Node> AllDescendants()
    {
        var queue = new Queue<Node>();
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
        var index = new Dictionary<Node, int>(ReferenceEqualityComparer.Instance);
        foreach (var node in this.AllDescendants())
        {
            var label = $"{(string.IsNullOrWhiteSpace(node.Name) ? "" : $"{node.Name}: ")}{node.Type}";
            var ix = graph.AddVertex(label);
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

    private bool TryFind(string name, [MaybeNullWhen(false)] out Node node)
    {
        return TryFind(name.Split('.'), out node);
    }

    private bool TryFind(Span<string> name, [MaybeNullWhen(false)] out Node node)
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
}
