using CefSharp.DevTools.Profiler;

namespace CefSharp.Dom.PageCoverage
{
    /// <summary>
    /// The CoverageEntry class for JavaScript.
    /// </summary>
    public class JSCoverageEntry : CoverageEntry
    {
        /// <summary>
        /// Raw V8 script coverage entry.
        /// </summary>
        public ScriptCoverage RawScriptCoverage { get; set; }
    }
}
