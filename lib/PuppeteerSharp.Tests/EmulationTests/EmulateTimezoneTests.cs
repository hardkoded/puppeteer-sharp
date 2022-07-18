using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.EmulationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class EmulateTimezoneTests : DevToolsContextBaseTest
    {
        public EmulateTimezoneTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("emulation.spec.ts", "Page.emulateTimezone", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.EvaluateExpressionAsync("globalThis.date = new Date(1479579154987);");
            await DevToolsContext.EmulateTimezoneAsync("America/Jamaica");
            Assert.Equal(
                "Sat Nov 19 2016 13:12:34 GMT-0500 (Eastern Standard Time)",
                await DevToolsContext.EvaluateExpressionAsync<string>("date.toString()"));

            await DevToolsContext.EmulateTimezoneAsync("Pacific/Honolulu");
            Assert.Equal(
                "Sat Nov 19 2016 08:12:34 GMT-1000 (Hawaii-Aleutian Standard Time)",
                await DevToolsContext.EvaluateExpressionAsync<string>("date.toString()"));

            await DevToolsContext.EmulateTimezoneAsync("America/Buenos_Aires");
            Assert.Equal(
                "Sat Nov 19 2016 15:12:34 GMT-0300 (Argentina Standard Time)",
                await DevToolsContext.EvaluateExpressionAsync<string>("date.toString()"));

            await DevToolsContext.EmulateTimezoneAsync("Europe/Berlin");
            Assert.Equal(
                "Sat Nov 19 2016 19:12:34 GMT+0100 (Central European Standard Time)",
                await DevToolsContext.EvaluateExpressionAsync<string>("date.toString()"));
        }

        [PuppeteerTest("emulation.spec.ts", "Page.emulateTimezone", "should throw for invalid timezone IDs")]
        [PuppeteerFact]
        public async Task ShouldThrowForInvalidTimezoneId()
        {
            var exception = await Assert.ThrowsAnyAsync<PuppeteerException>(
                () => DevToolsContext.EmulateTimezoneAsync("Foo/Bar"));
            Assert.Contains("Invalid timezone ID: Foo/Bar", exception.Message);

            exception = await Assert.ThrowsAnyAsync<PuppeteerException>(
                () => DevToolsContext.EmulateTimezoneAsync("Baz/Qux"));
            Assert.Contains("Invalid timezone ID: Baz/Qux", exception.Message);
        }
    }
}
