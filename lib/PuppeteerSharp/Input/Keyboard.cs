using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp.Input
{
    public class Keyboard
    {
        private readonly Session _client;
        private readonly HashSet<string> _pressedKeys = new HashSet<string>();

        public int Modifiers { get; set; }

        public Keyboard(Session client)
        {
            _client = client;
        }

        public async Task Down(string key, DownOptions options)
        {
            var description = KeyDescriptionForString(key);

            var autoRepeat = _pressedKeys.Contains(description.Code);
            _pressedKeys.Add(key);
            Modifiers |= ModifierBit(key);

            var text = options.Text == null ? description.Text : options.Text;

            await _client.SendAsync("Input.dispatchKeyEvent", new Dictionary<string, object>(){
                {"type", text != null ? "keyDown" : "rawKeyDown"},
                {"modifiers", Modifiers},
                {"windowsvirtualKeyCode", description.KeyCode},
                {"code", description.Code },
                {"key", description.Key},
                {"text", text},
                {"unmodifiedText", text},
                {"autoRepeat", autoRepeat},
                {"location", description.Location },
                {"isKeypad", description.Location == 3 }
            });
        }

        public async Task Up(string key)
        {
            var description = KeyDescriptionForString(key);

            Modifiers &= ModifierBit(key);
            _pressedKeys.Remove(description.Key);

            await _client.SendAsync("Input.dispatchKeyEvent", new Dictionary<string, object>(){
                {"type", "keyUp"},
                {"modifiers", Modifiers},
                {"key", key},
                {"windowsvirtualKeyCode", description.KeyCode},
                {"code", description.Code },
                {"location", description.Location }
            });
        }

        public async Task SendCharacter(string charText)
        {
            await _client.SendAsync("Input.dispatchKeyEvent", new Dictionary<string, object>(){
                {"type", "char"},
                {"modifiers", Modifiers},
                {"key", charText},
                {"key", charText}
            });
        }

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
                    await SendCharacter(letter.ToString());
                }
                if(delay > 0)
                {
                    await Task.Delay(delay);
                }
            }
        }

        public async Task PressAsync(string key, PressOptions options = null)
        {
            await Down(key, options);

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
                description.Key = definition.Key;
            if (shift > 0 && !string.IsNullOrEmpty(definition.ShiftKey))
                description.Key = definition.ShiftKey;

            if (definition.KeyCode > 0)
                description.KeyCode = definition.KeyCode;
            if (shift > 0 && definition.ShiftKeyCode != null)
                description.KeyCode = (int)definition.ShiftKeyCode;

            if (!string.IsNullOrEmpty(definition.Code))
                description.Code = definition.Code;

            if (definition.Location != 0)
                description.Location = definition.Location;

            if (description.Key.Length == 1)
                description.Text = description.Key;

            if (!string.IsNullOrEmpty(definition.Text))
                description.Text = definition.Text;
            if (shift > 0 && !string.IsNullOrEmpty(definition.ShiftText))
                description.Text = definition.ShiftText;

            // if any modifiers besides shift are pressed, no text should be sent
            if ((Modifiers & ~8) > 0)
                description.Text = string.Empty;

            return description;
        }
    }
}