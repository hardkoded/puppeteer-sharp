using System.Text.Json;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class AuditsInspectorIssue
    {
        public string Code { get; set; }

        public JsonElement Details { get; set; }
    }
}
