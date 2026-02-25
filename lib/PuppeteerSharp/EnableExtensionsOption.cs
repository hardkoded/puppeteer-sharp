namespace PuppeteerSharp
{
    /// <summary>
    /// Represents the value for the EnableExtensions launch option.
    /// Can be implicitly converted from a <see cref="bool"/> or a <see cref="string"/> array.
    /// </summary>
    public class EnableExtensionsOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnableExtensionsOption"/> class with a boolean value.
        /// </summary>
        /// <param name="enabled">Whether extensions are enabled.</param>
        public EnableExtensionsOption(bool enabled)
        {
            Enabled = enabled;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableExtensionsOption"/> class with an array of extension paths.
        /// </summary>
        /// <param name="paths">The paths to unpacked extensions to load.</param>
        public EnableExtensionsOption(string[] paths)
        {
            Enabled = paths is { Length: > 0 };
            Paths = paths;
        }

        /// <summary>
        /// Gets a value indicating whether extensions should be enabled.
        /// </summary>
        public bool Enabled { get; }

        /// <summary>
        /// Gets the paths to unpacked extensions to load.
        /// When <c>null</c>, no specific extensions are loaded, but extensions may still be enabled
        /// if <see cref="Enabled"/> is <c>true</c>.
        /// </summary>
        public string[] Paths { get; }

        /// <summary>
        /// Implicitly converts a <see cref="bool"/> to an <see cref="EnableExtensionsOption"/>.
        /// </summary>
        /// <param name="enabled">The boolean value.</param>
        public static implicit operator EnableExtensionsOption(bool enabled) => new(enabled);

        /// <summary>
        /// Implicitly converts a <see cref="string"/> array to an <see cref="EnableExtensionsOption"/>.
        /// </summary>
        /// <param name="paths">The array of extension paths.</param>
        public static implicit operator EnableExtensionsOption(string[] paths) => new(paths);
    }
}
