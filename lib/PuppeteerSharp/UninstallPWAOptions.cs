namespace PuppeteerSharp
{
    /// <summary>
    /// Options for <see cref="IBrowser.UninstallPWAAsync(UninstallPWAOptions)"/>.
    /// </summary>
    public class UninstallPWAOptions
    {
        /// <summary>
        /// Gets or sets the id from the web app's manifest file.
        /// </summary>
        public string ManifestId { get; set; }
    }
}
