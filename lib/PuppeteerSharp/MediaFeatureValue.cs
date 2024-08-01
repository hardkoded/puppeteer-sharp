using System.Text.Json.Serialization;

namespace PuppeteerSharp
{
    /// <summary>
    /// Media Feature. <see cref="IPage.EmulateMediaFeaturesAsync(System.Collections.Generic.IEnumerable{MediaFeatureValue})"/>.
    /// </summary>
    public class MediaFeatureValue
    {
        /// <summary>
        /// The CSS media feature name. Supported names are `'prefers-colors-scheme'` and `'prefers-reduced-motion'`.
        /// </summary>
        [JsonPropertyName("name")]
        public MediaFeature MediaFeature { get; set; }

        /// <summary>
        /// The value for the given CSS media feature.
        /// </summary>
        public string Value { get; set; }
    }
}
