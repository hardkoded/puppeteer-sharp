namespace PuppeteerSharp.Messaging
{
    internal class InputDispatchTouchEventRequest
    {
        public string Type { get; internal set; }
        public var TouchPoints { get; internal set; }
        public int Modifiers { get; internal set; }
    }
}
