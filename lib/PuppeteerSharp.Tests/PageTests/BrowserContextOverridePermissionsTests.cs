using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class BrowserContextOverridePermissionsTests : PuppeteerPageBaseTest
    {
        public BrowserContextOverridePermissionsTests(ITestOutputHelper output) : base(output)
        {
        }

        private Task<string> GetPermissionAsync(Page page, string name)
            => page.EvaluateFunctionAsync<string>(
                "name => navigator.permissions.query({ name }).then(result => result.state)",
                name);

        [Fact]
        public async Task ShouldBePromptByDefault()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal("prompt", await GetPermissionAsync(Page, "geolocation"));
        }

        [Fact]
        public async Task ShouldDenyPermissionWhenNotListed()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Context.OverridePermissionsAsync(TestConstants.EmptyPage, new OverridePermission[] { });
            Assert.Equal("denied", await GetPermissionAsync(Page, "geolocation"));
        }

        [Fact]
        public async Task ShouldGrantPermissionWwhenListed()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Context.OverridePermissionsAsync(TestConstants.EmptyPage, new OverridePermission[]
            {
                OverridePermission.Geolocation
            });
            Assert.Equal("granted", await GetPermissionAsync(Page, "geolocation"));
        }

        [Fact]
        public async Task ShouldResetPermissions()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Context.OverridePermissionsAsync(TestConstants.EmptyPage, new OverridePermission[]
            {
                OverridePermission.Geolocation
            });
            Assert.Equal("granted", await GetPermissionAsync(Page, "geolocation"));
            await Context.ClearPermissionOverridesAsync();
            Assert.Equal("prompt", await GetPermissionAsync(Page, "geolocation"));
        }

        [Fact]
        public async Task ShouldTriggerPermissionOnchange()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(@"() => {
                window.events = [];
                return navigator.permissions.query({ name: 'geolocation'}).then(function(result) {
                    window.events.push(result.state);
                    result.onchange = function() {
                        window.events.push(result.state);
                    };
                });
            }");
            Assert.Equal(new string[] { "prompt" }, await Page.EvaluateExpressionAsync<string[]>("window.events"));
            await Context.OverridePermissionsAsync(TestConstants.EmptyPage, new OverridePermission[] { });
            Assert.Equal(new string[] { "prompt", "denied" }, await Page.EvaluateExpressionAsync<string[]>("window.events"));
            await Context.OverridePermissionsAsync(TestConstants.EmptyPage, new OverridePermission[]
            {
                OverridePermission.Geolocation
            });
            Assert.Equal(
                new string[] { "prompt", "denied", "granted" },
                await Page.EvaluateExpressionAsync<string[]>("window.events"));
            await Context.ClearPermissionOverridesAsync();
            Assert.Equal(
                new string[] { "prompt", "denied", "granted", "prompt" },
                await Page.EvaluateExpressionAsync<string[]>("window.events"));
        }

        [Fact]
        public async Task ShouldIsolatePermissionsBetweenBrowserContexs()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var otherContext = await Browser.CreateIncognitoBrowserContextAsync();
            var otherPage = await otherContext.NewPageAsync();
            await otherPage.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal("prompt", await GetPermissionAsync(Page, "geolocation"));
            Assert.Equal("prompt", await GetPermissionAsync(otherPage, "geolocation"));

            await Context.OverridePermissionsAsync(TestConstants.EmptyPage, new OverridePermission[] { });
            await otherContext.OverridePermissionsAsync(TestConstants.EmptyPage, new OverridePermission[] { OverridePermission.Geolocation });
            Assert.Equal("denied", await GetPermissionAsync(Page, "geolocation"));
            Assert.Equal("granted", await GetPermissionAsync(otherPage, "geolocation"));

            await Context.ClearPermissionOverridesAsync();
            Assert.Equal("prompt", await GetPermissionAsync(Page, "geolocation"));
            Assert.Equal("granted", await GetPermissionAsync(otherPage, "geolocation"));

            await otherContext.CloseAsync();
        }
    }
}
