using System.Text;

public static class StringBuilderExtensions
{
    public static void AppendList<T>(this StringBuilder builder, string format, IEnumerable<T> values, string separator)
    {
        builder.AppendFormat(format, string.Join(separator, values));
    }
}