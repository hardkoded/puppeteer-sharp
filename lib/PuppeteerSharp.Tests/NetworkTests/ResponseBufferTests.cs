using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.NetworkTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ResponseBufferTests : PuppeteerPageBaseTest
    {
        public ResponseBufferTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            var imageBuffer = File.ReadAllBytes("./Assets/pptr.png");
            Assert.Equal(imageBuffer, await response.BufferAsync());
        }

        [Fact]
        public async Task ShouldWorkWithCompression()
        {
            Server.EnableGzip("/pptr.png");
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            var imageBuffer = File.ReadAllBytes("./Assets/pptr.png");
            Assert.Equal(imageBuffer, await response.BufferAsync());
        }
    }
}
