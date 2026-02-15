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
using System.Linq;
using System.Threading.Tasks;
using WebDriverBiDi.Network;

namespace PuppeteerSharp.Bidi.Core;

internal class Request : IDisposable
{
    private readonly BrowsingContext _browsingContext;
    private readonly BeforeRequestSentEventArgs _eventArgs;
    private string _error;
    private Request _redirect;
    private WebDriverBiDi.Network.ResponseData _response;
    private Task<byte[]> _responseContentPromise;
    private Task<string> _requestBodyPromise;

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

    internal event EventHandler<ResponseEventArgs> ResponseStarted;

    public bool IsDisposed { get; set; }

    public string Navigation => _eventArgs.NavigationId;

    public FetchTimingInfo Timings { get; private set; }

    public Request LastRedirect
    {
        get
        {
            var redirect = _redirect;
            while (redirect?._redirect != null)
            {
                redirect = redirect._redirect;
            }

            return redirect;
        }
    }

    public string Url => _eventArgs.Request.Url;

    public string Method => _eventArgs.Request.Method;

    public IList<ReadOnlyHeader> Headers => _eventArgs.Request.Headers;

    public WebDriverBiDi.Network.ResponseData Response => _response;

    public bool HasError => _error != null;

    public bool IsBlocked => _eventArgs.IsBlocked;

    public string ErrorText => _error;

    public ulong RedirectCount => _eventArgs.RedirectCount;

    public WebDriverBiDi.Network.InitiatorType? InitiatorType => _eventArgs.Initiator?.Type;

    public string Destination => _eventArgs.Request.Destination;

    public bool HasPostData => (_eventArgs.Request.BodySize ?? 0) > 0;

    private string Id => _eventArgs.Request.RequestId;

    private Session Session => _browsingContext.UserContext.Browser.Session;

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

    internal async Task ContinueRequestAsync(
        string url = null,
        string method = null,
        List<Header> headers = null,
        BytesValue body = null)
    {
        var commandParams = new ContinueRequestCommandParameters(Id)
        {
            Url = url,
            Method = method,
            Headers = headers,
            Body = body,
        };
        await Session.Driver.Network.ContinueRequestAsync(commandParams).ConfigureAwait(false);
    }

    internal async Task FailRequestAsync()
    {
        var commandParams = new FailRequestCommandParameters(Id);
        await Session.Driver.Network.FailRequestAsync(commandParams).ConfigureAwait(false);
    }

    internal async Task ProvideResponseAsync(
        uint? statusCode = null,
        string reasonPhrase = null,
        List<Header> headers = null,
        BytesValue body = null)
    {
        var commandParams = new ProvideResponseCommandParameters(Id)
        {
            StatusCode = statusCode,
            ReasonPhrase = reasonPhrase,
            Headers = headers,
            Body = body,
        };
        await Session.Driver.Network.ProvideResponseAsync(commandParams).ConfigureAwait(false);
    }

    internal async Task ContinueWithAuthAsync(ContinueWithAuthActionType action, AuthCredentials credentials = null)
    {
        var commandParams = new ContinueWithAuthCommandParameters(Id)
        {
            Action = action,
        };

        if (action == ContinueWithAuthActionType.ProvideCredentials && credentials != null)
        {
            commandParams.Credentials = credentials;
        }

        await Session.Driver.Network.ContinueWithAuthAsync(commandParams).ConfigureAwait(false);
    }

    internal async Task<string> FetchPostDataAsync()
    {
        if (!HasPostData)
        {
            return null;
        }

        _requestBodyPromise ??= FetchPostDataInternalAsync();
        return await _requestBodyPromise.ConfigureAwait(false);
    }

    internal async Task<byte[]> GetResponseContentAsync()
    {
        _responseContentPromise ??= GetResponseContentInternalAsync();
        return await _responseContentPromise.ConfigureAwait(false);
    }

    private async Task<string> FetchPostDataInternalAsync()
    {
        var commandParams = new GetDataCommandParameters(Id)
        {
            DataType = DataType.Request,
        };

        var result = await Session.Driver.Network.GetDataAsync(commandParams).ConfigureAwait(false);

        if (result.Bytes.Type == BytesValueType.String)
        {
            return result.Bytes.Value;
        }

        // Base64 encoding - decode to string
        return System.Text.Encoding.UTF8.GetString(result.Bytes.ValueAsByteArray);
    }

    private async Task<byte[]> GetResponseContentInternalAsync()
    {
        try
        {
            var commandParams = new GetDataCommandParameters(Id)
            {
                DataType = DataType.Response,
            };

            var result = await Session.Driver.Network.GetDataAsync(commandParams).ConfigureAwait(false);
            return result.Bytes.ValueAsByteArray;
        }
        catch (WebDriverBiDi.WebDriverBiDiException ex) when (
            ex.Message.Contains("No resource with given identifier found") ||
            ex.Message.Contains("no such network data"))
        {
            throw new PuppeteerException(
                "Could not load response body for this request. This might happen if the request is a preflight request.",
                ex);
        }
    }

    private void Initialize()
    {
        _browsingContext.Closed += (_, args) =>
        {
            _error = args.Reason;
            OnError(_error);
            Dispose();
        };

        Session.NetworkBeforeRequestSent += (_, args) =>
        {
            if (args.BrowsingContextId != _browsingContext.Id ||
               args.Request.RequestId != Id)
            {
                return;
            }

            // This is a workaround to detect if a beforeRequestSent is for a request
            // sent after continueWithAuth. Currently, only emitted in Firefox.
            var previousRequestHasAuth = _eventArgs.Request.Headers.Any(
                header => header.Name.Equals("authorization", StringComparison.OrdinalIgnoreCase));
            var newRequestHasAuth = args.Request.Headers.Any(
                header => header.Name.Equals("authorization", StringComparison.OrdinalIgnoreCase));
            var isAfterAuth = newRequestHasAuth && !previousRequestHasAuth;

            if (args.RedirectCount != _eventArgs.RedirectCount + 1 && !isAfterAuth)
            {
                return;
            }

            _redirect = From(_browsingContext, args);
            OnRedirect(_redirect);
            Dispose();
        };

        Session.NetworkAuthRequired += (_, args) =>
        {
            if (args.BrowsingContextId != _browsingContext.Id || args.Request.RequestId != Id || !args.IsBlocked)
            {
                return;
            }

            OnAuthenticate();
        };

        Session.NetworkFetchError += (_, args) =>
        {
            if (args.BrowsingContextId != _browsingContext.Id || args.Request.RequestId != Id || args.RedirectCount != _eventArgs.RedirectCount)
            {
                return;
            }

            _error = args.ErrorText;
            OnError(_error);
            Dispose();
        };

        Session.NetworkResponseStarted += (_, args) =>
        {
            if (args.BrowsingContextId != _browsingContext.Id ||
                args.Request.RequestId != Id ||
                _eventArgs.RedirectCount != args.RedirectCount)
            {
                return;
            }

            _response = args.Response;
            Timings = args.Request.Timings;
            OnResponseStarted(new ResponseEventArgs(_response));
        };

        Session.NetworkResponseComplete += (_, args) =>
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

    private void OnResponseStarted(ResponseEventArgs responseEventArgs) => ResponseStarted?.Invoke(this, responseEventArgs);

    private void OnAuthenticate() => Authenticate?.Invoke(this, EventArgs.Empty);

    private void OnRedirect(Request redirect) => Redirect?.Invoke(this, new RequestEventArgs(redirect));

    private void OnError(string error) => Error?.Invoke(this, new ErrorEventArgs(error));
}

#endif
