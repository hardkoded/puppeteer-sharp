
namespace PuppeteerSharp.Tests.Attributes
{
    /// <summary>
    /// Skip browsers.
    /// </summary>
    public class SkipBrowserFact : PuppeteerFact
    {
        /// <summary>
        /// Creates a new <seealso cref="SkipBrowserFact"/>
        /// </summary>
        /// <param name="skipFirefox">Skip firefox</param>
        /// <param name="skipChromium">Skip Chromium</param>
        public SkipBrowserFact(
            bool skipFirefox = false,
            bool skipChromium = false)
        {
            if (SkipBrowser(skipFirefox, skipChromium))
            {
                Skip = "Skipped by browser";
            }
        }

        private static bool SkipBrowser(bool skipFirefox, bool skipChromium)
            => (skipFirefox && !TestConstants.IsChrome) || (skipChromium && TestConstants.IsChrome);
    }
}
