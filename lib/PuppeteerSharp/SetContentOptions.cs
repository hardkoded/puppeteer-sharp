using System.Threading;

namespace PuppeteerSharp
{
    /// <summary>
    /// Options used by <see cref="IPage.SetContentAsync(string, SetContentOptions)"/> and
    /// <see cref="IFrame.SetContentAsync(string, SetContentOptions)"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="WaitUntilNavigation.Networkidle0"/> and <see cref="WaitUntilNavigation.Networkidle2"/>
    /// values are not supported for <c>SetContent</c> operations and will be ignored. They have never worked
    /// reliably for this operation in upstream Puppeteer. Use
    /// <see cref="IPage.WaitForNetworkIdleAsync(WaitForNetworkIdleOptions)"/> separately if you need to wait
    /// for network idle after setting content.
    /// </remarks>
    public record SetContentOptions
    {
        /// <summary>
        /// Maximum operation time in milliseconds, defaults to 30 seconds, pass <c>0</c> to disable timeout.
        /// </summary>
        /// <remarks>
        /// The default value can be changed by setting the <see cref="IPage.DefaultNavigationTimeout"/> or <see cref="IPage.DefaultTimeout"/> property.
        /// </remarks>
        public int? Timeout { get; set; }

        /// <summary>
        /// When to consider the operation succeeded, defaults to <see cref="WaitUntilNavigation.Load"/>.
        /// Given an array of <see cref="WaitUntilNavigation"/>, the operation is considered to be successful
        /// after all events have been fired.
        /// </summary>
        /// <remarks>
        /// <see cref="WaitUntilNavigation.Networkidle0"/> and <see cref="WaitUntilNavigation.Networkidle2"/>
        /// are not supported here.
        /// </remarks>
        public WaitUntilNavigation[] WaitUntil { get; set; }

        /// <summary>
        /// Optional cancellation token to abort the operation.
        /// </summary>
        public CancellationToken? CancellationToken { get; set; }

        internal NavigationOptions ToNavigationOptions() => new()
        {
            Timeout = Timeout,
            WaitUntil = WaitUntil,
            CancellationToken = CancellationToken,
        };
    }
}
