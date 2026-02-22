namespace PuppeteerSharp
{
    /// <summary>
    /// Request to delete cookies matching specific filters.
    /// </summary>
    public class DeleteCookiesRequest
    {
        /// <summary>
        /// Gets or sets the name of the cookies to remove.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL. If specified, deletes all the cookies with the given name where domain and path match
        /// the provided URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the domain. If specified, deletes only cookies with the exact domain.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the path. If specified, deletes only cookies with the exact path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the partition key. If specified, deletes cookies in the given partition key.
        /// In Chrome, partitionKey matches the top-level site the partitioned cookie is available in.
        /// </summary>
        public string PartitionKey { get; set; }
    }
}
