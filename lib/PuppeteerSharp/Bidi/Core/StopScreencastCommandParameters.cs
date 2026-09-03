#if !CDP_ONLY

using System.Text.Json.Serialization;
using WebDriverBiDi;

namespace PuppeteerSharp.Bidi.Core;

internal sealed class StopScreencastCommandParameters : CommandParameters<StopScreencastCommandResult>
{
    public StopScreencastCommandParameters(string screencast)
    {
        Screencast = screencast;
    }

    public override string MethodName => "browsingContext.stopScreencast";

    [JsonPropertyName("screencast")]
    public string Screencast { get; set; }
}

#endif
