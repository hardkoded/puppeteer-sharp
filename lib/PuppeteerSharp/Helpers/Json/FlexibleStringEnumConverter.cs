using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PuppeteerSharp.Helpers.Json
{
    internal class FlexibleStringEnumConverter : StringEnumConverter
    {
        private readonly Enum _fallbackValue;

        public FlexibleStringEnumConverter(Enum fallbackValue) => _fallbackValue = fallbackValue;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch
            {
                return _fallbackValue;
            }
        }
    }
}
