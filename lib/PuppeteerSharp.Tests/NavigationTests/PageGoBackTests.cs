using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.NavigationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageGoBackTests : PuppeteerPageBaseTest
    {
        public PageGoBackTests(ITestOutputHelper output) : base(output)
        {
        }

        //TODO: This is working in puppeteer. I don't know why is hanging here.
        [PuppeteerTest("page.spec.ts", "Page.goBack", "should work")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");

            var response = await Page.GoBackAsync();
            Assert.True(response.Ok);
            Assert.Equal(TestConstants.EmptyPage, response.Url);

            response = await Page.GoForwardAsync();
            Assert.True(response.Ok);
            Assert.Contains("grid", response.Url);

            response = await Page.GoForwardAsync();
            Assert.Null(response);
        }

        [PuppeteerTest("page.spec.ts", "Page.goBack", "should work with HistoryAPI")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithHistoryAPI()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateExpressionAsync(@"
              history.pushState({ }, '', '/first.html');
              history.pushState({ }, '', '/second.html');
            ");
            Assert.Equal(TestConstants.ServerUrl + "/second.html", Page.Url);

            await Page.GoBackAsync();
            Assert.Equal(TestConstants.ServerUrl + "/first.html", Page.Url);
            await Page.GoBackAsync();
            Assert.Equal(TestConstants.EmptyPage, Page.Url);
            await Page.GoForwardAsync();
            Assert.Equal(TestConstants.ServerUrl + "/first.html", Page.Url);
        }
    }
}
