/*
 * MIT License
 *
 * Copyright (c) 2020 Darío Kondratiuk
 * Modifications copyright (c) Microsoft Corporation.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace PuppeteerSharp.Tests.Attributes
{
    public class SkipAttribute : NUnitAttribute, IApplyToTest
    {
        private readonly Targets[] _combinations;

        [Flags]
        public enum Targets : short
        {
            Windows = 1 << 0,
            Linux = 1 << 1,
            OSX = 1 << 2,
            Chromium = 1 << 3,
            Firefox = 1 << 4,
            Webkit = 1 << 5
        }

        /// <summary>
        /// Skips the combinations provided.
        /// </summary>
        /// <param name="pairs"></param>
        public SkipAttribute(params Targets[] combinations)
        {
            _combinations = combinations;
        }

        public void ApplyToTest(Test test)
        {
            if (_combinations.Any(combination =>
            {
                var requirements = (Enum.GetValues(typeof(Targets)) as Targets[]).Where(x => combination.HasFlag(x));
                return requirements.All(flag =>
                    flag switch
                    {
                        Targets.Windows => RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows),
                        Targets.Linux => RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux),
                        Targets.OSX => RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX),
                        Targets.Chromium => TestConstants.IsChrome,
                        Targets.Firefox => !TestConstants.IsChrome,
                        _ => false,
                    });
            }))
            {
                test.RunState = RunState.Ignored;
                test.Properties.Set(global::NUnit.Framework.Internal.PropertyNames.SkipReason, "Skipped by browser/platform");
            }
        }
    }
}