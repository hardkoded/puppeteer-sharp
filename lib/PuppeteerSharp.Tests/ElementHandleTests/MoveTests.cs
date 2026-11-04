using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class MoveTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.move", "should work")]
        public async Task ShouldWork()
        {
            var handle = await Page.EvaluateExpressionHandleAsync("document");

            {
                await using var _ = handle;
                handle.Move();
            }

            Assert.That(handle.Disposed, Is.False);

            await handle.DisposeAsync();
            Assert.That(handle.Disposed, Is.True);
        }
    }
}
