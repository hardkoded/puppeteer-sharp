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

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.emulateMediaType", "should work")]
        public async Task ShouldWork()
        {
            Assert.That(await Page.EvaluateExpressionAsync<bool>("matchMedia('screen').matches"), Is.True);
            Assert.That(await Page.EvaluateExpressionAsync<bool>("matchMedia('print').matches"), Is.False);
            await Page.EmulateMediaTypeAsync(MediaType.Print);
            Assert.That(await Page.EvaluateExpressionAsync<bool>("matchMedia('screen').matches"), Is.False);
            Assert.That(await Page.EvaluateExpressionAsync<bool>("matchMedia('print').matches"), Is.True);
            await Page.EmulateMediaTypeAsync(MediaType.None);
            Assert.That(await Page.EvaluateExpressionAsync<bool>("matchMedia('screen').matches"), Is.True);
            Assert.That(await Page.EvaluateExpressionAsync<bool>("matchMedia('print').matches"), Is.False);
        }
    }
}
