using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Xunit;
using PuppeteerSharp.Tests.Attributes;
using CefSharp;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextQuerySelectorAllTests : DevToolsContextBaseTest
    {
        public DevToolsContextQuerySelectorAllTests(ITestOutputHelper output) : base(output)
        {
        }

#pragma warning disable xUnit1013 // Public method should be marked as test
        public static async Task Usage(IWebBrowser chromiumWebBrowser)
#pragma warning restore xUnit1013 // Public method should be marked as test
        {
            #region QuerySelectorAll

            // Add using CefSharp.DevTools.Dom to access CreateDevToolsContextAsync and related extension methods.
            await using var devToolsContext = await chromiumWebBrowser.CreateDevToolsContextAsync();

            // Get elements by tag name
            // https://developer.mozilla.org/en-US/docs/Web/API/Document/querySelectorAll
            var inputElements = await devToolsContext.QuerySelectorAllAsync<HtmlInputElement>("input");

            foreach (var element in inputElements)
            {
                var name = await element.GetNameAsync();
                var id = await element.GetIdAsync();

                var value = await element.GetValueAsync<int>();
            }

            #endregion
        }

        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var elements = await DevToolsContext.QuerySelectorAllAsync<HtmlTableRowElement>("tr");

            Assert.NotNull(elements);
            Assert.Equal(4, elements.Length);
        }

        [PuppeteerFact]
        public async Task ShouldReturnEmptyArray()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var elements = await DevToolsContext.QuerySelectorAllAsync<HtmlTableRowElement>("#table2 tr");

            Assert.NotNull(elements);
            Assert.Empty(elements);
        }

        [PuppeteerFact]
        public async Task ShouldQueryExistingElements()
        {
            await DevToolsContext.SetContentAsync("<div>A</div><br/><div>B</div>");
            var elements = await DevToolsContext.QuerySelectorAllAsync("div");
            Assert.Equal(2, elements.Length);
            var tasks = elements.Select(element => DevToolsContext.EvaluateFunctionAsync<string>("e => e.textContent", element));
            Assert.Equal(new[] { "A", "B" }, await Task.WhenAll(tasks));
        }

        [PuppeteerFact]
        public async Task ShouldReturnEmptyArrayIfNothingIsFound()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var elements = await DevToolsContext.QuerySelectorAllAsync("div");
            Assert.Empty(elements);
        }
    }
}
