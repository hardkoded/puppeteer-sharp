using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    [JsonConverter(typeof(StringEnumConverter), true)]
    public enum MediaType
    {
        Print,

        Screen,

        [EnumMember(Value = "")]
        None
    }
}
