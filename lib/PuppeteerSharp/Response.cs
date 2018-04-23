using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp
{
    public class Response
    {
        private Session _client;
        //TODO: In puppeteer this is a buffer but as I don't know the real implementation yet
        //I will consider this a string
        private bool _ok;

        public Response(Session client, Request request, HttpStatusCode status, Dictionary<string, object> headers, SecurityDetails securityDetails)
        {
            _client = client;
            Request = request;
            Status = status;
            _ok = (int)status >= 200 && (int)status <= 299;
            Url = request.Url;

            Headers = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> keyValue in headers)
            {
                Headers[keyValue.Key] = keyValue.Value;
            }
            SecurityDetails = securityDetails;
        }

        #region Properties
        public string Url { get; internal set; }
        public Dictionary<string, object> Headers { get; internal set; }
        public string ContentType { get; internal set; }
        public HttpStatusCode? Status { get; internal set; }
        public Task<string> ContentTask => ContentTaskWrapper.Task;
        public TaskCompletionSource<string> ContentTaskWrapper { get; internal set; }
        public Request Request { get; internal set; }
        public SecurityDetails SecurityDetails { get; internal set; }
        #endregion

        #region Public Methods

        public Task<string> BufferAsync()
        {
            if (ContentTaskWrapper == null)
            {
                ContentTaskWrapper = new TaskCompletionSource<string>();

                Request.CompleteTask.ContinueWith(async (task) =>
                {
                    try
                    {
                        var response = await _client.SendAsync("Network.getResponseBody", new Dictionary<string, object>
                        {
                            {"requestId", Request.RequestId}
                        });

                        ContentTaskWrapper.SetResult(response.body.ToString());
                    }
                    catch (Exception ex)
                    {
                        ContentTaskWrapper.SetException(new BufferException("Unable to get response body", ex));
                    }
                });
            }

            return ContentTaskWrapper.Task;
        }

        public Task<string> TextAsync() => BufferAsync();

        public async Task<JObject> JsonAsync() => JObject.Parse(await TextAsync());
        public async Task<T> JsonAsync<T>() => (await JsonAsync()).ToObject<T>();

        #endregion

    }
}