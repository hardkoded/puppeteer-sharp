namespace PuppeteerSharp
{
    public class WaitForSelectorOptions
    {
        public int Timeout { get; set; } = 30_000;
        public bool Visible { get; set; }
        public bool Hidden { get; set; }
    }
}