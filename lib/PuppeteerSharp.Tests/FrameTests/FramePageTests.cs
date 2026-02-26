using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.FrameTests
{
    public class FramePageTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("frame.spec", "Frame specs Frame.page", "should retrieve the page from a frame")]
        public async Task ShouldRetrieveThePageFromAFrame()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var mainFrame = Page.MainFrame;
            Assert.That(mainFrame.Page, Is.EqualTo(Page));
        }
    }
}
