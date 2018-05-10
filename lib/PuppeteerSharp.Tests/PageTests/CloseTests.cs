using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class CloseTests : PuppeteerBaseTest
    {
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
