using System.Collections.Generic;
using System.Threading.Tasks;

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

        internal int Modifiers { get; set; }

        internal Keyboard(CDPSession client)
        {
            _client = client;
        }

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
        public async Task DownAsync(string key, DownOptions options = null)
        {
            var description = KeyDescriptionForString(key);

            var autoRepeat = _pressedKeys.Contains(description.Code);
            _pressedKeys.Add(description.Code);
            Modifiers |= ModifierBit(key);

            var text = options?.Text == null ? description.Text : options.Text;

            await _client.SendAsync("Input.dispatchKeyEvent", new Dictionary<string, object>(){
                {"type", text != null ? "keyDown" : "rawKeyDown"},
                {"modifiers", Modifiers},
                {"windowsVirtualKeyCode", description.KeyCode},
                {"code", description.Code },
                {"key", description.Key},
                {"text", text},
                {"unmodifiedText", text},
                {"autoRepeat", autoRepeat},
                {"location", description.Location },
                {"isKeypad", description.Location == 3 }
            });
        }

        /// <summary>
        /// Dispatches a <c>keyup</c> event.
        /// </summary>
        /// <param name="key">Name of key to release, such as `ArrowLeft`. See <see cref="KeyDefinitions"/> for a list of all key names.</param>
        /// <returns>Task</returns>
        public async Task UpAsync(string key)
        {
            var description = KeyDescriptionForString(key);

            Modifiers &= ~ModifierBit(key);
            _pressedKeys.Remove(description.Key);

            await _client.SendAsync("Input.dispatchKeyEvent", new Dictionary<string, object>(){
                {"type", "keyUp"},
                {"modifiers", Modifiers},
                {"key", description.Key},
                {"windowsVirtualKeyCode", description.KeyCode},
                {"code", description.Code },
                {"location", description.Location }
            });
        }

        /// <summary>
        /// Dispatches a <c>keypress</c> and <c>input</c> event. This does not send a <c>keydown</c> or <c>keyup</c> event.
        /// </summary>
        /// <param name="charText">Character to send into the page</param>
        /// <returns>Task</returns>
        public async Task SendCharacterAsync(string charText)
        {
            await _client.SendAsync("Input.dispatchKeyEvent", new Dictionary<string, object>(){
                {"type", "char"},
                {"modifiers", Modifiers},
                {"text", charText },
                {"key", charText},
                {"unmodifiedText", charText }
            });
        }

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
            foreach (var letter in text)
            {
                if (KeyDefinitions.ContainsKey(letter.ToString()))
                {
                    await PressAsync(letter.ToString(), new PressOptions { Delay = delay });
                }
                else
                {
                    await SendCharacterAsync(letter.ToString());
                }
                if (delay > 0)
                {
                    await Task.Delay(delay);
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
            await DownAsync(key, options);
            if (options?.Delay > 0)
            {
                await Task.Delay((int)options.Delay);
            }
            await UpAsync(key);
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