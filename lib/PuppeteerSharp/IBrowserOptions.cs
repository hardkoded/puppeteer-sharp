namespace PuppeteerSharp
{
    /// <summary>
    /// Browser options.
    /// </summary>
    public interface IBrowserOptions
    {
        /// <summary>
        /// Whether to ignore HTTPS errors during navigation. Defaults to false.
        /// </summary>
        bool IgnoreHTTPSErrors { get; set; }

        /// <summary>
        /// Gets or sets the default Viewport.
        /// </summary>
        /// <value>The default Viewport.</value>
        ViewPortOptions DefaultViewport { get; set; }

        /// <summary>
        /// If `true`, the browser will enqueue all <seealso cref="Browser.NewPageAsync"/> calls in the <seealso cref="Browser.ScreenshotTaskQueue"/>.
        /// So new pages call won't interfere with the <seealso cref="Page.ScreenshotBase64Async(ScreenshotOptions)"/> method call.
        /// EnqueueNewPages might be needed when using Chromium with <seealso cref="LaunchOptions.Headless"/> in false and making calls in parallel.
        /// It is false by default because enqueuing <seealso cref="Browser.NewPageAsync"/> it's not necesarry all the time and it's a performance hit.
        /// </summary>
        bool EnqueueNewPages { get; set; }
    }
}