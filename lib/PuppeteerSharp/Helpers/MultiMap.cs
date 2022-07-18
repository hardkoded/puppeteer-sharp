using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CefSharp.DevTools.Dom.Helpers
{
    internal class MultiMap<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, ICollection<TValue>> _map = new ConcurrentDictionary<TKey, ICollection<TValue>>();

        internal void Add(TKey key, TValue value)
            => _map.GetOrAdd(key, _ => new ConcurrentSet<TValue>()).Add(value);

        internal ICollection<TValue> Get(TKey key)
            => _map.TryGetValue(key, out var set) ? set : new ConcurrentSet<TValue>();

        internal bool Has(TKey key, TValue value)
            => _map.TryGetValue(key, out var set) && set.Contains(value);

        internal bool Delete(TKey key, TValue value)
            => _map.TryGetValue(key, out var set) && set.Remove(value);

        internal TValue FirstValue(TKey key)
            => _map.TryGetValue(key, out var set) ? set.FirstOrDefault() : default;
    }
}
