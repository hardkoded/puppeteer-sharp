using System.Collections.Generic;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using CefSharp.DevTools.Dom.Input;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ClickTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ClickTests : DevToolsContextBaseTest
    {
        public ClickTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("click.spec.ts", "Page.click", "should click the button")]
        [PuppeteerFact]
        public async Task ShouldClickTheButton()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await DevToolsContext.ClickAsync("button");
            Assert.Equal("Clicked", await DevToolsContext.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerTest("click.spec.ts", "Page.click", "should click svg")]
        [PuppeteerFact]
        public async Task ShouldClickSvg()
        {
            await DevToolsContext.SetContentAsync($@"
                <svg height=""100"" width=""100"">
                  <circle onclick=""javascript:window.__CLICKED=42"" cx=""50"" cy=""50"" r=""40"" stroke=""black"" stroke-width=""3"" fill=""red""/>
                </svg>
            ");
            await DevToolsContext.ClickAsync("circle");
            Assert.Equal(42, await DevToolsContext.EvaluateFunctionAsync<int>("() => window.__CLICKED"));
        }

        [PuppeteerTest("click.spec.ts", "Page.click", "should click the button if window.Node is removed")]
        [PuppeteerFact]
        public async Task ShouldClickTheButtonIfWindowNodeIsRemoved()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await DevToolsContext.EvaluateExpressionAsync("delete window.Node");
            await DevToolsContext.ClickAsync("button");
            Assert.Equal("Clicked", await DevToolsContext.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerTest("click.spec.ts", "Page.click", "should click on a span with an inline element inside")]
        [PuppeteerFact(Skip = "See https://github.com/GoogleChrome/puppeteer/issues/4281")]
        public async Task ShouldClickOnASpanWithAnInlineElementInside()
        {
            await DevToolsContext.SetContentAsync($@"
                <style>
                span::before {{
                    content: 'q';
                }}
                </style>
                <span onclick='javascript:window.CLICKED=42'></span>
            ");
            await DevToolsContext.ClickAsync("span");
            Assert.Equal(42, await DevToolsContext.EvaluateFunctionAsync<int>("() => window.CLICKED"));
        }

        [PuppeteerFact]
        public async Task ShouldClickTheButtonAfterNavigation()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await DevToolsContext.ClickAsync("button");
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await DevToolsContext.ClickAsync("button");
            Assert.Equal("Clicked", await DevToolsContext.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerFact]
        public async Task ShouldClickWhenOneOfInlineBoxChildrenIsOutsideOfViewport()
        {
            await DevToolsContext.SetContentAsync($@"
            <style>
            i {{
                position: absolute;
                top: -1000px;
            }}
            </style>
            <span onclick='javascript:window.CLICKED = 42;'><i>woof</i><b>doggo</b></span>
            ");

            await DevToolsContext.ClickAsync("span");
            Assert.Equal(42, await DevToolsContext.EvaluateFunctionAsync<int>("() => window.CLICKED"));
        }

        [PuppeteerFact]
        public async Task ShouldSelectTheTextByTripleClicking()
        {
            const string expected = "This is the text that we are going to try to select. Let's see how it goes.";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");

            var element = await DevToolsContext.QuerySelectorAsync<HtmlTextAreaElement>("textarea");

            await element.FocusAsync();

            await element.SetValueAsync(expected);
            await element.ClickAsync();
            await element.ClickAsync(new ClickOptions { ClickCount = 2 });
            await element.ClickAsync(new ClickOptions { ClickCount = 3 });

            var actual = await DevToolsContext.EvaluateFunctionAsync<string>(@"() => {
                const textarea = document.querySelector('textarea');
                return textarea.value.substring(
                    textarea.selectionStart,
                    textarea.selectionEnd
                );
            }");

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldClickWrappedLinks()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/wrappedlink.html");
            await DevToolsContext.ClickAsync("a");
            Assert.True(await DevToolsContext.EvaluateExpressionAsync<bool>("window.__clicked"));
        }

        [PuppeteerFact]
        public async Task ShouldClickOnCheckboxInputAndToggle()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            Assert.Null(await DevToolsContext.EvaluateExpressionAsync<object>("result.check"));
            await DevToolsContext.ClickAsync("input#agree");
            Assert.True(await DevToolsContext.EvaluateExpressionAsync<bool>("result.check"));
            Assert.Equal(new[] {
                "mouseover",
                "mouseenter",
                "mousemove",
                "mousedown",
                "mouseup",
                "click",
                "input",
                "change"
            }, await DevToolsContext.EvaluateExpressionAsync<string[]>("result.events"));
            await DevToolsContext.ClickAsync("input#agree");
            Assert.False(await DevToolsContext.EvaluateExpressionAsync<bool>("result.check"));
        }

        [PuppeteerFact]
        public async Task ShouldClickOnCheckboxLabelAndToggle()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            Assert.Null(await DevToolsContext.EvaluateExpressionAsync<object>("result.check"));
            await DevToolsContext.ClickAsync("label[for=\"agree\"]");
            Assert.True(await DevToolsContext.EvaluateExpressionAsync<bool>("result.check"));
            Assert.Equal(new[] {
                "click",
                "input",
                "change"
            }, await DevToolsContext.EvaluateExpressionAsync<string[]>("result.events"));
            await DevToolsContext.ClickAsync("label[for=\"agree\"]");
            Assert.False(await DevToolsContext.EvaluateExpressionAsync<bool>("result.check"));
        }

        [PuppeteerFact]
        public async Task ShouldFailToClickAMissingButton()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var exception = await Assert.ThrowsAsync<SelectorException>(()
                => DevToolsContext.ClickAsync("button.does-not-exist"));
            Assert.Equal("No node found for selector: button.does-not-exist", exception.Message);
            Assert.Equal("button.does-not-exist", exception.Selector);
        }

        [PuppeteerFact]
        public async Task ShouldScrollAndClickTheButton()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await DevToolsContext.ClickAsync("#button-5");
            Assert.Equal("clicked", await DevToolsContext.EvaluateExpressionAsync<string>("document.querySelector(\"#button-5\").textContent"));
            await DevToolsContext.ClickAsync("#button-80");
            Assert.Equal("clicked", await DevToolsContext.EvaluateExpressionAsync<string>("document.querySelector(\"#button-80\").textContent"));
        }

        [PuppeteerFact]
        public async Task ShouldDoubleClickTheButton()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await DevToolsContext.EvaluateExpressionAsync(@"{
               window.double = false;
               const button = document.querySelector('button');
               button.addEventListener('dblclick', event => {
                 window.double = true;
               });
            }");
            var button = await DevToolsContext.QuerySelectorAsync<HtmlButtonElement>("button");
            await button.ClickAsync(new ClickOptions { ClickCount = 2 });
            Assert.True(await DevToolsContext.EvaluateExpressionAsync<bool>("double"));
            Assert.Equal("Clicked", await DevToolsContext.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerFact]
        public async Task ShouldClickAPartiallyObscuredButton()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await DevToolsContext.EvaluateExpressionAsync(@"{
                const button = document.querySelector('button');
                button.textContent = 'Some really long text that will go offscreen';
                button.style.position = 'absolute';
                button.style.left = '368px';
            }");
            await DevToolsContext.ClickAsync("button");
            Assert.Equal("Clicked", await DevToolsContext.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerFact]
        public async Task ShouldClickARotatedButton()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/rotatedButton.html");
            await DevToolsContext.ClickAsync("button");
            Assert.Equal("Clicked", await DevToolsContext.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerFact]
        public async Task ShouldFireContextmenuEventOnRightClick()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await DevToolsContext.ClickAsync("#button-8", new ClickOptions { Button = MouseButton.Right });
            Assert.Equal("context menu", await DevToolsContext.EvaluateExpressionAsync<string>("document.querySelector('#button-8').textContent"));
        }

        // @see https://github.com/GoogleChrome/puppeteer/issues/206
        [PuppeteerFact]
        public async Task ShouldClickLinksWhichCauseNavigation()
        {
            await DevToolsContext.SetContentAsync($"<a href=\"{TestConstants.EmptyPage}\">empty.html</a>");
            // This await should not hang.
            await DevToolsContext.ClickAsync("a");
        }

        [PuppeteerFact]
        public async Task ShouldClickTheButtonInsideAnIframe()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.SetContentAsync("<div style=\"width:100px;height:100px\">spacer</div>");
            await FrameUtils.AttachFrameAsync(DevToolsContext, "button-test", TestConstants.ServerUrl + "/input/button.html");
            var frame = DevToolsContext.FirstChildFrame();
            var button = await frame.QuerySelectorAsync<HtmlButtonElement>("button");
            await button.ClickAsync();
            Assert.Equal("Clicked", await frame.EvaluateExpressionAsync<string>("window.result"));
        }

        [PuppeteerFact(Skip = "TODO: Fix this if possible")]
        public async Task ShouldClickTheButtonWithDeviceScaleFactorSet()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions { Width = 400, Height = 400, DeviceScaleFactor = 5 });
            Assert.Equal(5, await DevToolsContext.EvaluateExpressionAsync<int>("window.devicePixelRatio"));
            await DevToolsContext.SetContentAsync("<div style=\"width:100px;height:100px\">spacer</div>");
            await FrameUtils.AttachFrameAsync(DevToolsContext, "button-test", TestConstants.ServerUrl + "/input/button.html");
            var frame = DevToolsContext.FirstChildFrame();
            var button = await frame.QuerySelectorAsync<HtmlButtonElement>("button");
            await button.ClickAsync();
            Assert.Equal("Clicked", await frame.EvaluateExpressionAsync<string>("window.result"));
        }
    }
}
