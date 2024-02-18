using System;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.ChromeLauncherTests
{
    public class RemoveMatchingFlagsTests : PuppeteerPageBaseTest
    {

        [Test, PuppeteerTimeout, Retry(2), PuppeteerTest("ChromeLauncher.test.ts", "removeMatchingFlags", "empty")]
        public void Empty()
        {
            var a = Array.Empty<string>();
            var result = ChromiumLauncher.RemoveMatchingFlags(a, "--foo");
            Assert.IsEmpty(result);
        }

        [Test, PuppeteerTimeout, Retry(2), PuppeteerTest("ChromeLauncher.test.ts", "removeMatchingFlags", "with one match")]
        public void WithOneMatch()
        {
            var a = new[] { "--foo=1", "--bar=baz" };
            var result = ChromiumLauncher.RemoveMatchingFlags(a, "--foo");
            Assert.AreEqual(new[] { "--bar=baz" }, result);
        }

        [Test, PuppeteerTimeout, Retry(2), PuppeteerTest("ChromeLauncher.test.ts", "removeMatchingFlags", "with multiple matches")]
        public void WithMultipleMatches()
        {
            var a = new[] { "--foo=1", "--bar=baz", "--foo=2" };
            var result = ChromiumLauncher.RemoveMatchingFlags(a, "--foo");
            Assert.AreEqual(new[] { "--bar=baz" }, result);
        }

        [Test, PuppeteerTimeout, Retry(2), PuppeteerTest("ChromeLauncher.test.ts", "removeMatchingFlags", "with no matches")]
        public void WithNoMatches()
        {
            var a = new[] { "--foo=1", "--bar=baz" };
            var result = ChromiumLauncher.RemoveMatchingFlags(a, "--baz");
            Assert.AreEqual(new[] { "--foo=1", "--bar=baz" }, result);
        }
    }
}
