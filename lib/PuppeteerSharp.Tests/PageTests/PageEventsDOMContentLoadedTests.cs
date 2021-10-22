using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageEventsDOMContentLoadedTests : PuppeteerPageBaseTest
    {
        public PageEventsDOMContentLoadedTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.DOMContentLoaded", "should fire when expected")]
        [PuppeteerFact]
        public async Task ShouldFireWhenExpected()
        {
            var _ = Page.GoToAsync(TestConstants.AboutBlank);
            var completion = new TaskCompletionSource<bool>();
            Page.DOMContentLoaded += (_, _) => completion.SetResult(true);
            await completion.Task;
        }
    }
}
