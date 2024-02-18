using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Media;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class EmulateMediaTypeTests : PuppeteerPageBaseTest
    {
        public EmulateMediaTypeTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("emulation.spec", "Emulation Page.emulateMediaType", "should work")]
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
