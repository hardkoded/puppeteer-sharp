namespace PuppeteerSharp
{
    /// <summary>
    /// View port options used on <see cref="IPage.SetViewportAsync(ViewPortOptions)"/>.
    /// </summary>
    public record ViewPortOptions
    {
        /// <summary>
        /// Default Viewport.
        /// </summary>
        public static ViewPortOptions Default => new()
        {
            Width = 800,
            Height = 600,
        };

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The page width width in pixels.</value>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The page height in pixels.</value>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets whether the meta viewport tag is taken into account.
        /// </summary>
        /// <value>Whether the meta viewport tag is taken into account. Defaults to <c>false</c>.</value>
        public bool IsMobile { get; set; }

        /// <summary>
        /// Gets or sets the device scale factor.
        /// </summary>
        /// <value>Specify device scale factor (can be thought of as dpr). Defaults to 1.</value>
        public double DeviceScaleFactor { get; set; } = 1;

        /// <summary>
        /// Gets or sets if viewport is in landscape mode.
        /// </summary>
        /// <value>Specifies if viewport is in landscape mode. Defaults to <c>false</c>.</value>
        public bool IsLandscape { get; set; }

        /// <summary>
        /// Gets or sets if viewport supports touch events.
        /// </summary>
        /// <value>Specifies if viewport supports touch events. Defaults to <c>false</c>.</value>
        public bool HasTouch { get; set; }
    }
}
