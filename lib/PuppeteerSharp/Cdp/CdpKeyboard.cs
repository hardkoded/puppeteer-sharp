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

using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Input;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc/>
public class CdpKeyboard : Keyboard
{
    private readonly HashSet<string> _pressedKeys = [];
    private CDPSession _client;

    internal CdpKeyboard(CDPSession client)
    {
        _client = client;
    }

    /// <inheritdoc/>
    public override Task DownAsync(string key, DownOptions options = null)
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
            IsKeypad = description.Location == 3,
        });
    }

    /// <inheritdoc/>
    public override Task UpAsync(string key)
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
            Location = description.Location,
        });
    }

    /// <inheritdoc/>
    public override Task SendCharacterAsync(string charText)
        => _client.SendAsync("Input.insertText", new InputInsertTextRequest
        {
            Text = charText,
        });

    /// <inheritdoc/>
    public override async Task TypeAsync(string text, TypeOptions options = null)
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
    public override async Task PressAsync(string key, PressOptions options = null)
    {
        await DownAsync(key, options).ConfigureAwait(false);
        if (options?.Delay > 0)
        {
            await Task.Delay((int)options.Delay).ConfigureAwait(false);
        }

        await UpAsync(key).ConfigureAwait(false);
    }

    internal void UpdateClient(CDPSession newSession) => _client = newSession;

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
            Location = 0,
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
