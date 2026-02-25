using System.Threading;

namespace PuppeteerSharp.Locators
{
    /// <summary>
    /// Options for locator scroll actions.
    /// </summary>
    public class LocatorScrollOptions
    {
        /// <summary>
        /// Gets or sets the cancellation token to cancel the action.
        /// </summary>
        public CancellationToken CancellationToken { get; set; } = default;

        /// <summary>
        /// Gets or sets the vertical scroll position.
        /// </summary>
        public decimal? ScrollTop { get; set; }

        /// <summary>
        /// Gets or sets the horizontal scroll position.
        /// </summary>
        public decimal? ScrollLeft { get; set; }
    }
}
