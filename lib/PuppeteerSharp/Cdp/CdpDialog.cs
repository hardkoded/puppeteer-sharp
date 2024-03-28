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

using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc />
public class CdpDialog : Dialog
{
    private readonly CDPSession _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="CdpDialog"/> class.
    /// </summary>
    /// <param name="client">Client.</param>
    /// <param name="type">Type.</param>
    /// <param name="message">Message.</param>
    /// <param name="defaultValue">Default value.</param>
    public CdpDialog(CDPSession client, DialogType type, string message, string defaultValue) : base(type, message, defaultValue)
    {
        _client = client;
    }

    internal override Task HandleAsync(bool accept, string text)
        => _client.SendAsync("Page.handleJavaScriptDialog", new PageHandleJavaScriptDialogRequest
        {
            Accept = accept,
            PromptText = text,
        });
}
