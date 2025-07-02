using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class RemoveExposeFunctionTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("page.spec", "Page Page.removeExposedFunction", "should work")]
        public async Task ShouldWork()
        {
            // Run twice to verify function can be added again after remove
            for (var i = 0; i < 2; i++)
            {
                await Page.ExposeFunctionAsync("compute", (int a, int b) => a * b);
                var result = await Page.EvaluateFunctionAsync<int>("async () => compute(9, 4)");
                Assert.That(result, Is.EqualTo(36));

                await Page.RemoveExposedFunctionAsync("compute");
            }
            Assert.ThrowsAsync<EvaluationFailedException>(() => Page.EvaluateFunctionAsync<int>("async () => compute(9, 4)"));
        }
    }
}
