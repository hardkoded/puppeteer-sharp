namespace PuppeteerSharp.Input
{
    /// <summary>
    /// options to use when pressing a key.
    /// </summary>
    /// <seealso cref="IKeyboard.PressAsync(string, PressOptions)"/>
    /// <seealso cref="IElementHandle.PressAsync(string, PressOptions)"/>
    public class PressOptions : DownOptions
    {
        /// <summary>
        /// Time to wait between <c>keydown</c> and <c>keyup</c> in milliseconds. Defaults to 0.
        /// </summary>
        public int? Delay { get; set; }
    }
}
