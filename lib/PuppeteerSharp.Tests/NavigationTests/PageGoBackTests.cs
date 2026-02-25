using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NavigationTests
{
    public class PageGoBackTests : PuppeteerPageBaseTest
    {
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
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goBack", "should error if no history is found")]
        public async Task ShouldErrorIfNoHistoryIsFound()
        {
            var exception = Assert.CatchAsync<PuppeteerException>(async () => await Page.GoBackAsync());
            Assert.That(exception, Is.Not.Null);
            Assert.That(
                exception.Message,
                Does.Contain("History entry to navigate to not found.").Or.Contain("no such history entry"));
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

            var response = await Page.GoBackAsync();
            Assert.That(response, Is.Null);
            Assert.That(Page.Url, Is.EqualTo(TestConstants.ServerUrl + "/first.html"));
            await Page.GoBackAsync();
            Assert.That(Page.Url, Is.EqualTo(TestConstants.EmptyPage));
            response = await Page.GoForwardAsync();
            Assert.That(response, Is.Null);
            Assert.That(Page.Url, Is.EqualTo(TestConstants.ServerUrl + "/first.html"));
        }
    }
}
