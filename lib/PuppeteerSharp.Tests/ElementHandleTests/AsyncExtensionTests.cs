using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class AsyncExtensionTests : DevToolsContextBaseTest
    {
        public AsyncExtensionTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact]
        public async Task ShouldAndThenFunction()
        {
            const string expected = "checkbox";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");

            var element = DevToolsContext.QuerySelectorAsync<HtmlInputElement>("#agree");
            var actual = await element.AndThen(x => x.GetAttributeAsync<string>("type"));

            Assert.Equal(expected, actual);
            Assert.True(element.Result.IsDisposed);
        }

        [PuppeteerFact]
        public async Task ShouldAndThenFunctionChain()
        {
            const string expected = "checkbox";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");

            var element = DevToolsContext.QuerySelectorAsync<HtmlBodyElement>("body");
            var actual = await element
                .AndThen(x => x.QuerySelectorAsync<HtmlInputElement>("#agree"))
                .AndThen(x => x.GetAttributeAsync<string>("type"));

            Assert.Equal(expected, actual);
            Assert.True(element.Result.IsDisposed);
        }

        [PuppeteerFact]
        public async Task ShouldAndThenFunctionWithoutDispose()
        {
            const string expected = "checkbox";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");

            var element = DevToolsContext.QuerySelectorAsync<HtmlInputElement>("#agree");
            var actual = await element.AndThen(x => x.GetAttributeAsync<string>("type"), dispose: false);

            Assert.Equal(expected, actual);
            Assert.False(element.Result.IsDisposed);
        }
    }
}
