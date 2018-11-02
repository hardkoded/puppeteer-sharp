using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PuppeteerSharp.Helpers
{
    internal class MultiMap<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, List<TValue>> _map = new ConcurrentDictionary<TKey, List<TValue>>();

        internal void Add(TKey key, TValue value)
        {
            if (_map.TryGetValue(key, out var set))
            {
                set.Add(value);
            }
            else
            {
                set = new List<TValue> { value };
                _map.TryAdd(key, set);
            }
        }

        internal List<TValue> Get(TKey key)
            => _map.TryGetValue(key, out var set) ? set : new List<TValue>();

        internal bool Has(TKey key, TValue value)
            => _map.TryGetValue(key, out var set) && set.Contains(value);

        internal bool Delete(TKey key, TValue value)
            => _map.TryGetValue(key, out var set) && set.Remove(value);

        internal TValue FirstValue(TKey key)
            => _map.TryGetValue(key, out var set) ? set.FirstOrDefault() : default;
    }
}
