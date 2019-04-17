namespace PuppeteerSharp.Abstractions
{
    /// <summary>
    /// Optional waiting parameters.
    /// </summary>
    /// <seealso cref="IPage.WaitForRequestAsync(Func{Request, bool}, WaitForOptions)"/>
    /// <seealso cref="IPage.WaitForRequestAsync(string, WaitForOptions)"/>
    /// <seealso cref="IPage.WaitForResponseAsync(string, WaitForOptions)"/>
    /// <seealso cref="IPage.WaitForResponseAsync(Func{Response, bool}, WaitForOptions)"/>
    public class WaitForOptions
    {
        /// <summary>
        /// Maximum time to wait for in milliseconds. Defaults to 30000 (30 seconds). Pass 0 to disable timeout.
        /// The default value can be changed by setting the <see cref="Page.DefaultTimeout"/> property.
        /// </summary>
        public int Timeout { get; set; } = Constants.DefaultTimeout;
    }
}
