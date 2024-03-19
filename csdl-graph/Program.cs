using csdlGraph;

var schema = new LabeledPropertyGraphSchema
{
    ["Schema"] = new NodeDef
    {
        Properties = ["Namespace", "Alias"],
        Contained = [("Elements", ["EnumType", "EntityType", "ComplexType", "PrimitiveType", "TypeDefinition", "Term"])]
    },

    ["TypeDefinition"] = new NodeDef
    {
        Properties = ["Name"],
        References = [("UnderlyingType", ["PrimitiveType"])],
        Contained = [("Elements", ["Annotation"])]
    },
    ["EnumType"] = new NodeDef
    {
        Properties = ["Name"],
        Contained = [("Members", ["Member"])]
    },
    ["Member"] = new NodeDef
    {
        Properties = ["Name", ("Value", PropertyType.Int)],
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
        Contained = [("Properties", ["Property", "NavigationProperty", "Annotation"])]
    },
    ["Property"] = new NodeDef
    {
        Properties = ["Name", ("Nullable", PropertyType.Bool)],
        References = [("Type", ["ComplexType", "EnumType", "PrimitiveType"])],
        Contained = [("Annotations", ["Annotation"])],
    },
    ["NavigationProperty"] = new NodeDef
    {
        Properties = ["Name", "ContainsTarget"],
        References = [("Type", ["EntityType"])],
        Contained = [("Annotations", ["Annotation"])],
    },
    ["PropertyRef"] = new NodeDef
    {
        Properties = ["Alias"],
        References = [("Name", ["Property"])],
    },
    ["PrimitiveType"] = new NodeDef
    {
        Properties = ["Name"],
        Contained = [("Annotations", ["Annotation"])],
    },
    ["Term"] = new NodeDef
    {
        Properties = [("Name", PropertyType.String), ("Nullable", PropertyType.Bool), ("DefaultValue", PropertyType.String), ("AppliesTo", PropertyType.String)],
        References = [("Type", ["ComplexType", "EnumType", "PrimitiveType"]), ("BaseTerm", ["Term"])],
        Contained = [("Annotations", ["Annotation"])],
    },
    // https://docs.oasis-open.org/odata/odata-csdl-xml/v4.01/odata-csdl-xml-v4.01.html#_Toc38530405
    ["Annotation"] = new NodeDef
    {
        Properties = [("Qualifier", PropertyType.String)],
        References = [("Term", ["Term"])],
    },
};

Environment.CurrentDirectory = System.IO.Path.Combine(Environment.CurrentDirectory, "data");

File.WriteAllText("schema.lpg", schema.ToString());

var graph = Graph.LoadGraph(schema, GetNodeName, "example.xml", "edm.xml", "core.xml");

graph.WriteTo("example.md");



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
            $"@{properties.Get("Term")}{PrefixIfNotNull("#", properties.Get("Qualifier"))}",
        _ =>
            properties.Get("Name") ?? $"unnamed {label}",
    };

    static string PrefixIfNotNull(string prefix, string? text) => text == null ? "" : prefix + text;
}