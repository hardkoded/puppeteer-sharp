using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class BoundingBoxTests : PuppeteerPageBaseTest
    {
        public BoundingBoxTests(): base()
        {
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.boundingBox", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
            await Page.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var elementHandle = await Page.QuerySelectorAsync(".box:nth-of-type(13)");
            var box = await elementHandle.BoundingBoxAsync();
            Assert.Equal(new BoundingBox(100, 50, 50, 50), box);
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.boundingBox", "should handle nested frames")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldHandleNestedFrames()
        {
            await Page.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html", WaitUntilNavigation.Networkidle0);
            var childFrame = Page.Frames.First(f => f.Url.Contains("two-frames.html"));
            var nestedFrame = childFrame.ChildFrames.Last();
            var elementHandle = await nestedFrame.QuerySelectorAsync("div");
            var box = await elementHandle.BoundingBoxAsync();

            if (TestConstants.IsChrome)
            {
                Assert.Equal(new BoundingBox(28, 182, 264, 18), box);
            }
            else
            {
                Assert.Equal(new BoundingBox(28, 182, 254, 18), box);
            }
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.boundingBox", "should return null for invisible elements")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldReturnNullForInvisibleElements()
        {
            await Page.SetContentAsync("<div style='display:none'>hi</div>");
            var elementHandle = await Page.QuerySelectorAsync("div");
            Assert.Null(await elementHandle.BoundingBoxAsync());
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.boundingBox", "should force a layout")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldForceALayout()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync("<div style='width: 100px; height: 100px'>hello</div>");
            var elementHandle = await Page.QuerySelectorAsync("div");
            await Page.EvaluateFunctionAsync("element => element.style.height = '200px'", elementHandle);
            var box = await elementHandle.BoundingBoxAsync();
            Assert.Equal(new BoundingBox(8, 8, 100, 200), box);
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.boundingBox", "should work with SVG nodes")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWworkWithSVGNodes()
        {
            await Page.SetContentAsync(@"
                <svg xmlns=""http://www.w3.org/2000/svg"" width=""500"" height=""500"">
                  <rect id=""theRect"" x=""30"" y=""50"" width=""200"" height=""300""></rect>
                </svg>
            ");

            var element = await Page.QuerySelectorAsync("#therect");
            var pptrBoundingBox = await element.BoundingBoxAsync();
            var webBoundingBox = await Page.EvaluateFunctionAsync<BoundingBox>(@"e =>
            {
                const rect = e.getBoundingClientRect();
                return { x: rect.x, y: rect.y, width: rect.width, height: rect.height};
            }", element);
            Assert.Equal(webBoundingBox, pptrBoundingBox);
        }
    }
}
