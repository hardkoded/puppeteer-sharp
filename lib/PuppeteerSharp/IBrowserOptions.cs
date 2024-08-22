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
        bool AcceptInsecureCerts { get; }

        /// <summary>
        /// Gets or sets the default Viewport.
        /// </summary>
        /// <value>The default Viewport.</value>
        ViewPortOptions DefaultViewport { get; set; }
        /* Restore when it's usable
        /// <summary>
        /// Protocol type.
        /// </summary>
        ProtocolType Protocol { get; set; }
        */
    }
}
