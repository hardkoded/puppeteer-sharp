using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageEventsDOMContentLoadedTests : PuppeteerPageBaseTest
    {
        public PageEventsDOMContentLoadedTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.Events.DOMContentLoaded", "should fire when expected")]
        public async Task ShouldFireWhenExpected()
        {
            var _ = Page.GoToAsync(TestConstants.AboutBlank);
            var completion = new TaskCompletionSource<bool>();
            Page.DOMContentLoaded += (_, _) => completion.SetResult(true);
            await completion.Task;
        }
    }
}
