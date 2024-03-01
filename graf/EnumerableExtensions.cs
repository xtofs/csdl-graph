namespace graf;

public static class EnumerableExtensions
{
    public static IEnumerable<(int, T)> Enumerate<T>(this IEnumerable<T> items)
    {
        var i = 0;
        foreach (var item in items)
        {
            yield return (i, item);
            i += 1;
        }
    }
}