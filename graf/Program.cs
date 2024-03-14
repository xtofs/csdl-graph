using graf;

var schema = new LabeledPropertyGraphSchema
{
    ["Schema"] = new NodeDef
    {
        Properties = ["Namespace", "Alias"],
        Contained = [("Elements", ["EnumType", "EntityType", "ComplexType"])]
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
        Properties = ["Name"],
        References = [("Type", ["ComplexType", "EnumType"])],
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
};

File.WriteAllText("schema.lpg", schema.ToString());

var graph = Graph.LoadGraph("example.xml", schema, GetNodeName);
graph.WriteTo("example.md", GetNodeName);


static string? GetNodeName(string Label, IReadOnlyDictionary<string, string> Properties) => Label switch
{
    "Schema" => Properties["Alias"] ?? Properties["Namespace"],
    "PropertyRef" => Properties.Get("Alias") ?? Properties.Get("Name"),
    _ => Properties.Get("Name"),
};

