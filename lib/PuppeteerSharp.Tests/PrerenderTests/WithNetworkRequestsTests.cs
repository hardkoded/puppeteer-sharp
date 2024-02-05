using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.PrerenderTests;

public class WithNetworkRequestsTests : PuppeteerPageBaseTest
{
    [PuppeteerTest("prerender.spec.ts", "with network requests", "can receive requests from the prerendered page")]
    [Skip(SkipAttribute.Targets.Firefox)]
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

        Assert.AreSame(mainFrame, Page.MainFrame);
        Assert.AreEqual("target", await mainFrame.EvaluateExpressionAsync<string>("document.body.innerText"));
        Assert.AreSame(mainFrame, Page.MainFrame);

        Assert.True(urls.Exists(url => url.Contains("prerender/target.html")));
        Assert.True(urls.Exists(url => url.Contains("prerender/index.html")));
        Assert.True(urls.Exists(url => url.Contains("prerender/target.html?fromPrerendered")));
    }
}
