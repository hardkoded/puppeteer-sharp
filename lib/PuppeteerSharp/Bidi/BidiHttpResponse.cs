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

using System.Net;
using System.Threading.Tasks;

namespace PuppeteerSharp.Bidi;

/// <inheritdoc />
public class BidiHttpResponse : Response<BidiHttpRequest>
{
    private readonly WebDriverBiDi.Network.ResponseData _data;
    private readonly BidiHttpRequest _request;
    private readonly bool _fromCache;

    private BidiHttpResponse(WebDriverBiDi.Network.ResponseData data, BidiHttpRequest request, bool cdpSupported)
    {
        _data = data;
        _request = request;
        Status = (HttpStatusCode)data.Status;
        Url = data.Url;
        _fromCache = data.FromCache;

        // TODO: Implement SecurityDetails support when webdriverbidi-net library supports extensibility
        // The upstream puppeteer implementation accesses a non-standard 'goog:securityDetails' property
        // which is not yet exposed in the webdriverbidi-net library
    }

    // Internal constructor for synthetic responses (e.g., cached history navigation)
    private BidiHttpResponse(string url, HttpStatusCode status, bool fromCache)
    {
        _data = null;
        _request = null;
        Url = url;
        Status = status;
        _fromCache = fromCache;
    }

    /// <inheritdoc />
    public override bool FromCache => _fromCache;

    /// <inheritdoc />
    public override ValueTask<byte[]> BufferAsync() => throw new System.NotImplementedException();

    internal static BidiHttpResponse From(WebDriverBiDi.Network.ResponseData data, BidiHttpRequest request, bool cdpSupported)
    {
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

    private void Initialize()
    {
        if (_data.FromCache)
        {
            _request.FromMemoryCache = true;
            ((Page)_request.Frame.Page).OnRequestServedFromCache(new RequestEventArgs(_request));
        }

        _request.BidiPage.OnResponse(new ResponseCreatedEventArgs(this));
    }
}
