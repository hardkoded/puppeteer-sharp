using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Input;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ScreencastTests
{
    public class ScreencastsTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("screencast.spec", "Screencasts Page.screencast", "should work")]
        public async Task ShouldWork()
        {
            var filePath = Path.Combine(Path.GetTempPath(), $"test-video-{Guid.NewGuid()}.webm");

            try
            {
                var recorder = await Page.ScreencastAsync(new ScreencastOptions
                {
                    Path = filePath,
                    Scale = 0.5m,
                    Crop = new BoundingBox(0, 0, 100, 100),
                    Speed = 0.5m,
                });

                await Page.GoToAsync("data:text/html,<input>");
                var input = await Page.WaitForSelectorAsync("input");
                await input.TypeAsync("ab", new TypeOptions { Delay = 100 });

                await recorder.StopAsync();

                Assert.That(new FileInfo(filePath).Length, Is.GreaterThan(0));
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Test, PuppeteerTest("screencast.spec", "Screencasts Page.screencast", "should work concurrently")]
        public async Task ShouldWorkConcurrently()
        {
            var filePath1 = Path.Combine(Path.GetTempPath(), $"test-video-{Guid.NewGuid()}.webm");
            var filePath2 = Path.Combine(Path.GetTempPath(), $"test-video-{Guid.NewGuid()}.webm");

            try
            {
                var recorder = await Page.ScreencastAsync(new ScreencastOptions { Path = filePath1 });
                var recorder2 = await Page.ScreencastAsync(new ScreencastOptions { Path = filePath2 });

                await Page.GoToAsync("data:text/html,<input>");
                var input = await Page.WaitForSelectorAsync("input");

                await input.TypeAsync("ab", new TypeOptions { Delay = 100 });
                await recorder.StopAsync();

                await input.TypeAsync("ab", new TypeOptions { Delay = 100 });
                await recorder2.StopAsync();

                // Since file2 spent about double the time of file1 recording, so file2
                // should be around double the size of file1.
                var ratio = (double)new FileInfo(filePath2).Length / new FileInfo(filePath1).Length;

                // We use a range because we cannot be precise.
                const double delta = 1.3;
                Assert.That(ratio, Is.GreaterThan(2 - delta));
                Assert.That(ratio, Is.LessThan(2 + delta));
            }
            finally
            {
                if (File.Exists(filePath1))
                {
                    File.Delete(filePath1);
                }

                if (File.Exists(filePath2))
                {
                    File.Delete(filePath2);
                }
            }
        }

        [Test, PuppeteerTest("screencast.spec", "Screencasts Page.screencast", "should validate options")]
        public async Task ShouldValidateOptions()
        {
            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Scale = 0 }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Scale = -1 }));

            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Speed = 0 }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Speed = -1 }));

            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions
            {
                Crop = new BoundingBox(0, 0, 0, 1),
            }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions
            {
                Crop = new BoundingBox(0, 0, 1, 0),
            }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions
            {
                Crop = new BoundingBox(-1, 0, 1, 1),
            }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions
            {
                Crop = new BoundingBox(0, -1, 1, 1),
            }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions
            {
                Crop = new BoundingBox(0, 0, 10000, 1),
            }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions
            {
                Crop = new BoundingBox(0, 0, 1, 10000),
            }));

            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Format = "gif" }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Format = "webm" }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Format = "mp4" }));

            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Fps = 0 }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Fps = -1 }));

            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Loop = 0 }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Loop = -1 }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Loop = double.PositiveInfinity }));

            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Delay = 0 }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Delay = -1 }));

            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Quality = 0 }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Quality = -1 }));

            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Colors = 0 }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Colors = -1 }));

            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Path = "test.webm" }));

            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Overwrite = true }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { Overwrite = false }));

            Assert.ThrowsAsync<PuppeteerException>(() => Page.ScreencastAsync(new ScreencastOptions { FfmpegPath = "non-existent-path" }));
        }
    }
}
