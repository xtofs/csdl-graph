
namespace test;

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        // https://learn.microsoft.com/en-us/dotnet/core/additional-tools/xml-serializer-generator
        var serializer = new XmlSerializer<Edmx>();

        var edmx = serializer.Deserialize(File.OpenRead(@"D:\source\csdl-graph\documents\edmx-example.xml")) as Edmx;

        // Console.WriteLine(x);

        // serializer.Serialize(Console.Out, o);
        // foreach (var r in edmx.References)
        // {
        //     var client = new HttpClient();
        //     var stream = await client.GetStreamAsync(r.Uri);
        //     serializer.Deserialize(stream);
        // }

        var x = edmx.DataServices.Schema;
        System.Console.WriteLine(x);
    }
}

[XmlRoot(ElementName = "Edmx", Namespace = XmlNamespaces.EDMX)]
public sealed class Edmx
{
    [XmlElement(ElementName = "Reference", Namespace = XmlNamespaces.EDMX)]
    required public Reference[] References { get; init; }

    [XmlElement(ElementName = "DataServices", Namespace = XmlNamespaces.EDMX)]
    required public DataServices DataServices { get; init; }

    [XmlAttribute(AttributeName = "Version")]
    required public string Version { get; init; }

    [XmlAttribute("schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    public string xsiSchemaLocation = "http://docs.oasis-open.org/odata/ns/edmx https://docs.oasis-open.org/odata/odata-csdl-xml/v4.01/os/schemas/edmx.xsd http://docs.oasis-open.org/odata/ns/edm https://docs.oasis-open.org/odata/odata-csdl-xml/v4.01/os/schemas/edm.xsd";
}


[XmlRoot(ElementName = "DataServices", Namespace = XmlNamespaces.EDMX)]
public sealed class DataServices
{

    [XmlElement(ElementName = "Schema", Namespace = XmlNamespaces.EDM)]
    required public XElement Schema { get; init; }
}



// [XmlRoot(ElementName = "Annotation", Namespace = XmlNamespaces.EDM)]
// public class Annotation
// {
//     [XmlAttribute(AttributeName = "Term")]
//     public string Term { get; set; }
// }

[XmlRoot(ElementName = "Include", Namespace = XmlNamespaces.EDMX)]
public sealed class Include
{
    // [XmlElement(ElementName = "Annotation", Namespace = XmlNamespaces.EDM)]
    // public Annotation Annotation { get; set; }

    [XmlAttribute(AttributeName = "Namespace")]
    public required string Namespace { get; init; }

    [XmlAttribute(AttributeName = "Alias")]
    public required string? Alias { get; init; }

    private bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"Namespace = {Namespace}");
        if (Alias != null) { builder.Append($", Alias = {Alias}"); }
        return true;
    }
}

[XmlRoot(ElementName = "Reference", Namespace = XmlNamespaces.EDMX)]
public sealed class Reference
{
    [XmlElement(ElementName = "Include", Namespace = XmlNamespaces.EDMX)]
    required public Include Include { get; set; }

    [XmlAttribute(AttributeName = "Uri")]
    required public string Uri { get; init; }
}

[XmlRoot(ElementName = "Schema", Namespace = XmlNamespaces.EDM)]
public sealed class Schema
{
    [XmlAttribute(AttributeName = "Namespace")]
    required public string Namespace { get; init; }

    [XmlAttribute(AttributeName = "Alias")]
    required public string? Alias { get; init; }

    private bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"Namespace = {Namespace}");
        if (Alias != null) { builder.Append($", Alias = {Alias}"); }
        return true;
    }
}


internal static class XmlNamespaces
{

    public const string EDMX = "http://docs.oasis-open.org/odata/ns/edmx";
    public const string EDM = "http://docs.oasis-open.org/odata/ns/edm";
}


class XmlSerializer<T>()
{
    private readonly XmlSerializer _serializer = new(typeof(T));
    public T Deserialize(Stream stream)
    {
        return (T)_serializer.Deserialize(stream)!;
    }
}