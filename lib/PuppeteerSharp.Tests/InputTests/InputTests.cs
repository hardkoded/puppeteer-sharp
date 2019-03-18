﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Input;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.InputTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class InputTests : PuppeteerPageBaseTest
    {
        private const string Dimensions = @"function dimensions() {
            const rect = document.querySelector('textarea').getBoundingClientRect();
            return {
                x: rect.left,
                y: rect.top,
                width: rect.width,
                height: rect.height
            };
        }";

        public InputTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldClickTheButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.ClickAsync("button");
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [Fact]
        public async Task ShouldClickWithDisabledJavascript()
        {
            await Page.SetJavaScriptEnabledAsync(false);
            await Page.GoToAsync(TestConstants.ServerUrl + "/wrappedlink.html");
            await Task.WhenAll(
                Page.ClickAsync("a"),
                Page.WaitForNavigationAsync()
            );
            Assert.Equal(TestConstants.ServerUrl + "/wrappedlink.html#clicked", Page.Url);
        }

        [Fact]
        public async Task ShouldClickOffscreenButtons()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/offscreenbuttons.html");
            var messages = new List<string>();
            Page.Console += (sender, e) => messages.Add(e.Message.Text);

            for (var i = 0; i < 11; ++i)
            {
                // We might've scrolled to click a button - reset to (0, 0).
                await Page.EvaluateFunctionAsync("() => window.scrollTo(0, 0)");
                await Page.ClickAsync($"#btn{i}");
            }
            Assert.Equal(new List<string>
            {
                "button #0 clicked",
                "button #1 clicked",
                "button #2 clicked",
                "button #3 clicked",
                "button #4 clicked",
                "button #5 clicked",
                "button #6 clicked",
                "button #7 clicked",
                "button #8 clicked",
                "button #9 clicked",
                "button #10 clicked"
            }, messages);
        }

        [Fact]
        public async Task ShouldClickWrappedLinks()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/wrappedlink.html");
            await Task.WhenAll(
                Page.ClickAsync("a"),
                Page.WaitForNavigationAsync()
            );
            Assert.Equal(TestConstants.ServerUrl + "/wrappedlink.html#clicked", Page.Url);
        }

        [Fact]
        public async Task ShouldClickOnCheckboxInputAndToggle()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            Assert.Null(await Page.EvaluateExpressionAsync("result.check"));
            await Page.ClickAsync("input#agree");
            Assert.True(await Page.EvaluateExpressionAsync<bool>("result.check"));
            Assert.Equal(new[] {
                "mouseover",
                "mouseenter",
                "mousemove",
                "mousedown",
                "mouseup",
                "click",
                "input",
                "change"
            }, await Page.EvaluateExpressionAsync<string[]>("result.events"));
            await Page.ClickAsync("input#agree");
            Assert.False(await Page.EvaluateExpressionAsync<bool>("result.check"));
        }

        [Fact]
        public async Task ShouldClickOnCheckboxLabelAndToggle()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            Assert.Null(await Page.EvaluateExpressionAsync("result.check"));
            await Page.ClickAsync("label[for=\"agree\"]");
            Assert.True(await Page.EvaluateExpressionAsync<bool>("result.check"));
            Assert.Equal(new[] {
                "click",
                "input",
                "change"
            }, await Page.EvaluateExpressionAsync<string[]>("result.events"));
            await Page.ClickAsync("label[for=\"agree\"]");
            Assert.False(await Page.EvaluateExpressionAsync<bool>("result.check"));
        }

        [Fact]
        public async Task ShouldFailToClickAMissingButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var exception = await Assert.ThrowsAsync<SelectorException>(()
                => Page.ClickAsync("button.does-not-exist"));
            Assert.Equal("No node found for selector: button.does-not-exist", exception.Message);
            Assert.Equal("button.does-not-exist", exception.Selector);
        }

        // https://github.com/GoogleChrome/puppeteer/issues/161
        [Fact]
        public async Task ShouldNotHangWithTouchEnabledViewports()
        {
            await Page.SetViewportAsync(TestConstants.IPhone.ViewPort);
            await Page.Mouse.DownAsync();
            await Page.Mouse.MoveAsync(100, 10);
            await Page.Mouse.UpAsync();
        }

        [Fact]
        public async Task ShouldClickTheButtonIfWindowNodeIsRemoved()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.EvaluateExpressionAsync("delete window.Node");
            await Page.ClickAsync("button");
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync("result"));
        }

        [Fact]
        public async Task ShouldClickTheButtonAfterNavigation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.ClickAsync("button");
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.ClickAsync("button");
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [Fact]
        public async Task ShouldUploadTheFile()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/fileupload.html");
            var filePath = "./Assets/file-to-upload.txt";
            var input = await Page.QuerySelectorAsync("input");
            await input.UploadFileAsync(filePath);
            Assert.Equal("file-to-upload.txt", await Page.EvaluateFunctionAsync<string>("e => e.files[0].name", input));
            Assert.Equal("contents of the file", await Page.EvaluateFunctionAsync<string>(@"e => {
                const reader = new FileReader();
                const promise = new Promise(fulfill => reader.onload = fulfill);
                reader.readAsText(e.files[0]);
                return promise.then(() => reader.result);
            }", input));
        }

        [Fact]
        public async Task ShouldResizeTheTextarea()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            var dimensions = await Page.EvaluateFunctionAsync<Dimensions>(Dimensions);
            var mouse = Page.Mouse;
            await mouse.MoveAsync(dimensions.X + dimensions.Width - 4, dimensions.Y + dimensions.Height - 4);
            await mouse.DownAsync();
            await mouse.MoveAsync(dimensions.X + dimensions.Width + 100, dimensions.Y + dimensions.Height + 100);
            await mouse.UpAsync();
            var newDimensions = await Page.EvaluateFunctionAsync<Dimensions>(Dimensions);
            Assert.Equal(Math.Round(dimensions.Width + 104, MidpointRounding.AwayFromZero), newDimensions.Width);
            Assert.Equal(Math.Round(dimensions.Height + 104, MidpointRounding.AwayFromZero), newDimensions.Height);
        }

        [Fact]
        public async Task ShouldScrollAndClickTheButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.ClickAsync("#button-5");
            Assert.Equal("clicked", await Page.EvaluateExpressionAsync<string>("document.querySelector(\"#button-5\").textContent"));
            await Page.ClickAsync("#button-80");
            Assert.Equal("clicked", await Page.EvaluateExpressionAsync<string>("document.querySelector(\"#button-80\").textContent"));
        }

        [Fact]
        public async Task ShouldDoubleClickTheButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.EvaluateExpressionAsync(@"{
               window.double = false;
               const button = document.querySelector('button');
               button.addEventListener('dblclick', event => {
                 window.double = true;
               });
            }");
            var button = await Page.QuerySelectorAsync("button");
            await button.ClickAsync(new ClickOptions { ClickCount = 2 });
            Assert.True(await Page.EvaluateExpressionAsync<bool>("double"));
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [Fact]
        public async Task ShouldClickAPartiallyObscuredButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.EvaluateExpressionAsync(@"{
                const button = document.querySelector('button');
                button.textContent = 'Some really long text that will go offscreen';
                button.style.position = 'absolute';
                button.style.left = '368px';
            }");
            await Page.ClickAsync("button");
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [Fact]
        public async Task ShouldClickARotatedButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/rotatedButton.html");
            await Page.ClickAsync("button");
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [Fact]
        public async Task ShouldSelectTheTextWithMouse()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.FocusAsync("textarea");
            const string text = "This is the text that we are going to try to select. Let's see how it goes.";
            await Page.Keyboard.TypeAsync(text);
            // Firefox needs an extra frame here after typing or it will fail to set the scrollTop
            await Page.EvaluateExpressionAsync("new Promise(requestAnimationFrame)");
            await Page.EvaluateExpressionAsync("document.querySelector('textarea').scrollTop = 0");
            var dimensions = await Page.EvaluateFunctionAsync<Dimensions>(Dimensions);
            await Page.Mouse.MoveAsync(dimensions.X + 2, dimensions.Y + 2);
            await Page.Mouse.DownAsync();
            await Page.Mouse.MoveAsync(100, 100);
            await Page.Mouse.UpAsync();
            Assert.Equal(text, await Page.EvaluateFunctionAsync<string>(@"() => {
                const textarea = document.querySelector('textarea');
                return textarea.value.substring(textarea.selectionStart, textarea.selectionEnd);
            }"));
        }

        [Fact]
        public async Task ShouldSelectTheTextByTripleClicking()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.FocusAsync("textarea");
            const string text = "This is the text that we are going to try to select. Let's see how it goes.";
            await Page.Keyboard.TypeAsync(text);
            await Page.ClickAsync("textarea");
            await Page.ClickAsync("textarea", new ClickOptions { ClickCount = 2 });
            await Page.ClickAsync("textarea", new ClickOptions { ClickCount = 3 });
            Assert.Equal(text, await Page.EvaluateExpressionAsync<string>("window.getSelection().toString()"));
        }

        [Fact]
        public async Task ShouldTriggerHoverState()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.HoverAsync("#button-6");
            Assert.Equal("button-6", await Page.EvaluateExpressionAsync<string>("document.querySelector('button:hover').id"));
            await Page.HoverAsync("#button-2");
            Assert.Equal("button-2", await Page.EvaluateExpressionAsync<string>("document.querySelector('button:hover').id"));
            await Page.HoverAsync("#button-91");
            Assert.Equal("button-91", await Page.EvaluateExpressionAsync<string>("document.querySelector('button:hover').id"));
        }

        [Fact]
        public async Task ShouldFireContextmenuEventOnRightClick()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.ClickAsync("#button-8", new ClickOptions { Button = MouseButton.Right });
            Assert.Equal("context menu", await Page.EvaluateExpressionAsync<string>("document.querySelector('#button-8').textContent"));
        }

        [Fact]
        public async Task ShouldTriggerHoverStateWithRemovedWindowNode()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.EvaluateExpressionAsync("delete window.Node");
            await Page.HoverAsync("#button-6");
            Assert.Equal("button-6", await Page.EvaluateExpressionAsync("document.querySelector('button:hover').id"));
        }

        [Fact]
        public async Task ShouldSetModifierKeysOnClick()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.EvaluateExpressionAsync("document.querySelector('#button-3').addEventListener('mousedown', e => window.lastEvent = e, true)");
            var modifiers = new Dictionary<string, string> { ["Shift"] = "shiftKey", ["Control"] = "ctrlKey", ["Alt"] = "altKey", ["Meta"] = "metaKey" };
            foreach (var modifier in modifiers)
            {
                await Page.Keyboard.DownAsync(modifier.Key);
                await Page.ClickAsync("#button-3");
                if (!(await Page.EvaluateFunctionAsync<bool>("mod => window.lastEvent[mod]", modifier.Value)))
                {
                    Assert.True(false, $"{modifier.Value} should be true");
                }

                await Page.Keyboard.UpAsync(modifier.Key);
            }
            await Page.ClickAsync("#button-3");
            foreach (var modifier in modifiers)
            {
                if (await Page.EvaluateFunctionAsync<bool>("mod => window.lastEvent[mod]", modifier.Value))
                {
                    Assert.False(true, $"{modifiers.Values} should be false");
                }
            }
        }

        // @see https://github.com/GoogleChrome/puppeteer/issues/206
        [Fact]
        public async Task ShouldClickLinksWhichCauseNavigation()
        {
            await Page.SetContentAsync($"<a href=\"{TestConstants.EmptyPage}\">empty.html</a>");
            // This await should not hang.
            await Page.ClickAsync("a");
        }

        [Fact]
        public async Task ShouldTweenMouseMovement()
        {
            await Page.Mouse.MoveAsync(100, 100);
            await Page.EvaluateExpressionAsync(@"{
              window.result = [];
              document.addEventListener('mousemove', event => {
                window.result.push([event.clientX, event.clientY]);
              });
            }");
            await Page.Mouse.MoveAsync(200, 300, new MoveOptions { Steps = 5 });
            Assert.Equal(new[] {
                new[]{ 120, 140 },
                new[]{ 140, 180 },
                new[]{ 160, 220 },
                new[]{ 180, 260 },
                new[]{ 200, 300 }
            }, await Page.EvaluateExpressionAsync<int[][]>("result"));
        }

        [Fact(Skip = "see https://crbug.com/929806")]
        public async Task ShouldWorkWithMobileViewportsAndCrossProcessNavigations()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetViewportAsync(new ViewPortOptions
            {
                Width = 360,
                Height = 640,
                IsMobile = true
            });
            await Page.GoToAsync(TestConstants.CrossProcessUrl + "/mobile.html");
            await Page.EvaluateFunctionAsync(@"() => {
                document.addEventListener('click', event => {
                    window.result = { x: event.clientX, y: event.clientY };
                });
            }");

            await Page.Mouse.ClickAsync(30, 40);

            Assert.Equal(new DomPointInternal()
            {
                X = 30,
                Y = 40
            }, await Page.EvaluateExpressionAsync<DomPointInternal>("result"));
        }

        [Fact]
        public async Task ShouldClickTheButtonInsideAnIframe()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync("<div style=\"width:100px;height:100px\">spacer</div>");
            await FrameUtils.AttachFrameAsync(Page, "button-test", TestConstants.ServerUrl + "/input/button.html");
            var frame = Page.FirstChildFrame();
            var button = await frame.QuerySelectorAsync("button");
            await button.ClickAsync();
            Assert.Equal("Clicked", await frame.EvaluateExpressionAsync<string>("window.result"));
        }

        [Fact]
        public async Task ShouldClickTheButtonWithDeviceScaleFactorSet()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 400, Height = 400, DeviceScaleFactor = 5 });
            Assert.Equal(5, await Page.EvaluateExpressionAsync<int>("window.devicePixelRatio"));
            await Page.SetContentAsync("<div style=\"width:100px;height:100px\">spacer</div>");
            await FrameUtils.AttachFrameAsync(Page, "button-test", TestConstants.ServerUrl + "/input/button.html");
            var frame = Page.FirstChildFrame();
            var button = await frame.QuerySelectorAsync("button");
            await button.ClickAsync();
            Assert.Equal("Clicked", await frame.EvaluateExpressionAsync<string>("window.result"));
        }

        [Fact]
        public async Task ShouldScrollAccordingToMouseWheel()
        {
            var waitForFunctionOptions = new WaitForFunctionOptions
            {
                PollingInterval = 50,
                Timeout = 1000
            };

            Task WaitForScrollPointAsync(DomPointInternal point)
                => Page.WaitForFunctionAsync(
                    @"(x, y) => document.body.scrollLeft === x && document.body.scrollTop === y",
                    waitForFunctionOptions,
                    point.X,
                    point.Y);

            await Page.GoToAsync(
                $@"{TestConstants.ServerUrl}/longText.html",
                WaitUntilNavigation.Networkidle0);

            var expectedWheelEvents = new[]
            {
                new WheelEventInternal(0, 500),
                new WheelEventInternal(0, -200),
                new WheelEventInternal(300, 0),
                new WheelEventInternal(-150, 0)
            };

            var expectedScrollPoint = new DomPointInternal
            {
                X = 0,
                Y = 0
            };

            await WaitForScrollPointAsync(expectedScrollPoint);

            foreach (var @event in expectedWheelEvents)
            {
                await Page.Mouse.WheelAsync(@event.DeltaX, @event.DeltaY);

                expectedScrollPoint.Scroll(@event.DeltaX, @event.DeltaY);
                await WaitForScrollPointAsync(expectedScrollPoint);
            }
        }

        internal struct WheelEventInternal
        {
            public WheelEventInternal(decimal deltaX, decimal deltaY)
            {
                DeltaX = deltaX;
                DeltaY = deltaY;
            }

            public decimal DeltaX { get; set; }

            public decimal DeltaY { get; set; }

            public override string ToString() => $"({DeltaX}, {DeltaY})";
        }

        internal struct DomPointInternal
        {
            public decimal X { get; set; }

            public decimal Y { get; set; }

            public override string ToString() => $"({X}, {Y})";

            public void Scroll(decimal deltaX, decimal deltaY)
            {
                X = Math.Max(0, X + deltaX);
                Y = Math.Max(0, Y + deltaY);
            }
        }
    }
}
