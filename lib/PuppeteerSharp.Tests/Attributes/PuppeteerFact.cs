
namespace PuppeteerSharp.Tests.Attributes
{
    /// <summary>
    /// Puppeteer Fact
    /// </summary>
    public class PuppeteerFact : FactAttribute
    {
        /// <summary>
        /// Creates a new <seealso cref="PuppeteerFact"/>
        /// </summary>
        public PuppeteerFact()
        {
            Timeout = System.Diagnostics.Debugger.IsAttached ? TestConstants.DebuggerAttachedTestTimeout : TestConstants.DefaultTestTimeout;
        }
    }
}
