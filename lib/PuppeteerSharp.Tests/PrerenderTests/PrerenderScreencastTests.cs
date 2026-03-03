using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PrerenderTests;

public class PrerenderScreencastTests : PuppeteerPageBaseTest
{
    [Test, PuppeteerTest("prerender.spec.ts", "Prerender", "can screencast")]
    public async Task CanScreencast()
    {
        var screencastFramesReceived = 0;
        var frameReceived = new TaskCompletionSource<bool>();
        Page.Client.MessageReceived += async (_, e) =>
        {
            if (e.MessageID == "Page.screencastFrame")
            {
                Assert.That(e.MessageData.GetProperty("data").GetString(), Is.Not.Null.Or.Empty);
                Interlocked.Increment(ref screencastFramesReceived);
                frameReceived.TrySetResult(true);

                await Page.Client.SendAsync("Page.screencastFrameAck", new
                {
                    sessionId = e.MessageData.GetProperty("sessionId").GetInt32(),
                });
            }
        };

        await Page.Client.SendAsync("Page.startScreencast", new
        {
            format = "png",
            everyNthFrame = 1,
        });

        await Page.GoToAsync(TestConstants.ServerUrl + "/prerender/index.html");

        var button = await Page.WaitForSelectorAsync("button");
        await button.ClickAsync();

        var link = await Page.WaitForSelectorAsync("a");
        await Task.WhenAll(
            Page.WaitForNavigationAsync(),
            link.ClickAsync()
        );

        var input = await Page.WaitForSelectorAsync("input");
        await input.TypeAsync("ab", new Input.TypeOptions { Delay = 100 });

        await frameReceived.Task.WaitAsync(TimeSpan.FromSeconds(5));

        await Page.Client.SendAsync("Page.stopScreencast");

        Assert.That(screencastFramesReceived, Is.GreaterThan(0));
    }
}
