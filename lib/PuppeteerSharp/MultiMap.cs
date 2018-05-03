using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal class MultiMap<TKey, TValue>
    {
        private readonly Dictionary<TKey, HashSet<TValue>> _map = new Dictionary<TKey, HashSet<TValue>>();

        public void Add(TKey key, TValue value)
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

        public HashSet<TValue> Get(TKey key)
            => _map.TryGetValue(key, out var set) ? set : new HashSet<TValue>();

        public bool Has(TKey key, TValue value)
            => _map.TryGetValue(key, out var set) && set.Contains(value);

        public bool Delete(TKey key, TValue value) 
            => _map.TryGetValue(key, out var set) && set.Remove(value);

        public TValue FirstValue(TKey key)
            => _map.TryGetValue(key, out var set) ? set.FirstOrDefault() : default(TValue);
    }
}
