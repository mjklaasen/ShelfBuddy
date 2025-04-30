namespace ShelfBuddy.SharedKernel.Extensions;

public static class DictionaryExtensions
{
    public static bool ContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, IEqualityComparer<TKey> comparer)
    {
        if (dictionary.ContainsKey(key))
        {
            return true;
        }
        foreach (var dictKey in dictionary.Keys)
        {
            if (comparer.Equals(key, dictKey))
            {
                return true;
            }
        }
        return false;
    }
}