using System.Collections.Immutable;
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
            new Element("Annotation") {
                new Reference("Term", "Core.Description") ,
                new Property("String", "the catogory of the product") ,
            }
        }
    },
        new Element("Category", "EntityType"){
            new Element("id", "Property") { new Reference("Type", "Edm.String" ) },
            new Element("Annotation") {
                new Reference("Term", "Core.Description" ),
                new Property("String", "a product catogory" ),
            }
        }
    },
};

model.WriteSchemaXml("example1.xml");
model.WriteGraphMarkdown("example1.md");


// var a = model.ResolvePathTarget("sales", "Category", "@Core.Description");
// System.Console.WriteLine(a);
var aa = model.ResolvePath("sales", "Category", "@Core.Description");

// var b = model.ResolvePathTarget("sales", "Product", "category", "@Core.Description");
// System.Console.WriteLine(b);
var bb = model.ResolvePath("sales", "Product", "category", "@Core.Description");


model.WriteGraphMarkdown("example1.1.md", new Dictionary<string, IEnumerable<Element>> { ["red"] = aa });
model.WriteGraphMarkdown("example1.2.md", new Dictionary<string, IEnumerable<Element>> { ["orange"] = bb });

// model.WriteGraphMarkdown("example1.md");

