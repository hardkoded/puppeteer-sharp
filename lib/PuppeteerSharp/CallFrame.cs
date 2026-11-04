// * MIT License
//  *
//  * Copyright (c) Dario Kondratiuk
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

namespace PuppeteerSharp;

/// <summary>
/// Represents a call frame in a stack trace.
/// </summary>
public class CallFrame
{
    /// <summary>
    /// Gets or sets the JavaScript function name.
    /// </summary>
    public string FunctionName { get; set; }

    /// <summary>
    /// Gets or sets the script identifier.
    /// </summary>
    public string ScriptId { get; set; }

    /// <summary>
    /// Gets or sets the URL of the script.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the line number in the script (0-based).
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// Gets or sets the column number in the script (0-based).
    /// </summary>
    public int ColumnNumber { get; set; }
}
