namespace PuppeteerSharp
{
    /// <summary>
    /// Parameters for adding a new screen.
    /// </summary>
    public class AddScreenParams
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
        /// Gets or sets the work area insets.
        /// </summary>
        public WorkAreaInsets WorkAreaInsets { get; set; }

        /// <summary>
        /// Gets or sets the device pixel ratio.
        /// </summary>
        public double? DevicePixelRatio { get; set; }

        /// <summary>
        /// Gets or sets the rotation in degrees.
        /// </summary>
        public int? Rotation { get; set; }

        /// <summary>
        /// Gets or sets the color depth.
        /// </summary>
        public int? ColorDepth { get; set; }

        /// <summary>
        /// Gets or sets the screen label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the screen is internal.
        /// </summary>
        public bool? IsInternal { get; set; }
    }
}
