namespace PuppeteerSharp.Input
{
    public class PressOptions : DownOptions
    {
        /// <summary>
        /// Time to wait between <c>keydown</c> and <c>keyup</c> in milliseconds. Defaults to 0.
        /// </summary>
        public int? Delay { get; set; }
    }
}