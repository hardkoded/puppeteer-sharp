using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class PageEmulateLocaleTests : PuppeteerPageBaseTest
    {
        public PageEmulateLocaleTests() : base()
        {
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.emulateLocale", "should work")]
        public async Task ShouldWork()
        {
            var defaultLocale = await Page.EvaluateFunctionAsync<string>("() => Intl.NumberFormat().resolvedOptions().locale");
            var defaultLanguage = await Page.EvaluateFunctionAsync<string>("() => navigator.language");

            await Page.EmulateLocaleAsync("de-DE");
            Assert.That(
                await Page.EvaluateFunctionAsync<string>("() => Intl.NumberFormat().resolvedOptions().locale"),
                Is.EqualTo("de-DE"));
            Assert.That(
                await Page.EvaluateFunctionAsync<string>("() => new Intl.NumberFormat().format(123456.78)"),
                Is.EqualTo("123.456,78"));
            Assert.That(
                await Page.EvaluateFunctionAsync<string>("() => navigator.language"),
                Is.EqualTo("de-DE"));
            Assert.That(
                await Page.EvaluateFunctionAsync<string>("() => navigator.languages[0]"),
                Is.EqualTo("de-DE"));

            await Page.EmulateLocaleAsync("fr-FR");
            Assert.That(
                await Page.EvaluateFunctionAsync<string>("() => Intl.DateTimeFormat().resolvedOptions().locale"),
                Is.EqualTo("fr-FR"));

            await Page.EmulateLocaleAsync();
            Assert.That(
                await Page.EvaluateFunctionAsync<string>("() => Intl.NumberFormat().resolvedOptions().locale"),
                Is.EqualTo(defaultLocale));
            Assert.That(
                await Page.EvaluateFunctionAsync<string>("() => navigator.language"),
                Is.EqualTo(defaultLanguage));
        }
    }
}
