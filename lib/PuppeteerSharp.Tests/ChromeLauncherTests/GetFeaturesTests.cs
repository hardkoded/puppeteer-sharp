using System;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ChromeLauncherTests
{
    public class GetFeaturesTests : PuppeteerPageBaseTest
    {

        [Test, Retry(2), PuppeteerTest("ChromeLauncher.test.ts", "getFeatures", "returns an empty array when no options are provided")]
        public void ReturnsAnEmptyArrayWhenNoOptionsAreProvided()
        {
            var result = ChromeLauncher.GetFeatures("--foo", Array.Empty<string>());
            Assert.IsEmpty(result);
        }

        [Test, Retry(2), PuppeteerTest("ChromeLauncher.test.ts", "getFeatures", "returns an empty array when no options match the flag")]
        public void ReturnsAnEmptyArrayWhenNoOptionsMatchTheFlag()
        {
            var result = ChromeLauncher.GetFeatures("--foo", new[] { "--bar", "--baz" });
            Assert.IsEmpty(result);
        }

        [Test, Retry(2), PuppeteerTest("ChromeLauncher.test.ts", "getFeatures", "returns an array of values when options match the flag")]
        public void ReturnsAnArrayOfValuesWhenOptionsMatchTheFlag()
        {
            var result = ChromeLauncher.GetFeatures("--foo", new[] { "--foo=bar", "--foo=baz" });
            Assert.AreEqual(new[] { "bar", "baz" }, result);
        }

        [Test, Retry(2), PuppeteerTest("ChromeLauncher.test.ts", "getFeatures", "does not handle whitespace")]
        public void DoesNotHandleWhitespace()
        {
            var result = ChromeLauncher.GetFeatures("--foo", new[] { "--foo bar", "--foo baz " });
            Assert.IsEmpty(result);
        }

        [Test, Retry(2), PuppeteerTest("ChromeLauncher.test.ts", "getFeatures", "handles equals sign around the flag and value")]
        public void HandlesEqualsSignAroundTheFlagAndValue()
        {
            var result = ChromeLauncher.GetFeatures("--foo", new[] { "--foo=bar", "--foo=baz" });
            Assert.AreEqual(new[] { "bar", "baz" }, result);
        }
    }
}
