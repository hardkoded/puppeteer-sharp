using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BFCacheTests;

public class BFCacheTests : PuppeteerPageBaseTest
{
    public BFCacheTests()
    {
        DefaultOptions = TestConstants.DefaultBrowserOptions();
        DefaultOptions.IgnoreHTTPSErrors = true;
    }

    [Test, Retry(2), PuppeteerTest("bfcache.spec", "BFCache", "can navigate to a BFCached page")]
    public async Task CanNavigateToABFCachedPage()
    {
        Page.DefaultTimeout = 30_000;
        await Page.GoToAsync(TestConstants.ServerUrl + "/cached/bfcache/index.html");
        await Task.WhenAll(Page.WaitForNavigationAsync(), Page.ClickAsync("a"));

        StringAssert.Contains("target", Page.Url);
        await Task.WhenAll(Page.WaitForNavigationAsync(), Page.GoBackAsync());
        Assert.AreEqual("BFCachednext", await Page.EvaluateExpressionAsync<string>("document.body.innerText"));
    }
}
