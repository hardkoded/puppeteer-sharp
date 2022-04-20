using System;
using System.Collections.Generic;
using PuppeteerSharp.Media;

namespace PuppeteerSharp
{
    /// <summary>
    /// Options to be used in <see cref="Page.PdfAsync(string, PdfOptions)"/>, <see cref="Page.PdfStreamAsync(PdfOptions)"/> and <see cref="Page.PdfDataAsync(PdfOptions)"/>
    /// </summary>
    public class PdfOptions : IEquatable<PdfOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PuppeteerSharp.PdfOptions"/> class.
        /// </summary>
        public PdfOptions()
        {
        }

        /// <summary>
        /// Scale of the webpage rendering. Defaults to <c>1</c>. Scale amount must be between 0.1 and 2.
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

        /// <summary>
        /// Give any CSS <c>@page</c> size declared in the page priority over what is declared in <c>width</c> and <c>height</c> or <c>format</c> options.
        /// Defaults to <c>false</c>, which will scale the content to fit the paper size.
        /// </summary>
        public bool PreferCSSPageSize { get; set; }

        /// <summary>
        /// Hides default white background and allows generating pdfs with transparency.
        /// </summary>
        public bool OmitBackground { get; set; }

        /// <inheritdoc/>
        public static bool operator ==(PdfOptions left, PdfOptions right)
            => EqualityComparer<PdfOptions>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(PdfOptions left, PdfOptions right) => !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((PdfOptions)obj);
        }

        /// <inheritdoc/>
        public bool Equals(PdfOptions options)
            => options != null &&
                   Scale == options.Scale &&
                   DisplayHeaderFooter == options.DisplayHeaderFooter &&
                   HeaderTemplate == options.HeaderTemplate &&
                   FooterTemplate == options.FooterTemplate &&
                   PrintBackground == options.PrintBackground &&
                   Landscape == options.Landscape &&
                   PageRanges == options.PageRanges &&
                   EqualityComparer<PaperFormat>.Default.Equals(Format, options.Format) &&
                   EqualityComparer<object>.Default.Equals(Width, options.Width) &&
                   EqualityComparer<object>.Default.Equals(Height, options.Height) &&
                   EqualityComparer<MarginOptions>.Default.Equals(MarginOptions, options.MarginOptions) &&
                   PreferCSSPageSize == options.PreferCSSPageSize &&
                   OmitBackground == options.OmitBackground;

        /// <inheritdoc/>
        public override int GetHashCode()
            => -711844102
                ^ Scale.GetHashCode()
                ^ DisplayHeaderFooter.GetHashCode()
                ^ EqualityComparer<string>.Default.GetHashCode(HeaderTemplate)
                ^ EqualityComparer<string>.Default.GetHashCode(FooterTemplate)
                ^ PrintBackground.GetHashCode()
                ^ OmitBackground.GetHashCode()
                ^ Landscape.GetHashCode()
                ^ EqualityComparer<string>.Default.GetHashCode(PageRanges)
                ^ EqualityComparer<PaperFormat>.Default.GetHashCode(Format)
                ^ EqualityComparer<object>.Default.GetHashCode(Width)
                ^ EqualityComparer<object>.Default.GetHashCode(Height)
                ^ EqualityComparer<MarginOptions>.Default.GetHashCode(MarginOptions)
                ^ PreferCSSPageSize.GetHashCode();
    }
}
