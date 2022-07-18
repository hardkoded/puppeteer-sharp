namespace CefSharp.DevTools.Dom.Input
{
    /// <summary>
    /// options to use when pressing a key.
    /// </summary>
    /// <seealso cref="Keyboard.PressAsync(string, PressOptions)"/>
    /// <seealso cref="ElementHandle.PressAsync(string, PressOptions)"/>
    public class PressOptions : DownOptions
    {
        /// <summary>
        /// Time to wait between <c>keydown</c> and <c>keyup</c> in milliseconds. Defaults to 0.
        /// </summary>
        public int? Delay { get; set; }

        /// <summary>
        /// Create a <see cref="PressOptions"/> instance with the specified <paramref name="delay"/>
        /// </summary>
        /// <param name="delay">time to wait in milliseconds</param>
        /// <returns>PressOptions</returns>
        public static PressOptions WithDelay(int delay)
        {
            return new PressOptions { Delay = delay };
        }
    }
}
