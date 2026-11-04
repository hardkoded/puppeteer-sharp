using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.WaitTaskTests
{
    public class ProtocolTimeoutTests : PuppeteerPageBaseTest
    {
        public ProtocolTimeoutTests()
        {
            DefaultOptions = TestConstants.DefaultBrowserOptions();
            DefaultOptions.ProtocolTimeout = 5000;
        }

        [Test, PuppeteerTest("waittask.spec", "waittask specs protocol timeout", "should error if underyling protocol command times out with raf polling")]
        public void ShouldErrorIfUnderlyingProtocolCommandTimesOutWithRafPolling()
        {
            var exception = Assert.CatchAsync<Exception>(async () =>
                await Page.WaitForFunctionAsync(
                    "() => false",
                    new WaitForFunctionOptions { Timeout = 6000 }));

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Waiting failed"));
            Assert.That(exception.InnerException, Is.Not.Null);
        }
    }
}
