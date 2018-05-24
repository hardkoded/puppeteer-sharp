using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    public class ProfilerTakePreciseCoverageResponse
    {
        [JsonProperty("result")]
        public ProfilerTakePreciseCoverageResponseItem[] Result { get; set; }
    }

    public class ProfilerTakePreciseCoverageResponseItem
    {
        [JsonProperty("scriptId")]
        public string ScriptId { get; set; }
        [JsonProperty("functions")]
        public ProfilerTakePreciseCoverageResponseFunction[] Functions { get; set; }
    }

    public class ProfilerTakePreciseCoverageResponseFunction
    {
        [JsonProperty("ranges")]
        public ProfilerTakePreciseCoverageResponseRange[] Ranges { get; set; }
    }

    public class ProfilerTakePreciseCoverageResponseRange
    {
        [JsonProperty("startOffset")]
        public int StartOffset { get; set; }
        [JsonProperty("endOffset")]
        public int EndOffset { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
