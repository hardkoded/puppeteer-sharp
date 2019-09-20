using System.Threading.Tasks;
using PuppeteerSharp.PageAccessibility;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.AccesibilityTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class RootOptionTests : PuppeteerPageBaseTest
    {
        public RootOptionTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWorkAButton()
        {
            await Page.SetContentAsync("<button>My Button</button>");

            var button = await Page.QuerySelectorAsync("button");
            Assert.Equal(
                new SerializedAXNode
                {
                    Role = "button",
                    Name = "My Button"
                },
                await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions { Root = button }));
        }

        [Fact]
        public async Task ShouldWorkAnInput()
        {
            await Page.SetContentAsync("<input title='My Input' value='My Value'>");

            var input = await Page.QuerySelectorAsync("input");
            Assert.Equal(
                new SerializedAXNode
                {
                    Role = "textbox",
                    Name = "My Input",
                    Value = "My Value"
                },
                await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions { Root = input }));
        }

        [Fact]
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
            Assert.Equal(
                new SerializedAXNode
                {
                    Role = "menu",
                    Name = "My Menu",
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
                },
                await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions { Root = menu }));
        }

        [Fact]
        public async Task ShouldReturnNullWhenTheElementIsNoLongerInDOM()
        {
            await Page.SetContentAsync("<button>My Button</button>");
            var button = await Page.QuerySelectorAsync("button");
            await Page.EvaluateFunctionAsync("button => button.remove()", button);
            Assert.Null(await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions { Root = button }));
        }

        [Fact]
        public async Task ShouldSupportTheInterestingOnlyOption()
        {
            await Page.SetContentAsync("<div><button>My Button</button></div>");
            var div = await Page.QuerySelectorAsync("div");
            Assert.Null(await Page.Accessibility.SnapshotAsync(new AccessibilitySnapshotOptions
            {
                Root = div
            }));
            Assert.Equal(
                new SerializedAXNode
                {
                    Role = "GenericContainer",
                    Name = "",
                    Children = new[]
                    {
                        new SerializedAXNode
                        {
                            Role = "button",
                            Name = "My Button"
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
