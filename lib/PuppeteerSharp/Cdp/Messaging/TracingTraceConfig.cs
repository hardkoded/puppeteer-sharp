namespace PuppeteerSharp.Cdp.Messaging
{
    internal class TracingTraceConfig
    {
        public string[] IncludedCategories { get; set; }

        public string[] ExcludedCategories { get; set; }

        public int? TraceBufferSizeInKb { get; set; }
    }
}
