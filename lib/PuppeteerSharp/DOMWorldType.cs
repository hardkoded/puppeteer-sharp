using System.Runtime.Serialization;
using CefSharp.Dom.Helpers.Json;
using Newtonsoft.Json;

namespace CefSharp.Dom
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
