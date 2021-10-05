using System.Runtime.Serialization;
using CefSharp.Puppeteer.Helpers.Json;
using Newtonsoft.Json;

namespace CefSharp.Puppeteer
{
    [JsonConverter(typeof(FlexibleStringEnumConverter), Other)]
    internal enum DOMWorldType
    {
        Other,
        [EnumMember(Value = "isolated")]
        Isolated,
        [EnumMember(Value = "default")]
        Default
    }
}
