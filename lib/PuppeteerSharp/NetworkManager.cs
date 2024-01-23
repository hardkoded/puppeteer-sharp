using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly bool _ignoreHTTPSErrors;
        private readonly NetworkEventManager _networkEventManager = new();
        private readonly ILogger _logger;
        private readonly ConcurrentSet<string> _attemptedAuthentications = [];
        private readonly InternalNetworkConditions _emulatedNetworkConditions = null;
        private readonly ConcurrentDictionary<CDPSession, DisposableActionsStack> _clients = new();
        private readonly FrameManager _frameManager;

        private Dictionary<string, string> _extraHTTPHeaders;
        private Credentials _credentials;
        private bool _userRequestInterceptionEnabled;
        private bool _protocolRequestInterceptionEnabled;
        private bool? _userCacheDisabled;
        private string _userAgent;
        private UserAgentMetadata _userAgentMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkManager"/> class.
        /// </summary>
        /// <param name="ignoreHTTPSErrors">If set to <c>true</c> ignore http errors.</param>
        /// <param name="frameManager">Frame manager.</param>
        /// <param name="connection">NOTE: We only need the connection to create a logger.</param>
        internal NetworkManager(bool ignoreHTTPSErrors, FrameManager frameManager, Connection connection)
        {
            _frameManager = frameManager;
            _ignoreHTTPSErrors = ignoreHTTPSErrors;
            _logger = connection.LoggerFactory.CreateLogger<NetworkManager>();
        }

        internal event EventHandler<ResponseCreatedEventArgs> Response;

        internal event EventHandler<RequestEventArgs> Request;

        internal event EventHandler<RequestEventArgs> RequestFinished;

        internal event EventHandler<RequestEventArgs> RequestFailed;

        internal event EventHandler<RequestEventArgs> RequestServedFromCache;

        internal Dictionary<string, string> ExtraHTTPHeaders => _extraHTTPHeaders?.Clone();

        internal int NumRequestsInProgress => _networkEventManager.NumRequestsInProgress;

        internal Task AddClientAsync(CDPSession client)
        {
            if (_clients.ContainsKey(client))
            {
                return Task.CompletedTask;
            }

            var subscriptions = new DisposableActionsStack();
            _clients[client] = subscriptions;
            subscriptions.Defer(() => client.MessageReceived -= Client_MessageReceived);

            return Task.WhenAll(
                _ignoreHTTPSErrors ? client.SendAsync("Security.setIgnoreCertificateErrors", new SecuritySetIgnoreCertificateErrorsRequest { Ignore = true }) : Task.CompletedTask,
                client.SendAsync("Network.enable"),
                ApplyExtraHTTPHeadersAsync(client),
                ApplyNetworkConditionsAsync(client),
                ApplyProtocolCacheDisabledAsync(client),
                ApplyProtocolRequestInterceptionAsync(client),
                ApplyUserAgentAsync(client));
        }

        internal void RemoveClient(CDPSession client)
        {
            if (!_clients.TryRemove(client, out var subscriptions))
            {
                return;
            }

            subscriptions.Dispose();
        }

        internal async Task AuthenticateAsync(Credentials credentials)
        {
            _credentials = credentials;
            var enabled = _userRequestInterceptionEnabled || _credentials != null;

            if (enabled == _protocolRequestInterceptionEnabled)
            {
                return;
            }

            _protocolRequestInterceptionEnabled = enabled;
            await ApplyToAllClientsAsync(ApplyProtocolRequestInterceptionAsync).ConfigureAwait(false);
        }

        internal Task SetUserAgentAsync(string userAgent, UserAgentMetadata userAgentMetadata)
            => ApplyToAllClientsAsync(client => client.SendAsync(
                "Network.setUserAgentOverride",
                new NetworkSetUserAgentOverrideRequest
                {
                    UserAgent = userAgent,
                    UserAgentMetadata = userAgentMetadata,
                }));

        internal Task SetCacheEnabledAsync(bool enabled)
        {
            _userCacheDisabled = !enabled;
            return ApplyToAllClientsAsync(client => client.SendAsync(
                "Network.setCacheDisabled",
                new NetworkSetCacheDisabledRequest(_userCacheDisabled.Value)));
        }

        internal async Task SetRequestInterceptionAsync(bool value)
        {
            _userRequestInterceptionEnabled = value;
            var enabled = _userRequestInterceptionEnabled || _credentials != null;

            if (enabled == _protocolRequestInterceptionEnabled)
            {
                return;
            }

            _protocolRequestInterceptionEnabled = enabled;
            await ApplyToAllClientsAsync(ApplyProtocolRequestInterceptionAsync).ConfigureAwait(false);
        }

        internal Task SetExtraHTTPHeadersAsync(Dictionary<string, string> extraHTTPHeaders)
        {
            _extraHTTPHeaders = [];

            foreach (var item in extraHTTPHeaders)
            {
                _extraHTTPHeaders[item.Key.ToLower(CultureInfo.CurrentCulture)] = item.Value;
            }

            return _client.SendAsync("Network.setExtraHTTPHeaders", new NetworkSetExtraHTTPHeadersRequest
            {
                Headers = _extraHTTPHeaders,
            });
        }

        internal async Task SetOfflineModeAsync(bool value)
        {
            _emulatedNetworkConditions.Offline = value;
            await UpdateNetworkConditionsAsync().ConfigureAwait(false);
        }

        internal async Task EmulateNetworkConditionsAsync(NetworkConditions networkConditions)
        {
            _emulatedNetworkConditions.Upload = networkConditions?.Upload ?? -1;
            _emulatedNetworkConditions.Download = networkConditions?.Download ?? -1;
            _emulatedNetworkConditions.Latency = networkConditions?.Latency ?? 0;
            await UpdateNetworkConditionsAsync().ConfigureAwait(false);
        }

        private Task UpdateNetworkConditionsAsync()
            => _client.SendAsync("Network.emulateNetworkConditions", new NetworkEmulateNetworkConditionsRequest
            {
                Offline = _emulatedNetworkConditions.Offline,
                Latency = _emulatedNetworkConditions.Latency,
                UploadThroughput = _emulatedNetworkConditions.Upload,
                DownloadThroughput = _emulatedNetworkConditions.Download,
            });

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                switch (e.MessageID)
                {
                    case "Fetch.requestPaused":
                        await OnRequestPausedAsync(e.MessageData.ToObject<FetchRequestPausedResponse>(true)).ConfigureAwait(false);
                        break;
                    case "Fetch.authRequired":
                        await OnAuthRequiredAsync(e.MessageData.ToObject<FetchAuthRequiredResponse>(true)).ConfigureAwait(false);
                        break;
                    case "Network.requestWillBeSent":
                        await OnRequestWillBeSentAsync(e.MessageData.ToObject<RequestWillBeSentPayload>(true)).ConfigureAwait(false);
                        break;
                    case "Network.requestServedFromCache":
                        OnRequestServedFromCache(e.MessageData.ToObject<RequestServedFromCacheResponse>(true));
                        break;
                    case "Network.responseReceived":
                        OnResponseReceived(e.MessageData.ToObject<ResponseReceivedResponse>(true));
                        break;
                    case "Network.loadingFinished":
                        OnLoadingFinished(e.MessageData.ToObject<LoadingFinishedEventResponse>(true));
                        break;
                    case "Network.loadingFailed":
                        OnLoadingFailed(e.MessageData.ToObject<LoadingFailedEventResponse>(true));
                        break;
                    case "Network.responseReceivedExtraInfo":
                        await OnResponseReceivedExtraInfoAsync(e.MessageData.ToObject<ResponseReceivedExtraInfoResponse>(true)).ConfigureAwait(false);
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

        private async Task OnResponseReceivedExtraInfoAsync(ResponseReceivedExtraInfoResponse e)
        {
            var redirectInfo = _networkEventManager.TakeQueuedRedirectInfo(e.RequestId);

            if (redirectInfo != null)
            {
                _networkEventManager.ResponseExtraInfo(e.RequestId).Add(e);
                await OnRequestAsync(redirectInfo.Event, redirectInfo.FetchRequestId).ConfigureAwait(false);
                return;
            }

            // We may have skipped response and loading events because we didn't have
            // this ExtraInfo event yet. If so, emit those events now.
            var queuedEvents = _networkEventManager.GetQueuedEventGroup(e.RequestId);

            if (queuedEvents != null)
            {
                EmitResponseEvent(queuedEvents.ResponseReceivedEvent, e);

                if (queuedEvents.LoadingFinishedEvent != null)
                {
                    OnLoadingFinished(queuedEvents.LoadingFinishedEvent);
                }

                if (queuedEvents.LoadingFailedEvent != null)
                {
                    OnLoadingFailed(queuedEvents.LoadingFailedEvent);
                }

                // We need this in .NET to avoid race conditions
                _networkEventManager.ForgetQueuedEventGroup(e.RequestId);
                return;
            }

            // Wait until we get another event that can use this ExtraInfo event.
            _networkEventManager.ResponseExtraInfo(e.RequestId).Add(e);
        }

        private void OnLoadingFailed(LoadingFailedEventResponse e)
        {
            var queuedEvents = _networkEventManager.GetQueuedEventGroup(e.RequestId);

            if (queuedEvents != null)
            {
                queuedEvents.LoadingFailedEvent = e;
            }
            else
            {
                EmitLoadingFailed(e);
            }
        }

        private void EmitLoadingFailed(LoadingFailedEventResponse e)
        {
            var request = _networkEventManager.GetRequest(e.RequestId);
            if (request == null)
            {
                return;
            }

            request.Failure = e.ErrorText;
            request.Response?.BodyLoadedTaskWrapper.TrySetResult(true);

            ForgetRequest(request, true);

            RequestFailed?.Invoke(this, new RequestEventArgs(request));
        }

        private void OnLoadingFinished(LoadingFinishedEventResponse e)
        {
            var queuedEvents = _networkEventManager.GetQueuedEventGroup(e.RequestId);

            if (queuedEvents != null)
            {
                queuedEvents.LoadingFinishedEvent = e;
            }
            else
            {
                EmitLoadingFinished(e);
            }
        }

        private void EmitLoadingFinished(LoadingFinishedEventResponse e)
        {
            var request = _networkEventManager.GetRequest(e.RequestId);
            if (request == null)
            {
                return;
            }

            request.Response?.BodyLoadedTaskWrapper.TrySetResult(true);

            ForgetRequest(request, true);

            RequestFinished?.Invoke(this, new RequestEventArgs(request));
        }

        private void ForgetRequest(Request request, bool events)
        {
            _networkEventManager.ForgetRequest(request.RequestId);

            if (request.InterceptionId != null)
            {
                _attemptedAuthentications.Remove(request.InterceptionId);
            }

            if (events)
            {
                _networkEventManager.Forget(request.RequestId);
            }
        }

        private void OnResponseReceived(ResponseReceivedResponse e)
        {
            var request = _networkEventManager.GetRequest(e.RequestId);
            ResponseReceivedExtraInfoResponse extraInfo = null;

            if (request is { FromMemoryCache: false } && e.HasExtraInfo)
            {
                extraInfo = _networkEventManager.ShiftResponseExtraInfo(e.RequestId);

                if (extraInfo == null)
                {
                    _networkEventManager.QueuedEventGroup(e.RequestId, new()
                    {
                        ResponseReceivedEvent = e,
                    });
                    return;
                }
            }

            EmitResponseEvent(e, extraInfo);
        }

        private void EmitResponseEvent(ResponseReceivedResponse e, ResponseReceivedExtraInfoResponse extraInfo)
        {
            var request = _networkEventManager.GetRequest(e.RequestId);

            // FileUpload sends a response without a matching request.
            if (request == null)
            {
                return;
            }

            var response = new Response(
                _client,
                request,
                e.Response,
                extraInfo);

            request.Response = response;

            Response?.Invoke(this, new ResponseCreatedEventArgs(response));
        }

        private async Task OnAuthRequiredAsync(CDPSession client, FetchAuthRequiredResponse e)
        {
            var response = "Default";
            if (_attemptedAuthentications.Contains(e.RequestId))
            {
                response = "CancelAuth";
            }
            else if (_credentials != null)
            {
                response = "ProvideCredentials";
                _attemptedAuthentications.Add(e.RequestId);
            }

            var credentials = _credentials ?? new Credentials();
            try
            {
                await client.SendAsync("Fetch.continueWithAuth", new ContinueWithAuthRequest
                {
                    RequestId = e.RequestId,
                    AuthChallengeResponse = new ContinueWithAuthRequestChallengeResponse
                    {
                        Response = response,
                        Username = credentials.Username,
                        Password = credentials.Password,
                    },
                }).ConfigureAwait(false);
            }
            catch (PuppeteerException ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task OnRequestPausedAsync(CDPSession client, FetchRequestPausedResponse e)
        {
            if (!_userRequestInterceptionEnabled && _protocolRequestInterceptionEnabled)
            {
                try
                {
                    await client.SendAsync("Fetch.continueRequest", new FetchContinueRequestRequest
                    {
                        RequestId = e.RequestId,
                    }).ConfigureAwait(false);
                }
                catch (PuppeteerException ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }

            if (string.IsNullOrEmpty(e.NetworkId))
            {
                OnRequestWithoutNetworkInstrumentation(client, e);
                return;
            }

            var requestWillBeSentEvent =
              _networkEventManager.GetRequestWillBeSent(e.NetworkId);

            // redirect requests have the same `requestId`,
            if (
                requestWillBeSentEvent != null &&
                (requestWillBeSentEvent.Request.Url != e.Request.Url ||
                requestWillBeSentEvent.Request.Method != e.Request.Method))
            {
                _networkEventManager.ForgetRequestWillBeSent(e.NetworkId);
                requestWillBeSentEvent = null;
            }

            if (requestWillBeSentEvent != null)
            {
                PatchRequestEventHeaders(requestWillBeSentEvent, e);
                await OnRequestAsync(requestWillBeSentEvent, e.RequestId).ConfigureAwait(false);
            }
            else
            {
                _networkEventManager.StoreRequestPaused(e.NetworkId, e);
            }
        }

        private void OnRequestWithoutNetworkInstrumentation(CDPSession client, FetchRequestPausedResponse e)
        {
            // If an event has no networkId it should not have any network events. We
            // still want to dispatch it for the interception by the user.
            var frame = !string.IsNullOrEmpty(e.FrameId)
                ? _frameManager.GetFrame(e.FrameId)
                : null;

            var request = new Request(
                    client,
                    frame,
                    e.RequestId,
                    _userRequestInterceptionEnabled,
                    e,
                    []);
            Request?.Invoke(this, new RequestEventArgs(request));
            void request.Fina();
        }

        private async Task OnRequestAsync(RequestWillBeSentPayload e, string fetchRequestId)
        {
            Request request;
            var redirectChain = new List<IRequest>();
            if (e.RedirectResponse != null)
            {
                ResponseReceivedExtraInfoResponse redirectResponseExtraInfo = null;
                if (e.RedirectHasExtraInfo)
                {
                    redirectResponseExtraInfo = _networkEventManager.ShiftResponseExtraInfo(e.RequestId);
                    if (redirectResponseExtraInfo == null)
                    {
                        _networkEventManager.QueueRedirectInfo(e.RequestId, new()
                        {
                            Event = e,
                            FetchRequestId = fetchRequestId,
                        });
                        return;
                    }
                }

                request = _networkEventManager.GetRequest(e.RequestId);

                // If we connect late to the target, we could have missed the requestWillBeSent event.
                if (request != null)
                {
                    HandleRequestRedirect(request, e.RedirectResponse, redirectResponseExtraInfo);
                    redirectChain = request.RedirectChainList;
                }
            }

            var frame = !string.IsNullOrEmpty(e.FrameId) ? await _frameManager.FrameTree.TryGetFrameAsync(e.FrameId).ConfigureAwait(false) : null;

            request = new Request(
                _client,
                frame,
                fetchRequestId,
                _userRequestInterceptionEnabled,
                e,
                redirectChain);

            _networkEventManager.StoreRequest(e.RequestId, request);

            Request?.Invoke(this, new RequestEventArgs(request));

            try
            {
                await request.FinalizeInterceptionsAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to FinalizeInterceptionsAsync");
            }
        }

        private void OnRequestServedFromCache(RequestServedFromCacheResponse response)
        {
            var request = _networkEventManager.GetRequest(response.RequestId);

            if (request != null)
            {
                request.FromMemoryCache = true;
            }

            RequestServedFromCache?.Invoke(this, new RequestEventArgs(request));
        }

        private void HandleRequestRedirect(Request request, ResponsePayload responseMessage, ResponseReceivedExtraInfoResponse extraInfo)
        {
            var response = new Response(
                _client,
                request,
                responseMessage,
                extraInfo);

            request.Response = response;
            request.RedirectChainList.Add(request);
            response.BodyLoadedTaskWrapper.TrySetException(
                new PuppeteerException("Response body is unavailable for redirect responses"));

            ForgetRequest(request, false);

            Response?.Invoke(this, new ResponseCreatedEventArgs(response));
            RequestFinished?.Invoke(this, new RequestEventArgs(request));
        }

        private async Task OnRequestWillBeSentAsync(RequestWillBeSentPayload e)
        {
            // Request interception doesn't happen for data URLs with Network Service.
            if (_userRequestInterceptionEnabled && !e.Request.Url.StartsWith("data:", StringComparison.InvariantCultureIgnoreCase))
            {
                _networkEventManager.StoreRequestWillBeSent(e.RequestId, e);

                var requestPausedEvent = _networkEventManager.GetRequestPaused(e.RequestId);
                if (requestPausedEvent != null)
                {
                    var fetchRequestId = requestPausedEvent.RequestId;
                    PatchRequestEventHeaders(e, requestPausedEvent);
                    await OnRequestAsync(e, fetchRequestId).ConfigureAwait(false);
                    _networkEventManager.ForgetRequestPaused(e.RequestId);
                }

                return;
            }

            await OnRequestAsync(e, null).ConfigureAwait(false);
        }

        private void PatchRequestEventHeaders(RequestWillBeSentPayload requestWillBeSentEvent, FetchRequestPausedResponse requestPausedEvent)
        {
            foreach (var kv in requestPausedEvent.Request.Headers)
            {
                requestWillBeSentEvent.Request.Headers[kv.Key] = kv.Value;
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
            if (enabled)
            {
                await Task.WhenAll(
                    UpdateProtocolCacheDisabledAsync(),
                    _client.SendAsync("Fetch.enable", new FetchEnableRequest
                    {
                        HandleAuthRequests = true,
                        Patterns = new[] { new FetchEnableRequest.Pattern { UrlPattern = "*", } },
                    })).ConfigureAwait(false);
            }
            else
            {
                await Task.WhenAll(
                    UpdateProtocolCacheDisabledAsync(),
                    _client.SendAsync("Fetch.disable")).ConfigureAwait(false);
            }
        }

        private async Task ApplyUserAgentAsync(CDPSession client)
        {
            if (_userAgent == null)
            {
                return;
            }

            await client.SendAsync(
                "Network.setUserAgentOverride",
                new NetworkSetUserAgentOverrideRequest
                {
                    UserAgent = _userAgent,
                    UserAgentMetadata = _userAgentMetadata,
                }).ConfigureAwait(false);
        }

        private async Task ApplyProtocolRequestInterceptionAsync(CDPSession client)
        {
            _userCacheDisabled ??= false;

            if (_protocolRequestInterceptionEnabled)
            {
                await Task.WhenAll(
                    ApplyProtocolCacheDisabledAsync(client),
                    client.SendAsync(
                        "Fetch.enable",
                        new FetchEnableRequest
                        {
                            HandleAuthRequests = true,
                            Patterns = [new FetchEnableRequest.Pattern("*")],
                        })).ConfigureAwait(false);
            }
            else
            {
                await Task.WhenAll(
                    ApplyProtocolCacheDisabledAsync(client),
                    client.SendAsync("Fetch.disable")).ConfigureAwait(false);
            }
        }

        private async Task ApplyProtocolCacheDisabledAsync(CDPSession client)
        {
            if (_userCacheDisabled == null)
            {
                return;
            }

            await client.SendAsync(
                "Network.setCacheDisabled",
                new NetworkSetCacheDisabledRequest(_userCacheDisabled.Value)).ConfigureAwait(false);
        }

        private async Task ApplyNetworkConditionsAsync(CDPSession client)
        {
            if (_emulatedNetworkConditions == null)
            {
                return;
            }

            await client.SendAsync(
                "Network.emulateNetworkConditions",
                new NetworkEmulateNetworkConditionsRequest
                {
                    Offline = _emulatedNetworkConditions.Offline,
                    Latency = _emulatedNetworkConditions.Latency,
                    UploadThroughput = _emulatedNetworkConditions.Upload,
                    DownloadThroughput = _emulatedNetworkConditions.Download,
                }).ConfigureAwait(false);
        }

        private async Task ApplyExtraHTTPHeadersAsync(CDPSession client)
        {
            if (_extraHTTPHeaders == null)
            {
                return;
            }

            await client.SendAsync(
                "Network.setExtraHTTPHeaders",
                new NetworkSetExtraHTTPHeadersRequest(_extraHTTPHeaders)).ConfigureAwait(false);
        }

        private Task ApplyToAllClientsAsync(Func<CDPSession, Task> func)
            => Task.WhenAll(_clients.Keys.Select(func));
    }
}
