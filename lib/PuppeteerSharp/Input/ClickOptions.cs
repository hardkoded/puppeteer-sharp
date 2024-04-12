namespace PuppeteerSharp.Input
{
    /// <summary>
    /// Options to use when clicking.
    /// </summary>
    public class ClickOptions
    {
        /// <summary>
        /// Time to wait between <c>mousedown</c> and <c>mouseup</c> in milliseconds. Defaults to 0.
        /// </summary>
        public int Delay { get; set; } = 0;

        /// <summary>
        /// Defaults to 1. See https://developer.mozilla.org/en-US/docs/Web/API/UIEvent/detail.
        /// </summary>
        public int Count { get; set; } = 1;

        /// <summary>
        /// The button to use for the click. Defaults to <see cref="MouseButton.Left"/>.
        /// </summary>
        public MouseButton Button { get; set; } = MouseButton.Left;

        /// <summary>
        /// Offset for the clickable point relative to the top-left corner of the border-box.
        /// </summary>
        public Offset? OffSet { get; set; }
    }
}
