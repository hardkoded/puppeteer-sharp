using System.Collections.Generic;
using System.Linq;

namespace PuppeteerSharp.Helpers
{
    internal static class DictionaryExtensions
    {
        internal static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            return dic.ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        internal static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            dictionary.TryGetValue(key, out TValue ret);
            return ret;
        }
    }
}
