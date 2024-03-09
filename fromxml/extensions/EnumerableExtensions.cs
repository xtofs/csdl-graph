namespace parsing;

static class EnumerableExtensions
{
    public delegate bool Try<S, T>(S source, [MaybeNullWhen(false)] out T result);

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
}
