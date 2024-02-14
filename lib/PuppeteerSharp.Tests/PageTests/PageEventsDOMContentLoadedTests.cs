using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageEventsDOMContentLoadedTests : PuppeteerPageBaseTest
    {
        public PageEventsDOMContentLoadedTests() : base()
        {
        }

        [Test, PuppeteerTest("page.spec", "Page.Events.DOMContentLoaded", "should fire when expected")]
        [PuppeteerTimeout]
        public async Task ShouldFireWhenExpected()
        {
            var _ = Page.GoToAsync(TestConstants.AboutBlank);
            var completion = new TaskCompletionSource<bool>();
            Page.DOMContentLoaded += (_, _) => completion.SetResult(true);
            await completion.Task;
        }
    }
}
