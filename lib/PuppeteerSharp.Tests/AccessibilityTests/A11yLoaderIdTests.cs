using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.PageAccessibility;

namespace PuppeteerSharp.Tests.AccessibilityTests
{
    public class A11yLoaderIdTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("a11yLoaderId.spec", "Accessibility loaderId", "should match loaderId for iframes")]
        public async Task ShouldMatchLoaderIdForIframes()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var frame1 = Page.Frames[1];
            await frame1.EvaluateFunctionAsync(@"() => {
                const button = document.createElement('button');
                button.innerText = 'value1';
                document.body.appendChild(button);
            }");
            var snapshot = await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions
            {
                InterestingOnly = true,
                IncludeIframes = true,
            });

            var mainLoaderId = ((Frame)Page.MainFrame).LoaderId;
            var frame1LoaderId = ((Frame)frame1).LoaderId;

            Assert.That(mainLoaderId, Is.TypeOf<string>());
            Assert.That(mainLoaderId, Is.Not.Null.And.Not.Empty);
            Assert.That(frame1LoaderId, Is.TypeOf<string>());
            Assert.That(frame1LoaderId, Is.Not.Null.And.Not.Empty);

            // Root level
            Assert.That(snapshot.Role, Is.EqualTo("RootWebArea"));
            Assert.That(snapshot.Name, Is.EqualTo(string.Empty));
            Assert.That(snapshot.LoaderId, Is.EqualTo(mainLoaderId));

            // Iframe node
            Assert.That(snapshot.Children, Is.Not.Null);
            Assert.That(snapshot.Children.Length, Is.GreaterThanOrEqualTo(1));
            var iframeNode = snapshot.Children[0];
            Assert.That(iframeNode.Role, Is.EqualTo("Iframe"));
            Assert.That(iframeNode.Name, Is.EqualTo(string.Empty));
            Assert.That(iframeNode.LoaderId, Is.EqualTo(mainLoaderId));

            // RootWebArea inside iframe
            Assert.That(iframeNode.Children, Is.Not.Null);
            Assert.That(iframeNode.Children.Length, Is.GreaterThanOrEqualTo(1));
            var iframeRoot = iframeNode.Children[0];
            Assert.That(iframeRoot.Role, Is.EqualTo("RootWebArea"));
            Assert.That(iframeRoot.Name, Is.EqualTo(string.Empty));
            Assert.That(iframeRoot.LoaderId, Is.EqualTo(frame1LoaderId));

            // Button inside iframe
            Assert.That(iframeRoot.Children, Is.Not.Null);
            Assert.That(iframeRoot.Children.Length, Is.GreaterThanOrEqualTo(1));
            var buttonNode = iframeRoot.Children[0];
            Assert.That(buttonNode.Role, Is.EqualTo("button"));
            Assert.That(buttonNode.Name, Is.EqualTo("value1"));
            Assert.That(buttonNode.LoaderId, Is.EqualTo(frame1LoaderId));
        }
    }
}
