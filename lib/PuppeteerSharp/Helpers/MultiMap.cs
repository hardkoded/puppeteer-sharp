using System.Collections.Generic;
using System.Linq;

namespace PuppeteerSharp.Helpers
{
    internal class MultiMap<TKey, TValue>
    {
        private readonly Dictionary<TKey, HashSet<TValue>> _map = new Dictionary<TKey, HashSet<TValue>>();

        internal void Add(TKey key, TValue value)
        {
            if (_map.TryGetValue(key, out var set))
            {
                set.Add(value);
            }
            else
            {
                set = new HashSet<TValue> { value };
                _map.Add(key, set);
            }
        }

        internal HashSet<TValue> Get(TKey key)
            => _map.TryGetValue(key, out var set) ? set : new HashSet<TValue>();

        internal bool Has(TKey key, TValue value)
            => _map.TryGetValue(key, out var set) && set.Contains(value);

        internal bool Delete(TKey key, TValue value)
            => _map.TryGetValue(key, out var set) && set.Remove(value);

        internal TValue FirstValue(TKey key)
            => _map.TryGetValue(key, out var set) ? set.FirstOrDefault() : default(TValue);
    }
}
