using graf;

var model = new Node("", "Model") {
    new Node("Edm", "Schema"){
        new Node("String", "PrimitiveType"),
        new Node("Int32", "PrimitiveType")
    },
    new Node("self", "Schema"){
        new Node("Product", "EntityType"){
             new Node("category", "NavigationProperty") {
                ["Type"]="self.Category"
            }
        },
        new Node("Category", "EntityType"){
              new Node("id", "Property") { ["Type"]="edm.String" }
        }
    },
};

Console.WriteLine(model.ToXml());
var g = model.ToGraph();
g.ToMermaid();

