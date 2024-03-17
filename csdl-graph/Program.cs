using graf;

var schema = new LabeledPropertyGraphSchema(GetNodeName)
{
    ["Schema"] = new NodeDef
    {
        Properties = ["Namespace", "Alias"],
        Contained = [("Elements", ["EnumType", "EntityType", "ComplexType", "PrimitiveType", "Term"])]
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
        Contained = [("Properties", ["Property", "NavigationProperty", "Annotation"]), ("Key", ["PropertyRef"])]
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


    // https://docs.oasis-open.org/odata/odata-csdl-xml/v4.01/odata-csdl-xml-v4.01.html#_Toc38530405
    ["Annotation"] = new NodeDef
    {
        Properties = [("Qualifier", Primitive.String)],
        References = [("Term", ["Term"])],
    },

    ["Term"] = new NodeDef
    {
        Properties = [("Name", Primitive.String)],
        References = [("Type", ["ComplexType", "EnumType", "PrimitiveType"]), ("BaseTerm", ["Term"])],
        Contained = [("Elements", ["Annotation"])]
    },
};

Environment.CurrentDirectory = "D:/source/csdl-graph/csdl-graph/data"; // System.IO.Path.Combine(Environment.CurrentDirectory, "data");

File.WriteAllText("schema.lpg", schema.ToString());

var graph = Graph.LoadGraph(schema, "example.xml", "edm.xml", "core.xml");

graph.WriteTo("example.md");


static string? GetNodeName(string Label, IReadOnlyDictionary<string, string> Properties) => Label switch
{
    "Schema" => Properties["Alias"] ?? Properties["Namespace"],
    "PropertyRef" => Properties.Get("Alias") ?? Properties.Get("Name"),
    _ => Properties.Get("Name"),
};

