using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    internal class NetworkEventManager
    {
        private readonly ConcurrentDictionary<string, Request> _requestIdToRequest = new();
        private readonly ConcurrentDictionary<string, RequestWillBeSentPayload> _requestIdToRequestWillBeSentEvent = new();
        private readonly ConcurrentDictionary<string, FetchRequestPausedResponse> _requestPausedMap = new();
        private readonly ConcurrentDictionary<string, Request> _httpRequestsMap = new();
        private readonly ConcurrentDictionary<string, QueuedEventGroup> _queuedEventGroupMap = new();
        private readonly ConcurrentDictionary<string, List<RedirectInfo>> _queuedRedirectInfoMap = new();
        private readonly ConcurrentDictionary<string, List<ResponseReceivedExtraInfoResponse>> _responseReceivedExtraInfoMap = new();

        public int NumRequestsInProgress
            => _httpRequestsMap.Values.Where(r => r.Response == null).Count();

        internal void ForgetRequest(string requestId)
            => _requestIdToRequestWillBeSentEvent.TryRemove(requestId, out _);

        internal void Forget(string requestId)
        {
            _requestIdToRequestWillBeSentEvent.TryRemove(requestId, out _);
            _requestPausedMap.TryRemove(requestId, out _);
            _queuedEventGroupMap.TryRemove(requestId, out _);
            _queuedRedirectInfoMap.TryRemove(requestId, out _);
            _responseReceivedExtraInfoMap.TryRemove(requestId, out _);
        }

        internal List<ResponseReceivedExtraInfoResponse> ResponseExtraInfo(string networkRequestId)
        {
            if (!_responseReceivedExtraInfoMap.ContainsKey(networkRequestId)) {
              _responseReceivedExtraInfoMap.TryAdd(networkRequestId, new List<ResponseReceivedExtraInfoResponse>());
            }
            _responseReceivedExtraInfoMap.TryGetValue(networkRequestId, out var result);
            return result;
        }

        internal FetchRequestPausedResponse GetRequestWillBeSent(string networkRequestId)
        {
            _requestPausedMap.TryGetValue(networkRequestId, out var result);
            return result;
        }

        private RedirectInfo[] QueuedRedirectInfo(string fetchRequestId)
        {
            if (!_queuedRedirectInfoMap.ContainsKey(fetchRequestId))
            {
                _queuedRedirectInfoMap.TryAdd(fetchRequestId, new List<RedirectInfo>());
            }
            _queuedRedirectInfoMap.TryGetValue(fetchRequestId, out var result);
            return result.ToArray();
        }

        internal RedirectInfo TakeQueuedRedirectInfo(string fetchRequestId)
            => QueuedRedirectInfo(fetchRequestId).FirstOrDefault();

        internal QueuedEventGroup GetQueuedEventGroup(string networkRequestId)
        {
            _queuedEventGroupMap.TryGetValue(networkRequestId, out var result);
            return result;
        }
    }
}