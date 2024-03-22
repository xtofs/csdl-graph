namespace Csdl.Graph;

internal record struct LineInfo(string Path, int LineNumber, int LinePosition)
{
    public LineInfo(string path, XObject xml) : this(path, ((IXmlLineInfo)xml).LineNumber, ((IXmlLineInfo)xml).LinePosition)
    {
    }

    public override readonly string ToString() => $"{Path}({LineNumber},{LinePosition})";
}