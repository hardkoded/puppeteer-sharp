using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    internal class NetworkManager
    {
        private readonly List<string> _attemptedAuthentications = new List<string>();
        private readonly ConcurrentDictionary<string, NetworkRequest> _requests = new ConcurrentDictionary<string, NetworkRequest>();
        private readonly ConcurrentDictionary<string, (string RequestId, string InterceptionId)> _requestHashesToIds = new ConcurrentDictionary<string, (string, string)>();
        private readonly CDPSession _client;
        private readonly ILogger<NetworkManager> _logger;

        private Dictionary<string, string> _extraHTTPHeaders;
        private Credentials _credentials;
        private bool _userRequestInterceptionEnabled = false;
        private bool _protocolRequestInterceptionEnabled = false;
        private bool _offline = false;

        internal event EventHandler<ResponseCreatedEventArgs> Response;
        internal event EventHandler<RequestEventArgs> Request;
        internal event EventHandler<RequestEventArgs> RequestFinished;
        internal event EventHandler<RequestEventArgs> RequestFailed;

        internal Dictionary<string, string> ExtraHTTPHeaders => _extraHTTPHeaders?.Clone();
        internal FrameManager FrameManager { get; set; }

        internal NetworkManager(CDPSession client)
        {
            _client = client;
            _client.MessageReceived += Client_MessageReceived;
            _logger = _client.Connection.LoggerFactory.CreateLogger<NetworkManager>();
        }

        internal Task SetExtraHTTPHeadersAsync(Dictionary<string, string> extraHTTPHeaders)
        {
            _extraHTTPHeaders = new Dictionary<string, string>();

            foreach (var item in extraHTTPHeaders)
            {
                _extraHTTPHeaders[item.Key.ToLower()] = item.Value;
            }
            return _client.SendAsync("Network.setExtraHTTPHeaders", new NetworkSetExtraHTTPHeadersRequest
            {
                Headers = _extraHTTPHeaders
            });
        }

        internal Task SetRequestInterceptionAsync(bool value)
        {
            _userRequestInterceptionEnabled = value;
            return UpdateProtocolRequestInterceptionAsync();
        }

        internal Task AuthenticateAsync(Credentials credentials)
        {
            _credentials = credentials;
            return UpdateProtocolRequestInterceptionAsync();
        }

        internal async Task SetOfflineModeAsync(bool value)
        {
            if (_offline != value)
            {
                _offline = value;

                await _client.SendAsync("Network.emulateNetworkConditions", new NetworkEmulateNetworkConditionsRequest
                {
                    Offline = value,
                    Latency = 0,
                    DownloadThroughput = -1,
                    UploadThroughput = -1
                }).ConfigureAwait(false);
            }
        }

        internal Task SetUserAgentAsync(string userAgent)
            => _client.SendAsync("Network.setUserAgentOverride", new NetworkSetUserAgentOverrideRequest
            {
                UserAgent = userAgent
            });

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                switch (e.MessageID)
                {
                    case "Network.requestWillBeSent":
                        await OnRequestWillBeSentAsync(e.MessageData.ToObject<RequestWillBeSentPayload>(true));
                        break;
                    case "Network.requestIntercepted":
                        await OnRequestInterceptedAsync(e.MessageData.ToObject<RequestInterceptedResponse>(true));
                        break;
                    case "Network.requestServedFromCache":
                        OnRequestServedFromCache(e.MessageData.ToObject<RequestServedFromCacheResponse>(true));
                        break;
                    case "Network.responseReceived":
                        OnResponseReceived(e.MessageData.ToObject<ResponseReceivedResponse>(true));
                        break;
                    case "Network.loadingFinished":
                        OnLoadingFinished(e.MessageData.ToObject<LoadingFinishedResponse>(true));
                        break;
                    case "Network.loadingFailed":
                        OnLoadingFailed(e.MessageData.ToObject<LoadingFailedResponse>(true));
                        break;
                }
            }
            catch (Exception ex)
            {
                var message = $"NetworkManager failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
                _logger.LogError(ex, message);
                _client.Close(message);
            }
        }

        private void OnLoadingFailed(LoadingFailedResponse e)
        {
            // For certain requestIds we never receive requestWillBeSent event. See: https://crbug.com/750469
            if (_requests.TryRemove(e.RequestId, out var networkRequest))
            {
                var request = networkRequest.Complete();
                request.Failure = e.ErrorText;
                request.Response?.BodyLoadedTaskWrapper.TrySetException(new PuppeteerException(e.ErrorText));
                RequestFailed?.Invoke(this, new RequestEventArgs
                {
                    Request = request
                });
            }
        }

        private void OnLoadingFinished(LoadingFinishedResponse e)
        {
            // For certain requestIds we never receive requestWillBeSent event. See: https://crbug.com/750469
            if (_requests.TryRemove(e.RequestId, out var networkRequest))
            {
                var request = networkRequest.Complete();
                request.Response?.BodyLoadedTaskWrapper.TrySetResult(true);
                RequestFinished?.Invoke(this, new RequestEventArgs
                {
                    Request = request
                });
            }
        }

        private async Task OnRequestWillBeSentAsync(RequestWillBeSentPayload e)
        {
            var added = _requestHashesToIds.TryAdd(e.Request.Hash, (e.RequestId, null));
            var networkRequest = _requests.GetOrAdd(e.RequestId, requestId => new NetworkRequest());
            var request = new Request(_client,
                                        await FrameManager?.GetFrameAsync(e.FrameId),
                                        null,
                                        _userRequestInterceptionEnabled,
                                        e,
                                        new List<Request>());

            if (e.RedirectResponse != null)
            {
                var rq = OnResponse(e.RequestId, e.RedirectResponse, request);
                RequestFinished?.Invoke(this, new RequestEventArgs
                {
                    Request = rq
                });
            }
            else
            {
                networkRequest.Add(request);
            }

            if (!added && _requestHashesToIds.TryRemove(e.Request.Hash, out var ids)
                && ids.InterceptionId != null)
            {
                request.InterceptionId = ids.InterceptionId;
            }

            if (!(_userRequestInterceptionEnabled || _protocolRequestInterceptionEnabled)
                || request.InterceptionId != null)
            {
                Request?.Invoke(this, new RequestEventArgs
                {
                    Request = request
                });
            }
        }

        private void OnResponseReceived(ResponseReceivedResponse e) => OnResponse(e.RequestId, e.Response);

        private Request OnResponse(string requestId, ResponsePayload payload, Request nextRequest = null)
        {
            // FileUpload sends a response without a matching request.
            if (_requests.TryGetValue(requestId, out var request))
            {
                if (request.Request.InterceptionId != null)
                {
                    _attemptedAuthentications.Remove(request.Request.InterceptionId);
                }

                var response = nextRequest == null ? request.Add(payload, _client) : request.Add(payload, _client, nextRequest);

                Response?.Invoke(this, new ResponseCreatedEventArgs
                {
                    Response = response
                });

                return response.Request;
            }

            return null;
        }

        private void OnRequestServedFromCache(RequestServedFromCacheResponse response)
        {
            if (_requests.TryGetValue(response.RequestId, out var networkRequest))
            {
                networkRequest.Request.FromMemoryCache = true;
            }
        }

        private async Task OnRequestInterceptedAsync(RequestInterceptedResponse e)
        {
            if (e.AuthChallenge != null)
            {
                var response = "Default";
                if (_attemptedAuthentications.Contains(e.InterceptionId))
                {
                    response = "CancelAuth";
                }
                else if (_credentials != null)
                {
                    response = "ProvideCredentials";
                    _attemptedAuthentications.Add(e.InterceptionId);
                }
                var credentials = _credentials ?? new Credentials();
                try
                {
                    await _client.SendAsync("Network.continueInterceptedRequest", new NetworkContinueInterceptedRequestRequest
                    {
                        InterceptionId = e.InterceptionId,
                        AuthChallengeResponse = new NetworkContinueInterceptedRequestChallengeResponse
                        {
                            Response = response,
                            Username = credentials.Username,
                            Password = credentials.Password
                        }
                    }).ConfigureAwait(false);
                }
                catch (PuppeteerException ex)
                {
                    _logger.LogError(ex.ToString());
                }
                return;
            }
            if (!_userRequestInterceptionEnabled && _protocolRequestInterceptionEnabled)
            {
                try
                {
                    await _client.SendAsync("Network.continueInterceptedRequest", new NetworkContinueInterceptedRequestRequest
                    {
                        InterceptionId = e.InterceptionId
                    }).ConfigureAwait(false);
                }
                catch (PuppeteerException ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }

            if (_requestHashesToIds.TryRemove(e.Request.Hash, out var ids)
                && _requests.TryGetValue(ids.RequestId, out var networkRequest))
            {
                var request = networkRequest.Find(e.Request.Url);
                request.InterceptionId = e.InterceptionId;
                Request?.Invoke(this, new RequestEventArgs
                {
                    Request = request
                });
            }
            else
            {
                _requestHashesToIds.TryAdd(e.Request.Hash, (null, e.InterceptionId));
            }
        }

        private async Task UpdateProtocolRequestInterceptionAsync()
        {
            var enabled = _userRequestInterceptionEnabled || _credentials != null;
            if (enabled == _protocolRequestInterceptionEnabled)
            {
                return;
            }

            _protocolRequestInterceptionEnabled = enabled;
            var patterns = enabled ?
                new object[] { new KeyValuePair<string, string>("urlPattern", "*") } :
                Array.Empty<object>();

            await Task.WhenAll(
                _client.SendAsync("Network.setCacheDisabled", new NetworkSetCacheDisabledRequest
                {
                    CacheDisabled = enabled
                }),
                _client.SendAsync("Network.setRequestInterception", new NetworkSetRequestInterceptionRequest
                {
                    Patterns = patterns
                })
            ).ConfigureAwait(false);
        }

        internal sealed class NetworkRequest
        {
            private readonly List<Request> _requestChain = new List<Request>();

            internal Request Request => _requestChain.Count == 0 ? null : _requestChain[_requestChain.Count - 1];

            internal void Add(Request request) => _requestChain.Add(request);

            internal Response Add(ResponsePayload response, CDPSession client, Request nextRequest = null)
            {
                var request = GetRequestChain().Reverse().FirstOrDefault(x => x.Url == response.Url);
                var rsp = new Response(client, request, response);
                if (nextRequest != null)
                {
                    request.RedirectChainList.Add(nextRequest);
                    rsp.BodyLoadedTaskWrapper.TrySetException(new PuppeteerException("Response body is unavailable for redirect responses"));
                }
                return request == null ? null : request.Response = rsp;
            }

            internal Request Find(string url) => GetRequestChain().FirstOrDefault(x => x.Url == url);

            internal Request Complete()
            {
                var chain = GetRequestChain().ToList();
                var lastRequest = chain[chain.Count - 1];
                lastRequest.RedirectChainList.AddRange(chain.TakeWhile(x => x != lastRequest));
                return lastRequest;
            }

            internal IEnumerable<Request> GetRequestChain()
            {
                foreach (var request in _requestChain)
                {
                    yield return request;
                    foreach (var redirect in request.RedirectChainList)
                    {
                        yield return redirect;
                    }
                }
            }
        }
    }
}