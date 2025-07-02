using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseBufferTests : PuppeteerPageBaseTest
    {
        public ResponseBufferTests() : base()
        {
        }

        [Test, PuppeteerTest("network.spec", "network Response.buffer", "should work")]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            var imageBuffer = File.ReadAllBytes("./Assets/pptr.png");
            Assert.That(await response.BufferAsync(), Is.EqualTo(imageBuffer));
        }

        [Test, PuppeteerTest("network.spec", "network Response.buffer", "should work with compression")]
        public async Task ShouldWorkWithCompression()
        {
            Server.EnableGzip("/pptr.png");
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            var imageBuffer = File.ReadAllBytes("./Assets/pptr.png");
            Assert.That(await response.BufferAsync(), Is.EqualTo(imageBuffer));
        }
    }
}
