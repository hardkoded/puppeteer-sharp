// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.

#if !CDP_ONLY

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using PuppeteerSharp.Input;
using WebDriverBiDi.Input;

namespace PuppeteerSharp.Bidi;

internal class BidiKeyboard(BidiPage page) : Keyboard
{
    // Reuse the same KeySourceActions instance to maintain a consistent ID for key repeat tracking.
    // WebDriver BiDi tracks key state per source ID, so using the same ID allows proper repeat detection.
    private readonly KeySourceActions _keySource = new();

    /// <inheritdoc/>
    public override async Task DownAsync(string key, DownOptions options = null)
    {
        _keySource.Actions.Clear();
        _keySource.Actions.Add(new KeyDownAction(GetBidiKeyValue(key, validateKey: true)));
        await page.BidiMainFrame.BrowsingContext.PerformActionsAsync([_keySource]).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task UpAsync(string key)
    {
        _keySource.Actions.Clear();
        _keySource.Actions.Add(new KeyUpAction(GetBidiKeyValue(key, validateKey: true)));
        await page.BidiMainFrame.BrowsingContext.PerformActionsAsync([_keySource]).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task PressAsync(string key, PressOptions options = null)
    {
        var delay = options?.Delay ?? 0;
        var keyValue = GetBidiKeyValue(key, validateKey: true);

        _keySource.Actions.Clear();
        _keySource.Actions.Add(new KeyDownAction(keyValue));

        if (delay > 0)
        {
            _keySource.Actions.Add(new PauseAction
            {
                Duration = TimeSpan.FromMilliseconds(delay),
            });
        }

        _keySource.Actions.Add(new KeyUpAction(keyValue));

        await page.BidiMainFrame.BrowsingContext.PerformActionsAsync([_keySource]).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task TypeAsync(string text, TypeOptions options = null)
    {
        var delay = options?.Delay ?? 0;
        _keySource.Actions.Clear();

        // Iterate over the string using StringInfo to handle code points rather than UTF-16 code units
        var textParts = StringInfo.GetTextElementEnumerator(text);
        if (delay <= 0)
        {
            while (textParts.MoveNext())
            {
                var letter = textParts.Current.ToString();
                var keyValue = GetBidiKeyValue(letter);

                _keySource.Actions.Add(new KeyDownAction(keyValue));
                _keySource.Actions.Add(new KeyUpAction(keyValue));
            }
        }
        else
        {
            while (textParts.MoveNext())
            {
                var letter = textParts.Current.ToString();
                var keyValue = GetBidiKeyValue(letter);

                _keySource.Actions.Add(new KeyDownAction(keyValue));
                _keySource.Actions.Add(new PauseAction
                {
                    Duration = TimeSpan.FromMilliseconds(delay),
                });
                _keySource.Actions.Add(new KeyUpAction(keyValue));
            }
        }

        await page.BidiMainFrame.BrowsingContext.PerformActionsAsync([_keySource]).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task SendCharacterAsync(string charText)
    {
        // Measures the number of code points rather than UTF-16 code units.
        var textElements = StringInfo.GetTextElementEnumerator(charText);
        var count = 0;
        while (textElements.MoveNext())
        {
            count++;
        }

        if (count > 1)
        {
            throw new PuppeteerException("Cannot send more than 1 character.");
        }

        // Use the main frame for now; in future we may need to find the focused frame
        await page.BidiMainFrame.EvaluateFunctionAsync(
            "async (char) => { document.execCommand('insertText', false, char); }",
            charText).ConfigureAwait(false);
    }

    private static string GetBidiKeyValue(string key, bool validateKey = false)
    {
        switch (key)
        {
            case "\r":
            case "\n":
                key = "Enter";
                break;
        }

        // Measures the number of code points rather than UTF-16 code units.
        var textElements = StringInfo.GetTextElementEnumerator(key);
        var count = 0;
        while (textElements.MoveNext())
        {
            count++;
        }

        if (count == 1)
        {
            // When validateKey is true (e.g., for PressAsync), validate against known keys
            if (validateKey && !KeyDefinitions.ContainsKey(key))
            {
                throw new KeyNotFoundException($"Unknown key: \"{key}\"");
            }

            return key;
        }

        return key switch
        {
            "Cancel" => "\uE001",
            "Help" => "\uE002",
            "Backspace" => "\uE003",
            "Tab" => "\uE004",
            "Clear" => "\uE005",
            "Enter" => "\uE007",
            "Shift" or "ShiftLeft" => "\uE008",
            "Control" or "ControlLeft" => "\uE009",
            "Alt" or "AltLeft" => "\uE00A",
            "Pause" => "\uE00B",
            "Escape" => "\uE00C",
            "PageUp" => "\uE00E",
            "PageDown" => "\uE00F",
            "End" => "\uE010",
            "Home" => "\uE011",
            "ArrowLeft" => "\uE012",
            "ArrowUp" => "\uE013",
            "ArrowRight" => "\uE014",
            "ArrowDown" => "\uE015",
            "Insert" => "\uE016",
            "Delete" => "\uE017",
            "NumpadEqual" => "\uE019",
            "Numpad0" => "\uE01A",
            "Numpad1" => "\uE01B",
            "Numpad2" => "\uE01C",
            "Numpad3" => "\uE01D",
            "Numpad4" => "\uE01E",
            "Numpad5" => "\uE01F",
            "Numpad6" => "\uE020",
            "Numpad7" => "\uE021",
            "Numpad8" => "\uE022",
            "Numpad9" => "\uE023",
            "NumpadMultiply" => "\uE024",
            "NumpadAdd" => "\uE025",
            "NumpadSubtract" => "\uE027",
            "NumpadDecimal" => "\uE028",
            "NumpadDivide" => "\uE029",
            "F1" => "\uE031",
            "F2" => "\uE032",
            "F3" => "\uE033",
            "F4" => "\uE034",
            "F5" => "\uE035",
            "F6" => "\uE036",
            "F7" => "\uE037",
            "F8" => "\uE038",
            "F9" => "\uE039",
            "F10" => "\uE03A",
            "F11" => "\uE03B",
            "F12" => "\uE03C",
            "Meta" or "MetaLeft" => "\uE03D",
            "ShiftRight" => "\uE050",
            "ControlRight" => "\uE051",
            "AltRight" => "\uE052",
            "MetaRight" => "\uE053",
            "Digit0" => "0",
            "Digit1" => "1",
            "Digit2" => "2",
            "Digit3" => "3",
            "Digit4" => "4",
            "Digit5" => "5",
            "Digit6" => "6",
            "Digit7" => "7",
            "Digit8" => "8",
            "Digit9" => "9",
            "KeyA" => "a",
            "KeyB" => "b",
            "KeyC" => "c",
            "KeyD" => "d",
            "KeyE" => "e",
            "KeyF" => "f",
            "KeyG" => "g",
            "KeyH" => "h",
            "KeyI" => "i",
            "KeyJ" => "j",
            "KeyK" => "k",
            "KeyL" => "l",
            "KeyM" => "m",
            "KeyN" => "n",
            "KeyO" => "o",
            "KeyP" => "p",
            "KeyQ" => "q",
            "KeyR" => "r",
            "KeyS" => "s",
            "KeyT" => "t",
            "KeyU" => "u",
            "KeyV" => "v",
            "KeyW" => "w",
            "KeyX" => "x",
            "KeyY" => "y",
            "KeyZ" => "z",
            "Semicolon" => ";",
            "Equal" => "=",
            "Comma" => ",",
            "Minus" => "-",
            "Period" => ".",
            "Slash" => "/",
            "Backquote" => "`",
            "BracketLeft" => "[",
            "Backslash" => "\\",
            "BracketRight" => "]",
            "Quote" => "\"",
            _ => throw new KeyNotFoundException($"Unknown key: \"{key}\""),
        };
    }
}

#endif
