namespace PuppeteerSharp.Messaging
{
    internal class PageGetLayoutMetricsResponse
    {
        public LayourContentSize ContentSize { get; set; }

        public class LayourContentSize
        {
            public decimal Width { get; set; }
            public decimal Height { get; set; }
        }
    }
}
