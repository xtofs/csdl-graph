using System.Text;
using parsing;

var text = """
    <Schema Alias="self" Namespace="example.org">
        <EnumType Name="a">
            <Member Name="x"/>
            <Member Name="y"/>
        </EnumType>
        <ComplexType Name="b"/> 
    </Schema>
""";

var xml = XDocument.Parse(text);
if (xml.Root != null && Schema.TryFromXml(xml.Root, out var schema))
{
    Console.WriteLine(schema);
}
else
{
    Console.WriteLine("can't parse from XML");
}

sealed record Schema(string Namespace, string? Alias, IEnumerable<SchemaElement> Elements) : IXmlParsable<Schema>
{
    public static bool TryFromXml(XElement xml, [MaybeNullWhen(false)] out Schema result)
    {
        if (xml.TryGetChildrenFromXml<SchemaElement>(XNamespace.None + "Schema", out var elements))
        {
            result = new Schema(xml.GetAttr("Namespace") ?? "?", xml.GetAttr("Alias"), elements);
            return true;
        }
        result = default;
        return false;
    }

    private bool PrintMembers(StringBuilder builder)
    {
        builder.AppendFormat("Namespace={0}", Namespace);
        builder.AppendFormat(", Alias={0}", Alias);
        builder.AppendList(", Elements=[{0}]", Elements, ", ");
        return true;
    }
}

abstract record SchemaElement(string Name) : IXmlParsable<SchemaElement>
{
    public static bool TryFromXml(XElement xml, [MaybeNullWhen(false)] out SchemaElement result)
    {

        switch (xml.Name.LocalName)
        {
            case "EnumType":
                return xml.TryFromXml<EnumType, SchemaElement>(out result);
            case "ComplexType":
                return xml.TryFromXml<ComplexType, SchemaElement>(out result);
        }
        result = default;
        return false;
    }
}


sealed record EnumType(string Name, IEnumerable<EnumMember> Members) : SchemaElement(Name), IXmlParsable<EnumType>
{
    public static bool TryFromXml(XElement xml, [MaybeNullWhen(false)] out EnumType result)
    {
        if (xml.TryGetChildrenFromXml<EnumMember>(XNamespace.None + "EnumType", out var members))
        {
            var name = xml.GetAttr("Name") ?? "?";
            result = new EnumType(name, members);
            return true;
        }
        result = default;
        return false;
    }

    protected override bool PrintMembers(StringBuilder builder)
    {
        builder.AppendFormat("Name={0}", Name);
        builder.AppendList(",  Members=[{0}]", Members, ", ");
        return true;
    }
}

sealed record EnumMember(string Name) : IXmlParsable<EnumMember>
{
    public static bool TryFromXml(XElement x, [MaybeNullWhen(false)] out EnumMember result)
    {
        if (x.Name == XNamespace.None + "Member")
        {
            result = new EnumMember(x.GetAttr("Name") ?? "?");
            return true;
        }
        result = default;
        return false;
    }
}


sealed record ComplexType(string Name) : SchemaElement(Name), IXmlParsable<ComplexType>
{
    public static bool TryFromXml(XElement x, [MaybeNullWhen(false)] out ComplexType result)
    {
        if (x.Name == XNamespace.None + "ComplexType")
        {
            result = new ComplexType(x.GetAttr("Name", ""));
            return true;
        }
        result = default;
        return false;
    }
}
