using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseBufferTests : PuppeteerPageBaseTest
    {
        public ResponseBufferTests() : base()
        {
        }

        [Test, PuppeteerTimeout, Retry(2), PuppeteerTest("network.spec", "network Response.buffer", "should work")]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            var imageBuffer = File.ReadAllBytes("./Assets/pptr.png");
            Assert.AreEqual(imageBuffer, await response.BufferAsync());
        }

        [Test, PuppeteerTimeout, Retry(2), PuppeteerTest("network.spec", "network Response.buffer", "should work with compression")]
        public async Task ShouldWorkWithCompression()
        {
            Server.EnableGzip("/pptr.png");
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            var imageBuffer = File.ReadAllBytes("./Assets/pptr.png");
            Assert.AreEqual(imageBuffer, await response.BufferAsync());
        }
    }
}
