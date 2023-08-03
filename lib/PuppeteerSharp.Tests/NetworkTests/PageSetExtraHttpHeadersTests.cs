using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class PageSetExtraHttpHeadersTests : PuppeteerPageBaseTest
    {
        public PageSetExtraHttpHeadersTests(): base()
        {
        }

        [PuppeteerTest("network.spec.ts", "Page.setExtraHTTPHeaders", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
            await Page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
            {
                ["Foo"] = "Bar"
            });

            var headerTask = Server.WaitForRequest("/empty.html", request => request.Headers["Foo"]);
            await Task.WhenAll(Page.GoToAsync(TestConstants.EmptyPage), headerTask);

            Assert.AreEqual("Bar", headerTask.Result);
        }
    }
}
