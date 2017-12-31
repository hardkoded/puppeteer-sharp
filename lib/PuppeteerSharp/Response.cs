using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace PuppeteerSharp
{
    public class Response
    {
        private Session _client;
        //TODO: In puppeteer this is a buffer but as I don't know the real implementation yet
        //I will consider this a string
        private bool _ok;
        private string _url;

        public Response(Session client, Request request, HttpStatusCode status, Dictionary<string, object> headers)
        {
            _client = client;
            Request = request;
            Status = status;
            _ok = (int)status >= 200 && (int)status <= 299;
            _url = request.Url;

            Headers = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> keyValue in headers)
            {
                Headers[keyValue.Key] = keyValue.Value;
            }

        }

        #region Properties
        public string Body { get; internal set; }
        public Dictionary<string, object> Headers { get; internal set; }
        public string ContentType { get; internal set; }
        public HttpStatusCode? Status { get; internal set; }

        public Task<string> ContentTask => ContentTaskWrapper.Task;
        public TaskCompletionSource<string> ContentTaskWrapper { get; internal set; }
        public Request Request { get; internal set; }
        #endregion

        #region Public Methods

        public Task<string> Buffer()
        {
            if (ContentTaskWrapper == null)
            {
                ContentTaskWrapper = new TaskCompletionSource<string>();

                Request.CompleteTask.ContinueWith(async (task) =>
                {

                    var response = await _client.SendAsync("Network.getResponseBody", new Dictionary<string, object>
                    {
                        {"requestID", Request.RequestId}
                    });

                    ContentTaskWrapper.SetResult(response.body);
                });
            }

            return ContentTaskWrapper.Task;
        }


        public async Task<string> TextAsync() => await Buffer();

        public async Task<JObject> Json()
        {
            var text = await TextAsync();
            return JObject.Parse(text);
        }

        public string GetRequestHash(Request request)
        {
            var normalizedUrl = request.Url;

            try
            {
                // Decoding is necessary to normalize URLs.
                // The method will throw if the URL is malformed. In this case,
                // consider URL to be normalized as-is.
                normalizedUrl = HttpUtility.UrlDecode(request.Url);
            }
            catch { }

            var hash = new RequestData()
            {
                Url = request.Url,
                Method = request.Method,
                PostData = request.PostData
            };

            if (!normalizedUrl.StartsWith("data:", System.StringComparison.Ordinal))
            {
                foreach (var item in request.Headers.Where(kv => kv.Key != "Accept" && kv.Key != "Referrer" &&
                                                           kv.Key != "X-DevTools-Emulate-Network-Conditions-Client-Id"))
                {
                    hash.Headers[item.Key] = item.Value;
                }
            }

            return JsonConvert.SerializeObject(request);
        }
        #endregion

    }
}