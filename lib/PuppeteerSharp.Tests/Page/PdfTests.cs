using System;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Puppeteer;
using Xunit;
using PdfSharp.Pdf.IO;

namespace PuppeteerSharp.Tests.Page
{
    public class PdfTests : PuppeteerBaseTest
    {

        [Fact]
        public async Task ShouldBeAbleToSaveFile()
        {
            var outputFile = Path.Combine(BaseDirectory, "output.pdf");
            var fileInfo = new FileInfo(outputFile);

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            var page = await Browser.NewPageAsync();
            await page.PdfAsync(outputFile);

            fileInfo = new FileInfo(outputFile);
            Assert.True(new FileInfo(outputFile).Length > 0);

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }

        [Fact]
        public async Task ShouldDefaultToPrintInLetterFormat()
        {
            var page = await Browser.NewPageAsync();

            var document = PdfReader.Open(await page.PdfStreamAsync(), PdfDocumentOpenMode.ReadOnly);

            Assert.Equal(1, document.Pages.Count);
            Assert.Equal(8.5, TruncateDouble(document.Pages[0].Width.Inch, 1));
            Assert.Equal(11, TruncateDouble(document.Pages[0].Height.Inch, 0));
        }

        [Fact]
        public async Task ShouldSupportSettingCustomFormat()
        {
            var page = await Browser.NewPageAsync();

            var document = PdfReader.Open(await page.PdfStreamAsync(new PdfOptions
            {
                Format = "a4"
            }), PdfDocumentOpenMode.ReadOnly);

            Assert.Equal(1, document.Pages.Count);
            Assert.Equal(8.2, TruncateDouble(document.Pages[0].Width.Inch, 1));
            Assert.Equal(842, document.Pages[0].Height.Point);
        }

        [Fact]
        public async Task ShouldSupportSettingPaperWidthAndHeight()
        {
            var page = await Browser.NewPageAsync();

            var document = PdfReader.Open(await page.PdfStreamAsync(new PdfOptions
            {
                Width = "10in",
                Height = "10in"
            }), PdfDocumentOpenMode.ReadOnly);

            Assert.Equal(1, document.Pages.Count);
            Assert.Equal(10, TruncateDouble(document.Pages[0].Width.Inch, 0));
            Assert.Equal(10, TruncateDouble(document.Pages[0].Height.Inch, 0));
        }

        [Fact]
        public async Task ShouldPrintMultiplePages()
        {
            var page = await Browser.NewPageAsync();
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            // Define width and height in CSS pixels.
            var width = 50 * 5 + 1;
            var height = 50 * 5 + 1;
            var document = PdfReader.Open(await page.PdfStreamAsync(new PdfOptions
            {
                Width = width,
                Height = height
            }));

            Assert.Equal(8, document.Pages.Count);
            Assert.Equal(CssPixelsToInches(width), TruncateDouble(document.Pages[0].Width.Inch, 0));
            Assert.Equal(CssPixelsToInches(height), TruncateDouble(document.Pages[0].Height.Inch, 0));
        }

        [Fact]
        public async Task ShouldSupportPageRanges()
        {
            var page = await Browser.NewPageAsync();
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            // Define width and height in CSS pixels.
            var width = 50 * 5 + 1;
            var height = 50 * 5 + 1;
            var document = PdfReader.Open(await page.PdfStreamAsync(new PdfOptions
            {
                Width = width,
                Height = height,
                PageRanges = "1,4-7"
            }));

            Assert.Equal(5, document.Pages.Count);
        }

        [Fact]
        public async Task ShowThrowFormatIsUnknown()
        {
            var page = await Browser.NewPageAsync();
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await page.PdfStreamAsync(new PdfOptions
                {
                    Format = "something"
                });
            });

            Assert.Equal("Unknown paper format", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowIfUnitsAreUnknown()
        {
            var page = await Browser.NewPageAsync();
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await page.PdfStreamAsync(new PdfOptions
                {
                    Width = "10em"
                });
            });

            Assert.Contains("Failed to parse parameter value", exception.Message);
        }

        private double TruncateDouble(double value, int precision)
        {
            double step = Math.Pow(10, precision);
            double tmp = Math.Truncate(step * value);
            return tmp / step;
        }

        private double CssPixelsToInches(int pixels)
        {
            return pixels / 96;
        }
    }
}
