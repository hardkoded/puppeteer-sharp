using Newtonsoft.Json;
using PuppeteerSharp.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.InputTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class InputTests : PuppeteerPageBaseTest
    {
        private Task dummy;
        const string Dimensions = @"function dimensions() {
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
        public async Task ShouldTypeIntoTheTextarea()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");

            var textarea = await Page.QuerySelectorAsync("textarea");
            await textarea.TypeAsync("Type in this text!");
            Assert.Equal("Type in this text!", await Page.EvaluateExpressionAsync<string>("result"));
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
            var filePath = "./assets/file-to-upload.txt";
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
        public async Task ShouldMoveWithTheArrowKeys()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.TypeAsync("textarea", "Hello World!");
            Assert.Equal("Hello World!", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
            for (var i = 0; i < "World!".Length; i++)
            {
                dummy = Page.Keyboard.PressAsync("ArrowLeft");
            }

            await Page.Keyboard.TypeAsync("inserted ");
            Assert.Equal("Hello inserted World!", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
            dummy = Page.Keyboard.DownAsync("Shift");
            for (var i = 0; i < "inserted ".Length; i++)
            {
                dummy = Page.Keyboard.PressAsync("ArrowLeft");
            }

            dummy = Page.Keyboard.UpAsync("Shift");
            await Page.Keyboard.PressAsync("Backspace");
            Assert.Equal("Hello World!", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
        }

        [Fact]
        public async Task ShouldSendACharacterWithElementHandlePress()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            var textarea = await Page.QuerySelectorAsync("textarea");
            await textarea.PressAsync("a", new PressOptions { Text = "f" });
            Assert.Equal("f", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));

            await Page.EvaluateExpressionAsync("window.addEventListener('keydown', e => e.preventDefault(), true)");

            await textarea.PressAsync("a", new PressOptions { Text = "y" });
            Assert.Equal("f", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
        }

        [Fact]
        public async Task ShouldSendACharacterWithSendCharacter()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.FocusAsync("textarea");
            await Page.Keyboard.SendCharacterAsync("嗨");
            Assert.Equal("嗨", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
            await Page.EvaluateExpressionAsync("window.addEventListener('keydown', e => e.preventDefault(), true)");
            await Page.Keyboard.SendCharacterAsync("a");
            Assert.Equal("嗨a", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
        }

        [Fact]
        public async Task ShouldReportShiftKey()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/keyboard.html");
            var keyboard = Page.Keyboard;
            var codeForKey = new Dictionary<string, int> { ["Shift"] = 16, ["Alt"] = 18, ["Meta"] = 91, ["Control"] = 17 };
            foreach (var modifier in codeForKey)
            {
                await keyboard.DownAsync(modifier.Key);
                Assert.Equal($"Keydown: {modifier.Key} {modifier.Key}Left {modifier.Value} [{modifier.Key}]", await Page.EvaluateExpressionAsync<string>("getResult()"));
                await keyboard.DownAsync("!");
                // Shift+! will generate a keypress
                if (modifier.Key == "Shift")
                {
                    Assert.Equal($"Keydown: ! Digit1 49 [{modifier.Key}]\nKeypress: ! Digit1 33 33 33 [{modifier.Key}]", await Page.EvaluateExpressionAsync<string>("getResult()"));
                }
                else
                {
                    Assert.Equal($"Keydown: ! Digit1 49 [{modifier.Key}]", await Page.EvaluateExpressionAsync<string>("getResult()"));
                }

                await keyboard.UpAsync("!");
                Assert.Equal($"Keyup: ! Digit1 49 [{modifier.Key}]", await Page.EvaluateExpressionAsync<string>("getResult()"));
                await keyboard.UpAsync(modifier.Key);
                Assert.Equal($"Keyup: {modifier.Key} {modifier.Key}Left {modifier.Value} []", await Page.EvaluateExpressionAsync<string>("getResult()"));
            }
        }

        [Fact]
        public async Task ShouldReportMultipleModifiers()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/keyboard.html");
            var keyboard = Page.Keyboard;
            await keyboard.DownAsync("Control");
            Assert.Equal("Keydown: Control ControlLeft 17 [Control]", await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.DownAsync("Meta");
            Assert.Equal("Keydown: Meta MetaLeft 91 [Control Meta]", await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.DownAsync(";");
            Assert.Equal("Keydown: ; Semicolon 186 [Control Meta]", await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.UpAsync(";");
            Assert.Equal("Keyup: ; Semicolon 186 [Control Meta]", await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.UpAsync("Control");
            Assert.Equal("Keyup: Control ControlLeft 17 [Meta]", await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.UpAsync("Meta");
            Assert.Equal("Keyup: Meta MetaLeft 91 []", await Page.EvaluateExpressionAsync<string>("getResult()"));
        }

        [Fact]
        public async Task ShouldSendProperCodesWhileTyping()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/keyboard.html");
            await Page.Keyboard.TypeAsync("!");
            Assert.Equal(string.Join("\n", new[] {
                "Keydown: ! Digit1 49 []",
                "Keypress: ! Digit1 33 33 33 []",
                "Keyup: ! Digit1 49 []" }), await Page.EvaluateExpressionAsync<string>("getResult()"));
            await Page.Keyboard.TypeAsync("^");
            Assert.Equal(string.Join("\n", new[] {
                "Keydown: ^ Digit6 54 []",
                "Keypress: ^ Digit6 94 94 94 []",
                "Keyup: ^ Digit6 54 []" }), await Page.EvaluateExpressionAsync<string>("getResult()"));
        }

        [Fact]
        public async Task ShouldSendProperCodesWhileTypingWithShift()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/keyboard.html");
            var keyboard = Page.Keyboard;
            await keyboard.DownAsync("Shift");
            await Page.Keyboard.TypeAsync("~");
            Assert.Equal(string.Join("\n", new[] {
                "Keydown: Shift ShiftLeft 16 [Shift]",
                "Keydown: ~ Backquote 192 [Shift]", // 192 is ` keyCode
                "Keypress: ~ Backquote 126 126 126 [Shift]", // 126 is ~ charCode
                "Keyup: ~ Backquote 192 [Shift]" }), await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.UpAsync("Shift");
        }

        [Fact]
        public async Task ShouldNotTypeCanceledEvents()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.FocusAsync("textarea");
            await Page.EvaluateExpressionAsync(@"{
              window.addEventListener('keydown', event => {
                event.stopPropagation();
                event.stopImmediatePropagation();
                if (event.key === 'l')
                  event.preventDefault();
                if (event.key === 'o')
                  Promise.resolve().then(() => event.preventDefault());
              }, false);
            }");
            await Page.Keyboard.TypeAsync("Hello World!");
            Assert.Equal("He Wrd!", await Page.EvaluateExpressionAsync<string>("textarea.value"));
        }

        [Fact]
        public async Task KeyboardModifiers()
        {
            var keyboard = Page.Keyboard;
            Assert.Equal(0, keyboard.Modifiers);
            await keyboard.DownAsync(Key.Shift);
            Assert.Equal(8, keyboard.Modifiers);
            await keyboard.DownAsync(Key.Alt);
            Assert.Equal(9, keyboard.Modifiers);
            await keyboard.UpAsync(Key.Shift);
            await keyboard.UpAsync(Key.Alt);
            Assert.Equal(0, keyboard.Modifiers);
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
            Assert.Equal(dimensions.Width + 104, newDimensions.Width);
            Assert.Equal(dimensions.Height + 104, newDimensions.Height);
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
            await Page.EvaluateExpressionAsync("document.querySelector('textarea').scrollTop = 0");
            var dimensions = await Page.EvaluateFunctionAsync<Dimensions>(Dimensions);
            await Page.Mouse.MoveAsync(dimensions.X + 2, dimensions.Y + 2);
            await Page.Mouse.DownAsync();
            await Page.Mouse.MoveAsync(100, 100);
            await Page.Mouse.UpAsync();
            Assert.Equal(text, await Page.EvaluateExpressionAsync<string>("window.getSelection().toString()"));
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

        [Fact]
        public async Task ShouldSpecifyRepeatProperty()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.FocusAsync("textarea");
            await Page.EvaluateExpressionAsync("document.querySelector('textarea').addEventListener('keydown', e => window.lastEvent = e, true)");
            await Page.Keyboard.DownAsync("a");
            Assert.False(await Page.EvaluateExpressionAsync<bool>("window.lastEvent.repeat"));
            await Page.Keyboard.PressAsync("a");
            Assert.True(await Page.EvaluateExpressionAsync<bool>("window.lastEvent.repeat"));

            await Page.Keyboard.DownAsync("b");
            Assert.False(await Page.EvaluateExpressionAsync<bool>("window.lastEvent.repeat"));
            await Page.Keyboard.DownAsync("b");
            Assert.True(await Page.EvaluateExpressionAsync<bool>("window.lastEvent.repeat"));

            await Page.Keyboard.UpAsync("a");
            await Page.Keyboard.DownAsync("a");
            Assert.False(await Page.EvaluateExpressionAsync<bool>("window.lastEvent.repeat"));
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

        [Fact]
        public async Task ShouldTapTheButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.TapAsync("button");
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [Fact(Skip = "test is ignored")]
        public async Task ShouldReportTouches()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/touches.html");
            var button = await Page.QuerySelectorAsync("button");
            await button.TapAsync();
            Assert.Equal(new object[] {
                new { Touchstart = 0 },
                new { Touchend = 0 }
            }, await Page.EvaluateExpressionAsync("getResult()"));
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
        public async Task ShouldTypeAllKindsOfCharacters()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.FocusAsync("textarea");
            const string text = "This text goes onto two lines.\nThis character is 嗨.";
            await Page.Keyboard.TypeAsync(text);
            Assert.Equal(text, await Page.EvaluateExpressionAsync<string>("result"));
        }

        [Fact]
        public async Task ShouldSpecifyLocation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.EvaluateExpressionAsync(@"{
              window.addEventListener('keydown', event => window.keyLocation = event.location, true);
            }");
            var textarea = await Page.QuerySelectorAsync("textarea");

            await textarea.PressAsync("Digit5");
            Assert.Equal(0, await Page.EvaluateExpressionAsync<int>("keyLocation"));

            await textarea.PressAsync("ControlLeft");
            Assert.Equal(1, await Page.EvaluateExpressionAsync<int>("keyLocation"));

            await textarea.PressAsync("ControlRight");
            Assert.Equal(2, await Page.EvaluateExpressionAsync<int>("keyLocation"));

            await textarea.PressAsync("NumpadSubtract");
            Assert.Equal(3, await Page.EvaluateExpressionAsync<int>("keyLocation"));
        }

        [Fact]
        public async Task ShouldThrowOnUnknownKeys()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => Page.Keyboard.PressAsync("NotARealKey"));

            await Assert.ThrowsAsync<KeyNotFoundException>(() => Page.Keyboard.PressAsync("ё"));

            await Assert.ThrowsAsync<KeyNotFoundException>(() => Page.Keyboard.PressAsync("😊"));
        }

        [Fact]
        public async Task ShouldTypeEmoji()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.TypeAsync("textarea", "👹 Tokyo street Japan \uD83C\uDDEF\uD83C\uDDF5");
            Assert.Equal(
                "👹 Tokyo street Japan \uD83C\uDDEF\uD83C\uDDF5",
                await Page.QuerySelectorAsync("textarea").EvaluateFunctionAsync<string>("t => t.value"));
        }

        [Fact]
        public async Task ShouldTypeEmojiIntoAniframe()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "emoji-test", TestConstants.ServerUrl + "/input/textarea.html");
            var frame = Page.FirstChildFrame();
            var textarea = await frame.QuerySelectorAsync("textarea");
            await textarea.TypeAsync("👹 Tokyo street Japan \uD83C\uDDEF\uD83C\uDDF5");
            Assert.Equal(
                "👹 Tokyo street Japan \uD83C\uDDEF\uD83C\uDDF5",
                await frame.QuerySelectorAsync("textarea").EvaluateFunctionAsync<string>("t => t.value"));
        }
    }
}