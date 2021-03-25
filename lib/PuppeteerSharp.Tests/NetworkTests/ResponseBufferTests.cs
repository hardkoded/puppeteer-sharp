using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
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

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            var imageBuffer = File.ReadAllBytes("./Assets/pptr.png");
            Assert.Equal(imageBuffer, await response.BufferAsync());
        }

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
