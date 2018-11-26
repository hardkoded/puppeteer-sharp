using System.Threading.Tasks;
using PuppeteerSharp.PageAccessibility;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.AccesibilityTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class AccesibilityTests : PuppeteerPageBaseTest
    {
        public AccesibilityTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
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
            Assert.Equal(
                new SerializedAXNode
                {
                    Role = "WebArea",
                    Name = "Accessibility Test",
                    Children = new SerializedAXNode[]
                    {
                        new SerializedAXNode
                        {
                            Role = "text",
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
                },
                await Page.Accessibility.SnapshotAsync());
        }

        [Fact]
        public async Task ShouldReportUninterestingNodes()
        {
            await Page.SetContentAsync("<textarea autofocus>hi</textarea>");
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
                            Role = "GenericContainer",
                            Name = "",
                            Children = new SerializedAXNode[]
                            {
                                new SerializedAXNode
                                {
                                    Role = "text",
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

        [Fact]
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
                    Role = "WebArea",
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

        [Fact]
        public async Task RichTextEditableFieldsShouldHaveChildren()
        {
            await Page.SetContentAsync(@"
            <div contenteditable='true'>
                Edit this image: <img src='fakeimage.png' alt='my fake image'>
            </div>");
            Assert.Equal(
                new SerializedAXNode
                {
                    Role = "GenericContainer",
                    Name = "",
                    Value = "Edit this image: ",
                    Children = new SerializedAXNode[]
                    {
                        new SerializedAXNode
                        {
                            Role = "text",
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

        [Fact]
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
                    Children = new SerializedAXNode[]
                    {
                        new SerializedAXNode
                        {
                            Role = "text",
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

        [Fact]
        public async Task PlainTextFieldWithRoleShouldNotHaveChildren()
        {
            await Page.SetContentAsync("<div contenteditable='plaintext-only' role='textbox'>Edit this image:<img src='fakeimage.png' alt='my fake image'></div>");
            Assert.Equal(
                new SerializedAXNode
                {
                    Role = "textbox",
                    Name = "",
                    Value = "Edit this image:"
                },
                (await Page.Accessibility.SnapshotAsync()).Children[0]);
        }

        [Fact]
        public async Task PlainTextFieldWithTabindexAndWithoutRoleShouldNotHaveContent()
        {
            await Page.SetContentAsync("<div contenteditable='plaintext-only' role='textbox' tabIndex=0>Edit this image:<img src='fakeimage.png' alt='my fake image'></div>");
            Assert.Equal(
                new SerializedAXNode
                {
                    Role = "textbox",
                    Name = "",
                    Value = "Edit this image:"
                },
                (await Page.Accessibility.SnapshotAsync()).Children[0]);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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
