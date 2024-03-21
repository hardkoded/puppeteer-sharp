using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class RemoveExposeFunctionTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.removeExposedFunction", "should work")]
        public async Task ShouldWork()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => a * b);
            var result = await Page.EvaluateFunctionAsync<int>("async () => compute(9, 4)");
            Assert.AreEqual(36, result);

            await Page.RemoveExposedFunctionAsync("compute");

            Assert.ThrowsAsync<EvaluationFailedException>(() => Page.EvaluateFunctionAsync<int>("async () => compute(9, 4)"));
        }
    }
}
