using csdlGraph;

var schema = new LabeledPropertyGraphSchema
{
    ["Schema"] = new NodeDef
    {
        Properties = ["Namespace", "Alias"],
        Elements = [("Elements", ["EnumType", "EntityType", "ComplexType", "PrimitiveType", "TypeDefinition", "Term"])]
    },

    ["TypeDefinition"] = new NodeDef
    {
        Properties = ["Name"],
        Associations = [new Reference("UnderlyingType", ["PrimitiveType"])],
        Elements = [("Elements", ["Annotation"])]
    },
    ["EnumType"] = new NodeDef
    {
        Properties = ["Name"],
        Elements = [("Members", ["Member"])]
    },
    ["Member"] = new NodeDef
    {
        Properties = ["Name", ("Value", PropertyType.Int)],
    },
    ["EntityType"] = new NodeDef
    {
        Properties = ["Name"],
        Associations = [new Reference("BaseType", null, ["EntityType"])],
        Elements = [("Properties", ["Property", "NavigationProperty", "Annotation"]), ("Key", ["PropertyRef"])]
    },
    ["ComplexType"] = new NodeDef
    {
        Properties = ["Name"],
        Associations = [new Reference("BaseType", ["ComplexType"])],
        Elements = [("Properties", ["Property", "NavigationProperty", "Annotation"])]
    },
    ["Property"] = new NodeDef
    {
        Properties = ["Name", ("Nullable", PropertyType.Bool)],
        Associations = [new Reference("Type", ["ComplexType", "EnumType", "PrimitiveType"])],
        Elements = [("Annotations", ["Annotation"])],
    },
    ["NavigationProperty"] = new NodeDef
    {
        Properties = ["Name", "ContainsTarget"],
        Associations = [new Reference("Type", ["EntityType"])],
        Elements = [("Annotations", ["Annotation"])],
    },
    // https://docs.oasis-open.org/odata/odata-csdl-xml/v4.01/csprd02/odata-csdl-xml-v4.01-csprd02.html#sec_Key
    ["PropertyRef"] = new NodeDef
    {
        Properties = ["Alias"],
        Associations = [
            // The value of Name is a path expression leading to a primitive property. The names of the properties in the path are joined together by forward slashes.
            new PathReference("Name", 1, ["NavigationProperty", "Property"])
        ],
    },
    // https://docs.oasis-open.org/odata/odata-csdl-xml/v4.01/csprd02/odata-csdl-xml-v4.01-csprd02.html#sec_NavigationPropertyBinding
    ["NavigationPropertyBinding"] = new NodeDef
    {
        // The value of Path is a path expression.
        // The value of Target is a target path.
    },

    ["PrimitiveType"] = new NodeDef
    {
        Properties = ["Name"],
        Elements = [("Annotations", ["Annotation"])],
    },
    ["Term"] = new NodeDef
    {
        Properties = [("Name", PropertyType.String), ("Nullable", PropertyType.Bool), ("DefaultValue", PropertyType.String), ("AppliesTo", PropertyType.String)],
        Associations = [new Reference("Type", ["ComplexType", "EnumType", "PrimitiveType"]), new Reference("BaseTerm", ["Term"])],
        Elements = [("Annotations", ["Annotation"])],
    },
    // https://docs.oasis-open.org/odata/odata-csdl-xml/v4.01/odata-csdl-xml-v4.01.html#_Toc38530405
    ["Annotation"] = new NodeDef
    {
        Properties = [("Qualifier", PropertyType.String)],
        Associations = [new Reference("Term", ["Term"])],
    },
};

Environment.CurrentDirectory = System.IO.Path.Combine(Environment.CurrentDirectory, "data"); // ../../../

File.WriteAllText("schema.lpg", schema.ToString());

// var input = "example1.xml";
var input = "ReportingLine.xml";

var graph = Graph.LoadGraph(schema, input, "edm.xml", "core.xml");

var output = Path.ChangeExtension(input, ".md");
graph.WriteTo(output);

