using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Input;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.KeyboardTests
{
    public class KeyboardTests : PuppeteerPageBaseTest
    {
        public KeyboardTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("keyboard.spec", "Keyboard", "should type into a textarea")]
        public async Task ShouldTypeIntoTheTextarea()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");

            var textarea = await Page.QuerySelectorAsync("textarea");
            await textarea.TypeAsync("Type in this text!");
            Assert.AreEqual("Type in this text!", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [Test, Retry(2), PuppeteerTest("keyboard.spec", "Keyboard", "should move with the arrow keys")]
        public async Task ShouldMoveWithTheArrowKeys()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.TypeAsync("textarea", "Hello World!");
            Assert.AreEqual("Hello World!", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
            for (var i = 0; i < "World!".Length; i++)
            {
                _ = Page.Keyboard.PressAsync("ArrowLeft");
            }

            await Page.Keyboard.TypeAsync("inserted ");
            Assert.AreEqual("Hello inserted World!", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
            _ = Page.Keyboard.DownAsync("Shift");
            for (var i = 0; i < "inserted ".Length; i++)
            {
                _ = Page.Keyboard.PressAsync("ArrowLeft");
            }

            _ = Page.Keyboard.UpAsync("Shift");
            await Page.Keyboard.PressAsync("Backspace");
            Assert.AreEqual("Hello World!", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
        }

        [Test, Retry(2), PuppeteerTest("keyboard.spec", "Keyboard", "should send a character with ElementHandle.press")]
        public async Task ShouldSendACharacterWithElementHandlePress()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            var textarea = await Page.QuerySelectorAsync("textarea");
            await textarea.PressAsync("a");
            Assert.AreEqual("a", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));

            await Page.EvaluateExpressionAsync("window.addEventListener('keydown', e => e.preventDefault(), true)");

            await textarea.PressAsync("b");
            Assert.AreEqual("a", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
        }

        [Test, Retry(2), PuppeteerTest("keyboard.spec", "Keyboard", "ElementHandle.press should not support |text| option")]
        public async Task ElementHandlePressShouldSupportTextOption()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            var textarea = await Page.QuerySelectorAsync("textarea");
            await textarea.PressAsync("a", new PressOptions { Text = "ё" });
            Assert.AreEqual("a", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
        }

        [Test, Retry(2), PuppeteerTest("keyboard.spec", "Keyboard", "should send a character with sendCharacter")]
        public async Task ShouldSendACharacterWithSendCharacter()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.FocusAsync("textarea");
            await Page.Keyboard.SendCharacterAsync("嗨");
            Assert.AreEqual("嗨", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
            await Page.EvaluateExpressionAsync("window.addEventListener('keydown', e => e.preventDefault(), true)");
            await Page.Keyboard.SendCharacterAsync("a");
            Assert.AreEqual("嗨a", await Page.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
        }

        [Test, Retry(2), PuppeteerTest("keyboard.spec", "Keyboard", "should report shiftKey")]
        public async Task ShouldReportShiftKey()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/keyboard.html");
            var keyboard = Page.Keyboard;
            var codeForKey = new Dictionary<string, int> { ["Shift"] = 16, ["Alt"] = 18, ["Control"] = 17 };
            foreach (var modifier in codeForKey)
            {
                await keyboard.DownAsync(modifier.Key);
                Assert.AreEqual($"Keydown: {modifier.Key} {modifier.Key}Left {modifier.Value} [{modifier.Key}]", await Page.EvaluateExpressionAsync<string>("getResult()"));
                await keyboard.DownAsync("!");
                // Shift+! will generate a keypress
                if (modifier.Key == "Shift")
                {
                    Assert.AreEqual($"Keydown: ! Digit1 49 [{modifier.Key}]\nKeypress: ! Digit1 33 33 [{modifier.Key}]", await Page.EvaluateExpressionAsync<string>("getResult()"));
                }
                else
                {
                    Assert.AreEqual($"Keydown: ! Digit1 49 [{modifier.Key}]", await Page.EvaluateExpressionAsync<string>("getResult()"));
                }

                await keyboard.UpAsync("!");
                Assert.AreEqual($"Keyup: ! Digit1 49 [{modifier.Key}]", await Page.EvaluateExpressionAsync<string>("getResult()"));
                await keyboard.UpAsync(modifier.Key);
                Assert.AreEqual($"Keyup: {modifier.Key} {modifier.Key}Left {modifier.Value} []", await Page.EvaluateExpressionAsync<string>("getResult()"));
            }
        }

        [Test, Retry(2), PuppeteerTest("keyboard.spec", "Keyboard", "should report multiple modifiers")]
        public async Task ShouldReportMultipleModifiers()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/keyboard.html");
            var keyboard = Page.Keyboard;
            await keyboard.DownAsync("Control");
            Assert.AreEqual("Keydown: Control ControlLeft 17 [Control]", await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.DownAsync("Alt");
            Assert.AreEqual("Keydown: Alt AltLeft 18 [Alt Control]", await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.DownAsync(";");
            Assert.AreEqual("Keydown: ; Semicolon 186 [Alt Control]", await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.UpAsync(";");
            Assert.AreEqual("Keyup: ; Semicolon 186 [Alt Control]", await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.UpAsync("Control");
            Assert.AreEqual("Keyup: Control ControlLeft 17 [Alt]", await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.UpAsync("Alt");
            Assert.AreEqual("Keyup: Alt AltLeft 18 []", await Page.EvaluateExpressionAsync<string>("getResult()"));
        }

        [Test, Retry(2), PuppeteerTest("keyboard.spec", "Keyboard", "should send proper codes while typing")]
        public async Task ShouldSendProperCodesWhileTyping()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/keyboard.html");
            await Page.Keyboard.TypeAsync("!");
            Assert.AreEqual(string.Join("\n", new[] {
                "Keydown: ! Digit1 49 []",
                "Keypress: ! Digit1 33 33 []",
                "Keyup: ! Digit1 49 []" }), await Page.EvaluateExpressionAsync<string>("getResult()"));
            await Page.Keyboard.TypeAsync("^");
            Assert.AreEqual(string.Join("\n", new[] {
                "Keydown: ^ Digit6 54 []",
                "Keypress: ^ Digit6 94 94 []",
                "Keyup: ^ Digit6 54 []" }), await Page.EvaluateExpressionAsync<string>("getResult()"));
        }

        [Test, Retry(2), PuppeteerTest("keyboard.spec", "Keyboard", "should send proper codes while typing with shift")]
        public async Task ShouldSendProperCodesWhileTypingWithShift()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/keyboard.html");
            var keyboard = Page.Keyboard;
            await keyboard.DownAsync("Shift");
            await Page.Keyboard.TypeAsync("~");
            Assert.AreEqual(string.Join("\n", new[] {
                "Keydown: Shift ShiftLeft 16 [Shift]",
                "Keydown: ~ Backquote 192 [Shift]", // 192 is ` keyCode
                "Keypress: ~ Backquote 126 126 [Shift]", // 126 is ~ charCode
                "Keyup: ~ Backquote 192 [Shift]" }), await Page.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.UpAsync("Shift");
        }

        [Test, Retry(2), PuppeteerTest("keyboard.spec", "Keyboard", "should not type canceled events")]
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
            Assert.AreEqual("He Wrd!", await Page.EvaluateExpressionAsync<string>("textarea.value"));
        }

        [Test, Retry(2), PuppeteerTest("keyboard.spec", "Keyboard", "should specify repeat property")]
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

        [Test, Retry(2), PuppeteerTest("keyboard.spec", "Keyboard", "should type all kinds of characters")]
        public async Task ShouldTypeAllKindsOfCharacters()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.FocusAsync("textarea");
            const string text = "This text goes onto two lines.\nThis character is 嗨.";
            await Page.Keyboard.TypeAsync(text);
            Assert.AreEqual(text, await Page.EvaluateExpressionAsync<string>("result"));
        }

        [Test, Retry(2), PuppeteerTest("keyboard.spec", "Keyboard", "should specify location")]
        public async Task ShouldSpecifyLocation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.EvaluateExpressionAsync(@"{
              window.addEventListener('keydown', event => window.keyLocation = event.location, true);
            }");
            var textarea = await Page.QuerySelectorAsync("textarea");

            await textarea.PressAsync("Digit5");
            Assert.AreEqual(0, await Page.EvaluateExpressionAsync<int>("keyLocation"));

            await textarea.PressAsync("ControlLeft");
            Assert.AreEqual(1, await Page.EvaluateExpressionAsync<int>("keyLocation"));

            await textarea.PressAsync("ControlRight");
            Assert.AreEqual(2, await Page.EvaluateExpressionAsync<int>("keyLocation"));

            await textarea.PressAsync("NumpadSubtract");
            Assert.AreEqual(3, await Page.EvaluateExpressionAsync<int>("keyLocation"));
        }

        [Test, Retry(2), PuppeteerTest("keyboard.spec", "Keyboard", "should throw on unknown keys")]
        public void ShouldThrowOnUnknownKeys()
        {
            Assert.ThrowsAsync<KeyNotFoundException>(() => Page.Keyboard.PressAsync("NotARealKey"));

            Assert.ThrowsAsync<KeyNotFoundException>(() => Page.Keyboard.PressAsync("ё"));

            Assert.ThrowsAsync<KeyNotFoundException>(() => Page.Keyboard.PressAsync("😊"));
        }

        [Test, Retry(2), PuppeteerTest("keyboard.spec", "Keyboard", "should type emoji")]
        public async Task ShouldTypeEmoji()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.TypeAsync("textarea", "👹 Tokyo street Japan \uD83C\uDDEF\uD83C\uDDF5");
            Assert.AreEqual(
                "👹 Tokyo street Japan \uD83C\uDDEF\uD83C\uDDF5",
                await Page.QuerySelectorAsync("textarea").EvaluateFunctionAsync<string>("t => t.value"));
        }

        [Test, Retry(2), PuppeteerTest("keyboard.spec", "Keyboard", "should type emoji into an iframe")]
        public async Task ShouldTypeEmojiIntoAniframe()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "emoji-test", TestConstants.ServerUrl + "/input/textarea.html");
            var frame = Page.FirstChildFrame();
            var textarea = await frame.QuerySelectorAsync("textarea");
            await textarea.TypeAsync("👹 Tokyo street Japan \uD83C\uDDEF\uD83C\uDDF5");
            Assert.AreEqual(
                "👹 Tokyo street Japan \uD83C\uDDEF\uD83C\uDDF5",
                await frame.QuerySelectorAsync("textarea").EvaluateFunctionAsync<string>("t => t.value"));
        }

        [Test, Retry(2), PuppeteerTest("keyboard.spec", "Keyboard", "should press the metaKey")]
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
            Assert.AreEqual("Meta", result[key]);
            Assert.AreEqual("MetaLeft", result[code]);
            Assert.AreEqual(true, result[metaKey]);
        }
    }
}
