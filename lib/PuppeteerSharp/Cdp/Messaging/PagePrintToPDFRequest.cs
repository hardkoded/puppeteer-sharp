namespace PuppeteerSharp.Cdp.Messaging
{
    internal class PagePrintToPDFRequest
    {
        public bool Landscape { get; set; }

        public bool DisplayHeaderFooter { get; set; }

        public string HeaderTemplate { get; set; }

        public string FooterTemplate { get; set; }

        public bool PrintBackground { get; set; }

        public decimal Scale { get; set; }

        public decimal PaperWidth { get; set; }

        public decimal PaperHeight { get; set; }

        public decimal MarginTop { get; set; }

        public decimal MarginBottom { get; set; }

        public decimal MarginLeft { get; set; }

        public decimal MarginRight { get; set; }

        public string PageRanges { get; set; }

        public bool PreferCSSPageSize { get; set; }

        public string TransferMode { get; set; }

        public bool GenerateTaggedPDF { get; set; }

        public bool GenerateDocumentOutline { get; set; }
    }
}
