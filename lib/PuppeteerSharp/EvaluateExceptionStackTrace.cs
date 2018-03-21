using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp
{
    public class EvaluateExceptionStackTrace
    {
        [JsonProperty("callFrames")]
        public IEnumerable<EvaluationExceptionCallFrame> CallFrames { get; internal set; }
    }
}