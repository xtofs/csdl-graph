

using CommandLine;

namespace Csdl.Graph;

internal record XmlCsdlGraphBuilder(LabeledPropertyGraphSchema Schema)
{

    public static Graph FromXml(LabeledPropertyGraphSchema schema, IEnumerable<(string Path, XElement Xml)> xmls)
    {
        var builder = new XmlCsdlGraphBuilder(schema);

        var root = builder.Graph.AddNode("$ROOT", new Dictionary<string, string>());

        // stage 1 constructing "containment" graph
        // var xmls = paths.Select(path => (System.IO.Path.Combine(Environment.CurrentDirectory, path), XElement.Load(path, LoadOptions.SetLineInfo)));
        foreach (var (path, xml) in xmls)
        {
            builder.Load(["Schema"], xml, path, root, null);
        }
        // stage 3 resolve the references 
        builder.ResolveReferences();

        Console.WriteLine();
        Console.WriteLine("# Names");
        Console.WriteLine();
        Console.WriteLine(string.Join(Environment.NewLine, builder.NameTable.Keys));
        return builder.Graph;
    }

    public Graph Graph { get; } = new();

    public Dictionary<string, int> NameTable { get; } = [];

    public List<(int Source, string Target, string Label)> Links { get; } = [];

    public void Load(string[] Names, XElement xml, string filePath, int parentId, string? gpath)
    {
        if (!Names.Contains(xml.Name.LocalName))
        {
            System.Console.WriteLine($"can't process {xml.Name.LocalName}, expected {string.Join(", ", Names)}");
        }

        var (properties, references, children) = Schema[xml.Name.LocalName];

        var primitivesAndReferences = properties.Concat(references.Select(r => new Property(r.Name, PropertyType.Path)));
        var props = (
            from p in primitivesAndReferences
            let v = xml.Attribute(p.Name)
            where v != null
            select (p.Type == PropertyType.Path ? $"${p.Name}" : p.Name, v.Value)
        ).ToDictionary();

        var id = Graph.AddNode(xml.Name.LocalName, props);
        Graph.AddEdge(parentId, id, "$contains");
        Graph.AddEdge(id, parentId, "$contained");

        // set name
        var node = Graph.nodes[id];
        node.Name = GetNodeName(node);

        // add qualified name (gpath) to name table            
        var parentLabel = Graph.nodes[parentId].Label;
        gpath = gpath is null ? node.Name : gpath + GetPathSeparator(parentLabel, node.Label) + node.Name;
        NameTable.TryAdd(gpath, id);

        // add links for references (add to the `Links` fixup table for later)
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
        var allowedAttrs = properties.Select(p => p.Name).Concat(references.Select(r => r.Name)).Prepend("xmlns").Prepend("String");
        foreach (var unused in xml.Attributes().Where(a => !allowedAttrs.Contains(a.Name.LocalName)))
        {
            var li = new LineInfo(filePath, unused);
            System.Console.WriteLine(@"unused xml attribute {0} of xml element {1} {2}", unused.Name.LocalName, xml.Name.LocalName, li);
        }

        var allowed = (children.Select(c => c.TypeAlternatives).FirstOrDefault() ?? []).Concat((children ?? []).Skip(1).Select(c => c.Name));
        foreach (var unused in xml.Elements().Where(e => !allowed.Contains(e.Name.LocalName)))
        {
            var li = new LineInfo(filePath, unused);
            Console.WriteLine(@"unused xml element {0} of {1} {2}", unused.Name.LocalName, xml.Name.LocalName, li);
        }

        // log property type errors
        foreach (var p in properties)
        {
            var attr = xml.Attribute(p.Name);
            if (attr == null)
            {
                var li = new LineInfo(filePath, xml);
                Console.WriteLine($"XML element {xml.Name.LocalName} at {li} is missing property {p.Name}");
            }
            else if (!p.Type.IsValid(attr.Value))
            {
                var li = new LineInfo(filePath, xml);
                Console.WriteLine($"XML element {xml.Name.LocalName} at {li} property {p.Name} has wrong value, expecting {p.Type}");
            }
        }

        // recurse
        foreach (var (i, (Key, Types)) in children.WidthIndex())
        {
            XNamespace NS = "http://docs.oasis-open.org/odata/ns/edm";
            var element = i == 0 ? xml : xml.Element(NS + Key);
            if (element != null)
            {
                foreach (var e in element.Elements())
                {
                    Load(Types, e, filePath, id, gpath);
                }
            }
        }
    }

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
                Console.WriteLine($"can't resolve {target}");
            }
        }
    }

    private static string GetPathSeparator(string parentLabel, string childLabel)
    {
        return (parentLabel, childLabel) switch
        {

            // Schema elements are separated from the schema by '.'
            ("Schema", _) => ".",

            // the only possible child of a NavigationProperty is an Annotation and a slash is not used 
            // see https://docs.oasis-open.org/odata/odata-csdl-xml/v4.01/odata-csdl-xml-v4.01.html#sec_PathSyntax paragraph before Example 64
            ("NavigationProperty", "Annotation") => "",

            // for everything else use a slash
            _ => "/"
        };
    }

    static string GetNodeName(Node node)
    {
        var (label, properties, _) = node;
        return label switch
        {
            "Schema" =>
                properties.Get("Alias") ?? properties.Get("Namespace") ?? $"unnamed {label}",
            "PropertyRef" =>
                properties.Get("Alias") ?? properties.Get("Name") ?? $"unnamed {label}",
            "Annotation" =>
                $"@{properties.Get("$Term")}{PrefixIfNotNull("#", properties.Get("Qualifier"))}",
            _ =>
                properties.Get("Name") ?? $"unnamed {label}",
        };

        static string PrefixIfNotNull(string prefix, string? text) => text == null ? "" : prefix + text;
    }
}
