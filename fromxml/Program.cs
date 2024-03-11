using SemanticGraph;

internal class Program
{
    static readonly XmlParser SchemaParser = new Lazy<XmlParser>(() =>
    {
        var member = Node.Parser("Member", ["Name", "Value"]);
        var @enum = Node.Parser("EnumType", ["Name"], member);
        var structuralProperty = Node.Parser("Property", ["Name", "Type:ComplexType|EnumType"]);
        var navigationProperty = Node.Parser("NavigationProperty", ["Name", "Type:EntityType"]);
        var property = Node.Parser(structuralProperty, navigationProperty);
        var complex = Node.Parser("ComplexType", ["Name", "BaseType:ComplexType"], property);
        var propertyRef = Node.Parser("PropertyRef", ["Name:Property", "Alias"]);
        var key = Node.Parser("Key", [], propertyRef);
        var entity = Node.Parser("EntityType", ["Name:Property", "Alias"], Node.Parser(key, property));
        var schema = Node.Parser("Schema", ["Namespace", "Alias"], Node.Parser(entity, complex, @enum));
        return schema;
    }).Value;

    private static void Main()
    {
        // var @ref = "Type:Complex|Enum";
        // var (name, types) = Node.ParseReferenceString(@ref);
        // System.Console.WriteLine("result: {0}: {1}", name, string.Join(" | ", types));


        if (SchemaParser(XElement.Load("schema.xml", LoadOptions.SetLineInfo), out var schema))
        {
            Console.WriteLine(schema);
            Console.WriteLine(schema.ToXml());
        }
        else
        {
            Console.WriteLine("failed to Parse");
        }
    }


    // static readonly RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
}