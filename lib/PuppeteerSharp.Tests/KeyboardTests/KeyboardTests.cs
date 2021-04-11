using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Input;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.KeyboardTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class KeyboardTests : PuppeteerPageBaseTest
    {
        public KeyboardTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should type into a textarea")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldTypeIntoTheTextarea()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");

            var textarea = await Page.QuerySelectorAsync("textarea");
            await textarea.TypeAsync("Type in this text!");
            Assert.Equal("Type in this text!", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should move with the arrow keys")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldMoveWithTheArrowKeys()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.TypeAsync("textarea", "Hello World!");
            Assert.Equal("Hello World!", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
            for (var i = 0; i < "World!".Length; i++)
            {
                _ = Page.Keyboard.PressAsync("ArrowLeft");
            }

            await Page.Keyboard.TypeAsync("inserted ");
            Assert.Equal("Hello inserted World!", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
            _ = Page.Keyboard.DownAsync("Shift");
            for (var i = 0; i < "inserted ".Length; i++)
            {
                _ = Page.Keyboard.PressAsync("ArrowLeft");
            }

            _ = Page.Keyboard.UpAsync("Shift");
            await Page.Keyboard.PressAsync("Backspace");
            Assert.Equal("Hello World!", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should send a character with ElementHandle.press")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldSendACharacterWithElementHandlePress()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            var textarea = await Page.QuerySelectorAsync("textarea");
            await textarea.PressAsync("a");
            Assert.Equal("a", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));

            await Page.EvaluateExpressionAsync("window.addEventListener('keydown', e => e.preventDefault(), true)");

            await textarea.PressAsync("b");
            Assert.Equal("a", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "ElementHandle.press should support |text| option")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ElementHandlePressShouldSupportTextOption()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            var textarea = await Page.QuerySelectorAsync("textarea");
            await textarea.PressAsync("a", new PressOptions { Text = "ё" });
            Assert.Equal("ё", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should send a character with sendCharacter")]
        [SkipBrowserFact(skipFirefox: true)]
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

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should report shiftKey")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldReportShiftKey()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/keyboard.html");
            var keyboard = Page.Keyboard;
            var codeForKey = new Dictionary<string, int> { ["Shift"] = 16, ["Alt"] = 18, ["Control"] = 17 };
            foreach (var modifier in codeForKey)
            {
                await keyboard.DownAsync(modifier.Key);
                Assert.Equal($"Keydown: {modifier.Key} {modifier.Key}Left {modifier.Value} [{modifier.Key}]", await Page.EvaluateExpressionAsync<string>("getResult()"));
                await keyboard.DownAsync("!");
                // Shift+! will generate a keypress
                if (modifier.Key == "Shift")
                {
                    Assert.Equal($"Keydown: ! Digit1 49 [{modifier.Key}]\nKeypress: ! Digit1 33 33 [{modifier.Key}]", await Page.EvaluateExpressionAsync<string>("getResult()"));
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

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should report multiple modifiers")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldReportMultipleModifiers()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/keyboard.html");
            var keyboard = Page.Keyboard;
            await keyboard.DownAsync("Control");
            Assert.Equal("Keydown: Control ControlLeft 17 [Control]", await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.DownAsync("Alt");
            Assert.Equal("Keydown: Alt AltLeft 18 [Alt Control]", await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.DownAsync(";");
            Assert.Equal("Keydown: ; Semicolon 186 [Alt Control]", await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.UpAsync(";");
            Assert.Equal("Keyup: ; Semicolon 186 [Alt Control]", await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.UpAsync("Control");
            Assert.Equal("Keyup: Control ControlLeft 17 [Alt]", await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.UpAsync("Alt");
            Assert.Equal("Keyup: Alt AltLeft 18 []", await Page.EvaluateExpressionAsync<string>("getResult()"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should send proper codes while typing")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldSendProperCodesWhileTyping()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/keyboard.html");
            await Page.Keyboard.TypeAsync("!");
            Assert.Equal(string.Join("\n", new[] {
                "Keydown: ! Digit1 49 []",
                "Keypress: ! Digit1 33 33 []",
                "Keyup: ! Digit1 49 []" }), await Page.EvaluateExpressionAsync<string>("getResult()"));
            await Page.Keyboard.TypeAsync("^");
            Assert.Equal(string.Join("\n", new[] {
                "Keydown: ^ Digit6 54 []",
                "Keypress: ^ Digit6 94 94 []",
                "Keyup: ^ Digit6 54 []" }), await Page.EvaluateExpressionAsync<string>("getResult()"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should send proper codes while typing with shift")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldSendProperCodesWhileTypingWithShift()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/keyboard.html");
            var keyboard = Page.Keyboard;
            await keyboard.DownAsync("Shift");
            await Page.Keyboard.TypeAsync("~");
            Assert.Equal(string.Join("\n", new[] {
                "Keydown: Shift ShiftLeft 16 [Shift]",
                "Keydown: ~ Backquote 192 [Shift]", // 192 is ` keyCode
                "Keypress: ~ Backquote 126 126 [Shift]", // 126 is ~ charCode
                "Keyup: ~ Backquote 192 [Shift]" }), await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.UpAsync("Shift");
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should not type canceled events")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
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
                  event.preventDefault();
              }, false);
            }");
            await Page.Keyboard.TypeAsync("Hello World!");
            Assert.Equal("He Wrd!", await Page.EvaluateExpressionAsync<string>("textarea.value"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should specify repeat property")]
        [SkipBrowserFact(skipFirefox: true)]
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

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should type all kinds of characters")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldTypeAllKindsOfCharacters()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.FocusAsync("textarea");
            const string text = "This text goes onto two lines.\nThis character is 嗨.";
            await Page.Keyboard.TypeAsync(text);
            Assert.Equal(text, await Page.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should specify location")]
        [SkipBrowserFact(skipFirefox: true)]
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

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should throw on unknown keys")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldThrowOnUnknownKeys()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => Page.Keyboard.PressAsync("NotARealKey"));

            await Assert.ThrowsAsync<KeyNotFoundException>(() => Page.Keyboard.PressAsync("ё"));

            await Assert.ThrowsAsync<KeyNotFoundException>(() => Page.Keyboard.PressAsync("😊"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should type emoji")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldTypeEmoji()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.TypeAsync("textarea", "👹 Tokyo street Japan \uD83C\uDDEF\uD83C\uDDF5");
            Assert.Equal(
                "👹 Tokyo street Japan \uD83C\uDDEF\uD83C\uDDF5",
                await Page.QuerySelectorAsync("textarea").EvaluateFunctionAsync<string>("t => t.value"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should type emoji into an iframe")]
        [SkipBrowserFact(skipFirefox: true)]
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

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should press the meta key")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldPressTheMetaKey()
        {
            await Page.EvaluateFunctionAsync(@"() =>
            {
                window.result = null;
                document.addEventListener('keydown', event => {
                    window.result = [event.key, event.code, event.metaKey];
                });
            }");
            await Page.Keyboard.PressAsync("Meta");
            const int key = 0;
            const int code = 1;
            const int metaKey = 2;
            var result = await Page.EvaluateExpressionAsync<object[]>("result");
            Assert.Equal("Meta", result[key]);
            Assert.Equal("MetaLeft", result[code]);
            Assert.Equal(true, result[metaKey]);
        }
    }
}
