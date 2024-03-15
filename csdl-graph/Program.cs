using graf;

var schema = new LabeledPropertyGraphSchema(GetNodeName)
{
    ["Schema"] = new NodeDef
    {
        Properties = ["Namespace", "Alias"],
        Contained = [("Elements", ["EnumType", "EntityType", "ComplexType", "PrimitiveType"])]
    },
    ["EnumType"] = new NodeDef
    {
        Properties = ["Name"],
        Contained = [("Members", ["Member"])]
    },
    ["Member"] = new NodeDef
    {
        Properties = ["Name", ("Value", Primitive.Int)],
    },
    ["EntityType"] = new NodeDef
    {
        Properties = ["Name"],
        References = [("BaseType", ["EntityType"])],
        Contained = [("Properties", ["Property", "NavigationProperty"]), ("Key", ["PropertyRef"])]
    },
    ["ComplexType"] = new NodeDef
    {
        Properties = ["Name"],
        References = [("BaseType", ["ComplexType"])],
        Contained = [("Properties", ["Property", "NavigationProperty"])]
    },
    ["Property"] = new NodeDef
    {
        Properties = ["Name", ("Nullable", Primitive.Bool)],
        References = [("Type", ["ComplexType", "EnumType", "PrimitiveType"])],
    },
    ["NavigationProperty"] = new NodeDef
    {
        Properties = ["Name", "ContainsTarget"],
        References = [("Type", ["EntityType"])],
    },
    ["PropertyRef"] = new NodeDef
    {
        Properties = ["Alias"],
        References = [("Name", ["Property"])],
    },
    ["PrimitiveType"] = new NodeDef
    {
        Properties = ["Name"],
    },
};

File.WriteAllText("schema.lpg", schema.ToString());

var graph = Graph.LoadGraph(schema, "example.xml", "edm.xml", "core.xml");

graph.WriteTo("example.md");


static string? GetNodeName(string Label, IReadOnlyDictionary<string, string> Properties) => Label switch
{
    "Schema" => Properties["Alias"] ?? Properties["Namespace"],
    "PropertyRef" => Properties.Get("Alias") ?? Properties.Get("Name"),
    _ => Properties.Get("Name"),
};

