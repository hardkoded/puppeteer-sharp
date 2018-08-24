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

        private async Task TheSourceOfTheProblems()
        {
            await Page.Client.SendAsync("ThisCommand.DoesNotExist");
        }
    }
}
