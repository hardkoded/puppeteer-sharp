using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;

namespace PuppeteerSharp.Tests.NavigationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextReloadTests : DevToolsContextBaseTest
    {
        public DevToolsContextReloadTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("navigation.spec.ts", "Page.reload", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.EvaluateFunctionAsync("() => (globalThis._foo = 10)");
            await DevToolsContext.ReloadAsync();
            Assert.Null(await DevToolsContext.EvaluateFunctionAsync("() => globalThis._foo"));
        }
    }
}
