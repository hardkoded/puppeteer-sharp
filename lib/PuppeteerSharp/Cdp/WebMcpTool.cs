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

namespace PuppeteerSharp.Cdp;

/// <summary>
/// Represents a registered WebMCP tool available on the page.
/// </summary>
public class WebMcpTool
{
    /// <summary>Tool name.</summary>
    public string Name { get; init; }

    /// <summary>Tool description.</summary>
    public string Description { get; init; }

    /// <summary>Schema for the tool's input parameters.</summary>
    public object InputSchema { get; init; }

    /// <summary>Optional annotations for the tool.</summary>
    public WebMcpAnnotation Annotations { get; init; }

    /// <summary>Frame the tool was defined for.</summary>
    public IFrame Frame { get; init; }

    /// <summary>Source location that defined the tool (if available).</summary>
    public ConsoleMessageLocation Location { get; init; }
}
