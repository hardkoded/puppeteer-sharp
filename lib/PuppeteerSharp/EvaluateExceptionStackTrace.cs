using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class EvaluateExceptionStackTrace
    {
        public IEnumerable<EvaluationExceptionCallFrame> CallFrames { get; internal set; }
    }
}