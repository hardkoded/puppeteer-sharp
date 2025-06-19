using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.EvaluationTests
{
    public class PageEvaluateOnNewDocumentTests : PuppeteerPageBaseTest
    {
        public PageEvaluateOnNewDocumentTests() : base()
        {
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluateOnNewDocument", "should evaluate before anything else on the page")]
        public async Task ShouldEvaluateBeforeAnythingElseOnThePage()
        {
            await Page.EvaluateFunctionOnNewDocumentAsync(@"function(){
                window.injected = 123;
            }");
            await Page.GoToAsync(TestConstants.ServerUrl + "/tamperable.html");
            Assert.That(await Page.EvaluateExpressionAsync<int>("window.result"), Is.EqualTo(123));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluateOnNewDocument", "should work with CSP")]
        public async Task ShouldWorkWithCSP()
        {
            Server.SetCSP("/empty.html", "script-src " + TestConstants.ServerUrl);
            await Page.EvaluateFunctionOnNewDocumentAsync(@"function(){
                window.injected = 123;
            }");
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(await Page.EvaluateExpressionAsync<int>("window.injected"), Is.EqualTo(123));

            // Make sure CSP works.
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.e = 10;"
            }).ContinueWith(_ => Task.CompletedTask);
            Assert.That(await Page.EvaluateExpressionAsync("window.e"), Is.Null);
        }

        [Test, Ignore("Inconsistent results on Firefox")]
        public async Task ShouldWorkWithExpressions()
        {
            await Page.EvaluateExpressionOnNewDocumentAsync("window.injected = 123;");
            await Page.GoToAsync(TestConstants.ServerUrl + "/tamperable.html");
            Assert.That(await Page.EvaluateExpressionAsync<int>("window.result"), Is.EqualTo(123));
        }
    }
}
