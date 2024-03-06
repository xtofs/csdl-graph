using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;

namespace graf;

public abstract record Node;

public sealed record Reference(string Name, string QualifiedName) : Node
{
}

public sealed record Property(string Name, string Value) : Node
{
}

public sealed record Element(string? Name, string Type) : Node, IEnumerable
// IEnumerable required for collection initializer
{
    public Element() : this(null, "Model")
    {
        nodes = [
            new Element("Edm", "Schema"){
                new Element("String", "PrimitiveType"),
                new Element("Int32", "PrimitiveType")
            },
            new Element("Core", "Schema"){
                // https://github.com/OData/vocabularies/blob/684075c5642413e37a474f2d3e76d79dd92cf029/Org.OData.Core.V1.xml#L64
                new Element("Description", "Term") {
                    new Reference("Type", "Edm.String" )
                }
            }
        ];
    }

    private readonly List<Node> nodes = [];

    public Element(string Type) : this(null, Type) { }

    // readonly string? name = name;

    // public string Name => name ?? CalcName();

    // private string CalcName()
    // {
    //     if (Type == "Annotation" && TryGetChild("Term", out var an))
    //     {
    //         return "@" + an.Name;
    //     }
    //     return default!;
    // }

    public IReadOnlyList<Node> Nodes => nodes;

    public void Add(Node node)
    {
        nodes.Add(node);
        if (node is Element element)
        {
            element.Parent = this;
        }
    }


    IEnumerator IEnumerable.GetEnumerator() => nodes.GetEnumerator();

    protected override bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.AppendFormatIfNotNull("Name={0}, ", Name);
        builder.AppendFormat("Type={0}", Type);

        builder.AppendJoinIfAny(", Nodes=[", nodes, ", ", "]");
        return true;
    }

    public XElement ToXml()
    {
        var xElement = new XElement(Type);
        if (Name != null) { xElement.Add(new XAttribute("Name", Name)); }
        foreach (var item in nodes)
        {
            switch (item)
            {
                case Element element:
                    xElement.Add(element.ToXml());
                    break;
                case Reference reference:
                    xElement.Add(new XAttribute(reference.Name, reference.QualifiedName));
                    break;
                case Property value:
                    xElement.Add(new XAttribute(value.Name, value.Value));
                    break;
            }
        }
        return xElement;
    }

    private IEnumerable<Element> AllDescendants()
    {
        var queue = new Queue<Element>() { this };

        while (queue.TryDequeue(out var node))
        {
            yield return node;
            foreach (var child in node.Nodes.OfType<Element>())
            {
                queue.Enqueue(child);
            }
        }
    }

    public Graph ToGraph(IReadOnlyDictionary<string, IEnumerable<Element>> highlights)
    {
        var graph = new Graph();
        var nodeIndex = new Dictionary<Element, int>(ReferenceEqualityComparer.Instance);
        foreach (var node in this.AllDescendants())
        {
            var label = $"{(string.IsNullOrWhiteSpace(node.Name) ? "" : $"{node.Name}: ")}{node.Type}";
            var ix = graph.AddNode(label);
            nodeIndex.Add(node, ix);
        }

        foreach (var node in nodeIndex.Keys)
        {
            foreach (var child in node.Nodes)
            {
                switch (child)
                {
                    case Element element:

                        var color = highlights.Keys.FirstOrDefault(hl => highlights[hl].Pairwise().Any(p => p.Item1 == node && p.Item2 == element));

                        var eix = graph.AddEdge(nodeIndex[node], nodeIndex[element], "has", color);

                        break;
                    case Reference reference:
                        var qn = reference.QualifiedName;
                        if (this.TryFindQualifiedName(reference.QualifiedName, out var target))
                        {
                            // Console.WriteLine($"found associated node {qn}");
                            graph.AddEdge(nodeIndex[node], nodeIndex[target], reference.Name);
                        }
                        else
                        {
                            Console.WriteLine($"couldn't find associated node {qn}");
                        }
                        break;
                }
            }
        }
        return graph;
    }

    private string QualifiedName
    {
        get
        {
            if (Type == "Annotation")
            {
                var @ref = nodes.OfType<Reference>().Single(r => r.Name == "Term");
                var qualifier = nodes.OfType<Property>().SingleOrDefault(r => r.Name == "Qualifier");
                return "@" + @ref.QualifiedName + (qualifier == null ? "" : "#" + qualifier.Value);
            }
            else if (Name != null)
            {
                return this.Name;
            }
            else
            {
                throw new ArgumentNullException(nameof(Name));
            }
        }
    }

    internal Element Parent { get; private set; } = default!;

    public bool TryGetChild(string name, [MaybeNullWhen(false)] out Element child)
    {
        var ix = nodes.FindIndex(n => n is Element e && string.Equals(e.QualifiedName, name, StringComparison.InvariantCultureIgnoreCase));
        if (ix < 0)
        {
            child = default; return false;
        }
        else
        {
            child = (Element)nodes[ix];
            return true;
        }
    }


    public bool TryFindQualifiedName(string name, [MaybeNullWhen(false)] out Element node)
    {
        return TryFindQualifiedName(name.Split('.'), out node);
    }

    private bool TryFindQualifiedName(Span<string> name, [MaybeNullWhen(false)] out Element node)
    {
        var current = this;
        while (name.Length > 0)
        {
            var first = name[0];

            var ix = current.nodes.FindIndex(n => n is Element e && string.Equals(e.Name, first, StringComparison.InvariantCultureIgnoreCase));
            if (ix < 0)
            {
                node = default; return false;
            }
            current = (Element)current.nodes[ix];
            name = name[1..];
        }
        node = current;
        return true;
    }

    public void WriteSchemaXml(string path)
    {
        var xml = this.ToXml();
        using var schema = File.CreateText(path);
        schema.WriteLine(xml);
    }

    public void WriteGraphMarkdown(string filePath, IReadOnlyDictionary<string, IEnumerable<Element>> highlight = null!)
    {
        using var writer = File.CreateText(filePath);
        WriteGraphMarkdown(writer, highlight);
    }

    public void WriteGraphMarkdown(TextWriter writer, IReadOnlyDictionary<string, IEnumerable<Element>> highlight = null!)
    {
        highlight ??= new Dictionary<string, IEnumerable<Element>>();
        var graph = this.ToGraph(highlight);

        writer.WriteLine("```mermaid");
        graph.WriteAsMermaid(writer);
        writer.WriteLine("```");
    }
}

public static class ElementExtensions
{
    // public static Element ResolvePathTarget(this Element element, params string[] segments)
    // {
    //     var cursor = element;
    //     foreach (var segment in segments)
    //     {
    //         if (cursor.TryGetChild(segment, out var child))
    //         {
    //             cursor = child;
    //         }
    //         else
    //         {
    //             throw new KeyNotFoundException(segment);
    //         }
    //     }
    //     return cursor;
    // }

    public static IEnumerable<Element> ResolvePath(this Element element, string path)
    {
        var segments = Path.Split(path);

        // also split by '.' of schema elements
        segments = segments.SelectMany(s => (!s.StartsWith('@') && s.Contains('.')) ? s.Split('.') : [s]);
        Console.WriteLine("{0} -> {1}", path, string.Join("; ", segments));

        var cursor = element;
        foreach (var segment in segments)
        {
            if (cursor.TryGetChild(segment, out var child))
            {
                yield return cursor;
                cursor = child;
            }
            else
            {
                throw new KeyNotFoundException(segment);
            }
        }
        yield return cursor;
    }

    // static IEnumerable<string> SplitPath(string v)
    // {
    //     return Regex.Matches(v, @"[a-z]+|@[a-z]+\.[a-z]+", RegexOptions.IgnoreCase).Cast<Match>().Select(m => m.Value);
    // }

}

public static class Path
{
    public static IEnumerable<string> Split(string path)
    {
        var fields = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var segments = fields.SelectMany(field => field.IndexOf('@') > 0 ? field.Split('@', 2).Select((s, i) => i == 0 ? s : "@" + s) : [field]);

        // Console.WriteLine("{0} => {1}", path, string.Join("; ", segments));

        return segments;
    }
}