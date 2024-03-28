using PuppeteerSharp.PageCoverage;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class ProfilerTakePreciseCoverageResponse
    {
        public ScriptCoverage[] Result { get; set; }
    }
}
