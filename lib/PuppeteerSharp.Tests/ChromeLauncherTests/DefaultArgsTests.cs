using System.Linq;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ChromeLauncherTests
{
    public class DefaultArgsTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("ChromeLauncher.test.ts", "ChromeLauncher", "disables WebUIOmniboxPopup and WebUIOmniboxAimPopup by default")]
        public void DisablesWebUIOmniboxPopupAndWebUIOmniboxAimPopupByDefault()
        {
            var args = ChromeLauncher.GetDefaultArgs(new LaunchOptions());
            var disableFeaturesFlag = args.FirstOrDefault(arg => arg.StartsWith("--disable-features=", System.StringComparison.Ordinal));
            Assert.That(disableFeaturesFlag, Is.Not.Null);

            var disabledFeatures = disableFeaturesFlag.Split('=')[1].Split(',');
            Assert.That(disabledFeatures, Does.Contain("WebUIOmniboxPopup"));
            Assert.That(disabledFeatures, Does.Contain("WebUIOmniboxAimPopup"));
        }

        [Test, PuppeteerTest("ChromeLauncher.test.ts", "ChromeLauncher", "removes disabled features if they are enabled explicitly")]
        public void RemovesDisabledFeaturesIfTheyAreEnabledExplicitly()
        {
            var args = ChromeLauncher.GetDefaultArgs(new LaunchOptions
            {
                Args = new[] { "--enable-features=Translate" },
            });

            var disableFeaturesFlag = args.FirstOrDefault(arg => arg.StartsWith("--disable-features=", System.StringComparison.Ordinal));
            Assert.That(disableFeaturesFlag, Is.Not.Null);

            var disabledFeatures = disableFeaturesFlag.Split('=')[1].Split(',');
            Assert.That(disabledFeatures, Does.Not.Contain("Translate"));
        }

        [Test, PuppeteerTest("ChromeLauncher.test.ts", "ChromeLauncher", "does not mutate options.args and returns the same result on repeated calls")]
        public void DoesNotMutateOptionsArgsAndReturnsTheSameResultOnRepeatedCalls()
        {
            var args = new[]
            {
                "--no-sandbox",
                "--enable-features=WebGPUUndelayedSwapOnPresentation,Foo",
                "--enable-features=Bar",
                "--disable-features=UserDisabledFeature",
            };
            var options = new LaunchOptions { Args = args };
            var originalArgs = args.ToArray();

            var first = ChromeLauncher.GetDefaultArgs(options);
            var second = ChromeLauncher.GetDefaultArgs(options);

            Assert.That(options.Args, Is.SameAs(args));
            Assert.That(options.Args, Is.EqualTo(originalArgs));
            Assert.That(second, Is.EqualTo(first));
        }

        [Test, PuppeteerTest("ChromeLauncher.test.ts", "ChromeLauncher", "merges multiple --enable-features flags and keeps user --disable-features")]
        public void MergesMultipleEnableFeaturesFlagsAndKeepsUserDisableFeatures()
        {
            var args = ChromeLauncher.GetDefaultArgs(new LaunchOptions
            {
                Args =
                [
                    "--no-sandbox",
                    "--enable-features=WebGPUUndelayedSwapOnPresentation,Foo",
                    "--enable-features=Bar",
                    "--disable-features=UserDisabledFeature",
                ],
            });

            var enableFeatureArgs = args.Where(arg => arg.StartsWith("--enable-features=", System.StringComparison.Ordinal));
            Assert.That(enableFeatureArgs, Is.EqualTo(new[] { "--enable-features=PdfOopif,WebGPUUndelayedSwapOnPresentation,Foo,Bar" }));

            var disableFeatureArgs = args.Where(arg => arg.StartsWith("--disable-features=", System.StringComparison.Ordinal)).ToArray();
            Assert.That(disableFeatureArgs, Has.Length.EqualTo(1));
            Assert.That(disableFeatureArgs[0].Split('=')[1].Split(','), Does.Contain("UserDisabledFeature"));
            Assert.That(args, Does.Contain("--no-sandbox"));
        }
    }
}
