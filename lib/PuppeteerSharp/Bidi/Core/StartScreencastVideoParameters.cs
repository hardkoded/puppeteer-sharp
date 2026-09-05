#if !CDP_ONLY

using System.Text.Json.Serialization;

namespace PuppeteerSharp.Bidi.Core;

internal sealed class StartScreencastVideoParameters
{
    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("frameRate")]
    public int? FrameRate { get; set; }
}

#endif
