using System.Threading.Tasks;
using PuppeteerSharp.PageAccessibility;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.AccesibilityTests
{
    public class RootOptionTests : PuppeteerPageBaseTest
    {
        public RootOptionTests(): base()
        {
        }

        [PuppeteerTest("accessibility.spec.ts", "root option", "should work a button")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWorkAButton()
        {
            await Page.SetContentAsync("<button>My Button</button>");

            var button = await Page.QuerySelectorAsync("button");
            Assert.AreEqual(
                new SerializedAXNode
                {
                    Role = "button",
                    Name = "My Button"
                },
                await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions { Root = button }));
        }

        [PuppeteerTest("accessibility.spec.ts", "root option", "should work an input")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWorkAnInput()
        {
            await Page.SetContentAsync("<input title='My Input' value='My Value'>");

            var input = await Page.QuerySelectorAsync("input");
            Assert.AreEqual(
                new SerializedAXNode
                {
                    Role = "textbox",
                    Name = "My Input",
                    Value = "My Value"
                },
                await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions { Root = input }));
        }

        [PuppeteerTest("accessibility.spec.ts", "root option", "should work a menu")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

            Assert.AreEqual(nodeToCheck, snapshot);
        }

        [PuppeteerTest("accessibility.spec.ts", "root option", "should return null when the element is no longer in DOM")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldReturnNullWhenTheElementIsNoLongerInDOM()
        {
            await Page.SetContentAsync("<button>My Button</button>");
            var button = await Page.QuerySelectorAsync("button");
            await Page.EvaluateFunctionAsync("button => button.remove()", button);
            Assert.Null(await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions { Root = button }));
        }

        [PuppeteerTest("accessibility.spec.ts", "root option", "should support the interestingOnly option")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldSupportTheInterestingOnlyOption()
        {
            await Page.SetContentAsync("<div><button>My Button</button></div>");
            var div = await Page.QuerySelectorAsync("div");
            Assert.Null(await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions
            {
                Root = div
            }));
            Assert.AreEqual(
                new SerializedAXNode
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
                                }
                            }
                        }
                    }
                },
                await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions
                {
                    Root = div,
                    InterestingOnly = false
                }));
        }
    }
}
