using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class ProfilerTakePreciseCoverageResponse
    {
        [JsonProperty("result")]
        public ProfilerTakePreciseCoverageResponseItem[] Result { get; set; }

        internal class ProfilerTakePreciseCoverageResponseItem
        {
            [JsonProperty("scriptId")]
            public string ScriptId { get; set; }
            [JsonProperty("functions")]
            public ProfilerTakePreciseCoverageResponseFunction[] Functions { get; set; }
        }

        internal class ProfilerTakePreciseCoverageResponseFunction
        {
            [JsonProperty("ranges")]
            public CoverageResponseRange[] Ranges { get; set; }
        }
    }
}
