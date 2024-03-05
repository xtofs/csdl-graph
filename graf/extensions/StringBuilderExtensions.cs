
using System.Text;

namespace graf;

static class StringBuilderExtensions
{
    public static void AppendJoinIfAny<T>(this StringBuilder builder, string open, IEnumerable<T> items, string separator, string close)
    {
        var i = 0;
        foreach (var item in items)
        {
            if (i == 0)
            {
                builder.Append(open);
            }
            else
            {
                builder.Append(separator);
            }
            builder.Append(item);
            i += 1;
        }
        if (i > 0) { builder.Append(close); }
    }

    public static void AppendFormatIfNotNull(this StringBuilder builder, string fmt, object? arg0)
    {
        if (arg0 != null)
        {
            builder.AppendFormat(fmt, arg0);
        }
    }
}


