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

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.BrowserData;

/// <summary>
/// Firefox channels.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter<FirefoxChannel>))]
public enum FirefoxChannel
{
    /// <summary>
    /// Stable.
    /// </summary>
    [EnumMember(Value = "stable")]
    Stable = 0,

    /// <summary>
    /// Beta.
    /// </summary>
    [EnumMember(Value = "beta")]
    Beta = 1,

    /// <summary>
    /// Nightly.
    /// </summary>
    [EnumMember(Value = "nightly")]
    Nightly = 2,

    /// <summary>
    /// ESR.
    /// </summary>
    [EnumMember(Value = "esr")]
    Esr = 3,

    /// <summary>
    /// Developer Edition.
    /// </summary>
    [EnumMember(Value = "devedition")]
    DevEdition = 4,
}
