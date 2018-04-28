namespace PuppeteerSharp
{
    public class ClickOptions
    {
        public int? Delay { get; set; }
        public int ClickCount { get; set; } = 1;
        public string Button { get; set; } = "left";
    }
}