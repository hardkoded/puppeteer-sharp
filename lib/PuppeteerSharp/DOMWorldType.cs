using System.Runtime.Serialization;
using CefSharp.DevTools.Dom.Helpers.Json;
using Newtonsoft.Json;

namespace CefSharp.DevTools.Dom
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
