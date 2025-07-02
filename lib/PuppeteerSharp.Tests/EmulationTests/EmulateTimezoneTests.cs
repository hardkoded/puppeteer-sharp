using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class EmulateTimezoneTests : PuppeteerPageBaseTest
    {
        public EmulateTimezoneTests() : base()
        {
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.emulateTimezone", "should work")]
        public async Task ShouldWork()
        {
            await Page.EvaluateExpressionAsync("globalThis.date = new Date(1479579154987);");
            await Page.EmulateTimezoneAsync("America/Jamaica");
            Assert.That(
                await Page.EvaluateExpressionAsync<string>("date.toString()"), Is.EqualTo("Sat Nov 19 2016 13:12:34 GMT-0500 (Eastern Standard Time)"));

            await Page.EmulateTimezoneAsync("Pacific/Honolulu");
            Assert.That(
                await Page.EvaluateExpressionAsync<string>("date.toString()"), Is.EqualTo("Sat Nov 19 2016 08:12:34 GMT-1000 (Hawaii-Aleutian Standard Time)"));

            await Page.EmulateTimezoneAsync("America/Buenos_Aires");
            Assert.That(
                await Page.EvaluateExpressionAsync<string>("date.toString()"), Is.EqualTo("Sat Nov 19 2016 15:12:34 GMT-0300 (Argentina Standard Time)"));

            await Page.EmulateTimezoneAsync("Europe/Berlin");
            Assert.That(
                await Page.EvaluateExpressionAsync<string>("date.toString()"), Is.EqualTo("Sat Nov 19 2016 19:12:34 GMT+0100 (Central European Standard Time)"));
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.emulateTimezone", "should throw for invalid timezone IDs")]
        public void ShouldThrowForInvalidTimezoneId()
        {
            var exception = Assert.ThrowsAsync<PuppeteerException>(
                () => Page.EmulateTimezoneAsync("Foo/Bar"));
            Assert.That(exception.Message, Does.Contain("Invalid timezone ID: Foo/Bar"));

            exception = Assert.ThrowsAsync<PuppeteerException>(
                () => Page.EmulateTimezoneAsync("Baz/Qux"));
            Assert.That(exception.Message, Does.Contain("Invalid timezone ID: Baz/Qux"));
        }
    }
}
