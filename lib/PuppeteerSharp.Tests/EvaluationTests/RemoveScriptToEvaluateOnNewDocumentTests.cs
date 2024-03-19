using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.EvaluationTests
{
    public class RemoveScriptToEvaluateOnNewDocumentTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("evaluation.spec", "Evaluation specs Page.removeScriptToEvaluateOnNewDocument", "should remove new document script")]
        public async Task ShouldRemoveNewDocumentScript()
        {
            var id = await Page.EvaluateFunctionOnNewDocumentAsync("() => globalThis.injected = 123");
            await Page.GoToAsync(TestConstants.ServerUrl + "/tamperable.html");
            var result = await Page.EvaluateFunctionAsync<int>("async () => globalThis.result");
            Assert.AreEqual(123, result);

            await Page.RemoveScriptToEvaluateOnNewDocumentAsync(id.Identifier);

            await Page.ReloadAsync();
            Assert.IsNull(await Page.EvaluateFunctionAsync("() => globalThis.result ?? null"));
        }
    }
}
