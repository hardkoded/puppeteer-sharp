using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.FollowSymlinksTests
{
    public class FollowSymlinksTests : PuppeteerPageBaseTest
    {
        private string _tmpDir;
        private string _scriptFile;
        private string _scriptSymlink;
        private string _styleFile;
        private string _styleSymlink;
        private bool _symlinksSupported = true;

        [SetUp]
        public void CreateTempFiles()
        {
            _tmpDir = Path.Combine(Path.GetTempPath(), "pptr-symlink-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tmpDir);

            _scriptFile = Path.Combine(_tmpDir, "script.js");
            _scriptSymlink = Path.Combine(_tmpDir, "script-link.js");
            File.WriteAllText(_scriptFile, "window.__injected = 123;");

            try
            {
                File.CreateSymbolicLink(_scriptSymlink, _scriptFile);
                _symlinksSupported = true;
            }
            catch
            {
                _symlinksSupported = false;
                return;
            }

            _styleFile = Path.Combine(_tmpDir, "style.css");
            _styleSymlink = Path.Combine(_tmpDir, "style-link.css");
            File.WriteAllText(_styleFile, "body { background-color: rgb(0, 255, 0); }");
            File.CreateSymbolicLink(_styleSymlink, _styleFile);
        }

        [TearDown]
        public void CleanupTempFiles()
        {
            Puppeteer.SetFollowSymlinks(true);

            try
            {
                if (!string.IsNullOrEmpty(_tmpDir) && Directory.Exists(_tmpDir))
                {
                    Directory.Delete(_tmpDir, true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        [Test, PuppeteerTest("followSymlinks.test", "followSymlinks when followSymlinks is false", "should reject addScriptTag with a symlinked path")]
        public async Task ShouldRejectAddScriptTagWithASymlinkedPath()
        {
            IgnoreIfWindowsOrSymlinksUnsupported();
            Puppeteer.SetFollowSymlinks(false);

            await Page.GoToAsync(TestConstants.EmptyPage);

            var exception = Assert.ThrowsAsync<IOException>(
                () => Page.AddScriptTagAsync(new AddTagOptions { Path = _scriptSymlink }));
            Assert.That(exception, Is.Not.Null);
        }

        [Test, PuppeteerTest("followSymlinks.test", "followSymlinks when followSymlinks is false", "should allow addScriptTag with a regular file path")]
        public async Task ShouldAllowAddScriptTagWithARegularFilePath()
        {
            Puppeteer.SetFollowSymlinks(false);

            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddTagOptions { Path = _scriptFile });
            Assert.That(await Page.EvaluateExpressionAsync<int>("window.__injected"), Is.EqualTo(123));
        }

        [Test, PuppeteerTest("followSymlinks.test", "followSymlinks when followSymlinks is false", "should reject addStyleTag with a symlinked path")]
        public async Task ShouldRejectAddStyleTagWithASymlinkedPath()
        {
            IgnoreIfWindowsOrSymlinksUnsupported();
            Puppeteer.SetFollowSymlinks(false);

            await Page.GoToAsync(TestConstants.EmptyPage);

            var exception = Assert.ThrowsAsync<IOException>(
                () => Page.AddStyleTagAsync(new AddTagOptions { Path = _styleSymlink }));
            Assert.That(exception, Is.Not.Null);
        }

        [Test, PuppeteerTest("followSymlinks.test", "followSymlinks when followSymlinks is false", "should allow addStyleTag with a regular file path")]
        public async Task ShouldAllowAddStyleTagWithARegularFilePath()
        {
            Puppeteer.SetFollowSymlinks(false);

            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddStyleTagAsync(new AddTagOptions { Path = _styleFile });
            Assert.That(
                await Page.EvaluateExpressionAsync<string>(
                    "window.getComputedStyle(document.body).getPropertyValue('background-color')"),
                Is.EqualTo("rgb(0, 255, 0)"));
        }

        [Test, PuppeteerTest("followSymlinks.test", "followSymlinks when followSymlinks is false", "should reject screenshot to an existing symlink path")]
        public async Task ShouldRejectScreenshotToAnExistingSymlinkPath()
        {
            IgnoreIfWindowsOrSymlinksUnsupported();
            Puppeteer.SetFollowSymlinks(false);

            await Page.GoToAsync(TestConstants.EmptyPage);

            var targetFile = Path.Combine(_tmpDir, "screenshot.png");
            var linkFile = Path.Combine(_tmpDir, "screenshot-link.png");
            File.WriteAllText(targetFile, "placeholder");
            File.CreateSymbolicLink(linkFile, targetFile);

            var exception = Assert.ThrowsAsync<IOException>(() => Page.ScreenshotAsync(linkFile));
            Assert.That(exception, Is.Not.Null);
        }

        [Test, PuppeteerTest("followSymlinks.test", "followSymlinks when followSymlinks is false", "should reject pdf to an existing symlink path")]
        public async Task ShouldRejectPdfToAnExistingSymlinkPath()
        {
            IgnoreIfWindowsOrSymlinksUnsupported();
            Puppeteer.SetFollowSymlinks(false);

            await Page.GoToAsync(TestConstants.EmptyPage);

            var targetFile = Path.Combine(_tmpDir, "output.pdf");
            var linkFile = Path.Combine(_tmpDir, "output-link.pdf");
            File.WriteAllText(targetFile, "placeholder");
            File.CreateSymbolicLink(linkFile, targetFile);

            var exception = Assert.ThrowsAsync<IOException>(() => Page.PdfAsync(linkFile));
            Assert.That(exception, Is.Not.Null);
        }

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

        [Test, PuppeteerTest("followSymlinks.test", "followSymlinks when followSymlinks is false", "should reject screencast to an existing symlink path")]
        public async Task ShouldRejectScreencastToAnExistingSymlinkPath()
        {
            IgnoreIfWindowsOrSymlinksUnsupported();
            Puppeteer.SetFollowSymlinks(false);

            await Page.GoToAsync(TestConstants.EmptyPage);

            var targetFile = Path.Combine(_tmpDir, "output.webm");
            var linkFile = Path.Combine(_tmpDir, "output-link.webm");
            File.WriteAllText(targetFile, "placeholder");
            File.CreateSymbolicLink(linkFile, targetFile);

            var exception = Assert.ThrowsAsync<IOException>(
                () => Page.ScreencastAsync(new ScreencastOptions { Path = linkFile }));
            Assert.That(exception, Is.Not.Null);
        }

        [Test, PuppeteerTest("followSymlinks.test", "followSymlinks when followSymlinks is true (default)", "should allow addScriptTag with a symlinked path")]
        public async Task ShouldAllowAddScriptTagWithASymlinkedPath()
        {
            IgnoreIfSymlinksUnsupported();

            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddTagOptions { Path = _scriptSymlink });
            Assert.That(await Page.EvaluateExpressionAsync<int>("window.__injected"), Is.EqualTo(123));
        }

        [Test, PuppeteerTest("followSymlinks.test", "followSymlinks when followSymlinks is true (default)", "should allow addStyleTag with a symlinked path")]
        public async Task ShouldAllowAddStyleTagWithASymlinkedPath()
        {
            IgnoreIfSymlinksUnsupported();

            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddStyleTagAsync(new AddTagOptions { Path = _styleSymlink });
            Assert.That(
                await Page.EvaluateExpressionAsync<string>(
                    "window.getComputedStyle(document.body).getPropertyValue('background-color')"),
                Is.EqualTo("rgb(0, 255, 0)"));
        }

        [Test, PuppeteerTest("followSymlinks.test", "followSymlinks when followSymlinks is true (default)", "should allow screenshot to a symlink path")]
        public async Task ShouldAllowScreenshotToASymlinkPath()
        {
            IgnoreIfSymlinksUnsupported();

            await Page.GoToAsync(TestConstants.EmptyPage);

            var targetFile = Path.Combine(_tmpDir, "screenshot-target.png");
            var linkFile = Path.Combine(_tmpDir, "screenshot-default-link.png");
            File.WriteAllText(targetFile, "placeholder");
            File.CreateSymbolicLink(linkFile, targetFile);

            await Page.ScreenshotAsync(linkFile);
            Assert.That(new FileInfo(targetFile).Length, Is.GreaterThan(0));
        }

        private void IgnoreIfWindowsOrSymlinksUnsupported()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Ignore("Symlink rejection is not enforced on Windows");
            }

            IgnoreIfSymlinksUnsupported();
        }

        private void IgnoreIfSymlinksUnsupported()
        {
            if (!_symlinksSupported)
            {
                Assert.Ignore("Creating symbolic links is not supported on this system");
            }
        }
    }
}
