using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Cdp.Messaging.Protocol.Network;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc/>
public class CdpHttpRequest : Request<CdpHttpResponse>
{
    private readonly CDPSession _client;
    private readonly bool _allowInterception;
    private readonly ILogger _logger;
    private readonly List<Func<IRequest, Task>> _interceptHandlers = [];
    private Payload _continueRequestOverrides = new();
    private ResponseData _responseForRequest;
    private RequestAbortErrorCode _abortErrorReason;
    private InterceptResolutionState _interceptResolutionState = new(InterceptResolutionAction.None);

    internal CdpHttpRequest(
        CDPSession client,
        IFrame frame,
        string interceptionId,
        bool allowInterception,
        RequestWillBeSentPayload data,
        List<IRequest> redirectChain,
        ILoggerFactory loggerFactory)
    {
        _client = client;
        _logger = loggerFactory.CreateLogger<CdpHttpRequest>();
        Id = data.RequestId;
        IsNavigationRequest = data.RequestId == data.LoaderId && data.Type == ResourceType.Document;
        InterceptionId = interceptionId;
        _allowInterception = allowInterception;
        Url = data.Request.Url;
        ResourceType = data.Type ?? ResourceType.Other;
        Method = data.Request.Method;
        PostData = data.Request.PostData;
        HasPostData = data.Request.HasPostData ?? false;

        Frame = frame;
        RedirectChainList = redirectChain;
        Initiator = data.Initiator;

        Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var keyValue in data.Request.Headers)
        {
            Headers[keyValue.Key] = keyValue.Value;
        }
    }

    /// <inheritdoc cref="Response"/>
    public override CdpHttpResponse Response { get; internal set; }

    internal override Payload ContinueRequestOverrides
    {
        get
        {
            if (!_allowInterception)
            {
                throw new PuppeteerException("Request Interception is not enabled!");
            }

            return _continueRequestOverrides;
        }
    }

    internal override ResponseData ResponseForRequest
    {
        get
        {
            if (!_allowInterception)
            {
                throw new PuppeteerException("Request Interception is not enabled!");
            }

            return _responseForRequest;
        }
    }

    internal override RequestAbortErrorCode AbortErrorReason
    {
        get
        {
            if (!_allowInterception)
            {
                throw new PuppeteerException("Request Interception is not enabled!");
            }

            return _abortErrorReason;
        }
    }

    private InterceptResolutionState InterceptResolutionState
    {
        get
        {
            if (!_allowInterception)
            {
                return new InterceptResolutionState(InterceptResolutionAction.Disabled);
            }

            return IsInterceptResolutionHandled
                ? new InterceptResolutionState(InterceptResolutionAction.AlreadyHandled)
                : _interceptResolutionState;
        }
    }

    private bool IsInterceptResolutionHandled { get; set; }

    /// <inheritdoc/>
    public override async Task ContinueAsync(Payload overrides = null, int? priority = null)
    {
        // Request interception is not supported for data: urls.
        if (Url.StartsWith("data:", StringComparison.InvariantCultureIgnoreCase))
        {
            return;
        }

        if (!_allowInterception)
        {
            throw new PuppeteerException("Request Interception is not enabled!");
        }

        if (IsInterceptResolutionHandled)
        {
            throw new PuppeteerException("Request is already handled!");
        }

        if (priority is null)
        {
            await ContinueInternalAsync(overrides).ConfigureAwait(false);
            return;
        }

        _continueRequestOverrides = overrides;

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

    /// <inheritdoc/>
    public override async Task RespondAsync(ResponseData response, int? priority = null)
    {
        if (Url.StartsWith("data:", StringComparison.Ordinal))
        {
            return;
        }

        if (!_allowInterception)
        {
            throw new PuppeteerException("Request Interception is not enabled!");
        }

        if (IsInterceptResolutionHandled)
        {
            throw new PuppeteerException("Request is already handled!");
        }

        if (priority is null)
        {
            Debug.Assert(response != null, nameof(response) + " != null");
            await RespondInternalAsync(response).ConfigureAwait(false);
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

    /// <inheritdoc/>
    public override async Task AbortAsync(RequestAbortErrorCode errorCode = RequestAbortErrorCode.Failed, int? priority = null)
    {
        // Request interception is not supported for data: urls.
        if (Url.StartsWith("data:", StringComparison.InvariantCultureIgnoreCase))
        {
            return;
        }

        if (!_allowInterception)
        {
            throw new PuppeteerException("Request Interception is not enabled!");
        }

        if (IsInterceptResolutionHandled)
        {
            throw new PuppeteerException("Request is already handled!");
        }

        if (priority is null)
        {
            await AbortInternalAsync(errorCode).ConfigureAwait(false);
            return;
        }

        _abortErrorReason = errorCode;

        if (_interceptResolutionState.Priority is null || priority > _interceptResolutionState.Priority)
        {
            _interceptResolutionState = new InterceptResolutionState(InterceptResolutionAction.Abort, priority);
        }
    }

    /// <inheritdoc />
    public override async Task<string> FetchPostDataAsync()
    {
        try
        {
            var result = await _client.SendAsync<GetRequestPostDataResponse>(
                "Network.getRequestPostData",
                new GetRequestPostDataRequest(Id)).ConfigureAwait(false);
            return result.PostData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.ToString());
        }

        return null;
    }

    internal override async Task FinalizeInterceptionsAsync()
    {
        foreach (var handler in _interceptHandlers)
        {
            await handler(this).ConfigureAwait(false);
        }

        switch (InterceptResolutionState.Action)
        {
            case InterceptResolutionAction.Abort:
                await AbortAsync(_abortErrorReason).ConfigureAwait(false);
                return;
            case InterceptResolutionAction.Respond:
                if (_responseForRequest is null)
                {
                    throw new PuppeteerException("Response is missing for the interception");
                }

                await RespondAsync(_responseForRequest).ConfigureAwait(false);
                return;
            case InterceptResolutionAction.Continue:
                await ContinueInternalAsync(_continueRequestOverrides).ConfigureAwait(false);
                break;
        }
    }

    internal override void EnqueueInterceptionAction(Func<IRequest, Task> pendingHandler)
        => _interceptHandlers.Add(pendingHandler);

    private Header[] HeadersArray(Dictionary<string, string> headers)
        => headers?.Select(pair => new Header { Name = pair.Key, Value = pair.Value }).ToArray();

    private async Task ContinueInternalAsync(Payload overrides = null)
    {
        IsInterceptResolutionHandled = true;

        try
        {
            var requestData = new FetchContinueRequestRequest
            {
                RequestId = InterceptionId,
            };
            if (overrides?.Url != null)
            {
                requestData.Url = overrides.Url;
            }

            if (overrides?.Method != null)
            {
                requestData.Method = overrides.Method.ToString();
            }

            if (overrides?.PostData != null)
            {
                requestData.PostData = Convert.ToBase64String(Encoding.UTF8.GetBytes(overrides.PostData));
            }

            if (overrides?.Headers?.Count > 0)
            {
                requestData.Headers = HeadersArray(overrides.Headers);
            }

            await _client.SendAsync("Fetch.continueRequest", requestData).ConfigureAwait(false);
        }
        catch (PuppeteerException ex)
        {
            IsInterceptResolutionHandled = false;

            // In certain cases, protocol will return error if the request was already canceled
            // or the page was closed. We should tolerate these errors
            _logger.LogError(ex.ToString());
        }
    }

    private async Task AbortInternalAsync(RequestAbortErrorCode errorCode)
    {
        var errorReason = errorCode.ToString();
        IsInterceptResolutionHandled = true;

        try
        {
            await _client.SendAsync("Fetch.failRequest", new FetchFailRequest
            {
                RequestId = InterceptionId,
                ErrorReason = errorReason,
            }).ConfigureAwait(false);
        }
        catch (PuppeteerException ex)
        {
            // In certain cases, protocol will return error if the request was already canceled
            // or the page was closed. We should tolerate these errors
            _logger.LogError(ex.ToString());
            IsInterceptResolutionHandled = false;
        }
    }

    private async Task RespondInternalAsync(ResponseData response)
    {
        IsInterceptResolutionHandled = true;

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
                        responseHeaders.Add(new Header { Name = keyValuePair.Key, Value = val.ToString() });
                    }
                }
                else
                {
                    responseHeaders.Add(new Header { Name = keyValuePair.Key, Value = keyValuePair.Value.ToString() });
                }
            }

            if (!response.Headers.ContainsKey("content-length") && response.BodyData != null)
            {
                responseHeaders.Add(new Header { Name = "content-length", Value = response.BodyData.Length.ToString(CultureInfo.CurrentCulture) });
            }
        }

        if (response.ContentType != null)
        {
            responseHeaders.Add(new Header { Name = "content-type", Value = response.ContentType });
        }

        if (string.IsNullOrEmpty(InterceptionId))
        {
            throw new PuppeteerException("HTTPRequest is missing _interceptionId needed for Fetch.fulfillRequest");
        }

        try
        {
            await _client.SendAsync("Fetch.fulfillRequest", new FetchFulfillRequest
            {
                RequestId = InterceptionId,
                ResponseCode = response.Status != null ? (int)response.Status : 200,
                ResponseHeaders = [.. responseHeaders],
                Body = response.BodyData != null ? Convert.ToBase64String(response.BodyData) : null,
            }).ConfigureAwait(false);
        }
        catch (PuppeteerException ex)
        {
            // In certain cases, protocol will return error if the request was already canceled
            // or the page was closed. We should tolerate these errors
            _logger.LogError(ex.ToString());
            IsInterceptResolutionHandled = false;
        }
    }
}
