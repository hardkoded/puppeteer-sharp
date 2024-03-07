using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        [Test, Retry(2), PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluateOnNewDocument", "should evaluate before anything else on the page")]
        public async Task ShouldEvaluateBeforeAnythingElseOnThePage()
        {
            await Page.EvaluateFunctionOnNewDocumentAsync(@"function(){
                window.injected = 123;
            }");
            await Page.GoToAsync(TestConstants.ServerUrl + "/tamperable.html");
            Assert.AreEqual(123, await Page.EvaluateExpressionAsync<int>("window.result"));
        }

        [Test, Retry(2), PuppeteerTest("evaluation.spec", "Evaluation specs Page.evaluateOnNewDocument", "should work with CSP")]
        public async Task ShouldWorkWithCSP()
        {
            Server.SetCSP("/empty.html", "script-src " + TestConstants.ServerUrl);
            await Page.EvaluateFunctionOnNewDocumentAsync(@"function(){
                window.injected = 123;
            }");
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.AreEqual(123, await Page.EvaluateExpressionAsync<int>("window.injected"));

            // Make sure CSP works.
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.e = 10;"
            }).ContinueWith(_ => Task.CompletedTask);
            Assert.Null(await Page.EvaluateExpressionAsync("window.e"));
        }

        public async Task ShouldWorkWithExpressions()
        {
            await Page.EvaluateExpressionOnNewDocumentAsync("window.injected = 123;");
            await Page.GoToAsync(TestConstants.ServerUrl + "/tamperable.html");
            Assert.AreEqual(123, await Page.EvaluateExpressionAsync<int>("window.result"));
        }
    }
}
