using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class TargetTests : PuppeteerBrowserBaseTest
    {
        public TargetTests() : base()
        {
        }

        [Test, PuppeteerTest("browser.spec.ts", "Browser.target", "should return browser target")]
        [PuppeteerTimeout]
        public void ShouldReturnBrowserTarget()
            => Assert.AreEqual(TargetType.Browser, Browser.Target.Type);
    }
}
