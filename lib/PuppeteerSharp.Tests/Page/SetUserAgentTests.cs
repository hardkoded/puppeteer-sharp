using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class SetUserAgentTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            Assert.Contains("Mozilla", await Page.EvaluateFunctionAsync<string>("() => navigator.userAgent"));
            await Page.SetUserAgentAsync("foobar");

            var userAgentTask = Server.WaitForRequest("/empty.html", request => request.Headers["User-Agent"].ToString());
            await Task.WhenAll(
                userAgentTask,
                Page.GoToAsync(TestConstants.EmptyPage)
            );
            Assert.Equal("foobar", userAgentTask.Result);
        }
    }
}
