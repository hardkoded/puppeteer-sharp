using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// Media Feature. See <see cref="IPage.EmulateMediaFeaturesAsync(System.Collections.Generic.IEnumerable{MediaFeatureValue})"/>.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter<MediaFeature>))]
    public enum MediaFeature
    {
        /// <summary>
        /// prefers-color-scheme media feature.
        /// </summary>
        [EnumMember(Value = "prefers-color-scheme")]
        PrefersColorScheme,

        /// <summary>
        /// prefers-reduced-motion media feature.
        /// </summary>
        [EnumMember(Value = "prefers-reduced-motion")]
        PrefersReducedMotion,

        /// <summary>
        /// prefers-contrast media feature.
        /// </summary>
        [EnumMember(Value = "prefers-contrast")]
        PrefersContrast,

        /// <summary>
        /// prefers-reduced-transparency media feature.
        /// </summary>
        [EnumMember(Value = "prefers-reduced-transparency")]
        PrefersReducedTransparency,

        /// <summary>
        /// forced-colors media feature.
        /// </summary>
        [EnumMember(Value = "forced-colors")]
        ForcedColors,
    }
}
