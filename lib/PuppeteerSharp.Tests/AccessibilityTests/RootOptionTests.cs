using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.PageAccessibility;

namespace PuppeteerSharp.Tests.AccessibilityTests
{
    public class RootOptionTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("accessibility.spec", "root option", "should work a button")]
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

        [Test, PuppeteerTest("accessibility.spec", "root option", "should work an input")]
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

        [Test, PuppeteerTest("accessibility.spec", "root option", "should work a menu")]
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

        [Test, PuppeteerTest("accessibility.spec", "root option", "should return null when the element is no longer in DOM")]
        public async Task ShouldReturnNullWhenTheElementIsNoLongerInDOM()
        {
            await Page.SetContentAsync("<button>My Button</button>");
            var button = await Page.QuerySelectorAsync("button");
            await Page.EvaluateFunctionAsync("button => button.remove()", button);
            Assert.That(await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions { Root = button }), Is.Null);
        }

        [Test, PuppeteerTest("accessibility.spec", "root option", "should support the interestingOnly option")]
        public async Task ShouldSupportTheInterestingOnlyOption()
        {
            await Page.SetContentAsync("<div><button>My Button</button></div><div class=\"uninteresting\"></div>");
            var div = await Page.QuerySelectorAsync("div.uninteresting");
            Assert.That(await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions
            {
                Root = div
            }), Is.Null);

            var divWithButton = await Page.QuerySelectorAsync("div");
            var snapshot = await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions
            {
                Root = divWithButton
            });
            Assert.That(snapshot.Name, Is.EqualTo("My Button"));
            Assert.That(snapshot.Role, Is.EqualTo("button"));

            var fullSnapshot = await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions
            {
                Root = divWithButton,
                InterestingOnly = false
            });
            Assert.That(fullSnapshot.Role, Is.EqualTo("generic"));
            Assert.That(fullSnapshot.Name, Is.EqualTo(""));
            Assert.That(fullSnapshot.Children, Has.Length.EqualTo(1));
            Assert.That(fullSnapshot.Children[0].Role, Is.EqualTo("button"));
            Assert.That(fullSnapshot.Children[0].Name, Is.EqualTo("My Button"));
            Assert.That(fullSnapshot.Children[0].Children, Has.Length.EqualTo(1));
            Assert.That(fullSnapshot.Children[0].Children[0].Role, Is.EqualTo("StaticText"));
            Assert.That(fullSnapshot.Children[0].Children[0].Name, Is.EqualTo("My Button"));
        }
    }
}
