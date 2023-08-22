using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PuppeteerSharp.Input
{
    /// <summary>
    /// The type of button click to use with <see cref="IMouse.DownAsync(ClickOptions)"/>, <see cref="IMouse.UpAsync(ClickOptions)"/> and <see cref="IMouse.ClickAsync(decimal, decimal, ClickOptions)"/>.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), true)]
    [Flags]
    public enum MouseButton
    {
        /// <summary>
        /// Non specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// The left mouse button.
        /// </summary>
        Left = 1,

        /// <summary>
        /// The right mouse button.
        /// </summary>
        Right = 1 << 1,

        /// <summary>
        /// The middle mouse button.
        /// </summary>
        Middle = 1 << 2,

        /// <summary>
        /// The back mouse button.
        /// </summary>
        Back = 1 << 3,

        /// <summary>
        /// The forward mouse button.
        /// </summary>
        Forward = 1 << 4,
    }
}
