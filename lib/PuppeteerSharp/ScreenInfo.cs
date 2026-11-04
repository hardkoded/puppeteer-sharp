namespace PuppeteerSharp
{
    /// <summary>
    /// Contains information about a screen.
    /// </summary>
    public class ScreenInfo
    {
        /// <summary>
        /// Gets or sets the left position of the screen.
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// Gets or sets the top position of the screen.
        /// </summary>
        public int Top { get; set; }

        /// <summary>
        /// Gets or sets the width of the screen.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the screen.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the available left position.
        /// </summary>
        public int AvailLeft { get; set; }

        /// <summary>
        /// Gets or sets the available top position.
        /// </summary>
        public int AvailTop { get; set; }

        /// <summary>
        /// Gets or sets the available width.
        /// </summary>
        public int AvailWidth { get; set; }

        /// <summary>
        /// Gets or sets the available height.
        /// </summary>
        public int AvailHeight { get; set; }

        /// <summary>
        /// Gets or sets the device pixel ratio.
        /// </summary>
        public double DevicePixelRatio { get; set; }

        /// <summary>
        /// Gets or sets the color depth.
        /// </summary>
        public int ColorDepth { get; set; }

        /// <summary>
        /// Gets or sets the screen orientation.
        /// </summary>
        public ScreenOrientationInfo Orientation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the screen is extended.
        /// </summary>
        public bool IsExtended { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the screen is internal.
        /// </summary>
        public bool IsInternal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the screen is the primary screen.
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Gets or sets the screen label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the screen ID.
        /// </summary>
        public string Id { get; set; }
    }
}
