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



// /sales.Category/@Core.Description
var entityAnnotation = model.ResolvePath("/sales.Category/@Core.Description").ToArray();

// /sales/Product/category@Core.Description
var navigationPropertyAnnotation = model.ResolvePath("/sales/Product/category@Core.Description").ToArray();


model.WriteGraphMarkdown("example1.1.md", new Dictionary<string, IEnumerable<Element>> { ["red"] = entityAnnotation });
model.WriteGraphMarkdown("example1.2.md", new Dictionary<string, IEnumerable<Element>> { ["orange"] = navigationPropertyAnnotation });

// model.WriteGraphMarkdown("example1.md");

