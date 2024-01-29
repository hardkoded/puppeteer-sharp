namespace PuppeteerSharp
{
    // This class represents both InternalQueryHandler and CustomQueryHandler upstream.

    /// <summary>
    /// Contains two functions `queryOne` and `queryAll` to be used as custom query handlers
    /// The functions `queryOne` and `queryAll` are executed in the page context.
    /// </summary>
    public class CustomQueryHandler
    {
        /// <summary>
        /// `queryOne` should take an `Element` and a selector string as argument and return a single `Element` or `null` if no element is found.
        /// </summary>
        public string QueryOne { get; set; }

        /// <summary>
        /// `queryAll` takes the same arguments but should instead return a `NodeListOf of Element` or `Array of Element` with all the elements that match the given query selector.
        /// </summary>
        public string QueryAll { get; set; }
    }
}
