using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Media;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PdfTests : PuppeteerPageBaseTest
    {
        [Test]
        public async Task Usage()
        {
            var outputFile = Path.Combine(BaseDirectory, "Usage.pdf");
            var fileInfo = new FileInfo(outputFile);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            #region pdfasync_example

            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            await using var page = await browser.NewPageAsync();
            await page.GoToAsync("http://www.google.com"); // In case of fonts being loaded from a CDN, use WaitUntilNavigation.Networkidle0 as a second param.
            await page.EvaluateExpressionHandleAsync("document.fonts.ready"); // Wait for fonts to be loaded. Omitting this might result in no text rendered in pdf.
            await page.PdfAsync(outputFile);

            #endregion

            Assert.That(File.Exists(outputFile), Is.True);
        }

        [Test, PuppeteerTest("pdf.spec", "Page.pdf", "can print to PDF and save to file")]
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
            Assert.That(new FileInfo(outputFile).Length, Is.GreaterThan(0));
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }

        [Test, PuppeteerTest("pdf.spec", "Page.pdf", "can print to PDF and stream the result")]
        public async Task CanPrintToPDFAndStreamTheResult()
        {
            // We test this differently compared to puppeteer.
            // We will compare that we can get to the same file using both PDF methods
            var outputFile = Path.Combine(BaseDirectory, "output.pdf");
            var fileInfo = new FileInfo(outputFile);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            await Page.PdfAsync(outputFile);

            var stream = await Page.PdfStreamAsync();

            // Firefox in Linux might generate and of by one result here.
            // If the difference is less than 2 bytes is good
            Assert.That(Math.Abs(new FileInfo(outputFile).Length - stream.Length), Is.LessThan(2));
        }

        [Test, PuppeteerTest("pdf.spec", "Page.pdf", "can print to PDF with accessible")]
        public async Task CanPrintToPdfWithAccessible()
        {
            // We test this differently compared to puppeteer.
            // We will compare that we can get to the same file using both PDF methods
            var outputFile = Path.Combine(BaseDirectory, "output.pdf");
            var fileInfo = new FileInfo(outputFile);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            var accessibleOutputFile = Path.Combine(BaseDirectory, "output-accessible.pdf");
            fileInfo = new FileInfo(accessibleOutputFile);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            await Page.GoToAsync(TestConstants.ServerUrl + "/pdf.html");
            await Page.PdfAsync(outputFile, new PdfOptions { Tagged = false });
            await Page.PdfAsync(accessibleOutputFile, new PdfOptions { Tagged = true });

            Assert.That(new FileInfo(accessibleOutputFile).Length, Is.GreaterThan(new FileInfo(outputFile).Length));
        }

        [Test, PuppeteerTest("pdf.spec", "Page.pdf", "can print to PDF with outline")]
        public async Task CanPrintToPdfWithOutline()
        {
            var outputFile = Path.Combine(BaseDirectory, "output.pdf");
            var outputFileOutlined = Path.Combine(BaseDirectory, "output-outlined.pdf");
            var fileInfo = new FileInfo(outputFile);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            fileInfo = new FileInfo(outputFileOutlined);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            await Page.GoToAsync(TestConstants.ServerUrl + "/pdf.html");
            await Page.PdfAsync(outputFile, new PdfOptions { Tagged = false });
            await Page.PdfAsync(outputFileOutlined, new PdfOptions { Tagged = true });

            Assert.That(new FileInfo(outputFileOutlined).Length, Is.GreaterThan(new FileInfo(outputFile).Length));
        }

        [Test]
        public void PdfOptionsShouldBeSerializable()
        {
            var pdfOptions = new PdfOptions
            {
                Width = 100,
                Height = 100,
                Format = PaperFormat.A4,
                DisplayHeaderFooter = true,
                MarginOptions = new MarginOptions
                {
                    Top = "20px",
                    Right = "20px",
                    Bottom = "40px",
                    Left = "20px"
                },
                FooterTemplate = "<div id=\"footer-template\" style=\"font-size:10px !important; color:#808080; padding-left:10px\">- <span class=\"pageNumber\"></span> - </div>"
            };

            var serialized = JsonSerializer.Serialize(pdfOptions);
            var newPdfOptions = JsonSerializer.Deserialize<PdfOptions>(serialized);
            Assert.That(newPdfOptions, Is.EqualTo(pdfOptions));
        }

        [Test]
        public void ConvertPrintParameterToInchesTests()
        {
            Assert.That(CdpPage.ConvertPrintParameterToInches("10"), Is.EqualTo(10m / 96));
            Assert.That(CdpPage.ConvertPrintParameterToInches("10px"), Is.EqualTo(10m / 96));
            Assert.That(CdpPage.ConvertPrintParameterToInches("0"), Is.EqualTo(0));
            Assert.That(CdpPage.ConvertPrintParameterToInches("0px"), Is.EqualTo(0));
            Assert.That(CdpPage.ConvertPrintParameterToInches("10in"), Is.EqualTo(10));
        }
    }
}
