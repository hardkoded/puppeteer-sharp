using Newtonsoft.Json;

namespace CefSharp.DevTools.Dom
{
    internal class ContextPayloadAuxData
    {
        public string FrameId { get; set; }

        public bool IsDefault { get; set; }

        public DOMWorldType Type { get; set; }
    }
}
