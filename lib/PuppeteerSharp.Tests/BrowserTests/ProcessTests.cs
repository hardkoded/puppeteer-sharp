using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class ProcessTests : PuppeteerBrowserBaseTest
    {
        public ProcessTests() : base() { }

        [PuppeteerTest("browser.spec.ts", "Browser.process", "should return child_process instance")]
        [PuppeteerTimeout]
        public void ShouldReturnProcessInstance()
        {
            var process = Browser.Process;
            Assert.True(process.Id > 0);
        }

        [PuppeteerTest("browser.spec.ts", "Browser.process", "should not return child_process for remote browser")]
        [PuppeteerTimeout]
        public async Task ShouldNotReturnChildProcessForRemoteBrowser()
        {
            var browserWSEndpoint = Browser.WebSocketEndpoint;
            var remoteBrowser = await Puppeteer.ConnectAsync(
                new ConnectOptions { BrowserWSEndpoint = browserWSEndpoint }, TestConstants.LoggerFactory);
            Assert.Null(remoteBrowser.Process);
            remoteBrowser.Disconnect();
        }
    }
}
