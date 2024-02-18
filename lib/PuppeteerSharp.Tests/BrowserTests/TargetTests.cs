using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class TargetTests : PuppeteerBrowserBaseTest
    {
        public TargetTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("browser.spec", "Browser.target", "should return browser target")]
        public void ShouldReturnBrowserTarget()
            => Assert.AreEqual(TargetType.Browser, Browser.Target.Type);
    }
}
