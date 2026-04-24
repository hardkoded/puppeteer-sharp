namespace PuppeteerSharp.Cdp.Messaging
{
    internal class TargetSetAutoAttachRequest
    {
        public bool AutoAttach { get; set; }

        public bool WaitForDebuggerOnStart { get; set; }

        public bool Flatten { get; set; }

        public AutoAttachFilter[] Filter { get; set; }

        internal class AutoAttachFilter
        {
            public string Type { get; set; }

            public bool? Exclude { get; set; }
        }
    }
}
