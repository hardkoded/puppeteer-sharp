using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NavigationTests
{
    public class PageGoBackTests : PuppeteerPageBaseTest
    {
        public PageGoBackTests() : base()
        {
        }

        //TODO: This is working in puppeteer. I don't know why is hanging here.
        [Test, PuppeteerTest("navigation.spec", "navigation Page.goBack", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");

            var response = await Page.GoBackAsync();
            Assert.That(response.Ok, Is.True);
            Assert.That(response.Url, Is.EqualTo(TestConstants.EmptyPage));

            response = await Page.GoForwardAsync();
            Assert.That(response.Ok, Is.True);
            Assert.That(response.Url, Does.Contain("grid"));

            response = await Page.GoForwardAsync();
            Assert.That(response, Is.Null);
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goBack", "should work with HistoryAPI")]
        public async Task ShouldWorkWithHistoryAPI()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateExpressionAsync(@"
              history.pushState({ }, '', '/first.html');
              history.pushState({ }, '', '/second.html');
            ");
            Assert.That(Page.Url, Is.EqualTo(TestConstants.ServerUrl + "/second.html"));

            await Page.GoBackAsync();
            Assert.That(Page.Url, Is.EqualTo(TestConstants.ServerUrl + "/first.html"));
            await Page.GoBackAsync();
            Assert.That(Page.Url, Is.EqualTo(TestConstants.EmptyPage));
            await Page.GoForwardAsync();
            Assert.That(Page.Url, Is.EqualTo(TestConstants.ServerUrl + "/first.html"));
        }
    }
}
