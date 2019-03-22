using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PuppeteerSharp.Media;
using Xunit;
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
