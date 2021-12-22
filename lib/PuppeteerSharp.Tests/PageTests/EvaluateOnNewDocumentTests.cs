using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class EvaluateOnNewDocumentTests : PuppeteerPageBaseTest
    {
        public EvaluateOnNewDocumentTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldEvaluateBeforeAnythingElseOnThePage()
        {
            await Page.EvaluateFunctionOnNewDocumentAsync(@"function(){
                window.injected = 123;
            }");
            await Page.GoToAsync(TestConstants.ServerUrl + "/tamperable.html");
            Assert.Equal(123, await Page.EvaluateExpressionAsync<int>("window.result"));
        }

        [Fact]
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

        [Fact]
        public async Task ShouldWorkWithExpressions()
        {
            await Page.EvaluateExpressionOnNewDocumentAsync("window.injected = 123;");
            await Page.GoToAsync(TestConstants.ServerUrl + "/tamperable.html");
            Assert.Equal(123, await Page.EvaluateExpressionAsync<int>("window.result"));
        }
    }
}
