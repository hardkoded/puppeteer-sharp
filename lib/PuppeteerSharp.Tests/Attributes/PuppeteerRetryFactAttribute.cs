using xRetry;

namespace PuppeteerSharp.Tests.Attributes
{
    /// <summary>
    /// Puppeteer Fact
    /// </summary>
    public class PuppeteerRetryFactAttribute : RetryFactAttribute
    {
        /// <summary>
        /// Creates a new <seealso cref="PuppeteerRetryFactAttribute"/>
        /// </summary>
        public PuppeteerRetryFactAttribute()
        {
            Timeout = System.Diagnostics.Debugger.IsAttached ? TestConstants.DebuggerAttachedTestTimeout : TestConstants.DefaultTestTimeout;
        }
    }
}
