using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp.Input
{
    /// <inheritdoc/>
    public class Keyboard : IKeyboard
    {
        private readonly CDPSession _client;
        private readonly HashSet<string> _pressedKeys = new HashSet<string>();

        internal Keyboard(CDPSession client)
        {
            _client = client;
        }

        internal int Modifiers { get; set; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public Task SendCharacterAsync(string charText)
            => _client.SendAsync("Input.insertText", new InputInsertTextRequest
            {
                Text = charText
            });

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
