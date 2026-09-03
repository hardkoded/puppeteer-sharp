using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Linux;
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

        [Test, PuppeteerTest("userDataDir.spec", "userDataDir", "should not launch the browser twice with the same userDataDir with pipe=true")]
        public async Task ShouldNotLaunchBrowserTwiceWithSameUserDataDirWithPipe()
        {
            using var userDataDir = new TempDirectory();
            var options = TestConstants.DefaultBrowserOptions();
            options.UserDataDir = userDataDir.Path;
            options.Pipe = true;

            var launcher = new Launcher(TestConstants.LoggerFactory);
            await using var browser = await launcher.LaunchAsync(options);

            // Open a page to make sure its functional.
            await browser.NewPageAsync();
            Assert.That(Directory.GetFiles(userDataDir.Path).Length, Is.GreaterThan(0));

            var secondLauncher = new Launcher(TestConstants.LoggerFactory);
            Assert.ThrowsAsync<ProcessException>(async () =>
            {
                await using var secondBrowser = await secondLauncher.LaunchAsync(options);
            });

            await browser.CloseAsync();
        }

        [Test, PuppeteerTest("userDataDir.test", "userDataDir", "should report a permission error when the userDataDir is not writable")]
        public async Task ShouldReportPermissionErrorWhenUserDataDirIsNotWritable()
        {
            // Windows ignores the read-only bit for the directory owner, and root
            // bypasses the write check entirely, so neither can observe the failure.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || IsRunningAsRoot())
            {
                Assert.Ignore("Windows and root cannot observe unwritable userDataDir failures.");
            }

            using var userDataDir = new TempDirectory();
            const FileAccessPermissions readOnlyPermissions =
                FileAccessPermissions.UserRead | FileAccessPermissions.UserExecute |
                FileAccessPermissions.GroupRead | FileAccessPermissions.GroupExecute |
                FileAccessPermissions.OtherRead | FileAccessPermissions.OtherExecute;
            const FileAccessPermissions writablePermissions =
                FileAccessPermissions.UserReadWriteExecute |
                FileAccessPermissions.GroupReadWriteExecute |
                FileAccessPermissions.OtherReadWriteExecute;

            LinuxSysCall.Chmod(userDataDir.Path, readOnlyPermissions);
            try
            {
                var options = TestConstants.DefaultBrowserOptions();
                options.UserDataDir = userDataDir.Path;

                var launcher = new Launcher(TestConstants.LoggerFactory);
                await using var browser = await launcher.LaunchAsync(options);
                Assert.Fail("Not reached");
            }
            catch (ProcessException ex)
            {
                Assert.That(ex.Message, Does.StartWith("The browser cannot write to"));
            }
            finally
            {
                LinuxSysCall.Chmod(userDataDir.Path, writablePermissions);
            }
        }

        private static bool IsRunningAsRoot()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) &&
                !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return false;
            }

            return getuid() == 0;
        }

        [DllImport("libc")]
        private static extern uint getuid();
    }
}
