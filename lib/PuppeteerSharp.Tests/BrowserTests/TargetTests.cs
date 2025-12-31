using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class TargetTests : PuppeteerBrowserBaseTest
    {
        public TargetTests() : base()
        {
        }

        [Test, PuppeteerTest("browser.spec", "Browser.target", "should return browser target")]
        public void ShouldReturnBrowserTarget()
            => Assert.That(Browser.Target.Type, Is.EqualTo(TargetType.Browser));
    }
}
