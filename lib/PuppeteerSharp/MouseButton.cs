using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PuppeteerSharp
{
    [JsonConverter(typeof(StringEnumConverter), true)]
    public enum MouseButton
    {
        None,
        Left,
        Right,
        Middle
    }
}