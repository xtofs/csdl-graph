using System.Collections.Immutable;
using System.Text.RegularExpressions;
using graf;

// https://docs.oasis-open.org/odata/odata-csdl-xml/v4.01/odata-csdl-xml-v4.01.html#sec_ProductsandCategoriesExample

var model = new Element() {
    new Element("sales", "Schema"){
        new Element("Product", "EntityType"){
            new Element("id", "Property") {
            new Reference("Type", "Edm.String" )
        },
        new Element("category", "NavigationProperty") {
            new Reference("Type", "sales.Category") ,
            new Element("@Core.Description", "Annotation") {
                new Reference("Term", "Core.Description") ,
                new Property("String", "the catogory of the product") ,
            }
        }
    },
        new Element("Category", "EntityType"){
            new Element("id", "Property") { new Reference("Type", "Edm.String" ) },
            new Element("@Core.Description", "Annotation") {
                new Reference("Term", "Core.Description" ),
                new Property("String", "a product catogory" ),
            }
        }
    },
};

model.WriteSchemaXml("example1.xml");
model.WriteGraphMarkdown("example1.md");

ShowHighlightedPath(model, "/sales.Category/@Core.Description", "example1.1.md");
ShowHighlightedPath(model, "/sales/Product/category@Core.Description", "example1.2.md");

static void ShowHighlightedPath(Element model, string path, string filePath)
{
    using var writer = File.CreateText(filePath);
    writer.WriteLine("## {0}", path);
    writer.WriteLine();

    var modelPath = model.ResolvePath(path).ToArray();
    model.WriteGraphMarkdown(writer, new Dictionary<string, IEnumerable<Element>> { ["red"] = modelPath });
}

