using System.Collections.Generic;

namespace FantasySimulator.Core
{
    public static class CollectionExtensions
    {
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            var value = dictionary[key];
            return value;
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            var value = default(TValue);
            if (dictionary.ContainsKey(key))
                value = dictionary.Get(key);
            return value;
        }
    }
}
