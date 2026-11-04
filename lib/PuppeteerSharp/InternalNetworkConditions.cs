namespace PuppeteerSharp
{
    internal class InternalNetworkConditions : NetworkConditions
    {
        public new double Download { get; set; } = -1;

        public new double Upload { get; set; } = -1;

        public new double Latency { get; set; }
    }
}
