using System.Collections.Generic;

namespace PuppeteerSharp.Messaging
{
    internal class PageHandleFileChooserRequest
    {
        public FileChooserAction Action { get; set; }

        public IEnumerable<string> Files { get; set; }
    }
}
