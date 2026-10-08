using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class EmulateMediaFeaturesAsyncTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("emulation.spec", "Emulation Page.emulateMediaFeatures", "should work")]
        public async Task ShouldWork()
        {
            await Page.EmulateMediaFeaturesAsync(new MediaFeatureValue[] {
                new MediaFeatureValue { MediaFeature = MediaFeature.PrefersReducedMotion, Value = "reduce" },
            });
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: reduce)').matches"), Is.True);
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: no-preference)').matches"), Is.False);
            await Page.EmulateMediaFeaturesAsync(new MediaFeatureValue[] {
                new MediaFeatureValue { MediaFeature = MediaFeature.PrefersColorScheme, Value = "light" },
            });
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: light)').matches"), Is.True);
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: dark)').matches"), Is.False);
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches"), Is.False);
            await Page.EmulateMediaFeaturesAsync(new MediaFeatureValue[] {
                new MediaFeatureValue { MediaFeature = MediaFeature.PrefersColorScheme, Value = "dark" },
            });
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: dark)').matches"), Is.True);
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: light)').matches"), Is.False);
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches"), Is.False);
            await Page.EmulateMediaFeaturesAsync(new MediaFeatureValue[] {
                new MediaFeatureValue { MediaFeature = MediaFeature.PrefersReducedMotion, Value = "reduce" },
                new MediaFeatureValue { MediaFeature = MediaFeature.PrefersColorScheme, Value = "light" },
            });
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: reduce)').matches"), Is.True);
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: no-preference)').matches"), Is.False);
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: light)').matches"), Is.True);
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: dark)').matches"), Is.False);
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches"), Is.False);
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.emulateMediaFeatures", "should work with prefers-contrast")]
        public async Task ShouldWorkWithPrefersContrast()
        {
            await Page.EmulateMediaFeaturesAsync(new MediaFeatureValue[] {
                new MediaFeatureValue { MediaFeature = MediaFeature.PrefersContrast, Value = "more" },
            });
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-contrast: more)').matches"), Is.True);
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-contrast: no-preference)').matches"), Is.False);
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.emulateMediaFeatures", "should work with prefers-reduced-transparency")]
        public async Task ShouldWorkWithPrefersReducedTransparency()
        {
            await Page.EmulateMediaFeaturesAsync(new MediaFeatureValue[] {
                new MediaFeatureValue { MediaFeature = MediaFeature.PrefersReducedTransparency, Value = "reduce" },
            });
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-transparency: reduce)').matches"), Is.True);
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-transparency: no-preference)').matches"), Is.False);
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.emulateMediaFeatures", "should work with forced-colors")]
        public async Task ShouldWorkWithForcedColors()
        {
            await Page.EmulateMediaFeaturesAsync(new MediaFeatureValue[] {
                new MediaFeatureValue { MediaFeature = MediaFeature.ForcedColors, Value = "active" },
            });
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(forced-colors: active)').matches"), Is.True);
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => matchMedia('(forced-colors: none)').matches"), Is.False);
        }
    }
}
