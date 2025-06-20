using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.NavigationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextGoBackTests : DevToolsContextBaseTest
    {
        public DevToolsContextGoBackTests(ITestOutputHelper output) : base(output)
        {
        }

        //TODO: This is working in puppeteer. I don't know why is hanging here.
        [PuppeteerTest("navigation.spec.ts", "Page.goBack", "should work")]
        [PuppeteerRetryFact()]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");

            await ChromiumWebBrowser.WaitForRenderIdleAsync();

            var response = await DevToolsContext.GoBackAsync();
            Assert.True(response.Ok);
            Assert.Equal(TestConstants.EmptyPage, response.Url);

            response = await DevToolsContext.GoForwardAsync();
            Assert.True(response.Ok);
            Assert.Contains("grid", response.Url);

            response = await DevToolsContext.GoForwardAsync();
            Assert.Null(response);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goBack", "should work with HistoryAPI")]
        [PuppeteerFact]
        public async Task ShouldWorkWithHistoryAPI()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.EvaluateExpressionAsync(@"
              history.pushState({ }, '', '/first.html');
              history.pushState({ }, '', '/second.html');
            ");
            Assert.Equal(TestConstants.ServerUrl + "/second.html", DevToolsContext.Url);

            await DevToolsContext.GoBackAsync();
            Assert.Equal(TestConstants.ServerUrl + "/first.html", DevToolsContext.Url);
            await DevToolsContext.GoBackAsync();
            Assert.Equal(TestConstants.EmptyPage, DevToolsContext.Url);
            await DevToolsContext.GoForwardAsync();
            Assert.Equal(TestConstants.ServerUrl + "/first.html", DevToolsContext.Url);
        }
    }
}
