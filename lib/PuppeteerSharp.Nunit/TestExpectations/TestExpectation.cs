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
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PuppeteerSharp.Nunit.TestExpectations;

public class TestExpectation
{
    public string TestIdPattern { get; set; }

    public Regex TestIdRegex
    {
        get
        {
            var patternRegExString = TestIdPattern
                // Replace `*` with non special character
                .Replace("*", "--STAR--");

            // Escape special characters https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Regular_Expressions#escaping
            patternRegExString = Regex.Escape(patternRegExString);

            // Replace placeholder with greedy match
            patternRegExString = patternRegExString.Replace("--STAR--", "(.*)?");

            // Match beginning and end explicitly
            return new Regex($"^{patternRegExString}$");
        }
    }

    public TestExpectationPlatform[] Platforms { get; set; }

    public TestExpectationsParameter[] Parameters { get; set; }

    public TestExpectationResult[] Expectations { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TestExpectationResult
    {
        [EnumMember(Value = "FAIL")] Fail,
        [EnumMember(Value = "PASS")] Pass,
        [EnumMember(Value = "SKIP")] Skip,
        [EnumMember(Value = "TIMEOUT")] Timeout,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TestExpectationsParameter
    {
        Firefox,
        Chrome,
        WebDriverBiDi,
        Cdp,
        [EnumMember(Value = "chrome-headless-shell")]
        ChromeHeadlessShell,
        Headless,
        Headful,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TestExpectationPlatform
    {
        Darwin,
        Linux,
        Win32,
    }
}
