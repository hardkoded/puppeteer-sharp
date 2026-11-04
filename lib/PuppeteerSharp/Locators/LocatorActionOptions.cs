using System.Threading;

namespace PuppeteerSharp.Locators
{
    /// <summary>
    /// Options for locator actions.
    /// </summary>
    public class LocatorActionOptions
    {
        /// <summary>
        /// Gets or sets the cancellation token to cancel the action.
        /// </summary>
        public CancellationToken CancellationToken { get; set; } = default;
    }
}
