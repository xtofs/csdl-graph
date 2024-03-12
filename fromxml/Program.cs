using System.Numerics;
using SemanticGraph;

internal partial class Program
{

    private static void Main()
    {

        var schema = new LabeledGraphSchema
        {
            ["Schema"] = (["Namespace", "Alias"], ["EntityType", "ComplexType"]),
            ["EntityType"] = (["Name"], ["Key", "Property", "NavigationProperty"]),
            ["ComplexType"] = (["Name"], ["Property", "NavigationProperty"]),
            ["Key"] = ([], ["PropertyRef"]),
            ["PropertyRef"] = (["Name", "Alias"], []),
            ["Property"] = (["Name"], []),
            ["NavigationProperty"] = (["Name", "Type"], []),
            //     var member = SemanticGraph.Node.Parser("Member", ["Name", "Value"]);
            //     var @enum = SemanticGraph.Node.Parser("EnumType", ["Name"], member);
            //     var structuralProperty = SemanticGraph.Node.Parser("Property", ["Name", "Type:ComplexType|EnumType"]);
            //     var navigationProperty = SemanticGraph.Node.Parser("NavigationProperty", ["Name", "Type:EntityType"]);
            //     var property = SemanticGraph.Node.Parser(structuralProperty, navigationProperty);
            //     var complex = SemanticGraph.Node.Parser("ComplexType", ["Name", "BaseType:ComplexType"], property);
            //     var propertyRef = SemanticGraph.Node.Parser("PropertyRef", ["Name:Property", "Alias"]);
            //     var key = SemanticGraph.Node.Parser("Key", [], propertyRef);
            //     var entity = SemanticGraph.Node.Parser("EntityType", ["Name:Property", "Alias"], SemanticGraph.Node.Parser(key, property));
            //     var schema = SemanticGraph.Node.Parser("Schema", ["Namespace", "Alias"], SemanticGraph.Node.Parser(entity, complex, @enum));
            //     return schema;
            // }
        };

        var graph = new Graph();
        var xml = XElement.Load("schema.xml", LoadOptions.SetLineInfo);
        schema.Load(["Schema"], xml, graph);

        // Console.WriteLine(graph);
        using var w = File.CreateText("mermaid.md");
        graph.WriteTo(w);
    }
}

