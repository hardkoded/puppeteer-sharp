using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Whenever the page sends a request, the following events are emitted by puppeteer's page:
    /// <see cref="Page.Request"/> emitted when the request is issued by the page.
    /// <see cref="Page.Response"/> emitted when/if the response is received for the request.
    /// <see cref="Page.RequestFinished"/> emitted when the response body is downloaded and the request is complete.
    /// 
    /// If request fails at some point, then instead of <see cref="Page.RequestFinished"/> event (and possibly instead of <see cref="Page.Response"/> event), the <see cref="Page.RequestFailed"/> event is emitted.
    /// 
    /// If request gets a 'redirect' response, the request is successfully finished with the <see cref="Page.RequestFinished"/> event, and a new request is issued to a redirected url.
    /// </summary>
    public class Request : Payload
    {
        #region Private Members
        private readonly Session _client;

        private bool _allowInterception;
        private bool _interceptionHandled;

        #endregion

        public Request(Session client, string requestId, string interceptionId, bool allowInterception, string url,
                      ResourceType resourceType, Payload payload, Frame frame)
        {
            _client = client;
            RequestId = requestId;
            InterceptionId = interceptionId;
            _allowInterception = allowInterception;
            _interceptionHandled = false;
            Url = url;
            ResourceType = resourceType;
            Method = payload.Method;
            PostData = payload.PostData;
            Frame = frame;

            Headers = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> keyValue in payload.Headers)
            {
                Headers[keyValue.Key] = keyValue.Value;
            }

            CompleteTaskWrapper = new TaskCompletionSource<bool>();
        }

        #region Properties
        public Response Response { get; set; }
        public string Failure { get; set; }
        public string RequestId { get; internal set; }
        public string InterceptionId { get; internal set; }
        public ResourceType ResourceType { get; internal set; }
        public Task<bool> CompleteTask => CompleteTaskWrapper.Task;
        public TaskCompletionSource<bool> CompleteTaskWrapper { get; internal set; }
        public Frame Frame { get; }
        #endregion

        #region Public Methods

        /// <summary>
        /// Continues request with optional request overrides. To use this, request interception should be enabled with <see cref="Page.SetRequestInterceptionAsync(bool)"/>. Exception is immediately thrown if the request interception is not enabled.
        /// </summary>
        /// <param name="overrides">Optional request overwrites.</param>
        /// <returns>Task</returns>
        public async Task ContinueAsync(Payload overrides = null)
        {
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
                var requestData = new Dictionary<string, object> { ["interceptionId"] = InterceptionId };
                if (overrides?.Url != null) requestData["url"] = overrides.Url;
                if (overrides?.Method != null) requestData["method"] = overrides.Method;
                if (overrides?.PostData != null) requestData["postData"] = overrides.PostData;
                if (overrides?.Headers != null) requestData["headers"] = overrides.Headers;

                await _client.SendAsync("Network.continueInterceptedRequest", requestData);
            }
            catch (Exception)
            {
                // In certain cases, protocol will return error if the request was already canceled
                // or the page was closed. We should tolerate these errors
                //TODO: Choose log mechanism
            }
        }

        /// <summary>
        /// Fulfills request with given response. To use this, request interception should be enabled with <see cref="Page.SetRequestInterceptionAsync(bool)"/>. Exception is thrown if request interception is not enabled.
        /// </summary>
        /// <param name="response">Response that will fulfill this request</param>
        /// <returns>Task</returns>
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

            var responseHeaders = new Dictionary<string, object>();

            if (response.Headers != null)
            {
                foreach (var keyValue in response.Headers)
                {
                    responseHeaders[keyValue.Key] = keyValue.Value;
                }
            }

            if (response.ContentType != null)
            {
                responseHeaders["content-type"] = response.ContentType;
            }

            if (!responseHeaders.ContainsKey("content-length"))
            {
                responseHeaders["content-length"] = response.BodyData != null ? response.BodyData.Length : response.Body.Length;
            }

            var statusCode = response.Status ?? HttpStatusCode.Accepted;
            var statusText = statusCode.ToString();
            var statusLine = $"HTTP / 1.1${(int)statusCode} ${statusText}";

            var text = new StringBuilder(statusLine + "\n");

            foreach (var header in responseHeaders)
            {
                text.AppendLine($"{header.Key}: {header.Value}");
            }
            text.AppendLine(string.Empty);

            if (!string.IsNullOrEmpty(response.Body))
            {
                text.Append(response.Body);
            }

            var responseData = Encoding.UTF8.GetBytes(text.ToString());

            if (response.BodyData != null)
            {
                var concatenatedData = new byte[responseData.Length + response.BodyData.Length];
                responseData.CopyTo(concatenatedData, 0);
                response.BodyData.CopyTo(concatenatedData, responseData.Length);
            }

            var responseBase64 = Convert.ToBase64String(responseData);

            try
            {
                await _client.SendAsync("Network.continueInterceptedRequest", new Dictionary<string, object>
                {
                    {"interceptionId", InterceptionId},
                    {"rawResponse", responseBase64}
                });
            }
            catch (Exception)
            {
                // In certain cases, protocol will return error if the request was already canceled
                // or the page was closed. We should tolerate these errors
                //TODO: Choose log mechanism
            }
        }

        /// <summary>
        /// Aborts request. To use this, request interception should be enabled with <see cref="Page.SetRequestInterceptionAsync(bool)"/>.
        /// Exception is immediately thrown if the request interception is not enabled.
        /// </summary>
        /// <param name="errorCode">Optional error code. Defaults to <see cref="RequestAbortErrorCode.Failed"/></param>
        /// <returns>Task</returns>
        public async Task AbortAsync(RequestAbortErrorCode errorCode = RequestAbortErrorCode.Failed)
        {
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
                await _client.SendAsync("Network.continueInterceptedRequest", new Dictionary<string, object>
                {
                    {"interceptionId", InterceptionId},
                    {"errorReason", errorReason}
                });
            }
            catch (Exception)
            {
                // In certain cases, protocol will return error if the request was already canceled
                // or the page was closed. We should tolerate these errors
                //TODO: Choose log mechanism
            }
        }
        #endregion
    }
}