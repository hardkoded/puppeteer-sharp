using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class SetContentTests : PuppeteerBaseTest
    {
        const string expectedOutput = "<html><head></head><body><div>hello</div></body></html>";

        [Fact]
        public async Task ShouldWork()
        {
            var page = await Browser.NewPageAsync();

            await page.SetContentAsync("<div>hello</div>");
            var result = await page.GetContentAsync();

            Assert.Equal(expectedOutput, result);
        }

        [Fact]
        public async Task ShouldWorkWithDoctype()
        {
            var page = await Browser.NewPageAsync();
            const string doctype = "<!DOCTYPE html>";

            await page.SetContentAsync($"{doctype}<div>hello</div>");
            var result = await page.GetContentAsync();

            Assert.Equal($"{doctype}{expectedOutput}", result);
        }

        [Fact]
        public async Task ShouldWorkWithHtml4Doctype()
        {
            var page = await Browser.NewPageAsync();
            const string doctype = "<!DOCTYPE html PUBLIC \" -//W3C//DTD HTML 4.01//EN\" " +
        "\"http://www.w3.org/TR/html4/strict.dtd\">";

            await page.SetContentAsync($"{doctype}<div>hello</div>");
            var result = await page.GetContentAsync();

            Assert.Equal($"{doctype}{expectedOutput}", result);
        }

        /*
        it('should work', async({ page, server}) => {
      await page.setContent('<div>hello</div>');
        const result = await page.content();
        expect(result).toBe(expectedOutput);
    });
    it('should work with doctype', async({ page, server}) => {
      const doctype = '<!DOCTYPE html>';
    await page.setContent(`${ doctype}<div>hello</div>`);
      const result = await page.content();
    expect(result).toBe(`${ doctype}${expectedOutput
}`);
    });
    it('should work with HTML 4 doctype', async({ page, server}) => {
      const doctype = '<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01//EN" ' +
        '"http://www.w3.org/TR/html4/strict.dtd">';
await page.setContent(`${ doctype}<div>hello</div>`);
      const result = await page.content();
expect(result).toBe(`${ doctype}${expectedOutput}`);
    });
    */
    }
}
