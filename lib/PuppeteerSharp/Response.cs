using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Response
    {
        private Session _client;
        private Request _request;
        //TODO: In puppeteer this is a buffer but as I don't know the real implementation yet
        //I will consider this a string
        private bool _ok;
        private string _url;

        public Response(Session client, Request request, HttpStatusCode status, Dictionary<string, object> headers)
        {
            _client = client;
            _request = request;
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
        public TaskCompletionSource<string> ContentTaskWrapper { get; }
        #endregion

        #region Public Methods

        public Task<string> Buffer()
        {
            if (ContentTaskWrapper == null)
            {

            }

            return ContentTaskWrapper.Task;
            /*
                buffer() {
                    if (!this._contentPromise)
                    {
                        this._contentPromise = this._request._completePromise.then(async () => {
                        const response = await this._client.send('Network.getResponseBody', {
                        requestId: this._request._requestId
                        });
                        return Buffer.from(response.body, response.base64Encoded ? 'base64' : 'utf8');
                    });
                }
                return this._contentPromise;
            }
            */
        }
        #endregion

    }
}