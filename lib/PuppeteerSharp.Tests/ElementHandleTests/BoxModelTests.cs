using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class BoxModelTests : PuppeteerPageBaseTest
    {
        public BoxModelTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.boxModel", "should work")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/resetcss.html");

            // Step 1: Add Frame and position it absolutely.
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.ServerUrl + "/resetcss.html");
            await Page.EvaluateExpressionAsync(@"
              const frame = document.querySelector('#frame1');
              frame.style = `
                position: absolute;
                left: 1px;
                top: 2px;
              `;");

            // Step 2: Add div and position it absolutely inside frame.
            var frame = Page.FirstChildFrame();
            var divHandle = (ElementHandle)await frame.EvaluateFunctionHandleAsync(@"() => {
              const div = document.createElement('div');
              document.body.appendChild(div);
              div.style = `
                box-sizing: border-box;
                position: absolute;
                border-left: 1px solid black;
                padding-left: 2px;
                margin-left: 3px;
                left: 4px;
                top: 5px;
                width: 6px;
                height: 7px;
              `;
              return div;
            }");

            // Step 3: query div's boxModel and assert box values.
            var box = await divHandle.BoxModelAsync();
            Assert.Equal(6, box.Width);
            Assert.Equal(7, box.Height);
            Assert.Equal(new BoxModelPoint
            {
                X = 1 + 4, // frame.left + div.left
                Y = 2 + 5
            }, box.Margin[0]);
            Assert.Equal(new BoxModelPoint
            {
                X = 1 + 4 + 3, // frame.left + div.left + div.margin-left
                Y = 2 + 5
            }, box.Border[0]);
            Assert.Equal(new BoxModelPoint
            {
                X = 1 + 4 + 3 + 1, // frame.left + div.left + div.marginLeft + div.borderLeft
                Y = 2 + 5
            }, box.Padding[0]);
            Assert.Equal(new BoxModelPoint
            {
                X = 1 + 4 + 3 + 1 + 2, // frame.left + div.left + div.marginLeft + div.borderLeft + dif.paddingLeft
                Y = 2 + 5
            }, box.Content[0]);
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.boxModel", "should return null for invisible elements")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldReturnNullForInvisibleElements()
        {
            await Page.SetContentAsync("<div style='display:none'>hi</div>");
            var elementHandle = await Page.QuerySelectorAsync("div");
            Assert.Null(await elementHandle.BoxModelAsync());
        }
    }
}
