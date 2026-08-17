using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.FollowSymlinksTests
{
    public class FollowSymlinksTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("followSymlinks.test", "followSymlinks when followSymlinks is false", "should reject screencast when overwrite is false and file exists")]
        public async Task ShouldRejectScreencastWhenOverwriteIsFalseAndFileExists()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            var tmpDir = Path.Combine(Path.GetTempPath(), "pptr-screencast-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tmpDir);
            var targetFile = Path.Combine(tmpDir, "output.webm");
            File.WriteAllText(targetFile, "placeholder");

            try
            {
                var exception = Assert.ThrowsAsync<IOException>(
                    () => Page.ScreencastAsync(new ScreencastOptions
                    {
                        Path = targetFile,
                        Overwrite = false,
                    }));
                Assert.That(exception, Is.Not.Null);
            }
            finally
            {
                try
                {
                    if (Directory.Exists(tmpDir))
                    {
                        Directory.Delete(tmpDir, true);
                    }
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
    }
}
