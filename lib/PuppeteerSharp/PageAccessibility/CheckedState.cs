namespace PuppeteerSharp.PageAccessibility
{
    /// <summary>
    /// Three-state boolean. See <seealso cref="SerializedAXNode.Checked"/> and <seealso cref="SerializedAXNode.Pressed"/>
    /// </summary>
    public enum CheckedState
    {
        /// <summary>
        /// Flse.
        /// </summary>
        False = 0,
        /// <summary>
        /// True.
        /// </summary>
        True,
        /// <summary>
        /// Mixed.
        /// </summary>
        Mixed
    }
}