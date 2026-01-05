using System;

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
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Window ID cannot be null or empty.", nameof(value));
            }

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
