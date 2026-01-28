namespace PuppeteerSharp
{
    /// <summary>
    /// Window bounds for creating a new window.
    /// </summary>
    public class WindowBounds
    {
        /// <summary>
        /// The left position of the window.
        /// </summary>
        public int? Left { get; set; }

        /// <summary>
        /// The top position of the window.
        /// </summary>
        public int? Top { get; set; }

        /// <summary>
        /// The width of the window.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// The height of the window.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// The window state (e.g., normal, minimized, maximized, fullscreen).
        /// </summary>
        public string WindowState { get; set; }
    }
}
