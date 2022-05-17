using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PuppeteerSharp.Input
{
    /// <summary>
    /// The type of button click to use with <see cref="IMouse.DownAsync(ClickOptions)"/>, <see cref="IMouse.UpAsync(ClickOptions)"/> and <see cref="IMouse.ClickAsync(decimal, decimal, ClickOptions)"/>
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), true)]
    public enum MouseButton
    {
        /// <summary>
        /// Non specified
        /// </summary>
        None,

        /// <summary>
        /// The left mouse button
        /// </summary>
        Left,

        /// <summary>
        /// The right mouse button
        /// </summary>
        Right,

        /// <summary>
        /// The middle mouse button
        /// </summary>
        Middle
    }
}
