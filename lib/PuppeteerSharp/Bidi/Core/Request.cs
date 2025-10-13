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
using WebDriverBiDi.Network;

namespace PuppeteerSharp.Bidi.Core;

internal class Request : IDisposable
{
    private readonly BrowsingContext _browsingContext;
    private readonly BeforeRequestSentEventArgs _eventArgs;
    private string _error;
    private Request _redirect;
    private WebDriverBiDi.Network.ResponseData _response;

    private Request(BrowsingContext browsingContext, BeforeRequestSentEventArgs args)
    {
        _browsingContext = browsingContext;
        _eventArgs = args;
        Timings = args.Request.Timings;
    }

    public event EventHandler<RequestEventArgs> Redirect;

    public event EventHandler<ErrorEventArgs> Error;

    public event EventHandler Authenticate;

    internal event EventHandler<ResponseEventArgs> Success;

    public bool IsDisposed { get; set; }

    public string Navigation => _eventArgs.NavigationId;

    public string Id => _eventArgs.Request.RequestId;

    public Session Session => _browsingContext.UserContext.Browser.Session;

    public FetchTimingInfo Timings { get; private set; }

    public Request LastRedirect { get; set; }

    public string Url => _eventArgs.Request.Url;

    public WebDriverBiDi.Network.ResponseData Response => _response;

    public bool HasError => _error != null;

    public static Request From(BrowsingContext browsingContext, BeforeRequestSentEventArgs args)
    {
        var request = new Request(browsingContext, args);
        request.Initialize();
        return request;
    }

    public void Dispose()
    {
        IsDisposed = true;
    }

    private void Initialize()
    {
        _browsingContext.Closed += (sender, args) =>
        {
            _error = args.Reason;
            OnError(_error);
            Dispose();
        };

        Session.NetworkBeforeRequestSent += (sender, args) =>
        {
            if (args.BrowsingContextId != _browsingContext.Id ||
               args.Request.RequestId != Id ||
               args.RedirectCount != _eventArgs.RedirectCount + 1)
            {
                return;
            }

            _redirect = From(_browsingContext, args);
            OnRedirect(_redirect);
            Dispose();
        };

        Session.NetworkAuthRequired += (sender, args) =>
        {
            if (args.BrowsingContextId != _browsingContext.Id || args.Request.RequestId != Id || !args.IsBlocked)
            {
                return;
            }

            OnAuthenticate();
        };

        Session.NetworkFetchError += (sender, args) =>
        {
            if (args.BrowsingContextId != _browsingContext.Id || args.Request.RequestId != Id || args.RedirectCount != _eventArgs.RedirectCount)
            {
                return;
            }

            _error = args.ErrorText;
            OnError(_error);
            Dispose();
        };

        Session.NetworkResponseComplete += (sender, args) =>
        {
            if (args.BrowsingContextId != _browsingContext.Id ||
                args.Request.RequestId != Id ||
                _eventArgs.RedirectCount != args.RedirectCount)
            {
                return;
            }

            _response = args.Response;
            Timings = args.Request.Timings;
            OnSuccess(new ResponseEventArgs(_response));

            // In case this is a redirect.
            if (_response.Status is >= 300 and < 400)
            {
                return;
            }

            Dispose();
        };
    }

    private void OnSuccess(ResponseEventArgs responseEventArgs) => Success?.Invoke(this, responseEventArgs);

    private void OnAuthenticate() => Authenticate?.Invoke(this, EventArgs.Empty);

    private void OnRedirect(Request redirect) => Redirect?.Invoke(this, new RequestEventArgs(redirect));

    private void OnError(string error) => Error?.Invoke(this, new ErrorEventArgs(error));
}
