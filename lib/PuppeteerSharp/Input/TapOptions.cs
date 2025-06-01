namespace CefSharp.Dom.Input
{
    /// <summary>
    /// Options to use for <see cref="ElementHandle.TapAsync(TapOptions)"/>
    /// </summary>
    public class TapOptions
    {
        /// <summary>
        /// Offset for the clickable point relative to the top-left corner of the border-box.
        /// </summary>
        public Offset? OffSet { get; set; }
    }
}
