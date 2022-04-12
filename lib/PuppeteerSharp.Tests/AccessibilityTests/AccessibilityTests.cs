using System.Threading.Tasks;
using PuppeteerSharp.PageAccessibility;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.AccesibilityTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class AccesibilityTests : PuppeteerPageBaseTest
    {
        public AccesibilityTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("accessibility.spec.ts", "Accessibility", "should work")]
        [SkipBrowserFact(skipFirefox: true)]
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
                        new SerializedAXNode
                        {
                            Role = "StaticText",
                            Name = "Hello World"
                        },
                        new SerializedAXNode
                        {
                            Role = "heading",
                            Name = "Inputs",
                            Level = 1
                        },
                        new SerializedAXNode{
                            Role = "textbox",
                            Name = "Empty input",
                            Focused = true
                        },
                        new SerializedAXNode{
                            Role = "textbox",
                            Name = "readonly input",
                            Readonly = true
                        },
                        new SerializedAXNode{
                            Role = "textbox",
                            Name = "disabled input",
                            Disabled= true
                        },
                        new SerializedAXNode{
                            Role = "textbox",
                            Name = "Input with whitespace",
                            Value= "  "
                        },
                        new SerializedAXNode{
                            Role = "textbox",
                            Name = "",
                            Value= "value only"
                        },
                        new SerializedAXNode{
                            Role = "textbox",
                            Name = "placeholder",
                            Value= "and a value"
                        },
                        new SerializedAXNode{
                            Role = "textbox",
                            Name = "placeholder",
                            Value= "and a value",
                            Description= "This is a description!"},
                        new SerializedAXNode{
                            Role= "combobox",
                            Name= "",
                            Value= "First Option",
                            HasPopup = "menu",
                            Children= new SerializedAXNode[]{
                                new SerializedAXNode
                                {
                                    Role = "menuitem",
                                    Name = "First Option",
                                    Selected= true
                                },
                                new SerializedAXNode
                                {
                                    Role = "menuitem",
                                    Name = "Second Option"
                                }
                            }
                        }
                    }
            };
            await Page.FocusAsync("[placeholder='Empty input']");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.Equal(nodeToCheck, snapshot);
        }

        [PuppeteerTest("accessibility.spec.ts", "Accessibility", "should report uninteresting nodes")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldReportUninterestingNodes()
        {
            await Page.SetContentAsync("<textarea autofocus>hi</textarea>");
            await Page.FocusAsync("textarea");

            Assert.Equal(
                new SerializedAXNode
                {
                    Role = "textbox",
                    Name = "",
                    Value = "hi",
                    Focused = true,
                    Multiline = true,
                    Children = new SerializedAXNode[]
                    {
                        new SerializedAXNode
                        {
                            Role = "generic",
                            Name = "",
                            Children = new SerializedAXNode[]
                            {
                                new SerializedAXNode
                                {
                                    Role = "StaticText",
                                    Name = "hi"
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

        [PuppeteerTest("accessibility.spec.ts", "Accessibility", "roledescription")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task RoleDescription()
        {
            await Page.SetContentAsync("<div tabIndex=-1 aria-roledescription='foo'>Hi</div>");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            // See https://chromium-review.googlesource.com/c/chromium/src/+/3088862
            Assert.Null(snapshot.Children[0].RoleDescription);
        }

        [PuppeteerTest("accessibility.spec.ts", "Accessibility", "orientation")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task Orientation()
        {
            await Page.SetContentAsync("<a href='' role='slider' aria-orientation='vertical'>11</a>");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.Equal("vertical", snapshot.Children[0].Orientation);
        }

        [PuppeteerTest("accessibility.spec.ts", "Accessibility", "autocomplete")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task AutoComplete()
        {
            await Page.SetContentAsync("<input type='number' aria-autocomplete='list' />");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.Equal("list", snapshot.Children[0].AutoComplete);
        }

        [PuppeteerTest("accessibility.spec.ts", "Accessibility", "multiselectable")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task MultiSelectable()
        {
            await Page.SetContentAsync("<div role='grid' tabIndex=-1 aria-multiselectable=true>hey</div>");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.True(snapshot.Children[0].Multiselectable);
        }

        [PuppeteerTest("accessibility.spec.ts", "Accessibility", "keyshortcuts")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task KeyShortcuts()
        {
            await Page.SetContentAsync("<div role='grid' tabIndex=-1 aria-keyshortcuts='foo'>hey</div>");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.Equal("foo", snapshot.Children[0].KeyShortcuts);
        }

        [PuppeteerTest("accessibility.spec.ts", "filtering children of leaf nodes", "should not report text nodes inside controls")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldNotReportTextNodesInsideControls()
        {
            await Page.SetContentAsync(@"
            <div role='tablist'>
                <div role='tab' aria-selected='true'><b>Tab1</b></div>
                <div role='tab'>Tab2</div>
            </div>");
            Assert.Equal(
                new SerializedAXNode
                {
                    Role = "RootWebArea",
                    Name = "",
                    Children = new SerializedAXNode[]
                    {
                        new SerializedAXNode
                        {
                            Role = "tab",
                            Name = "Tab1",
                            Selected = true
                        },
                        new SerializedAXNode
                        {
                            Role = "tab",
                            Name = "Tab2"
                        }
                    }
                },
                await Page.Accessibility.SnapshotAsync());
        }

        [PuppeteerTest("accessibility.spec.ts", "filtering children of leaf nodes", "rich text editable fields should have children")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task RichTextEditableFieldsShouldHaveChildren()
        {
            await Page.SetContentAsync(@"
            <div contenteditable='true'>
                Edit this image: <img src='fakeimage.png' alt='my fake image'>
            </div>");
            Assert.Equal(
                new SerializedAXNode
                {
                    Role = "generic",
                    Name = "",
                    Value = "Edit this image: ",
                    Children = new SerializedAXNode[]
                    {
                        new SerializedAXNode
                        {
                            Role = "StaticText",
                            Name = "Edit this image:"
                        },
                        new SerializedAXNode
                        {
                            Role = "img",
                            Name = "my fake image"
                        }
                    }
                },
                (await Page.Accessibility.SnapshotAsync()).Children[0]);
        }

        [PuppeteerTest("accessibility.spec.ts", "filtering children of leaf nodes", "rich text editable fields with role should have children")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task RichTextEditableFieldsWithRoleShouldHaveChildren()
        {
            await Page.SetContentAsync(@"
            <div contenteditable='true' role='textbox'>
                Edit this image: <img src='fakeimage.png' alt='my fake image'>
            </div>");
            Assert.Equal(
                new SerializedAXNode
                {
                    Role = "textbox",
                    Name = "",
                    Value = "Edit this image: ",
                    Multiline = true,
                    Children = new SerializedAXNode[]
                    {
                        new SerializedAXNode
                        {
                            Role = "StaticText",
                            Name = "Edit this image:"
                        },
                        new SerializedAXNode
                        {
                            Role = "img",
                            Name = "my fake image"
                        }
                    }
                },
                (await Page.Accessibility.SnapshotAsync()).Children[0]);
        }

        [PuppeteerTest("accessibility.spec.ts", "plaintext contenteditable", "plain text field with role should not have children")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task PlainTextFieldWithRoleShouldNotHaveChildren()
        {
            await Page.SetContentAsync("<div contenteditable='plaintext-only' role='textbox'>Edit this image:<img src='fakeimage.png' alt='my fake image'></div>");
            Assert.Equal(
                new SerializedAXNode
                {
                    Role = "textbox",
                    Name = "",
                    Value = "Edit this image:",
                    Multiline = true,
                },
                (await Page.Accessibility.SnapshotAsync()).Children[0]);
        }

        [PuppeteerTest("accessibility.spec.ts", "plaintext contenteditable", "plain text field with tabindex and without role should not have content")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task PlainTextFieldWithoutRoleShouldNotHaveContent()
        {
            await Page.SetContentAsync(
                "<div contenteditable='plaintext-only'>Edit this image:<img src='fakeimage.png' alt='my fake image'></div>");
            var snapshot = await Page.Accessibility.SnapshotAsync();
            Assert.Equal("generic", snapshot.Children[0].Role);
            Assert.Equal(string.Empty, snapshot.Children[0].Name);
        }

        [PuppeteerTest("accessibility.spec.ts", "filtering children of leaf nodes", "non editable textbox with role and tabIndex and label should not have children")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task NonEditableTextboxWithRoleAndTabIndexAndLabelShouldNotHaveChildren()
        {
            await Page.SetContentAsync(@"
            <div role='textbox' tabIndex=0 aria-checked='true' aria-label='my favorite textbox'>
                this is the inner content
                <img alt='yo' src='fakeimg.png'>
            </div>");
            Assert.Equal(
                new SerializedAXNode
                {
                    Role = "textbox",
                    Name = "my favorite textbox",
                    Value = "this is the inner content "
                },
                (await Page.Accessibility.SnapshotAsync()).Children[0]);
        }

        [PuppeteerTest("accessibility.spec.ts", "filtering children of leaf nodes", "checkbox with and tabIndex and label should not have children")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task CheckboxWithAndTabIndexAndLabelShouldNotHaveChildren()
        {
            await Page.SetContentAsync(@"
            <div role='checkbox' tabIndex=0 aria-checked='true' aria-label='my favorite checkbox'>
                this is the inner content
                <img alt='yo' src='fakeimg.png'>
            </div>");
            Assert.Equal(
                new SerializedAXNode
                {
                    Role = "checkbox",
                    Name = "my favorite checkbox",
                    Checked = CheckedState.True
                },
                (await Page.Accessibility.SnapshotAsync()).Children[0]);
        }

        [PuppeteerTest("accessibility.spec.ts", "filtering children of leaf nodes", "checkbox without label should not have children")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task CheckboxWithoutLabelShouldNotHaveChildren()
        {
            await Page.SetContentAsync(@"
            <div role='checkbox' aria-checked='true'>
                this is the inner content
                <img alt='yo' src='fakeimg.png'>
            </div>");
            Assert.Equal(
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
