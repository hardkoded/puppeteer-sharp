using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.PageAccessibility;

namespace PuppeteerSharp.Tests.AccessibilityTests
{
    public class RootOptionTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("accessibility.spec", "root option", "should work a button")]
        public async Task ShouldWorkAButton()
        {
            await Page.SetContentAsync("<button>My Button</button>");

            var button = await Page.QuerySelectorAsync("button");
            Assert.That(
                await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions { Root = button }),
                Is.EqualTo(new SerializedAXNode
                {
                    Role = "button",
                    Name = "My Button"
                }));
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "root option", "should work an input")]
        public async Task ShouldWorkAnInput()
        {
            await Page.SetContentAsync("<input title='My Input' value='My Value'>");

            var input = await Page.QuerySelectorAsync("input");
            Assert.That(
                await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions { Root = input }),
                Is.EqualTo(new SerializedAXNode
                {
                    Role = "textbox",
                    Name = "My Input",
                    Value = "My Value"
                }));
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "root option", "should work a menu")]
        public async Task ShouldWorkAMenu()
        {
            await Page.SetContentAsync(@"
            <div role=""menu"" title=""My Menu"" >
              <div role=""menuitem"">First Item</div>
              <div role=""menuitem"">Second Item</div>
              <div role=""menuitem"">Third Item</div>
            </div>
            ");

            var menu = await Page.QuerySelectorAsync("div[role=\"menu\"]");
            var snapshot = await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions { Root = menu });
            var nodeToCheck = new SerializedAXNode
            {
                Role = "menu",
                Name = "My Menu",
                Orientation = "vertical",
                Children = new[]
                    {
                        new SerializedAXNode
                        {
                            Role = "menuitem",
                            Name = "First Item"
                        },
                        new SerializedAXNode
                        {
                            Role = "menuitem",
                            Name = "Second Item"
                        },
                        new SerializedAXNode
                        {
                            Role = "menuitem",
                            Name = "Third Item"
                        }
                    }
            };

            Assert.That(snapshot, Is.EqualTo(nodeToCheck));
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "root option", "should return null when the element is no longer in DOM")]
        public async Task ShouldReturnNullWhenTheElementIsNoLongerInDOM()
        {
            await Page.SetContentAsync("<button>My Button</button>");
            var button = await Page.QuerySelectorAsync("button");
            await Page.EvaluateFunctionAsync("button => button.remove()", button);
            Assert.That(await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions { Root = button }), Is.Null);
        }

        [Test, Retry(2), PuppeteerTest("accessibility.spec", "root option", "should support the interestingOnly option")]
        public async Task ShouldSupportTheInterestingOnlyOption()
        {
            await Page.SetContentAsync("<div><button>My Button</button></div>");
            var div = await Page.QuerySelectorAsync("div");
            Assert.That(await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions
            {
                Root = div
            }), Is.Null);
            Assert.That(
                await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions
                {
                    Root = div,
                    InterestingOnly = false
                }),
                Is.EqualTo(new SerializedAXNode
                {
                    Role = "generic",
                    Name = "",
                    Children = new[]
                    {
                        new SerializedAXNode
                        {
                            Role = "button",
                            Name = "My Button",
                            Children = new[]
                            {
                                new SerializedAXNode
                                {
                                    Role = "StaticText",
                                    Name = "My Button",
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
                }));
        }
    }
}
