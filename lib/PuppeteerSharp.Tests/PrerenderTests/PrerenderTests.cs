using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.PrerenderTests;

public class PrerenderTests : PuppeteerPageBaseTest
{
    [PuppeteerTest("prerender.spec.ts", "Prerender", "can navigate to a prerendered page via input")]
    [Skip(SkipAttribute.Targets.Firefox)]
    public async Task CanNavigateToAPrerenderedPageViaInput()
    {
        await Page.GoToAsync(TestConstants.ServerUrl + "/prerender/index.html");

        var button = await Page.WaitForSelectorAsync("button");
        await button.ClickAsync();

        var link = await Page.WaitForSelectorAsync("a");
        await Task.WhenAll(
            Page.WaitForNavigationAsync(),
            link.ClickAsync()
        );

        Assert.AreEqual("target", await Page.EvaluateExpressionAsync<string>("document.body.innerText"));
    }

    [PuppeteerTest("prerender.spec.ts", "Prerender", "can navigate to a prerendered page via Puppeteer")]
    [Skip(SkipAttribute.Targets.Firefox)]
    public async Task CanNavigateToAPrerenderedPageViaPuppeteer()
    {
        await Page.GoToAsync(TestConstants.ServerUrl + "/prerender/index.html");

        var button = await Page.WaitForSelectorAsync("button");
        await button.ClickAsync();


        await Page.GoToAsync(TestConstants.ServerUrl + "/prerender/target.html");
        Assert.AreEqual("target", await Page.EvaluateExpressionAsync<string>("document.body.innerText"));
    }
}
