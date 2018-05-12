using PuppeteerSharp.Media;

namespace PuppeteerSharp
{
    public class PdfOptions
    {
        public PdfOptions()
        {
            Scale = 1;
            MarginOptions = new MarginOptions();
            HeaderTemplate = string.Empty;
            FooterTemplate = string.Empty;
            PageRanges = string.Empty;
            Format = string.Empty;
        }

        public decimal Scale { get; set; }
        public bool DisplayHeaderFooter { get; set; }
        public string HeaderTemplate { get; set; }
        public string FooterTemplate { get; set; }
        public bool PrintBackground { get; set; }
        public bool Landscape { get; set; }
        public string PageRanges { get; set; }
        public string Format { get; set; }
        public object Width { get; set; }
        public object Height { get; set; }
        public MarginOptions MarginOptions { get; set; }
    }
}