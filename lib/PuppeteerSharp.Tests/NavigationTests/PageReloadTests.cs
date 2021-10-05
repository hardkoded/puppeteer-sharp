using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;

namespace PuppeteerSharp.Tests.NavigationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageReloadTests : PuppeteerPageBaseTest
    {
        public PageReloadTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("navigation.spec.ts", "Page.reload", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync("() => (globalThis._foo = 10)");
            await Page.ReloadAsync();
            Assert.Null(await Page.EvaluateFunctionAsync("() => globalThis._foo"));
        }
    }
}
