using PuppeteerSharp.Input;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class InputDispatchTouchEventRequest
    {
        public string Type { get; internal set; }

        public TouchPoint[] TouchPoints { get; set; }

        public int Modifiers { get; internal set; }
    }
}
