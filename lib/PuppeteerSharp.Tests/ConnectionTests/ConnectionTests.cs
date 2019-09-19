using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ConnectionsTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ConnectionTests : PuppeteerPageBaseTest
    {
        public ConnectionTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldCleanCallbackList()
        {
            await Browser.GetVersionAsync();
            await Browser.GetVersionAsync();
            Assert.False(Browser.Connection.HasPendingCallbacks());

            await Page.SetJavaScriptEnabledAsync(false);
            await Page.SetJavaScriptEnabledAsync(true);
            Assert.False(Page.Client.HasPendingCallbacks());
        }
    }
}
