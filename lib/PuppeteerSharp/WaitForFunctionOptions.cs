namespace PuppeteerSharp
{
    /// <summary>
    /// Optional waiting parameters.
    /// </summary>
    /// <seealso cref="IPage.WaitForFunctionAsync(string, WaitForFunctionOptions, object[])"/>
    /// <seealso cref="IFrame.WaitForFunctionAsync(string, WaitForFunctionOptions, object[])"/>
    /// <seealso cref="WaitForSelectorOptions"/>
    public class WaitForFunctionOptions : WaitForOptions
    {
        /// <summary>
        /// An interval at which the <c>pageFunction</c> is executed. defaults to <see cref="WaitForFunctionPollingOption.Raf"/>.
        /// </summary>
        public WaitForFunctionPollingOption Polling { get; set; } = WaitForFunctionPollingOption.Raf;

        /// <summary>
        /// An interval at which the <c>pageFunction</c> is executed. If no value is specified will use <see cref="Polling"/>.
        /// </summary>
        public int? PollingInterval { get; set; }

        /// <summary>
        /// A <see cref="CancellationToken"/> to cancel the wait operation.
        /// </summary>
        /// <remarks>
        /// This is the .NET equivalent of the upstream AbortController/AbortSignal pattern.
        /// When the token is cancelled, the wait operation will throw an <see cref="System.OperationCanceledException"/>.
        /// </remarks>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Root element.
        /// </summary>
        internal IElementHandle Root { get; set; }
    }
}
