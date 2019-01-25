namespace PuppeteerSharp.Messaging
{
    internal class NetworkEmulateNetworkConditionsRequest
    {
        public bool Offline { get; set; }
        public int Latency { get; set; }
        public int DownloadThroughput { get; set; }
        public int UploadThroughput { get; set; }
    }
}
