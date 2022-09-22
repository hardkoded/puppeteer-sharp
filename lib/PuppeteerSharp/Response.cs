using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    public class Response : IResponse
    {
        private readonly CDPSession _client;
        private readonly bool _fromDiskCache;
        private byte[] _buffer;
        private static readonly Regex _extraInfoLines = new(@"[^ ]* [^ ]* (?<text>.*)", RegexOptions.Multiline);

        internal Response(
            CDPSession client,
            Request request,
            ResponsePayload responseMessage,
            ResponseReceivedExtraInfoResponse extraInfo)
        {
            _client = client;
            Request = request;
            Status = extraInfo != null ? extraInfo.StatusCode : responseMessage.Status;
            StatusText = ParseStatusTextFromExtrInfo(extraInfo) ?? responseMessage.StatusText;
            Url = request.Url;
            _fromDiskCache = responseMessage.FromDiskCache;
            FromServiceWorker = responseMessage.FromServiceWorker;

            Headers = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var headers = extraInfo != null ? extraInfo.Headers : responseMessage.Headers;
            if (headers != null)
            {
                foreach (var keyValue in responseMessage.Headers)
                {
                    Headers[keyValue.Key] = keyValue.Value;
                }
            }
            SecurityDetails = responseMessage.SecurityDetails;
            RemoteAddress = new RemoteAddress
            {
                IP = responseMessage.RemoteIPAddress,
                Port = responseMessage.RemotePort,
            };

            BodyLoadedTaskWrapper = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        /// <inheritdoc/>
        public string Url { get; }

        /// <inheritdoc/>
        public Dictionary<string, string> Headers { get; }

        /// <inheritdoc/>
        public HttpStatusCode Status { get; }

        /// <inheritdoc/>
        public bool Ok => Status == 0 || ((int)Status >= 200 && (int)Status <= 299);

        /// <inheritdoc/>
        public Request Request { get; }

        /// <inheritdoc/>
        IRequest IResponse.Request => Request;

        /// <inheritdoc/>
        public bool FromCache => _fromDiskCache || (Request?.FromMemoryCache ?? false);

        /// <inheritdoc/>
        public SecurityDetails SecurityDetails { get; }

        /// <inheritdoc/>
        public bool FromServiceWorker { get; }

        /// <inheritdoc/>
        public string StatusText { get; }

        /// <inheritdoc/>
        public RemoteAddress RemoteAddress { get; }

        internal TaskCompletionSource<bool> BodyLoadedTaskWrapper { get; }

        /// <inheritdoc/>
        public IFrame Frame => Request.Frame;

        private string ParseStatusTextFromExtrInfo(ResponseReceivedExtraInfoResponse extraInfo)
        {
            if (extraInfo == null || extraInfo.HeadersText == null)
            {
                return null;
            }

            var lines = extraInfo.HeadersText.Split('\r');
            if (lines.Length == 0)
            {
                return null;
            }
            var firstLine = lines[0];

            var match = _extraInfoLines.Match(firstLine);
            if (!match.Success)
            {
                return null;
            }
            var statusText = match.Groups["text"].Value;
            return statusText;
        }

        /// <summary>
        /// Returns a Task which resolves to a buffer with response body
        /// </summary>
        /// <returns>A Task which resolves to a buffer with response body</returns>
        public async ValueTask<byte[]> BufferAsync()
        {
            if (_buffer == null)
            {
                await BodyLoadedTaskWrapper.Task.ConfigureAwait(false);

                try
                {
                    var response = await _client.SendAsync<NetworkGetResponseBodyResponse>("Network.getResponseBody", new NetworkGetResponseBodyRequest
                    {
                        RequestId = Request.RequestId,
                    }).ConfigureAwait(false);

                    _buffer = response.Base64Encoded
                        ? Convert.FromBase64String(response.Body)
                        : Encoding.UTF8.GetBytes(response.Body);
                }
                catch (Exception ex)
                {
                    throw new BufferException("Unable to get response body", ex);
                }
            }

            return _buffer;
        }

        /// <inheritdoc/>
        public async Task<string> TextAsync() => Encoding.UTF8.GetString(await BufferAsync().ConfigureAwait(false));

        /// <inheritdoc/>
        public async Task<JObject> JsonAsync() => JObject.Parse(await TextAsync().ConfigureAwait(false));

        /// <inheritdoc/>
        public async Task<T> JsonAsync<T>() => (await JsonAsync().ConfigureAwait(false)).ToObject<T>(true);
    }
}
