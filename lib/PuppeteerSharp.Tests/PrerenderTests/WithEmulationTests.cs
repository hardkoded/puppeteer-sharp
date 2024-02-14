using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.PrerenderTests;

public class WithEmulationTests : PuppeteerPageBaseTest
{
    [PuppeteerTest("prerender.spec.ts", "Prerender with emulation", "can configure viewport for prerendered pages")]
    [Skip(SkipAttribute.Targets.Firefox)]
    public async Task CanConfigureViewportForPrerenderedPages()
    {
        await Page.SetViewportAsync(new ViewPortOptions()
        {
            Width = 300,
            Height = 400,
        });

        await Page.GoToAsync(TestConstants.ServerUrl + "/prerender/index.html");

        var button = await Page.WaitForSelectorAsync("button");
        await button.ClickAsync();

        var mainFrame = Page.MainFrame;
        var link = await mainFrame.WaitForSelectorAsync("a");
        await Task.WhenAll(
            mainFrame.WaitForNavigationAsync(),
            link.ClickAsync()
        );

        var result = await Page.EvaluateFunctionAsync<decimal[]>(@"() => {
            return [
                document.documentElement.clientWidth,
                document.documentElement.clientHeight,
                window.devicePixelRatio,
            ];
        }");

        Assert.AreEqual(300 * result[2], result[0]);
        Assert.AreEqual(400 * result[2], result[1]);
    }
}
