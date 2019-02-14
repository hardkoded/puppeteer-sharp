using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PuppeteerSharp.Helpers
{
    internal class AsyncDictionaryHelper<TKey, TValue>
    {
        public ConcurrentDictionary<TKey, TValue> Dictionary { get; }
        public string TimeoutMessage { get; set; }

        private MultiMap<TKey, TaskCompletionSource<TValue>> _pendingRequests;
        private const int WaitForRequestDelay = 1000;

        public AsyncDictionaryHelper(ConcurrentDictionary<TKey, TValue> dictionary, string timeoutMessage)
        {
            Dictionary = dictionary;
            TimeoutMessage = timeoutMessage;
            _pendingRequests = new MultiMap<TKey, TaskCompletionSource<TValue>>();
        }

        internal async Task<TValue> GetItemAsync(TKey key)
        {
            var tcs = new TaskCompletionSource<TValue>(TaskCreationOptions.RunContinuationsAsynchronously);
            _pendingRequests.Add(key, tcs);

            if (Dictionary.TryGetValue(key, out var item))
            {
                _pendingRequests.Delete(key, tcs);
                return item;
            }

            return await tcs.Task.WithTimeout(WaitForRequestDelay, new PuppeteerException(string.Format(TimeoutMessage, key)));
        }

        internal void AddItem(TKey key, TValue value)
        {
            Dictionary[key] = value;
            foreach (var tcs in _pendingRequests.Get(key))
            {
                tcs.TrySetResult(value);
            }
        }
    }
}
