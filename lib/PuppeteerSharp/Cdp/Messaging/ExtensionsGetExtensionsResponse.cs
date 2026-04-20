using System.Collections.Generic;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class ExtensionsGetExtensionsResponse
    {
        public List<ExtensionInfo> Extensions { get; set; } = new();
    }
}
