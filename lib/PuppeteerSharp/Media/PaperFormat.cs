namespace PuppeteerSharp.Media
{
    public class PaperFormat
    {
        internal decimal Width { get; set; }
        internal decimal Height { get; set; }

        private PaperFormat(decimal width, decimal height)
        {
            Width = width;
            Height = height;
        }

        public static readonly PaperFormat Letter = new PaperFormat(8.5m, 11);
        public static readonly PaperFormat Legal = new PaperFormat(8.5m, 14);
        public static readonly PaperFormat Tabloid = new PaperFormat(11, 17);
        public static readonly PaperFormat Ledger = new PaperFormat(17, 11);
        public static readonly PaperFormat A0 = new PaperFormat(33.1m, 46.8m);
        public static readonly PaperFormat A1 = new PaperFormat(23.4m, 33.1m);
        public static readonly PaperFormat A2 = new PaperFormat(16.5m, 23.4m);
        public static readonly PaperFormat A3 = new PaperFormat(11.7m, 16.5m);
        public static readonly PaperFormat A4 = new PaperFormat(8.27m, 11.7m);
        public static readonly PaperFormat A5 = new PaperFormat(5.83m, 8.27m);
        public static readonly PaperFormat A6 = new PaperFormat(4.13m, 5.83m);
    }
}