using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseBufferTests : PuppeteerPageBaseTest
    {
        public ResponseBufferTests(): base()
        {
        }

        [PuppeteerTest("network.spec.ts", "Response.buffer", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            var imageBuffer = File.ReadAllBytes("./Assets/pptr.png");
            Assert.AreEqual(imageBuffer, await response.BufferAsync());
        }

        [PuppeteerTest("network.spec.ts", "Response.buffer", "should work with compression")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWorkWithCompression()
        {
            Server.EnableGzip("/pptr.png");
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            var imageBuffer = File.ReadAllBytes("./Assets/pptr.png");
            Assert.AreEqual(imageBuffer, await response.BufferAsync());
        }
    }
}
