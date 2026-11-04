using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ScreencastTests
{
    public class PageScreencastTests : PuppeteerBrowserContextBaseTest
    {
        [Test, PuppeteerTest("puppeteer-sharp", "Screencast", "can start screencast")]
        public async Task CanStartScreencast()
        {
            await using var page = await Context.NewPageAsync();
            await page.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });

            var screencastFramesReceived = 0;
            var frameReceived = new TaskCompletionSource<bool>();
            page.Client.MessageReceived += async (_, e) =>
            {
                if (e.MessageID == "Page.screencastFrame")
                {
                    Assert.That(e.MessageData.GetProperty("data").GetString(), Is.Not.Null.Or.Empty);
                    Interlocked.Increment(ref screencastFramesReceived);
                    frameReceived.TrySetResult(true);

                    // acknowledge frame
                    await page.Client.SendAsync("Page.screencastFrameAck", new
                    {
                        sessionId = e.MessageData.GetProperty("sessionId").GetInt32(),
                    });
                }
            };

            await page.Client.SendAsync("Page.startScreencast", new
            {
                format = "png",
                everyNthFrame = 1
            });

            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");

            // Wait for at least one frame to be received before stopping
            await frameReceived.Task.WaitAsync(TimeSpan.FromSeconds(5));

            await page.Client.SendAsync("Page.stopScreencast");

            Assert.That(screencastFramesReceived, Is.GreaterThan(0));
        }
    }
}
