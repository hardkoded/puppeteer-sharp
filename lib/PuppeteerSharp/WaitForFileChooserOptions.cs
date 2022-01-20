namespace CefSharp.Puppeteer
{
    /// <summary>
    /// Optional waiting parameters.
    /// </summary>
    /// <seealso cref="DevToolsContext.WaitForFileChooserAsync(WaitForFileChooserOptions)"/>
    public class WaitForFileChooserOptions
    {
        /// <summary>
        /// Maximum time to wait for in milliseconds. Defaults to 30000 (30 seconds). Pass 0 to disable timeout.
        /// The default value can be changed by setting the <see cref="DevToolsContext.DefaultTimeout"/> property.
        /// </summary>
        public int Timeout { get; set; } = PuppeteerSharp.DefaultTimeout;
    }
}