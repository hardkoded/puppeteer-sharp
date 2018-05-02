using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp
{
    public class PageErrorPreview
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("Properties")]
        public List<PageErrorExceptionProperty> Properties { get; set; }
    }
}