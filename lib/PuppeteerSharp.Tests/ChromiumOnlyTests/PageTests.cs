using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.ChromiumSpecificTests
{
    public class PageTests : PuppeteerPageBaseTest
    {
        public PageTests(): base()
        {
        }

        [PuppeteerTest("chromiumonly.spec.ts", "Chromium-Specific Page Tests", "Page.setRequestInterception should work with intervention headers")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) => await e.Request.ContinueAsync();

            await Page.GoToAsync(TestConstants.ServerUrl + "/intervention");

            StringAssert.Contains("feature/5718547946799104", interventionHeader);
        }
    }
}
