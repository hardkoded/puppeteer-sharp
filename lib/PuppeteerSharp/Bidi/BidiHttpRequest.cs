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
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Bidi.Core;
using PuppeteerSharp.Helpers;
using WebDriverBiDi.Network;

namespace PuppeteerSharp.Bidi;

/// <inheritdoc />
public class BidiHttpRequest : Request<BidiHttpResponse>
{
    private readonly Request _request;
    private bool _authenticationHandled;

    private BidiHttpRequest(Request request, BidiFrame frame, BidiHttpRequest redirect)
    {
        _request = request;
        Frame = frame;
        RedirectChainList = redirect?.RedirectChainList ?? [];
        Requests.AddItem(request, this);
    }

    /// <summary>
    /// Gets the merged headers including extra HTTP headers.
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

            return headers;
        }
    }

    /// <inheritdoc/>
    public override InterceptResolutionState InterceptResolutionState
    {
        get
        {
            if (!CanBeIntercepted() && !IsInterceptResolutionHandled)
            {
                return new InterceptResolutionState(InterceptResolutionAction.Disabled);
            }

            return IsInterceptResolutionHandled
                ? new InterceptResolutionState(InterceptResolutionAction.AlreadyHandled)
                : PendingResolutionState;
        }
    }

    /// <inheritdoc/>
    public override bool IsInterceptResolutionHandled { get; protected set; }

    /// <inheritdoc/>
    /// <exception cref="NotSupportedException">Always thrown. Worker requests are not available in WebDriver BiDi.</exception>
    public override WebWorker Worker =>
        throw new NotSupportedException("Worker is not supported in WebDriver BiDi.");

    // TODO: I don't like having this static field at all. This will cause memory leaks for sure.
    // We need to move this to a place where we can control its lifecycle.
    internal static AsyncDictionaryHelper<Request, BidiHttpRequest> Requests { get; } =
        new("Request {0} not found");

    internal BidiPage BidiPage => (BidiPage)Frame.Page;

    // True only for the canonical request that actually emitted the Request event.
    // Firefox sends duplicate (phantom) BeforeRequestSent events for the same URL during navigation;
    // those phantoms are deduplicated and must not drive page-level events like RequestServedFromCache.
    internal bool RequestEventEmitted { get; private set; }

    internal WebDriverBiDi.Network.FetchTimingInfo Timings => _request.Timings;

    internal ConcurrentDictionary<string, string> ExtraHttpHeaders => BidiPage.ExtraHttpHeaders;

    internal bool HasInternalHeaderOverwrite => ExtraHttpHeaders.Values.Count != 0;

    /// <summary>
    /// Gets whether network interception is enabled at the page level.
    /// This is used to verify that the user has called SetRequestInterceptionAsync(true).
    /// </summary>
    private bool PageLevelInterceptionEnabled => BidiPage.IsNetworkInterceptionEnabled;

    /// <inheritdoc />
    public override Task<string> FetchPostDataAsync() => _request.FetchPostDataAsync();

    /// <inheritdoc />
    public override async Task<byte[]> ContentAsync()
    {
        try
        {
            return await _request.GetRequestContentAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            BidiPage
                .BidiBrowser.LoggerFactory?.CreateLogger<BidiHttpRequest>()
                .ErrorWithException(ex, ex.ToString());
            return null;
        }
    }

    internal static BidiHttpRequest From(
        Request bidiRequest,
        BidiFrame frame,
        BidiHttpRequest redirect = null
    )
    {
        var isNavigationRequest = bidiRequest.Navigation != null;
        var request = new BidiHttpRequest(bidiRequest, frame, redirect)
        {
            Url = bidiRequest.Url,
            Method = new System.Net.Http.HttpMethod(bidiRequest.Method),
            IsNavigationRequest = isNavigationRequest,
            HasPostData = bidiRequest.HasPostData,
            ResourceType = MapDestinationToResourceType(
                bidiRequest.Destination,
                isNavigationRequest
            ),
        };
        request.Initialize();
        return request;
    }

    internal Task<byte[]> GetResponseContentAsync() => _request.GetResponseContentAsync();

    internal override async Task FinalizeInterceptionsAsync()
    {
        await base.FinalizeInterceptionsAsync().ConfigureAwait(false);

        // Firefox phantom requests never raise the Request event, so no handler will ever
        // resolve them. Continue them to let the browser proceed. Requests the user has seen
        // are left to the user, as upstream does.
        if (
            InterceptResolutionState.Action == InterceptResolutionAction.None
            && !RequestEventEmitted
            && CanBeIntercepted()
        )
        {
            await ContinueInternalAsync().ConfigureAwait(false);
        }
    }

    internal override async Task ContinueInternalAsync(Payload overrides = null)
    {
        IsInterceptResolutionHandled = true;

        try
        {
            // Merge the original request headers with extra headers and user agent headers
            var mergedHeaders =
                HasInternalHeaderOverwrite || overrides?.Headers != null
                    ? GetMergedHeaders(overrides?.Headers)
                    : null;

            var bidiHeaders = ConvertToBidiHeaders(mergedHeaders);

            BytesValue body = null;
            if (overrides?.PostData != null)
            {
                body = BytesValue.FromBase64String(
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(overrides.PostData))
                );
            }

            await _request
                .ContinueRequestAsync(
                    url: overrides?.Url,
                    method: overrides?.Method?.ToString(),
                    headers: bidiHeaders?.Count > 0 ? bidiHeaders : null,
                    body: body
                )
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            IsInterceptResolutionHandled = false;
            HandleError(ex);
        }
    }

    internal override async Task AbortInternalAsync(RequestAbortErrorCode errorCode)
    {
        IsInterceptResolutionHandled = true;

        try
        {
            await _request.FailRequestAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            IsInterceptResolutionHandled = false;
            HandleError(ex);
        }
    }

    internal override async Task RespondInternalAsync(ResponseData response)
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
                        responseHeaders.Add(
                            new Header(keyValuePair.Key.ToLowerInvariant(), val.ToString())
                        );
                    }
                }
                else
                {
                    responseHeaders.Add(
                        new Header(
                            keyValuePair.Key.ToLowerInvariant(),
                            keyValuePair.Value.ToString()
                        )
                    );
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
        if (!hasContentLength && response.BodyData is { Length: > 0 })
        {
            responseHeaders.Add(
                new Header(
                    "content-length",
                    response.BodyData.Length.ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        if (response.ContentType != null)
        {
            // Like the CDP implementation, ContentType replaces a content-type
            // passed in Headers instead of sending both.
            responseHeaders.RemoveAll(header => header.Name == "content-type");
            responseHeaders.Add(new Header("content-type", response.ContentType));
        }

        var statusCode = response.Status != null ? (int)response.Status : 200;
        var reasonPhrase = GetReasonPhrase(statusCode);

        BytesValue body = null;
        if (response.BodyData != null)
        {
            body = BytesValue.FromByteArray(response.BodyData);
        }

        IsInterceptResolutionHandled = true;

        try
        {
            await _request
                .ProvideResponseAsync(
                    statusCode: (uint)statusCode,
                    reasonPhrase: reasonPhrase,
                    headers: responseHeaders.Count > 0 ? responseHeaders : null,
                    body: body
                )
                .ConfigureAwait(false);
        }
        catch (WebDriverBiDi.WebDriverBiDiConnectionException)
        {
            // Port-only: upstream lets this rejection surface as an unhandled promise rejection,
            // which is benign in Node. In .NET it would crash the process from an async void
            // event handler, so we tolerate responding to a request after the connection closed.
            IsInterceptResolutionHandled = false;
        }
        catch (Exception ex)
        {
            IsInterceptResolutionHandled = false;
            HandleError(ex);
        }
    }

    /// <inheritdoc/>
    protected override void VerifyInterception()
    {
        if (!PageLevelInterceptionEnabled)
        {
            throw new PuppeteerException("Request Interception is not enabled!");
        }

        if (IsInterceptResolutionHandled)
        {
            throw new PuppeteerException("Request is already handled!");
        }
    }

    /// <inheritdoc/>
    protected override bool CanBeIntercepted() => _request.IsBlocked;

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

    private static string GetReasonPhrase(int statusCode) =>
        HttpStatusTextHelper.GetStatusText(statusCode);

    private static ResourceType MapDestinationToResourceType(
        string destination,
        bool isNavigationRequest
    )
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

    private static void HandleError(Exception ex)
    {
        // Match upstream's handleError: only header validation errors are surfaced.
        // In certain cases, the protocol returns an error if the request was already
        // canceled or the page was closed. We should tolerate these errors.
        if (
            ex.Message.Contains("Invalid header")
            || ex.Message.Contains("Unsafe header")
            || ex.Message.Contains("Expected \"header\"")
            || ex.Message.Contains("invalid argument")
        )
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex).Throw();
        }
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
            _ = BidiPage.RequestInterceptionQueue.Enqueue(() =>
                httpRequest.FinalizeInterceptionsAsync()
            );
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

        // Register the internal header handler before any Request event subscriber can enqueue
        // its own, so it runs first and does not overwrite their priority-0 overrides.
        if (HasInternalHeaderOverwrite && CanBeIntercepted())
        {
            ((IInterceptableRequest)this).EnqueueInterceptionAction(async _ =>
            {
                await ContinueAsync(new Payload { Headers = Headers }, 0).ConfigureAwait(false);
            });
        }

        // Firefox BiDi sends duplicate BeforeRequestSent events with different request IDs for
        // speculative/parallel sub-resource loading during navigation.
        // Deduplicate by URL during navigation. The tracker is cleared after navigation completes
        // (in GoToAsync) so that subsequent fetch/XHR calls to the same URLs work correctly.
        if (BidiPage.TryMarkRequestEventFired(Url))
        {
            RequestEventEmitted = true;
            BidiPage.OnRequest(this);
            var requestArgs = new RequestEventArgs(this);
            ((BidiBrowserContext)BidiPage.BrowserContext).OnRequest(requestArgs);
            BidiPage.BidiBrowser.OnRequest(requestArgs);
        }
    }

    private void HandleAuthentication(object sender, EventArgs e)
    {
        var credentials = BidiPage.Credentials;

        if (credentials != null && !_authenticationHandled)
        {
            _authenticationHandled = true;
            _ = ContinueWithAuthSafeAsync(
                ContinueWithAuthActionType.ProvideCredentials,
                new AuthCredentials(credentials.Username, credentials.Password)
            );
        }
        else
        {
            _ = ContinueWithAuthSafeAsync(ContinueWithAuthActionType.Cancel);
        }
    }

    private async Task ContinueWithAuthSafeAsync(
        ContinueWithAuthActionType action,
        AuthCredentials credentials = null
    )
    {
        try
        {
            await _request.ContinueWithAuthAsync(action, credentials).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // The request may have been canceled or the page closed before the challenge was answered.
            BidiPage
                .BidiBrowser.LoggerFactory?.CreateLogger<BidiHttpRequest>()
                .FailedToContinueWithAuth(ex);
        }
    }
}

#endif
