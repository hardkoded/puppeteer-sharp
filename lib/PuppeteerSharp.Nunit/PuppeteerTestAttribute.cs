using System;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace PuppeteerSharp.Nunit
{
    /// <summary>
    /// Enables decorating test facts with information about the corresponding test in the upstream repository.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PuppeteerTestAttribute : NUnitAttribute, IApplyToTest
    {
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
