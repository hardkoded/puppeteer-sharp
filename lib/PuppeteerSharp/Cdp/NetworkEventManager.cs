using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Cdp
{
    internal class NetworkEventManager
    {
        private readonly ConcurrentDictionary<string, RequestWillBeSentPayload> _requestWillBeSentMap = new();
        private readonly ConcurrentDictionary<string, FetchRequestPausedResponse> _requestPausedMap = new();
        private readonly ConcurrentDictionary<string, CdpHttpRequest> _httpRequestsMap = new();
        private readonly ConcurrentDictionary<string, QueuedEventGroup> _queuedEventGroupMap = new();
        private readonly ConcurrentDictionary<string, List<RedirectInfo>> _queuedRedirectInfoMap = new();
        private readonly ConcurrentDictionary<string, List<ResponseReceivedExtraInfoResponse>> _responseReceivedExtraInfoMap = new();

        public int NumRequestsInProgress
            => _httpRequestsMap.Values.Count(r => r.Response == null);

        internal void Forget(string requestId)
        {
            _requestWillBeSentMap.TryRemove(requestId, out _);
            _requestPausedMap.TryRemove(requestId, out _);
            _queuedEventGroupMap.TryRemove(requestId, out _);
            _queuedRedirectInfoMap.TryRemove(requestId, out _);
            _responseReceivedExtraInfoMap.TryRemove(requestId, out _);
            _httpRequestsMap.TryRemove(requestId, out _);
        }

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

        internal void StoreRequestWillBeSent(string networkRequestId, RequestWillBeSentPayload e)
            => _requestWillBeSentMap.AddOrUpdate(networkRequestId, e, (_, _) => e);

        internal RequestWillBeSentPayload GetRequestWillBeSent(string networkRequestId)
        {
            _requestWillBeSentMap.TryGetValue(networkRequestId, out var result);
            return result;
        }

        internal void ForgetRequestWillBeSent(string networkRequestId)
            => _requestWillBeSentMap.TryRemove(networkRequestId, out _);

        internal FetchRequestPausedResponse GetRequestPaused(string networkRequestId)
        {
            _requestPausedMap.TryGetValue(networkRequestId, out var result);
            return result;
        }

        internal void ForgetRequestPaused(string networkRequestId)
            => _requestPausedMap.TryRemove(networkRequestId, out _);

        internal void StoreRequestPaused(string networkRequestId, FetchRequestPausedResponse e)
            => _requestPausedMap.AddOrUpdate(networkRequestId, e, (_, _) => e);

        internal CdpHttpRequest GetRequest(string networkRequestId)
        {
            _httpRequestsMap.TryGetValue(networkRequestId, out var result);
            return result;
        }

        internal void StoreRequest(string networkRequestId, CdpHttpRequest request)
            => _httpRequestsMap.AddOrUpdate(networkRequestId, request, (_, _) => request);

        internal void ForgetRequest(string requestId)
            => _requestWillBeSentMap.TryRemove(requestId, out _);

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
