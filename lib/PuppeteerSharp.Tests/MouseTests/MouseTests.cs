using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Input;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;
using System.Runtime.InteropServices;

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

        public MouseTests(): base()
        {
        }

        [PuppeteerTest("mouse.spec.ts", "Mouse", "should click the document")]
        [PuppeteerTimeout]
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

            Assert.AreEqual("click", e.Type);
            Assert.AreEqual(1, e.Detail);
            Assert.AreEqual(50, e.ClientX);
            Assert.AreEqual(60, e.ClientY);
            Assert.True(e.IsTrusted);
            Assert.AreEqual(0, e.Button);
        }

        [PuppeteerTest("mouse.spec.ts", "Mouse", "should resize the textarea")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
            Assert.AreEqual(Math.Round(dimensions.Width + 104, MidpointRounding.AwayFromZero), newDimensions.Width);
            Assert.AreEqual(Math.Round(dimensions.Height + 104, MidpointRounding.AwayFromZero), newDimensions.Height);
        }

        [PuppeteerTest("mouse.spec.ts", "Mouse", "should select the text with mouse")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
            Assert.AreEqual(text, await Page.EvaluateFunctionAsync<string>(@"() => {
                const textarea = document.querySelector('textarea');
                return textarea.value.substring(textarea.selectionStart, textarea.selectionEnd);
            }"));
        }

        [PuppeteerTest("mouse.spec.ts", "Mouse", "should trigger hover state")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldTriggerHoverState()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.HoverAsync("#button-6");
            Assert.AreEqual("button-6", await Page.EvaluateExpressionAsync<string>("document.querySelector('button:hover').id"));
            await Page.HoverAsync("#button-2");
            Assert.AreEqual("button-2", await Page.EvaluateExpressionAsync<string>("document.querySelector('button:hover').id"));
            await Page.HoverAsync("#button-91");
            Assert.AreEqual("button-91", await Page.EvaluateExpressionAsync<string>("document.querySelector('button:hover').id"));
        }

        [PuppeteerTest("mouse.spec.ts", "Mouse", "should trigger hover state with removed window.Node")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldTriggerHoverStateWithRemovedWindowNode()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.EvaluateExpressionAsync("delete window.Node");
            await Page.HoverAsync("#button-6");
            Assert.AreEqual("button-6", await Page.EvaluateExpressionAsync<string>("document.querySelector('button:hover').id"));
        }

        [PuppeteerTest("mouse.spec.ts", "Mouse", "should set modifier keys on click")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

        [PuppeteerTest("mouse.spec.ts", "Mouse", "should send mouse wheel events")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldSendMouseWheelEvents()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/wheel.html");
            var elem = await Page.QuerySelectorAsync("div");
            var boundingBoxBefore = await elem.BoundingBoxAsync();
            Assert.AreEqual(115, boundingBoxBefore.Width);
            Assert.AreEqual(115, boundingBoxBefore.Height);

            await Page.Mouse.MoveAsync(
                boundingBoxBefore.X + (boundingBoxBefore.Width / 2),
                boundingBoxBefore.Y + (boundingBoxBefore.Height / 2)
            );

            await Page.Mouse.WheelAsync(0, -100);
            var boundingBoxAfter = await elem.BoundingBoxAsync();

            // We don't have this OS check upstream
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.AreEqual(345, boundingBoxAfter.Width);
                Assert.AreEqual(345, boundingBoxAfter.Height);
            }
            else
            {
                Assert.AreEqual(230, boundingBoxAfter.Width);
                Assert.AreEqual(230, boundingBoxAfter.Height);
            }
        }

        [PuppeteerTest("mouse.spec.ts", "Mouse", "should tween mouse movement")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
            Assert.AreEqual(new[] {
                new[]{ 120, 140 },
                new[]{ 140, 180 },
                new[]{ 160, 220 },
                new[]{ 180, 260 },
                new[]{ 200, 300 }
            }, await Page.EvaluateExpressionAsync<int[][]>("result"));
        }

        [PuppeteerTest("mouse.spec.ts", "Mouse", "should work with mobile viewports and cross process navigations")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

            Assert.AreEqual(new DomPointInternal()
            {
                X = 30,
                Y = 40
            }, await Page.EvaluateExpressionAsync<DomPointInternal>("result"));
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
