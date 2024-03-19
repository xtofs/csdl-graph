
namespace csdlGraph;

internal record XmlCsdlGraphBuilder(LabeledPropertyGraphSchema Schema, Func<Node, string> GetNodeName)
{
    public Graph Graph { get; } = new();

    public Dictionary<string, int> NameTable { get; } = [];

    public List<(int Source, string Target, string Label)> Links { get; } = [];


    public void Load(string[] Names, XElement xml, string filePath, int parentId, string? gpath)
    {
        if (Names.Contains(xml.Name.LocalName))
        {
            var (properties, references, children) = Schema[xml.Name.LocalName];
            references ??= [];
            children ??= [];
            properties ??= [];

            var props = (from p in properties.Concat(references.Select(r => new Property(r.Name, (PropertyType)Int32.MaxValue)))
                         let v = xml.Attribute(p.Name)
                         where v != null
                         select (p.Name, v.Value)).ToDictionary();

            var id = Graph.AddNode(xml.Name.LocalName, props);
            Graph.AddEdge(parentId, id, "contains");

            // set name
            var node = Graph.nodes[id];
            node.Name = GetNodeName(node);

            // add qualified name (gpath) to name table
            // qn = qn.Add(node.Name);
            gpath = gpath is null ? node.Name : gpath + GetSeparator(node) + node.Name;
            NameTable.TryAdd(gpath, id);

            // add links for references (to the Links fixup table)
            var refs =
               from r in references
               let p = xml.Attribute(r.Name)
               where p != null
               select (r.Name, p.Value);
            foreach (var (Name, Value) in refs)
            {
                Links.Add((Source: id, Target: Value, Label: Name));
            }

            // log unprocessed XML attributes
            var allowedAttrs = properties.Select(p => p.Name).Concat(references.Select(r => r.Name)).Prepend("xmlns");
            foreach (var unused in xml.Attributes().Where(a => !allowedAttrs.Contains(a.Name.LocalName)))
            {
                var li = new LineInfo(filePath, unused);
                System.Console.WriteLine(@"unused xml attribute {0} of xml element {1} {2}", unused.Name.LocalName, xml.Name.LocalName, li);
            }
            var allowed = (children.Select(c => c.Types).FirstOrDefault() ?? []).Concat((children ?? []).Skip(1).Select(c => c.Name));
            foreach (var unused in xml.Elements().Where(e => !allowed.Contains(e.Name.LocalName)))
            {
                var li = new LineInfo(filePath, unused);
                Console.WriteLine(@"unused xml element {0} of {1} {2}", unused.Name.LocalName, xml.Name.LocalName, li);
            }

            // recurse
            foreach (var (i, (Key, Types)) in children.WidthIndex())
            {
                var element = i == 0 ? xml : xml.Element(Key);
                if (element != null)
                {
                    foreach (var e in element.Elements())
                    {
                        Load(Types, e, filePath, id, gpath);
                    }
                }
            }
        }
    }

    private static string GetSeparator(Node node)
    {
        return node.Label switch
        {
            "Term" => ".",
            "EntityType" => ".",
            "ComplexType" => ".",
            "PrimitiveType" => ".",
            "TypeDefinition" => ".",
            _ => "/"
        };
    }

    // internal void NameNodesAndUpdateNameTable(int id)
    // {
    //     var node = this.Graph.nodes[id];
    //     foreach (var (_, child) in node.Adjacent.Where(a => a.Label == "contains"))
    //     {
    //         NameNodesAndUpdateNameTable(child, []);
    //     }
    // }

    // private void NameNodesAndUpdateNameTable(int id, ImmutableList<string> qn)
    // {
    //     var node = this.Graph.nodes[id];
    //     node.Name = GetNodeName(node.Label, node.Properties);

    //     qn = qn.Add(node.Name);
    //     NameTable.TryAdd(string.Join("/", qn), id);

    //     foreach (var (_, child) in node.Adjacent.Where(a => a.Label == "contains"))
    //     {
    //         NameNodesAndUpdateNameTable(child, qn);
    //     }
    // }

    internal void ResolveReferences()
    {
        foreach (var (source, target, label) in Links)
        {
            if (NameTable.TryGetValue(target, out var tgt))
            {
                Graph.AddEdge(source, tgt, label);
            }
            else
            {
                // var li = Graph.nodes[source].LineInfo;
                Console.WriteLine($"can't resolve {target}");
            }
        }
    }

}


record struct LineInfo(string Path, int LineNumber, int LinePosition)
{

    public LineInfo(string path, XObject xml) : this(path, ((IXmlLineInfo)xml).LineNumber, ((IXmlLineInfo)xml).LinePosition)
    {
    }

    public override readonly string ToString() => $"{Path}({LineNumber},{LinePosition})";
}