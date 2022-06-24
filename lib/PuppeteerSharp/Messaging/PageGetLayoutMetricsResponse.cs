namespace PuppeteerSharp.Messaging
{
    internal class PageGetLayoutMetricsResponse
    {
        public LayoutContentSize ContentSize { get; set; }

        public LayoutContentSize CssLayoutViewport { get; set; }

        public LayoutContentSize LayoutViewport { get; set; }

        public class LayoutContentSize
        {
            public decimal ClientWidth { get; set; }

            public decimal ClientHeight { get; set; }

            public decimal PageX { get; set; }

            public decimal PageY { get; set; }
        }
    }
}
