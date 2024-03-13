using graf;

internal partial class Program
{
    private static void Main()
    {
        var schema = new LabeledPropertyGraphSchema
        {
            ["Schema"] = (
                ["Namespace", "Alias"],
                [],
                ["EnumType", "EntityType", "ComplexType"]),
            ["EnumType"] = (
                ["Name"],
                [],
                ["Member"]),
            ["Member"] = (
                ["Name", "Value"],
                [],
                []),
            ["EntityType"] = (
                ["Name"],
                [("BaseType", ["EntityType"])],
                ["Key", "Property", "NavigationProperty"]),
            ["ComplexType"] = (
                ["Name"],
                [("BaseType", ["ComplexType"])],
                ["StructuralProperty", "NavigationProperty"]),
            ["Property"] = (
                ["Name"],
                [("Type", ["ComplexType", "EnumType"])],
                []),
            ["NavigationProperty"] = (
                ["Name"],
                [("Type", ["EntityType"])],
                []),
            ["PropertyRef"] = (
                ["Alias"],
                [("Name", ["Property"])],
                []),
            ["Key"] = (
                [],
                [],
                ["PropertyRef"]),
        };

        var graph = Graph.LoadGraph("example.xml", schema, GetNodeName);

        graph.WriteTo("example.md", GetNodeName);
    }

    static string? GetNodeName(string Label, IReadOnlyDictionary<string, string> Properties) => Label switch
    {
        "Schema" => Properties["Alias"] ?? Properties["Namespace"],
        "PropertyRef" => Properties.Get("Alias") ?? Properties.Get("Name"),
        _ => Properties.Get("Name"),
    };
}

