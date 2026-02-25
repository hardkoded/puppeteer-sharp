using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.UserDataDirTests
{
    public class UserDataDirTests : PuppeteerBaseTest
    {
        [Test, PuppeteerTest("userDataDir.spec", "userDataDir", "should not launch the browser twice with the same userDataDir with pipe=false")]
        public async Task ShouldNotLaunchBrowserTwiceWithSameUserDataDir()
        {
            using var userDataDir = new TempDirectory();
            var options = TestConstants.DefaultBrowserOptions();
            options.UserDataDir = userDataDir.Path;

            var launcher = new Launcher(TestConstants.LoggerFactory);
            await using var browser = await launcher.LaunchAsync(options);

            // Open a page to make sure its functional.
            await browser.NewPageAsync();
            Assert.That(Directory.GetFiles(userDataDir.Path).Length, Is.GreaterThan(0));

            var secondLauncher = new Launcher(TestConstants.LoggerFactory);
            var exception = Assert.ThrowsAsync<ProcessException>(async () =>
            {
                await using var secondBrowser = await secondLauncher.LaunchAsync(options);
            });
            Assert.That(exception.Message, Does.StartWith("The browser is already running for"));

            await browser.CloseAsync();
        }
    }
}
