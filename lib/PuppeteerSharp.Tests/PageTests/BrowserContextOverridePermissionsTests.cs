using System;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class BrowserContextOverridePermissionsTests : PuppeteerPageBaseTest
    {
        public BrowserContextOverridePermissionsTests(): base()
        {
        }

        private Task<string> GetPermissionAsync(IPage page, string name)
            => page.EvaluateFunctionAsync<string>(
                "name => navigator.permissions.query({ name }).then(result => result.state)",
                name);

        [PuppeteerTest("page.spec.ts", "BrowserContext.overridePermissions", "should be prompt by default")]
        [PuppeteerFact]
        public async Task ShouldBePromptByDefault()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal("prompt", await GetPermissionAsync(Page, "geolocation"));
        }

        [PuppeteerTest("page.spec.ts", "BrowserContext.overridePermissions", "should deny permission when not listed")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldDenyPermissionWhenNotListed()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Context.OverridePermissionsAsync(TestConstants.EmptyPage, new OverridePermission[] { });
            Assert.Equal("denied", await GetPermissionAsync(Page, "geolocation"));
        }

        [PuppeteerTest("page.spec.ts", "BrowserContext.overridePermissions", "should grant permission when listed")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldGrantPermissionWhenListed()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Context.OverridePermissionsAsync(TestConstants.EmptyPage, new OverridePermission[]
            {
                OverridePermission.Geolocation
            });
            Assert.Equal("granted", await GetPermissionAsync(Page, "geolocation"));
        }

        [PuppeteerTest("page.spec.ts", "BrowserContext.overridePermissions", "should reset permissions")]
        [SkipBrowserFact(skipFirefox: true)]
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

        [PuppeteerTest("page.spec.ts", "BrowserContext.overridePermissions", "should trigger permission onchange")]
        [SkipBrowserFact(skipFirefox: true)]
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

        [PuppeteerTest("page.spec.ts", "BrowserContext.overridePermissions", "should isolate permissions between browser contexts")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldIsolatePermissionsBetweenBrowserContexts()
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

        [SkipBrowserFact(skipFirefox: true)]
        public async Task AllEnumsdAreValid()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Context.OverridePermissionsAsync(
                TestConstants.EmptyPage,
                Enum.GetValues(typeof(OverridePermission)).Cast<OverridePermission>().ToArray());
            Assert.Equal("granted", await GetPermissionAsync(Page, "geolocation"));
            await Context.ClearPermissionOverridesAsync();
            Assert.Equal("prompt", await GetPermissionAsync(Page, "geolocation"));
        }
    }
}
