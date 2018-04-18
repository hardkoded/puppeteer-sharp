namespace PuppeteerSharp
{
    public class NavigationOptions
    {
        public int? Timeout { get; set; }
        public WaitUntilNavigation[] WaitUntil { get; set; }
    }
}