using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests.Events
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DOMContentLoadedTests : PuppeteerPageBaseTest
    {
        public DOMContentLoadedTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldFireWhenExpected()
        {
            var _ = Page.GoToAsync(TestConstants.AboutBlank);
            var completion = new TaskCompletionSource<bool>();
            Page.DOMContentLoaded += (sender, e) => completion.SetResult(true);
            await completion.Task;
        }
    }
}
