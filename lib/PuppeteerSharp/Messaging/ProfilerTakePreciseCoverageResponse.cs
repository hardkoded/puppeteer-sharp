using System;
using System.Collections.Generic;
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
            public ProfilerTakePreciseCoverageResponseRange[] Ranges { get; set; }
        }

        internal class ProfilerTakePreciseCoverageResponseRange
        {
            [JsonProperty("startOffset")]
            public int StartOffset { get; set; }
            [JsonProperty("endOffset")]
            public int EndOffset { get; set; }
            [JsonProperty("count")]
            public int Count { get; set; }
        }
    }
}
