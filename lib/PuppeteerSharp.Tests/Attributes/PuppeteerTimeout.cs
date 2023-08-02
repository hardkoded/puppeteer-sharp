
using NUnit.Framework;

namespace PuppeteerSharp.Tests.Attributes
{
    /// <summary>
    /// Puppeteer Fact
    /// </summary>
    public class PuppeteerTimeout : TimeoutAttribute
    {
        /// <summary>
        /// Creates a new <seealso cref="PuppeteerTimeout"/>
        /// </summary>
        public PuppeteerTimeout() : base(System.Diagnostics.Debugger.IsAttached ? TestConstants.DebuggerAttachedTestTimeout : TestConstants.DefaultTestTimeout)
        {
        }
    }
}
