namespace PuppeteerSharp
{
    public class ClickOptions
    {
        /// <summary>
        /// Time to wait between `mousedown` and `mouseup` in milliseconds. Defaults to 0.
        /// </summary>
        public int Delay { get; set; } = 0;
        public int ClickCount { get; set; } = 1;
        public MouseButton Button { get; set; } = MouseButton.Left;
    }
}