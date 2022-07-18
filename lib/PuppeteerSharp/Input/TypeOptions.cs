namespace CefSharp.DevTools.Dom.Input
{
    /// <summary>
    /// Options to use when typing
    /// </summary>
    /// <seealso cref="IDevToolsContext.TypeAsync(string, string, TypeOptions)"/>
    /// <seealso cref="ElementHandle.TypeAsync(string, TypeOptions)"/>
    /// <seealso cref="Keyboard.TypeAsync(string, TypeOptions)"/>
    public class TypeOptions
    {
        /// <summary>
        /// Time to wait between <c>keydown</c> and <c>keyup</c> in milliseconds. Defaults to 0.
        /// </summary>
        public int? Delay { get; set; }
    }
}
