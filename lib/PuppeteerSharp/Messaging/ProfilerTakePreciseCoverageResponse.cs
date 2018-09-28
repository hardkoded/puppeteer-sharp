using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class ProfilerTakePreciseCoverageResponse
    {
        [JsonProperty(Constants.RESULT)]
        public ProfilerTakePreciseCoverageResponseItem[] Result { get; set; }

        internal class ProfilerTakePreciseCoverageResponseItem
        {
            [JsonProperty(Constants.SCRIPT_ID)]
            public string ScriptId { get; set; }
            [JsonProperty(Constants.FUNCTIONS)]
            public ProfilerTakePreciseCoverageResponseFunction[] Functions { get; set; }
        }

        internal class ProfilerTakePreciseCoverageResponseFunction
        {
            [JsonProperty(Constants.RANGES)]
            public CoverageResponseRange[] Ranges { get; set; }
        }
    }
}
