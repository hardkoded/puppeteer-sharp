using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter<InitializationStatus>))]
    internal enum InitializationStatus
    {
        Aborted,
        Success,
    }
}
