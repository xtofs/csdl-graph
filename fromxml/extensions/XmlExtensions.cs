using System.Xml.Linq;
namespace parsing;

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


    public static bool TryGetChildrenFromXml<S>(this XElement xml, XName name, [MaybeNullWhen(false)] out IEnumerable<S> result)
        where S : IXmlParsable<S>
    {
        if (xml.Name == name)
        {
            result = xml.Elements().TrySelect<XElement, S>(S.TryFromXml).ToArray();
            return true;
        }
        result = default;
        return false;
    }


    public static bool TryFromXml<A, B>(this XElement x, [MaybeNullWhen(false)] out B result) where A : B, IXmlParsable<A>
    {
        if (A.TryFromXml(x, out var a))
        {
            result = a; return true;
        }
        else
        {
            result = default; return false;
        }
    }
}

