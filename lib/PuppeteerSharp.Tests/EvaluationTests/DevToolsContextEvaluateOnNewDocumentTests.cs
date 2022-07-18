using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.EvaluationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextEvaluateOnNewDocumentTests : DevToolsContextBaseTest
    {
        public DevToolsContextEvaluateOnNewDocumentTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluateOnNewDocument", "should evaluate before anything else on the page")]
        [PuppeteerFact]
        public async Task ShouldEvaluateBeforeAnythingElseOnThePage()
        {
            await DevToolsContext.EvaluateFunctionOnNewDocumentAsync(@"function(){
                window.injected = 123;
            }");
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/tamperable.html");
            Assert.Equal(123, await DevToolsContext.EvaluateExpressionAsync<int>("window.result"));
        }

        [PuppeteerTest("evaluation.spec.ts", "Page.evaluateOnNewDocument", "should work with CSP")]
        [PuppeteerFact]
        public async Task ShouldWorkWithCSP()
        {
            Server.SetCSP("/empty.html", "script-src " + TestConstants.ServerUrl);
            await DevToolsContext.EvaluateFunctionOnNewDocumentAsync(@"function(){
                window.injected = 123;
            }");
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(123, await DevToolsContext.EvaluateExpressionAsync<int>("window.injected"));

            // Make sure CSP works.
            await DevToolsContext.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.e = 10;"
            }).ContinueWith(_ => Task.CompletedTask);
            Assert.Null(await DevToolsContext.EvaluateExpressionAsync("window.e"));
        }

        [PuppeteerFact]
        public async Task ShouldWorkWithExpressions()
        {
            await DevToolsContext.EvaluateExpressionOnNewDocumentAsync("window.injected = 123;");
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/tamperable.html");
            Assert.Equal(123, await DevToolsContext.EvaluateExpressionAsync<int>("window.result"));
        }
    }
}
