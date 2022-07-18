using System;
using System.Collections.Generic;
using System.Linq;

namespace CefSharp.DevTools.Dom.Helpers
{
    /// <summary>
    /// Dictionary Extensions
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Clone
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dic">dictionary</param>
        /// <returns>cloned dictionary</returns>
        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            return dic.ToDictionary(entry => entry.Key, entry => entry.Value);
        }

#if NETFRAMEWORK
        /// <summary>
        /// GetValue or default
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dictionary">dictionary</param>
        /// <param name="key">key</param>
        /// <returns>value or default</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            dictionary.TryGetValue(key, out TValue ret);
            return ret;
        }
#endif
    }
}
