using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// Window state for <see cref="WindowBounds"/>.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter<WindowState>))]
    public enum WindowState
    {
        /// <summary>
        /// Normal window state.
        /// </summary>
        [EnumMember(Value = "normal")]
        Normal,

        /// <summary>
        /// Minimized window state.
        /// </summary>
        [EnumMember(Value = "minimized")]
        Minimized,

        /// <summary>
        /// Maximized window state.
        /// </summary>
        [EnumMember(Value = "maximized")]
        Maximized,

        /// <summary>
        /// Fullscreen window state.
        /// </summary>
        [EnumMember(Value = "fullscreen")]
        Fullscreen,
    }
}
