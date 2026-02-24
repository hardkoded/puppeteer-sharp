using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class BackendNodeIdTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("backendNodeId.spec", "ElementHandle.backendNodeId", "should work")]
        public async Task ShouldWork()
        {
            var handle = (IElementHandle)await Page.EvaluateExpressionHandleAsync("document");
            var id = await handle.BackendNodeIdAsync();
            Assert.That(id, Is.GreaterThan(0));
        }
    }
}
