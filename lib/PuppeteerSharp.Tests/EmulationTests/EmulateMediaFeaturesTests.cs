using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using CefSharp.DevTools.Dom.Media;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.EmulationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class EmulateMediaFeaturesAsyncTests : DevToolsContextBaseTest
    {
        public EmulateMediaFeaturesAsyncTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("emulation.spec.ts", "Page.emulateMediaFeatures", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.EmulateMediaFeaturesAsync(new MediaFeatureValue[] {
                new MediaFeatureValue { MediaFeature = MediaFeature.PrefersReducedMotion, Value = "reduce" },
            });
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: reduce)').matches"));
            Assert.False(await DevToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: no-preference)').matches"));
            await DevToolsContext.EmulateMediaFeaturesAsync(new MediaFeatureValue[] {
                new MediaFeatureValue { MediaFeature = MediaFeature.PrefersColorScheme, Value = "light" },
            });
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: light)').matches"));
            Assert.False(await DevToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: dark)').matches"));
            Assert.False(await DevToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches"));
            await DevToolsContext.EmulateMediaFeaturesAsync(new MediaFeatureValue[] {
                new MediaFeatureValue { MediaFeature = MediaFeature.PrefersColorScheme, Value = "dark" },
            });
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: dark)').matches"));
            Assert.False(await DevToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: light)').matches"));
            Assert.False(await DevToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches"));
            await DevToolsContext.EmulateMediaFeaturesAsync(new MediaFeatureValue[] {
                new MediaFeatureValue { MediaFeature = MediaFeature.PrefersReducedMotion, Value = "reduce" },
                new MediaFeatureValue { MediaFeature = MediaFeature.PrefersColorScheme, Value = "light" },
            });
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: reduce)').matches"));
            Assert.False(await DevToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: no-preference)').matches"));
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: light)').matches"));
            Assert.False(await DevToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: dark)').matches"));
            Assert.False(await DevToolsContext.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches"));
        }
    }
}
