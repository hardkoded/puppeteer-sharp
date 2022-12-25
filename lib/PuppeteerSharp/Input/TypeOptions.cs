namespace PuppeteerSharp.Input
{
    /// <summary>
    /// Options to use when typing.
    /// </summary>
    /// <seealso cref="IPage.TypeAsync(string, string, TypeOptions)"/>
    /// <seealso cref="IElementHandle.TypeAsync(string, TypeOptions)"/>
    /// <seealso cref="IKeyboard.TypeAsync(string, TypeOptions)"/>
    public class TypeOptions
    {
        /// <summary>
        /// Time to wait between <c>keydown</c> and <c>keyup</c> in milliseconds. Defaults to 0.
        /// </summary>
        public int? Delay { get; set; }
    }
}
