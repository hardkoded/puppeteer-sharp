#if !CDP_ONLY

using System.Text.Json.Serialization;
using WebDriverBiDi;

namespace PuppeteerSharp.Bidi.Core;

internal sealed class StartScreencastCommandParameters : CommandParameters<StartScreencastCommandResult>
{
    public StartScreencastCommandParameters(string contextId)
    {
        Context = contextId;
    }

    public override string MethodName => "browsingContext.startScreencast";

    [JsonPropertyName("context")]
    public string Context { get; set; }

    [JsonPropertyName("audio")]
    public bool? Audio { get; set; }

    [JsonPropertyName("video")]
    public StartScreencastVideoParameters Video { get; set; }
}

#endif
