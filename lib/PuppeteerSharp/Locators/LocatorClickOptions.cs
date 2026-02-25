using System.Threading;
using PuppeteerSharp.Input;

namespace PuppeteerSharp.Locators
{
    /// <summary>
    /// Options for locator click actions.
    /// </summary>
    public class LocatorClickOptions
    {
        /// <summary>
        /// Gets or sets the cancellation token to cancel the action.
        /// </summary>
        public CancellationToken CancellationToken { get; set; } = default;

        /// <summary>
        /// Gets or sets the mouse button to use for clicking.
        /// </summary>
        public MouseButton Button { get; set; } = MouseButton.Left;

        /// <summary>
        /// Gets or sets the number of clicks.
        /// </summary>
        public int Count { get; set; } = 1;

        /// <summary>
        /// Gets or sets the time to wait between mousedown and mouseup in milliseconds.
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        /// Gets or sets the offset for the click relative to the top-left corner of the element's padding box.
        /// </summary>
        public Offset? OffSet { get; set; }
    }
}
