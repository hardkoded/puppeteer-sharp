using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp.Input
{
    public class Keyboard
    {
        private Session _client;

        private List<string> _pressedKeys = new List<string>();
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
            {",", 188},
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

        public async Task Down(string key, Dictionary<string, object> options)
        {
            var text = options["text"].ToString();
            bool autoRepeat = _pressedKeys.Contains(key);

            if (!autoRepeat)
            {
                _pressedKeys.Add(key);
            }

            Modifiers |= modifierBit(key);
            await _client.SendAsync("Input.dispatchKeyEvent", new Dictionary<string, object>(){
                {"type", text.Length > 0 ? "keyDown" : "rawKeyDown"},
                {"modifiers", Modifiers},
                {"windowsvirtualKeyCode", CodeForKey(key)},
                {"key", key},
                {"text", text},
                {"unmodifiedText", text},
                {"autoRepeat", autoRepeat}
            });
        }

        public async Task Up(string key)
        {
            Modifiers &= modifierBit(key);

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

        private int CodeForKey(string key)
        {
            if(_keys.ContainsKey(key))
            {
                return _keys[key];
            }
            if(key.Length == 1)
            {
                return key.ToUpper().ToCharArray()[0];
            }
            return 0;
        }

        private int modifierBit(string key)
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
    }
}