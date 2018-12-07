using System.Collections.Generic;

namespace PuppeteerSharp.Input
{
    internal static class KeyDefinitions
    {
        private static readonly Dictionary<string, KeyDefinition> Definitions = new Dictionary<string, KeyDefinition>
        {
            ["0"] = new KeyDefinition
            {
                KeyCode = 48,
                Key = "0",
                Code = "Digit0"
            },
            ["1"] = new KeyDefinition
            {
                KeyCode = 49,
                Key = "1",
                Code = "Digit1"
            },
            ["2"] = new KeyDefinition
            {
                KeyCode = 50,
                Key = "2",
                Code = "Digit2"
            },
            ["3"] = new KeyDefinition
            {
                KeyCode = 51,
                Key = "3",
                Code = "Digit3"
            },
            ["4"] = new KeyDefinition
            {
                KeyCode = 52,
                Key = "4",
                Code = "Digit4"
            },
            ["5"] = new KeyDefinition
            {
                KeyCode = 53,
                Key = "5",
                Code = "Digit5"
            },
            ["6"] = new KeyDefinition
            {
                KeyCode = 54,
                Key = "6",
                Code = "Digit6"
            },
            ["7"] = new KeyDefinition
            {
                KeyCode = 55,
                Key = "7",
                Code = "Digit7"
            },
            ["8"] = new KeyDefinition
            {
                KeyCode = 56,
                Key = "8",
                Code = "Digit8"
            },
            ["9"] = new KeyDefinition
            {
                KeyCode = 57,
                Key = "9",
                Code = "Digit9"
            },
            ["Power"] = new KeyDefinition
            {
                Key = "Power",
                Code = "Power"
            },
            ["Eject"] = new KeyDefinition
            {
                Key = "Eject",
                Code = "Eject"
            },
            ["Abort"] = new KeyDefinition
            {
                KeyCode = 3,
                Key = "Cancel",
                Code = "Abort"
            },
            ["Help"] = new KeyDefinition
            {
                KeyCode = 6,
                Key = "Help",
                Code = "Help"
            },
            ["Backspace"] = new KeyDefinition
            {
                KeyCode = 8,
                Key = "Backspace",
                Code = "Backspace"
            },
            ["Tab"] = new KeyDefinition
            {
                KeyCode = 9,
                Key = "Tab",
                Code = "Tab"
            },
            ["Numpad5"] = new KeyDefinition
            {
                KeyCode = 12,
                ShiftKeyCode = 101,
                Key = "Clear",
                ShiftKey = "5",
                Code = "Numpad5",
                Location = 3
            },
            ["NumpadEnter"] = new KeyDefinition
            {
                KeyCode = 13,
                Key = "Enter",
                Code = "NumpadEnter",
                Text = "\r",
                Location = 3
            },
            ["Enter"] = new KeyDefinition
            {
                KeyCode = 13,
                Key = "Enter",
                Code = "Enter",
                Text = "\r"
            },
            ["\r"] = new KeyDefinition
            {
                KeyCode = 13,
                Key = "Enter",
                Code = "Enter",
                Text = "\r"
            },
            ["\n"] = new KeyDefinition
            {
                KeyCode = 13,
                Key = "Enter",
                Code = "Enter",
                Text = "\r"
            },
            ["ShiftLeft"] = new KeyDefinition
            {
                KeyCode = 16,
                Key = "Shift",
                Code = "ShiftLeft",
                Location = 1
            },
            ["ShiftRight"] = new KeyDefinition
            {
                KeyCode = 16,
                Key = "Shift",
                Code = "ShiftRight",
                Location = 2
            },
            ["ControlLeft"] = new KeyDefinition
            {
                KeyCode = 17,
                Key = "Control",
                Code = "ControlLeft",
                Location = 1
            },
            ["ControlRight"] = new KeyDefinition
            {
                KeyCode = 17,
                Key = "Control",
                Code = "ControlRight",
                Location = 2
            },
            ["AltLeft"] = new KeyDefinition
            {
                KeyCode = 18,
                Key = "Alt",
                Code = "AltLeft",
                Location = 1
            },
            ["AltRight"] = new KeyDefinition
            {
                KeyCode = 18,
                Key = "Alt",
                Code = "AltRight",
                Location = 2
            },
            ["Pause"] = new KeyDefinition
            {
                KeyCode = 19,
                Key = "Pause",
                Code = "Pause"
            },
            ["CapsLock"] = new KeyDefinition
            {
                KeyCode = 20,
                Key = "CapsLock",
                Code = "CapsLock"
            },
            ["Escape"] = new KeyDefinition
            {
                KeyCode = 27,
                Key = "Escape",
                Code = "Escape"
            },
            ["Convert"] = new KeyDefinition
            {
                KeyCode = 28,
                Key = "Convert",
                Code = "Convert"
            },
            ["NonConvert"] = new KeyDefinition
            {
                KeyCode = 29,
                Key = "NonConvert",
                Code = "NonConvert"
            },
            ["Space"] = new KeyDefinition
            {
                KeyCode = 32,
                Key = " ",
                Code = "Space"
            },
            ["Numpad9"] = new KeyDefinition
            {
                KeyCode = 33,
                ShiftKeyCode = 105,
                Key = "PageUp",
                ShiftKey = "9",
                Code = "Numpad9",
                Location = 3
            },
            ["PageUp"] = new KeyDefinition
            {
                KeyCode = 33,
                Key = "PageUp",
                Code = "PageUp"
            },
            ["Numpad3"] = new KeyDefinition
            {
                KeyCode = 34,
                ShiftKeyCode = 99,
                Key = "PageDown",
                ShiftKey = "3",
                Code = "Numpad3",
                Location = 3
            },
            ["PageDown"] = new KeyDefinition
            {
                KeyCode = 34,
                Key = "PageDown",
                Code = "PageDown"
            },
            ["End"] = new KeyDefinition
            {
                KeyCode = 35,
                Key = "End",
                Code = "End"
            },
            ["Numpad1"] = new KeyDefinition
            {
                KeyCode = 35,
                ShiftKeyCode = 97,
                Key = "End",
                ShiftKey = "1",
                Code = "Numpad1",
                Location = 3
            },
            ["Home"] = new KeyDefinition
            {
                KeyCode = 36,
                Key = "Home",
                Code = "Home"
            },
            ["Numpad7"] = new KeyDefinition
            {
                KeyCode = 36,
                ShiftKeyCode = 103,
                Key = "Home",
                ShiftKey = "7",
                Code = "Numpad7",
                Location = 3
            },
            ["ArrowLeft"] = new KeyDefinition
            {
                KeyCode = 37,
                Key = "ArrowLeft",
                Code = "ArrowLeft"
            },
            ["Numpad4"] = new KeyDefinition
            {
                KeyCode = 37,
                ShiftKeyCode = 100,
                Key = "ArrowLeft",
                ShiftKey = "4",
                Code = "Numpad4",
                Location = 3
            },
            ["Numpad8"] = new KeyDefinition
            {
                KeyCode = 38,
                ShiftKeyCode = 104,
                Key = "ArrowUp",
                ShiftKey = "8",
                Code = "Numpad8",
                Location = 3
            },
            ["ArrowUp"] = new KeyDefinition
            {
                KeyCode = 38,
                Key = "ArrowUp",
                Code = "ArrowUp"
            },
            ["ArrowRight"] = new KeyDefinition
            {
                KeyCode = 39,
                Key = "ArrowRight",
                Code = "ArrowRight"
            },
            ["Numpad6"] = new KeyDefinition
            {
                KeyCode = 39,
                ShiftKeyCode = 102,
                Key = "ArrowRight",
                ShiftKey = "6",
                Code = "Numpad6",
                Location = 3
            },
            ["Numpad2"] = new KeyDefinition
            {
                KeyCode = 40,
                ShiftKeyCode = 98,
                Key = "ArrowDown",
                ShiftKey = "2",
                Code = "Numpad2",
                Location = 3
            },
            ["ArrowDown"] = new KeyDefinition
            {
                KeyCode = 40,
                Key = "ArrowDown",
                Code = "ArrowDown"
            },
            ["Select"] = new KeyDefinition
            {
                KeyCode = 41,
                Key = "Select",
                Code = "Select"
            },
            ["Open"] = new KeyDefinition
            {
                KeyCode = 43,
                Key = "Execute",
                Code = "Open"
            },
            ["PrintScreen"] = new KeyDefinition
            {
                KeyCode = 44,
                Key = "PrintScreen",
                Code = "PrintScreen"
            },
            ["Insert"] = new KeyDefinition
            {
                KeyCode = 45,
                Key = "Insert",
                Code = "Insert"
            },
            ["Numpad0"] = new KeyDefinition
            {
                KeyCode = 45,
                ShiftKeyCode = 96,
                Key = "Insert",
                ShiftKey = "0",
                Code = "Numpad0",
                Location = 3
            },
            ["Delete"] = new KeyDefinition
            {
                KeyCode = 46,
                Key = "Delete",
                Code = "Delete"
            },
            ["NumpadDecimal"] = new KeyDefinition
            {
                KeyCode = 46,
                ShiftKeyCode = 110,
                Key = "\u0000",
                ShiftKey = ".",
                Code = "NumpadDecimal",
                Location = 3
            },
            ["Digit0"] = new KeyDefinition
            {
                KeyCode = 48,
                Key = "0",
                ShiftKey = ")",
                Code = "Digit0"
            },
            ["Digit1"] = new KeyDefinition
            {
                KeyCode = 49,
                Key = "1",
                ShiftKey = "!",
                Code = "Digit1"
            },
            ["Digit2"] = new KeyDefinition
            {
                KeyCode = 50,
                Key = "2",
                ShiftKey = "@",
                Code = "Digit2"
            },
            ["Digit3"] = new KeyDefinition
            {
                KeyCode = 51,
                Key = "3",
                ShiftKey = "#",
                Code = "Digit3"
            },
            ["Digit4"] = new KeyDefinition
            {
                KeyCode = 52,
                Key = "4",
                ShiftKey = "$",
                Code = "Digit4"
            },
            ["Digit5"] = new KeyDefinition
            {
                KeyCode = 53,
                Key = "5",
                ShiftKey = "%",
                Code = "Digit5"
            },
            ["Digit6"] = new KeyDefinition
            {
                KeyCode = 54,
                Key = "6",
                ShiftKey = "^",
                Code = "Digit6"
            },
            ["Digit7"] = new KeyDefinition
            {
                KeyCode = 55,
                Key = "7",
                ShiftKey = "&",
                Code = "Digit7"
            },
            ["Digit8"] = new KeyDefinition
            {
                KeyCode = 56,
                Key = "8",
                ShiftKey = "*",
                Code = "Digit8"
            },
            ["Digit9"] = new KeyDefinition
            {
                KeyCode = 57,
                Key = "9",
                ShiftKey = "(",
                Code = "Digit9"
            },
            ["KeyA"] = new KeyDefinition
            {
                KeyCode = 65,
                Key = "a",
                ShiftKey = "A",
                Code = "KeyA"
            },
            ["KeyB"] = new KeyDefinition
            {
                KeyCode = 66,
                Key = "b",
                ShiftKey = "B",
                Code = "KeyB"
            },
            ["KeyC"] = new KeyDefinition
            {
                KeyCode = 67,
                Key = "c",
                ShiftKey = "C",
                Code = "KeyC"
            },
            ["KeyD"] = new KeyDefinition
            {
                KeyCode = 68,
                Key = "d",
                ShiftKey = "D",
                Code = "KeyD"
            },
            ["KeyE"] = new KeyDefinition
            {
                KeyCode = 69,
                Key = "e",
                ShiftKey = "E",
                Code = "KeyE"
            },
            ["KeyF"] = new KeyDefinition
            {
                KeyCode = 70,
                Key = "f",
                ShiftKey = "F",
                Code = "KeyF"
            },
            ["KeyG"] = new KeyDefinition
            {
                KeyCode = 71,
                Key = "g",
                ShiftKey = "G",
                Code = "KeyG"
            },
            ["KeyH"] = new KeyDefinition
            {
                KeyCode = 72,
                Key = "h",
                ShiftKey = "H",
                Code = "KeyH"
            },
            ["KeyI"] = new KeyDefinition
            {
                KeyCode = 73,
                Key = "i",
                ShiftKey = "I",
                Code = "KeyI"
            },
            ["KeyJ"] = new KeyDefinition
            {
                KeyCode = 74,
                Key = "j",
                ShiftKey = "J",
                Code = "KeyJ"
            },
            ["KeyK"] = new KeyDefinition
            {
                KeyCode = 75,
                Key = "k",
                ShiftKey = "K",
                Code = "KeyK"
            },
            ["KeyL"] = new KeyDefinition
            {
                KeyCode = 76,
                Key = "l",
                ShiftKey = "L",
                Code = "KeyL"
            },
            ["KeyM"] = new KeyDefinition
            {
                KeyCode = 77,
                Key = "m",
                ShiftKey = "M",
                Code = "KeyM"
            },
            ["KeyN"] = new KeyDefinition
            {
                KeyCode = 78,
                Key = "n",
                ShiftKey = "N",
                Code = "KeyN"
            },
            ["KeyO"] = new KeyDefinition
            {
                KeyCode = 79,
                Key = "o",
                ShiftKey = "O",
                Code = "KeyO"
            },
            ["KeyP"] = new KeyDefinition
            {
                KeyCode = 80,
                Key = "p",
                ShiftKey = "P",
                Code = "KeyP"
            },
            ["KeyQ"] = new KeyDefinition
            {
                KeyCode = 81,
                Key = "q",
                ShiftKey = "Q",
                Code = "KeyQ"
            },
            ["KeyR"] = new KeyDefinition
            {
                KeyCode = 82,
                Key = "r",
                ShiftKey = "R",
                Code = "KeyR"
            },
            ["KeyS"] = new KeyDefinition
            {
                KeyCode = 83,
                Key = "s",
                ShiftKey = "S",
                Code = "KeyS"
            },
            ["KeyT"] = new KeyDefinition
            {
                KeyCode = 84,
                Key = "t",
                ShiftKey = "T",
                Code = "KeyT"
            },
            ["KeyU"] = new KeyDefinition
            {
                KeyCode = 85,
                Key = "u",
                ShiftKey = "U",
                Code = "KeyU"
            },
            ["KeyV"] = new KeyDefinition
            {
                KeyCode = 86,
                Key = "v",
                ShiftKey = "V",
                Code = "KeyV"
            },
            ["KeyW"] = new KeyDefinition
            {
                KeyCode = 87,
                Key = "w",
                ShiftKey = "W",
                Code = "KeyW"
            },
            ["KeyX"] = new KeyDefinition
            {
                KeyCode = 88,
                Key = "x",
                ShiftKey = "X",
                Code = "KeyX"
            },
            ["KeyY"] = new KeyDefinition
            {
                KeyCode = 89,
                Key = "y",
                ShiftKey = "Y",
                Code = "KeyY"
            },
            ["KeyZ"] = new KeyDefinition
            {
                KeyCode = 90,
                Key = "z",
                ShiftKey = "Z",
                Code = "KeyZ"
            },
            ["MetaLeft"] = new KeyDefinition
            {
                KeyCode = 91,
                Key = "Meta",
                Code = "MetaLeft",
                Location = 1
            },
            ["MetaRight"] = new KeyDefinition
            {
                KeyCode = 92,
                Key = "Meta",
                Code = "MetaRight",
                Location = 2
            },
            ["ContextMenu"] = new KeyDefinition
            {
                KeyCode = 93,
                Key = "ContextMenu",
                Code = "ContextMenu"
            },
            ["NumpadMultiply"] = new KeyDefinition
            {
                KeyCode = 106,
                Key = "*",
                Code = "NumpadMultiply",
                Location = 3
            },
            ["NumpadAdd"] = new KeyDefinition
            {
                KeyCode = 107,
                Key = "+",
                Code = "NumpadAdd",
                Location = 3
            },
            ["NumpadSubtract"] = new KeyDefinition
            {
                KeyCode = 109,
                Key = "-",
                Code = "NumpadSubtract",
                Location = 3
            },
            ["NumpadDivide"] = new KeyDefinition
            {
                KeyCode = 111,
                Key = "/",
                Code = "NumpadDivide",
                Location = 3
            },
            ["F1"] = new KeyDefinition
            {
                KeyCode = 112,
                Key = "F1",
                Code = "F1"
            },
            ["F2"] = new KeyDefinition
            {
                KeyCode = 113,
                Key = "F2",
                Code = "F2"
            },
            ["F3"] = new KeyDefinition
            {
                KeyCode = 114,
                Key = "F3",
                Code = "F3"
            },
            ["F4"] = new KeyDefinition
            {
                KeyCode = 115,
                Key = "F4",
                Code = "F4"
            },
            ["F5"] = new KeyDefinition
            {
                KeyCode = 116,
                Key = "F5",
                Code = "F5"
            },
            ["F6"] = new KeyDefinition
            {
                KeyCode = 117,
                Key = "F6",
                Code = "F6"
            },
            ["F7"] = new KeyDefinition
            {
                KeyCode = 118,
                Key = "F7",
                Code = "F7"
            },
            ["F8"] = new KeyDefinition
            {
                KeyCode = 119,
                Key = "F8",
                Code = "F8"
            },
            ["F9"] = new KeyDefinition
            {
                KeyCode = 120,
                Key = "F9",
                Code = "F9"
            },
            ["F10"] = new KeyDefinition
            {
                KeyCode = 121,
                Key = "F10",
                Code = "F10"
            },
            ["F11"] = new KeyDefinition
            {
                KeyCode = 122,
                Key = "F11",
                Code = "F11"
            },
            ["F12"] = new KeyDefinition
            {
                KeyCode = 123,
                Key = "F12",
                Code = "F12"
            },
            ["F13"] = new KeyDefinition
            {
                KeyCode = 124,
                Key = "F13",
                Code = "F13"
            },
            ["F14"] = new KeyDefinition
            {
                KeyCode = 125,
                Key = "F14",
                Code = "F14"
            },
            ["F15"] = new KeyDefinition
            {
                KeyCode = 126,
                Key = "F15",
                Code = "F15"
            },
            ["F16"] = new KeyDefinition
            {
                KeyCode = 127,
                Key = "F16",
                Code = "F16"
            },
            ["F17"] = new KeyDefinition
            {
                KeyCode = 128,
                Key = "F17",
                Code = "F17"
            },
            ["F18"] = new KeyDefinition
            {
                KeyCode = 129,
                Key = "F18",
                Code = "F18"
            },
            ["F19"] = new KeyDefinition
            {
                KeyCode = 130,
                Key = "F19",
                Code = "F19"
            },
            ["F20"] = new KeyDefinition
            {
                KeyCode = 131,
                Key = "F20",
                Code = "F20"
            },
            ["F21"] = new KeyDefinition
            {
                KeyCode = 132,
                Key = "F21",
                Code = "F21"
            },
            ["F22"] = new KeyDefinition
            {
                KeyCode = 133,
                Key = "F22",
                Code = "F22"
            },
            ["F23"] = new KeyDefinition
            {
                KeyCode = 134,
                Key = "F23",
                Code = "F23"
            },
            ["F24"] = new KeyDefinition
            {
                KeyCode = 135,
                Key = "F24",
                Code = "F24"
            },
            ["NumLock"] = new KeyDefinition
            {
                KeyCode = 144,
                Key = "NumLock",
                Code = "NumLock"
            },
            ["ScrollLock"] = new KeyDefinition
            {
                KeyCode = 145,
                Key = "ScrollLock",
                Code = "ScrollLock"
            },
            ["AudioVolumeMute"] = new KeyDefinition
            {
                KeyCode = 173,
                Key = "AudioVolumeMute",
                Code = "AudioVolumeMute"
            },
            ["AudioVolumeDown"] = new KeyDefinition
            {
                KeyCode = 174,
                Key = "AudioVolumeDown",
                Code = "AudioVolumeDown"
            },
            ["AudioVolumeUp"] = new KeyDefinition
            {
                KeyCode = 175,
                Key = "AudioVolumeUp",
                Code = "AudioVolumeUp"
            },
            ["MediaTrackNext"] = new KeyDefinition
            {
                KeyCode = 176,
                Key = "MediaTrackNext",
                Code = "MediaTrackNext"
            },
            ["MediaTrackPrevious"] = new KeyDefinition
            {
                KeyCode = 177,
                Key = "MediaTrackPrevious",
                Code = "MediaTrackPrevious"
            },
            ["MediaStop"] = new KeyDefinition
            {
                KeyCode = 178,
                Key = "MediaStop",
                Code = "MediaStop"
            },
            ["MediaPlayPause"] = new KeyDefinition
            {
                KeyCode = 179,
                Key = "MediaPlayPause",
                Code = "MediaPlayPause"
            },
            ["Semicolon"] = new KeyDefinition
            {
                KeyCode = 186,
                Key = ";",
                ShiftKey = ":",
                Code = "Semicolon"
            },
            ["Equal"] = new KeyDefinition
            {
                KeyCode = 187,
                Key = "=",
                ShiftKey = "+",
                Code = "Equal"
            },
            ["NumpadEqual"] = new KeyDefinition
            {
                KeyCode = 187,
                Key = "=",
                Code = "NumpadEqual",
                Location = 3
            },
            ["Comma"] = new KeyDefinition
            {
                KeyCode = 188,
                Key = ",",
                ShiftKey = "<",
                Code = "Comma"
            },
            ["Minus"] = new KeyDefinition
            {
                KeyCode = 189,
                Key = "-",
                ShiftKey = "_",
                Code = "Minus"
            },
            ["Period"] = new KeyDefinition
            {
                KeyCode = 190,
                Key = ".",
                ShiftKey = ">",
                Code = "Period"
            },
            ["Slash"] = new KeyDefinition
            {
                KeyCode = 191,
                Key = "/",
                ShiftKey = "?",
                Code = "Slash"
            },
            ["Backquote"] = new KeyDefinition
            {
                KeyCode = 192,
                Key = "`",
                ShiftKey = "~",
                Code = "Backquote"
            },
            ["BracketLeft"] = new KeyDefinition
            {
                KeyCode = 219,
                Key = "[",
                ShiftKey = "{",
                Code = "BracketLeft"
            },
            ["Backslash"] = new KeyDefinition
            {
                KeyCode = 220,
                Key = "\\",
                ShiftKey = "|",
                Code = "Backslash"
            },
            ["BracketRight"] = new KeyDefinition
            {
                KeyCode = 221,
                Key = "]",
                ShiftKey = "}",
                Code = "BracketRight"
            },
            ["Quote"] = new KeyDefinition
            {
                KeyCode = 222,
                Key = "'",
                ShiftKey = "\"",
                Code = "Quote"
            },
            ["AltGraph"] = new KeyDefinition
            {
                KeyCode = 225,
                Key = "AltGraph",
                Code = "AltGraph"
            },
            ["Props"] = new KeyDefinition
            {
                KeyCode = 247,
                Key = "CrSel",
                Code = "Props"
            },
            ["Cancel"] = new KeyDefinition
            {
                KeyCode = 3,
                Key = "Cancel",
                Code = "Abort"
            },
            ["Clear"] = new KeyDefinition
            {
                KeyCode = 12,
                Key = "Clear",
                Code = "Numpad5",
                Location = 3
            },
            ["Shift"] = new KeyDefinition
            {
                KeyCode = 16,
                Key = "Shift",
                Code = "ShiftLeft",
                Location = 1
            },
            ["Control"] = new KeyDefinition
            {
                KeyCode = 17,
                Key = "Control",
                Code = "ControlLeft",
                Location = 1
            },
            ["Alt"] = new KeyDefinition
            {
                KeyCode = 18,
                Key = "Alt",
                Code = "AltLeft",
                Location = 1
            },
            ["Accept"] = new KeyDefinition
            {
                KeyCode = 30,
                Key = "Accept"
            },
            ["ModeChange"] = new KeyDefinition
            {
                KeyCode = 31,
                Key = "ModeChange"
            },
            [" "] = new KeyDefinition
            {
                KeyCode = 32,
                Key = " ",
                Code = "Space"
            },
            ["Print"] = new KeyDefinition
            {
                KeyCode = 42,
                Key = "Print"
            },
            ["Execute"] = new KeyDefinition
            {
                KeyCode = 43,
                Key = "Execute",
                Code = "Open"
            },
            ["\u0000"] = new KeyDefinition
            {
                KeyCode = 46,
                Key = "\u0000",
                Code = "NumpadDecimal",
                Location = 3
            },
            ["a"] = new KeyDefinition
            {
                KeyCode = 65,
                Key = "a",
                Code = "KeyA"
            },
            ["b"] = new KeyDefinition
            {
                KeyCode = 66,
                Key = "b",
                Code = "KeyB"
            },
            ["c"] = new KeyDefinition
            {
                KeyCode = 67,
                Key = "c",
                Code = "KeyC"
            },
            ["d"] = new KeyDefinition
            {
                KeyCode = 68,
                Key = "d",
                Code = "KeyD"
            },
            ["e"] = new KeyDefinition
            {
                KeyCode = 69,
                Key = "e",
                Code = "KeyE"
            },
            ["f"] = new KeyDefinition
            {
                KeyCode = 70,
                Key = "f",
                Code = "KeyF"
            },
            ["g"] = new KeyDefinition
            {
                KeyCode = 71,
                Key = "g",
                Code = "KeyG"
            },
            ["h"] = new KeyDefinition
            {
                KeyCode = 72,
                Key = "h",
                Code = "KeyH"
            },
            ["i"] = new KeyDefinition
            {
                KeyCode = 73,
                Key = "i",
                Code = "KeyI"
            },
            ["j"] = new KeyDefinition
            {
                KeyCode = 74,
                Key = "j",
                Code = "KeyJ"
            },
            ["k"] = new KeyDefinition
            {
                KeyCode = 75,
                Key = "k",
                Code = "KeyK"
            },
            ["l"] = new KeyDefinition
            {
                KeyCode = 76,
                Key = "l",
                Code = "KeyL"
            },
            ["m"] = new KeyDefinition
            {
                KeyCode = 77,
                Key = "m",
                Code = "KeyM"
            },
            ["n"] = new KeyDefinition
            {
                KeyCode = 78,
                Key = "n",
                Code = "KeyN"
            },
            ["o"] = new KeyDefinition
            {
                KeyCode = 79,
                Key = "o",
                Code = "KeyO"
            },
            ["p"] = new KeyDefinition
            {
                KeyCode = 80,
                Key = "p",
                Code = "KeyP"
            },
            ["q"] = new KeyDefinition
            {
                KeyCode = 81,
                Key = "q",
                Code = "KeyQ"
            },
            ["r"] = new KeyDefinition
            {
                KeyCode = 82,
                Key = "r",
                Code = "KeyR"
            },
            ["s"] = new KeyDefinition
            {
                KeyCode = 83,
                Key = "s",
                Code = "KeyS"
            },
            ["t"] = new KeyDefinition
            {
                KeyCode = 84,
                Key = "t",
                Code = "KeyT"
            },
            ["u"] = new KeyDefinition
            {
                KeyCode = 85,
                Key = "u",
                Code = "KeyU"
            },
            ["v"] = new KeyDefinition
            {
                KeyCode = 86,
                Key = "v",
                Code = "KeyV"
            },
            ["w"] = new KeyDefinition
            {
                KeyCode = 87,
                Key = "w",
                Code = "KeyW"
            },
            ["x"] = new KeyDefinition
            {
                KeyCode = 88,
                Key = "x",
                Code = "KeyX"
            },
            ["y"] = new KeyDefinition
            {
                KeyCode = 89,
                Key = "y",
                Code = "KeyY"
            },
            ["z"] = new KeyDefinition
            {
                KeyCode = 90,
                Key = "z",
                Code = "KeyZ"
            },
            ["Meta"] = new KeyDefinition
            {
                KeyCode = 91,
                Key = "Meta",
                Code = "MetaLeft",
                Location = 1
            },
            ["*"] = new KeyDefinition
            {
                KeyCode = 106,
                Key = "*",
                Code = "NumpadMultiply",
                Location = 3
            },
            ["+"] = new KeyDefinition
            {
                KeyCode = 107,
                Key = "+",
                Code = "NumpadAdd",
                Location = 3
            },
            ["-"] = new KeyDefinition
            {
                KeyCode = 109,
                Key = "-",
                Code = "NumpadSubtract",
                Location = 3
            },
            ["/"] = new KeyDefinition
            {
                KeyCode = 111,
                Key = "/",
                Code = "NumpadDivide",
                Location = 3
            },
            [";"] = new KeyDefinition
            {
                KeyCode = 186,
                Key = ";",
                Code = "Semicolon"
            },
            ["="] = new KeyDefinition
            {
                KeyCode = 187,
                Key = "=",
                Code = "Equal"
            },
            [","] = new KeyDefinition
            {
                KeyCode = 188,
                Key = ",",
                Code = "Comma"
            },
            ["."] = new KeyDefinition
            {
                KeyCode = 190,
                Key = ".",
                Code = "Period"
            },
            ["`"] = new KeyDefinition
            {
                KeyCode = 192,
                Key = "`",
                Code = "Backquote"
            },
            ["["] = new KeyDefinition
            {
                KeyCode = 219,
                Key = "[",
                Code = "BracketLeft"
            },
            ["\\"] = new KeyDefinition
            {
                KeyCode = 220,
                Key = "\\",
                Code = "Backslash"
            },
            ["]"] = new KeyDefinition
            {
                KeyCode = 221,
                Key = "]",
                Code = "BracketRight"
            },
            ["'"] = new KeyDefinition
            {
                KeyCode = 222,
                Key = "'",
                Code = "Quote"
            },
            ["Attn"] = new KeyDefinition
            {
                KeyCode = 246,
                Key = "Attn"
            },
            ["CrSel"] = new KeyDefinition
            {
                KeyCode = 247,
                Key = "CrSel",
                Code = "Props"
            },
            ["ExSel"] = new KeyDefinition
            {
                KeyCode = 248,
                Key = "ExSel"
            },
            ["EraseEof"] = new KeyDefinition
            {
                KeyCode = 249,
                Key = "EraseEof"
            },
            ["Play"] = new KeyDefinition
            {
                KeyCode = 250,
                Key = "Play"
            },
            ["ZoomOut"] = new KeyDefinition
            {
                KeyCode = 251,
                Key = "ZoomOut"
            },
            [")"] = new KeyDefinition
            {
                KeyCode = 48,
                Key = ")",
                Code = "Digit0"
            },
            ["!"] = new KeyDefinition
            {
                KeyCode = 49,
                Key = "!",
                Code = "Digit1"
            },
            ["@"] = new KeyDefinition
            {
                KeyCode = 50,
                Key = "@",
                Code = "Digit2"
            },
            ["#"] = new KeyDefinition
            {
                KeyCode = 51,
                Key = "#",
                Code = "Digit3"
            },
            ["$"] = new KeyDefinition
            {
                KeyCode = 52,
                Key = "$",
                Code = "Digit4"
            },
            ["%"] = new KeyDefinition
            {
                KeyCode = 53,
                Key = "%",
                Code = "Digit5"
            },
            ["^"] = new KeyDefinition
            {
                KeyCode = 54,
                Key = "^",
                Code = "Digit6"
            },
            ["&"] = new KeyDefinition
            {
                KeyCode = 55,
                Key = "&",
                Code = "Digit7"
            },
            ["("] = new KeyDefinition
            {
                KeyCode = 57,
                Key = "(",
                Code = "Digit9"
            },
            ["A"] = new KeyDefinition
            {
                KeyCode = 65,
                Key = "A",
                Code = "KeyA"
            },
            ["B"] = new KeyDefinition
            {
                KeyCode = 66,
                Key = "B",
                Code = "KeyB"
            },
            ["C"] = new KeyDefinition
            {
                KeyCode = 67,
                Key = "C",
                Code = "KeyC"
            },
            ["D"] = new KeyDefinition
            {
                KeyCode = 68,
                Key = "D",
                Code = "KeyD"
            },
            ["E"] = new KeyDefinition
            {
                KeyCode = 69,
                Key = "E",
                Code = "KeyE"
            },
            ["F"] = new KeyDefinition
            {
                KeyCode = 70,
                Key = "F",
                Code = "KeyF"
            },
            ["G"] = new KeyDefinition
            {
                KeyCode = 71,
                Key = "G",
                Code = "KeyG"
            },
            ["H"] = new KeyDefinition
            {
                KeyCode = 72,
                Key = "H",
                Code = "KeyH"
            },
            ["I"] = new KeyDefinition
            {
                KeyCode = 73,
                Key = "I",
                Code = "KeyI"
            },
            ["J"] = new KeyDefinition
            {
                KeyCode = 74,
                Key = "J",
                Code = "KeyJ"
            },
            ["K"] = new KeyDefinition
            {
                KeyCode = 75,
                Key = "K",
                Code = "KeyK"
            },
            ["L"] = new KeyDefinition
            {
                KeyCode = 76,
                Key = "L",
                Code = "KeyL"
            },
            ["M"] = new KeyDefinition
            {
                KeyCode = 77,
                Key = "M",
                Code = "KeyM"
            },
            ["N"] = new KeyDefinition
            {
                KeyCode = 78,
                Key = "N",
                Code = "KeyN"
            },
            ["O"] = new KeyDefinition
            {
                KeyCode = 79,
                Key = "O",
                Code = "KeyO"
            },
            ["P"] = new KeyDefinition
            {
                KeyCode = 80,
                Key = "P",
                Code = "KeyP"
            },
            ["Q"] = new KeyDefinition
            {
                KeyCode = 81,
                Key = "Q",
                Code = "KeyQ"
            },
            ["R"] = new KeyDefinition
            {
                KeyCode = 82,
                Key = "R",
                Code = "KeyR"
            },
            ["S"] = new KeyDefinition
            {
                KeyCode = 83,
                Key = "S",
                Code = "KeyS"
            },
            ["T"] = new KeyDefinition
            {
                KeyCode = 84,
                Key = "T",
                Code = "KeyT"
            },
            ["U"] = new KeyDefinition
            {
                KeyCode = 85,
                Key = "U",
                Code = "KeyU"
            },
            ["V"] = new KeyDefinition
            {
                KeyCode = 86,
                Key = "V",
                Code = "KeyV"
            },
            ["W"] = new KeyDefinition
            {
                KeyCode = 87,
                Key = "W",
                Code = "KeyW"
            },
            ["X"] = new KeyDefinition
            {
                KeyCode = 88,
                Key = "X",
                Code = "KeyX"
            },
            ["Y"] = new KeyDefinition
            {
                KeyCode = 89,
                Key = "Y",
                Code = "KeyY"
            },
            ["Z"] = new KeyDefinition
            {
                KeyCode = 90,
                Key = "Z",
                Code = "KeyZ"
            },
            [":"] = new KeyDefinition
            {
                KeyCode = 186,
                Key = ":",
                Code = "Semicolon"
            },
            ["<"] = new KeyDefinition
            {
                KeyCode = 188,
                Key = "<",
                Code = "Comma"
            },
            ["_"] = new KeyDefinition
            {
                KeyCode = 189,
                Key = "_",
                Code = "Minus"
            },
            [">"] = new KeyDefinition
            {
                KeyCode = 190,
                Key = ">",
                Code = "Period"
            },
            ["?"] = new KeyDefinition
            {
                KeyCode = 191,
                Key = "?",
                Code = "Slash"
            },
            ["~"] = new KeyDefinition
            {
                KeyCode = 192,
                Key = "~",
                Code = "Backquote"
            },
            ["{"] = new KeyDefinition
            {
                KeyCode = 219,
                Key = "{",
                Code = "BracketLeft"
            },
            ["|"] = new KeyDefinition
            {
                KeyCode = 220,
                Key = "|",
                Code = "Backslash"
            },
            ["}"] = new KeyDefinition
            {
                KeyCode = 221,
                Key = "}",
                Code = "BracketRight"
            },
            ["\""] = new KeyDefinition
            {
                KeyCode = 222,
                Key = "\"",
                Code = "Quote"
            },
        };

        internal static KeyDefinition Get(string key) => Definitions[key];
        
        internal static bool ContainsKey(string key) => Definitions.ContainsKey(key);
    }
}