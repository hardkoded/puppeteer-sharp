using System.Collections.Generic;
using System.Net.Http;
using CefSharp.DevTools.Dom.Helpers.Json;
using Newtonsoft.Json;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// Payload information.
    /// </summary>
    public class Payload
    {
        /// <summary>
        /// Gets or sets the HTTP method.
        /// </summary>
        /// <value>HTTP method.</value>
        [JsonConverter(typeof(HttpMethodConverter))]
        public HttpMethod Method { get; set; }
        /// <summary>
        /// Gets or sets the post data.
        /// </summary>
        /// <value>The post data.</value>
        public string PostData { get; set; }
        /// <summary>
        /// Gets or sets the HTTP headers.
        /// </summary>
        /// <value>HTTP headers.</value>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }
    }
}
