namespace PuppeteerSharp.Input
{
    internal class KeyDefinition
    {
        internal int KeyCode { get; set; }

        internal int? ShiftKeyCode { get; set; }

        internal string Key { get; set; }

        internal string ShiftKey { get; set; }

        internal string Code { get; set; }

        internal string Text { get; set; }

        internal string ShiftText { get; set; }

        internal int Location { get; set; }
    }
}
