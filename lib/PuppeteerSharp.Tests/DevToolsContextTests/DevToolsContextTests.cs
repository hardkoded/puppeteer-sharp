using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using CefSharp.Puppeteer;
using Xunit;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Http;

namespace PuppeteerSharp.Tests.DevToolsContextTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextTests : DevToolsContextBaseTest
    {
        public DevToolsContextTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact]
        public async Task ShouldReturnSameContextForMultipleCallsWhenNotDisposed()
        {
            var ctx = await ChromiumWebBrowser.CreateDevToolsContextAsync();

            Assert.Equal(DevToolsContext, ctx);
        }

        [PuppeteerFact]
        public async Task ShouldReturnDiffContextForMultipleCallsWhenDisposed()
        {
            DevToolsContext.Dispose();

            var ctx = await ChromiumWebBrowser.CreateDevToolsContextAsync();

            Assert.NotEqual(DevToolsContext, ctx);
        }

        [PuppeteerTest("chromiumonly.spec.ts", "Chromium-Specific Page Tests", "Page.setRequestInterception should work with intervention headers")]
        [PuppeteerFact]
        public async Task ShouldWorkWithInterventionHeaders()
        {
            Server.SetRoute("/intervention", context => context.Response.WriteAsync($@"
              <script>
                document.write('<script src=""{TestConstants.CrossProcessHttpPrefix}/intervention.js"">' + '</scr' + 'ipt>');
              </script>
            "));
            Server.SetRedirect("/intervention.js", "/redirect.js");

            string interventionHeader = null;
            Server.SetRoute("/redirect.js", context =>
            {
                interventionHeader = context.Request.Headers["intervention"];
                return context.Response.WriteAsync("console.log(1);");
            });

            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) => await e.Request.ContinueAsync();

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/intervention");

            Assert.Contains("feature/5718547946799104", interventionHeader);
        }
    }
}
