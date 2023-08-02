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
        public ResponseBufferTests(): base()
        {
        }

        [PuppeteerTest("network.spec.ts", "Response.buffer", "should work")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            var imageBuffer = File.ReadAllBytes("./Assets/pptr.png");
            Assert.Equal(imageBuffer, await response.BufferAsync());
        }

        [PuppeteerTest("network.spec.ts", "Response.buffer", "should work with compression")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithCompression()
        {
            Server.EnableGzip("/pptr.png");
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            var imageBuffer = File.ReadAllBytes("./Assets/pptr.png");
            Assert.Equal(imageBuffer, await response.BufferAsync());
        }
    }
}
