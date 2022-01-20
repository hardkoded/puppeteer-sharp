using System.Threading.Tasks;
using CefSharp.Puppeteer.Media;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.EmulationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class EmulateMediaTypeTests : PuppeteerPageBaseTest
    {
        public EmulateMediaTypeTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("emulation.spec.ts", "Page.emulateMediaType", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            Assert.True(await DevToolsContext.EvaluateExpressionAsync<bool>("matchMedia('screen').matches"));
            Assert.False(await DevToolsContext.EvaluateExpressionAsync<bool>("matchMedia('print').matches"));
            await DevToolsContext.EmulateMediaTypeAsync(MediaType.Print);
            Assert.False(await DevToolsContext.EvaluateExpressionAsync<bool>("matchMedia('screen').matches"));
            Assert.True(await DevToolsContext.EvaluateExpressionAsync<bool>("matchMedia('print').matches"));
            await DevToolsContext.EmulateMediaTypeAsync(MediaType.None);
            Assert.True(await DevToolsContext.EvaluateExpressionAsync<bool>("matchMedia('screen').matches"));
            Assert.False(await DevToolsContext.EvaluateExpressionAsync<bool>("matchMedia('print').matches"));
        }
    }
}
