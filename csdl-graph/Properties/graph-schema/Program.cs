namespace Csdl.Graph;
using CommandLine;

internal class Program
{
    private static void Main(string[] args)
    {
        Parser.Default
            .ParseArguments<Options>(args)
            .WithParsed<Options>(o =>
            {
                var outputFile = o.OutputFile?.FullName ?? Path.ChangeExtension(o.InputFile.FullName, ".md");
                Run(o.InputFile.FullName, outputFile);
            });
    }

    private static void Run(string inputFile, string outputFile)
    {
        var iDir = Path.GetDirectoryName(inputFile)!;
        var oDir = Path.GetDirectoryName(outputFile)!;
        File.WriteAllText(Path.Combine(oDir, "schema.lpg"), SCHEMA.ToString());

        // var core = Path.Combine(iDir, "core.xml");
        var graph = Graph.LoadGraph(SCHEMA, inputFile);

        graph.WriteTo(outputFile);
    }

    private static readonly LabeledPropertyGraphSchema SCHEMA = new()
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
            Properties = ["Name", ("ContainsTarget", PropertyType.Bool)],
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
}

public class Options
{
    [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; init; }

    [Option('i', "input", Required = true)]
    required public FileInfo InputFile { get; init; }

    [Option('o', "output", Required = false)]
    required public FileInfo? OutputFile { get; init; }
}