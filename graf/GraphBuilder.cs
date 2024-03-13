namespace graf;

internal record GraphBuilder(LabeledPropertyGraphSchema Schema, Func<string, IReadOnlyDictionary<string, string>, string?> GetName)
{
    public Graph Graph { get; private set; } = new();

    public Dictionary<string, int> NameTable { get; private set; } = [];

    public List<(int Source, string Target, string Label)> Links = [];

    public void Load(string[] Names, XElement xml, int? parentId, IImmutableList<string> qn)
    {
        if (Names.Contains(xml.Name.LocalName))
        {
            var (attributes, references, children) = Schema[xml.Name.LocalName];

            var attrs = from a in attributes
                        let v = xml.Attribute(a)
                        where v != null
                        select (a, v.Value);

            var name = GetName(xml.Name.LocalName, attrs.ToDictionary());
            if (name != null) { qn = qn.Add(name); }

            var id = Graph.AddNode(xml.Name.LocalName, name, attrs.ToDictionary());
            NameTable.TryAdd(string.Join("/", qn), id);

            var refs =
               from r in references
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

            foreach (var (i, (Key, Types)) in children.Enumerate())
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