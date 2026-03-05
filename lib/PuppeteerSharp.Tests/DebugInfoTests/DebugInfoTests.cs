using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.DebugInfoTests
{
    public class DebugInfoTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("debugInfo.spec", "DebugInfo Browser.debugInfo", "should work")]
        public async Task ShouldWork()
        {
            // Ensure that previous tests are flushed
            for (var i = 0; i < 5; i++)
            {
                if (Browser.DebugInfo.PendingProtocolErrors.Count == 0)
                {
                    break;
                }

                await Task.Delay(200);
            }

            Assert.That(Browser.DebugInfo.PendingProtocolErrors, Is.Empty);

            var promise = Page.EvaluateFunctionAsync(@"() => new Promise(resolve => {
                window.resolve = resolve;
            })");

            try
            {
                Assert.That(Browser.DebugInfo.PendingProtocolErrors, Has.Count.EqualTo(1));
            }
            finally
            {
                await Page.EvaluateFunctionAsync("() => window.resolve()");
            }

            await promise;
            Assert.That(Browser.DebugInfo.PendingProtocolErrors, Is.Empty);
        }
    }
}
