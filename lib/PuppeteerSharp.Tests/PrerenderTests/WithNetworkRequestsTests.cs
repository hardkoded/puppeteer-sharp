using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PrerenderTests;

public class WithNetworkRequestsTests : PuppeteerPageBaseTest
{
    [Test, PuppeteerTest("prerender.spec", "Prerender with network requests", "can receive requests from the prerendered page")]
    public async Task CanNavigateToAPrerenderedPageViaInput()
    {
        var urls = new List<string>();
        Page.Request += (_, e) => urls.Add(e.Request.Url);
        await Page.GoToAsync(TestConstants.ServerUrl + "/prerender/index.html");

        var button = await Page.WaitForSelectorAsync("button");
        await button.ClickAsync();

        var mainFrame = Page.MainFrame;
        var link = await mainFrame.WaitForSelectorAsync("a");
        await Task.WhenAll(
            mainFrame.WaitForNavigationAsync(),
            link.ClickAsync()
        );

        Assert.That(Page.MainFrame, Is.SameAs(mainFrame));
        Assert.That(await mainFrame.EvaluateExpressionAsync<string>("document.body.innerText"), Is.EqualTo("target"));
        Assert.That(Page.MainFrame, Is.SameAs(mainFrame));

        Assert.That(urls.Exists(url => url.Contains("prerender/target.html")), Is.True);
        Assert.That(urls.Exists(url => url.Contains("prerender/index.html")), Is.True);
        Assert.That(urls.Exists(url => url.Contains("prerender/target.html?fromPrerendered")), Is.True);
    }
}
