using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.InterventionHeadersTests;

public class InterventionHeadersTests : PuppeteerPageBaseTest
{
    [Test, PuppeteerTest("interventionHeaders.spec", "Page.setRequestInterception", "should work with intervention headers")]
    public async Task ShouldWorkWithInterventionHeaders()
    {
        Server.SetRoute("/intervention", context => context.Response.WriteAsync($@"
          <script>
            document.write('<script src=""{TestConstants.CrossProcessHttpPrefix}/intervention.js"">' + '</scr' + 'ipt>');
          </script>
        "));
        Server.SetRedirect("/intervention.js", "/redirect.js");

        var redirectRequested = false;
        string interventionHeader = null;
        Server.SetRoute("/redirect.js", context =>
        {
            redirectRequested = true;
            interventionHeader = context.Request.Headers["intervention"];
            return context.Response.WriteAsync("console.log(1);");
        });

        await Page.SetRequestInterceptionAsync(true);
        Page.Request += async (_, e) => await e.Request.ContinueAsync();

        await Page.GoToAsync(TestConstants.ServerUrl + "/intervention");

        // The intercepted document.write() script must still reach the server.
        Assert.That(redirectRequested, Is.True);

        // Chrome 157+ no longer attaches the `Intervention` request header
        // (https://crrev.com/c/8504605). Older versions still do; check for the
        // feature URL substring rather than https://www.chromestatus.com to make it
        // work with Edgium.
        if (!string.IsNullOrEmpty(interventionHeader))
        {
            Assert.That(interventionHeader, Does.Contain("feature/5718547946799104"));
        }
    }
}

