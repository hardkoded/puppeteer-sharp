using CefSharp.DevTools.Dom.Input;

namespace CefSharp.DevTools.Dom.Messaging
{
    internal class InputDispatchTouchEventRequest
    {
        public string Type { get; internal set; }

        public TouchPoint[] TouchPoints { get; set; }

        public int Modifiers { get; internal set; }
    }
}
