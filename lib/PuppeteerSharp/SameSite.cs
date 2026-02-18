using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// SameSite values in cookies.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter<SameSite>))]
    [DefaultEnumValue((int)None)]
    public enum SameSite
    {
        /// <summary>
        /// None.
        /// </summary>
        None,

        /// <summary>
        /// Strict.
        /// </summary>
        Strict,

        /// <summary>
        /// Lax.
        /// </summary>
        Lax,

        /// <summary>
        /// Extended.
        /// </summary>
        Extended,

        /// <summary>
        /// Default. The browser applies its default SameSite behavior.
        /// </summary>
        Default,
    }
}
