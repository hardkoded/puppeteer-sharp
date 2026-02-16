using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Input;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.MouseTests
{
    public class MouseTests : PuppeteerPageBaseTest
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

        public MouseTests() : base()
        {
        }

        [Test, PuppeteerTest("mouse.spec", "Mouse", "should click the document")]
        public async Task ShouldClickTheDocument()
        {
            await Page.EvaluateFunctionAsync(@"() => {
                globalThis.clickPromise = new Promise((resolve) => {
                    document.addEventListener('click', (event) => {
                    resolve({
                        type: event.type,
                        detail: event.detail,
                        clientX: event.clientX,
                        clientY: event.clientY,
                        isTrusted: event.isTrusted,
                        button: event.button,
                    });
                    });
                });
            }");
            await Page.Mouse.ClickAsync(50, 60);
            var e = await Page.EvaluateFunctionAsync<MouseEvent>("() => globalThis.clickPromise");

            Assert.That(e.Type, Is.EqualTo("click"));
            Assert.That(e.Detail, Is.EqualTo(1));
            Assert.That(e.ClientX, Is.EqualTo(50));
            Assert.That(e.ClientY, Is.EqualTo(60));
            Assert.That(e.IsTrusted, Is.True);
            Assert.That(e.Button, Is.EqualTo(0));
        }

        [Test, PuppeteerTest("mouse.spec", "Mouse", "should resize the textarea")]
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
            Assert.That(newDimensions.Width, Is.EqualTo(Math.Round(dimensions.Width + 104, MidpointRounding.AwayFromZero)));
            Assert.That(newDimensions.Height, Is.EqualTo(Math.Round(dimensions.Height + 104, MidpointRounding.AwayFromZero)));
        }

        [Test, PuppeteerTest("mouse.spec", "Mouse", "should select the text with mouse")]
        public async Task ShouldSelectTheTextWithMouse()
        {
            const string text = "This is the text that we are going to try to select. Let's see how it goes.";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.FocusAsync("textarea");
            await Page.Keyboard.TypeAsync(text);
            await Page.WaitForFunctionAsync(@"(text) => document.querySelector('textarea').value === text", text);
            var dimensions = await Page.EvaluateFunctionAsync<Dimensions>(Dimensions);
            await Page.Mouse.MoveAsync(dimensions.X + 2, dimensions.Y + 2);
            await Page.Mouse.DownAsync();
            await Page.Mouse.MoveAsync(100, 100);
            await Page.Mouse.UpAsync();
            Assert.That(
                await Page.EvaluateFunctionAsync<string>(@"() => {
                    const textarea = document.querySelector('textarea');
                    return textarea.value.substring(textarea.selectionStart, textarea.selectionEnd);
                }"),
                Is.EqualTo(text));
        }

        [Test, PuppeteerTest("mouse.spec", "Mouse", "should trigger hover state")]
        public async Task ShouldTriggerHoverState()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.HoverAsync("#button-6");
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.querySelector('button:hover').id"), Is.EqualTo("button-6"));
            await Page.HoverAsync("#button-2");
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.querySelector('button:hover').id"), Is.EqualTo("button-2"));
            await Page.HoverAsync("#button-91");
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.querySelector('button:hover').id"), Is.EqualTo("button-91"));
        }

        [Test, PuppeteerTest("mouse.spec", "Mouse", "should trigger hover state with removed window.Node")]
        public async Task ShouldTriggerHoverStateWithRemovedWindowNode()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.EvaluateExpressionAsync("delete window.Node");
            await Page.HoverAsync("#button-6");
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.querySelector('button:hover').id"), Is.EqualTo("button-6"));
        }

        [Test, PuppeteerTest("mouse.spec", "Mouse", "should set modifier keys on click")]
        public async Task ShouldSetModifierKeysOnClick()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.EvaluateExpressionAsync("document.querySelector('#button-3').addEventListener('mousedown', e => window.lastEvent = e, true)");
            var modifiers = new Dictionary<string, string> { ["Shift"] = "shiftKey", ["Control"] = "ctrlKey", ["Alt"] = "altKey", ["Meta"] = "metaKey" };
            foreach (var modifier in modifiers)
            {
                await Page.Keyboard.DownAsync(modifier.Key);
                await Page.ClickAsync("#button-3");
                Assert.That(await Page.EvaluateFunctionAsync<bool>("mod => window.lastEvent[mod]", modifier.Value), Is.True, $"{modifier.Value} should be true");
                await Page.Keyboard.UpAsync(modifier.Key);
            }
            await Page.ClickAsync("#button-3");
            foreach (var modifier in modifiers)
            {
                Assert.That(await Page.EvaluateFunctionAsync<bool>("mod => window.lastEvent[mod]", modifier.Value), Is.False, $"{modifiers.Values} should be false");
            }
        }

        [Test, PuppeteerTest("mouse.spec", "Mouse", "should send mouse wheel events")]
        public async Task ShouldSendMouseWheelEvents()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/wheel.html");
            await using var elem = await Page.QuerySelectorAsync("div");
            var boundingBoxBefore = await elem.BoundingBoxAsync();
            Assert.That(boundingBoxBefore.Width, Is.EqualTo(115));
            Assert.That(boundingBoxBefore.Height, Is.EqualTo(115));

            await Page.Mouse.MoveAsync(
                boundingBoxBefore.X + (boundingBoxBefore.Width / 2),
                boundingBoxBefore.Y + (boundingBoxBefore.Height / 2)
            );

            await Page.Mouse.WheelAsync(0, -100);
            var boundingBoxAfter = await elem.BoundingBoxAsync();

            Assert.That(boundingBoxAfter.Width, Is.EqualTo(230));
            Assert.That(boundingBoxAfter.Height, Is.EqualTo(230));
        }

        [Test, PuppeteerTest("mouse.spec", "Mouse", "should set ctrlKey on the wheel event")]
        public async Task ShouldSetCtrlKeyOnTheWheelEvent()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var ctrlKeyTask = Page.EvaluateFunctionAsync<bool>(@"() => {
                return new Promise(resolve => {
                    window.addEventListener('wheel', event => {
                        resolve(event.ctrlKey);
                    }, { once: true });
                });
            }");
            await Page.Keyboard.DownAsync("Control");
            await Page.Mouse.WheelAsync(0, -100);
            // Scroll back to work around
            // https://bugzilla.mozilla.org/show_bug.cgi?id=1901211.
            if (!TestConstants.IsChrome)
            {
                await Page.Mouse.WheelAsync(0, 100);
            }
            await Page.Keyboard.UpAsync("Control");
            Assert.That(await ctrlKeyTask, Is.True);
        }

        [Test, PuppeteerTest("mouse.spec", "Mouse", "should tween mouse movement")]
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
            Assert.That(await Page.EvaluateExpressionAsync<int[][]>("result"),
                Is.EqualTo(new[] {
                    new[]{ 120, 140 },
                    new[]{ 140, 180 },
                    new[]{ 160, 220 },
                    new[]{ 180, 260 },
                    new[]{ 200, 300 }
            }));
        }

        [Test, PuppeteerTest("mouse.spec", "Mouse", "should work with mobile viewports and cross process navigations")]
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

            Assert.That(await Page.EvaluateExpressionAsync<DomPointInternal>("result"),
                Is.EqualTo(new DomPointInternal()
                {
                    X = 30,
                    Y = 40
                }));
        }

        [Test, PuppeteerTest("mouse.spec", "Mouse", "should not throw if buttons are pressed twice")]
        public async Task ShouldNotThrowIfButtonsArePressedTwice()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Page.Mouse.DownAsync();
            await Page.Mouse.DownAsync();
        }

        [Test, PuppeteerTest("mouse.spec", "Mouse", "should not throw if clicking in parallel")]
        public async Task ShouldNotThrowIfClickingInParallel()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await AddMouseDataListenersAsync(Page);

            await Task.WhenAll(
                Page.Mouse.ClickAsync(0, 5),
                Page.Mouse.ClickAsync(6, 10));

            var data = await Page.EvaluateExpressionAsync<ClickData[]>("window.clicks");
            var commonAttrs = new
            {
                IsTrusted = true,
                Detail = 1,
                ClientY = 5,
                ClientX = 0,
                Button = 0,
            };

            Assert.That(data[0].Type, Is.EqualTo("mousedown"));
            Assert.That(data[0].Buttons, Is.EqualTo(1));
            Assert.That(data[0].IsTrusted, Is.EqualTo(commonAttrs.IsTrusted));
            Assert.That(data[0].Detail, Is.EqualTo(commonAttrs.Detail));
            Assert.That(data[0].ClientX, Is.EqualTo(commonAttrs.ClientX));
            Assert.That(data[0].ClientY, Is.EqualTo(commonAttrs.ClientY));
            Assert.That(data[0].Button, Is.EqualTo(commonAttrs.Button));

            Assert.That(data[1].Type, Is.EqualTo("mouseup"));
            Assert.That(data[1].Buttons, Is.EqualTo(0));
            Assert.That(data[1].IsTrusted, Is.EqualTo(commonAttrs.IsTrusted));
            Assert.That(data[1].Detail, Is.EqualTo(commonAttrs.Detail));
            Assert.That(data[1].ClientX, Is.EqualTo(commonAttrs.ClientX));
            Assert.That(data[1].ClientY, Is.EqualTo(commonAttrs.ClientY));
            Assert.That(data[1].Button, Is.EqualTo(commonAttrs.Button));

            Assert.That(data[2].Type, Is.EqualTo("click"));
            Assert.That(data[2].Buttons, Is.EqualTo(0));
            Assert.That(data[2].IsTrusted, Is.EqualTo(commonAttrs.IsTrusted));
            Assert.That(data[2].Detail, Is.EqualTo(commonAttrs.Detail));
            Assert.That(data[2].ClientX, Is.EqualTo(commonAttrs.ClientX));
            Assert.That(data[2].ClientY, Is.EqualTo(commonAttrs.ClientY));
            Assert.That(data[2].Button, Is.EqualTo(commonAttrs.Button));

            Assert.That(data[3].Type, Is.EqualTo("mousedown"));
            Assert.That(data[3].Buttons, Is.EqualTo(1));
            Assert.That(data[3].IsTrusted, Is.True);
            Assert.That(data[3].Detail, Is.EqualTo(1));
            Assert.That(data[3].ClientX, Is.EqualTo(6));
            Assert.That(data[3].ClientY, Is.EqualTo(10));
            Assert.That(data[3].Button, Is.EqualTo(0));

            Assert.That(data[4].Type, Is.EqualTo("mouseup"));
            Assert.That(data[4].Buttons, Is.EqualTo(0));
            Assert.That(data[4].IsTrusted, Is.True);
            Assert.That(data[4].Detail, Is.EqualTo(1));
            Assert.That(data[4].ClientX, Is.EqualTo(6));
            Assert.That(data[4].ClientY, Is.EqualTo(10));
            Assert.That(data[4].Button, Is.EqualTo(0));

            Assert.That(data[5].Type, Is.EqualTo("click"));
            Assert.That(data[5].Buttons, Is.EqualTo(0));
            Assert.That(data[5].IsTrusted, Is.True);
            Assert.That(data[5].Detail, Is.EqualTo(1));
            Assert.That(data[5].ClientX, Is.EqualTo(6));
            Assert.That(data[5].ClientY, Is.EqualTo(10));
            Assert.That(data[5].Button, Is.EqualTo(0));
        }

        [Test, PuppeteerTest("mouse.spec", "Mouse", "should reset properly")]
        public async Task ShouldResetProperly()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Page.Mouse.MoveAsync(5, 5);
            await Task.WhenAll(
                Page.Mouse.DownAsync(new ClickOptions() { Button = MouseButton.Left }),
                Page.Mouse.DownAsync(new ClickOptions() { Button = MouseButton.Middle }),
                Page.Mouse.DownAsync(new ClickOptions() { Button = MouseButton.Right }));

            await AddMouseDataListenersAsync(Page, true);
            await Page.Mouse.ResetAsync();

            var data = await Page.EvaluateExpressionAsync<ClickData[]>("window.clicks");
            var commonAttrs = new
            {
                IsTrusted = true,
                ClientY = 5,
                ClientX = 5,
            };

            Assert.That(data[0].Type, Is.EqualTo("mouseup"));
            Assert.That(data[0].Button, Is.EqualTo(2));
            Assert.That(data[0].Buttons, Is.EqualTo(5));
            Assert.That(data[0].Detail, Is.EqualTo(1));
            Assert.That(data[0].ClientX, Is.EqualTo(commonAttrs.ClientX));
            Assert.That(data[0].ClientY, Is.EqualTo(commonAttrs.ClientY));
            Assert.That(data[0].IsTrusted, Is.EqualTo(commonAttrs.IsTrusted));

            Assert.That(data[1].Type, Is.EqualTo("auxclick"));
            Assert.That(data[1].Button, Is.EqualTo(2));
            Assert.That(data[1].Buttons, Is.EqualTo(5));
            Assert.That(data[1].Detail, Is.EqualTo(1));
            Assert.That(data[1].ClientX, Is.EqualTo(commonAttrs.ClientX));
            Assert.That(data[1].ClientY, Is.EqualTo(commonAttrs.ClientY));
            Assert.That(data[1].IsTrusted, Is.EqualTo(commonAttrs.IsTrusted));

            // TODO(crbug/1485040): This should align with the firefox implementation.
            if (TestConstants.IsChrome)
            {
                Assert.That(data[2].Type, Is.EqualTo("mouseup"));
                Assert.That(data[2].Button, Is.EqualTo(1));
                Assert.That(data[2].Buttons, Is.EqualTo(1));
                Assert.That(data[2].Detail, Is.EqualTo(0));
                Assert.That(data[2].ClientX, Is.EqualTo(commonAttrs.ClientX));
                Assert.That(data[2].ClientY, Is.EqualTo(commonAttrs.ClientY));
                Assert.That(data[2].IsTrusted, Is.EqualTo(commonAttrs.IsTrusted));

                Assert.That(data[3].Type, Is.EqualTo("mouseup"));
                Assert.That(data[3].Button, Is.EqualTo(0));
                Assert.That(data[3].Buttons, Is.EqualTo(0));
                Assert.That(data[3].Detail, Is.EqualTo(0));
                Assert.That(data[3].ClientX, Is.EqualTo(commonAttrs.ClientX));
                Assert.That(data[3].ClientY, Is.EqualTo(commonAttrs.ClientY));
                Assert.That(data[3].IsTrusted, Is.EqualTo(commonAttrs.IsTrusted));
                return;
            }

            Assert.That(data[2].Type, Is.EqualTo("mouseup"));
            Assert.That(data[2].Button, Is.EqualTo(1));
            Assert.That(data[2].Buttons, Is.EqualTo(1));
            Assert.That(data[2].Detail, Is.EqualTo(1));
            Assert.That(data[2].ClientX, Is.EqualTo(commonAttrs.ClientX));
            Assert.That(data[2].ClientY, Is.EqualTo(commonAttrs.ClientY));
            Assert.That(data[2].IsTrusted, Is.EqualTo(commonAttrs.IsTrusted));

            Assert.That(data[3].Type, Is.EqualTo("auxclick"));
            Assert.That(data[3].Button, Is.EqualTo(1));
            Assert.That(data[3].Buttons, Is.EqualTo(1));
            Assert.That(data[3].Detail, Is.EqualTo(1));
            Assert.That(data[3].ClientX, Is.EqualTo(commonAttrs.ClientX));
            Assert.That(data[3].ClientY, Is.EqualTo(commonAttrs.ClientY));
            Assert.That(data[3].IsTrusted, Is.EqualTo(commonAttrs.IsTrusted));

            Assert.That(data[4].Type, Is.EqualTo("mouseup"));
            Assert.That(data[4].Button, Is.EqualTo(0));
            Assert.That(data[4].Buttons, Is.EqualTo(0));
            Assert.That(data[4].Detail, Is.EqualTo(1));
            Assert.That(data[4].ClientX, Is.EqualTo(commonAttrs.ClientX));
            Assert.That(data[4].ClientY, Is.EqualTo(commonAttrs.ClientY));
            Assert.That(data[4].IsTrusted, Is.EqualTo(commonAttrs.IsTrusted));

            Assert.That(data[5].Type, Is.EqualTo("click"));
            Assert.That(data[5].Button, Is.EqualTo(0));
            Assert.That(data[5].Buttons, Is.EqualTo(0));
            Assert.That(data[5].Detail, Is.EqualTo(1));
            Assert.That(data[5].ClientX, Is.EqualTo(commonAttrs.ClientX));
            Assert.That(data[5].ClientY, Is.EqualTo(commonAttrs.ClientY));
            Assert.That(data[5].IsTrusted, Is.EqualTo(commonAttrs.IsTrusted));
        }

        [Test, PuppeteerTest("mouse.spec", "Mouse", "should evaluate before mouse event")]
        public async Task ShouldEvaluateBeforeMouseEvent()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.GoToAsync(TestConstants.CrossProcessUrl + "/input/button.html");

            await using var button = await Page.WaitForSelectorAsync("button");

            var point = await button.ClickablePointAsync();

            var resultTask = Page.EvaluateFunctionAsync(@"() => {
                return new Promise(resolve => {
                    document.querySelector('button')?.addEventListener('click', resolve, { once: true });
                });
            }");
            await Page.Mouse.ClickAsync(point.X, point.Y);
            await resultTask;
        }

        private Task AddMouseDataListenersAsync(IPage page, bool includeMove = false)
        {
            return Page.EvaluateFunctionAsync(@"(includeMove) => {
                const clicks = [];
                const mouseEventListener = (event) => {
                    clicks.push({
                        type: event.type,
                        detail: event.detail,
                        clientX: event.clientX,
                        clientY: event.clientY,
                        isTrusted: event.isTrusted,
                        button: event.button,
                        buttons: event.buttons,
                    });
                };
                document.addEventListener('mousedown', mouseEventListener);
                if (includeMove) {
                    document.addEventListener('mousemove', mouseEventListener);
                }
                document.addEventListener('mouseup', mouseEventListener);
                document.addEventListener('click', mouseEventListener);
                document.addEventListener('auxclick', mouseEventListener);
                window.clicks = clicks;
            }",
            includeMove);
        }

        internal struct ClickData
        {
            public string Type { get; set; }

            public int Detail { get; set; }

            public int ClientX { get; set; }

            public int ClientY { get; set; }

            public bool IsTrusted { get; set; }

            public int Button { get; set; }

            public int Buttons { get; set; }
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

        internal struct MouseEvent
        {
            public string Type { get; set; }

            public int Detail { get; set; }

            public int ClientX { get; set; }

            public int ClientY { get; set; }

            public bool IsTrusted { get; set; }

            public int Button { get; set; }
        }
    }
}
