﻿using PuppeteerSharp.Input;

namespace PuppeteerSharp.Messaging
{
    internal class InputDispatchMouseEventRequest
    {
        public MouseEventType Type { get; set; }
        public MouseButton Button { get; set; }
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public int Modifiers { get; set; }
        public int ClickCount { get; set; }
        public decimal DeltaX { get; set; }
        public decimal DeltaY { get; set; }
    }
}