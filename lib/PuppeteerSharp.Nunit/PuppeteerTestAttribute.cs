using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using PuppeteerSharp.Nunit.TestExpectations;

namespace PuppeteerSharp.Nunit
{
    /// <summary>
    /// Enables decorating test facts with information about the corresponding test in the upstream repository.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PuppeteerTestAttribute : NUnitAttribute, IApplyToTest
    {
        private static TestExpectation[] _localExpectations;
        private static TestExpectation[] _upstreamExpectations;
        public static readonly bool IsChrome = Environment.GetEnvironmentVariable("PRODUCT") != "FIREFOX";
        // TODO: Change implementation when we implement Webdriver Bidi
        public static readonly bool IsCdp = true;
        public static readonly HeadlessMode Headless =
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HEADLESS_MODE")) ?
            (System.Diagnostics.Debugger.IsAttached ? HeadlessMode.False : HeadlessMode.True) :
            GetHeadlessModeFromEnvVar(Environment.GetEnvironmentVariable("HEADLESS_MODE"));

        private static HeadlessMode GetHeadlessModeFromEnvVar(string getEnvironmentVariable)
        {
            return getEnvironmentVariable switch
            {
                "headless" => HeadlessMode.True,
                "headful" => HeadlessMode.False,
                "headless-shell" => HeadlessMode.Shell,
                _ => throw new ArgumentException("Invalid value for HEADLESS_MODE environment variable"),
            };
        }

        /// <summary>
        /// Creates a new instance of the attribute.
        /// </summary>
        /// <param name="fileName"><see cref="FileName"/></param>
        /// <param name="nameOfTest"><see cref="TestName"/></param>
        public PuppeteerTestAttribute(string fileName, string nameOfTest)
        {
            FileName = fileName;
            TestName = nameOfTest;
        }

        /// <summary>
        /// Creates a new instance of the attribute.
        /// </summary>
        /// <param name="fileName"><see cref="FileName"/></param>
        /// <param name="describe"><see cref="Describe"/></param>
        /// <param name="nameOfTest"><see cref="TestName"/></param>
        public PuppeteerTestAttribute(string fileName, string describe, string nameOfTest) : this(fileName, nameOfTest)
        {
            Describe = describe;
        }

        /// <summary>
        /// The file name origin of the test.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Returns the trimmed file name.
        /// </summary>
        public string TrimmedName => FileName.Substring(0, FileName.IndexOf('.'));

        /// <summary>
        /// The name of the test, the decorated code is based on.
        /// </summary>
        public string TestName { get; }

        /// <summary>
        /// The describe of the test, the decorated code is based on, if one exists.
        /// </summary>
        public string Describe { get; }

        public override string ToString()
            => Describe == null ? $"[FileName] {TestName}" : $"[{FileName}] {Describe} {TestName}";

        public void ApplyToTest(Test test)
        {
            if (test == null)
            {
                return;
            }

            if (ShouldSkipByExpectation(test, out var expectation))
            {
                test.RunState = RunState.Ignored;
                test.Properties.Set(global::NUnit.Framework.Internal.PropertyNames.SkipReason, $"Skipped by expectation {expectation.TestIdPattern}");
            }
        }

        private bool ShouldSkipByExpectation(Test test, out TestExpectation output)
        {
            var currentExpectationPlatform = GetCurrentExpectationPlatform();
            List<TestExpectation.TestExpectationsParameter> parameters =
            [
                IsChrome
                    ? TestExpectation.TestExpectationsParameter.Chrome
                    : TestExpectation.TestExpectationsParameter.Firefox,

                IsCdp
                    ? TestExpectation.TestExpectationsParameter.Cdp
                    : TestExpectation.TestExpectationsParameter.WebDriverBiDi,
                Headless.Equals(HeadlessMode.True)
                    ? TestExpectation.TestExpectationsParameter.Headless
                    : Headless.Equals(HeadlessMode.False)
                        ? TestExpectation.TestExpectationsParameter.Headful
                        : TestExpectation.TestExpectationsParameter.ChromeHeadlessShell
            ];

            var localExpectations = GetLocalExpectations();
            var upstreamExpectations = GetUpstreamExpectations();
            // Join local and upstream in one variable
            var allExpectations = localExpectations.Concat(upstreamExpectations).ToArray();

            foreach (var expectation in allExpectations)
            {
                if (expectation.TestIdRegex.IsMatch(ToString()))
                {
                    if (expectation.Platforms.Contains(currentExpectationPlatform) &&
                        expectation.Parameters.All(parameters.Contains) &&
                        (
                            expectation.Expectations.Contains(TestExpectation.TestExpectationResult.Skip) ||
                            expectation.Expectations.Contains(TestExpectation.TestExpectationResult.Fail) ||
                            expectation.Expectations.Contains(TestExpectation.TestExpectationResult.Timeout)))
                    {
                        output = expectation;
                        return true;
                    }
                }
            }

            output = null;
            return false;
        }

        private static TestExpectation.TestExpectationPlatform GetCurrentExpectationPlatform()
        {
            var currentExpectationPlatform =
                TestExpectation.TestExpectationPlatform.Linux;

            if (RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                currentExpectationPlatform = TestExpectation.TestExpectationPlatform.Win32;
            }
            else if (RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                currentExpectationPlatform = TestExpectation.TestExpectationPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
            {
                currentExpectationPlatform = TestExpectation.TestExpectationPlatform.Darwin;
            }

            return currentExpectationPlatform;
        }

        private static TestExpectation[] GetLocalExpectations() =>
            _localExpectations ??= LoadExpectationsFromResource("PuppeteerSharp.Nunit.TestExpectations.TestExpectations.local.json");

        private static TestExpectation[] GetUpstreamExpectations() =>
            _upstreamExpectations ??= LoadExpectationsFromResource("PuppeteerSharp.Nunit.TestExpectations.TestExpectations.upstream.json");

        private static TestExpectation[] LoadExpectationsFromResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            var fileContent = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<TestExpectation[]>(fileContent);
        }
    }
}
