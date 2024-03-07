using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.PageAccessibility;

namespace PuppeteerSharp.Tests.AccessibilityTests
{
    public class AccessibilityTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("accessibility.spec", "Accessibility", "should work")]
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
                            Children= new SerializedAXNode[]{
                                new() {
                                    Role = "menuitem",
                                    Name = "First Option",
                                    Selected= true
                                },
                                new() {
                                    Role = "menuitem",
                                    Name = "Second Option"
                                }
                            }
                        }
                    }
            };
            await Page.FocusAsync("[placeholder='Empty input']");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.AreEqual(nodeToCheck, snapshot);
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "Accessibility", "should report uninteresting nodes")]
        public async Task ShouldReportUninterestingNodes()
        {
            await Page.SetContentAsync("<textarea autofocus>hi</textarea>");
            await Page.FocusAsync("textarea");

            // This object has more children than in upstream.
            // Because upstream uses `toMatchObject` which stops going deeper if the element has not Children.
            Assert.AreEqual(
                new SerializedAXNode
                {
                    Role = "textbox",
                    Name = "",
                    Value = "hi",
                    Focused = true,
                    Multiline = true,
                    Children = new SerializedAXNode[]
                    {
                        new() {
                            Role = "generic",
                            Name = "",
                            Children = new SerializedAXNode[]
                            {
                                new() {
                                    Role = "StaticText",
                                    Name = "hi",
                                    Children = new SerializedAXNode[]
                                    {
                                        new()
                                        {
                                            Role = "InlineTextBox",
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                FindFocusedNode(await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions
                {
                    InterestingOnly = false
                })));
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "Accessibility", "get snapshots while the tree is re-calculated")]
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
            Assert.AreEqual("Show", await GetAccessibleNameAsync(Page, button));
            await button?.ClickAsync();
            await Page.WaitForSelectorAsync("aria/Hide");
        }

        private async Task<string> GetAccessibleNameAsync(IPage page, IElementHandle element)
        {
            return (await page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions
            {
                Root = element
            })).Name;
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "Accessibility", "roledescription")]
        public async Task RoleDescription()
        {
            await Page.SetContentAsync("<div tabIndex=-1 aria-roledescription='foo'>Hi</div>");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            // See https://chromium-review.googlesource.com/c/chromium/src/+/3088862
            Assert.Null(snapshot.Children[0].RoleDescription);
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "Accessibility", "orientation")]
        public async Task Orientation()
        {
            await Page.SetContentAsync("<a href='' role='slider' aria-orientation='vertical'>11</a>");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.AreEqual("vertical", snapshot.Children[0].Orientation);
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "Accessibility", "autocomplete")]
        public async Task AutoComplete()
        {
            await Page.SetContentAsync("<input type='number' aria-autocomplete='list' />");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.AreEqual("list", snapshot.Children[0].AutoComplete);
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "Accessibility", "multiselectable")]
        public async Task MultiSelectable()
        {
            await Page.SetContentAsync("<div role='grid' tabIndex=-1 aria-multiselectable=true>hey</div>");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.True(snapshot.Children[0].Multiselectable);
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "Accessibility", "keyshortcuts")]
        public async Task KeyShortcuts()
        {
            await Page.SetContentAsync("<div role='grid' tabIndex=-1 aria-keyshortcuts='foo'>hey</div>");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.AreEqual("foo", snapshot.Children[0].KeyShortcuts);
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "filtering children of leaf nodes", "should not report text nodes inside controls")]
        public async Task ShouldNotReportTextNodesInsideControls()
        {
            await Page.SetContentAsync(@"
            <div role='tablist'>
                <div role='tab' aria-selected='true'><b>Tab1</b></div>
                <div role='tab'>Tab2</div>
            </div>");
            Assert.AreEqual(
                new SerializedAXNode
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
                },
                await Page.Accessibility.SnapshotAsync());
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "filtering children of leaf nodes", "rich text editable fields should have children")]
        public async Task RichTextEditableFieldsShouldHaveChildren()
        {
            await Page.SetContentAsync(@"
            <div contenteditable='true'>
                Edit this image: <img src='fakeimage.png' alt='my fake image'>
            </div>");
            Assert.AreEqual(
                new SerializedAXNode
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
                },
                (await Page.Accessibility.SnapshotAsync()).Children[0]);
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "filtering children of leaf nodes", "rich text editable fields with role should have children")]
        public async Task RichTextEditableFieldsWithRoleShouldHaveChildren()
        {
            await Page.SetContentAsync(@"
            <div contenteditable='true' role='textbox'>
                Edit this image: <img src='fakeimage.png' alt='my fake image'>
            </div>");
            Assert.AreEqual(
                new SerializedAXNode
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
                },
                (await Page.Accessibility.SnapshotAsync()).Children[0]);
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "plaintext contenteditable", "plain text field with role should not have children")]
        public async Task PlainTextFieldWithRoleShouldNotHaveChildren()
        {
            await Page.SetContentAsync("<div contenteditable='plaintext-only' role='textbox'>Edit this image:<img src='fakeimage.png' alt='my fake image'></div>");
            Assert.AreEqual(
                new SerializedAXNode
                {
                    Role = "textbox",
                    Name = "",
                    Value = "Edit this image:",
                    Multiline = true,
                },
                (await Page.Accessibility.SnapshotAsync()).Children[0]);
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "plaintext contenteditable", "plain text field with tabindex and without role should not have content")]
        public async Task PlainTextFieldWithoutRoleShouldNotHaveContent()
        {
            await Page.SetContentAsync(
                "<div contenteditable='plaintext-only'>Edit this image:<img src='fakeimage.png' alt='my fake image'></div>");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.AreEqual("generic", snapshot.Children[0].Role);
            Assert.AreEqual(string.Empty, snapshot.Children[0].Name);
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "filtering children of leaf nodes", "non editable textbox with role and tabIndex and label should not have children")]
        public async Task NonEditableTextboxWithRoleAndTabIndexAndLabelShouldNotHaveChildren()
        {
            await Page.SetContentAsync(@"
            <div role='textbox' tabIndex=0 aria-checked='true' aria-label='my favorite textbox'>
                this is the inner content
                <img alt='yo' src='fakeimg.png'>
            </div>");
            Assert.AreEqual(
                new SerializedAXNode
                {
                    Role = "textbox",
                    Name = "my favorite textbox",
                    Value = "this is the inner content "
                },
                (await Page.Accessibility.SnapshotAsync()).Children[0]);
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "filtering children of leaf nodes", "checkbox with and tabIndex and label should not have children")]
        public async Task CheckboxWithAndTabIndexAndLabelShouldNotHaveChildren()
        {
            await Page.SetContentAsync(@"
            <div role='checkbox' tabIndex=0 aria-checked='true' aria-label='my favorite checkbox'>
                this is the inner content
                <img alt='yo' src='fakeimg.png'>
            </div>");
            Assert.AreEqual(
                new SerializedAXNode
                {
                    Role = "checkbox",
                    Name = "my favorite checkbox",
                    Checked = CheckedState.True
                },
                (await Page.Accessibility.SnapshotAsync()).Children[0]);
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "filtering children of leaf nodes", "checkbox without label should not have children")]
        public async Task CheckboxWithoutLabelShouldNotHaveChildren()
        {
            await Page.SetContentAsync(@"
            <div role='checkbox' aria-checked='true'>
                this is the inner content
                <img alt='yo' src='fakeimg.png'>
            </div>");
            Assert.AreEqual(
                new SerializedAXNode
                {
                    Role = "checkbox",
                    Name = "this is the inner content yo",
                    Checked = CheckedState.True
                },
                (await Page.Accessibility.SnapshotAsync()).Children[0]);
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
