using System;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.ChromeLauncherTests
{
    public class GetFeaturesTests : PuppeteerPageBaseTest
    {

        [PuppeteerTest("ChromeLauncher.test.ts", "getFeatures", "returns an empty array when no options are provided")]
        public void ReturnsAnEmptyArrayWhenNoOptionsAreProvided()
        {
            var result = ChromiumLauncher.GetFeatures("--foo", Array.Empty<string>());
            Assert.IsEmpty(result);
        }

        [PuppeteerTest("ChromeLauncher.test.ts", "getFeatures", "returns an empty array when no options match the flag")]
        public void ReturnsAnEmptyArrayWhenNoOptionsMatchTheFlag()
        {
            var result = ChromiumLauncher.GetFeatures("--foo", new[] { "--bar", "--baz" });
            Assert.IsEmpty(result);
        }

        [PuppeteerTest("ChromeLauncher.test.ts", "getFeatures", "returns an array of values when options match the flag")]
        public void ReturnsAnArrayOfValuesWhenOptionsMatchTheFlag()
        {
            var result = ChromiumLauncher.GetFeatures("--foo", new[] { "--foo=bar", "--foo=baz" });
            Assert.AreEqual(new[] { "bar", "baz" }, result);
        }

        [PuppeteerTest("ChromeLauncher.test.ts", "getFeatures", "does not handle whitespace")]
        public void DoesNotHandleWhitespace()
        {
            var result = ChromiumLauncher.GetFeatures("--foo", new[] { "--foo bar", "--foo baz " });
            Assert.IsEmpty(result);
        }

        [PuppeteerTest("ChromeLauncher.test.ts", "getFeatures", "handles equals sign around the flag and value")]
        public void HandlesEqualsSignAroundTheFlagAndValue()
        {
            var result = ChromiumLauncher.GetFeatures("--foo", new[] { "--foo=bar", "--foo=baz" });
            Assert.AreEqual(new[] { "bar", "baz" }, result);
        }
    }
}
