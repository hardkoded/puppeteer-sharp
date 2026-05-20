using System.Linq;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ChromeLauncherTests
{
    public class DefaultArgsTests : PuppeteerPageBaseTest
    {
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
    }
}
