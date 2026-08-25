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

using System.Threading;

namespace PuppeteerSharp.Cdp;

/// <summary>
/// Options for <see cref="WebMcpTool.ExecuteAsync(object, WebMcpToolExecuteOptions)"/>.
/// </summary>
public class WebMcpToolExecuteOptions
{
    /// <summary>
    /// A <see cref="CancellationToken"/> that allows you to cancel the tool execution.
    /// </summary>
    /// <remarks>
    /// This is the .NET equivalent of the upstream AbortController/AbortSignal pattern.
    /// When the token is cancelled, PuppeteerSharp requests cancellation via CDP and the
    /// execution resolves with <see cref="WebMcpInvocationStatus.Canceled"/> rather than
    /// throwing <see cref="System.OperationCanceledException"/>.
    /// </remarks>
    public CancellationToken CancellationToken { get; set; }
}
