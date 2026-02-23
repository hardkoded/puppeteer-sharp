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
        /// Experimental setting to disable monitoring network events by default. When
        /// set to <c>false</c>, parts of Puppeteer that depend on network events would not
        /// work such as <see cref="IRequest"/> and <see cref="IResponse"/>.
        /// </summary>
        bool NetworkEnabled { get; set; }

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
