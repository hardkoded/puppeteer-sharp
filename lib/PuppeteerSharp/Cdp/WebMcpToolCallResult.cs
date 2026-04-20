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

using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Cdp;

/// <summary>
/// Result of a WebMCP tool invocation.
/// </summary>
public class WebMcpToolCallResult
{
    /// <summary>Tool invocation identifier.</summary>
    public string Id { get; init; }

    /// <summary>The corresponding tool call if available.</summary>
    public WebMcpToolCall Call { get; init; }

    /// <summary>Error text.</summary>
    public string ErrorText { get; init; }

    /// <summary>The exception object, if the JavaScript tool threw an error.</summary>
    public RemoteObject Exception { get; init; }

    /// <summary>Output delivered to the agent. Present only when Status is Completed.</summary>
    public object Output { get; init; }

    /// <summary>Status of the invocation.</summary>
    public WebMcpInvocationStatus Status { get; init; }
}
