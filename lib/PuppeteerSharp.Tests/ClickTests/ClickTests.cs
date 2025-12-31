using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Input;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ClickTests
{
    public class ClickTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("click.spec", "Page.click", "should click the button")]
        public async Task ShouldClickTheButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.ClickAsync("button");
            Assert.That(await Page.EvaluateExpressionAsync<string>("result"), Is.EqualTo("Clicked"));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should click svg")]
        public async Task ShouldClickSvg()
        {
            await Page.SetContentAsync($@"
                <svg height=""100"" width=""100"">
                  <circle onclick=""javascript:window.__CLICKED=42"" cx=""50"" cy=""50"" r=""40"" stroke=""black"" stroke-width=""3"" fill=""red""/>
                </svg>
            ");
            await Page.ClickAsync("circle");
            Assert.That(await Page.EvaluateFunctionAsync<int>("() => window.__CLICKED"), Is.EqualTo(42));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should click the button if window.Node is removed")]
        public async Task ShouldClickTheButtonIfWindowNodeIsRemoved()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.EvaluateExpressionAsync("delete window.Node");
            await Page.ClickAsync("button");
            Assert.That(await Page.EvaluateExpressionAsync<string>("result"), Is.EqualTo("Clicked"));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should click on a span with an inline element inside")]
        [Ignore("See https://github.com/GoogleChrome/puppeteer/issues/4281")]
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
            Assert.That(await Page.EvaluateFunctionAsync<int>("() => window.CLICKED"), Is.EqualTo(42));
        }

        /// <summary>
        /// This test is called ShouldNotThrowUnhandledPromiseRejectionWhenPageCloses in puppeteer.
        /// </summary>
        [Test, PuppeteerTest("click.spec", "Page.click", "should not throw UnhandledPromiseRejection when page closes")]
        [Ignore("We don't need this test")]
        public async Task ShouldGracefullyFailWhenPageCloses()
        {
            var newPage = await Browser.NewPageAsync();
            await Task.WhenAll(
                newPage.CloseAsync(),
                newPage.Mouse.ClickAsync(1, 2));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should click the button after navigation")]
        public async Task ShouldClickTheButtonAfterNavigation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.ClickAsync("button");
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.ClickAsync("button");
            Assert.That(await Page.EvaluateExpressionAsync<string>("result"), Is.EqualTo("Clicked"));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should click with disabled javascript")]
        public async Task ShouldClickWithDisabledJavascript()
        {
            await Page.SetJavaScriptEnabledAsync(false);
            await Page.GoToAsync(TestConstants.ServerUrl + "/wrappedlink.html");
            await Task.WhenAll(
                Page.ClickAsync("a"),
                Page.WaitForNavigationAsync()
            );
            Assert.That(Page.Url, Is.EqualTo(TestConstants.ServerUrl + "/wrappedlink.html#clicked"));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should scroll and click with disabled javascript")]
        public async Task ShouldScrollAndClickWithDisabledJavascript()
        {
            await Page.SetJavaScriptEnabledAsync(false);
            await Page.GoToAsync(TestConstants.ServerUrl + "/wrappedlink.html");
            var body = await Page.WaitForSelectorAsync("body");
            await body.EvaluateFunctionAsync("body => body.style.paddingTop = '3000px'", body);
            await Task.WhenAll(
                Page.ClickAsync("a"),
                Page.WaitForNavigationAsync()
            );
            Assert.That(Page.Url, Is.EqualTo(TestConstants.ServerUrl + "/wrappedlink.html#clicked"));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should click when one of inline box children is outside of viewport")]
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
            Assert.That(await Page.EvaluateFunctionAsync<int>("() => window.CLICKED"), Is.EqualTo(42));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should select the text by triple clicking")]
        public async Task ShouldSelectTheTextByTripleClicking()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.FocusAsync("textarea");
            const string text = "This is the text that we are going to try to select. Let's see how it goes.";
            await Page.Keyboard.TypeAsync(text);
            await Page.ClickAsync("textarea");
            await Page.ClickAsync("textarea", new ClickOptions { Count = 2 });
            await Page.ClickAsync("textarea", new ClickOptions { Count = 3 });
            Assert.That(await Page.EvaluateFunctionAsync<string>(@"() => {
                const textarea = document.querySelector('textarea');
                return textarea.value.substring(
                    textarea.selectionStart,
                    textarea.selectionEnd
                );
            }"), Is.EqualTo(text));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should click offscreen buttons")]
        public async Task ShouldClickOffscreenButtons()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/offscreenbuttons.html");
            var messages = new List<string>();
            Page.Console += (_, e) => messages.Add(e.Message.Text);

            for (var i = 0; i < 11; ++i)
            {
                // We might've scrolled to click a button - reset to (0, 0).
                await Page.EvaluateFunctionAsync("() => window.scrollTo(0, 0)");
                await Page.ClickAsync($"#btn{i}");
            }

            // It seems that the console event is coming a little bit late
            await Task.Delay(500);

            Assert.That(messages, Is.EqualTo(new List<string>
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
            }));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should click wrapped links")]
        public async Task ShouldClickWrappedLinks()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/wrappedlink.html");
            await Page.ClickAsync("a");
            Assert.That(await Page.EvaluateExpressionAsync<bool>("window.__clicked"), Is.True);
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should click on checkbox input and toggle")]
        public async Task ShouldClickOnCheckboxInputAndToggle()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            Assert.That(await Page.EvaluateExpressionAsync("result.check"), Is.Null);
            await Page.ClickAsync("input#agree");
            Assert.That(await Page.EvaluateExpressionAsync<bool>("result.check"), Is.True);
            Assert.That(await Page.EvaluateExpressionAsync<string[]>("result.events"),
                Is.EqualTo(new[] {
                    "mouseover",
                    "mouseenter",
                    "mousemove",
                    "mousedown",
                    "mouseup",
                    "click",
                    "input",
                    "change"
            }));
            await Page.ClickAsync("input#agree");
            Assert.That(await Page.EvaluateExpressionAsync<bool>("result.check"), Is.False);
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should click on checkbox label and toggle")]
        public async Task ShouldClickOnCheckboxLabelAndToggle()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            Assert.That(await Page.EvaluateExpressionAsync("result.check"), Is.Null);
            await Page.ClickAsync("label[for=\"agree\"]");
            Assert.That(await Page.EvaluateExpressionAsync<bool>("result.check"), Is.True);
            Assert.That(await Page.EvaluateExpressionAsync<string[]>("result.events"),
                Is.EqualTo(new[] {
                    "click",
                    "input",
                    "change"
            }));
            await Page.ClickAsync("label[for=\"agree\"]");
            Assert.That(await Page.EvaluateExpressionAsync<bool>("result.check"), Is.False);
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should fail to click a missing button")]
        public async Task ShouldFailToClickAMissingButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var exception = Assert.ThrowsAsync<SelectorException>(()
                => Page.ClickAsync("button.does-not-exist"));
            Assert.That(exception.Message, Is.EqualTo("No node found for selector: button.does-not-exist"));
            Assert.That(exception.Selector, Is.EqualTo("button.does-not-exist"));
        }

        // https://github.com/GoogleChrome/puppeteer/issues/161
        [Test, PuppeteerTest("click.spec", "Page.click", "should not hang with touch-enabled viewports")]
        public async Task ShouldNotHangWithTouchEnabledViewports()
        {
            await Page.SetViewportAsync(TestConstants.IPhone.ViewPort);
            await Page.Mouse.DownAsync();
            await Page.Mouse.MoveAsync(100, 10);
            await Page.Mouse.UpAsync();
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should scroll and click the button")]
        public async Task ShouldScrollAndClickTheButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.ClickAsync("#button-5");
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.querySelector(\"#button-5\").textContent"), Is.EqualTo("clicked"));
            await Page.ClickAsync("#button-80");
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.querySelector(\"#button-80\").textContent"), Is.EqualTo("clicked"));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should double click the button")]
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
            await button.ClickAsync(new ClickOptions { Count = 2 });
            Assert.That(await Page.EvaluateExpressionAsync<bool>("double"), Is.True);
            Assert.That(await Page.EvaluateExpressionAsync<string>("result"), Is.EqualTo("Clicked"));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should click a partially obscured button")]
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
            Assert.That(await Page.EvaluateExpressionAsync<string>("result"), Is.EqualTo("Clicked"));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should click a rotated button")]
        public async Task ShouldClickARotatedButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/rotatedButton.html");
            await Page.ClickAsync("button");
            Assert.That(await Page.EvaluateExpressionAsync<string>("result"), Is.EqualTo("Clicked"));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should fire contextmenu event on right click")]
        public async Task ShouldFireContextmenuEventOnRightClick()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.ClickAsync("#button-8", new ClickOptions { Button = MouseButton.Right });
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.querySelector('#button-8').textContent"), Is.EqualTo("context menu"));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should fire aux event on middle click")]
        public async Task ShouldFireAuxEventOnMiddleClick()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.ClickAsync("#button-8", new ClickOptions { Button = MouseButton.Middle });
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.querySelector('#button-8').textContent"), Is.EqualTo("aux click"));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should fire back click")]
        public async Task ShouldFireBackClick()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.ClickAsync("#button-8", new ClickOptions { Button = MouseButton.Back });
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.querySelector('#button-8').textContent"), Is.EqualTo("back click"));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should fire forward click")]
        public async Task ShouldFireForwardClick()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.ClickAsync("#button-8", new ClickOptions { Button = MouseButton.Forward });
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.querySelector('#button-8').textContent"), Is.EqualTo("forward click"));
        }

        // @see https://github.com/GoogleChrome/puppeteer/issues/206
        [Test, PuppeteerTest("click.spec", "Page.click", "should click links which cause navigation")]
        public async Task ShouldClickLinksWhichCauseNavigation()
        {
            await Page.SetContentAsync($"<a href=\"{TestConstants.EmptyPage}\">empty.html</a>");
            // This await should not hang.
            await Page.ClickAsync("a");
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should click the button inside an iframe")]
        public async Task ShouldClickTheButtonInsideAnIframe()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync("<div style=\"width:100px;height:100px\">spacer</div>");
            await FrameUtils.AttachFrameAsync(Page, "button-test", TestConstants.ServerUrl + "/input/button.html");
            var frame = Page.FirstChildFrame();
            var button = await frame.QuerySelectorAsync("button");
            await button.ClickAsync();
            Assert.That(await frame.EvaluateExpressionAsync<string>("window.result"), Is.EqualTo("Clicked"));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should click the button with fixed position inside an iframe")]
        [Ignore("see https://github.com/GoogleChrome/puppeteer/issues/4110")]
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
            Assert.That(await frame.EvaluateExpressionAsync<string>("window.result"), Is.EqualTo("Clicked"));
        }

        [Test, PuppeteerTest("click.spec", "Page.click", "should click the button with deviceScaleFactor set")]
        public async Task ShouldClickTheButtonWithDeviceScaleFactorSet()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 400, Height = 400, DeviceScaleFactor = 5 });
            Assert.That(await Page.EvaluateExpressionAsync<int>("window.devicePixelRatio"), Is.EqualTo(5));
            await Page.SetContentAsync("<div style=\"width:100px;height:100px\">spacer</div>");
            await FrameUtils.AttachFrameAsync(Page, "button-test", TestConstants.ServerUrl + "/input/button.html");
            var frame = Page.FirstChildFrame();
            var button = await frame.QuerySelectorAsync("button");
            await button.ClickAsync();
            Assert.That(await frame.EvaluateExpressionAsync<string>("window.result"), Is.EqualTo("Clicked"));
        }
    }
}
