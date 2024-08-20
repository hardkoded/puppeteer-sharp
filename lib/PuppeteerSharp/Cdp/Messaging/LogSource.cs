using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Cdp.Messaging;

[JsonConverter(typeof(JsonStringEnumMemberConverter<LogSource>))]
internal enum LogSource
{
    Xml = 0,
    Javascript,
    Network,
    Storage,
    Appcache,
    Rendering,
    Security,
    Deprecation,
    Worker,
    Violation,
    Intervention,
    Recommendation,
    Other,
}
