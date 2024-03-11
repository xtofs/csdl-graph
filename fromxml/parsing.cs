using System.Xml;

namespace SemanticGraph;

public delegate bool XmlParser(XElement xml, [MaybeNullWhen(false)] out Node.Element result);

public abstract partial record Node
{
    private Node(string Name) => this.Name = Name;

    public string Name { get; }
    public sealed record Element(string Name, IEnumerable<Node> Nodes) : Node(Name)
    {
        public Element(string Name, params IEnumerable<Node>[] nodess) : this(Name, nodess.Aggregate((a, b) => a.Concat(b))) { }

        protected override bool PrintMembers(StringBuilder builder)
        {
            builder.AppendFormat("Name={0}", Name);
            foreach (var a in Nodes.OfType<Attribute>())
            {
                builder.AppendFormat(", {0}={1}", a.Name, a.Value);
            }
            builder.AppendList(", Nodes={0}", Nodes.OfType<Element>(), ", ");
            return true;
        }

        public XElement ToXml()
        {
            return new XElement(XNamespace.None + this.Name,
                // addNs ? new XAttribute(XNamespace.Xmlns + "meta", "foo.org") : null,
                Nodes.SelectMany<Node, XObject?>(n =>
                    n switch
                    {
                        Element element => [
                            element.ToXml()
                        ],
                        Attribute attribute => [
                            attribute.Value != null ? new XAttribute(attribute.Name, attribute.Value) : null
                        ],
                        Reference reference => [
                            reference.Path != null ? new XAttribute(reference.Name, reference.Path) : null,
                        ],
                        _ =>
                            throw new InvalidCastException("Unable to cast parameter to a known subtype of Node")
                    }
                )
            );
        }
    }

    public sealed record Attribute(string Name, string? Value) : Node(Name);

    public sealed record Reference(string Name, string? Path, string[] Types) : Node(Name);

    public static XmlParser Parser(XName name, string[] attributes, XmlParser? children = null)
    {
        return (XElement xml, [MaybeNullWhen(false)] out Element result) =>
        {
            if (xml.Name == name)
            {
                var attrs = attributes.Where(a => a.IndexOf(':') < 0).Select(a => new Attribute(a, xml.Attribute(a)?.Value));
                var refrs = attributes.Where(a => a.IndexOf(':') > 0)
                    .TrySelect<string, (string name, string[] types)>(TryParseReferenceString)
                    .Select(r => new Reference(r.name, xml.Attribute(r.name)?.Value, r.types));

                var elements = children != null ?
                     xml.Elements().TrySelect((XElement xml, [MaybeNullWhen(false)] out Element result) => children(xml, out result)) :
                     Enumerable.Empty<Element>();
                if (children != null)
                {
                    foreach (var unknown in xml.Elements().Where(e => children(e, out var _) == false))
                    {
                        var (ln, co) = (((IXmlLineInfo)xml).LineNumber, ((IXmlLineInfo)xml).LinePosition);
                        Console.WriteLine("WARNING: unknown element '{0}' @{1}", unknown.Name, (ln, co));
                    }
                }

                result = new Element(name.LocalName, attrs, refrs, elements);
                return true;
            }
            result = default;
            return false;
        };
    }

    /// <summary>
    /// parse alternatives
    /// </summary>
    /// <param name="alternatives"></param>
    /// <returns></returns>
    internal static XmlParser Parser(params XmlParser[] alternatives)
    {
        return (XElement xml, [MaybeNullWhen(false)] out Element result) =>
        {
            foreach (var alt in alternatives)
            {
                if (alt(xml, out var v))
                {
                    result = v;
                    return true;
                }
            }
            result = default;
            return false;
        };
    }

    public static bool TryParseReferenceString(string reference, [NotNullWhen(false)] out (string name, string[] types) result)
    {
        var match = ReferenceRegex().Match(reference);
        if (match.Success)
        {
            var name = match.Groups[1].Value;
            var types = match.Groups[2].Captures.Cast<Capture>().Select(c => c.Value);

            result = (name, types.ToArray());
            return true;
        }
        result = default;
        return false;
    }

    [GeneratedRegex(@"(?<name>\w+):((?<type>\w+)(\|(?<type>\w+))*)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture, "en-US")]
    private static partial Regex ReferenceRegex();
}


