using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    public abstract class Response<TRequest>
        : IResponse
        where TRequest : IRequest
    {
        internal Response()
        {
        }

        /// <inheritdoc/>
        public RemoteAddress RemoteAddress { get; protected init; }

        /// <inheritdoc/>
        public string Url { get; protected init; }

        /// <inheritdoc/>
        public bool Ok => Status == 0 || ((int)Status >= 200 && (int)Status <= 299);

        /// <inheritdoc/>
        public HttpStatusCode Status { get; protected init; }

        /// <inheritdoc/>
        public string StatusText { get; protected init; }

        /// <inheritdoc/>
        public Dictionary<string, string> Headers { get; protected init; }

        /// <inheritdoc/>
        IRequest IResponse.Request => Request;

        /// <inheritdoc/>
        public abstract bool FromCache { get; }

        /// <inheritdoc/>
        public SecurityDetails SecurityDetails { get; protected init; }

        /// <inheritdoc/>
        public bool FromServiceWorker { get; protected init; }

        /// <inheritdoc/>
        public IFrame Frame => Request.Frame;

        /// <inheritdoc/>
        public ResourceTiming Timing { get; protected init; }

        /// <inheritdoc cref="Request"/>
        protected TRequest Request { get; init; }

        /// <summary>
        /// Returns a Task which resolves to a buffer with response body.
        /// </summary>
        /// <returns>A Task which resolves to a buffer with response body.</returns>
        public abstract ValueTask<byte[]> BufferAsync();

        /// <inheritdoc/>
        public async Task<string> TextAsync() => Encoding.UTF8.GetString(await BufferAsync().ConfigureAwait(false));

        /// <inheritdoc/>
        public async Task<JsonDocument> JsonAsync(JsonDocumentOptions options = default)
        {
            var content = await TextAsync().ConfigureAwait(false);
            return JsonDocument.Parse(content, options);
        }

        /// <inheritdoc/>
        public async Task<T> JsonAsync<T>(JsonSerializerOptions options = default)
        {
            var content = await TextAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(content, options ?? JsonHelper.DefaultJsonSerializerSettings.Value);
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> AsFetchResponseAsync()
        {
            var isNullBodyStatus = Status is HttpStatusCode.SwitchingProtocols or HttpStatusCode.NoContent or HttpStatusCode.ResetContent or HttpStatusCode.NotModified;
            var content = isNullBodyStatus ? null : new ByteArrayContent(await BufferAsync().ConfigureAwait(false));
            var response = new HttpResponseMessage(Status)
            {
                Content = content,
                ReasonPhrase = StatusText,
            };

            foreach (var header in Headers)
            {
                var isContentHeader = content != null && header.Key.StartsWith("content-", StringComparison.OrdinalIgnoreCase);
                var target = isContentHeader ? (HttpHeaders)content.Headers : response.Headers;

                // Set-Cookie values are joined with newlines, so each one becomes its own header entry.
                var values = header.Key.Equals("set-cookie", StringComparison.OrdinalIgnoreCase)
                    ? header.Value.Split('\n')
                    : [header.Value];
                foreach (var value in values)
                {
                    target.TryAddWithoutValidation(header.Key, value.Trim());
                }
            }

            return response;
        }
    }
}
