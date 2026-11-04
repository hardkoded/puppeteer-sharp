namespace PuppeteerSharp
{
    /// <summary>
    /// Options for query operations like <see cref="IPage.QuerySelectorAllAsync(string, QueryOptions)"/>.
    /// </summary>
    public class QueryOptions
    {
        /// <summary>
        /// Whether to run the query in isolation. When returning many elements
        /// from <see cref="IPage.QuerySelectorAllAsync(string, QueryOptions)"/> or similar methods,
        /// it might be useful to turn off the isolation to improve performance.
        /// By default, the querying code will be executed in a separate sandbox realm.
        /// </summary>
        /// <value>Defaults to <c>true</c>.</value>
        public bool Isolate { get; set; } = true;
    }
}
