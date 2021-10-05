using Newtonsoft.Json;

namespace CefSharp.Puppeteer
{
    internal class ContextPayloadAuxData
    {
        public string FrameId { get; set; }

        public bool IsDefault { get; set; }

        public DOMWorldType Type { get; set; }
    }
}
