using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal class EvaluateExceptionStackTrace
    {
        [JsonProperty("callFrames")]
        internal EvaluationExceptionCallFrame[] CallFrames { get; set; }
    }
}