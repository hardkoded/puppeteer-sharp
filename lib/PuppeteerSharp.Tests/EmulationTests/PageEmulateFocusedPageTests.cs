using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class PageEmulateFocusedPageTests : PuppeteerPageBaseTest
    {
        public PageEmulateFocusedPageTests() : base()
        {
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.emulateFocusedPage", "should emulate focus")]
        public async Task ShouldEmulateFocus()
        {
            await Page.EmulateFocusedPageAsync(true);
            Assert.That(
                await Page.EvaluateFunctionAsync<bool>("() => document.hasFocus()"),
                Is.True);

            var page2 = await Context.NewPageAsync();
            // Move page into background by focusing page2.
            await page2.BringToFrontAsync();

            Assert.That(
                await Page.EvaluateFunctionAsync<bool>("() => document.hasFocus()"),
                Is.True);
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.emulateFocusedPage", "should reset focus")]
        public async Task ShouldResetFocus()
        {
            await Page.EmulateFocusedPageAsync(true);

            var page2 = await Context.NewPageAsync();
            // Move page into background by focusing page2.
            await page2.BringToFrontAsync();

            Assert.That(
                await Page.EvaluateFunctionAsync<bool>("() => document.hasFocus()"),
                Is.True);

            await Page.EmulateFocusedPageAsync(false);
            Assert.That(
                await Page.EvaluateFunctionAsync<bool>("() => document.hasFocus()"),
                Is.False);
        }
    }
}
