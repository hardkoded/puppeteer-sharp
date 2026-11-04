namespace PuppeteerSharp
{
    /// <summary>
    /// Options for <see cref="IBrowser.InstallPWAAsync(InstallPWAOptions)"/>.
    /// </summary>
    public class InstallPWAOptions
    {
        /// <summary>
        /// Gets or sets the id from the web app's manifest file, commonly the URL of the site installing the web app.
        /// See <see href="https://web.dev/learn/pwa/web-app-manifest">Web app manifest</see>.
        /// </summary>
        public string ManifestId { get; set; }

        /// <summary>
        /// Gets or sets the URL used to install the app, or the URL of its signed web bundle.
        /// </summary>
        /// <remarks>
        /// This is required because the browser-scoped CDP session has no associated page from which Chromium
        /// could derive an install URL.
        /// </remarks>
        public string InstallUrlOrBundleUrl { get; set; }

        /// <summary>
        /// Gets or sets whether the app should open in a standalone window or a browser tab.
        /// </summary>
        /// <remarks>
        /// <c>PWA.install</c> alone leaves the app at Chromium's default display mode (<c>browser</c>);
        /// setting this chains a <c>PWA.changeAppUserSettings</c> call to apply the preference.
        /// </remarks>
        public PWADisplayMode? DisplayMode { get; set; }
    }
}
