#if !CDP_ONLY

using System.Text.Json.Serialization;
using WebDriverBiDi;

namespace PuppeteerSharp.Bidi.Core;

internal sealed record StartScreencastCommandResult : CommandResult
{
    [JsonPropertyName("screencast")]
    public string Screencast { get; init; }

    [JsonPropertyName("path")]
    public string Path { get; init; }
}

#endif
