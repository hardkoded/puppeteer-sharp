using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.Messaging.Protocol.Network;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    public class Request : IRequest
    {
        private readonly CDPSession _client;
        private readonly bool _allowInterception;
        private readonly ILogger _logger;
        private bool _interceptionHandled;

        internal Request(
            CDPSession client,
            Frame frame,
            string interceptionId,
            bool allowInterception,
            RequestWillBeSentPayload e,
            List<IRequest> redirectChain)
        {
            _client = client;
            _allowInterception = allowInterception;
            _interceptionHandled = false;
            _logger = _client.Connection.LoggerFactory.CreateLogger<Request>();

            RequestId = e.RequestId;
            InterceptionId = interceptionId;
            IsNavigationRequest = e.RequestId == e.LoaderId && e.Type == ResourceType.Document;
            Url = e.Request.Url;
            ResourceType = e.Type;
            Method = e.Request.Method;
            PostData = e.Request.PostData;
            HasPostData = e.Request.HasPostData ?? false;
            Frame = frame;
            RedirectChainList = redirectChain;

            Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var keyValue in e.Request.Headers)
            {
                Headers[keyValue.Key] = keyValue.Value;
            }

            FromMemoryCache = false;
        }

        /// <inheritdoc cref="Response"/>
        public Response Response { get; internal set; }

        /// <inheritdoc/>
        IResponse IRequest.Response => Response;

        /// <inheritdoc/>
        public string Failure { get; internal set; }

        /// <inheritdoc/>
        public string RequestId { get; internal set; }

        /// <inheritdoc/>
        public string InterceptionId { get; internal set; }

        /// <inheritdoc/>
        public ResourceType ResourceType { get; internal set; }

        /// <inheritdoc/>
        public IFrame Frame { get; }

        /// <inheritdoc/>
        public bool IsNavigationRequest { get; }

        /// <inheritdoc/>
        public HttpMethod Method { get; internal set; }

        /// <inheritdoc/>
        public object PostData { get; internal set; }

        /// <inheritdoc/>
        public Dictionary<string, string> Headers { get; internal set; }

        /// <inheritdoc/>
        public string Url { get; internal set; }

        /// <inheritdoc/>
        public IRequest[] RedirectChain => RedirectChainList.ToArray();

        /// <inheritdoc/>
        public bool HasPostData { get; private set; }

        internal bool FromMemoryCache { get; set; }

        internal List<IRequest> RedirectChainList { get; }

        /// <inheritdoc/>
        public async Task ContinueAsync(Payload overrides = null)
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

            if (_interceptionHandled)
            {
                throw new PuppeteerException("Request is already handled!");
            }

            _interceptionHandled = true;

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
                    requestData.PostData = Convert.ToBase64String(Encoding.UTF8.GetBytes(overrides?.PostData));
                }

                if (overrides?.Headers?.Count > 0)
                {
                    requestData.Headers = HeadersArray(overrides.Headers);
                }

                await _client.SendAsync("Fetch.continueRequest", requestData).ConfigureAwait(false);
            }
            catch (PuppeteerException ex)
            {
                // In certain cases, protocol will return error if the request was already canceled
                // or the page was closed. We should tolerate these errors
                _logger.LogError(ex.ToString());
            }
        }

        /// <inheritdoc/>
        public async Task RespondAsync(ResponseData response)
        {
            if (Url.StartsWith("data:", StringComparison.Ordinal))
            {
                return;
            }

            if (!_allowInterception)
            {
                throw new PuppeteerException("Request Interception is not enabled!");
            }

            if (_interceptionHandled)
            {
                throw new PuppeteerException("Request is already handled!");
            }

            _interceptionHandled = true;

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

            try
            {
                await _client.SendAsync("Fetch.fulfillRequest", new FetchFulfillRequest
                {
                    RequestId = InterceptionId,
                    ResponseCode = response.Status != null ? (int)response.Status : 200,
                    ResponseHeaders = responseHeaders.ToArray(),
                    Body = response.BodyData != null ? Convert.ToBase64String(response.BodyData) : null,
                }).ConfigureAwait(false);
            }
            catch (PuppeteerException ex)
            {
                // In certain cases, protocol will return error if the request was already canceled
                // or the page was closed. We should tolerate these errors
                _logger.LogError(ex.ToString());
            }
        }

        /// <inheritdoc/>
        public async Task AbortAsync(RequestAbortErrorCode errorCode = RequestAbortErrorCode.Failed)
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

            if (_interceptionHandled)
            {
                throw new PuppeteerException("Request is already handled!");
            }

            var errorReason = errorCode.ToString();

            _interceptionHandled = true;

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
            }
        }

        /// <inheritdoc />
        public async Task<string> FetchPostDataAsync()
        {
            try
            {
                var result = await _client.SendAsync<GetRequestPostDataResponse>(
                    "Network.getRequestPostData",
                    new GetRequestPostDataRequest(RequestId)).ConfigureAwait(false);
                return result.PostData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.ToString());
            }

            return null;
        }

        private Header[] HeadersArray(Dictionary<string, string> headers)
            => headers?.Select(pair => new Header { Name = pair.Key, Value = pair.Value }).ToArray();
    }
}
