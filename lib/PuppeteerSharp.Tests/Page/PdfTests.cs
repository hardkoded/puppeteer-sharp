using System;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Puppeteer;
using Xunit;
using PdfSharp.Pdf.IO;

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
            await page.PdfAsync(new
            {
                path = outputFile
            });

            Assert.True(new FileInfo(outputFile).Length > 0);

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }

        public async Task ShouldDefaultToPrintInLetterFormat()
        {
            var page = await _browser.NewPageAsync();

            var document = PdfReader.Open(await page.PdfAsync(), PdfDocumentOpenMode.ReadOnly);

            Assert.Equal(1, document.Pages.Count);
            Assert.Equal(8.5, TruncateDouble(document.Pages[0].Width.Inch, 1));
            Assert.Equal(11, TruncateDouble(document.Pages[0].Width.Inch, 0));
        }

        public async Task ShouldSupportSettingCustomFormat()
        {
            var page = await _browser.NewPageAsync();

            var document = PdfReader.Open(await page.PdfAsync(new
            {
                format = "a4"
            }), PdfDocumentOpenMode.ReadOnly);

            Assert.Equal(1, document.Pages.Count);
            Assert.Equal(8.27, TruncateDouble(document.Pages[0].Width.Inch, 2));
            Assert.Equal(11.7, TruncateDouble(document.Pages[0].Width.Inch, 1));
        }

        public async Task ShouldSupportSettingPaperWidthAndHeight()
        {
            var page = await _browser.NewPageAsync();

            var document = PdfReader.Open(await page.PdfAsync(new
            {
                width = "10in",
                height = "10in"
            }), PdfDocumentOpenMode.ReadOnly);

            Assert.Equal(1, document.Pages.Count);
            Assert.Equal(10, TruncateDouble(document.Pages[0].Width.Inch, 0));
            Assert.Equal(10, TruncateDouble(document.Pages[0].Width.Inch, 0));
        }
        /*


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
        public double TruncateDouble(double value, int precision)
        {
            double step = Math.Pow(10, precision);
            double tmp = Math.Truncate(step * value);
            return tmp / step;
        }
    }
}
