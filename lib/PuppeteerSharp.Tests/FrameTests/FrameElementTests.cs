using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.FrameTests
{
    public class FrameElementTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("frame.spec", "Frame specs Frame.prototype.frameElement", "should work")]
        public async Task ShouldWork()
        {
            await FrameUtils.AttachFrameAsync(Page, "theFrameId", TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(@"(url) => {
                const frame = document.createElement('iframe');
                frame.name = 'theFrameName';
                frame.src = url;
                document.body.appendChild(frame);
                return new Promise(x => frame.onload = x);
            }", TestConstants.EmptyPage);

            await using var mainFrameElement = await Page.MainFrame.FrameElementAsync();
            Assert.That(mainFrameElement, Is.Null);

            var childFrames = Page.Frames.Where(f => f != Page.MainFrame).ToArray();
            Assert.That(childFrames, Has.Length.EqualTo(2));

            await using var frame1 = await childFrames[0].FrameElementAsync();
            Assert.That(frame1, Is.Not.Null);

            await using var frame2 = await childFrames[1].FrameElementAsync();
            Assert.That(frame2, Is.Not.Null);

            var name1 = await frame1.EvaluateFunctionAsync<string>("frame => frame.id");
            var name2 = await frame2.EvaluateFunctionAsync<string>("frame => frame.name");

            // The order of child frames may vary, so check both possibilities
            Assert.That(
                new[] { name1, name2 },
                Is.EquivalentTo(new[] { "theFrameId", "theFrameName" }));
        }

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
            var frame = Page.MainFrame.ChildFrames.First();
            await using var frameElement = await frame.FrameElementAsync();
            Assert.That(frameElement, Is.Not.Null);
            Assert.That(
                await frameElement.EvaluateFunctionAsync<string>("el => el.tagName.toLocaleLowerCase()"),
                Is.EqualTo("iframe"));
        }

        [Test, PuppeteerTest("frame.spec", "Frame specs Frame.prototype.frameElement", "should return ElementHandle in the correct world")]
        public async Task ShouldReturnElementHandleInTheCorrectWorld()
        {
            await FrameUtils.AttachFrameAsync(Page, "theFrameId", TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(@"() => {
                globalThis.isMainWorld = true;
            }");

            Assert.That(Page.Frames, Has.Length.EqualTo(2));

            var childFrame = Page.MainFrame.ChildFrames.First();
            await using var frameElement = await childFrame.FrameElementAsync();
            Assert.That(frameElement, Is.Not.Null);

            var isMainWorld = await frameElement.EvaluateFunctionAsync<bool>("() => globalThis.isMainWorld");
            Assert.That(isMainWorld, Is.True);
        }
    }
}
