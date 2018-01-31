using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp.Helpers
{
    public static class DynamicHelpers
    {
        public static T ToStatic<T>(this JObject obj)
        {
            return JsonConvert.DeserializeObject<T>(obj.ToString());
        }
    }
}
