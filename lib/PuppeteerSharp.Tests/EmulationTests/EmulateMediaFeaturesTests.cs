using System.Threading.Tasks;
using PuppeteerSharp.Media;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.EmulationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class EmulateMediaFeaturesAsyncTests : PuppeteerPageBaseTest
    {
        public EmulateMediaFeaturesAsyncTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("emulation.spec.ts", "Page.emulateMediaFeatures", "should work")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWork()
        {
            await Page.EmulateMediaFeaturesAsync(new MediaFeatureValue[] {
                new MediaFeatureValue { MediaFeature = MediaFeature.PrefersReducedMotion, Value = "reduce" },
            });
            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: reduce)').matches"));
            Assert.False(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: no-preference)').matches"));
            await Page.EmulateMediaFeaturesAsync(new MediaFeatureValue[] {
                new MediaFeatureValue { MediaFeature = MediaFeature.PrefersColorScheme, Value = "light" },
            });
            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: light)').matches"));
            Assert.False(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: dark)').matches"));
            Assert.False(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches"));
            await Page.EmulateMediaFeaturesAsync(new MediaFeatureValue[] {
                new MediaFeatureValue { MediaFeature = MediaFeature.PrefersColorScheme, Value = "dark" },
            });
            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: dark)').matches"));
            Assert.False(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: light)').matches"));
            Assert.False(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches"));
            await Page.EmulateMediaFeaturesAsync(new MediaFeatureValue[] {
                new MediaFeatureValue { MediaFeature = MediaFeature.PrefersReducedMotion, Value = "reduce" },
                new MediaFeatureValue { MediaFeature = MediaFeature.PrefersColorScheme, Value = "light" },
            });
            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: reduce)').matches"));
            Assert.False(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: no-preference)').matches"));
            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: light)').matches"));
            Assert.False(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: dark)').matches"));
            Assert.False(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches"));
        }
    }
}
