namespace PuppeteerSharp.Media
{
    /// <summary>
    /// Paper format.
    /// </summary>
    /// <seealso cref="PdfOptions.Format"/>
    public record PaperFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaperFormat"/> class.
        /// Page width and height in inches.
        /// </summary>
        /// <param name="width">Page width in inches.</param>
        /// <param name="height">Page height in inches.</param>
        public PaperFormat(decimal width, decimal height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Letter: 8.5 inches x 11 inches.
        /// </summary>
        public static PaperFormat Letter => new PaperFormat(8.5m, 11);

        /// <summary>
        /// Legal: 8.5 inches by 14 inches.
        /// </summary>
        public static PaperFormat Legal => new PaperFormat(8.5m, 14);

        /// <summary>
        /// Tabloid: 11 inches by 17 inches.
        /// </summary>
        public static PaperFormat Tabloid => new PaperFormat(11, 17);

        /// <summary>
        /// Ledger: 17 inches by 11 inches.
        /// </summary>
        public static PaperFormat Ledger => new PaperFormat(17, 11);

        /// <summary>
        /// A0: 33.1102 inches by 46.811 inches.
        /// </summary>
        public static PaperFormat A0 => new PaperFormat(33.1102m, 46.811m);

        /// <summary>
        /// A1: 23.3858 inches by 33.1102 inches.
        /// </summary>
        public static PaperFormat A1 => new PaperFormat(23.3858m, 33.1102m);

        /// <summary>
        /// A2: 16.5354 inches by 23.3858 inches.
        /// </summary>
        public static PaperFormat A2 => new PaperFormat(16.5354m, 23.3858m);

        /// <summary>
        /// A3: 11.6929 inches by 16.5354 inches.
        /// </summary>
        public static PaperFormat A3 => new PaperFormat(11.6929m, 16.5354m);

        /// <summary>
        /// A4: 8.2677 inches by 11.6929 inches.
        /// </summary>
        public static PaperFormat A4 => new PaperFormat(8.2677m, 11.6929m);

        /// <summary>
        /// A5: 5.8268 inches by 8.2677 inches.
        /// </summary>
        public static PaperFormat A5 => new PaperFormat(5.8268m, 8.2677m);

        /// <summary>
        /// A6: 4.1339 inches by 5.8268 inches.
        /// </summary>
        public static PaperFormat A6 => new PaperFormat(4.1339m, 5.8268m);

        /// <summary>
        /// Page width in inches.
        /// </summary>
        /// <value>The width.</value>
        public decimal Width { get; set; }

        /// <summary>
        /// Page height in inches.
        /// </summary>
        /// <value>The Height.</value>
        public decimal Height { get; set; }
    }
}
