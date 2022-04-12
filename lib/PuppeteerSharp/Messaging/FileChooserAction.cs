using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CefSharp.Puppeteer.Messaging
{
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum FileChooserAction
    {
        [EnumMember(Value = "accept")]
        Accept,
        [EnumMember(Value = "fallback")]
        Fallback,
        [EnumMember(Value = "cancel")]
        Cancel
    }
}
