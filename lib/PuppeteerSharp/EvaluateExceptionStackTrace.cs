using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal class EvaluateExceptionStackTrace
    {
        [JsonProperty(Constants.CALL_FRAMES)]
        internal EvaluationExceptionCallFrame[] CallFrames { get; set; }
    }
}