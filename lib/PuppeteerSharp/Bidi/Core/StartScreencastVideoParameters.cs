#if !CDP_ONLY

using System.Text.Json.Serialization;

namespace PuppeteerSharp.Bidi.Core;

internal sealed class StartScreencastVideoParameters
{
    [JsonPropertyName("width")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Width { get; set; }

    [JsonPropertyName("height")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Height { get; set; }

    [JsonPropertyName("frameRate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? FrameRate { get; set; }
}

#endif
