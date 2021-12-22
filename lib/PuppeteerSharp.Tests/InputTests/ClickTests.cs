using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Input;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.InputTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ClickTests : PuppeteerPageBaseTest
    {
        public ClickTests(ITestOutputHelper output) : base(output)
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
        public async Task ShouldClickSvg()
        {
            await Page.SetContentAsync($@"
                <svg height=""100"" width=""100"">
                  <circle onclick=""javascript:window.__CLICKED=42"" cx=""50"" cy=""50"" r=""40"" stroke=""black"" stroke-width=""3"" fill=""red""/>
                </svg>
            ");
            await Page.ClickAsync("circle");
            Assert.Equal(42, await Page.EvaluateFunctionAsync<int>("() => window.__CLICKED"));
        }

        [Fact]
        public async Task ShouldClickTheButtonIfWindowNodeIsRemoved()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.EvaluateExpressionAsync("delete window.Node");
            await Page.ClickAsync("button");
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync("result"));
        }

        [Fact(Skip = "See https://github.com/GoogleChrome/puppeteer/issues/4281")]
        public async Task ShouldClickOnASpanWithAnInlineElementInside()
        {
            await Page.SetContentAsync($@"
                <style>
                span::before {{
                    content: 'q';
                }}
                </style>
                <span onclick='javascript:window.CLICKED=42'></span>
            ");
            await Page.ClickAsync("span");
            Assert.Equal(42, await Page.EvaluateFunctionAsync<int>("() => window.CLICKED"));
        }

        /// <summary>
        /// This test is called ShouldNotThrowUnhandledPromiseRejectionWhenPageCloses in puppeteer.
        /// </summary>
        [Fact]
        public async Task ShouldGracefullyFailWhenPageCloses()
        {
            var newPage = await Browser.NewPageAsync();
            await Assert.ThrowsAsync<TargetClosedException>(() => Task.WhenAll(
                newPage.CloseAsync(),
                newPage.Mouse.ClickAsync(1, 2)
             ));
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
        public async Task ShouldClickWhenOneOfInlineBoxChildrenIsOutsideOfViewport()
        {
            await Page.SetContentAsync($@"
            <style>
            i {{
                position: absolute;
                top: -1000px;
            }}
            </style>
            <span onclick='javascript:window.CLICKED = 42;'><i>woof</i><b>doggo</b></span>
            ");

            await Page.ClickAsync("span");
            Assert.Equal(42, await Page.EvaluateFunctionAsync<int>("() => window.CLICKED"));
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
            await Page.ClickAsync("a");
            Assert.True(await Page.EvaluateExpressionAsync<bool>("window.__clicked"));
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
        public async Task ShouldFireContextmenuEventOnRightClick()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.ClickAsync("#button-8", new ClickOptions { Button = MouseButton.Right });
            Assert.Equal("context menu", await Page.EvaluateExpressionAsync<string>("document.querySelector('#button-8').textContent"));
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

        [Fact(Skip = "see https://github.com/GoogleChrome/puppeteer/issues/4110")]
        public async Task ShouldClickTheButtonWithFixedPositionInsideAnIframe()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await Page.SetContentAsync("<div style=\"width:100px;height:2000px\">spacer</div>");
            await FrameUtils.AttachFrameAsync(Page, "button-test", TestConstants.ServerUrl + "/input/button.html");
            var frame = Page.FirstChildFrame();
            await frame.QuerySelectorAsync("button").EvaluateFunctionAsync("button => button.style.setProperty('position', 'fixed')");
            await frame.ClickAsync("button");
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
    }
}
