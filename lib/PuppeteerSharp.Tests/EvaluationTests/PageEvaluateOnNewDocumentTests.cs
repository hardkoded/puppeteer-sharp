using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.EvaluationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageEvaluateOnNewDocumentTests : PuppeteerPageBaseTest
    {
        public PageEvaluateOnNewDocumentTests(): base()
        {
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluateOnNewDocument", "should evaluate before anything else on the page")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldEvaluateBeforeAnythingElseOnThePage()
        {
            await Page.EvaluateFunctionOnNewDocumentAsync(@"function(){
                window.injected = 123;
            }");
            await Page.GoToAsync(TestConstants.ServerUrl + "/tamperable.html");
            Assert.Equal(123, await Page.EvaluateExpressionAsync<int>("window.result"));
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluateOnNewDocument", "should work with CSP")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithCSP()
        {
            Server.SetCSP("/empty.html", "script-src " + TestConstants.ServerUrl);
            await Page.EvaluateFunctionOnNewDocumentAsync(@"function(){
                window.injected = 123;
            }");
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(123, await Page.EvaluateExpressionAsync<int>("window.injected"));

            // Make sure CSP works.
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.e = 10;"
            }).ContinueWith(_ => Task.CompletedTask);
            Assert.Null(await Page.EvaluateExpressionAsync("window.e"));
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithExpressions()
        {
            await Page.EvaluateExpressionOnNewDocumentAsync("window.injected = 123;");
            await Page.GoToAsync(TestConstants.ServerUrl + "/tamperable.html");
            Assert.Equal(123, await Page.EvaluateExpressionAsync<int>("window.result"));
        }
    }
}
