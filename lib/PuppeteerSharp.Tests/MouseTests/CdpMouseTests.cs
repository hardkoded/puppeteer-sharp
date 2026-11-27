using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Input;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.MouseTests
{
    public class CdpMouseTests
    {
        private readonly List<InputDispatchMouseEventRequest> _requests = [];
        private readonly List<TaskCompletionSource<JsonElement?>> _responses = [];

        [SetUp]
        public void ClearCommands()
        {
            _requests.Clear();
            _responses.Clear();
        }

        [
            Test,
            PuppeteerTest(
                "puppeteer-sharp",
                "Mouse",
                "should not keep the button of a failed press made in parallel with another press"
            )
        ]
        public async Task ShouldNotKeepTheButtonOfAFailedPressMadeInParallelWithAnotherPress()
        {
            var client = CreateClient();
            using var mouse = new CdpMouse(client, new CdpKeyboard(client));

            var left = mouse.DownAsync(new ClickOptions { Button = MouseButton.Left });
            var right = mouse.DownAsync(new ClickOptions { Button = MouseButton.Right });
            _responses[0].SetException(new PuppeteerException("Left failed"));
            _responses[1].SetResult(null);

            Assert.ThrowsAsync<PuppeteerException>(() => left);
            await right;
            Assert.That(await GetPressedButtonsAsync(mouse), Is.EqualTo(MouseButton.Right));
        }

        [
            Test,
            PuppeteerTest(
                "puppeteer-sharp",
                "Mouse",
                "should keep the button of a failed release made in parallel with a press"
            )
        ]
        public async Task ShouldKeepTheButtonOfAFailedReleaseMadeInParallelWithAPress()
        {
            var client = CreateClient();
            using var mouse = new CdpMouse(client, new CdpKeyboard(client));

            var press = mouse.DownAsync(new ClickOptions { Button = MouseButton.Left });
            _responses[0].SetResult(null);
            await press;

            var release = mouse.UpAsync(new ClickOptions { Button = MouseButton.Left });
            var right = mouse.DownAsync(new ClickOptions { Button = MouseButton.Right });
            _responses[1].SetException(new PuppeteerException("Release failed"));
            _responses[2].SetResult(null);

            Assert.ThrowsAsync<PuppeteerException>(() => release);
            await right;
            Assert.That(
                await GetPressedButtonsAsync(mouse),
                Is.EqualTo(MouseButton.Left | MouseButton.Right)
            );
        }

        [
            Test,
            PuppeteerTest(
                "puppeteer-sharp",
                "Mouse",
                "should apply commands in the order they were sent"
            )
        ]
        public async Task ShouldApplyCommandsInTheOrderTheyWereSent()
        {
            var client = CreateClient();
            using var mouse = new CdpMouse(client, new CdpKeyboard(client));

            var press = mouse.DownAsync();
            var release = mouse.UpAsync();
            _responses[1].SetResult(null);
            await release;
            _responses[0].SetResult(null);
            await press;

            Assert.That(await GetPressedButtonsAsync(mouse), Is.EqualTo(MouseButton.None));
        }

        private async Task<MouseButton> GetPressedButtonsAsync(CdpMouse mouse)
        {
            var move = mouse.MoveAsync(1, 1);
            _responses[^1].SetResult(null);
            await move;
            return (MouseButton)_requests[^1].Buttons;
        }

        private CDPSession CreateClient()
        {
            var client = Substitute.For<CDPSession>();
            client
                .SendAsync(
                    Arg.Any<string>(),
                    Arg.Any<object>(),
                    Arg.Any<bool>(),
                    Arg.Any<CommandOptions>()
                )
                .Returns(callInfo =>
                {
                    var response = new TaskCompletionSource<JsonElement?>();
                    _requests.Add(callInfo.ArgAt<InputDispatchMouseEventRequest>(1));
                    _responses.Add(response);
                    return response.Task;
                });
            return client;
        }
    }
}
