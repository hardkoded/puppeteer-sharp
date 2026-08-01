namespace PuppeteerSharp
{
    /// <summary>
    /// Options for <see cref="IBrowser.GetPWAStateAsync(GetPWAStateOptions)"/>.
    /// </summary>
    public class GetPWAStateOptions
    {
        /// <summary>
        /// Gets or sets the id from the web app's manifest file.
        /// </summary>
        public string ManifestId { get; set; }
    }
}
