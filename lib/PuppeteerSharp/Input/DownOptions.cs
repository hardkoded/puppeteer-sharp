namespace PuppeteerSharp.Input
{
    /// <summary>
    /// options to use with <see cref="IKeyboard.DownAsync(string, DownOptions)"/>.
    /// </summary>
    public class DownOptions
    {
        /// <summary>
        /// If specified, generates an input event with this text.
        /// </summary>
        public string Text { get; set; }
    }
}
