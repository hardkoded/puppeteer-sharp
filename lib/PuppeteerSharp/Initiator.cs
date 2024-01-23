// * MIT License
//  *
//  * Copyright (c) 2020 Darío Kondratiuk
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
/// Information about the request initiator.
/// </summary>
public class Initiator
{
    /// <summary>
    /// Gets or sets the type of the initiator.
    /// </summary>
    public InitiatorType Type { get; set; }

    /// <summary>
    /// Initiator URL, set for Parser type or for Script type (when script is importing module) or for SignedExchange type.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Initiator line number, set for Parser type or for Script type (when script is importing module) (0-based).
    /// </summary>
    public int? LineNumber { get; set; }

    /// <summary>
    /// Initiator column number, set for Parser type or for Script type (when script is importing module) (0-based).
    /// </summary>
    public int? ColumnNumber { get; set; }

    /// <summary>
    /// Set if another request triggered this request (e.g. preflight).
    /// </summary>
    public string RequestId { get; set; }
}
