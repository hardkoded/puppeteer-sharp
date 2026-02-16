using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class CreateCDPSessionTests : PuppeteerBrowserBaseTest
    {
        [Test, Retry(2), PuppeteerTest("puppeteer-sharp", "Browser.CreateCDPSessionAsync", "should work")]
        public async Task ShouldWork()
        {
            var session = await Browser.CreateCDPSessionAsync();
            Assert.That(session, Is.Not.Null);

            var response = await session.SendAsync("Browser.getVersion");
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Value.GetProperty("product").GetString(), Is.Not.Empty);

            await session.DetachAsync();
        }
    }
}
