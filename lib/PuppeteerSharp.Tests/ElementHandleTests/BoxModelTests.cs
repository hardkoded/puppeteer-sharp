using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class BoxModelTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.boxModel", "should work")]
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
            var divHandle = (IElementHandle)await frame.EvaluateFunctionHandleAsync(@"() => {
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
            Assert.That(box.Width, Is.EqualTo(6));
            Assert.That(box.Height, Is.EqualTo(7));
            Assert.That(box.Margin[0], Is.EqualTo(new BoxModelPoint
            {
                X = 1 + 4, // frame.left + div.left
                Y = 2 + 5
            }));
            Assert.That(box.Border[0], Is.EqualTo(new BoxModelPoint
            {
                X = 1 + 4 + 3, // frame.left + div.left + div.margin-left
                Y = 2 + 5
            }));
            Assert.That(box.Padding[0], Is.EqualTo(new BoxModelPoint
            {
                X = 1 + 4 + 3 + 1, // frame.left + div.left + div.marginLeft + div.borderLeft
                Y = 2 + 5
            }));
            Assert.That(box.Content[0], Is.EqualTo(new BoxModelPoint
            {
                X = 1 + 4 + 3 + 1 + 2, // frame.left + div.left + div.marginLeft + div.borderLeft + dif.paddingLeft
                Y = 2 + 5
            }));
        }

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.boxModel", "should return null for invisible elements")]
        public async Task ShouldReturnNullForInvisibleElements()
        {
            await Page.SetContentAsync("<div style='display:none'>hi</div>");
            var elementHandle = await Page.QuerySelectorAsync("div");
            Assert.That(await elementHandle.BoxModelAsync(), Is.Null);
        }
    }
}
