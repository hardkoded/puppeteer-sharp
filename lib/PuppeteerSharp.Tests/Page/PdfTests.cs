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
            _browser = PuppeteerSharp.Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions,
                                                            TestConstants.ChromiumRevision).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _browser.CloseAsync().GetAwaiter().GetResult();
        }

        [Fact]
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

        [Fact]
        public async Task ShouldDefaultToPrintInLetterFormat()
        {
            var page = await _browser.NewPageAsync();

            var document = PdfReader.Open(await page.PdfAsync(), PdfDocumentOpenMode.ReadOnly);

            Assert.Equal(1, document.Pages.Count);
            Assert.Equal(8.5, TruncateDouble(document.Pages[0].Width.Inch, 1));
            Assert.Equal(11, TruncateDouble(document.Pages[0].Width.Inch, 0));
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public async Task ShouldPrintMultiplePages()
        {
            var page = await _browser.NewPageAsync();
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            // Define width and height in CSS pixels.
            var width = 50 * 5 + 1;
            var height = 50 * 5 + 1;
            var document = PdfReader.Open(await page.PdfAsync(new { width, height }));
            Assert.Equal(8, document.Pages.Count);
            Assert.Equal(CssPixelsToInches(width), TruncateDouble(document.Pages[0].Width.Inch, 0));
            Assert.Equal(CssPixelsToInches(height), TruncateDouble(document.Pages[0].Width.Inch, 0));
        }

        [Fact]
        public async Task ShouldSupportPageRanges()
        {
            var page = await _browser.NewPageAsync();
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            // Define width and height in CSS pixels.
            var width = 50 * 5 + 1;
            var height = 50 * 5 + 1;
            var document = PdfReader.Open(await page.PdfAsync(new { width, height, pageRanges = "1,4-7" }));
            Assert.Equal(5, document.Pages.Count);
        }

        [Fact]
        public async Task ShowThrowFormatIsUnknown()
        {
            var page = await _browser.NewPageAsync();
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await page.PdfAsync(new { format = "something" });
            });

            Assert.Equal("Unknown paper format", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowIfUnitsAreUnknown()
        {
            var page = await _browser.NewPageAsync();
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await page.PdfAsync(new { width = "10em" });
            });

            Assert.Equal("Failed to parse parameter value", exception.Message);
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
