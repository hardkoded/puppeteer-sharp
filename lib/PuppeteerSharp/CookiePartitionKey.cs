namespace PuppeteerSharp
{
    /// <summary>
    /// Cookie's Partition Key data.
    /// </summary>
    public class CookiePartitionKey
    {
        /// <summary>
        /// The site of the top-level URL the browser was visiting at the start of the request to the endpoint that set the cookie.
        /// </summary>
        public string TopLevelSite { get; set; }

        /// <summary>
        /// Indicates if the cookie has any ancestors that are cross-site to the TopLevelSite.
        /// </summary>
        public bool HasCrossSiteAncestor { get; set; }
    }
}
