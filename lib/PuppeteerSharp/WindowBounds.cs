namespace PuppeteerSharp
{
    /// <summary>
    /// Bounds for a browser window.
    /// </summary>
    public class WindowBounds
    {
        /// <summary>
        /// Gets or sets the left position of the window.
        /// </summary>
        public int? Left { get; set; }

        /// <summary>
        /// Gets or sets the top position of the window.
        /// </summary>
        public int? Top { get; set; }

        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Gets or sets the window state.
        /// </summary>
        public WindowState? WindowState { get; set; }
    }
}
