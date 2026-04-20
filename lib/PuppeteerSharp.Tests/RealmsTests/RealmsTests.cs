using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.RealmsTests
{
    public class RealmsTests : PuppeteerBaseTest
    {
        private static readonly string _extensionPath = Path.Combine(AppContext.BaseDirectory, "Assets", "simple-extension");

        private static LaunchOptions BrowserWithExtensionOptions() => new()
        {
            Headless = false,
            EnableExtensions = true,
            Pipe = true,
        };

        [Test, PuppeteerTest("realms.spec", "extension realms", "should include content script realms")]
        public async Task ShouldIncludeContentScriptRealms()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);

            var page = await browserWithExtension.NewPageAsync();
            var extId = await browserWithExtension.InstallExtensionAsync(_extensionPath);
            await browserWithExtension.WaitForTargetAsync(t => t.Url.Contains(extId));

            await page.GoToAsync(TestConstants.EmptyPage);

            var realms = page.ExtensionRealms();
            Assert.That(realms.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test, PuppeteerTest("realms.spec", "extension realms", "realm should return extension that created it")]
        public async Task RealmShouldReturnExtensionThatCreatedIt()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);

            var page = await browserWithExtension.NewPageAsync();
            var extId = await browserWithExtension.InstallExtensionAsync(_extensionPath);
            await browserWithExtension.WaitForTargetAsync(t => t.Url.Contains(extId));

            await page.GoToAsync(TestConstants.EmptyPage);
            var realms = page.ExtensionRealms();

            Realm realm = null;
            foreach (var r in realms)
            {
                var ext = await r.ExtensionAsync();
                if (ext != null && ext.Id == extId)
                {
                    realm = r;
                    break;
                }
            }

            Assert.That(realm, Is.Not.Null);
            var extension = await realm.ExtensionAsync();
            Assert.That(extension, Is.Not.Null);
            Assert.That(extension.Id, Is.EqualTo(extId));
        }

        [Test, PuppeteerTest("realms.spec", "extension realms", "should evaluate in content script realms")]
        public async Task ShouldEvaluateInContentScriptRealms()
        {
            await using var browserWithExtension = await Puppeteer.LaunchAsync(
                BrowserWithExtensionOptions(),
                TestConstants.LoggerFactory);

            var page = await browserWithExtension.NewPageAsync();
            var extId = await browserWithExtension.InstallExtensionAsync(_extensionPath);
            await browserWithExtension.WaitForTargetAsync(t => t.Url.Contains(extId));

            await page.GoToAsync(TestConstants.EmptyPage);
            var realms = page.ExtensionRealms();

            Realm contentScriptRealm = null;
            foreach (var r in realms)
            {
                var ext = await r.ExtensionAsync();
                if (ext != null && ext.Id == extId)
                {
                    contentScriptRealm = r;
                    break;
                }
            }

            Assert.That(contentScriptRealm, Is.Not.Null);

            var isContentScript = await contentScriptRealm.EvaluateFunctionAsync<bool>("() => globalThis.thisIsTheContentScript");
            Assert.That(isContentScript, Is.True);

            var isContentScriptInMain = await page.EvaluateFunctionAsync<object>("() => globalThis.thisIsTheContentScript");
            Assert.That(isContentScriptInMain, Is.Null);
        }
    }
}
