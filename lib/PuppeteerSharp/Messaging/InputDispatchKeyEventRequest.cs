using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class InputDispatchKeyEventRequest
    {
        public DispatchKeyEventType Type { get; set; }
        public int Modifiers { get; set; }
        public int WindowsVirtualKeyCode { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string UnmodifiedText { get; set; }
        public bool AutoRepeat { get; set; }
        public int Location { get; set; }
        public bool IsKeypad { get; set; }
    }
}
