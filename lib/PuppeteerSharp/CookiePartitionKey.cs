using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Cookie partition key.
    /// </summary>
    public class CookiePartitionKey : IEquatable<CookiePartitionKey>
    {
        /// <summary>
        /// The site of the top-level URL the browser was visiting at the start of the request
        /// to the endpoint that set the cookie.
        /// In Chrome, maps to the CDP's topLevelSite partition key.
        /// </summary>
        public string SourceOrigin { get; set; }

        /// <summary>
        /// Indicates if the cookie has any ancestors that are cross-site to the topLevelSite.
        /// Supported only in Chrome.
        /// </summary>
        public bool? HasCrossSiteAncestor { get; set; }

        /// <inheritdoc/>
        public bool Equals(CookiePartitionKey other)
        {
            if (other is null)
            {
                return false;
            }

            return SourceOrigin == other.SourceOrigin && HasCrossSiteAncestor == other.HasCrossSiteAncestor;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => Equals(obj as CookiePartitionKey);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = SourceOrigin?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ (HasCrossSiteAncestor?.GetHashCode() ?? 0);
            return hashCode;
        }
    }
}
