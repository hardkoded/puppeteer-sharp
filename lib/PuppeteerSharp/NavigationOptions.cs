namespace PuppeteerSharp
{
    public class NavigationOptions
    {
        public int? Timeout { get; internal set; }
        public string[] WaitUntil { get; internal set; }
    }
}