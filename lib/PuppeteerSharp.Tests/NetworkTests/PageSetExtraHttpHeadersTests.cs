using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.NetworkTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageSetExtraHttpHeadersTests : PuppeteerPageBaseTest
    {
        public PageSetExtraHttpHeadersTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            await Page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
            {
                ["Foo"] = "Bar"
            });

            var headerTask = Server.WaitForRequest("/empty.html", request => request.Headers["Foo"]);
            await Task.WhenAll(Page.GoToAsync(TestConstants.EmptyPage), headerTask);

            Assert.Equal("Bar", headerTask.Result);
        }
    }
}
