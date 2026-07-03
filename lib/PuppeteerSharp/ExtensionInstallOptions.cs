namespace PuppeteerSharp
{
    /// <summary>
    /// Options for <see cref="IBrowser.InstallExtensionAsync(string, ExtensionInstallOptions)"/>.
    /// </summary>
    public class ExtensionInstallOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to enable the extension in Incognito or OTR profiles in Chrome.
        /// </summary>
        public bool EnabledInIncognito { get; set; }
    }
}
