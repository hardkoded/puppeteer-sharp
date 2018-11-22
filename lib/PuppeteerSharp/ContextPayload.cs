using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal class ContextPayload
    {
        public int Id { get; set; }
        public ContextPayloadAuxData AuxData { get; set; }
    }
}