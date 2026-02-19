using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserContextTests
{
    public class BrowserContextSetPermissionTests : PuppeteerPageBaseTest
    {
        public BrowserContextSetPermissionTests() : base()
        {
        }

        private Task<string> GetPermissionAsync(IPage page, string name)
            => page.EvaluateFunctionAsync<string>(
                "name => navigator.permissions.query({ name }).then(result => result.state)",
                name);

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext BrowserContext.setPermission", "should set permission")]
        public async Task ShouldSetPermission()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Context.SetPermissionAsync(TestConstants.EmptyPage, new PermissionEntry
            {
                Permission = new PermissionDescriptor { Name = "geolocation" },
                State = PermissionState.Granted,
            });
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("granted"));

            await Context.SetPermissionAsync(TestConstants.EmptyPage, new PermissionEntry
            {
                Permission = new PermissionDescriptor { Name = "geolocation" },
                State = PermissionState.Denied,
            });
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("denied"));

            await Context.SetPermissionAsync(TestConstants.EmptyPage, new PermissionEntry
            {
                Permission = new PermissionDescriptor { Name = "geolocation" },
                State = PermissionState.Prompt,
            });
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("prompt"));
        }

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext BrowserContext.setPermission", "should support * as origin")]
        public async Task ShouldSupportStarAsOrigin()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Context.SetPermissionAsync("*", new PermissionEntry
            {
                Permission = new PermissionDescriptor { Name = "geolocation" },
                State = PermissionState.Granted,
            });
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("granted"));

            await Context.SetPermissionAsync("*", new PermissionEntry
            {
                Permission = new PermissionDescriptor { Name = "geolocation" },
                State = PermissionState.Denied,
            });
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("denied"));

            await Context.SetPermissionAsync("*", new PermissionEntry
            {
                Permission = new PermissionDescriptor { Name = "geolocation" },
                State = PermissionState.Prompt,
            });
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("prompt"));
        }

        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext BrowserContext.setPermission", "should support multiple permissions")]
        public async Task ShouldSupportMultiplePermissions()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Context.SetPermissionAsync(
                TestConstants.EmptyPage,
                new PermissionEntry { Permission = new PermissionDescriptor { Name = "geolocation" }, State = PermissionState.Granted },
                new PermissionEntry { Permission = new PermissionDescriptor { Name = "midi" }, State = PermissionState.Granted });
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("granted"));
            Assert.That(await GetPermissionAsync(Page, "midi"), Is.EqualTo("granted"));

            await Context.SetPermissionAsync(
                TestConstants.EmptyPage,
                new PermissionEntry { Permission = new PermissionDescriptor { Name = "geolocation" }, State = PermissionState.Denied },
                new PermissionEntry { Permission = new PermissionDescriptor { Name = "midi" }, State = PermissionState.Denied });
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("denied"));
            Assert.That(await GetPermissionAsync(Page, "midi"), Is.EqualTo("denied"));

            await Context.SetPermissionAsync(
                TestConstants.EmptyPage,
                new PermissionEntry { Permission = new PermissionDescriptor { Name = "geolocation" }, State = PermissionState.Prompt },
                new PermissionEntry { Permission = new PermissionDescriptor { Name = "midi" }, State = PermissionState.Prompt });
            Assert.That(await GetPermissionAsync(Page, "geolocation"), Is.EqualTo("prompt"));
            Assert.That(await GetPermissionAsync(Page, "midi"), Is.EqualTo("prompt"));
        }
    }
}
