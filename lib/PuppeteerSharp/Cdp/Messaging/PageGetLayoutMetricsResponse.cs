namespace PuppeteerSharp.Cdp.Messaging
{
    internal class PageGetLayoutMetricsResponse
    {
        public Rect ContentSize { get; set; }

        public Rect CssContentSize { get; set; }

        public LayoutContentSize CssVisualViewport { get; set; }

        public LayoutContentSize LayoutViewport { get; set; }

        public class LayoutContentSize
        {
            public decimal ClientWidth { get; set; }

            public decimal ClientHeight { get; set; }

            public decimal PageX { get; set; }

            public decimal PageY { get; set; }
        }

        public class Rect
        {
            public decimal X { get; set; }

            public decimal Y { get; set; }

            public decimal Width { get; set; }

            public decimal Height { get; set; }
        }
    }
}
