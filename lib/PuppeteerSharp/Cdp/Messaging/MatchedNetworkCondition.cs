namespace PuppeteerSharp.Cdp.Messaging
{
    internal class MatchedNetworkCondition
    {
        public string UrlPattern { get; set; }

        public double Latency { get; set; }

        public double DownloadThroughput { get; set; }

        public double UploadThroughput { get; set; }
    }
}
