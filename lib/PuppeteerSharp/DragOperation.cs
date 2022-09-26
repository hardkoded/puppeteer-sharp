namespace PuppeteerSharp
{
    /// <summary>
    /// Drag operations available on <see cref="DragData"/>.
    /// </summary>
    public enum DragOperation
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Copy.
        /// </summary>
        Copy = 1,

        /// <summary>
        /// Link.
        /// </summary>
        Link = 2,

        /// <summary>
        /// Move.
        /// </summary>
        Move = 16,
    }
}
