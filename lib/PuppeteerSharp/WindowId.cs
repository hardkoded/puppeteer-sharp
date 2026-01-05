namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a browser window identifier.
    /// </summary>
    public class WindowId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowId"/> class.
        /// </summary>
        /// <param name="value">The window ID value.</param>
        public WindowId(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the window ID value.
        /// </summary>
        public string Value { get; }

        /// <inheritdoc/>
        public override string ToString() => Value;
    }
}
