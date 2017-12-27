using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Request
    {
        #region Private Members
        private Session _client;
        private string _requestId;
        private string _interceptionId;
        private bool _allowInterception;
        private string _url;
        private string _resourceType;
        private bool _interceptionHandled;
        private Response _response;
        private string _failureText;
        private Task<bool> _completeTask;
        private Dictionary<string, object> _headers;
        private string _method;
        private object _postData;

        private readonly Dictionary<string, string> _errorReasons = new Dictionary<string, string>
        {
            {"aborted", "Aborted"},
            {"accessdenied", "AccessDenied"},
            {"addressunreachable", "AddressUnreachable"},
            {"connectionaborted", "ConnectionAborted"},
            {"connectionclosed", "ConnectionClosed"},
            {"connectionfailed", "ConnectionFailed"},
            {"connectionrefused", "ConnectionRefused"},
            {"connectionreset", "ConnectionReset"},
            {"internetdisconnected", "InternetDisconnected"},
            {"namenotresolved", "NameNotResolved"},
            {"timedout", "TimedOut"},
            {"failed", "Failed"},
        };
        #endregion

        public Request(Session client, string requestId, string interceptionId, bool allowInterception, string url,
                      string resourceType, Payload payload)
        {
            _client = client;
            _requestId = requestId;
            _interceptionId = interceptionId;
            _allowInterception = allowInterception;
            _interceptionHandled = false;
            _url = url;
            _resourceType = resourceType.ToLower();
            _method = payload.Method;
            _postData = payload.PostData;

            _headers = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> keyValue in payload.Headers)
            {
                _headers[keyValue.Key] = keyValue.Value;
            }
        }

        #region Properties
        public Response Response => _response;
        public string Failure => _failureText;
        #endregion

        #region Public Methods

        public async Task Continue(RequestData overrides)
        {
            Contract.Requires(_allowInterception, "Request interception is not enabled!");
            Contract.Requires(_interceptionHandled, "Request interception is already handled!");

            _interceptionHandled = true;

            try
            {
                await _client.SendAsync("Network.continueInterceptedRequest", new Dictionary<string, object>()
                {
                    {"interceptionId", _interceptionId},
                    {"url", overrides.Url},
                    {"method", overrides.Method},
                    {"postData", overrides.PostData},
                    {"headers", overrides.Headers}
                });
            }
            catch (Exception)
            {
                // In certain cases, protocol will return error if the request was already canceled
                // or the page was closed. We should tolerate these errors
                //TODO: Choose log mechanism
            }
        }


        public async Task Respond()
        {
            if (_url.StartsWith("data:", StringComparison.Ordinal))
            {
                return;
            }

            Contract.Requires(_allowInterception, "Request interception is not enabled!");
            Contract.Requires(_interceptionHandled, "Request is already handled!");

            _interceptionHandled = true;

            //In puppeteer this is a buffer but as I don't know the real implementation yet
            //I will consider this a string
            var responseBody = _response.Body;
            var responseHeaders = new Dictionary<string, object>();

            if (_response.Headers != null)
            {
                foreach (var keyValue in _response.Headers)
                {
                    responseHeaders[keyValue.Key] = keyValue.Value;
                }
            }

            if (_response.ContentType != null)
            {
                responseHeaders["content-type"] = _response.ContentType;
            }

            if (!string.IsNullOrEmpty(responseBody) && !responseHeaders.ContainsKey("content-length"))
            {
                responseHeaders["content-length"] = responseBody.Length;
            }

            var statusCode = Response.Status ?? HttpStatusCode.Accepted;
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
                    {"interceptionId", _interceptionId},
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

        public async Task Abort(string errorCode = "failed")
        {

        }
        #endregion

    }
}
