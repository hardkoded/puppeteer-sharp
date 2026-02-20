using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.PageAccessibility;

namespace PuppeteerSharp.Tests.AccessibilityTests
{
    public class AccessibilityTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("accessibility.spec", "Accessibility", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync(@"
            <head>
                <title>Accessibility Test</title>
            </head>
            <body>
                <div>Hello World</div>
                <h1>Inputs</h1>
                <input placeholder='Empty input' autofocus />
                <input placeholder='readonly input' readonly />
                <input placeholder='disabled input' disabled />
                <input aria-label='Input with whitespace' value='  ' />
                <input value='value only' />
                <input aria-placeholder='placeholder' value='and a value' />
                <div aria-hidden='true' id='desc'>This is a description!</div>
                <input aria-placeholder='placeholder' value='and a value' aria-describedby='desc' />
                <select>
                    <option>First Option</option>
                    <option>Second Option</option>
                </select>
            </body>");

            var nodeToCheck = new SerializedAXNode
            {
                Role = "RootWebArea",
                Name = "Accessibility Test",
                Children = new SerializedAXNode[]
                    {
                        new() {
                            Role = "StaticText",
                            Name = "Hello World"
                        },
                        new() {
                            Role = "heading",
                            Name = "Inputs",
                            Level = 1
                        },
                        new (){
                            Role = "textbox",
                            Name = "Empty input",
                            Focused = true
                        },
                        new (){
                            Role = "textbox",
                            Name = "readonly input",
                            Readonly = true
                        },
                        new (){
                            Role = "textbox",
                            Name = "disabled input",
                            Disabled= true
                        },
                        new (){
                            Role = "textbox",
                            Name = "Input with whitespace",
                            Value= "  "
                        },
                        new (){
                            Role = "textbox",
                            Name = "",
                            Value= "value only"
                        },
                        new (){
                            Role = "textbox",
                            Name = "placeholder",
                            Value= "and a value"
                        },
                        new (){
                            Role = "textbox",
                            Name = "placeholder",
                            Value= "and a value",
                            Description= "This is a description!"
                        },
                        new (){
                            Role= "combobox",
                            Name= "",
                            Value= "First Option",
                            HasPopup = "menu",
                            Children=
                            [
                                new() {
                                    Role = "option",
                                    Name = "First Option",
                                    Selected= true
                                },
                                new() {
                                    Role = "option",
                                    Name = "Second Option"
                                }
                            ]
                        }
                    }
            };
            await Page.FocusAsync("[placeholder='Empty input']");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.That(snapshot, Is.EqualTo(nodeToCheck));
        }

        [Test, PuppeteerTest("accessibility.spec", "Accessibility", "should report uninteresting nodes")]
        public async Task ShouldReportUninterestingNodes()
        {
            await Page.SetContentAsync("<textarea autofocus>hi</textarea>");
            await Page.FocusAsync("textarea");

            // Upstream uses toMatchObject (partial matching), so we assert individual properties
            var focusedNode = FindFocusedNode(await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions
            {
                InterestingOnly = false
            }));
            Assert.That(focusedNode.Role, Is.EqualTo("textbox"));
            Assert.That(focusedNode.Name, Is.EqualTo(""));
            Assert.That(focusedNode.Value, Is.EqualTo("hi"));
            Assert.That(focusedNode.Focused, Is.True);
            Assert.That(focusedNode.Multiline, Is.True);
            Assert.That(focusedNode.Children, Has.Length.EqualTo(1));
            Assert.That(focusedNode.Children[0].Role, Is.EqualTo("generic"));
            Assert.That(focusedNode.Children[0].Name, Is.EqualTo(""));
            Assert.That(focusedNode.Children[0].Children, Has.Length.EqualTo(1));
            Assert.That(focusedNode.Children[0].Children[0].Role, Is.EqualTo("StaticText"));
            Assert.That(focusedNode.Children[0].Children[0].Name, Is.EqualTo("hi"));
        }

        [Test, PuppeteerTest("accessibility.spec", "Accessibility", "get snapshots while the tree is re-calculated")]
        public async Task GetSnapshotsWhileTheTreeIsReCalculated()
        {
            await Page.SetContentAsync(@"
            <!DOCTYPE html>
            <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Accessible name + aria-expanded puppeteer bug</title>
                <style>
                [aria-expanded=""false""] + * {
                    display: none;
                }
                </style>
            </head>
            <body>
                <button hidden>Show</button>
                <p>Some content</p>
                <script>
                const button = document.querySelector('button');
                button.removeAttribute('hidden')
                button.setAttribute('aria-expanded', 'false');
                button.addEventListener('click', function() {
                    button.setAttribute('aria-expanded', button.getAttribute('aria-expanded') !== 'true')
                    if (button.getAttribute('aria-expanded') == 'true') {
                    button.textContent = 'Hide'
                    } else {
                    button.textContent = 'Show'
                    }
                })
                </script>
            </body>
            </html>");

            var button = await Page.QuerySelectorAsync("button");
            Assert.That(await GetAccessibleNameAsync(Page, button), Is.EqualTo("Show"));
            await button!.ClickAsync();
            await Page.WaitForSelectorAsync("aria/Hide");
        }

        private async Task<string> GetAccessibleNameAsync(IPage page, IElementHandle element)
        {
            return (await page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions
            {
                Root = element
            })).Name;
        }

        [Test, PuppeteerTest("accessibility.spec", "Accessibility", "roledescription")]
        public async Task RoleDescription()
        {
            await Page.SetContentAsync("<div tabIndex=-1 aria-roledescription='foo'>Hi</div>");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            // See https://chromium-review.googlesource.com/c/chromium/src/+/3088862
            Assert.That(snapshot.Children[0].RoleDescription, Is.Null);
        }

        [Test, PuppeteerTest("accessibility.spec", "Accessibility", "orientation")]
        public async Task Orientation()
        {
            await Page.SetContentAsync("<a href='' role='slider' aria-orientation='vertical'>11</a>");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.That(snapshot.Children[0].Orientation, Is.EqualTo("vertical"));
        }

        [Test, PuppeteerTest("accessibility.spec", "Accessibility", "autocomplete")]
        public async Task AutoComplete()
        {
            await Page.SetContentAsync("<input type='number' aria-autocomplete='list' />");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.That(snapshot.Children[0].AutoComplete, Is.EqualTo("list"));
        }

        [Test, PuppeteerTest("accessibility.spec", "Accessibility", "multiselectable")]
        public async Task MultiSelectable()
        {
            await Page.SetContentAsync("<div role='grid' tabIndex=-1 aria-multiselectable=true>hey</div>");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.That(snapshot.Children[0].Multiselectable, Is.True);
        }

        [Test, PuppeteerTest("accessibility.spec", "Accessibility", "keyshortcuts")]
        public async Task KeyShortcuts()
        {
            await Page.SetContentAsync("<div role='grid' tabIndex=-1 aria-keyshortcuts='foo'>hey</div>");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.That(snapshot.Children[0].KeyShortcuts, Is.EqualTo("foo"));
        }

        [Test, PuppeteerTest("accessibility.spec", "filtering children of leaf nodes", "should not report text nodes inside controls")]
        public async Task ShouldNotReportTextNodesInsideControls()
        {
            await Page.SetContentAsync(@"
            <div role='tablist'>
                <div role='tab' aria-selected='true'><b>Tab1</b></div>
                <div role='tab'>Tab2</div>
            </div>");
            Assert.That(
                await Page.Accessibility.SnapshotAsync(),
                Is.EqualTo(new SerializedAXNode
                {
                    Role = "RootWebArea",
                    Name = "",
                    Children = new SerializedAXNode[]
                    {
                        new() {
                            Role = "tab",
                            Name = "Tab1",
                            Selected = true
                        },
                        new() {
                            Role = "tab",
                            Name = "Tab2"
                        }
                    }
                }));
        }

        [Test, PuppeteerTest("accessibility.spec", "filtering children of leaf nodes", "rich text editable fields should have children")]
        public async Task RichTextEditableFieldsShouldHaveChildren()
        {
            await Page.SetContentAsync(@"
            <div contenteditable='true'>
                Edit this image: <img src='fakeimage.png' alt='my fake image'>
            </div>");
            Assert.That(
                (await Page.Accessibility.SnapshotAsync()).Children[0],
                Is.EqualTo(new SerializedAXNode
                {
                    Role = "generic",
                    Name = "",
                    Value = "Edit this image: ",
                    Children = new SerializedAXNode[]
                    {
                        new() {
                            Role = "StaticText",
                            Name = "Edit this image: "
                        },
                        new() {
                            Role = "image",
                            Name = "my fake image"
                        }
                    }
                }));
        }

        [Test, PuppeteerTest("accessibility.spec", "filtering children of leaf nodes", "rich text editable fields with role should have children")]
        public async Task RichTextEditableFieldsWithRoleShouldHaveChildren()
        {
            await Page.SetContentAsync(@"
            <div contenteditable='true' role='textbox'>
                Edit this image: <img src='fakeimage.png' alt='my fake image'>
            </div>");
            Assert.That(
                (await Page.Accessibility.SnapshotAsync()).Children[0],
                Is.EqualTo(new SerializedAXNode
                {
                    Role = "textbox",
                    Name = "",
                    Value = "Edit this image: ",
                    Multiline = true,
                    Children = new SerializedAXNode[]
                    {
                        new() {
                            Role = "StaticText",
                            Name = "Edit this image: "
                        },
                    }
                }));
        }

        [Test, PuppeteerTest("accessibility.spec", "plaintext contenteditable", "plain text field with role should not have children")]
        public async Task PlainTextFieldWithRoleShouldNotHaveChildren()
        {
            await Page.SetContentAsync("<div contenteditable='plaintext-only' role='textbox'>Edit this image:<img src='fakeimage.png' alt='my fake image'></div>");
            Assert.That(
                (await Page.Accessibility.SnapshotAsync()).Children[0],
                Is.EqualTo(new SerializedAXNode
                {
                    Role = "textbox",
                    Name = "",
                    Value = "Edit this image:",
                    Multiline = true,
                }));
        }

        [Test, PuppeteerTest("accessibility.spec", "plaintext contenteditable", "plain text field with tabindex and without role should not have content")]
        public async Task PlainTextFieldWithoutRoleShouldNotHaveContent()
        {
            await Page.SetContentAsync(
                "<div contenteditable='plaintext-only'>Edit this image:<img src='fakeimage.png' alt='my fake image'></div>");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.That(snapshot.Children[0].Role, Is.EqualTo("generic"));
            Assert.That(snapshot.Children[0].Name, Is.EqualTo(string.Empty));
        }

        [Test, PuppeteerTest("accessibility.spec", "filtering children of leaf nodes", "non editable textbox with role and tabIndex and label should not have children")]
        public async Task NonEditableTextboxWithRoleAndTabIndexAndLabelShouldNotHaveChildren()
        {
            await Page.SetContentAsync(@"
            <div role='textbox' tabIndex=0 aria-checked='true' aria-label='my favorite textbox'>
                this is the inner content
                <img alt='yo' src='fakeimg.png'>
            </div>");
            Assert.That(
                (await Page.Accessibility.SnapshotAsync()).Children[0],
                Is.EqualTo(new SerializedAXNode
                {
                    Role = "textbox",
                    Name = "my favorite textbox",
                    Value = "this is the inner content "
                }));
        }

        [Test, PuppeteerTest("accessibility.spec", "filtering children of leaf nodes", "checkbox with and tabIndex and label should not have children")]
        public async Task CheckboxWithAndTabIndexAndLabelShouldNotHaveChildren()
        {
            await Page.SetContentAsync(@"
            <div role='checkbox' tabIndex=0 aria-checked='true' aria-label='my favorite checkbox'>
                this is the inner content
                <img alt='yo' src='fakeimg.png'>
            </div>");
            Assert.That(
                (await Page.Accessibility.SnapshotAsync()).Children[0],
                Is.EqualTo(new SerializedAXNode
                {
                    Role = "checkbox",
                    Name = "my favorite checkbox",
                    Checked = CheckedState.True
                }));
        }

        [Test, PuppeteerTest("accessibility.spec", "filtering children of leaf nodes", "checkbox without label should not have children")]
        public async Task CheckboxWithoutLabelShouldNotHaveChildren()
        {
            await Page.SetContentAsync(@"
            <div role='checkbox' aria-checked='true'>
                this is the inner content
                <img alt='yo' src='fakeimg.png'>
            </div>");
            Assert.That(
                (await Page.Accessibility.SnapshotAsync()).Children[0],
                Is.EqualTo(new SerializedAXNode
                {
                    Role = "checkbox",
                    Name = "this is the inner content yo",
                    Checked = CheckedState.True
                }));
        }

        [Test, PuppeteerTest("accessibility.spec", "Accessibility", "should not report Document as leaf node")]
        public async Task ShouldNotReportDocumentAsLeafNode()
        {
            await Page.SetContentAsync(@"
            <main><span>Hello</span><div> </div><div>World</div></main>");

            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.That(snapshot.Role, Is.EqualTo("RootWebArea"));
            Assert.That(snapshot.Children.Length, Is.EqualTo(2));
            Assert.That(snapshot.Children[0].Role, Is.EqualTo("StaticText"));
            Assert.That(snapshot.Children[0].Name, Is.EqualTo("Hello"));
            Assert.That(snapshot.Children[1].Role, Is.EqualTo("StaticText"));
            Assert.That(snapshot.Children[1].Name, Is.EqualTo("World"));
        }

        [Test, PuppeteerTest("accessibility.spec", "Accessibility", "should capture new accessibility properties and not prune them")]
        public async Task ShouldCaptureNewAccessibilityPropertiesAndNotPruneThem()
        {
            await Page.SetContentAsync(@"
                <div role=""alert"" aria-busy=""true"">This is an alert</div>
                <div aria-live=""polite"" aria-atomic=""true"" aria-relevant=""additions text"">
                  This is polite live region
                </div>
                <div aria-modal=""true"" role=""dialog"" aria-roledescription=""My Modal"">
                  Modal content
                </div>
                <div id=""error"">Error message</div>
                <input aria-invalid=""true"" aria-errormessage=""error"" value=""invalid input"">
                <div id=""details"">Additional details</div>
                <div aria-details=""details"">Element with details</div>
                <div aria-description=""This is a description""></div>");

            var snapshot = await Page.Accessibility.SnapshotAsync();

            Assert.That(snapshot.Role, Is.EqualTo("RootWebArea"));
            Assert.That(snapshot.Children.Length, Is.GreaterThanOrEqualTo(8));

            // alert node with busy, live, atomic
            var alertNode = snapshot.Children[0];
            Assert.That(alertNode.Role, Is.EqualTo("alert"));
            Assert.That(alertNode.Name, Is.EqualTo(string.Empty));
            Assert.That(alertNode.Busy, Is.True);
            Assert.That(alertNode.Live, Is.EqualTo("assertive"));
            Assert.That(alertNode.Atomic, Is.True);
            Assert.That(alertNode.Children, Has.Length.EqualTo(1));
            Assert.That(alertNode.Children[0].Role, Is.EqualTo("StaticText"));
            Assert.That(alertNode.Children[0].Name, Is.EqualTo("This is an alert"));

            // polite live region with atomic and relevant
            var liveRegionNode = snapshot.Children[1];
            Assert.That(liveRegionNode.Role, Is.EqualTo("generic"));
            Assert.That(liveRegionNode.Name, Is.EqualTo(string.Empty));
            Assert.That(liveRegionNode.Live, Is.EqualTo("polite"));
            Assert.That(liveRegionNode.Atomic, Is.True);
            Assert.That(liveRegionNode.Relevant, Is.EqualTo("additions text"));
            Assert.That(liveRegionNode.Children, Has.Length.EqualTo(1));
            Assert.That(liveRegionNode.Children[0].Role, Is.EqualTo("StaticText"));
            Assert.That(liveRegionNode.Children[0].Name, Is.EqualTo("This is polite live region"));

            // dialog with modal and roledescription
            var dialogNode = snapshot.Children[2];
            Assert.That(dialogNode.Role, Is.EqualTo("dialog"));
            Assert.That(dialogNode.Name, Is.EqualTo(string.Empty));
            Assert.That(dialogNode.Modal, Is.True);
            Assert.That(dialogNode.RoleDescription, Is.EqualTo("My Modal"));
            Assert.That(dialogNode.Children, Has.Length.EqualTo(1));
            Assert.That(dialogNode.Children[0].Role, Is.EqualTo("StaticText"));
            Assert.That(dialogNode.Children[0].Name, Is.EqualTo("Modal content"));

            // Error message static text
            Assert.That(snapshot.Children[3].Role, Is.EqualTo("StaticText"));
            Assert.That(snapshot.Children[3].Name, Is.EqualTo("Error message"));

            // input with invalid and errormessage
            var inputNode = snapshot.Children[4];
            Assert.That(inputNode.Role, Is.EqualTo("textbox"));
            Assert.That(inputNode.Value, Is.EqualTo("invalid input"));
            Assert.That(inputNode.Invalid, Is.EqualTo("true"));
            Assert.That(inputNode.Errormessage, Is.EqualTo("error"));

            // Additional details static text
            Assert.That(snapshot.Children[5].Role, Is.EqualTo("StaticText"));
            Assert.That(snapshot.Children[5].Name, Is.EqualTo("Additional details"));

            // element with details
            var detailsNode = snapshot.Children[6];
            Assert.That(detailsNode.Role, Is.EqualTo("generic"));
            Assert.That(detailsNode.Details, Is.EqualTo("details"));
            Assert.That(detailsNode.Children, Has.Length.EqualTo(1));
            Assert.That(detailsNode.Children[0].Role, Is.EqualTo("StaticText"));
            Assert.That(detailsNode.Children[0].Name, Is.EqualTo("Element with details"));

            // element with description only (no name)
            var descriptionNode = snapshot.Children[7];
            Assert.That(descriptionNode.Role, Is.EqualTo("generic"));
            Assert.That(descriptionNode.Description, Is.EqualTo("This is a description"));
        }

        private SerializedAXNode FindFocusedNode(SerializedAXNode serializedAXNode)
        {
            if (serializedAXNode.Focused)
            {
                return serializedAXNode;
            }
            foreach (var item in serializedAXNode.Children)
            {
                var focusedChild = FindFocusedNode(item);
                if (focusedChild != null)
                {
                    return focusedChild;
                }
            }

            return null;
        }
    }
}
