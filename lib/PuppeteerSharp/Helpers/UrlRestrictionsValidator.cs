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

namespace PuppeteerSharp.Helpers
{
    /// <summary>
    /// Provides validation for URL restriction options (blocklist/allowlist).
    /// </summary>
    internal static class UrlRestrictionsValidator
    {
        /// <summary>
        /// Validates that the URL restrictions (<paramref name="blockList"/>/<paramref name="allowList"/>)
        /// are supported by the given <paramref name="protocol"/>. URL restrictions are only enforced by
        /// the CDP target/navigation path; BiDi connections (and Firefox's default BiDi launch path) do
        /// not support them.
        /// </summary>
        /// <param name="protocol">The protocol that the connection or launch is using.</param>
        /// <param name="blockList">The blocklist URL patterns.</param>
        /// <param name="allowList">The allowlist URL patterns.</param>
        /// <exception cref="PuppeteerException">
        /// Thrown if both <paramref name="blockList"/> and <paramref name="allowList"/> are specified,
        /// or if either is specified together with <see cref="ProtocolType.WebdriverBiDi"/>.
        /// </exception>
        public static void AssertSupportedUrlRestrictions(ProtocolType protocol, string[] blockList, string[] allowList)
        {
            if (blockList != null && allowList != null)
            {
                throw new PuppeteerException("Cannot specify both blocklist and allowlist");
            }

            if (protocol == ProtocolType.WebdriverBiDi && (blockList != null || allowList != null))
            {
                throw new PuppeteerException("blocklist and allowlist are only supported with the CDP protocol");
            }
        }
    }
}
