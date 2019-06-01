using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PuppeteerSharp.Helpers
{
    internal class AsyncDictionaryHelper<TKey, TValue>
    {
        private readonly string _timeoutMessage;
        private MultiMap<TKey, TaskCompletionSource<TValue>> _pendingRequests;
        private ConcurrentDictionary<TKey, TValue> _dictionary;

        public AsyncDictionaryHelper(ConcurrentDictionary<TKey, TValue> dictionary, string timeoutMessage)
        {
            _dictionary = dictionary;
            _timeoutMessage = timeoutMessage;
            _pendingRequests = new MultiMap<TKey, TaskCompletionSource<TValue>>();
        }

        internal async Task<TValue> GetItemAsync(TKey key)
        {
            var tcs = new TaskCompletionSource<TValue>(TaskCreationOptions.RunContinuationsAsynchronously);
            _pendingRequests.Add(key, tcs);

            if (_dictionary.TryGetValue(key, out var item))
            {
                _pendingRequests.Delete(key, tcs);
                return item;
            }

            return await tcs.Task.WithTimeout(new Action(() =>
                throw new PuppeteerException(string.Format(_timeoutMessage, key)))).ConfigureAwait(false);
        }

        internal void AddItem(TKey key, TValue value)
        {
            _dictionary[key] = value;
            foreach (var tcs in _pendingRequests.Get(key))
            {
                tcs.TrySetResult(value);
            }
        }
    }
}
