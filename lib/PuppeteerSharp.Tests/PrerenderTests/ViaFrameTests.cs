using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.PrerenderTests;

public class ViaFrameTests : PuppeteerPageBaseTest
{
    [PuppeteerTest("prerender.spec.ts", "via frame", "can navigate to a prerendered page via input")]
    [Skip(SkipAttribute.Targets.Firefox)]
    public async Task CanNavigateToAPrerenderedPageViaInput()
    {
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
    }

    [PuppeteerTest("prerender.spec.ts", "via frame", "can navigate to a prerendered page via Puppeteer")]
    [Skip(SkipAttribute.Targets.Firefox)]
    public async Task CanNavigateToAPrerenderedPageViaPuppeteer()
    {
        await Page.GoToAsync(TestConstants.ServerUrl + "/prerender/index.html");

        var button = await Page.WaitForSelectorAsync("button");
        await button.ClickAsync();

        var mainFrame = Page.MainFrame;
        await mainFrame.GoToAsync(TestConstants.ServerUrl + "/prerender/target.html");
        Assert.AreSame(mainFrame, Page.MainFrame);
        Assert.AreEqual("target", await mainFrame.EvaluateExpressionAsync<string>("document.body.innerText"));
        Assert.AreSame(mainFrame, Page.MainFrame);
    }
}
