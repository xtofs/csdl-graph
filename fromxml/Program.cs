using SemanticGraph;

internal partial class Program
{
    private static void Main()
    {
        var schema = new LabeledGraphSchema
        {
            ["Member"] = (
                ["Name", "Value"],
                [],
                []),
            ["EnumType"] = (
                ["Name"],
                [],
                ["Member"]),
            ["Property"] = (
                ["Name"],
                [("Type", ["ComplexType", "EnumType"])],
                []),
            ["NavigationProperty"] = (
                ["Name"],
                [("Type", ["EntityType"])],
                []),
            ["StructuralProperty"] = (
                ["Name"],
                [("Type", ["ComplexType", "EnumType"])],
                []),
            ["ComplexType"] = (
                ["Name"],
                [("BaseType", ["ComplexType"])],
                ["StructuralProperty", "NavigationProperty"]),
            ["PropertyRef"] = (
                ["Alias"],
                [("Name", ["Property"])],
                []),
            ["Key"] = (
                [],
                [],
                ["PropertyRef"]),
            ["EntityType"] = (
                ["Name"],
                [("BaseType", ["EntityType"])],
                ["Key", "StructuralProperty", "NavigationProperty"]),
            ["Schema"] = (
                ["Namespace", "Alias"],
                [],
                ["EnumType", "EntityType", "ComplexType"]),
        };

        var graph = schema.LoadGraph("example.xml");

        graph.WriteTo("example.md", Format);
    }


    static string? Format(string Name, IReadOnlyDictionary<string, string> Properties) => Name switch
    {
        "Schema" => Properties["Alias"] ?? Properties["Namespace"],
        "Key" => null,
        "PropertyRef" => Properties.Get("Alias") ?? Properties.Get("Name"),
        _ => Properties.Get("Name"),
    };
}

