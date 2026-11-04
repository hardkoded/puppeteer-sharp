using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Input;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageRecordTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("page.test", "Page Page.record", "should record page")]
        public async Task ShouldRecordPage()
        {
            var filePath = Path.Combine(Path.GetTempPath(), $"test-video-{Guid.NewGuid()}.mp4");

            try
            {
                var recording = await Page.RecordAsync(new RecordOptions
                {
                    Path = filePath,
                });

                await Page.GoToAsync("data:text/html,<input>");
                var input = await Page.WaitForSelectorAsync("input");
                await input.TypeAsync("ab", new TypeOptions { Delay = 100 });

                await recording.StopAsync();

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

        [Test, PuppeteerTest("ScreenRecording.test", "ScreenRecording", "should validate options")]
        public async Task ShouldValidateOptions()
        {
            Assert.ThrowsAsync<PuppeteerException>(() => Page.RecordAsync(new RecordOptions { MaxWidth = 0 }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.RecordAsync(new RecordOptions { MaxWidth = -10 }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.RecordAsync(new RecordOptions { MaxHeight = 0 }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.RecordAsync(new RecordOptions { MaxHeight = -10 }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.RecordAsync(new RecordOptions { FrameRate = 0 }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.RecordAsync(new RecordOptions { FrameRate = -5 }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.RecordAsync(new RecordOptions { Fps = 0 }));
            Assert.ThrowsAsync<PuppeteerException>(() => Page.RecordAsync(new RecordOptions { Fps = -5 }));
        }
    }
}
