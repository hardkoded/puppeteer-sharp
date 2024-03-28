namespace PuppeteerSharp.Cdp.Messaging
{
    internal class AccessibilityQueryAXTreeRequest
    {
        public string ObjectId { get; set; }

        public string AccessibleName { get; set; }

        public string Role { get; set; }
    }
}
