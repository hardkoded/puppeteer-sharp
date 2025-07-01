using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserContextTests
{
    public class BrowserContextOverridePermissionsTests : PuppeteerPageBaseTest
    {
        public BrowserContextOverridePermissionsTests() : base()
        {
        }

        private Task<string> GetPermissionAsync(IPage page, string name)
            => page.EvaluateFunctionAsync<string>(
                "name => navigator.permissions.query({ name }).then(result => result.state)",
                name);

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext BrowserContext.overridePermissions", "should be prompt by default")]
        public async Task ShouldBePromptByDefault()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("prompt"));
        }

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext BrowserContext.overridePermissions", "should deny permission when not listed")]
        public async Task ShouldDenyPermissionWhenNotListed()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Context.OverridePermissionsAsync(TestConstants.EmptyPage, new OverridePermission[] { });
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("denied"));
        }

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext BrowserContext.overridePermissions", "should grant permission when listed")]
        public async Task ShouldGrantPermissionWhenListed()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Context.OverridePermissionsAsync(TestConstants.EmptyPage, new OverridePermission[]
            {
                OverridePermission.Geolocation
            });
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("granted"));
        }

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext BrowserContext.overridePermissions", "should reset permissions")]
        public async Task ShouldResetPermissions()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Context.OverridePermissionsAsync(TestConstants.EmptyPage, new OverridePermission[]
            {
                OverridePermission.Geolocation
            });
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("granted"));
            await Context.ClearPermissionOverridesAsync();
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("prompt"));
        }

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext BrowserContext.overridePermissions", "should trigger permission onchange")]
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
            Assert.That(await Page.EvaluateExpressionAsync<string[]>("window.events"), Is.EqualTo(new string[] { "prompt" }));
            await Context.OverridePermissionsAsync(TestConstants.EmptyPage, new OverridePermission[] { });
            Assert.That(await Page.EvaluateExpressionAsync<string[]>("window.events"), Is.EqualTo(new string[] { "prompt", "denied" }));
            await Context.OverridePermissionsAsync(TestConstants.EmptyPage, new OverridePermission[]
            {
                OverridePermission.Geolocation
            });
            Assert.That(
                await Page.EvaluateExpressionAsync<string[]>("window.events"), Is.EqualTo(new string[] { "prompt", "denied", "granted" }));
            await Context.ClearPermissionOverridesAsync();
            Assert.That(
                await Page.EvaluateExpressionAsync<string[]>("window.events"), Is.EqualTo(new string[] { "prompt", "denied", "granted", "prompt" }));
        }

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext BrowserContext.overridePermissions", "should isolate permissions between browser contexts")]
        public async Task ShouldIsolatePermissionsBetweenBrowserContexts()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var otherContext = await Browser.CreateBrowserContextAsync();
            var otherPage = await otherContext.NewPageAsync();
            await otherPage.GoToAsync(TestConstants.EmptyPage);
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("prompt"));
            Assert.That(await GetPermissionAsync(otherPage, "geolocation"), Is.EqualTo("prompt"));

            await Context.OverridePermissionsAsync(TestConstants.EmptyPage, new OverridePermission[] { });
            await otherContext.OverridePermissionsAsync(TestConstants.EmptyPage, new OverridePermission[] { OverridePermission.Geolocation });
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("denied"));
            Assert.That(await GetPermissionAsync(otherPage, "geolocation"), Is.EqualTo("granted"));

            await Context.ClearPermissionOverridesAsync();
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("prompt"));
            Assert.That(await GetPermissionAsync(otherPage, "geolocation"), Is.EqualTo("granted"));

            await otherContext.CloseAsync();
        }

        [Test, Ignore("Fails on Firefox")]
        public async Task AllEnumsdAreValid()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Context.OverridePermissionsAsync(
                TestConstants.EmptyPage,
                Enum.GetValues<OverridePermission>());
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("granted"));
            await Context.ClearPermissionOverridesAsync();
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("prompt"));
        }
    }
}
