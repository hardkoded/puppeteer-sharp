namespace PuppeteerSharp
{
    public class WaitForFunctionOptions
    {
        public int Timeout { get; set; } = 30_000;
        public string Polling { get; set; } = "raf";
    }
}