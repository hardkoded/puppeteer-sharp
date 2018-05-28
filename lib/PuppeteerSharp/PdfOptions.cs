using PuppeteerSharp.Media;

namespace PuppeteerSharp
{
    /// <summary>
    /// Options to be used in <see cref="Page.PdfAsync(string, PdfOptions)"/>, <see cref="Page.PdfStreamAsync(PdfOptions)"/> and <see cref="Page.PdfDataAsync(PdfOptions)"/>
    /// </summary>
    public class PdfOptions
    {
        /// <summary>
        /// Scale of the webpage rendering. Defaults to <c>1</c>
        /// </summary>
        public decimal Scale { get; set; } = 1;

        /// <summary>
        /// Display header and footer. Defaults to <c>false</c>
        /// </summary>
        public bool DisplayHeaderFooter { get; set; }

        /// <summary>
        /// HTML template for the print header. Should be valid HTML markup with following classes used to inject printing values into them:
        ///   <c>date</c> - formatted print date
        ///   <c>title</c> - document title
        ///   <c>url</c> - document location
        ///   <c>pageNumber</c> - current page number
        ///   <c>totalPages</c> - total pages in the document
        /// </summary>
        public string HeaderTemplate { get; set; } = string.Empty;

        /// <summary>
        /// HTML template for the print footer. Should be valid HTML markup with following classes used to inject printing values into them:
        ///   <c>date</c> - formatted print date
        ///   <c>title</c> - document title
        ///   <c>url</c> - document location
        ///   <c>pageNumber</c> - current page number
        ///   <c>totalPages</c> - total pages in the document
        /// </summary>
        public string FooterTemplate { get; set; } = string.Empty;

        /// <summary>
        /// Print background graphics. Defaults to <c>false</c>
        /// </summary>
        public bool PrintBackground { get; set; }

        /// <summary>
        /// Paper orientation.. Defaults to <c>false</c>
        /// </summary>
        public bool Landscape { get; set; }

        /// <summary>
        /// Paper ranges to print, e.g., <c>1-5, 8, 11-13</c>. Defaults to the empty string, which means print all pages
        /// </summary>
        public string PageRanges { get; set; } = string.Empty;

        /// <summary>
        /// Paper format. If set, takes priority over <see cref="Width"/> and <see cref="Height"/>
        /// </summary>
        public PaperFormat Format { get; set; }

        /// <summary>
        /// Paper width, accepts values labeled with units
        /// </summary>
        public object Width { get; set; }

        /// <summary>
        /// Paper height, accepts values labeled with units
        /// </summary>
        public object Height { get; set; }

        /// <summary>
        /// Paper margins, defaults to none
        /// </summary>
        public MarginOptions MarginOptions { get; set; } = new MarginOptions();
    }
}