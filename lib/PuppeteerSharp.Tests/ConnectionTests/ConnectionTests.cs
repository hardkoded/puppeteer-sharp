using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ConnectionsTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ConnectionTests : PuppeteerPageBaseTest
    {
        public ConnectionTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldThrowNiceErrors()
        {
            var exception = await Assert.ThrowsAsync<MessageException>(async () =>
            {
                await TheSourceOfTheProblems();
            });
            Assert.Contains("TheSourceOfTheProblems", exception.StackTrace);
            Assert.Contains("ThisCommand.DoesNotExist", exception.Message);
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

        private async Task TheSourceOfTheProblems() => await Page.Client.SendAsync("ThisCommand.DoesNotExist");
    }
}
