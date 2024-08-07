using System.Collections.Generic;
using System.Net;
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
    }
}
