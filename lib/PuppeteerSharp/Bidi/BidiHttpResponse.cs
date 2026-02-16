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
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PuppeteerSharp.Bidi;

/// <inheritdoc />
public class BidiHttpResponse : Response<BidiHttpRequest>
{
    private readonly BidiHttpRequest _request;
    private WebDriverBiDi.Network.ResponseData _data;

    private BidiHttpResponse(WebDriverBiDi.Network.ResponseData data, BidiHttpRequest request, bool cdpSupported)
    {
        _data = data;
        _request = request;
        Request = request;
        Status = (HttpStatusCode)data.Status;
        StatusText = data.StatusText;
        Url = data.Url;
        FromCache = data.FromCache;

        // BiDi doesn't provide remote address information
        RemoteAddress = new RemoteAddress { IP = string.Empty, Port = -1 };

        // Convert headers
        Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var header in data.Headers)
        {
            Headers[header.Name] = header.Value.Value;
        }
    }

    // Internal constructor for synthetic responses (e.g., cached history navigation)
    private BidiHttpResponse(string url, HttpStatusCode status, bool fromCache)
    {
        _data = null;
        _request = null;
        Url = url;
        Status = status;
        FromCache = fromCache;
    }

    // Constructor for creating response from raw data without a request
    // Used for Firefox reload workaround when we capture the response directly
    private BidiHttpResponse(WebDriverBiDi.Network.ResponseData data)
    {
        _data = data;
        _request = null;
        Status = (HttpStatusCode)data.Status;
        StatusText = data.StatusText;
        Url = data.Url;
        FromCache = data.FromCache;
        RemoteAddress = new RemoteAddress { IP = string.Empty, Port = -1 };

        Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var header in data.Headers)
        {
            Headers[header.Name] = header.Value.Value;
        }
    }

    /// <inheritdoc />
    public override bool FromCache { get; }

    /// <inheritdoc />
    public override async ValueTask<byte[]> BufferAsync()
    {
        if (_request == null)
        {
            throw new PuppeteerException("Response body is not available for this response type.");
        }

        return await _request.GetResponseContentAsync().ConfigureAwait(false);
    }

    internal static BidiHttpResponse From(WebDriverBiDi.Network.ResponseData data, BidiHttpRequest request, bool cdpSupported)
    {
        var existingResponse = request.Response;
        if (existingResponse != null)
        {
            // Update existing response data with up-to-date data.
            existingResponse._data = data;
            return existingResponse;
        }

        var response = new BidiHttpResponse(data, request, cdpSupported);
        response.Initialize();
        return response;
    }

    // Creates a synthetic response for cached navigation (e.g., history navigation)
    // See: https://github.com/w3c/webdriver-bidi/issues/502
    internal static BidiHttpResponse FromCachedNavigation(string url)
    {
        return new BidiHttpResponse(url, HttpStatusCode.OK, fromCache: true);
    }

    // Creates a response from raw ResponseData when we don't have a tracked request
    // Used for Firefox reload workaround when we capture the response directly
    internal static BidiHttpResponse FromResponseData(WebDriverBiDi.Network.ResponseData data)
    {
        return new BidiHttpResponse(data);
    }

    private void Initialize()
    {
        if (_data.FromCache)
        {
            _request.FromMemoryCache = true;

            // Only fire RequestServedFromCache if this URL hasn't already been reported.
            // Firefox BiDi can send duplicate BeforeRequestSent events with different request IDs,
            // each getting its own BidiHttpRequest, but we should only report the cache event once per URL.
            if (_request.BidiPage.TryMarkCacheEventFired(_request.Url))
            {
                ((Page)_request.Frame.Page).OnRequestServedFromCache(new RequestEventArgs(_request));
            }
        }

        _request.BidiPage.OnResponse(new ResponseCreatedEventArgs(this));
    }
}

#endif
