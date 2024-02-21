using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PrerenderTests;

public class ViaFrameTests : PuppeteerPageBaseTest
{
    [Test, Retry(2), PuppeteerTest("prerender.spec", "Prerender via frame", "can navigate to a prerendered page via input")]
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

    [Test, Retry(2), PuppeteerTest("prerender.spec", "Prerender via frame", "can navigate to a prerendered page via Puppeteer")]
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
