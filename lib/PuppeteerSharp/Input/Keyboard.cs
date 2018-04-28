using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp.Input
{
    public class Keyboard
    {
        private Session _client;

        private HashSet<string> _pressedKeys = new HashSet<string>();
        private readonly Dictionary<string, int> _keys = new Dictionary<string, int>(){
            {"Cancel", 3},
            {"Help", 6},
            {"Backspace", 8},
            {"Tab", 9},
            {"Clear", 12},
            {"Enter", 13},
            {"Shift", 16},
            {"Control", 17},
            {"Alt", 18},
            {"Pause", 19},
            {"CapsLock", 20},
            {"Escape", 27},
            {"Convert", 28},
            {"NonConvert", 29},
            {"Accept", 30},
            {"ModeChange", 31},
            {"PageUp", 33},
            {"PageDown", 34},
            {"End", 35},
            {"Home", 36},
            {"ArrowLeft", 37},
            {"ArrowUp", 38},
            {"ArrowRight", 39},
            {"ArrowDown", 40},
            {"Select", 41},
            {"Print", 42},
            {"Execute", 43},
            {"PrintScreen", 44},
            {"Insert", 45},
            {"Delete", 46},
            {")", 48},
            {"!", 49},
            {"@", 50},
            {"#", 51},
            {"$", 52},
            {"%", 53},
            {"^", 54},
            {"&", 55},
            {"*", 56},
            {"(", 57},
            {"Meta", 91},
            {"ContextMenu", 93},
            {"F1", 112},
            {"F2", 113},
            {"F3", 114},
            {"F4", 115},
            {"F5", 116},
            {"F6", 117},
            {"F7", 118},
            {"F8", 119},
            {"F9", 120},
            {"F10", 121},
            {"F11", 122},
            {"F12", 123},
            {"F13", 124},
            {"F14", 125},
            {"F15", 126},
            {"F16", 127},
            {"F17", 128},
            {"F18", 129},
            {"F19", 130},
            {"F20", 131},
            {"F21", 132},
            {"F22", 133},
            {"F23", 134},
            {"F24", 135},
            {"NumLock", 144},
            {"ScrollLock", 145},
            {"AudioVolumeMute", 173},
            {"AudioVolumeDown", 174},
            {"AudioVolumeUp", 175},
            {"MediaTrackNext", 176},
            {"MediaTrackPrevious", 177},
            {"MediaStop", 178},
            {"MediaPlayPause", 179},
            {";", 186},
            {",", 186},
            {"=", 187},
            {"+", 187},
            {"<", 188},
            {"-", 189},
            {"_", 189},
            {".", 190},
            {">", 190},
            {"/", 191},
            {"?", 191},
            {"`", 192},
            {"~", 192},
            {"[", 219},
            {"{", 219},
            {"\\", 220},
            {"|", 220},
            {"]", 221},
            {"}", 221},
            {"\"", 222},
            {"AltGraph", 225},
            {"Attn", 246},
            {"CrSel", 247},
            {"ExSel", 248},
            {"EraseEof", 249},
            {"Play", 250},
            {"ZoomOut", 251}
        };

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
            Modifiers &= ModifierBit(key);

            if (_pressedKeys.Contains(key))
            {
                _pressedKeys.Remove(key);
            }

            await _client.SendAsync("Input.dispatchKeyEvent", new Dictionary<string, object>(){
                {"type", "keyUp"},
                {"modifiers", Modifiers},
                {"windowsvirtualKeyCode", CodeForKey(key)},
                {"key", key}
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

        public async Task SendAsync(string text, TypeOptions options = null)
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
            }

        }

        public async Task PressAsync(string key, PressOptions options = null)
        {
            await Down(key, options);

        }

        private int CodeForKey(string key)
        {
            if (_keys.ContainsKey(key))
            {
                return _keys[key];
            }
            if (key.Length == 1)
            {
                return key.ToUpper().ToCharArray()[0];
            }
            return 0;
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

            if (definition.Location != null)
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

    public class PressOptions : DownOptions
    {
        public int? Delay { get; set; }
    }

    public class DownOptions
    {
        public string Text { get; set; }
    }
}