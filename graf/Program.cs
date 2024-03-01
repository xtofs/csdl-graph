using graf;

var model = new Element("Model") {
    new Element("Edm", "Schema"){
        new Element("String", "PrimitiveType"),
        new Element("Int32", "PrimitiveType")
    },
    new Element("Core", "Schema"){
        // https://github.com/OData/vocabularies/blob/684075c5642413e37a474f2d3e76d79dd92cf029/Org.OData.Core.V1.xml#L64
        new Element("Description", "Term") { ["Type"]="Edm.String" }
    },
    new Element("sales", "Schema"){
        new Element("Product", "EntityType"){
              new Element("id", "Property") { ["Type"]="edm.String" },
              new Element("category", "NavigationProperty") { ["Type"]="sales.Category" }
        },
        new Element("Category", "EntityType"){
              new Element("id", "Property") { ["Type"]="edm.String" }
        }
    },
};

model.WriteSchemaXml("example1.xml");
model.WriteGraphMarkdown("example1.md");
