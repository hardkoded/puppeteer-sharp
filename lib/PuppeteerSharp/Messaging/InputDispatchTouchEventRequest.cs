using CefSharp.Puppeteer.Input;

namespace CefSharp.Puppeteer.Messaging
{
    internal class InputDispatchTouchEventRequest
    {
        public string Type { get; internal set; }

        public TouchPoint[] TouchPoints { get; set; }

        public int Modifiers { get; internal set; }
    }
}
