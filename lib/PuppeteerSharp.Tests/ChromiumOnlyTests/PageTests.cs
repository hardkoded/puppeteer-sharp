using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using CefSharp.Puppeteer;
using Xunit;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Http;

namespace PuppeteerSharp.Tests.ChromiumSpecificTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageTests : PuppeteerPageBaseTest
    {
        public PageTests(ITestOutputHelper output) : base(output)
        {
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
