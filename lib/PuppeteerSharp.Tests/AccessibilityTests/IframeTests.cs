using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.PageAccessibility;

namespace PuppeteerSharp.Tests.AccessibilityTests
{
    public class IframeTests : PuppeteerPageBaseTest
    {
        [
            Test,
            PuppeteerTest(
                "accessibility.spec",
                "Accessibility iframes",
                "should not include iframe data if not requested"
            )
        ]
        public async Task ShouldNotIncludeIframeDataIfNotRequested()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var frame1 = Page.Frames[1];
            await frame1.EvaluateFunctionAsync(
                @"() => {
                const button = document.createElement('button');
                button.innerText = 'value1';
                document.body.appendChild(button);
            }"
            );
            var snapshot = await Page.Accessibility.SnapshotAsync(
                new AccessibilitySnapshotOptions { InterestingOnly = true }
            );
            Assert.That(snapshot.Role, Is.EqualTo("RootWebArea"));
            Assert.That(snapshot.Name, Is.EqualTo(string.Empty));
        }

        [
            Test,
            PuppeteerTest(
                "accessibility.spec",
                "Accessibility iframes",
                "same-origin iframe (interesting only)"
            )
        ]
        public async Task SameOriginIframeInterestingOnly()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var frame1 = Page.Frames[1];
            await frame1.EvaluateFunctionAsync(
                @"() => {
                const button = document.createElement('button');
                button.innerText = 'value1';
                document.body.appendChild(button);
            }"
            );
            var snapshot = await Page.Accessibility.SnapshotAsync(
                new AccessibilitySnapshotOptions { InterestingOnly = true, IncludeIframes = true }
            );
            Assert.That(snapshot.Role, Is.EqualTo("RootWebArea"));
            Assert.That(snapshot.Name, Is.EqualTo(string.Empty));
            Assert.That(snapshot.Children, Is.Not.Null);
            Assert.That(snapshot.Children.Length, Is.GreaterThanOrEqualTo(1));

            var iframeNode = FindNodeByRole(snapshot, "Iframe");
            Assert.That(iframeNode, Is.Not.Null, "Expected an Iframe node in the snapshot");
            Assert.That(iframeNode.Children, Is.Not.Null);
            Assert.That(iframeNode.Children.Length, Is.GreaterThanOrEqualTo(1));

            var rootWebArea = iframeNode.Children[0];
            Assert.That(rootWebArea.Role, Is.EqualTo("RootWebArea"));
            Assert.That(rootWebArea.Children, Is.Not.Null);

            var buttonNode = FindNodeByRoleAndName(rootWebArea, "button", "value1");
            Assert.That(buttonNode, Is.Not.Null, "Expected a button node inside the iframe");
        }

        [
            Test,
            PuppeteerTest(
                "accessibility.spec",
                "Accessibility iframes",
                "cross-origin iframe (interesting only)"
            )
        ]
        public async Task CrossOriginIframeInterestingOnly()
        {
            await FrameUtils.AttachFrameAsync(
                Page,
                "frame1",
                TestConstants.CrossProcessUrl + "/empty.html"
            );
            var frame1 = Page.Frames[1];
            await frame1.EvaluateFunctionAsync(
                @"() => {
                const button = document.createElement('button');
                button.innerText = 'value1';
                document.body.appendChild(button);
            }"
            );
            var snapshot = await Page.Accessibility.SnapshotAsync(
                new AccessibilitySnapshotOptions { InterestingOnly = true, IncludeIframes = true }
            );
            Assert.That(snapshot.Role, Is.EqualTo("RootWebArea"));
            Assert.That(snapshot.Name, Is.EqualTo(string.Empty));
            Assert.That(snapshot.Children, Is.Not.Null);

            var iframeNode = FindNodeByRole(snapshot, "Iframe");
            Assert.That(iframeNode, Is.Not.Null, "Expected an Iframe node in the snapshot");
            Assert.That(iframeNode.Children, Is.Not.Null);

            var rootWebArea = iframeNode.Children[0];
            Assert.That(rootWebArea.Role, Is.EqualTo("RootWebArea"));

            var buttonNode = FindNodeByRoleAndName(rootWebArea, "button", "value1");
            Assert.That(buttonNode, Is.Not.Null, "Expected a button node inside the iframe");
        }

        [
            Test,
            PuppeteerTest(
                "accessibility.spec",
                "Accessibility iframes",
                "same-origin iframe (all nodes)"
            )
        ]
        public async Task SameOriginIframeAllNodes()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var frame1 = Page.Frames[1];
            await frame1.EvaluateFunctionAsync(
                @"() => {
                const button = document.createElement('button');
                button.innerText = 'value1';
                document.body.appendChild(button);
            }"
            );
            var snapshot = await Page.Accessibility.SnapshotAsync(
                new AccessibilitySnapshotOptions { InterestingOnly = false, IncludeIframes = true }
            );
            Assert.That(snapshot.Role, Is.EqualTo("RootWebArea"));
            Assert.That(snapshot.Name, Is.EqualTo(string.Empty));

            var iframeNode = FindNodeByRole(snapshot, "Iframe");
            Assert.That(iframeNode, Is.Not.Null, "Expected an Iframe node in the snapshot");
            Assert.That(iframeNode.Children, Is.Not.Null);

            var rootWebArea = FindNodeByRole(iframeNode, "RootWebArea");
            Assert.That(rootWebArea, Is.Not.Null);

            var buttonNode = FindNodeByRoleAndName(rootWebArea, "button", "value1");
            Assert.That(buttonNode, Is.Not.Null, "Expected a button node inside the iframe");
        }

        [
            Test,
            PuppeteerTest(
                "accessibility.spec",
                "Accessibility iframes",
                "same-origin iframe with root option"
            )
        ]
        public async Task SameOriginIframeWithRootOption()
        {
            await Page.SetContentAsync(
                $@"
                <section aria-label=""Container"">
                  <iframe src=""{TestConstants.EmptyPage}""></iframe>
                </section>
            "
            );
            var frame1 = Page.Frames[1];
            await frame1.EvaluateFunctionAsync(
                @"() => {
                const button = document.createElement('button');
                button.innerText = 'value1';
                document.body.appendChild(button);
            }"
            );
            await using var container = await Page.QuerySelectorAsync("section");
            var containerSnapshot = await Page.Accessibility.SnapshotAsync(
                new AccessibilitySnapshotOptions
                {
                    Root = container,
                    InterestingOnly = true,
                    IncludeIframes = true,
                }
            );
            Assert.That(containerSnapshot, Is.Not.Null);
            Assert.That(containerSnapshot.Role, Is.EqualTo("region"));
            Assert.That(containerSnapshot.Name, Is.EqualTo("Container"));
            Assert.That(containerSnapshot.Children, Has.Length.EqualTo(1));
            AssertIframeWithButton(containerSnapshot.Children[0]);

            await using var iframeElement = await Page.QuerySelectorAsync("iframe");
            var iframeSnapshot = await Page.Accessibility.SnapshotAsync(
                new AccessibilitySnapshotOptions
                {
                    Root = iframeElement,
                    InterestingOnly = true,
                    IncludeIframes = true,
                }
            );
            Assert.That(iframeSnapshot, Is.Not.Null);
            AssertIframeWithButton(iframeSnapshot);
        }

        [
            Test,
            PuppeteerTest(
                "accessibility.spec",
                "Accessibility iframes",
                "cross-origin iframe with root option"
            )
        ]
        public async Task CrossOriginIframeWithRootOption()
        {
            await Page.SetContentAsync(
                $@"
                <section aria-label=""Container"">
                  <iframe src=""{TestConstants.CrossProcessUrl}/empty.html""></iframe>
                </section>
            "
            );
            var frame1 = Page.Frames[1];
            await frame1.EvaluateFunctionAsync(
                @"() => {
                const button = document.createElement('button');
                button.innerText = 'value1';
                document.body.appendChild(button);
            }"
            );
            await using var container = await Page.QuerySelectorAsync("section");
            var snapshot = await Page.Accessibility.SnapshotAsync(
                new AccessibilitySnapshotOptions
                {
                    Root = container,
                    InterestingOnly = true,
                    IncludeIframes = true,
                }
            );
            Assert.That(snapshot, Is.Not.Null);
            Assert.That(snapshot.Role, Is.EqualTo("region"));
            Assert.That(snapshot.Name, Is.EqualTo("Container"));
            Assert.That(snapshot.Children, Has.Length.EqualTo(1));
            AssertIframeWithButton(snapshot.Children[0]);
        }

        private static void AssertIframeWithButton(SerializedAXNode iframeNode)
        {
            Assert.That(iframeNode.Role, Is.EqualTo("Iframe"));
            Assert.That(iframeNode.Name, Is.EqualTo(string.Empty));
            Assert.That(iframeNode.Children, Has.Length.EqualTo(1));

            var rootWebArea = iframeNode.Children[0];
            Assert.That(rootWebArea.Role, Is.EqualTo("RootWebArea"));
            Assert.That(rootWebArea.Name, Is.EqualTo(string.Empty));
            Assert.That(rootWebArea.Children, Has.Length.EqualTo(1));
            Assert.That(rootWebArea.Children[0].Role, Is.EqualTo("button"));
            Assert.That(rootWebArea.Children[0].Name, Is.EqualTo("value1"));
        }

        private static SerializedAXNode FindNodeByRole(SerializedAXNode node, string role)
        {
            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    if (child.Role == role)
                    {
                        return child;
                    }

                    var result = FindNodeByRole(child, role);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private static SerializedAXNode FindNodeByRoleAndName(
            SerializedAXNode node,
            string role,
            string name
        )
        {
            if (node.Role == role && node.Name == name)
            {
                return node;
            }

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    var result = FindNodeByRoleAndName(child, role, name);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }
    }
}
