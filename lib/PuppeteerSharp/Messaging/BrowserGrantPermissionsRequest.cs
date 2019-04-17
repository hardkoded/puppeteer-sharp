
namespace PuppeteerSharp.Messaging
{
    internal class BrowserGrantPermissionsRequest
    {
        public string Origin { get; set; }
        public string BrowserContextId { get; set; }
        public Abstractions.OverridePermission[] Permissions { get; set; }
    }
}
