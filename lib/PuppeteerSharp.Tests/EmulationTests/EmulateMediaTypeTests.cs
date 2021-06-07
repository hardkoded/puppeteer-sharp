using System.Threading.Tasks;
using PuppeteerSharp.Media;
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
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWork()
        {
            Assert.True(await Page.EvaluateExpressionAsync<bool>("matchMedia('screen').matches"));
            Assert.False(await Page.EvaluateExpressionAsync<bool>("matchMedia('print').matches"));
            await Page.EmulateMediaTypeAsync(MediaType.Print);
            Assert.False(await Page.EvaluateExpressionAsync<bool>("matchMedia('screen').matches"));
            Assert.True(await Page.EvaluateExpressionAsync<bool>("matchMedia('print').matches"));
            await Page.EmulateMediaTypeAsync(MediaType.None);
            Assert.True(await Page.EvaluateExpressionAsync<bool>("matchMedia('screen').matches"));
            Assert.False(await Page.EvaluateExpressionAsync<bool>("matchMedia('print').matches"));
        }
    }
}
