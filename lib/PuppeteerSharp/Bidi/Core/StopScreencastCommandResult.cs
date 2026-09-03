#if !CDP_ONLY

using System.Text.Json.Serialization;
using WebDriverBiDi;

namespace PuppeteerSharp.Bidi.Core;

internal sealed record StopScreencastCommandResult : CommandResult
{
    [JsonPropertyName("path")]
    public string Path { get; init; }

    [JsonPropertyName("error")]
    public string Error { get; init; }
}

#endif
