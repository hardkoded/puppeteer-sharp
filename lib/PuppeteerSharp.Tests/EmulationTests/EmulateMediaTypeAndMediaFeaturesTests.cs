using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Media;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class EmulateMediaTypeAndMediaFeaturesTests : PuppeteerPageBaseTest
    {
        [
            Test,
            PuppeteerTest(
                "emulation.spec",
                "Emulation Page.emulateMediaType and Page.emulateMediaFeatures",
                "should not reset each other"
            )
        ]
        public async Task ShouldNotResetEachOther()
        {
            await Page.EmulateMediaTypeAsync(MediaType.Print);
            await Page.EmulateMediaFeaturesAsync(
                new MediaFeatureValue[]
                {
                    new MediaFeatureValue
                    {
                        MediaFeature = MediaFeature.PrefersReducedMotion,
                        Value = "reduce",
                    },
                }
            );
            Assert.That(
                await Page.EvaluateFunctionAsync<bool[]>(
                    @"() => {
                        return [
                            matchMedia('print').matches,
                            matchMedia('(prefers-reduced-motion: reduce)').matches,
                        ];
                    }"
                ),
                Is.EqualTo(new[] { true, true })
            );
            await Page.EmulateMediaTypeAsync(MediaType.Screen);
            Assert.That(
                await Page.EvaluateFunctionAsync<bool[]>(
                    @"() => {
                        return [
                            matchMedia('print').matches,
                            matchMedia('(prefers-reduced-motion: reduce)').matches,
                        ];
                    }"
                ),
                Is.EqualTo(new[] { false, true })
            );
        }
    }
}
