namespace Csdl.Graph;

internal record XmlCsdlGraphBuilder(LabeledPropertyGraphSchema Schema)
{
    private readonly Graph _graph = new();

    private readonly Dictionary<string, int> _nameTable = [];

    private readonly List<(int Source, string Target, string Label)> _links = [];

    public static Graph FromXml(LabeledPropertyGraphSchema schema, IEnumerable<(string Path, XElement Xml)> xmls)
    {
        var builder = new XmlCsdlGraphBuilder(schema);

        var root = builder._graph.AddNode("$ROOT", new Dictionary<string, string>());
        builder.AddEdmSchema(root);

        // stage 1 constructing "containment" graph
        // var xmls = paths.Select(path => (System.IO.Path.Combine(Environment.CurrentDirectory, path), XElement.Load(path, LoadOptions.SetLineInfo)));
        foreach (var (path, xml) in xmls)
        {
            builder.Load(["Schema"], xml, path, root, null);
        }

        // stage 3 resolve the references 
        builder.ResolveReferences();

        return builder._graph;
    }

    private static readonly (string Name, string Label, bool IsAbstract)[] EdmItems = [
        ("Binary", "PrimitiveType", false),
        ("Boolean", "PrimitiveType", false),
        ("Byte", "PrimitiveType", false),
        ("Date", "PrimitiveType", false),
        ("DateTimeOffset", "PrimitiveType", false),
        ("Decimal", "PrimitiveType", false),
        ("Double", "PrimitiveType", false),
        ("Duration", "PrimitiveType", false),
        ("Guid", "PrimitiveType", false),
        ("Int", "PrimitiveType", false),
        ("SByte", "PrimitiveType", false),
        ("Single", "PrimitiveType", false),
        ("Stream", "PrimitiveType", false),
        ("String", "PrimitiveType", false),
        ("TimeOfDay", "PrimitiveType", false),
        //
        ("AnnotationPath", "PrimitiveType", false),
        ("AnyPropertyPath", "PrimitiveType", false),
        ("ModelElementPath", "PrimitiveType", false),
        ("NavigationPropertyPath", "PrimitiveType", false),
        ("PropertyPath", "PrimitiveType", false),
//
        ("Untyped", "PrimitiveType", true),
        ("PrimitiveType", "PrimitiveType", true),
        ("ComplexType", "PrimitiveType", true),
        ("EntityType", "PrimitiveType", true),
        // ("Geography", "PrimitiveType", false),
        // ("GeographyCollection", "PrimitiveType", false),
        // ("GeographyLineString", "PrimitiveType", false),
        // ("GeographyMultiLineString", "PrimitiveType", false),
        // ("GeographyMultiPoint", "PrimitiveType", false),
        // ("GeographyMultiPolygon", "PrimitiveType", false),
        // ("GeographyPoint", "PrimitiveType", false),
        // ("GeographyPolygon", "PrimitiveType", false),
        // ("Geometry", "PrimitiveType", false),
        // ("GeometryCollection", "PrimitiveType", false),
        // ("GeometryLineString", "PrimitiveType", false),
        // ("GeometryMultiLineString", "PrimitiveType", false),
        // ("GeometryMultiPoint", "PrimitiveType", false),
        // ("GeometryMultiPolygon", "PrimitiveType", false),
        // ("GeometryPoint", "PrimitiveType", false),
        // ("GeometryPolygon", "PrimitiveType", false),
    ];

    private void AddEdmSchema(int root)
    {
        var pathLookup = new Dictionary<int, (string Label, string Path)> { [root] = ("$ROOT", null!) };

        var edmSchema = Add(root, "Schema", new Dictionary<string, string> { ["Namespace"] = "odata.edm", ["Alias"] = "Edm" });

        foreach (var (Name, Label, IsAbstract) in EdmItems)
        {
            var props = new Dictionary<string, string> { ["Name"] = Name };
            if (IsAbstract) { props.Add("Abstract", "true"); }
            var i = Add(edmSchema, "PrimitiveType", props);
        }

        // create node as child node, name it, and add model path to name table and reverse model path lookup
        int Add(int parent, string label, IReadOnlyDictionary<string, string> properties)
        {
            var id = _graph.AddChildNode(parent, label, properties);
            var node = _graph.nodes[id];
            node.Name = GetNodeName(node);

            var parentInfo = pathLookup[parent];
            var path = parentInfo.Path == null ? node.Name : parentInfo.Path + GetPathSeparator(parentInfo.Label, node.Label) + node.Name;
            pathLookup.Add(id, (label, path));
            _nameTable.Add(path, id);

            return id;
        }
    }

    public void Load(string[] Names, XElement xml, string filePath, int parentId, string? gpath)
    {
        if (!Names.Contains(xml.Name.LocalName))
        {
            System.Console.WriteLine($"can't process {xml.Name.LocalName}, expected {string.Join(", ", Names)}");
        }

        var (properties, associations, elements) = Schema[xml.Name.LocalName];
        // Property[] properties = def.Properties ?? [];
        // Association[] associations = def.Associations ?? [];
        // Element[] elements = def.Elements ?? [];

        var primitivesAndReferences = properties.Concat(associations.Select(r => new Property(r.Name, PropertyType.Path)));
        var props = (
            from p in primitivesAndReferences
            let v = xml.Attribute(p.Name)
            where v != null
            select (p.Type == PropertyType.Path ? $"${p.Name}" : p.Name, v.Value)
        ).ToDictionary();

        var id = _graph.AddChildNode(parentId, xml.Name.LocalName, props);

        // set name
        var node = _graph.nodes[id];
        node.Name = GetNodeName(node);

        // add fully qualified model path to name table            
        var parentLabel = _graph.nodes[parentId].Label;
        gpath = gpath is null ? node.Name : gpath + GetPathSeparator(parentLabel, node.Label) + node.Name;
        _nameTable.TryAdd(gpath, id);

        // add links for references (add to the `Links` fixup table for later)
        var refs =
           from r in associations
           let p = xml.Attribute(r.Name)
           where p != null
           select (r.Name, p.Value);
        foreach (var (Name, Value) in refs)
        {
            _links.Add((Source: id, Target: Value, Label: Name));
        }

        // log unprocessed XML attributes
        var allowedAttrs = properties.Select(p => p.Name).Concat(associations.Select(r => r.Name)).Prepend("xmlns").Prepend("String");
        foreach (var unused in xml.Attributes().Where(a => !allowedAttrs.Contains(a.Name.LocalName)))
        {
            var li = new LineInfo(filePath, unused);
            System.Console.WriteLine(@"unused xml attribute {0} of xml element {1} {2}", unused.Name.LocalName, xml.Name.LocalName, li);
        }

        var allowed = (elements.Select(c => c.TypeAlternatives).FirstOrDefault() ?? []).Concat((elements ?? []).Skip(1).Select(c => c.Name));
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
                Console.WriteLine($"missing XML attribute {p.Name} of XML element {xml.Name.LocalName} at {li} ");
            }
            else if (!p.Type.IsValid(attr.Value))
            {
                var li = new LineInfo(filePath, attr);
                Console.WriteLine($"invalid XML attribute value for attribute {p.Name} of XML element {xml.Name.LocalName} at {li} property {p.Name} expecting {p.Type}");
            }
        }

        // recurse
        foreach (var (i, (Key, Types)) in elements.WidthIndex())
        {
            XNamespace NS = "http://docs.oasis-open.org/odata/ns/edm";
            var element = i == 0 ? xml : xml.Element(NS + Key);
            // Console.WriteLine($"load recurse {element}");
            if (element != null)
            {
                foreach (var e in element.Elements().Where(e => !elements!.Any(c => c.Name == e.Name.LocalName)))
                {
                    Load(Types, e, filePath, id, gpath);
                }
            }
        }
    }

    internal void ResolveReferences()
    {
        foreach (var (source, target, label) in _links)
        {
            if (_nameTable.TryGetValue(target, out var tgt))
            {
                _graph.AddEdge(source, tgt, label);
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
                properties.Get("Alias") ?? properties.Get("Namespace") ?? $"`unnamed {label}`",
            "PropertyRef" =>
                properties.Get("Alias") ?? properties.Get("Name") ?? $"`unnamed {label}`",
            "Annotation" =>
                $"@{properties.Get("$Term")}{PrefixIfNotNull("#", properties.Get("Qualifier"))}",
            "EntityType" or "CompleType" or "PrimitiveType" =>
                properties.Get("Name") ?? $"`unnamed {label}`",
            _ =>
                $"`unnamed unknwon {label}`"
        };

        static string PrefixIfNotNull(string prefix, string? text) => text == null ? "" : prefix + text;
    }
}
