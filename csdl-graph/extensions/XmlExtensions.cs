using System.Xml.Linq;
namespace csdlGraph;

static class XmlExtensions
{
    public static string GetAttr(this XElement element, XName name, string @default)
    {
        return element.Attribute(name)?.Value ?? @default;
    }

    public static string? GetAttr(this XElement element, XName name)
    {
        return element.Attribute(name)?.Value ?? default;
    }


}

