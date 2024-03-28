namespace PuppeteerSharp.Cdp.Messaging
{
    internal class InputDispatchKeyEventRequest
    {
        public DispatchKeyEventType Type { get; set; }

        public int Modifiers { get; set; }

        public int WindowsVirtualKeyCode { get; set; }

        public string Code { get; set; }

        public string Key { get; set; }

        public string Text { get; set; }

        public string UnmodifiedText { get; set; }

        public bool AutoRepeat { get; set; }

        public int Location { get; set; }

        public bool IsKeypad { get; set; }
    }
}
