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

    private BidiHttpResponse(WebDriverBiDi.Network.ResponseData data, BidiHttpRequest request, bool cdpSupported)
    {
        _data = data;
        _request = request;
        Status = (HttpStatusCode)data.Status;
        Url = data.Url;
    }

    /// <inheritdoc />
    public override bool FromCache { get; }

    /// <inheritdoc />
    public override ValueTask<byte[]> BufferAsync() => throw new System.NotImplementedException();

    internal static BidiHttpResponse From(WebDriverBiDi.Network.ResponseData data, BidiHttpRequest request, bool cdpSupported)
    {
        var response = new BidiHttpResponse(data, request, cdpSupported);
        response.Initialize();
        return response;
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
