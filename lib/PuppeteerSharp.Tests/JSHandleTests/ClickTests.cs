using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    public class ClickTests : PuppeteerPageBaseTest
    {
        public ClickTests(): base()
        {
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.click", "should work")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWork()
        {
            var clicks = new List<BoxModelPoint>();

            await Page.ExposeFunctionAsync("reportClick", (int x, int y) =>
            {
                clicks.Add(new BoxModelPoint { X = x, Y = y });

                return true;
            });

            await Page.EvaluateExpressionAsync(@"document.body.style.padding = '0';
                document.body.style.margin = '0';
                document.body.innerHTML = '<div style=""cursor: pointer; width: 120px; height: 60px; margin: 30px; padding: 15px;""></div>';
                document.body.addEventListener('click', e => {
                    window.reportClick(e.clientX, e.clientY);
                });");

            var divHandle = await Page.QuerySelectorAsync("div");

            await divHandle.ClickAsync();
            await divHandle.ClickAsync(new Input.ClickOptions { OffSet = new Offset(10, 15) });

            await TestUtils.ShortWaitForCollectionToHaveAtLeastNElementsAsync(clicks, 2);

            // margin + middle point offset
            Assert.Equal(45 + 60, clicks[0].X);
            Assert.Equal(45 + 30, clicks[0].Y);

            // margin + offset
            Assert.Equal(30 + 10, clicks[1].X);
            Assert.Equal(30 + 15, clicks[1].Y);
        }
    }
}
