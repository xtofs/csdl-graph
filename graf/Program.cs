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
                ["Property", "NavigationProperty"],
                [("Key", ["PropertyRef"])]),
            ["ComplexType"] = (
                ["Name"],
                [("BaseType", ["ComplexType"])],
                ["Property", "NavigationProperty"]),
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
        };
        System.Console.WriteLine(schema);

        // var graph = Graph.LoadGraph("example.xml", schema, GetNodeName);

        // graph.WriteTo("example.md", GetNodeName);
    }

    static string? GetNodeName(string Label, IReadOnlyDictionary<string, string> Properties) => Label switch
    {
        "Schema" => Properties["Alias"] ?? Properties["Namespace"],
        "PropertyRef" => Properties.Get("Alias") ?? Properties.Get("Name"),
        _ => Properties.Get("Name"),
    };
}

