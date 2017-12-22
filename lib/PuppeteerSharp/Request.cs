using System;
using System.Collections.Generic;
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
            foreach(KeyValuePair<string, object> keyValue in payload.Headers)
            {
                _headers[keyValue.Key] = keyValue.Value;
            }
        }

    }
}
