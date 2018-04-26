using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
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

        public async Task ContinueAsync(Payload overrides = null)
        {
            overrides = overrides ?? new Payload();
            /*
            if (!_allowInterception)
            {
                throw new PuppeteerException("Request interception is not enabled!");
            }
            if (_interceptionHandled)
            {
                throw new PuppeteerException("Request is already handled!");
            }*/
            Contract.Requires(_allowInterception, "Request interception is not enabled!");
            Contract.Requires(!_interceptionHandled, "Request interception is already handled!");

            _interceptionHandled = true;

            try
            {
                var requestData = new Dictionary<string, object> { ["interceptionId"] = InterceptionId };
                if (overrides.Url != null) requestData["url"] = overrides.Url;
                if (overrides.Method != null) requestData["method"] = overrides.Method;
                if (overrides.PostData != null) requestData["postData"] = overrides.PostData;
                if (overrides.Headers != null) requestData["headers"] = overrides.Headers;

                await _client.SendAsync("Network.continueInterceptedRequest", requestData);
            }
            catch (Exception ex)
            {
                // In certain cases, protocol will return error if the request was already canceled
                // or the page was closed. We should tolerate these errors
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task RespondAsync(ResponseData response)
        {
            if (Url.StartsWith("data:", StringComparison.Ordinal))
            {
                return;
            }

            if (!_allowInterception)
            {
                throw new InvalidOperationException("Request interception is not enabled!");
            }
            if (_interceptionHandled)
            {
                throw new InvalidOperationException("Request is already handled!");
            }

            _interceptionHandled = true;

            //TODO: In puppeteer this is a buffer but as I don't know the real implementation yet
            //I will consider this a string
            var responseBody = response.Body;
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

            if (!string.IsNullOrEmpty(responseBody) && !responseHeaders.ContainsKey("content-length"))
            {
                responseHeaders["content-length"] = responseBody.Length;
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

            //This is a buffer in puppeteer but I don't know the final implementation here
            var responseBuffer = text.ToString();

            if (!string.IsNullOrEmpty(responseBody))
            {
                responseBuffer += responseBody;
            }

            try
            {
                await _client.SendAsync("Network.continueInterceptedRequest", new Dictionary<string, object>
                {
                    {"interceptionId", InterceptionId},
                    {"rawResponse", responseBuffer}
                });
            }
            catch (Exception)
            {
                // In certain cases, protocol will return error if the request was already canceled
                // or the page was closed. We should tolerate these errors
                //TODO: Choose log mechanism
            }
        }

        public async Task AbortAsync(RequestAbortErrorCode errorCode = RequestAbortErrorCode.Failed)
        {
            if (!_allowInterception)
            {
                throw new PuppeteerException("Request interception is not enabled!");
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
            catch (Exception ex)
            {
                // In certain cases, protocol will return error if the request was already canceled
                // or the page was closed. We should tolerate these errors
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion
    }
}