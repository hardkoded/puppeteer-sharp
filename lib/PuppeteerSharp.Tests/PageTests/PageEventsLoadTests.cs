using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageEventsLoadTests : PuppeteerPageBaseTest
    {
        public PageEventsLoadTests(): base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Load", "should fire when expected")]
        [PuppeteerTimeout]
        public async Task ShouldFireWhenExpected()
        {
            var _ = Page.GoToAsync(TestConstants.AboutBlank);
            var completion = new TaskCompletionSource<bool>();
            Page.Load += (_, _) => completion.SetResult(true);
            await completion.Task;
        }
    }
}
