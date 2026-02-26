using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.FrameTests
{
    public class FrameClientTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("frame.spec", "Frame specs Frame.client", "should return the client instance")]
        public void ShouldReturnTheClientInstance()
        {
            Assert.That(((Frame)Page.MainFrame).Client, Is.InstanceOf<ICDPSession>());
        }
    }
}
