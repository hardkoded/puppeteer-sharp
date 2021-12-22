using System.Threading.Tasks;
using PuppeteerSharp.Media;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class EmulateTimezoneTests : PuppeteerPageBaseTest
    {
        public EmulateTimezoneTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            await Page.EvaluateExpressionAsync("globalThis.date = new Date(1479579154987);");
            await Page.EmulateTimezoneAsync("America/Jamaica");
            Assert.Equal(
                "Sat Nov 19 2016 13:12:34 GMT-0500 (Eastern Standard Time)",
                await Page.EvaluateExpressionAsync<string>("date.toString()"));

            await Page.EmulateTimezoneAsync("Pacific/Honolulu");
            Assert.Equal(
                "Sat Nov 19 2016 08:12:34 GMT-1000 (Hawaii-Aleutian Standard Time)",
                await Page.EvaluateExpressionAsync<string>("date.toString()"));

            await Page.EmulateTimezoneAsync("America/Buenos_Aires");
            Assert.Equal(
                "Sat Nov 19 2016 15:12:34 GMT-0300 (Argentina Standard Time)",
                await Page.EvaluateExpressionAsync<string>("date.toString()"));

            await Page.EmulateTimezoneAsync("Europe/Berlin");
            Assert.Equal(
                "Sat Nov 19 2016 19:12:34 GMT+0100 (Central European Standard Time)",
                await Page.EvaluateExpressionAsync<string>("date.toString()"));
        }

        [Fact]
        public async Task ShouldThrowForInvalidTimezoneId()
        {
            var exception = await Assert.ThrowsAnyAsync<PuppeteerException>(
                () => Page.EmulateTimezoneAsync("Foo/Bar"));
            Assert.Contains("Invalid timezone ID: Foo/Bar", exception.Message);

            exception = await Assert.ThrowsAnyAsync<PuppeteerException>(
                () => Page.EmulateTimezoneAsync("Baz/Qux"));
            Assert.Contains("Invalid timezone ID: Baz/Qux", exception.Message);
        }
    }
}
