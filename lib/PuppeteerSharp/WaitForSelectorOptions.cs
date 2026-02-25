using System.Threading;

namespace PuppeteerSharp
{
    /// <summary>
    /// Optional waiting parameters.
    /// </summary>
    /// <seealso cref="IPage.WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
    /// <seealso cref="IFrame.WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
    public class WaitForSelectorOptions : WaitForOptions
    {
        /// <summary>
        /// Wait for element to be present in DOM and to be visible.
        /// </summary>
        public bool? Visible { get; set; }

        /// <summary>
        /// Wait for element to not be found in the DOM or to be hidden.
        /// </summary>
        public bool? Hidden { get; set; }

        /// <summary>
        /// Root element.
        /// </summary>
        public IElementHandle Root { get; set; }

        /// <summary>
        /// A <see cref="CancellationToken"/> to cancel the waitForSelector operation.
        /// </summary>
        /// <remarks>
        /// This is the .NET equivalent of the upstream AbortController/AbortSignal pattern.
        /// When the token is cancelled, the wait operation will throw an <see cref="System.OperationCanceledException"/>.
        /// </remarks>
        public CancellationToken CancellationToken { get; set; }
    }
}
