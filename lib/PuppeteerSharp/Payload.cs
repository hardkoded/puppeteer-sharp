using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using PuppeteerSharp.Helpers.Json;

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
