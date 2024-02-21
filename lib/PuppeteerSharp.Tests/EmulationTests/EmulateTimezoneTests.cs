using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Media;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class EmulateTimezoneTests : PuppeteerPageBaseTest
    {
        public EmulateTimezoneTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("emulation.spec", "Emulation Page.emulateTimezone", "should work")]
        public async Task ShouldWork()
        {
            await Page.EvaluateExpressionAsync("globalThis.date = new Date(1479579154987);");
            await Page.EmulateTimezoneAsync("America/Jamaica");
            Assert.AreEqual(
                "Sat Nov 19 2016 13:12:34 GMT-0500 (Eastern Standard Time)",
                await Page.EvaluateExpressionAsync<string>("date.toString()"));

            await Page.EmulateTimezoneAsync("Pacific/Honolulu");
            Assert.AreEqual(
                "Sat Nov 19 2016 08:12:34 GMT-1000 (Hawaii-Aleutian Standard Time)",
                await Page.EvaluateExpressionAsync<string>("date.toString()"));

            await Page.EmulateTimezoneAsync("America/Buenos_Aires");
            Assert.AreEqual(
                "Sat Nov 19 2016 15:12:34 GMT-0300 (Argentina Standard Time)",
                await Page.EvaluateExpressionAsync<string>("date.toString()"));

            await Page.EmulateTimezoneAsync("Europe/Berlin");
            Assert.AreEqual(
                "Sat Nov 19 2016 19:12:34 GMT+0100 (Central European Standard Time)",
                await Page.EvaluateExpressionAsync<string>("date.toString()"));
        }

        [Test, Retry(2), PuppeteerTest("emulation.spec", "Emulation Page.emulateTimezone", "should throw for invalid timezone IDs")]
        public void ShouldThrowForInvalidTimezoneId()
        {
            var exception = Assert.ThrowsAsync<PuppeteerException>(
                () => Page.EmulateTimezoneAsync("Foo/Bar"));
            StringAssert.Contains("Invalid timezone ID: Foo/Bar", exception.Message);

            exception = Assert.ThrowsAsync<PuppeteerException>(
                () => Page.EmulateTimezoneAsync("Baz/Qux"));
            StringAssert.Contains("Invalid timezone ID: Baz/Qux", exception.Message);
        }
    }
}
