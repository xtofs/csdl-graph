namespace Csdl.Graph;

static class StringExtensions
{
    public static string[] SplitAtLast(this string field, char separator)
    {
        var ix = field.LastIndexOf(separator);
        return ix > 0 ? [field[..ix], field[(ix + 1)..]] : [field];
    }
}
