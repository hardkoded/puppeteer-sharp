using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using PdfSharp.Pdf.IO;
using PuppeteerSharp.Media;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class PdfTests : PuppeteerPageBaseTest
    {
        public PdfTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldBeAbleToSaveFile()
        {
            var outputFile = Path.Combine(BaseDirectory, "output.pdf");
            var fileInfo = new FileInfo(outputFile);

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            await Page.PdfAsync(outputFile);

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
            var document = PdfReader.Open(await Page.PdfStreamAsync(), PdfDocumentOpenMode.ReadOnly);

            Assert.Equal(1, document.Pages.Count);
            Assert.Equal(8.5, TruncateDouble(document.Pages[0].Width.Inch, 1));
            Assert.Equal(11, TruncateDouble(document.Pages[0].Height.Inch, 0));
        }

        [Fact]
        public async Task ShouldSupportSettingCustomFormat()
        {
            var document = PdfReader.Open(await Page.PdfStreamAsync(new PdfOptions
            {
                Format = PaperFormat.A4
            }), PdfDocumentOpenMode.ReadOnly);

            Assert.Equal(1, document.Pages.Count);
            Assert.Equal(8.2, TruncateDouble(document.Pages[0].Width.Inch, 1));
            Assert.Equal(842, document.Pages[0].Height.Point);
        }

        [Fact]
        public async Task ShouldSupportSettingPaperWidthAndHeight()
        {
            var document = PdfReader.Open(await Page.PdfStreamAsync(new PdfOptions
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
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            // Define width and height in CSS pixels.
            var width = (50 * 5) + 1;
            var height = (50 * 5) + 1;
            var document = PdfReader.Open(await Page.PdfStreamAsync(new PdfOptions
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
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            // Define width and height in CSS pixels.
            var width = (50 * 5) + 1;
            var height = (50 * 5) + 1;
            var document = PdfReader.Open(await Page.PdfStreamAsync(new PdfOptions
            {
                Width = width,
                Height = height,
                PageRanges = "1,4-7"
            }));

            Assert.Equal(5, document.Pages.Count);
        }

        [Fact]
        public async Task ShouldThrowIfUnitsAreUnknown()
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await Page.PdfDataAsync(new PdfOptions
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
