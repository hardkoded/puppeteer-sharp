using System.Linq;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class BoundingBoxTests : DevToolsContextBaseTest
    {
        public BoundingBoxTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.boundingBox", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var elementHandle = await DevToolsContext.QuerySelectorAsync<HtmlElement>(".box:nth-of-type(13)");
            var box = await elementHandle.BoundingBoxAsync();
            Assert.Equal(new BoundingBox(100, 50, 50, 50), box);
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.boundingBox", "should handle nested frames")]
        [PuppeteerFact]
        public async Task ShouldHandleNestedFrames()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html", WaitUntilNavigation.Networkidle0);
            var childFrame = DevToolsContext.Frames.First(f => f.Url.Contains("two-frames.html"));
            var nestedFrame = childFrame.ChildFrames.Last();
            var elementHandle = await nestedFrame.QuerySelectorAsync<HtmlElement>("div");
            var box = await elementHandle.BoundingBoxAsync();

            Assert.Equal(new BoundingBox(28, 182, 264, 18), box);
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.boundingBox", "should return null for invisible elements")]
        [PuppeteerFact]
        public async Task ShouldReturnNullForInvisibleElements()
        {
            await DevToolsContext.SetContentAsync("<div style='display:none'>hi</div>");
            var elementHandle = await DevToolsContext.QuerySelectorAsync<HtmlDivElement>("div");
            Assert.Null(await elementHandle.BoundingBoxAsync());
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.boundingBox", "should force a layout")]
        [PuppeteerFact]
        public async Task ShouldForceALayout()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await DevToolsContext.SetContentAsync("<div style='width: 100px; height: 100px'>hello</div>");
            var elementHandle = await DevToolsContext.QuerySelectorAsync<HtmlDivElement>("div");
            await DevToolsContext.EvaluateFunctionAsync("element => element.style.height = '200px'", (JSHandle)elementHandle);
            var box = await elementHandle.BoundingBoxAsync();
            Assert.Equal(new BoundingBox(8, 8, 100, 200), box);
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.boundingBox", "should work with SVG nodes")]
        [PuppeteerFact(Skip = "SVG NOT WORKING IN CEF REASON UNKNOWN")]
        public async Task ShouldWworkWithSVGNodes()
        {
            await DevToolsContext.SetContentAsync(@"
                <svg xmlns=""http://www.w3.org/2000/svg"" width=""500"" height=""500"">
                  <rect id=""theRect"" x=""30"" y=""50"" width=""200"" height=""300""></rect>
                </svg>
            ");

            var element = await DevToolsContext.QuerySelectorAsync("#therect");
            var pptrBoundingBox = await element.BoundingBoxAsync();
            var webBoundingBox = await DevToolsContext.EvaluateFunctionAsync<BoundingBox>(@"e =>
            {
                const rect = e.getBoundingClientRect();
                return { x: rect.x, y: rect.y, width: rect.width, height: rect.height};
            }", (JSHandle)element);
            Assert.Equal(webBoundingBox, pptrBoundingBox);
        }
    }
}
