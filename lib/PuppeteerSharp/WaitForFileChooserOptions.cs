namespace PuppeteerSharp
{
    /// <summary>
    /// Optional waiting parameters.
    /// </summary>
    /// <seealso cref="IPage.WaitForFileChooserAsync(WaitForFileChooserOptions)"/>
    public class WaitForFileChooserOptions
    {
        /// <summary>
        /// Maximum time to wait for in milliseconds. Defaults to 30000 (30 seconds). Pass 0 to disable timeout.
        /// The default value can be changed by setting the <see cref="IPage.DefaultTimeout"/> property.
        /// </summary>
        public int Timeout { get; set; } = Puppeteer.DefaultTimeout;
    }
}
