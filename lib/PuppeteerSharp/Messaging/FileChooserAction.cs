using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PuppeteerSharp.Messaging
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
