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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Bidi;

/// <inheritdoc />
public class BidiHttpRequest : Request<BidiHttpResponse>
{
    private readonly Request _request;
    private readonly BidiRequestInterception _interception = new();

    private BidiHttpRequest(Request request, BidiFrame frame, BidiHttpRequest redirect)
    {
        _request = request;
        Frame = frame;
        RedirectChainList = [];
        Requests.AddItem(request, this);
    }

    // TODO: I don't like having this static field at all. This will cause memory leaks for sure.
    // We need to move this to a place where we can control its lifecycle.
    internal static AsyncDictionaryHelper<Request, BidiHttpRequest> Requests { get; } = new("Request {0} not found");

    internal override Payload ContinueRequestOverrides { get; }

    internal override ResponseData ResponseForRequest { get; }

    internal override RequestAbortErrorCode AbortErrorReason { get; }

    internal BidiPage BidiPage => (BidiPage)Frame.Page;

    internal ConcurrentDictionary<string, string> ExtraHttpHeaders => BidiPage.ExtraHttpHeaders;

    internal ConcurrentDictionary<string, string> UserAgentHeaders => BidiPage.UserAgentHeaders;

    internal bool HasInternalHeaderOverwrite => ExtraHttpHeaders.Values.Count != 0 || UserAgentHeaders.Values.Count != 0;

    /// <inheritdoc />
    public override async Task ContinueAsync(Payload payloadOverrides = null, int? priority = null)
    {
        // Merge the original request headers with extra headers and user agent headers
        var mergedHeaders = GetMergedHeaders(payloadOverrides?.Headers);
        var bidiHeaders = ConvertToBidiHeaders(mergedHeaders);

        await _request.ContinueRequestAsync(
            url: payloadOverrides?.Url,
            method: payloadOverrides?.Method?.ToString(),
            headers: bidiHeaders?.Count > 0 ? bidiHeaders : null).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override Task RespondAsync(ResponseData response, int? priority = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task AbortAsync(RequestAbortErrorCode errorCode = RequestAbortErrorCode.Failed, int? priority = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<string> FetchPostDataAsync() => throw new NotImplementedException();

    internal static BidiHttpRequest From(Request bidiRequest, BidiFrame frame, BidiHttpRequest redirect = null)
    {
        var request = new BidiHttpRequest(bidiRequest, frame, redirect) { Url = bidiRequest.Url, };
        request.Initialize();
        return request;
    }

    internal override async Task FinalizeInterceptionsAsync()
    {
        foreach (var handler in _interception.Handlers)
        {
            await handler().ConfigureAwait(false);
        }

        _interception.Handlers.Clear();
    }

    internal override void EnqueueInterceptionAction(Func<IRequest, Task> pendingHandler) => throw new NotImplementedException();

    private static List<WebDriverBiDi.Network.Header> ConvertToBidiHeaders(Dictionary<string, string> headers)
    {
        if (headers == null || headers.Count == 0)
        {
            return null;
        }

        var bidiHeaders = new List<WebDriverBiDi.Network.Header>();
        foreach (var kvp in headers)
        {
            bidiHeaders.Add(new WebDriverBiDi.Network.Header(kvp.Key, kvp.Value));
        }

        return bidiHeaders;
    }

    private Dictionary<string, string> GetMergedHeaders(Dictionary<string, string> overrideHeaders)
    {
        // Start with the original request headers
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var header in _request.Headers)
        {
            headers[header.Name.ToLowerInvariant()] = header.Value.Value;
        }

        // Add extra HTTP headers from the page
        foreach (var kvp in ExtraHttpHeaders)
        {
            headers[kvp.Key.ToLowerInvariant()] = kvp.Value;
        }

        // Add user agent headers from the page
        foreach (var kvp in UserAgentHeaders)
        {
            headers[kvp.Key.ToLowerInvariant()] = kvp.Value;
        }

        // Apply any override headers from the payload
        if (overrideHeaders != null)
        {
            foreach (var kvp in overrideHeaders)
            {
                headers[kvp.Key.ToLowerInvariant()] = kvp.Value;
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

            request.Success += (sender, e) =>
            {
                BidiPage.OnRequestFinished(new RequestEventArgs(httpRequest));
            };

            request.Success += (sender, e) =>
            {
                BidiPage.OnRequestFinished(new RequestEventArgs(httpRequest));
            };

            request.Error += (o, args) =>
            {
                BidiPage.OnRequestFailed(new RequestEventArgs(httpRequest));
            };

            _ = httpRequest.FinalizeInterceptionsAsync();
        };

        _request.Success += (sender, e) =>
        {
            Response = BidiHttpResponse.From(e.Response, this, BidiPage.BidiBrowser.CdpSupported);
        };

        _request.Success += (sender, args) =>
        {
            Response = BidiHttpResponse.From(args.Response, this, BidiPage.BidiBrowser.CdpSupported);
        };

        _request.Authenticate += HandleAuthentication;

        BidiPage.OnRequest(this);

        if (HasInternalHeaderOverwrite)
        {
            _interception.Handlers.Add(async () =>
            {
                await ContinueAsync(
                new Payload()
                {
                    Headers = Headers,
                },
                0).ConfigureAwait(false);
            });
        }
    }

    private void HandleAuthentication(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}
