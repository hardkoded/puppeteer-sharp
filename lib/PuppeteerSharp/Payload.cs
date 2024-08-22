using System.Collections.Generic;
using System.Net.Http;

namespace PuppeteerSharp
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
        public Dictionary<string, string> Headers { get; set; } = [];

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Payload"/> has post data.
        /// </summary>
        public bool? HasPostData { get; set; }
    }
}
