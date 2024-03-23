namespace Csdl.Graph;

public delegate bool Try<S, T>(S source, [MaybeNullWhen(false)] out T result);

static class EnumerableExtensions
{
    public static IEnumerable<T> TrySelect<S, T>(this IEnumerable<S> items, Try<S, T> selector)
    {
        foreach (var item in items)
        {
            if (selector(item, out var res))
            {
                yield return res;
            }
        }
    }


    public static IEnumerable<(int Index, T Item)> WidthIndex<T>(this IEnumerable<T>? items)
    {
        var i = 0;
        foreach (var item in items ?? []) { yield return (i, item); i += 1; }
    }
}
