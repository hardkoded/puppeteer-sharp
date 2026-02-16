// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.

#if !CDP_ONLY

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;
using PuppeteerSharp.Helpers;
using WebDriverBiDi.Network;

namespace PuppeteerSharp.Bidi;

/// <inheritdoc />
public class BidiHttpRequest : Request<BidiHttpResponse>
{
    private readonly Request _request;
    private readonly BidiRequestInterception _interception = new();
    private readonly List<Func<IRequest, Task>> _interceptHandlers = [];
    private Payload _continueRequestOverrides = new();
    private ResponseData _responseForRequest;
    private RequestAbortErrorCode _abortErrorReason;
    private InterceptResolutionState _interceptResolutionState = new(InterceptResolutionAction.None);
    private bool _isInterceptResolutionHandled;
    private bool _authenticationHandled;

    private BidiHttpRequest(Request request, BidiFrame frame, BidiHttpRequest redirect)
    {
        _request = request;
        Frame = frame;
        RedirectChainList = redirect?.RedirectChainList ?? [];
        Requests.AddItem(request, this);
    }

    /// <summary>
    /// Gets the merged headers including extra HTTP headers and user agent headers.
    /// </summary>
    public override Dictionary<string, string> Headers
    {
        get
        {
            // Callers should not be allowed to mutate internal structure.
            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in _request.Headers)
            {
                headers[header.Name.ToLowerInvariant()] = header.Value.Value;
            }

            foreach (var kvp in ExtraHttpHeaders)
            {
                headers[kvp.Key.ToLowerInvariant()] = kvp.Value;
            }

            foreach (var kvp in UserAgentHeaders)
            {
                headers[kvp.Key.ToLowerInvariant()] = kvp.Value;
            }

            return headers;
        }
    }

    // TODO: I don't like having this static field at all. This will cause memory leaks for sure.
    // We need to move this to a place where we can control its lifecycle.
    internal static AsyncDictionaryHelper<Request, BidiHttpRequest> Requests { get; } = new("Request {0} not found");

    internal override Payload ContinueRequestOverrides => _continueRequestOverrides;

    internal override ResponseData ResponseForRequest => _responseForRequest;

    internal override RequestAbortErrorCode AbortErrorReason => _abortErrorReason;

    internal BidiPage BidiPage => (BidiPage)Frame.Page;

    internal ConcurrentDictionary<string, string> ExtraHttpHeaders => BidiPage.ExtraHttpHeaders;

    internal ConcurrentDictionary<string, string> UserAgentHeaders => BidiPage.UserAgentHeaders;

    internal bool HasInternalHeaderOverwrite => ExtraHttpHeaders.Values.Count != 0 || UserAgentHeaders.Values.Count != 0;

    /// <summary>
    /// Gets whether network interception is enabled at the page level.
    /// This is used to verify that the user has called SetRequestInterceptionAsync(true).
    /// </summary>
    private bool PageLevelInterceptionEnabled => BidiPage.IsNetworkInterceptionEnabled;

    /// <summary>
    /// Gets whether this specific request can be intercepted (i.e., is blocked by the browser).
    /// </summary>
    private bool CanBeIntercepted => _request.IsBlocked;

    private InterceptResolutionState InterceptResolutionState
    {
        get
        {
            if (!CanBeIntercepted)
            {
                return new InterceptResolutionState(InterceptResolutionAction.Disabled);
            }

            return _isInterceptResolutionHandled
                ? new InterceptResolutionState(InterceptResolutionAction.AlreadyHandled)
                : _interceptResolutionState;
        }
    }

    /// <inheritdoc />
    public override async Task ContinueAsync(Payload payloadOverrides = null, int? priority = null)
    {
        // First verify that page-level interception is enabled
        if (!PageLevelInterceptionEnabled)
        {
            throw new PuppeteerException("Request Interception is not enabled!");
        }

        if (_isInterceptResolutionHandled)
        {
            throw new PuppeteerException("Request is already handled!");
        }

        // If this specific request is not blocked, return early (don't throw)
        if (!CanBeIntercepted)
        {
            return;
        }

        if (priority is null)
        {
            await ContinueInternalAsync(payloadOverrides).ConfigureAwait(false);
            return;
        }

        _continueRequestOverrides = payloadOverrides;

        if (_interceptResolutionState.Priority is null || priority > _interceptResolutionState.Priority)
        {
            _interceptResolutionState = new InterceptResolutionState(InterceptResolutionAction.Continue, priority);
            return;
        }

        if (priority == _interceptResolutionState.Priority)
        {
            if (_interceptResolutionState.Action == InterceptResolutionAction.Abort ||
                _interceptResolutionState.Action == InterceptResolutionAction.Respond)
            {
                return;
            }

            _interceptResolutionState.Action = InterceptResolutionAction.Continue;
        }
    }

    /// <inheritdoc />
    public override async Task RespondAsync(ResponseData response, int? priority = null)
    {
        if (response == null)
        {
            throw new ArgumentNullException(nameof(response));
        }

        // First verify that page-level interception is enabled
        if (!PageLevelInterceptionEnabled)
        {
            throw new PuppeteerException("Request Interception is not enabled!");
        }

        if (_isInterceptResolutionHandled)
        {
            throw new PuppeteerException("Request is already handled!");
        }

        // If this specific request is not blocked, return early (don't throw)
        if (!CanBeIntercepted)
        {
            return;
        }

        if (priority is null)
        {
            await RespondInternalAsync(response).ConfigureAwait(false);
            return;
        }

        _responseForRequest = response;

        if (_interceptResolutionState.Priority is null || priority > _interceptResolutionState.Priority)
        {
            _interceptResolutionState = new InterceptResolutionState(InterceptResolutionAction.Respond, priority);
            return;
        }

        if (priority == _interceptResolutionState.Priority)
        {
            if (_interceptResolutionState.Action == InterceptResolutionAction.Abort)
            {
                return;
            }

            _interceptResolutionState.Action = InterceptResolutionAction.Respond;
        }
    }

    /// <inheritdoc />
    public override async Task AbortAsync(RequestAbortErrorCode errorCode = RequestAbortErrorCode.Failed, int? priority = null)
    {
        // First verify that page-level interception is enabled
        if (!PageLevelInterceptionEnabled)
        {
            throw new PuppeteerException("Request Interception is not enabled!");
        }

        if (_isInterceptResolutionHandled)
        {
            throw new PuppeteerException("Request is already handled!");
        }

        // If this specific request is not blocked, return early (don't throw)
        if (!CanBeIntercepted)
        {
            return;
        }

        if (priority is null)
        {
            await AbortInternalAsync().ConfigureAwait(false);
            return;
        }

        _abortErrorReason = errorCode;

        if (_interceptResolutionState.Priority is null || priority >= _interceptResolutionState.Priority)
        {
            _interceptResolutionState = new InterceptResolutionState(InterceptResolutionAction.Abort, priority);
        }
    }

    /// <inheritdoc />
    public override Task<string> FetchPostDataAsync() => _request.FetchPostDataAsync();

    internal static BidiHttpRequest From(Request bidiRequest, BidiFrame frame, BidiHttpRequest redirect = null)
    {
        var isNavigationRequest = bidiRequest.Navigation != null;
        var request = new BidiHttpRequest(bidiRequest, frame, redirect)
        {
            Url = bidiRequest.Url,
            Method = new System.Net.Http.HttpMethod(bidiRequest.Method),
            IsNavigationRequest = isNavigationRequest,
            HasPostData = bidiRequest.HasPostData,
            ResourceType = MapDestinationToResourceType(bidiRequest.Destination, isNavigationRequest),
        };
        request.Initialize();
        return request;
    }

    internal Task<byte[]> GetResponseContentAsync() => _request.GetResponseContentAsync();

    internal override async Task FinalizeInterceptionsAsync()
    {
        // First, run any handlers registered via the handler list
        foreach (var handler in _interception.Handlers)
        {
            await handler().ConfigureAwait(false);
        }

        _interception.Handlers.Clear();

        // Then run any handlers registered via EnqueueInterceptionAction
        foreach (var handler in _interceptHandlers)
        {
            await handler(this).ConfigureAwait(false);
        }

        _interceptHandlers.Clear();

        // Finally, handle the intercept resolution state
        switch (InterceptResolutionState.Action)
        {
            case InterceptResolutionAction.Abort:
                await AbortInternalAsync().ConfigureAwait(false);
                return;
            case InterceptResolutionAction.Respond:
                if (_responseForRequest is null)
                {
                    throw new PuppeteerException("Response is missing for the interception");
                }

                await RespondInternalAsync(_responseForRequest).ConfigureAwait(false);
                return;
            case InterceptResolutionAction.Continue:
                await ContinueInternalAsync(_continueRequestOverrides).ConfigureAwait(false);
                break;
            case InterceptResolutionAction.None:
                // When no explicit action was set but the request is blocked,
                // we need to continue it to allow the browser to proceed.
                // This happens when interception is enabled but no handler called
                // ContinueAsync, AbortAsync, or RespondAsync.
                if (CanBeIntercepted)
                {
                    await ContinueInternalAsync().ConfigureAwait(false);
                }

                break;
        }
    }

    internal override void EnqueueInterceptionActionCore(Func<IRequest, Task> pendingHandler)
        => _interceptHandlers.Add(pendingHandler);

    private static List<Header> ConvertToBidiHeaders(Dictionary<string, string> headers)
    {
        if (headers == null || headers.Count == 0)
        {
            return null;
        }

        var bidiHeaders = new List<Header>();
        foreach (var kvp in headers)
        {
            bidiHeaders.Add(new Header(kvp.Key.ToLowerInvariant(), kvp.Value));
        }

        return bidiHeaders;
    }

    private static string GetReasonPhrase(int statusCode)
        => HttpStatusTextHelper.GetStatusText(statusCode);

    private static ResourceType MapDestinationToResourceType(string destination, bool isNavigationRequest)
    {
        // Navigation requests are always Document type
        if (isNavigationRequest)
        {
            return ResourceType.Document;
        }

        // Map BiDi destination to ResourceType based on the Fetch spec
        // https://fetch.spec.whatwg.org/#concept-request-destination
        return destination switch
        {
            "document" or "frame" or "iframe" => ResourceType.Document,
            "style" => ResourceType.StyleSheet,
            "script" => ResourceType.Script,
            "image" => ResourceType.Image,
            "font" => ResourceType.Font,
            "audio" or "video" => ResourceType.Media,
            "track" => ResourceType.TextTrack,
            "manifest" => ResourceType.Manifest,
            "worker" or "sharedworker" or "serviceworker" => ResourceType.Other,
            "xslt" => ResourceType.Other,
            "json" => ResourceType.Fetch,
            "report" => ResourceType.Ping,
            "" => ResourceType.Other, // Empty string is for generic requests
            _ => ResourceType.Other,
        };
    }

    private Dictionary<string, string> GetMergedHeaders(Dictionary<string, string> overrideHeaders)
    {
        // Start with the merged headers (original + extra + user agent)
        var headers = new Dictionary<string, string>(Headers, StringComparer.OrdinalIgnoreCase);

        // Apply any override headers from the payload
        if (overrideHeaders != null)
        {
            foreach (var kvp in overrideHeaders)
            {
                // Handle null values by removing the header
                if (kvp.Value == null)
                {
                    headers.Remove(kvp.Key.ToLowerInvariant());
                }
                else
                {
                    headers[kvp.Key.ToLowerInvariant()] = kvp.Value;
                }
            }
        }

        return headers;
    }

    private async Task ContinueInternalAsync(Payload overrides = null)
    {
        _isInterceptResolutionHandled = true;

        try
        {
            // Merge the original request headers with extra headers and user agent headers
            var mergedHeaders = HasInternalHeaderOverwrite || overrides?.Headers != null
                ? GetMergedHeaders(overrides?.Headers)
                : null;

            var bidiHeaders = ConvertToBidiHeaders(mergedHeaders);

            BytesValue body = null;
            if (overrides?.PostData != null)
            {
                body = BytesValue.FromBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes(overrides.PostData)));
            }

            await _request.ContinueRequestAsync(
                url: overrides?.Url,
                method: overrides?.Method?.ToString(),
                headers: bidiHeaders?.Count > 0 ? bidiHeaders : null,
                body: body).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _isInterceptResolutionHandled = false;

            // Only swallow specific protocol errors that are safe to ignore.
            // Match upstream's handleError behavior.
            if (ex is WebDriverBiDi.WebDriverBiDiException bidiEx &&
                (bidiEx.Message.Contains("Invalid request id") ||
                 bidiEx.Message.Contains("no such request")))
            {
                return;
            }

            throw;
        }
    }

    private async Task AbortInternalAsync()
    {
        _isInterceptResolutionHandled = true;

        try
        {
            await _request.FailRequestAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _isInterceptResolutionHandled = false;

            // Only swallow specific protocol errors that are safe to ignore.
            if (ex is WebDriverBiDi.WebDriverBiDiException bidiEx &&
                (bidiEx.Message.Contains("Invalid request id") ||
                 bidiEx.Message.Contains("no such request")))
            {
                return;
            }

            throw;
        }
    }

    private async Task RespondInternalAsync(ResponseData response)
    {
        _isInterceptResolutionHandled = true;

        try
        {
            var responseHeaders = new List<Header>();

            if (response.Headers != null)
            {
                foreach (var keyValuePair in response.Headers)
                {
                    if (keyValuePair.Value == null)
                    {
                        continue;
                    }

                    if (keyValuePair.Value is ICollection values)
                    {
                        foreach (var val in values)
                        {
                            responseHeaders.Add(new Header(keyValuePair.Key.ToLowerInvariant(), val.ToString()));
                        }
                    }
                    else
                    {
                        responseHeaders.Add(new Header(keyValuePair.Key.ToLowerInvariant(), keyValuePair.Value.ToString()));
                    }
                }
            }

            // Check if the content-length header exists
            var hasContentLength = false;
            foreach (var header in responseHeaders)
            {
                if (header.Name.Equals("content-length", StringComparison.OrdinalIgnoreCase))
                {
                    hasContentLength = true;
                    break;
                }
            }

            // Add content-length if not present and we have body data
            if (!hasContentLength && response.BodyData != null)
            {
                responseHeaders.Add(new Header("content-length", response.BodyData.Length.ToString(CultureInfo.InvariantCulture)));
            }

            if (response.ContentType != null)
            {
                responseHeaders.Add(new Header("content-type", response.ContentType));
            }

            var statusCode = response.Status != null ? (int)response.Status : 200;
            var reasonPhrase = GetReasonPhrase(statusCode);

            BytesValue body = null;
            if (response.BodyData != null)
            {
                body = BytesValue.FromByteArray(response.BodyData);
            }

            await _request.ProvideResponseAsync(
                statusCode: (uint)statusCode,
                reasonPhrase: reasonPhrase,
                headers: responseHeaders.Count > 0 ? responseHeaders : null,
                body: body).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _isInterceptResolutionHandled = false;

            // Only swallow specific protocol errors that are safe to ignore.
            if (ex is WebDriverBiDi.WebDriverBiDiException bidiEx &&
                (bidiEx.Message.Contains("Invalid request id") ||
                 bidiEx.Message.Contains("no such request")))
            {
                return;
            }

            throw;
        }
    }

    private void Initialize()
    {
        _request.Redirect += (sender, e) =>
        {
            var request = e.Request;

            var httpRequest = From(request, Frame as BidiFrame, this);
            RedirectChainList.Add(this);

            var successFired = false;
            var errorFired = false;

            request.Success += (_, _) =>
            {
                // Emulate 'once' behavior - only fire once
                if (successFired)
                {
                    return;
                }

                successFired = true;
                BidiPage.OnRequestFinished(new RequestEventArgs(httpRequest));
            };

            request.Error += (_, args) =>
            {
                // Emulate 'once' behavior - only fire once
                if (errorFired)
                {
                    return;
                }

                errorFired = true;

                // Only set FailureText from the browser error if we haven't already set it
                // (e.g., from a custom abort error code in AbortInternalAsync).
                if (httpRequest.FailureText == null)
                {
                    httpRequest.FailureText = args.Error;
                }

                if (BidiPage.TryMarkFailedEventFired(httpRequest.Url))
                {
                    BidiPage.OnRequestFailed(new RequestEventArgs(httpRequest));
                }
            };

            // Use the request interception queue to serialize handler execution for redirects.
            _ = BidiPage.RequestInterceptionQueue.Enqueue(
                () => httpRequest.FinalizeInterceptionsAsync());
        };

        _request.ResponseStarted += (_, e) =>
        {
            Response = BidiHttpResponse.From(e.Response, this, BidiPage.BidiBrowser.CdpSupported);
        };

        _request.Success += (_, e) =>
        {
            Response = BidiHttpResponse.From(e.Response, this, BidiPage.BidiBrowser.CdpSupported);
        };

        _request.Error += (_, args) =>
        {
            // Only set FailureText from the browser error if we haven't already set it
            // (e.g., from a custom abort error code in AbortInternalAsync).
            if (FailureText == null)
            {
                FailureText = args.Error;
            }
        };

        _request.Authenticate += HandleAuthentication;

        // Firefox BiDi sends duplicate BeforeRequestSent events with different request IDs for
        // speculative/parallel sub-resource loading during navigation.
        // Deduplicate by URL during navigation. The tracker is cleared after navigation completes
        // (in GoToAsync) so that subsequent fetch/XHR calls to the same URLs work correctly.
        if (BidiPage.TryMarkRequestEventFired(Url))
        {
            BidiPage.OnRequest(this);
        }

        if (HasInternalHeaderOverwrite && CanBeIntercepted)
        {
            _interception.Handlers.Add(async () =>
            {
                await ContinueAsync(
                    new Payload
                    {
                        Headers = Headers,
                    },
                    0).ConfigureAwait(false);
            });
        }
    }

    private void HandleAuthentication(object sender, EventArgs e)
    {
        var credentials = BidiPage.Credentials;

        if (credentials != null && !_authenticationHandled)
        {
            _authenticationHandled = true;

            // Fire-and-forget (matches upstream's `void` pattern)
            _ = _request.ContinueWithAuthAsync(
                ContinueWithAuthActionType.ProvideCredentials,
                new AuthCredentials(credentials.Username, credentials.Password));
        }
        else
        {
            // Fire-and-forget (matches upstream's `void` pattern)
            _ = _request.ContinueWithAuthAsync(ContinueWithAuthActionType.Cancel);
        }
    }
}

#endif
