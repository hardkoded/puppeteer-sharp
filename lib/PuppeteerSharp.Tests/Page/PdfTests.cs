using System;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Puppeteer;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    public class PdfTests : IDisposable
    {
        private string _baseDirectory;
        Browser _browser;

        public PdfTests()
        {
            _baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "test-pdf");
            var dirInfo = new DirectoryInfo(_baseDirectory);

            if (dirInfo.Exists)
            {
                dirInfo.Delete(true);
            }

            dirInfo.Create();
            _browser = PuppeteerSharp.Puppeteer.LaunchAsync(PuppeteerLaunchTests.DefaultBrowserOptions,
                                                            PuppeteerLaunchTests.ChromiumRevision).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _browser.CloseAsync().GetAwaiter().GetResult();
        }

        public async Task ShouldBeAbleToSaveFile()
        {
            var outputFile = Path.Combine(_baseDirectory, "assets", "output.pdf");
            var fileInfo = new FileInfo(outputFile);

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            var page = await _browser.NewPageAsync();
            await page.Pdf(new
            {
                path = outputFile
            });

            Assert.True(new FileInfo(outputFile).Length > 0);

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }

        /*
        public async Task ShouldDefaultToPrintininLetterFormat()
        {
            
            var pages = await getPDFPages(await page.pdf());
            expect(pages.length).toBe(1);
            expect(pages[0].width).toBeCloseTo(8.5, 2);
            expect(pages[0].height).toBeCloseTo(11, 2);
        }

        public async Task ShouldSupportSettingCustomFormat()
        {
            const pages = await getPDFPages(await page.pdf({
            format: 'a4'
            }));
            expect(pages.length).toBe(1);
            expect(pages[0].width).toBeCloseTo(8.27, 1);
            expect(pages[0].height).toBeCloseTo(11.7, 1);
        }

        public async Task ShouldSupportSettingPaperWidthAndHeight()
        {
            const pages = await getPDFPages(await page.pdf({
            width: '10in',
                height: '10in',
              }));
            expect(pages.length).toBe(1);
            expect(pages[0].width).toBeCloseTo(10, 2);
            expect(pages[0].height).toBeCloseTo(10, 2);
        }

        public async Task ShouldPrintMultiplePages()
        {
            await page.goto(server.PREFIX + '/grid.html');
            // Define width and height in CSS pixels.
            const width = 50 * 5 + 1;
            const height = 50 * 5 + 1;
            const pages = await getPDFPages(await page.pdf({ width, height}));
            expect(pages.length).toBe(8);
            expect(pages[0].width).toBeCloseTo(cssPixelsToInches(width), 2);
            expect(pages[0].height).toBeCloseTo(cssPixelsToInches(height), 2);
        }

        public async Task ShouldSupportPageRanges()
        {
            await page.goto(server.PREFIX + '/grid.html');
            // Define width and height in CSS pixels.
            const width = 50 * 5 + 1;
            const height = 50 * 5 + 1;
            const pages = await getPDFPages(await page.pdf({ width, height, pageRanges: '1,4-7'}));
            expect(pages.length).toBe(5);
        }

        public async Task ShowThrowFormatIsUnknown()
        {

            let error = null;
            try
            {
                await getPDFPages(await page.pdf({
                format: 'something'
                }));
            }
            catch (e)
            {
                error = e;
            }
            expect(error).toBeTruthy();
            expect(error.message).toContain('Unknown paper format');
        }

        public async Task ShouldThrowIfUnitsAreUnknown()
        {

            let error = null;
            try
            {
                await getPDFPages(await page.pdf({
                width: '10em',
          height: '10em',
        }));
            }
            catch (e)
            {
                error = e;
            }
            expect(error).toBeTruthy();
            expect(error.message).toContain('Failed to parse parameter value');
        }
*/
    }
}
