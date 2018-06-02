using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class CloseTests : PuppeteerBrowserBaseTest
    {
        public CloseTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task ShouldRejectAllPromisesWhenPageIsClosed()
        {
            var newPage = await Browser.NewPageAsync();
            var neverResolves = newPage.EvaluateFunctionAsync("() => new Promise(r => {})");

            // Put into a var to avoid warning
            var t = newPage.CloseAsync();

            var exception = await Assert.ThrowsAsync<TargetClosedException>(async () => await neverResolves);

            Assert.Contains("Protocol error", exception.Message);
        }
    }
}
