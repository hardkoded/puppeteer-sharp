using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Cdp
{
    internal class NetworkEventManager
    {
        private readonly ConcurrentDictionary<string, StoredRequestWillBeSent> _requestWillBeSentMap = new();
        private readonly ConcurrentDictionary<string, FetchRequestPausedResponse> _requestPausedMap = new();
        private readonly ConcurrentDictionary<string, CDPSession> _requestPausedClientMap = new();
        private readonly ConcurrentDictionary<string, CdpHttpRequest> _httpRequestsMap = new();
        private readonly ConcurrentDictionary<string, QueuedEventGroup> _queuedEventGroupMap = new();
        private readonly ConcurrentDictionary<string, List<RedirectInfo>> _queuedRedirectInfoMap = new();
        private readonly ConcurrentDictionary<string, List<RequestWillBeSentExtraInfoResponse>> _requestWillBeSentExtraInfoMap = new();
        private readonly ConcurrentDictionary<string, List<ResponseReceivedExtraInfoResponse>> _responseReceivedExtraInfoMap = new();

        public int NumRequestsInProgress
            => _httpRequestsMap.Values.Count(r => r.Response == null);

        internal void Forget(string requestId)
        {
            _requestWillBeSentMap.TryRemove(requestId, out _);
            _requestPausedMap.TryRemove(requestId, out _);
            _requestPausedClientMap.TryRemove(requestId, out _);
            _requestWillBeSentExtraInfoMap.TryRemove(requestId, out _);
            _queuedEventGroupMap.TryRemove(requestId, out _);
            _queuedRedirectInfoMap.TryRemove(requestId, out _);
            _responseReceivedExtraInfoMap.TryRemove(requestId, out _);
            _httpRequestsMap.TryRemove(requestId, out _);
        }

        /// <summary>
        /// Ids of the requests whose Network events come from <paramref name="client"/>, both the
        /// ones already emitted and the ones still waiting for more events.
        /// </summary>
        /// <param name="client">The session.</param>
        /// <returns>The request ids.</returns>
        internal HashSet<string> RequestIdsForClient(CDPSession client)
        {
            var ids = new HashSet<string>();
            foreach (var item in _httpRequestsMap)
            {
                if (item.Value.Client == client)
                {
                    ids.Add(item.Key);
                }
            }

            foreach (var item in _requestWillBeSentMap)
            {
                if (item.Value.Client == client)
                {
                    ids.Add(item.Key);
                }
            }

            foreach (var item in _queuedRedirectInfoMap)
            {
                if (item.Value.ToArray().Any(info => info.Client == client))
                {
                    ids.Add(item.Key);
                }
            }

            return ids;
        }

        internal List<RequestWillBeSentExtraInfoResponse> RequestExtraInfo(string networkRequestId)
            => _requestWillBeSentExtraInfoMap.GetOrAdd(networkRequestId, static _ => new());

        internal List<ResponseReceivedExtraInfoResponse> ResponseExtraInfo(string networkRequestId)
            => _responseReceivedExtraInfoMap.GetOrAdd(networkRequestId, static _ => new());

        internal void QueueRedirectInfo(string fetchRequestId, RedirectInfo redirectInfo)
            => QueuedRedirectInfo(fetchRequestId).Add(redirectInfo);

        internal RedirectInfo TakeQueuedRedirectInfo(string fetchRequestId)
        {
            var list = QueuedRedirectInfo(fetchRequestId);
            var result = list.FirstOrDefault();

            if (result != null)
            {
                list.Remove(result);
            }

            return result;
        }

        internal ResponseReceivedExtraInfoResponse ShiftResponseExtraInfo(string networkRequestId)
        {
            var list = _responseReceivedExtraInfoMap.GetOrAdd(networkRequestId, static _ => new());
            var result = list.FirstOrDefault();

            if (result != null)
            {
                list.Remove(result);
            }

            return result;
        }

        internal void StoreRequestWillBeSent(string networkRequestId, RequestWillBeSentResponse e, CDPSession client)
        {
            var stored = new StoredRequestWillBeSent { Event = e, Client = client };
            _requestWillBeSentMap.AddOrUpdate(networkRequestId, stored, (_, _) => stored);
        }

        internal StoredRequestWillBeSent GetStoredRequestWillBeSent(string networkRequestId)
        {
            _requestWillBeSentMap.TryGetValue(networkRequestId, out var result);
            return result;
        }

        internal void ForgetRequestWillBeSent(string networkRequestId)
            => _requestWillBeSentMap.TryRemove(networkRequestId, out _);

        /// <summary>
        /// With request interception enabled, Network.loadingFailed can arrive before
        /// the Fetch.requestPaused event that creates the request. The failure is
        /// kept with the stored requestWillBeSent event until then, so that
        /// <c>RequestFailed</c> is not lost.
        /// </summary>
        /// <param name="networkRequestId">The network request id.</param>
        /// <param name="e">The loading failed event.</param>
        /// <param name="client">The session that reported the failure.</param>
        internal void StoreLoadingFailedBeforeRequest(string networkRequestId, LoadingFailedEventResponse e, CDPSession client)
        {
            if (_requestWillBeSentMap.TryGetValue(networkRequestId, out var stored))
            {
                stored.LoadingFailed = new StoredLoadingFailed { Event = e, Client = client };
            }
        }

        internal FetchRequestPausedResponse GetRequestPaused(string networkRequestId)
        {
            _requestPausedMap.TryGetValue(networkRequestId, out var result);
            return result;
        }

        internal void ForgetRequestPaused(string networkRequestId)
        {
            _requestPausedMap.TryRemove(networkRequestId, out _);
            _requestPausedClientMap.TryRemove(networkRequestId, out _);
        }

        internal void StoreRequestPaused(string networkRequestId, FetchRequestPausedResponse e, CDPSession client)
        {
            _requestPausedMap.AddOrUpdate(networkRequestId, e, (_, _) => e);
            _requestPausedClientMap.AddOrUpdate(networkRequestId, client, (_, _) => client);
        }

        internal CDPSession GetRequestPausedClient(string networkRequestId)
        {
            _requestPausedClientMap.TryGetValue(networkRequestId, out var result);
            return result;
        }

        internal CdpHttpRequest GetRequest(string networkRequestId)
        {
            _httpRequestsMap.TryGetValue(networkRequestId, out var result);
            return result;
        }

        internal void StoreRequest(string networkRequestId, CdpHttpRequest request)
            => _httpRequestsMap.AddOrUpdate(networkRequestId, request, (_, _) => request);

        internal void ForgetRequest(string requestId)
            => _httpRequestsMap.TryRemove(requestId, out _);

        internal void QueuedEventGroup(string networkRequestId, QueuedEventGroup group)
            => _queuedEventGroupMap.AddOrUpdate(networkRequestId, group, (_, _) => group);

        internal QueuedEventGroup GetQueuedEventGroup(string networkRequestId)
        {
            _queuedEventGroupMap.TryGetValue(networkRequestId, out var result);
            return result;
        }

        // Puppeteer doesn't have this. but it seems that .NET needs this to avoid race conditions
        internal void ForgetQueuedEventGroup(string networkRequestId)
            => _queuedEventGroupMap.TryRemove(networkRequestId, out _);

        private List<RedirectInfo> QueuedRedirectInfo(string fetchRequestId)
            => _queuedRedirectInfoMap.GetOrAdd(fetchRequestId, static _ => new());
    }
}
