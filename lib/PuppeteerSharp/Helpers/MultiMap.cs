using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PuppeteerSharp.Helpers
{
    internal class MultiMap<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, IList<TValue>> _map = new ConcurrentDictionary<TKey, IList<TValue>>();

        internal void Add(TKey key, TValue value)
            => _map.GetOrAdd(key, _ => new ConcurrentList<TValue>()).Add(value);

        internal IList<TValue> Get(TKey key)
            => _map.TryGetValue(key, out var set) ? set : new ConcurrentList<TValue>();

        internal bool Has(TKey key, TValue value)
            => _map.TryGetValue(key, out var set) && set.Contains(value);

        internal bool Delete(TKey key, TValue value)
            => _map.TryGetValue(key, out var set) && set.Remove(value);

        internal TValue FirstValue(TKey key)
            => _map.TryGetValue(key, out var set) ? set.FirstOrDefault() : default;
    }
}
