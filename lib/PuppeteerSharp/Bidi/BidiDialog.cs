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

using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;

namespace PuppeteerSharp.Bidi;

/// <summary>
/// BiDi implementation of <see cref="Dialog"/>.
/// </summary>
public class BidiDialog : Dialog
{
    private readonly UserPrompt _prompt;

    private BidiDialog(UserPrompt prompt)
        : base(ConvertDialogType(prompt.Info.PromptType), prompt.Info.Message, prompt.Info.DefaultValue ?? string.Empty)
    {
        _prompt = prompt;
    }

    internal static BidiDialog From(UserPrompt prompt)
    {
        return new BidiDialog(prompt);
    }

    internal override Task HandleAsync(bool accept, string text)
    {
        return _prompt.HandleAsync(accept, text);
    }

    private static DialogType ConvertDialogType(WebDriverBiDi.BrowsingContext.UserPromptType type)
    {
        return type switch
        {
            WebDriverBiDi.BrowsingContext.UserPromptType.Alert => DialogType.Alert,
            WebDriverBiDi.BrowsingContext.UserPromptType.Confirm => DialogType.Confirm,
            WebDriverBiDi.BrowsingContext.UserPromptType.Prompt => DialogType.Prompt,
            WebDriverBiDi.BrowsingContext.UserPromptType.BeforeUnload => DialogType.BeforeUnload,
            _ => DialogType.Alert,
        };
    }
}

#endif
