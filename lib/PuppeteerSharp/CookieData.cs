using System;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// Cookie parameter object used to set cookies in the browser-level cookies API.
    /// </summary>
    public class CookieData
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets if it's secure.
        /// </summary>
        public bool? Secure { get; set; }

        /// <summary>
        /// Gets or sets if it's HTTP only.
        /// </summary>
        public bool? HttpOnly { get; set; }

        /// <summary>
        /// Gets or sets the cookies SameSite value.
        /// </summary>
        public SameSite? SameSite { get; set; }

        /// <summary>
        /// Gets or sets the expiration. Unix time in seconds.
        /// </summary>
        public double? Expires { get; set; }

        /// <summary>
        /// Cookie Priority. Supported only in Chrome.
        /// </summary>
        public CookiePriority? Priority { get; set; }

        /// <summary>
        /// Always set to false. Supported only in Chrome.
        /// </summary>
        [Obsolete("SameParty is deprecated and always ignored.")]
        public bool? SameParty { get; set; }

        /// <summary>
        /// Cookie source scheme type. Supported only in Chrome.
        /// </summary>
        public CookieSourceScheme? SourceScheme { get; set; }

        /// <summary>
        /// Cookie partition key. In Chrome, it matches the top-level site the
        /// partitioned cookie is available in. In Firefox, it matches the
        /// source origin.
        /// </summary>
        [JsonConverter(typeof(CookiePartitionKeyConverter))]
        public CookiePartitionKey PartitionKey { get; set; }
    }
}
