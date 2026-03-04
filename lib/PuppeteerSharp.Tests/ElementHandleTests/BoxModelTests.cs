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
            var frame = await Page.FirstChildFrameAsync();
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

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.boxModel", "should correctly compute box model with offsets")]
        public async Task ShouldCorrectlyComputeBoxModelWithOffsets()
        {
            var border = 10;
            var padding = 11;
            var margin = 12;
            var width = 200;
            var height = 100;
            var verticalOffset = 100;
            var horizontalOffset = 100;

            await Page.SetContentAsync(
                $"<div style=\"position:absolute; left: {horizontalOffset}px; top: {verticalOffset}px; width: {width}px; height: {height}px; border: {border}px solid green; padding: {padding}px; margin: {margin}px;\" id=\"box\"></div>");

            var element = await Page.QuerySelectorAsync("#box");
            var boxModel = await element.BoxModelAsync();

            BoxModelPoint[] MakeQuad(BoxModelPoint topLeft, BoxModelPoint bottomRight)
            {
                return new[]
                {
                    new BoxModelPoint { X = topLeft.X, Y = topLeft.Y },
                    new BoxModelPoint { X = bottomRight.X, Y = topLeft.Y },
                    new BoxModelPoint { X = bottomRight.X, Y = bottomRight.Y },
                    new BoxModelPoint { X = topLeft.X, Y = bottomRight.Y },
                };
            }

            Assert.That(boxModel.Width, Is.EqualTo(width + (padding * 2) + (border * 2)));
            Assert.That(boxModel.Height, Is.EqualTo(height + (padding * 2) + (border * 2)));

            Assert.That(boxModel.Content, Is.EqualTo(MakeQuad(
                new BoxModelPoint { X = horizontalOffset + padding + margin + border, Y = verticalOffset + padding + margin + border },
                new BoxModelPoint { X = horizontalOffset + width + padding + margin + border, Y = verticalOffset + height + padding + margin + border })));

            Assert.That(boxModel.Padding, Is.EqualTo(MakeQuad(
                new BoxModelPoint { X = horizontalOffset + margin + border, Y = verticalOffset + margin + border },
                new BoxModelPoint { X = horizontalOffset + width + (padding * 2) + margin + border, Y = verticalOffset + (padding * 2) + height + margin + border })));

            Assert.That(boxModel.Border, Is.EqualTo(MakeQuad(
                new BoxModelPoint { X = horizontalOffset + margin, Y = verticalOffset + margin },
                new BoxModelPoint { X = horizontalOffset + width + (padding * 2) + margin + (border * 2), Y = verticalOffset + (padding * 2) + height + margin + (border * 2) })));

            Assert.That(boxModel.Margin, Is.EqualTo(MakeQuad(
                new BoxModelPoint { X = horizontalOffset, Y = verticalOffset },
                new BoxModelPoint { X = horizontalOffset + width + (padding * 2) + (margin * 2) + (border * 2), Y = verticalOffset + (padding * 2) + height + (margin * 2) + (border * 2) })));
        }
    }
}
