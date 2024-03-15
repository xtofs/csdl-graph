namespace graf;

internal record XmlCsdlLoader(LabeledPropertyGraphSchema Schema, Func<string, IReadOnlyDictionary<string, string>, string?> GetNodeName)
{
    public Graph Graph { get; private set; } = new(GetNodeName);

    public Dictionary<string, int> NameTable { get; private set; } = [];

    public List<(int Source, string Target, string Label)> Links = [];


    public void Load(string[] Names, XElement xml, int? parentId = null, IImmutableList<string> qn = null!)
    {
        qn ??= ImmutableList<string>.Empty;
        if (Names.Contains(xml.Name.LocalName))
        {
            var (properties, references, children) = Schema[xml.Name.LocalName];

            var props = from p in properties
                        let v = xml.Attribute(p.Name)
                        where v != null
                        select (p.Name, v.Value);

            var name = this.GetNodeName(xml.Name.LocalName, props.ToDictionary());
            if (name != null) { qn = qn.Add(name); }

            var id = Graph.AddNode(xml.Name.LocalName, name, props.ToDictionary());
            NameTable.TryAdd(string.Join("/", qn), id);

            var refs =
               from r in references ?? []
               let p = xml.Attribute(r.Name)
               where p != null
               select (r.Name, p.Value);
            foreach (var (Name, Value) in refs)
            {
                Links.Add((Source: id, Target: Value, Label: Name));
            }

            if (parentId != null)
            {
                Graph.AddEdge(parentId.Value, id, "contains");
            }

            foreach (var (i, (Key, Types)) in children.WidthIndex())
            {
                var element = i == 0 ? xml : xml.Element(Key);
                if (element != null)
                {
                    foreach (var e in element.Elements())
                    {
                        Load(Types, e, id, qn);
                    }
                }
            }
        }
    }

    public void Resolve()
    {
        foreach (var (source, target, label) in Links)
        {
            if (NameTable.TryGetValue(target, out var tgt))
            {
                Graph.AddEdge(source, tgt, label);
            }
            else
            {
                System.Console.WriteLine($"can't resolve {target}");
            }
        }
    }
}