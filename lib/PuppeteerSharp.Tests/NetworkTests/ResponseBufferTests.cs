using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
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

        [PuppeteerTest("network.spec.ts", "Response.buffer", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            var response = await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            var imageBuffer = File.ReadAllBytes("./Assets/pptr.png");
            Assert.Equal(imageBuffer, await response.BufferAsync());
        }

        [PuppeteerTest("network.spec.ts", "Response.buffer", "should work with compression")]
        [PuppeteerFact]
        public async Task ShouldWorkWithCompression()
        {
            Server.EnableGzip("/pptr.png");
            var response = await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            var imageBuffer = File.ReadAllBytes("./Assets/pptr.png");
            Assert.Equal(imageBuffer, await response.BufferAsync());
        }
    }
}
