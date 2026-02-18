using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.FrameTests
{
    public class FrameElementTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("frame.spec", "Frame specs Frame.prototype.frameElement", "should handle shadow roots")]
        public async Task ShouldHandleShadowRoots()
        {
            await Page.SetContentAsync(@"
                <div id=""shadow-host""></div>
                <script>
                    const host = document.getElementById('shadow-host');
                    const shadowRoot = host.attachShadow({ mode: 'closed' });
                    const frame = document.createElement('iframe');
                    frame.srcdoc = '<p>Inside frame</p>';
                    shadowRoot.appendChild(frame);
                </script>
            ");
            // Wait for the iframe to load inside shadow DOM
            await Page.WaitForFrameAsync(f => f != Page.MainFrame);
            Assert.That(Page.Frames, Has.Length.EqualTo(2));
            var frame = Page.Frames[1];
            await using var frameElement = await frame.FrameElementAsync();
            Assert.That(frameElement, Is.Not.Null);
            Assert.That(
                await frameElement.EvaluateFunctionAsync<string>("el => el.tagName.toLocaleLowerCase()"),
                Is.EqualTo("iframe"));
        }
    }
}
