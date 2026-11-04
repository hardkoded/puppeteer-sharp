namespace PuppeteerSharp
{
    /// <summary>
    /// The OS-integration state of an installed web app, returned by <see cref="IBrowser.GetPWAStateAsync(GetPWAStateOptions)"/>.
    /// </summary>
    public class PWAState
    {
        /// <summary>
        /// Gets or sets the current badge count shown on the app icon.
        /// </summary>
        public int BadgeCount { get; set; }

        /// <summary>
        /// Gets or sets the file handlers registered by the app with the OS.
        /// </summary>
        public PWAFileHandler[] FileHandlers { get; set; }
    }
}
