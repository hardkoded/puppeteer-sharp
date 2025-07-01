using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class ProcessTests : PuppeteerBrowserBaseTest
    {
        public ProcessTests() : base() { }

        [Test, PuppeteerTest("browser.spec", "Browser.process", "should return child_process instance")]
        public void ShouldReturnProcessInstance()
        {
            var process = Browser.Process;
            Assert.That(process.Id, Is.GreaterThan(0));
        }

        [Test, PuppeteerTest("browser.spec", "Browser.process", "should not return child_process for remote browser")]
        public async Task ShouldNotReturnChildProcessForRemoteBrowser()
        {
            var browserWSEndpoint = Browser.WebSocketEndpoint;
            var remoteBrowser = await Puppeteer.ConnectAsync(
                new ConnectOptions { BrowserWSEndpoint = browserWSEndpoint }, TestConstants.LoggerFactory);
            Assert.That(remoteBrowser.Process, Is.Null);
            remoteBrowser.Disconnect();
        }
    }
}
