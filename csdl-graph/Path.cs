namespace graf;

public static class ModelPath
{
    public static IEnumerable<string> Split(string path)
    {
        var fields = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return fields.SelectMany(SplitField); ;
    }

    private static IEnumerable<string> SplitField(string field)
    {
        var atIx = field.IndexOf('@');
        switch (atIx)
        {
            case -1: // no @ sign, split at last '.'
                return field.SplitAtLast('.');
            case 0: // starts with @ sign
                return [field];
            case > 0: //  @ sign in the middle
                return [field[..atIx], field[atIx..]];
            default:
                throw new InvalidDataException();
        }
    }

}

static class StringExtensions
{
    public static string[] SplitAtLast(this string field, char separator)
    {
        var ix = field.LastIndexOf(separator);
        return ix > 0 ? [field[..ix], field[(ix + 1)..]] : [field];
    }
}
