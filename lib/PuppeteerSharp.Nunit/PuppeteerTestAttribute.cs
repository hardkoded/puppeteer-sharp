using System;
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
        public static readonly bool IsChrome = Environment.GetEnvironmentVariable("PRODUCT") != "FIREFOX";

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
            => Describe == null ? $"{FileName}: {TestName}" : $"{FileName}: {Describe}: {TestName}";

        public void ApplyToTest(Test test)
        {
            if(ShouldSkipByExpectation(test, out var expectation))
            {
                test.RunState = RunState.Ignored;
                test.Properties.Set(global::NUnit.Framework.Internal.PropertyNames.SkipReason, $"Skipped by expectation {expectation.TestIdPattern}");
            }

            if (_combinations.Any(combination =>
                {
                    var requirements = (Enum.GetValues(typeof(Targets)) as Targets[]).Where(x => combination.HasFlag(x));

                }))
            {
                test.RunState = RunState.Ignored;
                test.Properties.Set(global::NUnit.Framework.Internal.PropertyNames.SkipReason, "Skipped by browser/platform");
            }
        }

        private bool ShouldSkipByExpectation(Test test, out TestExpectation output)
        {
            TestExpectation.TestExpectationPlatform currentExpectationPlatform =
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

            var localExpectations = GetLocalExpectations();
            var upstreamExpectations = GetUpstreamExpectations();
            // Join local and upstream in one variable
            var allExpectations = localExpectations.Concat(upstreamExpectations).ToArray();

            foreach(var expectation in allExpectations)
            {
                if (expectation.TestIdRegex.IsMatch(ToString()))
                {
                    if (expectation.Platforms.Contains(currentExpectationPlatform) &&
                        expectation.Parameters.Contains(IsChrome ? TestExpectation.TestExpectationsParameter.Chromium : TestExpectation.TestExpectationsParameter.Firefox) &&
                        (
                            expectation.Expectations.Contains(TestExpectation.TestExpectationResult.Skip) ||
                            expectation.Expectations.Contains(TestExpectation.TestExpectationResult.Fail)))
                    {
                        output = expectation;
                        return true;
                    }
                }
            }

            output = null;
            return false;
        }

        private static TestExpectation[] GetLocalExpectations() =>
            _localExpectations ??= LoadExpectationsFromResource("PuppeteerSharp.Nunit.TestExpectations.local.json");

        private static TestExpectation[] GetUpstreamExpectations() =>
            _localExpectations ??= LoadExpectationsFromResource("PuppeteerSharp.Nunit.TestExpectations.upstream.json");

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
