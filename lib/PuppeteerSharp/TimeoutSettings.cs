namespace PuppeteerSharp
{
    internal class TimeoutSettings
    {
        private int? _defaultNavigationTimeout;

        public int NavigationTimeout
        {
            get => _defaultNavigationTimeout ?? Timeout;
            set => _defaultNavigationTimeout = value;
        }

        public int Timeout { get; set; } = 30000;
    }
}
