using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EvaluateOnNewDocumentTests : PuppeteerPageBaseTest
    {
        public EvaluateOnNewDocumentTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldEvaluateBeforeAnythingElseOnThePage()
        {
            await Page.EvaluateOnNewDocumentAsync(@"function(){
                window.injected = 123;
            }");
            await Page.GoToAsync(TestConstants.ServerUrl + "/tamperable.html");
            Assert.Equal(123, await Page.EvaluateExpressionAsync<int>("window.result"));
        }

        [Fact]
        public async Task ShouldWorkWithCSP()
        {
            Server.SetCSP("/empty.html", "script-src " + TestConstants.ServerUrl);
            await Page.EvaluateOnNewDocumentAsync(@"function(){
                window.injected = 123;
            }");
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(123, await Page.EvaluateExpressionAsync<int>("window.injected"));

            // Make sure CSP works.
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.e = 10;"
            }).WithExceptionIgnore();
            Assert.Null(await Page.EvaluateExpressionAsync("window.e"));
        }
    }
}
