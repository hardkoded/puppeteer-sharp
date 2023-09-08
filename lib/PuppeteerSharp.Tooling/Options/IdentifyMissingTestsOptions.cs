/*
 * MIT License
 *
 * Copyright (c) Microsoft Corporation.
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

using CommandLine;

namespace PuppeteerSharp.Tooling
{
    /// <summary>
    /// Describes the options for scaffolding the tests.
    /// </summary>
    [Verb("missing-tests", HelpText = "Checks if there are missing tests in the C# variant, compared to the specs.")]
    internal class IdentifyMissingTestsOptions
    {
        [Option(Required = true, HelpText = "Location of spec files.")]
        public string SpecFileLocations { get; set; }

        [Option(Required = false, HelpText = "The search pattern to use for spec files.", Default = "*.spec.ts")]
        public string Pattern { get; set; }

        [Option(Required = false, Default = true, HelpText = "When True, looks inside subdirectories of specified location as well.")]
        public bool Recursive { get; set; }
    }
}
