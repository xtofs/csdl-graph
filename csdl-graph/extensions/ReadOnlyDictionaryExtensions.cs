namespace Csdl.Graph
{
    public static class ReadOnlyDictionaryExtensions
    {
        public static string? Get(this IReadOnlyDictionary<string, string> dict, string key)
            => dict.TryGetValue(key, out var a) ? a : null;
    }
}