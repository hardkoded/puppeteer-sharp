using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp.Input
{
    /// <summary>
    /// Keyboard provides an api for managing a virtual keyboard. The high level api is <see cref="TypeAsync(string, TypeOptions)"/>, which takes raw characters and generates proper keydown, keypress/input, and keyup events on your page.
    ///
    /// For finer control, you can use <see cref="Keyboard.DownAsync(string, DownOptions)"/>, <see cref="UpAsync(string)"/>, and <see cref="SendCharacterAsync(string)"/> to manually fire events as if they were generated from a real keyboard.
    /// </summary>
    public class Keyboard
    {
        private readonly CDPSession _client;
        private readonly HashSet<string> _pressedKeys = new HashSet<string>();

        internal Keyboard(CDPSession client)
        {
            _client = client;
        }

        internal int Modifiers { get; set; }

        /// <summary>
        /// Dispatches a <c>keydown</c> event
        /// </summary>
        /// <param name="key">Name of key to press, such as <c>ArrowLeft</c>. <see cref="KeyDefinitions"/> for a list of all key names.</param>
        /// <param name="options">down options</param>
        /// <remarks>
        /// If <c>key</c> is a single character and no modifier keys besides <c>Shift</c> are being held down, a <c>keypress</c>/<c>input</c> event will also generated. The <c>text</c> option can be specified to force an input event to be generated.
        /// If <c>key</c> is a modifier key, <c>Shift</c>, <c>Meta</c>, <c>Control</c>, or <c>Alt</c>, subsequent key presses will be sent with that modifier active. To release the modifier key, use <see cref="UpAsync(string)"/>
        /// After the key is pressed once, subsequent calls to <see cref="DownAsync(string, DownOptions)"/> will have <see href="https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/repeat">repeat</see> set to <c>true</c>. To release the key, use <see cref="UpAsync(string)"/>
        /// </remarks>
        /// <returns>Task</returns>
        public Task DownAsync(string key, DownOptions options = null)
        {
            var description = KeyDescriptionForString(key);

            var autoRepeat = _pressedKeys.Contains(description.Code);
            _pressedKeys.Add(description.Code);
            Modifiers |= ModifierBit(key);

            var text = options?.Text == null ? description.Text : options.Text;

            return _client.SendAsync("Input.dispatchKeyEvent", new InputDispatchKeyEventRequest
            {
                Type = text != null ? DispatchKeyEventType.KeyDown : DispatchKeyEventType.RawKeyDown,
                Modifiers = Modifiers,
                WindowsVirtualKeyCode = description.KeyCode,
                Code = description.Code,
                Key = description.Key,
                Text = text,
                UnmodifiedText = text,
                AutoRepeat = autoRepeat,
                Location = description.Location,
                IsKeypad = description.Location == 3
            });
        }

        /// <summary>
        /// Dispatches a <c>keyup</c> event.
        /// </summary>
        /// <param name="key">Name of key to release, such as `ArrowLeft`. See <see cref="KeyDefinitions"/> for a list of all key names.</param>
        /// <returns>Task</returns>
        public Task UpAsync(string key)
        {
            var description = KeyDescriptionForString(key);

            Modifiers &= ~ModifierBit(key);
            _pressedKeys.Remove(description.Code);

            return _client.SendAsync("Input.dispatchKeyEvent", new InputDispatchKeyEventRequest
            {
                Type = DispatchKeyEventType.KeyUp,
                Modifiers = Modifiers,
                Key = description.Key,
                WindowsVirtualKeyCode = description.KeyCode,
                Code = description.Code,
                Location = description.Location
            });
        }

        /// <summary>
        /// Dispatches a <c>keypress</c> and <c>input</c> event. This does not send a <c>keydown</c> or <c>keyup</c> event.
        /// </summary>
        /// <param name="charText">Character to send into the page</param>
        /// <returns>Task</returns>
        public Task SendCharacterAsync(string charText)
            => _client.SendAsync("Input.insertText", new InputInsertTextRequest
            {
                Text = charText
            });

        /// <summary>
        /// Sends a <c>keydown</c>, <c>keypress</c>/<c>input</c>, and <c>keyup</c> event for each character in the text.
        /// </summary>
        /// <param name="text">A text to type into a focused element</param>
        /// <param name="options">type options</param>
        /// <remarks>
        /// To press a special key, like <c>Control</c> or <c>ArrowDown</c>, use <see cref="PressAsync(string, PressOptions)"/>
        /// </remarks>
        /// <returns>Task</returns>
        public async Task TypeAsync(string text, TypeOptions options = null)
        {
            var delay = 0;
            if (options?.Delay != null)
            {
                delay = (int)options.Delay;
            }

            var textParts = StringInfo.GetTextElementEnumerator(text);
            while (textParts.MoveNext())
            {
                var letter = textParts.Current;
                if (KeyDefinitions.ContainsKey(letter.ToString()))
                {
                    await PressAsync(letter.ToString(), new PressOptions { Delay = delay }).ConfigureAwait(false);
                }
                else
                {
                    if (delay > 0)
                    {
                        await Task.Delay(delay).ConfigureAwait(false);
                    }
                    await SendCharacterAsync(letter.ToString()).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Shortcut for <see cref="DownAsync(string, DownOptions)"/> and <see cref="UpAsync(string)"/>
        /// </summary>
        /// <param name="key">Name of key to press, such as <c>ArrowLeft</c>. <see cref="KeyDefinitions"/> for a list of all key names.</param>
        /// <param name="options">press options</param>
        /// <remarks>
        /// If <paramref name="key"/> is a single character and no modifier keys besides <c>Shift</c> are being held down, a <c>keypress</c>/<c>input</c> event will also generated. The <see cref="DownOptions.Text"/> option can be specified to force an input event to be generated.
        /// Modifier keys DO effect <see cref="ElementHandle.PressAsync(string, PressOptions)"/>. Holding down <c>Shift</c> will type the text in upper case.
        /// </remarks>
        /// <returns>Task</returns>
        public async Task PressAsync(string key, PressOptions options = null)
        {
            await DownAsync(key, options).ConfigureAwait(false);
            if (options?.Delay > 0)
            {
                await Task.Delay((int)options.Delay).ConfigureAwait(false);
            }
            await UpAsync(key).ConfigureAwait(false);
        }

        private int ModifierBit(string key)
        {
            if (key == "Alt")
            {
                return 1;
            }
            if (key == "Control")
            {
                return 2;
            }
            if (key == "Meta")
            {
                return 4;
            }
            if (key == "Shift")
            {
                return 8;
            }
            return 0;
        }

        private KeyDefinition KeyDescriptionForString(string keyString)
        {
            var shift = Modifiers & 8;
            var description = new KeyDefinition
            {
                Key = string.Empty,
                KeyCode = 0,
                Code = string.Empty,
                Text = string.Empty,
                Location = 0
            };

            var definition = KeyDefinitions.Get(keyString);

            if (!string.IsNullOrEmpty(definition.Key))
            {
                description.Key = definition.Key;
            }

            if (shift > 0 && !string.IsNullOrEmpty(definition.ShiftKey))
            {
                description.Key = definition.ShiftKey;
            }

            if (definition.KeyCode > 0)
            {
                description.KeyCode = definition.KeyCode;
            }

            if (shift > 0 && definition.ShiftKeyCode != null)
            {
                description.KeyCode = (int)definition.ShiftKeyCode;
            }

            if (!string.IsNullOrEmpty(definition.Code))
            {
                description.Code = definition.Code;
            }

            if (definition.Location != 0)
            {
                description.Location = definition.Location;
            }

            if (description.Key.Length == 1)
            {
                description.Text = description.Key;
            }

            if (!string.IsNullOrEmpty(definition.Text))
            {
                description.Text = definition.Text;
            }

            if (shift > 0 && !string.IsNullOrEmpty(definition.ShiftText))
            {
                description.Text = definition.ShiftText;
            }

            // if any modifiers besides shift are pressed, no text should be sent
            if ((Modifiers & ~8) > 0)
            {
                description.Text = string.Empty;
            }

            return description;
        }
    }
}
