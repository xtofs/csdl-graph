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



    public static IEnumerable<TResult> Pairwise<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> resultSelector)
    {
        TSource previous = default!;

        using var it = source.GetEnumerator();
        if (it.MoveNext())
        { previous = it.Current; }

        while (it.MoveNext())
        { yield return resultSelector(previous, previous = it.Current); }
    }
    public static IEnumerable<(TSource, TSource)> Pairwise<TSource>(this IEnumerable<TSource> source)
        => Pairwise(source, ValueTuple.Create);
}

public static class DictionaryExtensions
{
    public static void AddItem<K, V>(this Dictionary<K, List<V>> dictionary, K key, V value)
        where K : notnull
    {
        if (dictionary.TryGetValue(key, out var values))
        {
            values.Add(value);
        }
        else
        {
            dictionary.Add(key, [value]);
        }
    }
}