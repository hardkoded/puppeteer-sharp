namespace PuppeteerSharp
{
    /// <summary>
    /// Represents the bounds of a browser window.
    /// </summary>
    public class WindowBounds
    {
        /// <summary>
        /// The offset from the left edge of the screen to the window in pixels.
        /// </summary>
        public int? Left { get; set; }

        /// <summary>
        /// The offset from the top edge of the screen to the window in pixels.
        /// </summary>
        public int? Top { get; set; }

        /// <summary>
        /// The window width in pixels.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// The window height in pixels.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// The window state. Defaults to normal.
        /// </summary>
        public WindowState? WindowState { get; set; }
    }
}
