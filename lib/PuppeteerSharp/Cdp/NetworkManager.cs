using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Cdp
{
    internal class NetworkManager
    {
        private readonly bool _ignoreHTTPSErrors;
        private readonly NetworkEventManager _networkEventManager = new();
        private readonly ILogger _logger;
        private readonly ConcurrentSet<string> _attemptedAuthentications = [];
        private readonly ConcurrentDictionary<ICDPSession, DisposableActionsStack> _clients = new();
        private readonly IFrameProvider _frameManager;
        private readonly ILoggerFactory _loggerFactory;

        private InternalNetworkConditions _emulatedNetworkConditions;
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
        /// <param name="loggerFactory">Logger factory.</param>
        internal NetworkManager(bool ignoreHTTPSErrors, IFrameProvider frameManager, ILoggerFactory loggerFactory)
        {
            _frameManager = frameManager;
            _ignoreHTTPSErrors = ignoreHTTPSErrors;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<NetworkManager>();
        }

        internal event EventHandler<ResponseCreatedEventArgs> Response;

        internal event EventHandler<RequestEventArgs> Request;

        internal event EventHandler<RequestEventArgs> RequestFinished;

        internal event EventHandler<RequestEventArgs> RequestFailed;

        internal event EventHandler<RequestEventArgs> RequestServedFromCache;

        internal Dictionary<string, string> ExtraHTTPHeaders => _extraHTTPHeaders?.Clone();

        internal int NumRequestsInProgress => _networkEventManager.NumRequestsInProgress;

        internal Task AddClientAsync(ICDPSession client)
        {
            if (_clients.ContainsKey(client))
            {
                return Task.CompletedTask;
            }

            var subscriptions = new DisposableActionsStack();
            _clients[client] = subscriptions;
            client.MessageReceived += Client_MessageReceived;
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

        internal Task SetExtraHTTPHeadersAsync(Dictionary<string, string> extraHTTPHeaders)
        {
            _extraHTTPHeaders = [];

            foreach (var item in extraHTTPHeaders)
            {
                _extraHTTPHeaders[item.Key.ToLower(CultureInfo.CurrentCulture)] = item.Value;
            }

            return ApplyToAllClientsAsync(ApplyExtraHTTPHeadersAsync);
        }

        internal Task SetUserAgentAsync(string userAgent, UserAgentMetadata userAgentMetadata)
        {
            _userAgent = userAgent;
            _userAgentMetadata = userAgentMetadata;
            return ApplyToAllClientsAsync(ApplyUserAgentAsync);
        }

        internal Task SetCacheEnabledAsync(bool enabled)
        {
            _userCacheDisabled = !enabled;
            return ApplyToAllClientsAsync(ApplyProtocolCacheDisabledAsync);
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

        internal Task SetOfflineModeAsync(bool value)
        {
            _emulatedNetworkConditions ??= new InternalNetworkConditions();
            _emulatedNetworkConditions.Offline = value;
            return ApplyToAllClientsAsync(ApplyNetworkConditionsAsync);
        }

        internal Task EmulateNetworkConditionsAsync(NetworkConditions networkConditions)
        {
            _emulatedNetworkConditions ??= new InternalNetworkConditions();
            _emulatedNetworkConditions.Upload = networkConditions?.Upload ?? -1;
            _emulatedNetworkConditions.Download = networkConditions?.Download ?? -1;
            _emulatedNetworkConditions.Latency = networkConditions?.Latency ?? 0;
            return ApplyToAllClientsAsync(ApplyNetworkConditionsAsync);
        }

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                var client = sender as CDPSession;
                switch (e.MessageID)
                {
                    case "Fetch.requestPaused":
                        await OnRequestPausedAsync(client, e.MessageData.ToObject<FetchRequestPausedResponse>(true)).ConfigureAwait(false);
                        break;
                    case "Fetch.authRequired":
                        await OnAuthRequiredAsync(client, e.MessageData.ToObject<FetchAuthRequiredResponse>(true)).ConfigureAwait(false);
                        break;
                    case "Network.requestWillBeSent":
                        await OnRequestWillBeSentAsync(client, e.MessageData.ToObject<RequestWillBeSentPayload>(true)).ConfigureAwait(false);
                        break;
                    case "Network.requestServedFromCache":
                        OnRequestServedFromCache(e.MessageData.ToObject<RequestServedFromCacheResponse>(true));
                        break;
                    case "Network.responseReceived":
                        OnResponseReceived(client, e.MessageData.ToObject<ResponseReceivedResponse>(true));
                        break;
                    case "Network.loadingFinished":
                        OnLoadingFinished(e.MessageData.ToObject<LoadingFinishedEventResponse>(true));
                        break;
                    case "Network.loadingFailed":
                        OnLoadingFailed(e.MessageData.ToObject<LoadingFailedEventResponse>(true));
                        break;
                    case "Network.responseReceivedExtraInfo":
                        await OnResponseReceivedExtraInfoAsync(sender as CDPSession, e.MessageData.ToObject<ResponseReceivedExtraInfoResponse>(true)).ConfigureAwait(false);
                        break;
                }
            }
            catch (Exception ex)
            {
                var message = $"NetworkManager failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
                _logger.LogError(ex, message);
                _ = ApplyToAllClientsAsync(client =>
                {
                    (client as CDPSession)?.Close(message);
                    return Task.CompletedTask;
                });
            }
        }

        private async Task OnResponseReceivedExtraInfoAsync(CDPSession client, ResponseReceivedExtraInfoResponse e)
        {
            var redirectInfo = _networkEventManager.TakeQueuedRedirectInfo(e.RequestId);

            if (redirectInfo != null)
            {
                _networkEventManager.ResponseExtraInfo(e.RequestId).Add(e);
                await OnRequestAsync(client, redirectInfo.Event, redirectInfo.FetchRequestId).ConfigureAwait(false);
                return;
            }

            // We may have skipped response and loading events because we didn't have
            // this ExtraInfo event yet. If so, emit those events now.
            var queuedEvents = _networkEventManager.GetQueuedEventGroup(e.RequestId);

            if (queuedEvents != null)
            {
                _networkEventManager.ForgetQueuedEventGroup(e.RequestId);
                EmitResponseEvent(client, queuedEvents.ResponseReceivedEvent, e);

                if (queuedEvents.LoadingFinishedEvent != null)
                {
                    EmitLoadingFinished(queuedEvents.LoadingFinishedEvent);
                }

                if (queuedEvents.LoadingFailedEvent != null)
                {
                    EmitLoadingFailed(queuedEvents.LoadingFailedEvent);
                }

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

            request.FailureText = e.ErrorText;
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

        private void ForgetRequest(CdpHttpRequest request, bool events)
        {
            _networkEventManager.ForgetRequest(request.Id);

            if (request.InterceptionId != null)
            {
                _attemptedAuthentications.Remove(request.InterceptionId);
            }

            if (events)
            {
                _networkEventManager.Forget(request.Id);
            }
        }

        private void OnResponseReceived(CDPSession client, ResponseReceivedResponse e)
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

            EmitResponseEvent(client, e, extraInfo);
        }

        private void EmitResponseEvent(CDPSession client, ResponseReceivedResponse e, ResponseReceivedExtraInfoResponse extraInfo)
        {
            var request = _networkEventManager.GetRequest(e.RequestId);

            // FileUpload sends a response without a matching request.
            if (request == null)
            {
                return;
            }

            if (e.Response.FromDiskCache)
            {
                extraInfo = null;
            }

            var response = new CdpHttpResponse(
                client,
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
                OnRequestWithoutNetworkInstrumentationAsync(client, e);
                return;
            }

            var requestWillBeSentEvent = _networkEventManager.GetRequestWillBeSent(e.NetworkId);

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
                await OnRequestAsync(client, requestWillBeSentEvent, e.RequestId).ConfigureAwait(false);
            }
            else
            {
                _networkEventManager.StoreRequestPaused(e.NetworkId, e);
            }
        }

        private async void OnRequestWithoutNetworkInstrumentationAsync(CDPSession client, FetchRequestPausedResponse e)
        {
            // If an event has no networkId it should not have any network events. We
            // still want to dispatch it for the interception by the user.
            var frame = !string.IsNullOrEmpty(e.FrameId)
                ? await _frameManager.GetFrameAsync(e.FrameId).ConfigureAwait(false)
                : null;

            var request = new CdpHttpRequest(
                    client,
                    frame,
                    e.RequestId,
                    _userRequestInterceptionEnabled,
                    e,
                    [],
                    _loggerFactory);

            Request?.Invoke(this, new RequestEventArgs(request));
            _ = request.FinalizeInterceptionsAsync();
        }

        private async Task OnRequestAsync(CDPSession client, RequestWillBeSentPayload e, string fetchRequestId)
        {
            CdpHttpRequest request;
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
                    HandleRequestRedirect(client, request, e.RedirectResponse, redirectResponseExtraInfo);
                    redirectChain = request.RedirectChainList;
                }
            }

            var frame = !string.IsNullOrEmpty(e.FrameId) ? await _frameManager.GetFrameAsync(e.FrameId).ConfigureAwait(false) : null;

            request = new CdpHttpRequest(
                client,
                frame,
                fetchRequestId,
                _userRequestInterceptionEnabled,
                e,
                redirectChain,
                _loggerFactory);

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

        private void HandleRequestRedirect(CDPSession client, CdpHttpRequest request, ResponsePayload responseMessage, ResponseReceivedExtraInfoResponse extraInfo)
        {
            var response = new CdpHttpResponse(
                client,
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

        private async Task OnRequestWillBeSentAsync(CDPSession client, RequestWillBeSentPayload e)
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
                    await OnRequestAsync(client, e, fetchRequestId).ConfigureAwait(false);
                    _networkEventManager.ForgetRequestPaused(e.RequestId);
                }

                return;
            }

            await OnRequestAsync(client, e, null).ConfigureAwait(false);
        }

        private void PatchRequestEventHeaders(RequestWillBeSentPayload requestWillBeSentEvent, FetchRequestPausedResponse requestPausedEvent)
        {
            foreach (var kv in requestPausedEvent.Request.Headers)
            {
                requestWillBeSentEvent.Request.Headers[kv.Key] = kv.Value;
            }
        }

        private async Task ApplyUserAgentAsync(ICDPSession client)
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

        private async Task ApplyProtocolRequestInterceptionAsync(ICDPSession client)
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

        private async Task ApplyProtocolCacheDisabledAsync(ICDPSession client)
        {
            if (_userCacheDisabled == null)
            {
                return;
            }

            await client.SendAsync(
                "Network.setCacheDisabled",
                new NetworkSetCacheDisabledRequest(_userCacheDisabled.Value)).ConfigureAwait(false);
        }

        private async Task ApplyNetworkConditionsAsync(ICDPSession client)
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

        private async Task ApplyExtraHTTPHeadersAsync(ICDPSession client)
        {
            if (_extraHTTPHeaders == null)
            {
                return;
            }

            await client.SendAsync(
                "Network.setExtraHTTPHeaders",
                new NetworkSetExtraHTTPHeadersRequest(_extraHTTPHeaders)).ConfigureAwait(false);
        }

        private Task ApplyToAllClientsAsync(Func<ICDPSession, Task> func)
            => Task.WhenAll(_clients.Keys.Select(func));
    }
}
