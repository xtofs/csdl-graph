namespace parsing;

public interface IXmlParsable<TSelf> where TSelf : IXmlParsable<TSelf>?
{

    static abstract bool TryFromXml(XElement x, [MaybeNullWhen(false)] out TSelf result);
}
