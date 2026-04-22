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

using System.Text.Json;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Cdp;

/// <summary>
/// Represents a pending or completed WebMCP tool invocation call.
/// </summary>
public class WebMcpToolCall
{
    internal WebMcpToolCall(string invocationId, WebMcpTool tool, string input)
    {
        Id = invocationId;
        Tool = tool;
        try
        {
            Input = JsonSerializer.Deserialize<object>(input, JsonHelper.DefaultJsonSerializerSettings.Value);
        }
        catch
        {
            Input = null;
        }
    }

    /// <summary>Tool invocation identifier.</summary>
    public string Id { get; }

    /// <summary>The input parameters used for the call.</summary>
    public object Input { get; }

    /// <summary>Tool that was called.</summary>
    public WebMcpTool Tool { get; }
}
