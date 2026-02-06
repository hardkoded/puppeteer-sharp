using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class TargetTests : PuppeteerBrowserBaseTest
    {
        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.target", "should return browser target")]
        public void ShouldReturnBrowserTarget()
            => Assert.That(Browser.Target.Type, Is.EqualTo(TargetType.Browser));
    }
}
