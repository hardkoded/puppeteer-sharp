using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PuppeteerSharp.Media;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PdfTests : PuppeteerPageBaseTest
    {
        public PdfTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact(Timeout = -1)]
        public async Task Usage()
        {
            var outputFile = Path.Combine(BaseDirectory, "Usage.pdf");
            var fileInfo = new FileInfo(outputFile);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            #region PdfAsync

            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions {Headless = true});
            await using var page = await browser.NewPageAsync();
            await page.GoToAsync("http://www.google.com"); // In case of fonts being loaded from a CDN, use WaitUntilNavigation.Networkidle0 as a second param.
            await page.EvaluateExpressionHandleAsync("document.fonts.ready"); // Wait for fonts to be loaded. Omitting this might result in no text rendered in pdf.
            await page.PdfAsync(outputFile);

            #endregion

            Assert.True(File.Exists(outputFile));
        }

        [PuppeteerTest("page.spec.ts", "printing to PDF", "can print to PDF and save to file")]
        [PuppeteerFact]
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

        [PuppeteerTest("page.spec.ts", "printing to PDF", "can print to PDF and stream the result")]
        [PuppeteerFact]
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
            Assert.Equal(new FileInfo(outputFile).Length, stream.Length);
        }

        [PuppeteerFact]
        public void PdfOptionsShouldBeSerializable()
        {
            var pdfOptions = new PdfOptions
            {
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

            var serialized = JsonConvert.SerializeObject(pdfOptions);
            var newPdfOptions = JsonConvert.DeserializeObject<PdfOptions>(serialized);
            Assert.Equal(pdfOptions, newPdfOptions);
        }
    }
}
