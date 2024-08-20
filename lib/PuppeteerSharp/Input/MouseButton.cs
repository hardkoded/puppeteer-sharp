using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Input
{
    /// <summary>
    /// The type of button click to use with <see cref="IMouse.DownAsync(ClickOptions)"/>, <see cref="IMouse.UpAsync(ClickOptions)"/> and <see cref="IMouse.ClickAsync(decimal, decimal, ClickOptions)"/>.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter<MouseButton>))]
    [Flags]
#pragma warning disable CA1714 // Flags enums should have plural names. We don't want to break compatibility for this
    public enum MouseButton
#pragma warning restore CA1714 // Flags enums should have plural names
    {
        /// <summary>
        /// Non specified.
        /// </summary>
        [EnumMember(Value = "none")]
        None = 0,

        /// <summary>
        /// The left mouse button.
        /// </summary>
        [EnumMember(Value = "left")]
        Left = 1,

        /// <summary>
        /// The right mouse button.
        /// </summary>
        [EnumMember(Value = "right")]
        Right = 1 << 1,

        /// <summary>
        /// The middle mouse button.
        /// </summary>
        [EnumMember(Value = "middle")]
        Middle = 1 << 2,

        /// <summary>
        /// The back mouse button.
        /// </summary>
        [EnumMember(Value = "back")]
        Back = 1 << 3,

        /// <summary>
        /// The forward mouse button.
        /// </summary>
        [EnumMember(Value = "forward")]
        Forward = 1 << 4,
    }
}
